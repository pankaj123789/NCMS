namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProductCategory : LegacyEntityBase, IDynamicLookupType, ILookupType
    {
        protected ProductCategory()
        {
        }

        public ProductCategory(int id) : base(id)
        {
        }

        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }
        public virtual string Code { get; set; }
        public virtual ProductType ProductType { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
