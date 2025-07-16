namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CredentialRequestWithPendingRefundRequest
    {
        public int CredentialApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal InitialPaidAmount { get; set; }
    }
}
