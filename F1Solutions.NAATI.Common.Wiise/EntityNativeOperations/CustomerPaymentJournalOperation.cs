using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class CustomerPaymentJournalOperation : BaseEntityOperation
    {
        internal CustomerPaymentJournalOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient)
        {

        }

        internal async Task<ApiResponse<CustomerPaymentJournal>> CreateCustomerPaymentJournalAsync(string accessToken, string tenantId, string journalNumber)
        {
            var customerPaymentJournal = new CustomerPaymentJournal();
            var responseCode = System.Net.HttpStatusCode.OK;
            var response = await PostAsyncWithHttpInfo(accessToken, tenantId, new CustomerPaymentJournal()
            {
                Code = journalNumber,
            }, "/customerPaymentJournals");

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

        internal async Task<ApiResponse<CustomerPaymentJournals>> GetCustomerPaymentJournalsAsync(string accessToken, string tenantId, string filter = null)
        {
            return await GetAsyncWithHttpInfo<CustomerPaymentJournals>(accessToken, tenantId, "/customerPaymentJournals", filter);
        }
    }
}
