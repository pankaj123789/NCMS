namespace Ncms.Contracts.Models.TestSpecification
{
    public class TestComponent : BaseModelClass
    {
        public int ComponentNumber { get; set; }
        public string Name { get; set; }
        public int GroupNumber { get; set; }
        public string Label { get; set; }
        public string TestSpecificationDescription { get; set; }
        public string TestComponentTypeName { get; set; }
    }
}
