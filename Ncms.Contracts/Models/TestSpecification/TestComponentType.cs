using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestSpecification
{
    public class TestComponentType : BaseModelClass
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TestComponentBaseTypeId { get; set; }
        public int MinExaminerCommentLength { get; set; }
        public int MinNaatiCommentLength { get; set; }
        public bool RoleplayersRequired { get; set; }
        public bool CandidateBriefRequired { get; set; }
        public int CandidateBriefavailabilitydays { get; set; }
        public double DefaultMaterialRequestHours { get; set; }
        public double DefaultMaterialRequestDueDays { get; set; }
        public List<TestComponentTypeStandardMarkingScheme> TestComponentTypeStandardMarkingScheme { get; set; }
        public List<RubricMarkingCompetency> RubricMarkingCompetency { get; set; }
        public string TestSpecificationDescription { get; set; }
        //public List<TestComponent> TestComponents { get; set; }
        public TestComponentType()
        {
            TestComponentTypeStandardMarkingScheme = new List<TestComponentTypeStandardMarkingScheme>();
            RubricMarkingCompetency = new List<RubricMarkingCompetency>();
            //TestComponents = new List<TestComponent>();
        }
    }
}
