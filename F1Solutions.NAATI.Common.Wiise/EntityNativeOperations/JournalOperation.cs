using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class JournalOperation : BaseEntityOperation
    {
        internal JournalOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient)
        {

        }

        internal async Task<ApiResponse<CustomerPaymentJournal>> CreateJournalAsync(string accessToken, string tenantId, string journalNumber)
        {
            var customerPaymentJournal = new CustomerPaymentJournal();
            var responseCode = System.Net.HttpStatusCode.OK;

            var response = await PostAsyncWithHttpInfo(accessToken, tenantId, new CustomerPaymentJournal()
            {
                Code = journalNumber,
                DisplayName = journalNumber
            }, "/journals");

            if (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Created))
            {
                //this will be overwritten on multiple errors but should be ok
                responseCode = response.StatusCode;
                customerPaymentJournal.HasValidationErrors = true;
                if (customerPaymentJournal.ValidationErrors == null)
                {
                    customerPaymentJournal.ValidationErrors = new List<ValidationError>();
                }
                customerPaymentJournal.ValidationErrors.Add(new ValidationError()
                {
                    Message = response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText
                });

            }
            return new ApiResponse<CustomerPaymentJournal>(responseCode, response.Data);
        }

        internal async Task<ApiResponse<Journals>> GetJournalsAsync(string accessToken, string tenantId, string filter = null)
        {
            return await GetJournalsAsyncWithHttpInfo(accessToken, tenantId, filter);
        }

        internal async Task<ApiResponse<Journal>> PostJournal(string accessToken, string tenantId, Journal journal)
        {
            // verify the required parameter 'invoices' is set
            if (journal == null || journal.Id == null)
                throw new ApiException(400, "Missing required parameter 'journalId' when calling AccountingApi-> Post Journal");

            string path = String.Format("/journals({0})/Microsoft.NAV.post", journal.Id);

            var response = await PostAsyncWithHttpInfo(accessToken, tenantId, journal, path);

            return response;
        }

        private async Task<ApiResponse<Journals>> GetJournalsAsyncWithHttpInfo(string accessToken, string tenantId, string filter = null)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                RequestOptions.QueryParameters.Add("$filter", filter);
            }

            var response = await GetAsyncWithHttpInfo<Journals>(accessToken, tenantId, "/journals");

            return response;
        }
    }
}
