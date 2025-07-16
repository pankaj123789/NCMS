using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class AccountingProcessingService
    {
        private readonly ExternalAccountingQueueService _queueService;
        private readonly List<string> _wiiseErrorsThatNeedToBeRetried;


        public AccountingProcessingService(ExternalAccountingQueueService queueService)
        {
            _queueService = queueService;
            var wiiseErrorsThatNeedToBeRetriedValue = (NHibernateSession.Current.Query<SystemValue>().FirstOrDefault(x => x.ValueKey == "WiiseErrorsThatNeedToBeRetried")).Value;
            _wiiseErrorsThatNeedToBeRetried = wiiseErrorsThatNeedToBeRetriedValue.Split('|').ToList();
        }

        private ExternalAccountingOperationResult<TResult> PerformOperation<TResult>(ExternalAccountingOperation operationRequest, int userId)
            where TResult : class
        {
            var result = new ExternalAccountingOperationResult<TResult> { OperationId = operationRequest.Id };

            var operation = _queueService.GetOperation(operationRequest);

            var status = (ExternalAccountingOperationStatusName)operationRequest.Status.Id;
            if (!(status == ExternalAccountingOperationStatusName.Requested || status == ExternalAccountingOperationStatusName.Failed))
            {
                throw new Exception($"Cannot perform this Wiise operation as the operation status is already '{operationRequest.Status.DisplayName}'.");
            }

            // update the operation request status in the queue
            operationRequest.ProcessedDateTime = DateTime.Now;
            _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.InProgress, userId);



            try
            {
                WiiseExecutor.Execute((svc, token) => operation.PerformOperation(svc, token));

                result.Result = operation.Result as TResult;
                result.Success = true;
                var operationStatus = ExternalAccountingOperationStatusName.Successful;

                operationRequest.Output = operation.Output;
                operationRequest.Message = null;

                if (Type.GetType(operationRequest.CompletionType) == typeof(CreateApplicationPaymentCompletionOperation))
                {
                    //payment operation
                    var payments = (Payments)operation.Result;
                    operationRequest.PrerequisiteOperation.Output = payments._Payments.FirstOrDefault()?.InvoiceNumber;
                    LoggingHelper.LogInfo($"invoice operation { operationRequest.PrerequisiteOperation?.Id } has final output value of { operationRequest.PrerequisiteOperation.Output }");
                    operationRequest.Output = payments._Payments.FirstOrDefault()?.Reference;
                }
                if (Type.GetType(operationRequest.CompletionType) == typeof(CreateApplicationRefundPaymentCompletionOperation))
                {
                    var payments = (Payments)operation.Result;
                    operationRequest.PrerequisiteOperation.Output = payments._Payments.FirstOrDefault()?.CreditNoteNumber;
                    LoggingHelper.LogInfo($"invoice operation { operationRequest.Id } has final output value of { operationRequest.PrerequisiteOperation.Output }");
                    operationRequest.Output = payments._Payments.FirstOrDefault()?.Reference;
                }
                if (Type.GetType(operationRequest.CompletionType) == typeof(CreateApplicationInvoiceCompletionOperation))
                {
                    if(operation.Result.GetType() == typeof(Invoice))
                    {
                        var invoice = (Invoice)operation.Result;
                        if (invoice.HasValidationErrors || (invoice.HasErrors.HasValue && invoice.HasErrors.Value))
                        {
                            result.Success = false;
                            operationRequest.Message = invoice.ValidationErrors.First().Message;
                            operationStatus = ExternalAccountingOperationStatusName.Failed;
                            throw new Exception($"Create Invoice failed. Details: {operationRequest.Message} ");
                        }
                    }
                }
                _queueService.SetOperationStatusAndSave(operationRequest, operationStatus, userId);
            }
            catch (Exception ex)
            {
                operationRequest.Message = WiiseExceptionHelper.GetMessage(ex);
                operationRequest.Exception = JsonConvert.SerializeObject(ex);
                _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.Failed, userId);

                result.ErrorMessage = operationRequest.Message;
                result.Exception = ex;
                result.Success = false;
            }

            if (result.Success)
            {
                try
                {
                    _queueService.GetCompletionOperation(operationRequest)?.PerformOperation(operation.Result);
                }
                catch (Exception ex)
                {
                    result.WarningMessage = $"The Wiise operation succeeded, but an error occurred during post-Wiise processing. {WiiseExceptionHelper.GetMessage(ex)} (Operation ID: {operationRequest.Id}).";
                    result.Exception = ex;
                }
            }

            result.Status = (ExternalAccountingOperationStatusName)operationRequest.Status.Id;
            return result;
        }

        private ExternalAccountingOperationResult<T> PerformOperationAndPrerequisites<T>(ExternalAccountingOperation operationRequest, int userId)
            where T : class
        {
            var operations = new Stack<ExternalAccountingOperation>();
            var current = operationRequest;
            while (current != null)
            {
                operations.Push(current);
                current = current.PrerequisiteOperation;
            }

            ExternalAccountingOperationResult<T> result = null;

            while (operations.Any())
            {
                current = operations.Pop();
                var status = (ExternalAccountingOperationStatusName)current.Status.Id;

                // don't perform prerequisites that have already run successfully or are in progress
                var skip = operations.Any() &&
                           status != ExternalAccountingOperationStatusName.Requested &&
                           status != ExternalAccountingOperationStatusName.Failed;
                if (skip)
                {
                    continue;
                }

                result = PerformOperation<T>(current, userId);

                if (result.Success)
                {
                    continue;
                }

                if (operations.Any())
                {
                    result.ErrorMessage = "A prerequisite operation failed. " + result.ErrorMessage;
                }

                break;
            }

            return result;
        }

        public ExternalAccountingOperationResult<TResult> CreateObject<TRequest, TOperation, TResult>(ExternalAccountingOperationTypeName requestType, TRequest request,
            string reference, WiiseCompletionOperation completionOp, string description, int userId, bool batchProcess, bool deferred)
            where TRequest : WiiseOperationRequest
            where TOperation : WiiseOperation<TRequest, TResult>, new()
            where TResult : class
        {
            var operation = new TOperation
            {
                Request = request,
            };
            var operationRequest = _queueService.QueueOperation(requestType, operation, reference, completionOp, description, userId, request.PrerequisiteRequestId, batchProcess);

            return CreateObject<TResult>(operationRequest, userId, batchProcess, deferred);
        }

        public ExternalAccountingOperationResult<TResult> CreateObject<TResult>(ExternalAccountingOperation operationRequest, int userId, bool batchProcess, bool deferred)
            where TResult : class
        {
            // to be used if operation is deferred or exception occurs while performing it
            var result = new ExternalAccountingOperationResult<TResult> { OperationId = operationRequest.Id };

            if (!batchProcess && !deferred)
            {
                // once the operation is queued, we have to return something no matter what happens
                try
                {
                    result = PerformOperationAndPrerequisites<TResult>(operationRequest, userId);
                }
                catch (Exception ex)
                {
                    // only for exceptions we don't handle elsewhere
                    result.ErrorMessage = "The Wiise operation failed. An error has been logged.";
                    result.Exception = ex;
                    result.Success = false;
                }
            }
            else
            {
                result.Success = true;
            }

            result.Status = (ExternalAccountingOperationStatusName)operationRequest.Status.Id;
            return result;
        }

        public ExternalAccountingOperationResult<Invoice> CreateInvoice(WiiseCreateInvoiceRequest request, WiiseCompletionOperation invoiceCompletionOperation, string description, int userId, bool batchProcess)
        {
            if (String.IsNullOrEmpty(description))
            {
                description = "Invoice creation; " + request.Reference;
            }

            return CreateObject<WiiseCreateInvoiceRequest, WiiseInvoiceCreationOperation, Invoice>(ExternalAccountingOperationTypeName.CreateInvoice, request,
                request.Reference, invoiceCompletionOperation, description, userId, batchProcess, false);
        }

        public ExternalAccountingOperationResult<CreditNote> CreateCreditNote(WiiseCreateCreditNoteRequest request, WiiseCompletionOperation invoiceCompletionOperation, string description, int userId, bool batchProcess)
        {
            if (String.IsNullOrEmpty(description))
            {
                description = "Credit Note creation; " + request.Reference;
            }

            return CreateObject<WiiseCreateCreditNoteRequest, WiiseCreditNoteCreationOperation, CreditNote>(ExternalAccountingOperationTypeName.CreateCreditNote, request,
                request.Reference, invoiceCompletionOperation, description, userId, batchProcess, false);
        }

        public ExternalAccountingOperationResult<Invoice> CreateInvoice(int operationId, int userId, bool batchProcess)
        {
            var operationRequest = _queueService.GetOperationRequest(operationId);
            if (operationRequest == null)
            {
                throw new WebServiceException("The specified Invoice Creation request does not exist in the queue.");
            }

            if (!(operationRequest.Status.Name == ExternalAccountingOperationStatusName.InProgress.ToString()
                || operationRequest.Status.Name == ExternalAccountingOperationStatusName.Failed.ToString()))
            {
                throw new WebServiceException($"Cannot process this Invoice Creation request as it is marked as '{operationRequest.Status.DisplayName}'.");
            }

            return CreateObject<Invoice>(operationRequest, userId, batchProcess, false);
        }

        public ExternalAccountingOperationResult<Contact> CreateContact(WiiseCreateContactRequest request, int userId, bool batchProcess)
        {
            return CreateObject<WiiseCreateContactRequest, WiiseContactCreationOperation, Contact>(ExternalAccountingOperationTypeName.CreateContact, request,
                request.AccountNumber, null, $"Contact creation; NAATI #{request.AccountNumber}", userId, batchProcess, false);
        }

        public ExternalAccountingOperationResult<Payments> CreatePayment(WiiseCreatePaymentRequest request, CreatePaymentCompletionOperation paymentCompletionOperation, int userId, bool batchProcess, bool deferred)
        {
            return CreateObject<WiiseCreatePaymentRequest, WiisePaymentCreationOperation, Payments>(ExternalAccountingOperationTypeName.CreatePayment, request,
                request.Reference, paymentCompletionOperation, $"Payment(s) creation; {request.Reference}", userId, batchProcess, deferred);
        }

        public ExternalAccountingOperationResult<IEnumerable<Payment>> CreatePayment(int operationId, int userId, bool batchProcess)
        {
            var operationRequest = _queueService.GetOperationRequest(operationId);

            if (operationRequest == null)
            {
                throw new WebServiceException("The specified Payment Creation request does not exist in the queue.");
            }

            if (operationRequest.Status.Name == ExternalAccountingOperationStatusName.InProgress.ToString())
            {
                throw new WebServiceException(
                    $"Cannot process this Payment Creation request as it is marked as '{operationRequest.Status.DisplayName}'.");
            }

            // if the operation has already succeeded somehow, just return success
            if (operationRequest.Status.Name == ExternalAccountingOperationStatusName.Successful.ToString())
            {
                return new ExternalAccountingOperationResult<IEnumerable<Payment>>
                {
                    OperationId = operationId,
                    Status = ExternalAccountingOperationStatusName.Successful,
                    Success = true
                    // it would be a lot of effort to get the actual Payment (Reference isn't unique, so we also need the Invoice ID), so there's no point doing that as it currently isn't needed by SAM
                };
            }

            return CreateObject<IEnumerable<Payment>>(operationRequest, userId, batchProcess, false);
        }

        private IList<FinanceBatchInvoiceOperationResult> ProcessInvoiceBatch(IList<ExternalAccountingOperation> invoiceOperations, int userId)
        {
            LoggingHelper.LogDebug("Processing invoice batch");

            var result = new List<FinanceBatchInvoiceOperationResult>();

            var invoiceCreationOperations = invoiceOperations
                .Select(_queueService.GetOperation<WiiseInvoiceCreationOperation>);

            var models = invoiceCreationOperations
                .Select(x =>
                {
                    var item = x.GetPreparedInput();
                    item.OperationId = x.OperationId;
                    return item;
                })
                .ToList();

            List<Invoice> wiiseResults;

            invoiceOperations.ForEach(x =>
            {
                x.ProcessedDateTime = DateTime.Now;
                _queueService.SetOperationStatusAndSave(x, ExternalAccountingOperationStatusName.InProgress, userId);
            });

            try
            {
                var invoices = new Invoices
                {
                    _Invoices = models
                };
                var results = WiiseExecutor.Execute((svc, token) => svc.Api.CreateInvoicesAsync(token.Value, token.Tenant, invoices));
                wiiseResults = results.Data._Invoices;

            }
            catch (ApiException ex)
            {
                HandleApiException(invoiceOperations, userId, ex);
                throw ex;
            }
            catch (Exception ex)
            {
                HandleException(invoiceOperations, userId, ex);
                throw ex;
            }

            for (int i = 0; i < wiiseResults.Count; i++)
            {
                var thisResult = new FinanceBatchInvoiceOperationResult();

                var wiiseResult = wiiseResults[i];
                var operationRequest = invoiceOperations[i];
                var completion = _queueService.GetCompletionOperation(operationRequest) as CreateApplicationInvoiceCompletionOperation;
                completion.NotNull("No completion operation on Invoice operation");

                thisResult.CredentialWorkflowFeeId = completion.CredentialWorkflowFeeId;
                thisResult.InvoiceReference = operationRequest.Reference;

                if (!wiiseResult.HasValidationErrors)
                {
                    try
                    {
                        thisResult.Success = true;
                        thisResult.WarningMessage = wiiseResult.Warnings == null ? "" : String.Join("; ", wiiseResult.Warnings.Select(x => x.Message));
                        thisResult.InvoiceNumber = wiiseResult.InvoiceNumber;
                        operationRequest.Message = thisResult.WarningMessage;
                        operationRequest.Output = wiiseResult.InvoiceNumber;
                        _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.Successful, userId);
                        completion.PerformOperation(wiiseResult);
                    }
                    catch (Exception ex)
                    {
                        thisResult.WarningMessage =
                            $"The Wiise operation succeeded, but an error occurred during post-Wiise processing. {WiiseExceptionHelper.GetMessage(ex)} (Operation ID: {operationRequest.Id}).";
                    }
                }
                else
                {
                    HandleErrorResponse(userId, thisResult, wiiseResult, operationRequest);
                    LoggingHelper.LogError("Wiise invoice creation error: {Message}}", operationRequest.Message);
                }
                result.Add(thisResult);
            }
            return result;
        }

        private IList<FinanceBatchPaymentOperationResult> ProcessPaymentBatch(IList<ExternalAccountingOperation> paymentOperations, int userId)
        {
            LoggingHelper.LogDebug("Processing payment batch");

            var result = new List<FinanceBatchPaymentOperationResult>();
            var models = paymentOperations
                .Select(_queueService.GetOperation<WiisePaymentCreationOperation>)
                .Select(x =>
                    {
                        var item = x.GetPreparedInput()
                          .Requires(y => y._Payments.Count() == 1, "Cannot batch a payment creation request containing multiple payments")
                          ._Payments
                          .Single();
                        item.OperationId = x.OperationId;
                        return item; 
                    })
                .ToList();

            var wiiseResults = new List<Common.Wiise.PublicModels.Payment>();

            paymentOperations.ForEach(x =>
            {
                x.ProcessedDateTime = DateTime.Now;
                _queueService.SetOperationStatusAndSave(x, ExternalAccountingOperationStatusName.InProgress, userId);
            });

            try
            {
                var payments = new Common.Wiise.PublicModels.Payments
                {
                    _Payments = models
                };

                var journalNumber = WiiseExtensions.WiiseJournalNumber();
                var results = WiiseExecutor.Execute((svc, token) => svc.Api.CreatePaymentsAsync(token.Value, token.Tenant, payments, journalNumber));
                if (results.StatusCode.IsStatusCodeAnError())
                {
                    LoggingHelper.LogError($"Error in CreatepaymentsAsync {journalNumber}");
                }
                wiiseResults = results.Data._Payments;
            }
            catch (ApiException ex)
            {
                HandleApiException(paymentOperations, userId, ex);
                throw ex;
            }
            catch (Exception ex)
            {
                HandleException(paymentOperations, userId, ex);
                throw ex;
            }

            for (int i = 0; i < wiiseResults.Count; i++)
            {
                var thisResult = new FinanceBatchPaymentOperationResult();

                var wiiseResult = wiiseResults[i];
                var operationRequest = paymentOperations[i];

                if (operationRequest.CompletionType == typeof(CreateApplicationPaymentCompletionOperation).FullName)
                {
                    var completion = _queueService.GetCompletionOperation(operationRequest) as CreateApplicationPaymentCompletionOperation;
                    completion.NotNull("No completion operation on Payment operation");

                    thisResult.CredentialWorkflowFeeId = completion.CredentialWorkflowFeeId;

                    if (!wiiseResult.HasValidationErrors)
                    {
                        try
                        {
                            thisResult.Success = true;
                            //thisResult.InvoiceReference = wiiseResult.Invoice.Reference;
                            thisResult.PaymentReference = wiiseResult.Reference;
                            thisResult.InvoiceNumber = wiiseResult.InvoiceNumber;

                            operationRequest.Message = thisResult.WarningMessage;
                            operationRequest.Output = wiiseResult.Reference;

                            if (!string.IsNullOrEmpty(wiiseResult.InvoiceNumber))
                                operationRequest.PrerequisiteOperation.Output = wiiseResult.InvoiceNumber;

                            LoggingHelper.LogInfo($"invoice operation {operationRequest.PrerequisiteOperation.Id} has final output value of {wiiseResult.InvoiceNumber}");

                            _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.Successful, userId);

                            completion.PerformOperation(wiiseResult);
                        }
                        catch (Exception ex)
                        {
                            thisResult.WarningMessage =
                                $"The Wiise operation succeeded, but an error occurred during post-Wiise processing. {ex.ToString()} (Operation ID: {operationRequest.Id}).";
                        }
                    }
                    else
                    {
                        //check for listed strings in response
                        if (_wiiseErrorsThatNeedToBeRetried.Contains(wiiseResult.ValidationErrors.First().Message))
                        //if (operationRequest.Message.Contains("The server committed a protocol violation") || operationRequest.Message.Contains("Sorry, we just updated this page"))
                        {
                            try
                            {
                                LoggingHelper.LogError("Possible Wiise error detected: {Message}", operationRequest.Message);

                                //attempt recovery
                                var operationId = wiiseResult.OperationId;
                                var operation = NHibernateSession.Current.Get<ExternalAccountingOperation>(operationId);
                                var workflowFeeId = (JsonConvert.DeserializeObject<Dictionary<string, int>>(operation.CompletionInput))["CredentialWorkflowFeeId"];
                                var workflowFee = NHibernateSession.Current.Query<CredentialWorkflowFee>().FirstOrDefault(x => x.Id == workflowFeeId);
                                var getInvoicesAsync = WiiseExecutor.Execute((svc, token) => svc.Api.GetInvoicesAsync(token.Value, token.Tenant, null, null, null, null, null, workflowFee.InvoiceId.Value.ToString()));
                                var invoice = getInvoicesAsync.Data._Invoices.FirstOrDefault();
                                if(invoice == null)
                                {
                                    HandleErrorResponse(userId, thisResult, wiiseResult, operationRequest);
                                    LoggingHelper.LogError("Wiise payment creation error: {Message}", operationRequest.Message);
                                    continue;
                                }
                                thisResult.Success = true;
                                thisResult.PaymentReference = invoice.Reference;
                                thisResult.InvoiceNumber = invoice.InvoiceNumber;
                                operationRequest.Output = wiiseResult.Reference;
                                operationRequest.PrerequisiteOperation.Output = wiiseResult.InvoiceNumber;
                                LoggingHelper.LogInfo($"invoice operation {operationRequest.PrerequisiteOperation.Id} has final output value of {wiiseResult.InvoiceNumber}");

                                _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.Successful, userId);

                                completion.PerformOperation(wiiseResult);

                            }
                            catch
                            {
                                //do nothing for now
                            }
                        }
                        else
                        {
                            HandleErrorResponse(userId, thisResult, wiiseResult, operationRequest);
                            LoggingHelper.LogError("Wiise payment creation error: {Message}", operationRequest.Message);
                        }
                    }
                    result.Add(thisResult);
                }
                else if (operationRequest.CompletionType == typeof(CreateApplicationRefundPaymentCompletionOperation).FullName)
                {
                    var completion = _queueService.GetCompletionOperation(operationRequest) as CreateApplicationRefundPaymentCompletionOperation;
                    completion.NotNull("No completion operation on Payment operation");

                    thisResult.CredentialApplicationRefundId = completion.CredentialApplicationRefundId;

                    if (!wiiseResult.HasValidationErrors)
                    {
                        try
                        {
                            thisResult.Success = true;
                            //thisResult.CreditNoteReference = wiiseResult.CreditNote.Reference;
                            thisResult.PaymentReference = wiiseResult.Reference;
                            thisResult.CreditNoteNumber = wiiseResult.CreditNoteNumber;

                            operationRequest.Message = thisResult.WarningMessage;
                            operationRequest.Output = wiiseResult.Reference;

                            if (!string.IsNullOrEmpty(wiiseResult.CreditNoteNumber))
                                operationRequest.PrerequisiteOperation.Output = wiiseResult.CreditNoteNumber;

                            LoggingHelper.LogInfo($"credit note operation {operationRequest.PrerequisiteOperation.Id} has final output value of {wiiseResult.CreditNoteNumber}");

                            _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.Successful, userId);

                            completion.PerformOperation(wiiseResult);
                        }
                        catch (Exception ex)
                        {
                            thisResult.WarningMessage =
                                $"The Wiise operation succeeded, but an error occurred during post-Wiise processing. {WiiseExceptionHelper.GetMessage(ex)} (Operation ID: {operationRequest.Id}).";
                        }
                    }
                    else
                    {
                        //check for listed strings in response
                        if (_wiiseErrorsThatNeedToBeRetried.Contains(wiiseResult.ValidationErrors.First().Message))
                        //if (operationRequest.Message.Contains("The server committed a protocol violation") || operationRequest.Message.Contains("Sorry, we just updated this page"))
                        {
                            try
                            {
                                LoggingHelper.LogError("Possible Wiise error detected: {Message}", operationRequest.Message);

                                //attempt recovery
                                var operationId = wiiseResult.OperationId;
                                var operation = NHibernateSession.Current.Get<ExternalAccountingOperation>(operationId);
                                var credentialApplicationRefundId = (JsonConvert.DeserializeObject<Dictionary<string, int>>(operation.CompletionInput))["CredentialApplicationRefundId"];
                                var workflowFee = NHibernateSession.Current.Query<CredentialApplicationRefund>().FirstOrDefault(x => x.Id == credentialApplicationRefundId);
                                var getCreditNotesAsync = WiiseExecutor.Execute((svc, token) => svc.Api.GetCreditNotesAsync(token.Value, token.Tenant, $"CreditNoteNumber = \"{credentialApplicationRefundId.ToString().ToUpper()}\""));
                                var creditNote = getCreditNotesAsync.Data._CreditNotes.FirstOrDefault();
                                if (creditNote == null)
                                {
                                    HandleErrorResponse(userId, thisResult, wiiseResult, operationRequest);
                                    LoggingHelper.LogError("Wiise creditnote creation error: {Message}", operationRequest.Message);
                                    continue;
                                }
                                thisResult.Success = true;
                                //thisResult = invoice.Reference;
                                thisResult.CreditNoteNumber = wiiseResult.CreditNoteNumber;
                                operationRequest.Output = wiiseResult.CreditNoteNumber;
                                LoggingHelper.LogInfo($"credit note operation {operationRequest.PrerequisiteOperation.Id} has final output value of {wiiseResult.CreditNoteNumber}");

                                _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.Successful, userId);

                                completion.PerformOperation(wiiseResult);

                            }
                            catch
                            {
                                //do nothing for now
                            }
                        }
                        else
                        {
                            HandleErrorResponse(userId, thisResult, wiiseResult, operationRequest);
                            LoggingHelper.LogError("Wiise payment creation error: {Message}", operationRequest.Message);
                        }
                    }
                    result.Add(thisResult);
                }
            }
            return result;
        }

        private IList<FinanceBatchCreditNoteOperationResult> ProcessCreditNoteBatch(IList<ExternalAccountingOperation> creditNoteOperations, int userId)
        {
            LoggingHelper.LogDebug("Processing credit note batch");
            var result = new List<FinanceBatchCreditNoteOperationResult>();
            //var models = creditNoteOperations
            //    .Select(_queueService.GetOperation<WiiseCreditNoteCreationOperation>)
            //    .Select(x => x.GetPreparedInput())
            //    .ToList();

            var wiiseResults = new List<CreditNote>();
            var models = new List<CreditNote>();

            creditNoteOperations.ForEach(x =>
            {
                var operation = _queueService.GetOperation<WiiseCreditNoteCreationOperation>(x);
                x.ProcessedDateTime = DateTime.Now;
                _queueService.SetOperationStatusAndSave(x, ExternalAccountingOperationStatusName.InProgress, userId);
                var model = operation.GetPreparedInput();
                model.OperationId = operation.OperationId;
                model.PostCreditNoteOnCreate = IsRefundDirectDeposit(x.CompletionInput);
                models.Add(model);
            });


            try
            {
                var creditNotes = new CreditNotes { _CreditNotes = models };
                var results = WiiseExecutor.Execute((svc, token) => svc.Api.CreateCreditNotesAsync(token.Value, token.Tenant, creditNotes));
                wiiseResults = results.Data._CreditNotes;
            }
            catch (ApiException ex)
            {
                HandleApiException(creditNoteOperations, userId, ex);
                throw ex;
            }
            catch (Exception ex)
            {
                HandleException(creditNoteOperations, userId, ex);
                throw ex;
            }

            for (int i = 0; i < wiiseResults.Count; i++)
            {
                var thisResult = new FinanceBatchCreditNoteOperationResult();

                var wiiseResult = wiiseResults[i];
                var operationRequest = creditNoteOperations[i];
                var completion = _queueService.GetCompletionOperation(operationRequest) as CreateApplicationRefundCompletionOperation;
                completion.NotNull("No completion operation on Credit note operation");

                thisResult.CredentialApplicationRefundId = completion.CredentialApplicationRefundId;
                thisResult.CreditNoteReference = operationRequest.Reference;

                if (!wiiseResult.HasValidationErrors)
                {
                    try
                    {
                        thisResult.Success = true;
                        thisResult.CreditNoteNumber = wiiseResult.CreditNoteNumber;
                        operationRequest.Message = thisResult.WarningMessage;
                        operationRequest.Output = wiiseResult.CreditNoteNumber;
                        _queueService.SetOperationStatusAndSave(operationRequest, ExternalAccountingOperationStatusName.Successful, userId);
                        //defensive. Dont know whether this is valid or not as WIISE config means we cant test credit notes in test
                        if(operationRequest.PrerequisiteOperation != null)
                        {
                                LoggingHelper.LogInfo($"credit note operation {operationRequest.PrerequisiteOperation.Id} has final output value of {wiiseResult.CreditNoteNumber}");
                        }
                        completion.PerformOperation(wiiseResult);
                    }
                    catch (Exception ex)
                    {
                        thisResult.WarningMessage =
                            $"The Wiise operation succeeded, but an error occurred during post-Wiise processing. {WiiseExceptionHelper.GetMessage(ex)} (Operation ID: {operationRequest.Id}).";
                    }
                }
                else
                {
                    HandleErrorResponse(userId, thisResult, wiiseResult, operationRequest);
                    LoggingHelper.LogError("Wiise creditnote creation error: {Message}", operationRequest.Message);
                }
                result.Add(thisResult);
            }
            return result;
        }

        private bool IsRefundDirectDeposit(string completionInput)
        {
            var refundId = (JsonConvert.DeserializeObject<Dictionary<string, string>>(completionInput))["CredentialApplicationRefundId"];
            var refund = NHibernateSession.Current.Query<CredentialApplicationRefund>().SingleOrDefault(x => x.Id == Convert.ToInt16(refundId));
            return refund.RefundMethodType.Name == "DirectDeposit";
        }

        private void HandleErrorResponse(int userId, FinanceBatchOperationResult thisResult, BaseModel wiiseResult, ExternalAccountingOperation operationRequest)
        {
            var resetToStatus = ExternalAccountingOperationStatusName.Failed;
            if (wiiseResult.ValidationErrors.Where(error =>
                   error.ErrorCode == (int)WiiseHttpExceptionResult.TooManyRequests
                || error.ErrorCode == (int)WiiseHttpExceptionResult.GatewayTimeout
                || error.ErrorCode == (int)WiiseHttpExceptionResult.RequestTimeout).FirstOrDefault() != null)
            {
                resetToStatus = ExternalAccountingOperationStatusName.Requested;
                operationRequest.ProcessedDateTime = null;
            }
            operationRequest.Message = String.Join("; ", wiiseResult.ValidationErrors.Select(x => x.Message));
            thisResult.ErrorMessage = operationRequest.Message;
            _queueService.SetOperationStatusAndSave(operationRequest, resetToStatus, userId);
        }

        private void HandleException(IList<ExternalAccountingOperation> paymentOperations, int userId, Exception ex)
        {
            LoggingHelper.LogInfo($"Exception (Non Wiise)- setting to status of 3");

            paymentOperations.ForEach(x =>
            {
                x.Exception = ex.StackTrace;
                x.Message = ex.Message;
                _queueService.SetOperationStatusAndSave(x, ExternalAccountingOperationStatusName.Failed, userId);
            });
        }

        private void HandleApiException(IList<ExternalAccountingOperation> paymentOperations, int userId, ApiException ex)
        {
            LoggingHelper.LogInfo($"Wiise {ex.ErrorCode} exception invoked", ex.ToString());
            var resetToStatus = ExternalAccountingOperationStatusName.Failed;
            if (ex.ErrorCode == (int)WiiseHttpExceptionResult.TooManyRequests
                || ex.ErrorCode == (int)WiiseHttpExceptionResult.GatewayTimeout
                || ex.ErrorCode == (int)WiiseHttpExceptionResult.RequestTimeout)
            {
                resetToStatus = ExternalAccountingOperationStatusName.Requested;
            }
            LoggingHelper.LogInfo($"Wiise API Exception - setting to status of {resetToStatus.ToString()}");
            paymentOperations.ForEach(x =>
            {
                x.ProcessedDateTime = resetToStatus == ExternalAccountingOperationStatusName.Requested ? null : x.ProcessedDateTime;
                x.Exception = ex.StackTrace;
                x.Message = ex.Message;
                _queueService.SetOperationStatusAndSave(x, resetToStatus, userId);
            });
            LoggingHelper.LogInfo($"Wiise API Exception. Http Code: {ex.ErrorCode} -  completed setting to status of {resetToStatus.ToString()}");
        }


        public FinanceBatchProcessResponse ProcessBatch(IList<ExternalAccountingOperation> operations, int maxBatchSize, int userId)
        {
            var result = new FinanceBatchProcessResponse();

            if (!operations.Any())
            {
                return result;
            }

            string step = "Initialisation";

            try
            {
                var invoiceBatch = operations
                   .Where(x => x.InputType == typeof(WiiseInvoiceCreationOperation).FullName)
                   .Take(maxBatchSize)
                   .ToList();

                var creditNoteBatch = operations
                   .Where(x => x.InputType == typeof(WiiseCreditNoteCreationOperation).FullName)
                   .Take(maxBatchSize)
                   .ToList();

                var paymentBatch = operations
                    .Where(x => x.InputType == typeof(WiisePaymentCreationOperation).FullName
                                && x.PrerequisiteOperation != null
                                // include payments where the invoice or credit note is also in this batch, OR the invoice isn't in the list of operations, meaning it was already created in an earlier batch
                                && (invoiceBatch.Contains(x.PrerequisiteOperation)
                                    || creditNoteBatch.Contains(x.PrerequisiteOperation)
                                    || !operations.Contains(x.PrerequisiteOperation)))
                    .ToList();

                if (invoiceBatch.Any())
                {
                    step = "Invoice creation";
                    result.Invoices = ProcessInvoiceBatch(invoiceBatch, userId);
                }

                if (creditNoteBatch.Any())
                {
                    step = "Credit note creation";
                    result.CreditNotes = ProcessCreditNoteBatch(creditNoteBatch, userId);
                }

                if (paymentBatch.Any())
                {
                    step = "Payment creation";
                    // filter out payments for any invoices or credit notes that failed, as the payment will fail too
                    var filteredBatch = paymentBatch
                        .Where(x => x.PrerequisiteOperation.Status.Name == ExternalAccountingOperationStatusName.Successful.ToString())
                        .Take(maxBatchSize)
                        .ToList();
                    if (filteredBatch.Any())
                    {
                        result.Payments = ProcessPaymentBatch(filteredBatch, userId);
                    }
                }

            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = $"An error occured during the {step} step of the batch Wiise update process: {WiiseExceptionHelper.GetMessage(ex)}";
                result.StackTrace = ex.StackTrace;
                LoggingHelper.LogException(ex, result.ErrorMessage);
            }

            return result;
        }

    }
}

