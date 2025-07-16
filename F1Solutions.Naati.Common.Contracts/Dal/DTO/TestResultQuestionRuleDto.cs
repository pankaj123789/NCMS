using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestResultQuestionRuleDto
    {
        public TestResultEligibilityTypeName TestResultEligibilityType { get; set; }
        public int? TestComponentTypeId { get; set; }
        public int MinimumQuestionsAttempted { get; set; }
        public int MinimumQuestionsPassed { get; set; }
        public string RuleGroup { get; set; }
    }
}