namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrderInvoiceRequest : CreateInvoiceRequest
    {
        public int OrderId { get; set; }
    }
}