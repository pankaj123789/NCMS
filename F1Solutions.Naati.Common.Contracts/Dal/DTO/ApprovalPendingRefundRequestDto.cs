namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ApprovalPendingRefundRequestDto : RefundDto
    {
        public string InvoiceNumber { get; set; }
        public int NAATINumber { get; set; }
        public string Policy { get; set; }
        public decimal? RefundableAmount { get; set; }

    }
}
