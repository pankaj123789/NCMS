using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;

namespace F1Solutions.Naati.Common.Bl.Refund
{
    public class RefundCalculator : IRefundCalculator
    {
        protected readonly IApplicationQueryService _mApplicationQueryService;
        private readonly IDictionary<string, Type> _mRefundPolicyServices;
        private readonly ISystemQueryService _systemService;
        public RefundCalculator(IApplicationQueryService applicationQueryService,
            ISystemQueryService systemService)
        {
            _mApplicationQueryService = applicationQueryService;
            _systemService = systemService; 

            _mRefundPolicyServices = new Dictionary<string, Type>
            {
                { "Default", typeof(DefaultRefundPolicyService) },
                { "General", typeof(GeneralRefundPolicyService) }
            };
        }

        public virtual RefundCalculationDto CalculateRefund(int credentialRequestId)
        {
            var workflowFeeData = _mApplicationQueryService.GetPaidWorkflowFeesForCredentialRequest(credentialRequestId);

            if (workflowFeeData.Data == null)
            {
                LoggingHelper.LogWarning("No paid workflow fee found to refund credential request {credentialRequestId}. One possible reason could be that invoice was issued to a sponsor", credentialRequestId);
                return new RefundCalculationDto()
                {
                    AvailableRefundMethodTypes = new List<RefundMethodTypeName>(),
                    RefundCalculationResultType = RefundCalculationResultTypeName.NotCalculated
                };
            }

            var calculationResult = new RefundCalculationDto
            {
                CredentialWorkflowFeeId = workflowFeeData.Data.CredentialWorkFlowFeeId,
                InvoiceNumber = workflowFeeData.Data.InvoiceNumber,
                AvailableRefundMethodTypes = GetRefundMethodTypes(workflowFeeData.Data.TransactionId, workflowFeeData.Data.OrderNumber)
            };

            if (workflowFeeData.Data.Policy == null)
            {
                LoggingHelper.LogWarning("No Policy or More than one policy found to refund credential request {credentialRequestId}", credentialRequestId);

                return calculationResult;
            }

            calculationResult.Policy = workflowFeeData.Data.Policy.Description;

            var refundPolicyCalculatorType = _mRefundPolicyServices[workflowFeeData.Data.Policy.Name];

            if (workflowFeeData.Data.CredentialRequestStatusTypeId == (int)CredentialRequestStatusTypeName.RefundRequested)
            {
                var result = TryGetRefundPercentageFromRequest(
                    workflowFeeData.Data.CredentialRequestId,
                    workflowFeeData.Data.CredentialWorkFlowFeeId);
                if (!result.valid)
                {
                    throw new Exception($"Error getting refund request for credential request {workflowFeeData.Data.CredentialRequestId}");
                }

                calculationResult.RefundPercentage = result.credentialApplicationRefund.RefundPercentage;
                calculationResult.CredentialApplicationRefundId = result.credentialApplicationRefund.Id;
                calculationResult.PaidAmount = result.credentialApplicationRefund.InitialPaidAmount;
                calculationResult.Comments = result.credentialApplicationRefund.Comments; 
                calculationResult.BankDetails = result.credentialApplicationRefund.BankDetails; 
            }
            else
            {
                var refundPolicyCalculator = Activator.CreateInstance(refundPolicyCalculatorType) as IRefundPolicyService;
                var refundPercentage = refundPolicyCalculator.CalculateRefund(workflowFeeData.Data);

                calculationResult.RefundPercentage = refundPercentage;
            }

            if (calculationResult.RefundPercentage.HasValue && calculationResult.RefundPercentage <= 0)
            {
                calculationResult.RefundCalculationResultType = RefundCalculationResultTypeName.NoRefund;
            }
            if (calculationResult.RefundPercentage.HasValue && calculationResult.RefundPercentage > 0)
            {
                calculationResult.RefundCalculationResultType = RefundCalculationResultTypeName.RefundAvailable;
            }

            return calculationResult;
        }
        
        private (bool valid, RefundDto credentialApplicationRefund) TryGetRefundPercentageFromRequest(int credentialRequestId, int credentialWorkflowFeeId)
        {
            var results = _mApplicationQueryService.GetCredentialRequestRefunds(credentialRequestId).Results.ToList();
            if (!results.Any())
            {
                LoggingHelper.LogError("No refund request found for credential requestId {credentialRequestId}", credentialRequestId);
                return (false, null);
            }

            if (results.Count > 1)
            {
                LoggingHelper.LogError("More than one suitable refund request found for credential request {credentialRequestId}", credentialRequestId);
                return (false, null);
            }

            var refund = results.First();
            if (refund.CredentialWorkflowFeeId != credentialWorkflowFeeId)
            {
                LoggingHelper.LogError("Mismatch between the requested refund and workflow fee for credential request {credentialRequestId}", credentialRequestId);
                return (false, null);
            }

            return (true, refund);
        }

        public IList<RefundMethodTypeName> GetRefundMethodTypes(string transactionId, string orderNumber)
        {
            var availableRefundMethodTypes = new List<RefundMethodTypeName>() { RefundMethodTypeName.DirectDeposit };

            if (!string.IsNullOrEmpty(transactionId) && !string.IsNullOrEmpty(orderNumber) && !orderNumber.StartsWith(PayPalService.PAYPALORDERPREFIX))
            {
                availableRefundMethodTypes.Insert(0, RefundMethodTypeName.CreditCard);
            }

            if (!string.IsNullOrEmpty(orderNumber) && orderNumber.StartsWith(PayPalService.PAYPALORDERPREFIX))
            {
                availableRefundMethodTypes.Insert(0, RefundMethodTypeName.PayPal);
            }

            return availableRefundMethodTypes; 
        }

        public bool ValidateRefundRequest(int credentialRequestId)
        {
            var paidCredentialRequests = _mApplicationQueryService.GetPaidWorkflowFeesForCredentialRequest(credentialRequestId).Data;
            var cutoffDate = _systemService.GetSystemValue(new GetSystemValueRequest { ValueKey = "AccountingCutOffDate" }).Value;

            if (paidCredentialRequests != null && paidCredentialRequests.PaymentActionProcessedDate < Convert.ToDateTime(cutoffDate))
            {
                return false;
            }

            return true;
        }
    }
}
