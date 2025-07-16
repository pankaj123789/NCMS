using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class PurchaseInvoiceOperation : BaseEntityOperation
    {
        internal PurchaseInvoiceOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }

        internal async Task<ApiResponse<PurchaseInvoices>> GetPurchaseInvoicesAsync(string accessToken, string tenantId, string filter)
        {
            string path = $"/purchaseInvoices{filter}";

            return await GetAsyncWithHttpInfo<PurchaseInvoices>(accessToken, tenantId, path);
        }

        internal async Task<ApiResponse<PurchaseInvoice>> CreateInvoiceAsync(string accessToken, string tenantId, PurchaseInvoice invoice)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            if (!invoice.HasValidationErrors)
            {
                ApiResponse<PurchaseInvoice> response = await PostAsyncWithHttpInfo(accessToken, tenantId, invoice, "/purchaseInvoices");

                if (!string.IsNullOrEmpty(response.ErrorText) || response.StatusCode.IsStatusCodeAnError())
                {
                    responseCode = response.StatusCode;
                    invoice.HasValidationErrors = true;
                    if (invoice.ValidationErrors == null)
                    {
                        invoice.ValidationErrors = new List<ValidationError>();
                    }
                    invoice.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = (int)response.StatusCode,
                        Message = response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText
                    });
                }
                else
                {
                    //post the purchase invoice 
                    invoice.id = response.Data.id;
                    invoice.status = response.Data.status;
                    invoice.InvoiceDate = response.Data.InvoiceDate;
                    invoice.DueDate = response.Data.DueDate;
                    invoice.number = invoice.vendorInvoiceNumber;
                }
            }

            return new ApiResponse<PurchaseInvoice>(responseCode, invoice);
        }

        internal async Task<ApiResponse<PurchaseInvoices>> CreateInvoicesAsync(string accessToken, string tenantId, PurchaseInvoices invoices)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            var newInvoices = new PurchaseInvoices();
            foreach (PurchaseInvoice invoice in invoices._PurchaseInvoices)
            {
                if (!invoice.HasValidationErrors)
                {
                    ApiResponse<PurchaseInvoice> response = null;
                    ApiException errorResponse = null;

                    try
                    {
                        response = await PostAsyncWithHttpInfo(accessToken, tenantId, invoice, "/purchaseInvoices");
                    }
                    catch (ApiException ex)
                    {
                        errorResponse = ex;
                    }

                    if (errorResponse != null ||
                       (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                    {
                        responseCode = errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : response.StatusCode;
                        invoice.HasValidationErrors = true;
                        if (invoice.ValidationErrors == null)
                        {
                            invoice.ValidationErrors = new List<ValidationError>();
                        }
                        invoice.ValidationErrors.Add(new ValidationError()
                        {
                            ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)response.StatusCode,
                            Message = errorResponse != null ? errorResponse.Message : (response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText)
                        });
                    }
                    else
                    {
                        //post the purchase invoice 
                        invoice.id = response.Data.id;
                        invoice.status = response.Data.status;
                        invoice.InvoiceDate = response.Data.InvoiceDate;
                        invoice.DueDate = response.Data.DueDate;
                        invoice.number = invoice.vendorInvoiceNumber;
                        var postResult = await PostInvoiceAsync(accessToken, tenantId, invoice);
                        if (postResult.Data.HasValidationErrors)
                        {
                            responseCode = postResult.StatusCode;
                            if (invoice.ValidationErrors == null)
                            {
                                invoice.ValidationErrors = new List<ValidationError>();
                            }
                            invoice.ValidationErrors.AddRange(postResult.Data.ValidationErrors);
                        }
                    }
                    newInvoices._PurchaseInvoices.Add(invoice);
                }
            }
            return new ApiResponse<PurchaseInvoices>(responseCode, newInvoices);

        }

        public async Task<ApiResponse<PurchaseInvoice>> PostInvoiceAsync(string accessToken, string tenantId, PurchaseInvoice purchaseInvoice)
        {
            var responseCode = System.Net.HttpStatusCode.OK;

            if (!purchaseInvoice.HasValidationErrors)
            {
                string path = $"/purchaseInvoices({purchaseInvoice.id.Value})/Microsoft.NAV.post";

                ApiResponse<PurchaseInvoice> response = null;
                ApiException errorResponse = null;

                try
                {
                    response = await PostAsyncWithHttpInfo<PurchaseInvoice>(accessToken, tenantId, null, path);
                }
                catch (ApiException exApi)
                {
                    errorResponse = exApi;
                }

                if (errorResponse != null ||
                     (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                {
                    responseCode = errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : response.StatusCode;
                    purchaseInvoice.HasValidationErrors = true;
                    if (purchaseInvoice.ValidationErrors == null)
                    {
                        purchaseInvoice.ValidationErrors = new List<ValidationError>();
                    }
                    purchaseInvoice.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)response.StatusCode,
                        Message = errorResponse != null ? errorResponse.Message : (response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText)
                    });
                }
            }
            return new ApiResponse<PurchaseInvoice>(responseCode, purchaseInvoice);
        }
    }
}