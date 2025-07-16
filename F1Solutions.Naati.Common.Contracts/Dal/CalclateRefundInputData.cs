using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CalculateRefundInputData
    {
        public int CredentialWorkFlowFeeId { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialTypeId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public string PaymentReference { get; set; }
        public DateTime? PaymentActionProcessedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public CredentialApplicationRefundPolicyData Policy { get; set; }
        public string OrderNumber { get; set; }
        public string TransactionId { get; set; }
        public string Comments { get; set; }
    }
}
