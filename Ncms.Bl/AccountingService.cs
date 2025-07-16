using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Global.Common.Logging;
using Ncms.Bl.Export;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Newtonsoft.Json;
using EndOfPeriodRequest = Ncms.Contracts.Models.Accounting.EndOfPeriodRequest;
using IApplicationService = Ncms.Contracts.IApplicationService;
using IApplicationQueryService = F1Solutions.Naati.Common.Contracts.Dal.QueryServices.IApplicationQueryService;


namespace Ncms.Bl
{
    public class AccountingService : IAccountingService
    {
        private readonly IUserService _userService;
        private readonly IApplicationService _applicationService;
        private readonly IFinanceService _financeService;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IApplicationQueryService _applicationQueryService;

        public AccountingService(IUserService userService, IApplicationService applicationService, 
                                IFinanceService financeService, IAutoMapperHelper autoMapperHelper, 
                                IApplicationQueryService applicationQueryService)
        {
            _userService = userService;
            _applicationService = applicationService;
            _financeService = financeService;
            _autoMapperHelper = autoMapperHelper;
            _applicationQueryService = applicationQueryService;
        }

        public GenericResponse<IEnumerable<OfficeModel>> GetOffices()
        {
            GetOfficesResponse officesResponse = null;

            officesResponse = _financeService.GetOffices();

            return officesResponse.ConvertServiceListResponse<OfficeDto, OfficeModel>(x => x.OrderBy(y => y.Name));
        }

        public GenericResponse<IEnumerable<EftMachineModel>> GetEftMachines()
        {
            GetEftMachinesResponse eftResponse = null;

            eftResponse = _financeService.GetEftMachines();

            return eftResponse.ConvertServiceListResponse<EftMachineDto, EftMachineModel>(x => x.OrderBy(y => y.TerminalNumber));
        }

        public GenericResponse<AccountingOptionsModel> GetAccountingOptions()
        {
            var accountsResponse = GetBankAccounts();
            var themesResponse = GetInvoiceBrandingThemes();

            var response = new BusinessServiceResponse[] { accountsResponse, themesResponse }
                .CombineResponses<AccountingOptionsModel>();

            response.Data.PaymentAccounts = accountsResponse.Data;
            response.Data.InvoiceBrandingThemes = themesResponse.Data;

            return response;
        }

        public GenericResponse<IEnumerable<InvoiceBrandingThemeModel>> GetInvoiceBrandingThemes()
        {
            GetInvoiceBrandingThemesResponse themesResponse = null;
            var response = new GenericResponse<IEnumerable<InvoiceBrandingThemeModel>>();

            themesResponse = _financeService.GetInvoiceBrandingThemes();

            if (!themesResponse.Error)
            {
                response.Data = themesResponse.InvoiceBrandingThemes.Select(_autoMapperHelper.Mapper.Map<InvoiceBrandingThemeModel>);
            }
            else
            {
                response.Success = false;
                response.Errors.Add(themesResponse.ErrorMessage);
                if (themesResponse.ApiKeyError)
                {
                    response.Warnings.Add("Cannot retrieve branding themes until Wiise API key is set correctly.");
                }
            }
            return response;
        }

        public GenericResponse<IEnumerable<BankAcountModel>> GetBankAccounts()
        {
            var response = new GenericResponse<IEnumerable<BankAcountModel>>();
            GetBankAccountsResponse accountsResponse = null;

            accountsResponse = _financeService.GetBankAccounts();

            if (!accountsResponse.Error)
            {
                response.Data = accountsResponse.BankAccounts.Select(_autoMapperHelper.Mapper.Map<BankAcountModel>);
            }
            else
            {
                response.Errors.Add(accountsResponse.ErrorMessage);
                response.Success = false;
                if (accountsResponse.ApiKeyError)
                {
                    response.Warnings.Add("Cannot retrieve bank accounts until Wiise API key is set correctly.");
                }
            }

            return response;
        }

        public GenericResponse<IEnumerable<InvoiceModel>> GetPurchaseInvoices(InvoiceRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<GetInvoicesRequest>(request);
            GetInvoicesResponse serviceResponse = null;
            var response = new GenericResponse<IEnumerable<InvoiceModel>>();

            serviceResponse = _financeService.GetPurchaseInvoices(serviceRequest);

            if (!string.IsNullOrEmpty(serviceResponse.WarningMessage))
            {
                response.Warnings.Add(serviceResponse.WarningMessage);
            }

            if (!serviceResponse.Error)
            {
                response.Data = serviceResponse.Invoices.Select(_autoMapperHelper.Mapper.Map<InvoiceModel>);
            }
            else
            {
                response.Success = false;
                response.Errors.Add(serviceResponse.ErrorMessage);

                if (serviceResponse.ApiKeyError)
                {
                    response.Warnings.Add("Cannot retrieve invoices until Wiise API key is set correctly.");
                }
            }

            return response;
        }

        public GenericResponse<IEnumerable<InvoiceModel>> GetInvoices(InvoiceRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<GetInvoicesRequest>(request);
            GetInvoicesResponse serviceResponse = null;
            var response = new GenericResponse<IEnumerable<InvoiceModel>>();

            serviceResponse = _financeService.GetInvoices(serviceRequest);

            if (!string.IsNullOrEmpty(serviceResponse.WarningMessage))
            {
                response.Warnings.Add(serviceResponse.WarningMessage);
            }

            if (!serviceResponse.Error)
            {
                response.Data = serviceResponse.Invoices.Select(_autoMapperHelper.Mapper.Map<InvoiceModel>);
            }
            else
            {
                response.Success = false;
                response.Errors.Add(serviceResponse.ErrorMessage);

                if (serviceResponse.ApiKeyError)
                {
                    response.Warnings.Add("Cannot retrieve invoices until Wiise API key is set correctly.");
                }
            }

            return response;
        }


        public GenericResponse<IEnumerable<PaymentModel>> GetPayments(EndOfPeriodRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<GetPaymentsRequest>(request);
            GetPaymentsResponse serviceResponse = null;
            var response = new GenericResponse<IEnumerable<PaymentModel>>();

            serviceResponse = _financeService.GetPayments(serviceRequest);

            if (!string.IsNullOrEmpty(serviceResponse.WarningMessage))
            {
                response.Warnings.Add(serviceResponse.WarningMessage);
            }

            if (!serviceResponse.Error)
            {
                response.Data = serviceResponse.Payments.Select(_autoMapperHelper.Mapper.Map<PaymentModel>);
            }
            else
            {
                response.Success = false;
                response.Errors.Add(serviceResponse.ErrorMessage);

                if (serviceResponse.ApiKeyError)
                {
                    response.Warnings.Add("Cannot retrieve payments until Wiise API key is set correctly.");
                }
            }

            return response;
        }

        public GenericResponse<InvoiceCreateResponseModel> CreateInvoice(InvoiceCreateRequestModel model)
        {
            var request = _autoMapperHelper.Mapper.Map<CreateInvoiceRequest>(model);
            request.UserId = _userService.Get().Id;

            var serviceResponse = _financeService.CreateInvoice(request);

            return serviceResponse.ConvertServiceResponse<CreateInvoiceResponse, InvoiceCreateResponseModel>();
        }

        public GenericResponse<InvoiceCreateResponseModel> CreateApplicationInvoice(ApplicationInvoiceCreateRequestModel model)
        {
            var request = _autoMapperHelper.Mapper.Map<CreateApplicationInvoiceRequest>(model);

            request.UserId = _userService.Get().Id;
            request.CancelOperationIfError = true;

            var serviceResponse = _financeService.CreateApplicationInvoice(request);

            return serviceResponse.ConvertServiceResponse<CreateInvoiceResponse, InvoiceCreateResponseModel>();
        }

        public GenericResponse<RefundCreateResponseModel> CreateApplicationRefund(ApplicationRefundCreateRequestModel model)
        {
            var request = _autoMapperHelper.Mapper.Map<CreateApplicationRefundRequest>(model);
            request.CancelOperationIfError = true;

            var serviceResponse = _financeService.CreateApplicationRefund(request);

            return serviceResponse.ConvertServiceResponse<CreateCreditNoteResponse, RefundCreateResponseModel>();
        }


        [Obsolete("Use GetInvoicePdfById instead")]
        public FileModel GetInvoicePdf(string invoiceNumber, InvoiceTypeModel type)
        {
            var serviceResponse = _financeService.GetInvoicePdf(new GetInvoicePdfRequest
            {
                InvoiceNumber = invoiceNumber,
                Type = (InvoiceType)type
            });
            if (serviceResponse.RateLimitExceeded)
            {
                throw new WiiseRateExceededException(serviceResponse.ErrorMessage);
            }
            if (serviceResponse.Error)
            {
                throw new UserFriendlySamException(serviceResponse.ErrorMessage);
            }

            return new FileModel
            {
                FileData = new MemoryStream(serviceResponse.FileContent),
                FileName = $"{invoiceNumber}.pdf",
                FileType = FileType.Pdf
            };
        }

        public FileModel GetInvoicePdfById(Guid invoiceId, string invoiceNumber, InvoiceTypeModel type)
        {
            Stream stream;

            var serviceResponse = _financeService.GetInvoicePdfById(new GetInvoicePdfByIdRequest
            {
                InvoiceId = invoiceId,
                Type = (InvoiceType)type
            });

            if (serviceResponse.RateLimitExceeded)
            {
                throw new WiiseRateExceededException(serviceResponse.ErrorMessage);
            }
            if (serviceResponse.Error)
            {
                throw new UserFriendlySamException(serviceResponse.ErrorMessage);
            }

            stream = new MemoryStream(serviceResponse.FileContent);
            return new FileModel
            {
                FileData = stream,
                FileName = $"{invoiceNumber}.pdf",
                FileType = FileType.Pdf
            };
        }

        public GenericResponse<InvoiceCreateResponseModel> RetryCreateInvoice(int operationId)
        {
            var request = new SubmitQueuedOperationRequest
            {
                OperationId = operationId,
                UserId = _userService.Get().Id
            };

            var serviceResponse = _financeService.CreateInvoiceFromQueue(request);

            return serviceResponse.ConvertServiceResponse<CreateInvoiceResponse, InvoiceCreateResponseModel>();
        }

        public GenericResponse<PaymentCreateResponseModel> CreatePayment(IEnumerable<PaymentCreateRequestModel> models)
        {
            CreatePaymentResponse serviceResponse = null;
            var request = new CreatePaymentRequest
            {
                UserId = _userService.Get().Id,
                Payments = models.Select(model =>
                {
                    var requestModel = _autoMapperHelper.Mapper.Map<CreatePaymentModel>(model);
                    requestModel.Date = model.DatePaid;
                    return requestModel;
                })
            };

            serviceResponse = _financeService.CreatePayment(request);

            var response = serviceResponse.ConvertServiceResponse<CreatePaymentResponse, PaymentCreateResponseModel>();
            if (response.Success)
            {
                var updateInvoiceResponse =
                    _applicationService.UpdateOutstandingInvoices(
                        new UpdateOutstandingInvoicesRequestModel
                        {
                            InvoiceNumber = models.First().InvoiceNumber
                        });

                response.Success = updateInvoiceResponse.Success;
                response.Errors.AddRange(updateInvoiceResponse.Errors);

                var updateRefundsResponse = _applicationService.UpdateOutstandingRefunds(
                    new OutstandingRefundsRequestModel() { CreditNotes = new[] { models.First().InvoiceNumber } });
                response.Success = response.Success && updateRefundsResponse.Success;
                response.Errors.AddRange(updateRefundsResponse.Errors);
            }

            return response;
        }

        public GenericResponse<PaymentCreateResponseModel> RetryCreatePayment(int operationId)
        {
            CreatePaymentResponse serviceResponse = null;

            var request = new SubmitQueuedOperationRequest
            {
                OperationId = operationId,
                UserId = _userService.Get().Id
            };

            serviceResponse = _financeService.CreatePaymentFromQueue(request);

            return serviceResponse.ConvertServiceResponse<CreatePaymentResponse, PaymentCreateResponseModel>();
        }

        public BusinessServiceResponse CancelOperation(OperationCancelRequestModel model)
        {
            FinanceServiceResponse serviceResponse = null;

            serviceResponse = _financeService.CancelOperation(model.OperationId);

            return serviceResponse.ConvertServiceResponse();
        }

        public FileModel ExportEndOfPeriod(InvoiceRequest endOfPeriodRequest)
        {
            var payments = GetPayments(endOfPeriodRequest).Data.ToArray();
            var cash = payments.Where(x => x.PaymentType == "Cash").ToArray();
            var cheque = payments.Where(x => x.PaymentType == "Cheque").ToArray();
            var eft = payments.Where(x => x.PaymentType == "EFTPOS").ToArray();
            var amex = payments.Where(x => x.PaymentType == "AMEX").ToArray();
            var paypal = payments.Where(x => x.PaymentType == "PAYPAL").ToArray();
            var unknown = payments.Where(x => x.PaymentType == "Unknown").ToArray();
            var summary = new[]
            {
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "Cash Payments",
                    Count = cash.Length,
                    Amount = cash.Sum(x => x.Amount.GetValueOrDefault())
                },
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "Cheque Payments",
                    Count = cheque.Length,
                    Amount = cheque.Sum(x => x.Amount.GetValueOrDefault())
                },
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "Cash & Cheque Payments",
                    Count = cash.Length + cheque.Length,
                    Amount = cash.Concat(cheque).Sum(x => x.Amount.GetValueOrDefault())
                },
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "EFT Payments",
                    Count = eft.Length,
                    Amount = eft.Sum(x => x.Amount.GetValueOrDefault())
                },
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "AMEX Payments",
                    Count = amex.Length,
                    Amount = amex.Sum(x => x.Amount.GetValueOrDefault())
                },
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "PayPal Payments",
                    Count = paypal.Length,
                    Amount = paypal.Sum(x => x.Amount.GetValueOrDefault())
                },
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "Unknown Payments",
                    Count = unknown.Length,
                    Amount = unknown.Sum(x => x.Amount.GetValueOrDefault())
                },
                new EndOfPeriodSummaryModel
                {
                    SummaryItem = "Payments Total",
                    Count = payments.Length,
                    Amount = payments.Sum(x => x.Amount.GetValueOrDefault())
                }
            };

            var exporter = new EndOfPeriodExporter(endOfPeriodRequest, payments, summary, this);

            return new FileModel
            {
                FileData = exporter.Export(FileType.Xlsx),
                FileName = "NCMS-EndOfPeriod.xlsx",
                FileType = FileType.Xlsx
            };
        }

        public IEnumerable<object> GetQueuedOperations(string request)
        {
            var queuedOperationsRequest = JsonConvert.DeserializeObject<GetQueuedOperationsRequest>(request);
            GetQueuedOperationsResponse queuedOperationsResponse = null;

            try
            {
                queuedOperationsResponse = _financeService.GetQueuedOperations(queuedOperationsRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return queuedOperationsResponse?.Data.Select(_autoMapperHelper.Mapper.Map<QueuedOperationModel>).ToList() ?? new List<QueuedOperationModel>();
        }

        public GenericResponse<bool> ProgressCreditNote(ProgressCreditNoteModel model)
        {
            //find application
            var applicationResult = _applicationService.GetApplication(model.ApplicationId);
            if (!applicationResult.Success)
            {
                return GenerateGenericResponseError(model.ApplicationId,"Can't find corresponding application");
            }

            var application = applicationResult.Data;

            if (!(application.ApplicationStatusTypeId == 6 || application.ApplicationStatusTypeId == 5)) //allow 5 for reprocessing
            {
                return GenerateGenericResponseError(model.ApplicationId, "Credential Application status must be In progress. Can't continue");
            }

            //credential requests
            var credentialRequestsResult = _applicationService.GetCredentialRequests(model.ApplicationId);

            if(!credentialRequestsResult.Success)
            {
                return GenerateGenericResponseError(model.ApplicationId,"Can't find credential requests");
            }

            var credentialRequests = credentialRequestsResult.Data;

            if (credentialRequests.Count() > 1)
            {
                if (!credentialRequestsResult.Success)
                {
                    return GenerateGenericResponseError(model.ApplicationId,"More than one credential request.Can't continue");
                }
            }

            var credentialRequest = credentialRequests.First();

            var credentialWorkFlowFeeResult = _financeService.FindCredentialWorkflowFee(credentialRequest.Id);

            if (!credentialWorkFlowFeeResult.Success)
            {
                return GenerateGenericResponseError(model.ApplicationId, "Credential Workflow Fee not found. Can't continue");
            }

            var credentialWorkflowFee = credentialWorkFlowFeeResult.Data;

            //all workflows for a credit result in withdrawn

            switch (credentialWorkflowFee.ProductSpecification.ProductCategoryId)
            {
                case (int)ProductCategoryTypeName.ApplicationFee:
                    //do nothing
                    break;
                case (int)ProductCategoryTypeName.AssessmentFee:
                    {
                        //do nothing
                        break;
                    }
                case (int)ProductCategoryTypeName.ReviewFee:
                    {
                        //do nothing
                        break;
                    }

                case (int)ProductCategoryTypeName.SupplementaryTestFee:
                    {
                        //do nothing
                        break;
                    }

                case (int)ProductCategoryTypeName.TestingFee:
                    {
                        //do nothing
                        break;
                    }
                default:
                    {
                        return GenerateGenericResponseError(model.ApplicationId, $"Unknown Fee Type {credentialWorkflowFee.ProductSpecification.ProductCategoryId}");
                    }
            }

            //now start checking. If credit note is already processed it will have an "CR" prefix

            //now the CredentialApplicationRefund Record
            var refundResult = _applicationQueryService.GetCredentialRequestRefunds(credentialRequest.Id);

            if (refundResult.Results.Count() == 0)
            {
                return GenerateGenericResponseError(model.ApplicationId, "No refund found");
            }

            if (refundResult.Results.Count() > 1)
            {
                return GenerateGenericResponseError(model.ApplicationId, "Too many refunds found");
            }

            var refund = refundResult.Results.First();

            if (refund.CreditNoteNumber.StartsWith("CR"))
            {
                //already processed. Has the payment gone through
                if (credentialWorkflowFee.PaymentActionProcessedDate.HasValue)
                {
                    return GenerateGenericResponseError(model.ApplicationId, $"Credit Note has been processed {refund.CreditNoteNumber}, Refund also processed on {refund.CreditNotePaymentProcessedDate.Value.ToShortDateString()}");
                }
                //otherwise Invoice was processed successfully. Notify that PDF is most likely failing
                return GenerateGenericResponseError(model.ApplicationId, $"Credit Note has been processed {refund.CreditNoteNumber} but workflow has not progressed. Notify F1.");
            }

            if (!(credentialRequest.StatusTypeId == (int)CredentialRequestStatusTypeName.RefundRequestApproved || credentialRequest.StatusTypeId == (int)CredentialRequestStatusTypeName.Withdrawn)) //allow 16 for reprocessing
            {
                return GenerateGenericResponseError(model.ApplicationId, "Credential Request status must be Refund Request Approved.Can't continue");
            }

            refund.CreditNoteNumber = model.CreditNoteNo;
            refund.PaymentReference = model.PaymentReference;
            refund.RefundedDate = DateTime.Now;
            refund.OnPaymentCreatedSystemActionTypeId = 1056;
            refund.DisallowProcessing = false;

            var response = _financeService.ProgressCreditNote(model.ApplicationId, credentialRequest.Id, refund);

            if(!response.Success)
            {
                return response;
            }

            return new GenericResponse<bool>(true);
        }

        public GenericResponse<bool> ProgressInvoice(ProgressInvoiceModel model)
        {
            //find application
            var applicationResult = _applicationService.GetApplication(model.ApplicationId);
            if (!applicationResult.Success)
            {
                return GenerateGenericResponseError(model.ApplicationId, "Can't find corresponding application");
            }

            //Check Applicaiton Status
            var application = applicationResult.Data;

            if (!(application.ApplicationStatusTypeId == (int)CredentialApplicationStatusTypeName.Completed || //allow Completed for reprocessing
                application.ApplicationStatusTypeId == (int)CredentialApplicationStatusTypeName.InProgress)) 
            {
                return GenerateGenericResponseError(model.ApplicationId, "Credential Application status must be In progress. Found {application.ApplicationStatusTypeId}. Can't continue");
            }


            //Does the Application have multiple Credential Requests 
            //if so might still be ok if there is only one at Processing Test Invoice

            var credentialRequestResult = _applicationService.GetCredentialRequests(model.ApplicationId);

            if(!credentialRequestResult.Success || credentialRequestResult.Data.Count() == 0)
            {
                return GenerateGenericResponseError(model.ApplicationId, "No Credential Requests Exist");
            }

            var credentialRequests = credentialRequestResult.Data;

            var candidateCredentialRequests = credentialRequests.Where(x => x.StatusTypeId == 32);

            if (candidateCredentialRequests == null || candidateCredentialRequests.Count() > 1)
            {
                return GenerateGenericResponseError(model.ApplicationId, "Credential Request status must be Processing Test Payment. Can't continue");
            }

            //credential requests

            var credentialRequest = credentialRequests.First();

            //if we got this far then all clean. But what is happening in ExternalAccounting and CredentialWorkflow?

            var credentialWorkFlowFeeResult = _financeService.FindCredentialWorkflowFee(credentialRequest.Id);

            if(!credentialWorkFlowFeeResult.Success)
            {
                return GenerateGenericResponseError(model.ApplicationId, "Credential Workflow Fee not found. Can't continue");
            }

            var credentialWorkflowFee = credentialWorkFlowFeeResult.Data;

            //currently we are only suipport Testing Fee so return if not.

            switch(credentialWorkflowFee.ProductSpecification.ProductCategoryId)
            {
                case (int)ProductCategoryTypeName.ApplicationFee:
                    return GenerateGenericResponseError(model.ApplicationId, "Application Fees cannot be processed at this time");
                case (int)ProductCategoryTypeName.AssessmentFee:
                {
                    return GenerateGenericResponseError(model.ApplicationId, "Assessment Fees cannot be processed at this time");
                }    
                case (int)ProductCategoryTypeName.ReviewFee:
                {
                    return GenerateGenericResponseError(model.ApplicationId, "Review Fees cannot be processed at this time");
                }

                case (int)ProductCategoryTypeName.SupplementaryTestFee:
                {
                    return GenerateGenericResponseError(model.ApplicationId, "Supplementary Test Fees cannot be processed at this time");
                }

                case (int)ProductCategoryTypeName.TestingFee:
                {
                    //do nothing
                    break;
                }
                default:
                {
                        return GenerateGenericResponseError(model.ApplicationId, $"Unknown Fee Type {credentialWorkflowFee.ProductSpecification.ProductCategoryId}");
                 }
            }

            if (!(credentialRequest.StatusTypeId == (int)CredentialRequestStatusTypeName.ProcessingTestInvoice))
            {
                return GenerateGenericResponseError(model.ApplicationId, $"Credential Application status must be processing Test Invoice. Found {credentialRequest.StatusTypeId}. Can't continue");
            }

            //now start checking. If invoice is already processed it will have an "I" prefix

            if (credentialWorkflowFee.InvoiceNumber.StartsWith("I"))
            {
                //already processed. Has the payment gone through
                if(credentialWorkflowFee.PaymentActionProcessedDate.HasValue)
                {
                    return GenerateGenericResponseError(model.ApplicationId, $"Invoice has been processed {credentialWorkflowFee.InvoiceNumber}, Payment also processed on {credentialWorkflowFee.PaymentActionProcessedDate.Value.ToShortDateString()}");
                }
                //otherwise Invoice was processed successfully. Notify that PDF is most likely failing
                return GenerateGenericResponseError(model.ApplicationId, $"Invoice has been processed {credentialWorkflowFee.InvoiceNumber} but workflow has not progressed. Notify F1.");
            }

            //passed validation. Can be pushed forward.
            var result = _financeService.ProgressInvoice(model.ApplicationId, credentialRequest.Id,model.InvoiceNo,model.InvoiceId, model.PaymentId);
            if(!result.Success)
            {
                return result;
            }


            return new GenericResponse<bool>() { Data = true};
        }

        private GenericResponse<bool> GenerateGenericResponseError(int applicationId, string error)
        {
            LoggingHelper.LogDebug($"Progress Credit Note for {applicationId} Error: {error}");

            return new GenericResponse<bool>()
            {
                Success = false,
                Errors = new List<string>
                {
                    error
                }
            };
        }
    }
}
