using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.Rubrics.Rules
{
    public class RubricRule
    {
        public TestResultEligibilityTypeName TestResultEligibilityType { get; set; } = TestResultEligibilityTypeName.Pass;
        public string RuleGroup { get; set; }
    }
}