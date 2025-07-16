namespace Ncms.Contracts.Models.TestSpecification
{
    public class RubricMarkingBand : BaseModelClass
    {
        public string Label { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string Competency { get; set; }
        public string TestSpecificationDescription { get; set; }
        public string TestComponentTypeName { get; set; }
        public string RMarkingAssessmentCriterionName { get; set; }
    }
}
