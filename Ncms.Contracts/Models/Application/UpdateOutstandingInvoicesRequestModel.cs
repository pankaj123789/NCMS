namespace Ncms.Contracts.Models.Application
{
    public class UpdateOutstandingInvoicesRequestModel
    {
        public  string InvoiceNumber { get; set; }
        public string PaymentReference { get; set; }
        public string TransactionId { get; set; }
        public string OrderNumber { get; set; }
    }
}
