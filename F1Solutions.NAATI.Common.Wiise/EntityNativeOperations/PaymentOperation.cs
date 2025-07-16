using F1Solutions.Naati.Common.Wiise.HttpOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Wiise.NativeModels;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class PaymentOperation: BaseEntityOperation
    {
        internal PaymentOperation(IAsynchronousClient asynchronousClient):base(asynchronousClient)
        {

        }


        internal async Task<ApiResponse<List<JournalLine>>> CreateLineItemsAsync(string accessToken, string tenantId, List<JournalLine> lineItems, Journal journal)
        {
            var responseCode = System.Net.HttpStatusCode.OK;

            foreach (JournalLine lineItem in lineItems)
            {
                lineItem.HasValidationErrors = false;

                var path = string.Format("/journals({0})/journalLines", journal.Id);

                var response = await PostAsyncWithHttpInfo<JournalLine>(accessToken, tenantId, lineItem, path);
                if (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Created))
                {
                    //this will be overwritten on multiple errors but should be ok
                    responseCode = response.StatusCode;
                    lineItem.HasValidationErrors = true;
                    if (lineItem.ValidationErrors == null)
                    {
                        lineItem.ValidationErrors = new List<ValidationError>();
                    }
                    lineItem.ValidationErrors.Add(new ValidationError()
                    {
                        Message = response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText
                    });
                }
            }
            return new ApiResponse<List<JournalLine>>(responseCode, lineItems);
        }


        internal async Task<ApiResponse<List<Payment>>> CreatePaymentsAsync(string accessToken, string tenantId, List<Payment> payments, Journal journal)
        {
            var responseCode = System.Net.HttpStatusCode.OK;

            foreach (Payment payment in payments)
            {
                payment.HasValidationErrors = false;

                var path = string.Format("/journals({0})/customerPayments", journal.Id);

                var response = await PostAsyncWithHttpInfo(accessToken, tenantId, payment, path);
                if (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Created))
                {
                    //this will be overwritten on multiple errors but should be ok
                    responseCode = response.StatusCode;
                    payment.HasValidationErrors = true;
                    if (payment.ValidationErrors == null)
                    {
                        payment.ValidationErrors = new List<ValidationError>();
                    }
                    payment.ValidationErrors.Add(new ValidationError()
                    {
                        Message = response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText
                    });
                }
            }
            return new ApiResponse<List<Payment>>(responseCode, payments);
        }
        

        internal async Task<ApiResponse<Payments>> GetPaymentsAsync(string accessToken, string tenantId, CustomerPaymentJournal customerPaymentJournal, string filter = null)
        {
            var path = string.Format("/customerPaymentJournals({0})/customerPayments", customerPaymentJournal.Id);
            return  await GetAsyncWithHttpInfo<Payments>(accessToken, tenantId, path);
        }


    }
}