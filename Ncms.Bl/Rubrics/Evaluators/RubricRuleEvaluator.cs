using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Bl.Rubrics.Rules;

namespace Ncms.Bl.Rubrics.Evaluators
{
    public abstract class RubricRuleEvaluator<TRule, TInput> 
        where TRule : RubricRule
    {
        public bool Evaluate(IList<TRule> rules, TInput taskResults, TestResultEligibilityTypeName eligibility)
        {
            var groupRules = rules
                .Where(x => x.TestResultEligibilityType == eligibility &&
                            x.RuleGroup != null)
                .GroupBy(x => x.RuleGroup)
                .ToList();

            var individualRules = rules
                .Where(x => x.TestResultEligibilityType == eligibility &&
                            x.RuleGroup == null)
                .ToList();

            var result = true;

            if (individualRules.Any())
            {
                result = EvaluateAnd(individualRules, taskResults);
            }
            if (groupRules.Any())
            {
                result = result && groupRules.All(x => EvaluateOr(x.ToList(), taskResults));
            }
            return result;
        }

        public virtual bool EvaluateAnd(IList<TRule> rules, TInput input)
        {
            return rules?.Any() != true || rules.All(rule => EvaluateRule(rule, input));
        }

        public virtual bool EvaluateOr(IList<TRule> rules, TInput input)
        {
            return rules?.Any() != true || rules.Any(rule => EvaluateRule(rule, input));
        }

        protected abstract bool EvaluateRule(TRule rule, TInput input);
    }
}