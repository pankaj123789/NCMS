namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RubricTestBandRuleDto
    {
        public int TestSpecificationId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int TestResultEligibilityTypeId { get; set; }
        public int NumberOfQuestions { get; set; }
        public int RubricMarkingAssessmentCriterionId { get; set; }
        public int MaximumBandLevel { get; set; }
        public string RuleGroup { get; set; }
    }
}