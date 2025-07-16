using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RubricPassRulesDto
    {
        public int TestSpecificationId { get; set; }
        public bool ResultAutoCalculationEnabled { get; set; }
        public IEnumerable<QuestionPassRuleDto> QuestionPassRules { get; set; }
        public IEnumerable<TestResultBandRuleDto> TestResultBandRules { get; set; }
        public IEnumerable<TestResultQuestionRuleDto> TestResultQuestionRules { get; set; }
    }
}