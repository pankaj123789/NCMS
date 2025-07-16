using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Bl.Refund
{
    public class RefundCalculationDto
    {
        public RefundCalculationResultTypeName RefundCalculationResultType { get; set; }
        public int CredentialWorkflowFeeId { get; set; } 
        public int? CredentialApplicationRefundId { get; set; }
        public double? RefundPercentage { get; set; }
        public decimal? PaidAmount { get; set; }
        public string Policy { get; set; }
        public string InvoiceNumber { get; set; }
        public IList<RefundMethodTypeName> AvailableRefundMethodTypes { get; set; }
        public string Comments { get; set; }
        public string BankDetails { get; set; }
        public decimal? RefundableAmount { get; set; }

    }
}
