using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Bl.Rubrics.Rules;

namespace Ncms.Bl.Rubrics.Evaluators
{
    public abstract class RubricBandRuleEvaluator<TRule, TInput> : RubricRuleEvaluator<TRule, TInput>
        where TRule : RubricMaximumBandRule
    {
        protected bool EvaluateMaximumBandRule(RubricMarkingAssessmentCriterionDto criterion, RubricMaximumBandRule rule)
        {
            var band = criterion?.Bands.SingleOrDefault(x => x.BandId == criterion.SelectedBandId);
            // to pass the rule, the band level achieved must be better (less) than or equal to the rule
            return band?.Level <= rule.MaximumBandLevel;
        }
    }
}