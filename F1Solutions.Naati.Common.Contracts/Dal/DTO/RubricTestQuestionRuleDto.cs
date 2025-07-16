namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RubricTestQuestionRuleDto
    {
        public int TestSpecificationId { get; set; }
        public int? TestComponentTypeId { get; set; }
        public int TestResultEligibilityTypeId { get; set; }
        public int MinimumQuestionsAttempted { get; set; }
        public int MinimumQuestionsPassed { get; set; }
        public string RuleGroup { get; set; }
    }
}