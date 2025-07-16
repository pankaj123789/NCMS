namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class ProductSpecificationLookupModel
    {
        public int Id { get; set; }
        public string DisplayNameWithGlCode { get; set; }
        public decimal CostPerUnit { get; set; }
        public bool Inactive { get; set; }
    }

    public class ProductSpecificationModel : ProductSpecificationLookupModel
    {
        public string GlCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual string Description { get; set; }
        public bool GstApplies { get; set; }
        public int CredentialFeeProductId { get; set; }
    }
}
