using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static F1Solutions.Naati.Common.Wiise.Mappers.PaymentMapper;

namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class SalesInvoiceOperation : BaseEntityOperation
    {
        internal SalesInvoiceOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }

        public async Task<ApiResponse<Invoices>> CreateInvoicesAsync(string accessToken, string tenantId, Invoices invoices)
        {
            var result = new List<Invoice>();
            var combinedStatusCode = System.Net.HttpStatusCode.OK;

            foreach (Invoice invoice in invoices._Invoices)
            {
                //based on type we are goibng to raise either a salesinvoice or a purchaseinvoice
                try
                {
                    switch (invoice.InvoiceType)
                    {
                        case InvoiceType.Bill:
                            var purchaseInvoice = await CreatePurchaseInvoiceAsync(accessToken, tenantId, invoice);
                            combinedStatusCode = GetStausCode(combinedStatusCode, purchaseInvoice);
                            result.Add(purchaseInvoice);
                            break;
                        case InvoiceType.Invoice:
                            //create or update the contact
                            ApiResponse<NativeModels.Contacts> contactResult = null; 
                            switch (invoice.PaymentMethodId.Value.ToPaymentType())
                            {
                                case PaymentType.PayPal:
                                case PaymentType.SecurePay:
                                    contactResult = await CreateOrUpdateContact(accessToken, tenantId, invoice, true);
                                    break;
                                case PaymentType.DirectDeposit:
                                    //not updating the payment method so that it won't close the sponsor invoices when posted
                                    contactResult = await CreateOrUpdateContact(accessToken, tenantId, invoice, false);
                                    break; 
                            }
                            if (contactResult.StatusCode.IsStatusCodeAnError())
                            {
                                invoice.HasValidationErrors = true;
                                invoice.ValidationErrors.AddRange(contactResult.Data?._Contacts?.SingleOrDefault()?.ToPublicBaseModel().ValidationErrors);
                                combinedStatusCode = contactResult.StatusCode;
                                result.Add(invoice);
                            }
                            else
                            {
                                invoice.Contact.ContactID = contactResult.Data._Contacts.First().Id;
                                switch (invoice.PaymentMethodId.Value.ToPaymentType())
                                {
                                    case PaymentType.PayPal:
                                    case PaymentType.SecurePay:
                                        var salesInvoice = await CreateSalesInvoiceAsync(accessToken, tenantId, invoice, false);
                                        combinedStatusCode = GetStausCode(combinedStatusCode, salesInvoice);
                                        result.Add(salesInvoice);
                                        break;
                                    case PaymentType.DirectDeposit:
                                        var postedSalesInvoice = await CreateSalesInvoiceAsync(accessToken, tenantId, invoice, true);
                                        combinedStatusCode = GetStausCode(combinedStatusCode, postedSalesInvoice);
                                        result.Add(postedSalesInvoice);
                                        break;
                                }
                            }
                            break;
                        default:
                            invoice.HasValidationErrors = true;
                            invoice.ValidationErrors.Add(new ValidationError() { Message = $"{invoice.InvoiceType.ToString()} is not a known Invoice Type" });
                            result.Add(invoice);
                            break;
                    }
                }
                catch (ApiException ex)
                {
                    combinedStatusCode = (HttpStatusCode)ex.ErrorCode;
                    invoice.HasValidationErrors = true;
                    invoice.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = ex.ErrorCode,
                        Message = ex.Message
                    });
                    result.Add(invoice);
                    LoggingHelper.LogError($"Invoice operation { invoice.OperationId } resulted in API exception { ex.ErrorCode } : { ex.Message }");
                }
                catch (Exception ex)
                {
                    combinedStatusCode = 0;
                    invoice.HasValidationErrors = true;
                    invoice.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = 0,
                        Message = ex.Message
                    });
                    result.Add(invoice);
                    LoggingHelper.LogError($"Invoice operation { invoice.OperationId } resulted in exception { ex.Message } : { ex.StackTrace }");
                }
            }

            return new ApiResponse<Invoices>(combinedStatusCode, new Invoices { _Invoices = result } );
        }

        private async Task<Invoice> CreateSalesInvoiceAsync(string accessToken, string tenantId, Invoice invoice, bool postInvoice)
        {
            var nativeOperation = new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient);
            var nativeSalesInvoice = invoice.ToNativeSalesInvoice();
            //create invoice
            var createdInvoice = await nativeOperation.CreateInvoiceAsync(accessToken, tenantId, nativeSalesInvoice);
            if (!createdInvoice.StatusCode.IsStatusCodeAnError() && !createdInvoice.Data.HasValidationErrors)
            {
                if (postInvoice)
                {
                    LoggingHelper.LogInfo($"invoice operation {invoice.OperationId} has draft output value of {createdInvoice.Data.Number }");
                    //post invoice
                    nativeSalesInvoice.ExternalDocumentNumber = invoice.Reference;
                    var postedInvoice = await nativeOperation.PostInvoiceAsync(accessToken, tenantId, nativeSalesInvoice);
                    if (!postedInvoice.StatusCode.IsStatusCodeAnError() && !postedInvoice.Data.HasValidationErrors)
                    {
                        //retirieve new invoice number
                        var newInvoice = (await nativeOperation.GetInvoicesAsync(accessToken, tenantId, postedInvoice.Data.Id.Value.ToString()))
                            .Data._SalesInvoices.First().ToPublicInvoice();
                        LoggingHelper.LogInfo($"invoice operation { invoice.OperationId } has final output value of { newInvoice.InvoiceNumber }");
                        return newInvoice;
                    }
                    return postedInvoice.Data.ToPublicInvoice();
                }
            }
            return createdInvoice.Data.ToPublicInvoice();
        }

        private async Task<Invoice> CreatePurchaseInvoiceAsync(string accessToken, string tenantId, Invoice invoice)
        {
            var nativeOperation = new EntityNativeOperations.PurchaseInvoiceOperation(this.AsynchronousClient);
            var nativePurcahseInvoice = invoice.ToNativePurchaseInvoice();
            //create invoice
            var createdInvoice = await nativeOperation.CreateInvoiceAsync(accessToken, tenantId, nativePurcahseInvoice);
            if (!createdInvoice.StatusCode.IsStatusCodeAnError() && !createdInvoice.Data.HasValidationErrors)
            {
                //post invoice
                var postedInvoice = await nativeOperation.PostInvoiceAsync(accessToken, tenantId, nativePurcahseInvoice);
                return postedInvoice.Data.ToPublicInvoice();
            }
            return createdInvoice.Data.ToPublicInvoice();
        }

        private async Task<ApiResponse<NativeModels.Contacts>> CreateOrUpdateContact(string accessToken, string tenantId, Invoice invoice, bool updatePaymentMethod)
        {
            var contactOperation = new EntityNativeOperations.ContactOperation(this.AsynchronousClient);

            ApiResponse<NativeModels.Contacts> contacts = null;
            ApiException errorResponse = null;
            try
            {
                contacts = await contactOperation.GetContactsAsync(accessToken, tenantId, $"?$filter=number%20eq%20%27{invoice.Contact.AccountNumber}%27");
            }
            catch (ApiException ex)
            {
                errorResponse = ex;
            }
            if (errorResponse != null || contacts.StatusCode.IsStatusCodeAnError())
            {
                var responseContact = invoice.Contact.ToNativeModelContact();
                responseContact.HasValidationErrors = true;
                responseContact.ValidationErrors.Add(new NativeModels.ValidationError
                {
                    ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)contacts.StatusCode,
                    Message = errorResponse != null ? errorResponse.Message : (contacts.ErrorText == null ? contacts.StatusCode.ToString() : contacts.ErrorText)
                });
                return new ApiResponse<NativeModels.Contacts>
                    (errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : contacts.StatusCode,
                    new NativeModels.Contacts() { _Contacts = new List<NativeModels.Contact> { responseContact } });
            }
            else if (contacts.Data._Contacts.Count == 0)
            {
                //create
                var nativeContact = invoice.Contact.ToNativeModelContact();
                if (updatePaymentMethod)
                    nativeContact.PaymentMethodId = invoice.PaymentMethodId;
                var nativeContacts = new NativeModels.Contacts { _Contacts = new List<NativeModels.Contact> { nativeContact } };

                var result = await contactOperation.CreateContactsAsync(
                    accessToken,
                    tenantId,
                    nativeContacts);

                return result;
            }
            else
            {
                // update
                var srcContact = contacts.Data._Contacts.Single();
                invoice.Contact.ContactID = srcContact.Id;
                var targetContact = invoice.Contact.ToNativeModelContact();
                if (updatePaymentMethod)
                    targetContact.PaymentMethodId = invoice.PaymentMethodId;
                if (srcContact.IsUpdated(targetContact))
                {
                    var result = await contactOperation.UpdateContactsAsync(
                        accessToken,
                        tenantId,
                        new NativeModels.Contacts { _Contacts = new List<NativeModels.Contact> { targetContact } },
                        srcContact.OdataEtag);

                    return result;
                }
                else
                {
                    var foundContacts = new NativeModels.Contacts();
                    foundContacts._Contacts.Add(targetContact);
                    return new ApiResponse<NativeModels.Contacts>(System.Net.HttpStatusCode.OK, foundContacts);
                }
            }
        }

        public async Task<ApiResponse<Invoices>> GetInvoicesAsync(string accessToken, string tenantId, string where = null, List<Guid> contactIDs = null, List<string> statuses = null, int? page = null, string invoiceId = null)
        {
            //generate the filter based on the where, contactIds and statuses  
            var filters = new List<string>();
            if (where != null)
            {
                var whereFilter = where?.ToSalesInvoiceFilter();
                if (!string.IsNullOrWhiteSpace(whereFilter)) filters.Add(whereFilter);
            }

            if (contactIDs != null)
            {
                var filterParams = contactIDs.Select(contactId => string.Format("(customerId eq {0})", contactId));
                filters.Add("(" + string.Join(" or ", filterParams) + ")");
            }
            if (statuses != null)
            {
                var filterParams = statuses.Select(status => string.Format("(status eq \'{0}\')", InvoiceMapper.ToNativeInvoiceStatus(status)));
                filters.Add("(" + string.Join(" or ", filterParams) + ")");
            }

            var filter = string.Join(" and ", filters);
            var nativeResponse = await new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient).GetInvoicesAsync(accessToken, tenantId, invoiceId, "salesInvoiceLines, customer", filter);
            var publicResponse = new ApiResponse<Invoices>(nativeResponse.StatusCode, nativeResponse.Data.ToPublicInvoices());
            return publicResponse;
        }

        public async Task<ApiResponse<Invoices>> PostInvoicesAsync(string accessToken, string tenantId, Invoices invoices)
        {
            var nativeResponse = await new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient).PostInvoicesAsync(accessToken, tenantId, invoices.ToNativeSalesInvoices());
            return new ApiResponse<Invoices>(nativeResponse.StatusCode, nativeResponse.Data.ToPublicInvoices());

        }

        public async Task<ApiResponse<Stream>> GetInvoiceAsPdfAsync(string accessToken, string tenantId, Guid invoiceId)
        {
            var nativeResponse = await new EntityNativeOperations.SalesInvoiceOperation(this.AsynchronousClient).GetInvoiceAsPdfAsync(accessToken, tenantId, invoiceId);
            return new ApiResponse<Stream>(nativeResponse.StatusCode, nativeResponse.Data);
        }
    }
}