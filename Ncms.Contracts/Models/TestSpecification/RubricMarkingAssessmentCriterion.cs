using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestSpecification
{
    public class RubricMarkingAssessmentCriterion : BaseModelClass
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public int DisplayOrder { get; set; }
        public List<RubricMarkingBand> RubricMarkingBand { get; set; }
        public string Competency { get; set; }
        public string TestSpecificationDescription { get; set; }
        public string TestComponentTypeName { get; set; }
        public RubricMarkingAssessmentCriterion()
        {
            RubricMarkingBand = new List<RubricMarkingBand>();
        }
    }
}
