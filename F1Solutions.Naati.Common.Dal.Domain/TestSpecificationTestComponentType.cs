
namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSpecificationTestComponentType : EntityBase
    {
        public virtual TestSpecification TestSpecification { get; set; }
        public virtual TestComponentType TestComponentType { get; set; }
        public virtual int NumberRequired { get; set; }
    }
}
