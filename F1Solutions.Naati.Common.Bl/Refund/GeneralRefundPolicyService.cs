using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using System;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using System.Linq;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Bl.Refund
{
    public class GeneralRefundPolicyService : IRefundPolicyService
    {
        public double? CalculateRefund(CalculateRefundInputData inputData)
        {
            if (inputData.CredentialRequestStatusTypeId != (int)CredentialRequestStatusTypeName.TestAccepted)
            {
                return null;
            }

            ValidateRefundPolicyParams(inputData.Policy.RefundPolicyParameters);

            // Credential type = CCL
            var sessionQueryService = ServiceLocator.Resolve<ITestSessionQueryService>();
            var testSittings = sessionQueryService.GetTestSittings(inputData.CredentialRequestId).Data;
            var latestTestSitting = testSittings.FirstOrDefault();

            if (latestTestSitting == null || latestTestSitting.Sat || latestTestSitting.RejectedDate == null)
            {
                return null;
            }

            var refundPercentage = inputData.Policy.RefundPolicyParameters.Single(x => x.Name.Equals("RefundPercentage", StringComparison.InvariantCultureIgnoreCase)).Value;
            var hoursBeforeTestSitting = inputData.Policy.RefundPolicyParameters.Single(x => x.Name.Equals("HoursBeforeTestSitting", StringComparison.InvariantCultureIgnoreCase)).Value;

            if (IsValidRejection(latestTestSitting, Convert.ToInt32(hoursBeforeTestSitting)))
            {
                return Convert.ToDouble(refundPercentage);
            }

            return 0;

        }

        private static bool IsValidRejection(TestSittingHistoryItemDto testSitting, int hoursBeforeTestSitting)
        {
            return ((testSitting.TestDateTime - testSitting.RejectedDate.Value).TotalHours >= hoursBeforeTestSitting);           
        }

        private void ValidateRefundPolicyParams(IList<RefundPolicyParameterData> refundPolicyParameters)
        {
            if (!refundPolicyParameters.Any())
            {
                throw new Exception("Empty refund policy parameters");
            }

            var refundPercentage = refundPolicyParameters.FirstOrDefault(x => x.Name.Equals("RefundPercentage", StringComparison.InvariantCultureIgnoreCase));
            var hoursBeforeTestSitting = refundPolicyParameters.FirstOrDefault(x => x.Name.Equals("HoursBeforeTestSitting", StringComparison.InvariantCultureIgnoreCase));

            if (refundPercentage == null || 
                !Double.TryParse(refundPercentage.Value, out double percent) ||
                hoursBeforeTestSitting == null || 
                !Int32.TryParse(hoursBeforeTestSitting.Value, out int hours))
            {
                throw new Exception("Invalid refund policy parameters");
            }
        }
    }
}
