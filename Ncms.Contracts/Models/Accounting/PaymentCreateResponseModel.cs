namespace Ncms.Contracts.Models.Accounting
{
    public class PaymentCreateResponseModel
    {
        public string InvoiceId { get; set; }
        public string Reference { get; set; }
        public int? OperationId { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
    }
}