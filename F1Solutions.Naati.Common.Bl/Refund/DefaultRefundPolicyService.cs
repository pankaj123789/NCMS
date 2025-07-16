using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Bl.Refund
{
    public class DefaultRefundPolicyService : IRefundPolicyService
    {
        public double? CalculateRefund(CalculateRefundInputData inputData)
        {
            ValidateRefundPolicyParams(inputData.Policy.RefundPolicyParameters);

            var refundPercentage = inputData.Policy.RefundPolicyParameters.SingleOrDefault(x => x.Name.Equals("RefundPercentage", StringComparison.InvariantCultureIgnoreCase))?.Value;

            return refundPercentage == null ? (double?)null : Convert.ToDouble(refundPercentage);
        }

        private void ValidateRefundPolicyParams(IList<RefundPolicyParameterData> refundPolicyParameters)
        {
            var refundPercentage = refundPolicyParameters.SingleOrDefault(x => x.Name.Equals("RefundPercentage", StringComparison.InvariantCultureIgnoreCase));

            if (refundPercentage != null && !Double.TryParse(refundPercentage.Value, out double percent))
            {
                throw new Exception("Invalid refund percentage");
            }
        }
    }
}
