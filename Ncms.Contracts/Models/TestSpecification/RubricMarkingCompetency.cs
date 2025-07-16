using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestSpecification
{
    public class RubricMarkingCompetency : BaseModelClass
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public int DisplayOrder { get; set; }
        public List<RubricMarkingAssessmentCriterion> RubricMarkingAssessmentCriterion { get; set; }
        public string ComponentType { get; set; }
        public string TestSpecificationDescription { get; set; }
        public RubricMarkingCompetency()
        {
            RubricMarkingAssessmentCriterion = new List<RubricMarkingAssessmentCriterion>();
        }
    }
}
