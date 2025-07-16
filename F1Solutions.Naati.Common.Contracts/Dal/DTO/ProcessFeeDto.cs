using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ProcessFeeDto
    {
        public int CredentialWorkflowFeeId { get; set; }
        public ProcessTypeName Type { get; set; }

        public string PaymentReference { get; set; }
        public string TransactionId { get; set; }
        public string OrderNumber { get; set; }
    }
}