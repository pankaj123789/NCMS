using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestSpecification
{
    public class TestSpecification : BaseModelClass
    {
        public string CredentialTypeName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool AutomaticIssuing { get; set; }
        public bool ResultAutoCalculation { get; set; }
        public List<TestComponentType> TestComponentTypes { get; set; }
        //public List<TestComponents> TestComponents { get; set; }
        public List<TestSpecificationStandardMarkingScheme> TestSpecificationStandardMarkingSchemes { get; set; }
        public List<TestComponent> TestComponents { get; set; }
        public double? MaxScoreDifference { get; set; }

        public TestSpecification()
        {
            TestComponentTypes = new List<TestComponentType>();
            TestComponents = new List<TestComponent>();
            TestSpecificationStandardMarkingSchemes = new List<TestSpecificationStandardMarkingScheme>();
        }
    }
}
