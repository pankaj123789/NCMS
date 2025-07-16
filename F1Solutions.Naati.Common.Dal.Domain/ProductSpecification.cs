using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProductSpecification : LegacyEntityBase
    {
        public ProductSpecification()
        {
        }

        public ProductSpecification(int id) : base(id)
        {
        }

        public virtual ProductCategory ProductCategory { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Code { get; set; }
        public virtual decimal CostPerUnit { get; set; }
        public virtual bool GSTApplies { get; set; }
        public virtual GLCode GLCode { get; set; }
        public virtual int BatchQuantity { get; set; }
        public virtual bool Inactive { get; set; }
        public virtual JobType JobType { get; set; }
        public virtual string TrackingActivity { get; set; }
    }
}
