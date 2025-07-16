namespace Ncms.Contracts.Models.Accounting
{
    public class InvoiceLineItemModel
    {
        public int NaatiNumber { get; set; }
        public int ProductSpecificationId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal IncGstCostPerUnit { get; set; }
        public decimal ExGstCostPerUnit { get; set; }
        public bool GstApplies { get; set; }
        public int EntityId { get; set; }
        public string ProductCode { get; set; }
        public string GlCode { get; set; }
    }
}
