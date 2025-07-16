namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ProductSpecificationDetailsDto 
    {
        public int Id { get; set; }
        public string GlCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual string Description { get; set; }
        public decimal CostPerUnit { get; set; }
        public bool Inactive { get; set; }
        public bool GstApplies { get; set; } 
        public int ProductCategoryId { get; set; }
    }
}