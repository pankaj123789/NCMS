using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Bl.Rubrics.Rules;

namespace Ncms.Bl.Rubrics.Evaluators
{
    public class RubricQuestionBandRuleEvaluator : RubricBandRuleEvaluator<RubricQuestionPassRule, IList<RubricMarkingAssessmentCriterionDto>>
    {
        public override bool EvaluateAnd(IList<RubricQuestionPassRule> rules, IList<RubricMarkingAssessmentCriterionDto> criteria)
        {
            return criteria?.Any() != true || base.EvaluateAnd(rules, criteria);
        }

        public override bool EvaluateOr(IList<RubricQuestionPassRule> rules, IList<RubricMarkingAssessmentCriterionDto> criteria)
        {
            return criteria?.Any() != true || base.EvaluateOr(rules, criteria);
        }

        protected override bool EvaluateRule(RubricQuestionPassRule rule, IList<RubricMarkingAssessmentCriterionDto> criteria)
        {
            var criterion = criteria.SingleOrDefault(x => x.AssessmentCriterionId == rule.RubricMarkingAssessmentCriterionId);
            return EvaluateMaximumBandRule(criterion, rule);
        }
    }
}