namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class InvoiceLineItemDto
    {
        public int ProductSpecificationId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal IncGstCostPerUnit { get; set; }
        public bool GstApplies { get; set; }
        public int NaatiNumber { get; set; }
        public int EntityId { get; set; }
        public string GlCode { get; set; }
    }
}