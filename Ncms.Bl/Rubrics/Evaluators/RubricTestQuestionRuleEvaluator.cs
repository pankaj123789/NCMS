using System.Collections.Generic;
using System.Linq;
using Ncms.Bl.Rubrics.Rules;

namespace Ncms.Bl.Rubrics.Evaluators
{
    public class RubricTestQuestionRuleEvaluator : RubricRuleEvaluator<RubricTestResultQuestionRule, IList<(int TaskTypeId, bool Attempted, bool Passed)>>
    {
        protected override bool EvaluateRule(RubricTestResultQuestionRule rule, IList<(int TaskTypeId, bool Attempted, bool Passed)> taskResults)
        {
            // get the tasks which match the rule. if the rule doesn't have a task type, evaluate the rule against all tasks.
            var ruleTasks = taskResults.Where(x => rule.TestComponentTypeId == null || x.TaskTypeId == rule.TestComponentTypeId).ToList();

            // to pass the rule, number of questions attempted and number of questions passed must be greater than or equal to the required counts
            return ruleTasks.Count(x => x.Attempted) >= rule.MinimumQuestionsAttempted &&
                   ruleTasks.Count(x => x.Passed) >= rule.MinimumQuestionsPassed;
        }
    }
}