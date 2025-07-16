using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using System;
using System.Text;

namespace F1Solutions.Naati.Common.Bl.Refund
{
    public class CredentialApplicationRefundService : ICredentialApplicationRefundService
    {
        private readonly IApplicationQueryService _applicationQueryService;

        public CredentialApplicationRefundService(IApplicationQueryService applicationQueryService)
        {
            _applicationQueryService = applicationQueryService;
        }

        public bool ValidatePayPalDateForRefund(int credentialRequestId)
        {
            // get paid work flow fees for the credential request
            var payPalProcessedDateResponse = _applicationQueryService.GetPaypalPaymentProcessedDate(credentialRequestId);

            if (!payPalProcessedDateResponse.Success)
            {
                var errors = new StringBuilder();

                foreach(var error in payPalProcessedDateResponse.Errors)
                {
                    errors.AppendLine(error);
                }

                throw new Exception($"{errors.ToString()}");
            }

            // if the process date for the paid work flow doesnt exist then return false
            if (!payPalProcessedDateResponse.Data.HasValue)
            {
                return false;
            }

            // set payPal cut off date -- 6 months
            var payPalCutOffDate = DateTime.Now.AddDays(-180);
            // check the processed date is less than the cutoff date, if so return false
            if (payPalProcessedDateResponse.Data < payPalCutOffDate)
            {
                return false;
            }

            // refund is valid, return true
            return true;
        }
    }
}
