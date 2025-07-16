using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Bl.Rubrics.Rules;

namespace Ncms.Bl.Rubrics.Evaluators
{
    public class RubricTestBandRuleEvaluator : RubricBandRuleEvaluator<RubricTestResultBandRule, IEnumerable<TestMarkingComponentDto>>
    {
        protected override bool EvaluateRule(RubricTestResultBandRule rule, IEnumerable<TestMarkingComponentDto> results)
        {
            // get all questions matching the rule task type, count the number of questions that pass the rule, and return true if the count is high enough
            return results
                       .Where(x => x.WasAttempted == true && x.TestComponentTypeId == rule.TestComponentTypeId)
                       .Select(question => question.RubricMarkingCompentencies
                           .SelectMany(y => y.RubricMarkingAssessmentCriteria)
                           .Single(x => x.AssessmentCriterionId == rule.RubricMarkingAssessmentCriterionId))
                       .Count(criterion => EvaluateMaximumBandRule(criterion, rule)) >= rule.NumberOfQuestions;
        }
    }
}