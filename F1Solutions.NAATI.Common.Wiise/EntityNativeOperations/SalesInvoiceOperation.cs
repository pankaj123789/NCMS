using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class SalesInvoiceOperation : BaseEntityOperation
    {
        internal SalesInvoiceOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient)
        {

        }

        internal async Task<ApiResponse<SalesInvoice>> CreateInvoiceAsync(string accessToken, string tenantId, SalesInvoice invoice)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            if (!invoice.HasValidationErrors)
            {
                ApiResponse<SalesInvoice> response = await PostAsyncWithHttpInfo(accessToken, tenantId, invoice, "/salesInvoices");

                if (!string.IsNullOrEmpty(response.ErrorText) || response.StatusCode.IsStatusCodeAnError())
                {
                    responseCode = response.StatusCode;
                    invoice.HasValidationErrors = true;
                    invoice.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = (int)response.StatusCode,
                        Message = response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText
                    });
                }
                else
                {
                    invoice.Number = response.Data.Number;
                    invoice.Status = response.Data.Status;
                    invoice.Id = response.Data.Id;
                }
            }
            return new ApiResponse<SalesInvoice>(responseCode, invoice);
        }

        internal async Task<ApiResponse<SalesInvoices>> GetInvoicesAsync(string accessToken, string tenantId, string invoiceGuid = null, string expand = null, string filter = null, int? pageSize = null, int? pageNumber = null)
        {
            var overallResponse = new ApiResponse<SalesInvoices>(System.Net.HttpStatusCode.OK, null);

            if (!String.IsNullOrEmpty(invoiceGuid))
            {
                invoiceGuid = $"({invoiceGuid})";
                string path = $"/salesInvoices{invoiceGuid}";
                var response = await GetAsyncWithHttpInfo<SalesInvoice>(accessToken, tenantId, path, filter, expand, pageSize, pageNumber);

                overallResponse.StatusCode = response.StatusCode;
                overallResponse.Data = new SalesInvoices() { _SalesInvoices = new List<SalesInvoice>() { response.Data } };
            }
            else
            {
                var response = await GetAsyncWithHttpInfo<SalesInvoices>(accessToken, tenantId, $"/salesInvoices", filter, expand, pageSize, pageNumber);
                overallResponse.StatusCode = response.StatusCode;
                overallResponse.Data = response.Data;
            }

            return overallResponse;
        }

        internal async Task<ApiResponse<SalesInvoice>> PatchInvoiceAsync(string accessToken, string tenantId, SalesInvoice invoice)
        {
            var responseCode = System.Net.HttpStatusCode.OK;

            if (!invoice.HasValidationErrors)
            {
                ApiResponse<SalesInvoice> response = null;
                ApiException errorResponse = null;
                var id = invoice.Id;
                var eTag = invoice.OdataEtag;
                invoice.Id = null;
                invoice.OdataEtag = null;

                try
                {
                    response = await PatchAsyncWithHttpInfo(accessToken, tenantId, invoice, eTag, $"/salesInvoices({id})");
                    invoice.Id = id;
                }
                catch (ApiException ex)
                {
                    errorResponse = ex;
                }

                if (errorResponse != null ||
                    (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                {
                    //this will be overwritten on multiple errors but should be ok
                    responseCode = errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : response.StatusCode;
                    invoice.HasValidationErrors = true;
                    invoice.Id = id;
                    invoice.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)response.StatusCode,
                        Message = errorResponse != null ? errorResponse.Message : (response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText)
                    });
                }
            }
            return new ApiResponse<SalesInvoice>(responseCode, invoice);
        }


        internal async Task<ApiResponse<SalesInvoices>> PostInvoicesAsync(string accessToken, string tenantId, SalesInvoices invoices)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            foreach (SalesInvoice invoice in invoices._SalesInvoices)
            {
                if (!invoice.HasValidationErrors)
                {
                    ApiResponse<SalesInvoice> response = null;
                    ApiException errorResponse = null;
                    try
                    {
                        string path = String.Format("/salesInvoices({0})/Microsoft.NAV.post", invoice.Id);
                        response = await PostAsyncWithHttpInfo<SalesInvoice>(accessToken, tenantId, null, path);
                    }
                    catch (ApiException ex)
                    {
                        errorResponse = ex;
                    }

                    if (errorResponse != null ||
                        !string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError()))
                    {
                        //this will be overwritten on multiple errors but should be ok
                        responseCode = AddErrorResponse(invoice, response, errorResponse);
                    }
                    else
                    {
                        //retrieve the posted invoice number
                        ApiResponse<SalesInvoices> invoicesResponse = null;
                        ApiException invoicesErrorResponse = null;

                        try
                        {
                            invoicesResponse = await GetInvoicesAsync(accessToken, tenantId, invoice.Id.Value.ToString());
                        }
                        catch (ApiException ex)
                        {
                            invoicesErrorResponse = ex;
                        }
                        if (invoicesErrorResponse != null ||
                            !string.IsNullOrEmpty(invoicesResponse.ErrorText) || invoicesResponse.StatusCode.IsStatusCodeAnError())
                        {
                            LoggingHelper.LogError($"Failed to retrieve posted sales invoice {invoice.Id.Value.ToString()}");
                            responseCode = AddErrorResponse(invoice, invoicesResponse, invoicesErrorResponse);
                        }
                        else
                        {
                            var newInvoice = invoicesResponse.Data._SalesInvoices.Single();
                            invoice.Id = newInvoice.Id;
                            invoice.Number = newInvoice.Number;
                        }
                    }
                }
            }
            return new ApiResponse<SalesInvoices>(responseCode, invoices);
        }

        internal async Task<ApiResponse<SalesInvoice>> PostInvoiceAsync(string accessToken, string tenantId, SalesInvoice invoice)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            if (!invoice.HasValidationErrors)
            {
                string path = String.Format("/salesInvoices({0})/Microsoft.NAV.post", invoice.Id);

                ApiResponse<SalesInvoice> response = null;
                ApiException errorResponse = null;

                try
                {
                    response = await PostAsyncWithHttpInfo<SalesInvoice>(accessToken, tenantId, null, path);
                }
                catch (ApiException ex)
                {
                    errorResponse = ex;
                }

                if (errorResponse != null ||
                         (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                {

                    responseCode = AddErrorResponse(invoice, response, errorResponse);
                }
            }
            return new ApiResponse<SalesInvoice>(responseCode, invoice);
        }



        internal async Task<ApiResponse<Stream>> GetInvoiceAsPdfAsync(string accessToken, string tenantId, Guid invoiceId)
        {
            var path = string.Format("/salesInvoices({0})/pdfDocument/pdfDocumentContent", invoiceId.ToString());

            var contentResponse = await GetAsyncWithHttpInfo<Stream>(accessToken, tenantId, path);

            return contentResponse;
        }

        private static HttpStatusCode AddErrorResponse<T>(SalesInvoice invoice, ApiResponse<T> invoicesResponse, ApiException invoicesErrorResponse)
        {
            HttpStatusCode responseCode;
            responseCode = invoicesErrorResponse != null ? (HttpStatusCode)invoicesErrorResponse.ErrorCode : invoicesResponse.StatusCode;
            invoice.HasValidationErrors = true;
            if (invoice.ValidationErrors == null)
            {
                invoice.ValidationErrors = new List<ValidationError>();
            }
            invoice.ValidationErrors.Add(new ValidationError()
            {
                ErrorCode = invoicesErrorResponse != null ? invoicesErrorResponse.ErrorCode : (int)invoicesResponse.StatusCode,
                Message = invoicesErrorResponse != null ? invoicesErrorResponse.Message : (invoicesResponse.ErrorText == null ? invoicesResponse.StatusCode.ToString() : invoicesResponse.ErrorText)
            });
            return responseCode;
        }
    }
}