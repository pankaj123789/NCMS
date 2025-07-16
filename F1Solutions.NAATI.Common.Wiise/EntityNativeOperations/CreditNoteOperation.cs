using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class CreditNoteOperation : BaseEntityOperation
    {
        internal CreditNoteOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient)
        {
        }

        internal async Task<ApiResponse<CreditNote>> CreateCreditNoteAsync(string accessToken, string tenantId, CreditNote creditNote)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            if (!creditNote.HasValidationErrors)
            {
                ApiResponse<CreditNote> response = await PostAsyncWithHttpInfo(accessToken, tenantId, creditNote, "/salesCreditMemos");

                if (!string.IsNullOrEmpty(response.ErrorText) || response.StatusCode.IsStatusCodeAnError())
                {
                    responseCode = response.StatusCode;
                    creditNote.HasValidationErrors = true;
                    creditNote.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = (int)response.StatusCode,
                        Message = response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText
                    });
                }
                else
                {
                    creditNote.Number = response.Data.Number;
                    creditNote.Status = response.Data.Status;
                    creditNote.Id = response.Data.Id;
                }
            }
            return new ApiResponse<CreditNote>(responseCode, creditNote);
        }

        internal async Task<ApiResponse<CreditNote>> PostCreditNoteAsync(string accessToken, string tenantId, CreditNote creditNote)
        {
            var responseCode = System.Net.HttpStatusCode.OK;

            if (!creditNote.HasValidationErrors)
            {
                string path = String.Format("/salesCreditMemos({0})/Microsoft.NAV.postAndSend", creditNote.Id);

                ApiResponse<CreditNote> response = null;
                ApiException errorResponse = null;

                try
                {
                    response = await PostAsyncWithHttpInfo<CreditNote>(accessToken, tenantId, null, path);
                }
                catch (ApiException ex)
                {
                    errorResponse = ex;
                }

                if (errorResponse != null ||
                   (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                {
                    responseCode = errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : response.StatusCode;
                    creditNote.HasValidationErrors = true;
                    creditNote.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)response.StatusCode,
                        Message = errorResponse != null ? errorResponse.Message : (response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText)
                    });
                }
            }
            return new ApiResponse<CreditNote>(responseCode, creditNote);
        }

        internal async Task<ApiResponse<CreditNote>> PatchCreditNoteAsync(string accessToken, string tenantId, CreditNote creditNote)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            if (!creditNote.HasValidationErrors)
            {
                ApiResponse<CreditNote> response = null;
                ApiException errorResponse = null;

                var id = creditNote.Id;
                var eTag = creditNote.OdataEtag;
                creditNote.Id = null;
                creditNote.OdataEtag = null;
                try
                {
                    response = await PatchAsyncWithHttpInfo(accessToken, tenantId, creditNote, eTag, $"/salesCreditMemos({id})");
                    creditNote.Id = id;
                }
                catch (ApiException ex)
                {
                    errorResponse = ex;
                }

                if (errorResponse != null ||
                    (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                {
                    responseCode = errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : response.StatusCode;
                    creditNote.HasValidationErrors = true;
                    creditNote.Id = id;
                    if (creditNote.ValidationErrors == null)
                    {
                        creditNote.ValidationErrors = new List<ValidationError>();
                    }
                    creditNote.ValidationErrors.Add(new ValidationError()
                    {
                        ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)response.StatusCode,
                        Message = errorResponse != null ? errorResponse.Message : (response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText)
                    });
                }
            }
            return new ApiResponse<CreditNote>(responseCode, creditNote);
        }

        internal async Task<ApiResponse<CreditNotes>> GetCreditNotesAsync(string accessToken, string tenantId, string filter = null, string expand = null)
        {
            return await GetAsyncWithHttpInfo<CreditNotes>(accessToken, tenantId, "/salesCreditMemos", filter);
        }

        internal async Task<ApiResponse<Stream>> GetCreditNoteAsPdfAsync(string accessToken, string tenantId, Guid invoiceId)
        {
            var path = string.Format("/salesCreditMemos({0})/pdfDocument/pdfDocumentContent", invoiceId.ToString());

            var contentResponse = await GetAsyncWithHttpInfo<Stream>(accessToken, tenantId, path);

            return contentResponse;
        }

    }
}
