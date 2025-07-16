using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestResultBandRuleDto 
    {
        public TestResultEligibilityTypeName TestResultEligibilityType { get; set; }
        public int RubricMarkingAssessmentCriterionId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int NumberOfQuestions { get; set; }
        public int MaximumBandLevel { get; set; }
        public string RuleGroup { get; set; }
    }
}