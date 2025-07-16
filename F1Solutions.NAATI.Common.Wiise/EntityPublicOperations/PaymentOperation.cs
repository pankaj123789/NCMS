using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class PaymentOperation : BaseEntityOperation
    {
        internal PaymentOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }
        internal async Task<ApiResponse<Payments>> CreatePaymentsAsync(string accessToken, string tenantId, Payments payments, string journalNumber)
        {
            var combinedStatusCode = System.Net.HttpStatusCode.OK;
            var result = new List<Payment>();
            foreach (var payment in payments._Payments)
            {
                try
                {
                    if (!string.IsNullOrEmpty(payment.InvoiceNumber))
                    {
                        //create invoice payment
                        var invoicePaymentResult = await CreateInvoicePaymentAsync(accessToken, tenantId, payment, journalNumber);
                        combinedStatusCode = GetStausCode(combinedStatusCode, invoicePaymentResult);
                        result.Add(invoicePaymentResult);
                    }
                    else if (!string.IsNullOrEmpty(payment.CreditNoteNumber))
                    {
                        //create credit note payment
                        var creditNotePaymentResult = await CreateCreditNotePaymentAsync(accessToken, tenantId, payment);
                        combinedStatusCode = GetStausCode(combinedStatusCode, creditNotePaymentResult);
                        result.Add(creditNotePaymentResult);
                    }
                }
                catch(ApiException ex)
                {
                    combinedStatusCode = (HttpStatusCode)ex.ErrorCode;
                    payment.HasValidationErrors = true;
                    payment.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = ex.ErrorCode,
                        Message = ex.Message
                    });
                    result.Add(payment);
                    LoggingHelper.LogError($"Payment operation { payment.OperationId } resulted in API exception { ex.ErrorCode } : { ex.Message }");
                }
                catch(Exception ex)
                {
                    combinedStatusCode = 0;
                    payment.HasValidationErrors = true;
                    payment.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = 0,
                        Message = ex.Message
                    });
                    result.Add(payment);
                    LoggingHelper.LogError($"Payment operation { payment.OperationId } resulted in exception { ex.Message } : { ex.StackTrace }");
                }
            }

            return new ApiResponse<Payments>(combinedStatusCode, new Payments { _Payments = result });
        }
        private async Task<Payment> CreateInvoicePaymentAsync(string accessToken, string tenantId, Payment invoicePaymentToBePosted, string journalNumber)
        {
            var filter = string.Format("(number eq \'{0}\')", invoicePaymentToBePosted.InvoiceNumber);

            ApiResponse<NativeModels.SalesInvoices> invoiceResponse = await new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient).GetInvoicesAsync(accessToken, tenantId, null, null, filter);

            if (invoiceResponse.StatusCode.IsStatusCodeAnError())
            {
                invoicePaymentToBePosted.HasValidationErrors = true;
                invoicePaymentToBePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)invoiceResponse.StatusCode, Message = invoiceResponse.ErrorText });
            }
            else
            {
                var invoiceToBePosted = invoiceResponse.Data._SalesInvoices.Single();
                invoicePaymentToBePosted.InvoiceId = invoiceToBePosted.Id.Value;
                var payRefInvoice = new NativeModels.SalesInvoice()
                {
                    Id = invoiceToBePosted.Id,
                    ExternalDocumentNumber = invoicePaymentToBePosted.Reference.ToPayPalPart(),
                    OdataEtag = invoiceToBePosted.OdataEtag
                };
                invoiceToBePosted.ExternalDocumentNumber = invoicePaymentToBePosted.Reference;

                var referenceOutput = await new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient).PatchInvoiceAsync(accessToken, tenantId, payRefInvoice);
                if (referenceOutput.StatusCode.IsStatusCodeAnError())
                {
                    invoicePaymentToBePosted.HasValidationErrors = true;
                    invoicePaymentToBePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)referenceOutput.StatusCode, Message = referenceOutput.ErrorText });
                }
                else
                {
                    var paymentOutput = await new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient).PostInvoiceAsync(accessToken, tenantId, invoiceToBePosted);
                    if (paymentOutput.StatusCode.IsStatusCodeAnError())
                    {
                        invoicePaymentToBePosted.HasValidationErrors = true;
                        invoicePaymentToBePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)referenceOutput.StatusCode, Message = referenceOutput.ErrorText });
                    }
                    else
                    {
                        var paymentGetOutput = await new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient).GetInvoicesAsync(accessToken, tenantId, paymentOutput.Data.Id.ToString());
                        if (paymentGetOutput.StatusCode.IsStatusCodeAnError())
                        {
                            invoicePaymentToBePosted.HasValidationErrors = true;
                            invoicePaymentToBePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)referenceOutput.StatusCode, Message = referenceOutput.ErrorText });
                        }
                        else
                        {
                            invoicePaymentToBePosted.InvoiceNumber = paymentGetOutput.Data._SalesInvoices.Single().Number;
                        }
                    }
                }
            }

            return invoicePaymentToBePosted;
        }
        private async Task<Payment> CreateCreditNotePaymentAsync(string accessToken, string tenantId, Payment creditNotePaymentTobePosted)
        {
            var filter = string.Format("(number eq \'{0}\')", creditNotePaymentTobePosted.CreditNoteNumber);

            var creditNotesResponse = await new EntityNativeOperations.CreditNoteOperation(this.AsynchronousClient).GetCreditNotesAsync(accessToken, tenantId, filter);

            if (creditNotesResponse.StatusCode.IsStatusCodeAnError())
            {
                creditNotePaymentTobePosted.HasValidationErrors = true;
                creditNotePaymentTobePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)creditNotesResponse.StatusCode, Message = creditNotesResponse.ErrorText });
            }
            else
            {
                var creditNoteToBePosted = creditNotesResponse.Data._CreditNotes.Single();
                //update payments with the credit note id
                creditNotePaymentTobePosted.CreditNoteId = creditNoteToBePosted.Id.Value;

                //update invoices with the payment reference. Use new invoice objects
                var payRefCreditNote = new NativeModels.CreditNote()
                {
                    Id = creditNoteToBePosted.Id,
                    ExternalDocumentNumber = creditNotePaymentTobePosted.Reference.ToPayPalPart(),
                    OdataEtag = creditNoteToBePosted.OdataEtag
                };
                creditNoteToBePosted.ExternalDocumentNumber = creditNotePaymentTobePosted.Reference;

                var referenceOutput = await new EntityNativeOperations.CreditNoteOperation(this.AsynchronousClient).PatchCreditNoteAsync(accessToken, tenantId, payRefCreditNote);

                if (referenceOutput.StatusCode.IsStatusCodeAnError())
                {
                    creditNotePaymentTobePosted.HasValidationErrors = true;
                    creditNotePaymentTobePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)referenceOutput.StatusCode, Message = referenceOutput.ErrorText });
                }
                else
                {
                    var paymentOutput = await new EntityNativeOperations.CreditNoteOperation(this.AsynchronousClient).PostCreditNoteAsync(accessToken, tenantId, creditNoteToBePosted);
                    if (paymentOutput.StatusCode.IsStatusCodeAnError())
                    {
                        creditNotePaymentTobePosted.HasValidationErrors = true;
                        creditNotePaymentTobePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)referenceOutput.StatusCode, Message = referenceOutput.ErrorText });
                    }
                    else
                    {
                        var creditNoteFilter = string.Format("(id eq {0})", paymentOutput.Data.Id); 
                        var paymentGetOutput = await new EntityNativeOperations.CreditNoteOperation(this.AsynchronousClient).GetCreditNotesAsync(accessToken, tenantId, creditNoteFilter);
                        if (paymentGetOutput.StatusCode.IsStatusCodeAnError())
                        {
                            creditNotePaymentTobePosted.HasValidationErrors = true;
                            creditNotePaymentTobePosted.ValidationErrors.Add(new ValidationError { ErrorCode = (int)referenceOutput.StatusCode, Message = referenceOutput.ErrorText });
                        }
                        else
                        {
                            //Check this code.
                            creditNotePaymentTobePosted.CreditNoteNumber = paymentGetOutput.Data._CreditNotes.Single().Number;
                        }
                    }
                }
            }

            return creditNotePaymentTobePosted;
        }
        /// <summary>
        /// Payment retrieval is not used
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="tenantId"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        internal async Task<ApiResponse<Payments>> GetPaymentsAsync(string accessToken, string tenantId, string where)
        {
            //TODO - Awaitng response from wiise on retrieving the payments
            return new ApiResponse<Payments>(HttpStatusCode.OK, new Payments { _Payments = new List<Payment>() });
        }
    }
}
