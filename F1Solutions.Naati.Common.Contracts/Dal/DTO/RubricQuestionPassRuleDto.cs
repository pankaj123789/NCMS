namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RubricQuestionPassRuleDto
    {
        public int TestSpecificationId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int RubricMarkingAssessmentCriterionId { get; set; }
        public int MaximumBandLevel { get; set; }
        public string RuleGroup { get; set; }
    }
}