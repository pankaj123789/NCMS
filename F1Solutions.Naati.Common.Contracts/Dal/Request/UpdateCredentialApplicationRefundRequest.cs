namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateCredentialApplicationRefundRequest
    {
        public int Id { get; set; }
        public bool? DisallowProcessing { get; set; }
        public decimal? InitialPaidAmount { get; set; }
        public decimal? RefundAmount { get; set; }
        public decimal? InitialPaidTax { get; set; }

    }
}
