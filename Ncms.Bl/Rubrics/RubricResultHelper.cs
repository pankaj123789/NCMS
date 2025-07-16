using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Bl.Rubrics.Evaluators;
using Ncms.Bl.Rubrics.Rules;

namespace Ncms.Bl.Rubrics
{
    public class RubricResultHelper
    {
        private readonly IList<TestMarkingComponentDto> _markingResults;
        private readonly RubricPassRulesDto _rules;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IDictionary<int, (TestMarkingComponentDto Question, bool Attempted, bool Passed)> _questionResults;

        public RubricResultHelper(IEnumerable<TestMarkingComponentDto> markingResults, RubricPassRulesDto rules, IAutoMapperHelper autoMapperHelper)
        {
            markingResults.ArgNotNull(nameof(markingResults));
            rules.ArgNotNull(nameof(rules));

            _markingResults = markingResults.ToList();
            _rules = rules;
            _autoMapperHelper = autoMapperHelper;

            _questionResults = ComputeQuestionResults();
        }

        public bool ComputeQuestionPass(int testComponentId)
        {
            // questions are already computed, so just return the result
            return _questionResults.ContainsKey(testComponentId) && _questionResults[testComponentId].Passed;
        }

        public bool ComputeExaminerRubricResultEligibility(TestResultEligibilityTypeName eligibilityType)
        {
            var testQuestionRules = _rules.TestResultQuestionRules
                .Select(_autoMapperHelper.Mapper.Map<RubricTestResultQuestionRule>)
                .ToList();

            var testBandRules = _rules.TestResultBandRules
                .Select(_autoMapperHelper.Mapper.Map<RubricTestResultBandRule>)
                .ToList();

            var questionResults =
                _questionResults.Values
                    .Select(x => (x.Question.TestComponentTypeId, x.Attempted, x.Passed));

            return new RubricTestBandRuleEvaluator().Evaluate(testBandRules, _markingResults, eligibilityType) &&
                   new RubricTestQuestionRuleEvaluator().Evaluate(testQuestionRules, questionResults.ToList(), eligibilityType);
        }

        private IDictionary<int, (TestMarkingComponentDto Question, bool Attempted, bool Passed)> ComputeQuestionResults()
        {
            var results = new Dictionary<int, (TestMarkingComponentDto Question, bool Attempted, bool Passed)>();

            var questionPassRules = _rules.QuestionPassRules
                .Select(_autoMapperHelper.Mapper.Map<RubricQuestionPassRule>)
                .ToList();

            var questionRuleEvaluator = new RubricQuestionBandRuleEvaluator();

            foreach (var question in _markingResults)
            {
                var criteria = question.RubricMarkingCompentencies.SelectMany(x => x.RubricMarkingAssessmentCriteria).ToList();
                var criteriaIds = criteria.Select(y => y.AssessmentCriterionId);
                var questionRules = questionPassRules.Where(x => criteriaIds.Contains(x.RubricMarkingAssessmentCriterionId)).ToList();
                results[question.TestComponentId] =
                    (question, question.WasAttempted == true, questionRuleEvaluator.Evaluate(questionRules, criteria, TestResultEligibilityTypeName.Pass));
            }

            return results;
        }

        /// <summary>
        /// Given multiple rubric sets, returns a single set with the best bands selected.
        /// </summary>
        public static IEnumerable<TestMarkingComponentDto> ComputeFinalRubric(IEnumerable<JobExaminerMarkingDto> markings, IList<TestMarkingComponentDto> finalMarkings){
            var taskGroups = markings.SelectMany(x => x.TestComponents)
                .GroupBy(x => x.TestComponentId);

            var result = new List<TestMarkingComponentDto>();

            foreach (var taskGroup in taskGroups)
            {
                var exitingFinalTask = finalMarkings?.FirstOrDefault(x => x.TestComponentId == taskGroup.Key);
                var finalCompentencies = new List<RubricMarkingCompetencyDto>();

                var sourceCompetencies = taskGroup.SelectMany(x => x.RubricMarkingCompentencies).ToList();
                foreach (var competencyGroup in sourceCompetencies.GroupBy(x => x.CompetencyId))
                {
                    var existingFinalCompetency =
                        exitingFinalTask?.RubricMarkingCompentencies.FirstOrDefault(x =>
                            x.CompetencyId == competencyGroup.Key);

                    var finalCompetency = new RubricMarkingCompetencyDto
                    {
                        CompetencyId = competencyGroup.First().CompetencyId,
                        DisplayOrder = competencyGroup.First().CompetencyId,
                        Label = competencyGroup.First().Label,
                        Name = competencyGroup.First().Name,
                        RubricMarkingAssessmentCriteria = new List<RubricMarkingAssessmentCriterionDto>(),
                    };

                    var competencyCriteriaGroups = competencyGroup.SelectMany(x => x.RubricMarkingAssessmentCriteria)
                        .GroupBy(x => x.AssessmentCriterionId);

                    var finalCriteria = new List<RubricMarkingAssessmentCriterionDto>();
                    foreach (var criteriaGroup in competencyCriteriaGroups)
                    {
                        var exitingFinalCriterion =
                            existingFinalCompetency?.RubricMarkingAssessmentCriteria.FirstOrDefault(x =>
                                x.AssessmentCriterionId == criteriaGroup.Key);

                        var notSelectedBand = criteriaGroup.Any(x => x.SelectedBandId == null);
                        var example = criteriaGroup.First();
                        var bands = example.Bands.ToList();

                        int? selectedBandId = null;
                        if (!notSelectedBand)
                        {
                            // the final band is the better (lowest level number) of all examiners
                            var bestBandLevel = bands
                                .Where(x => criteriaGroup
                                    .Select(y => y.SelectedBandId)
                                    .Contains(x.BandId))
                                .Min(y => y.Level);
                            selectedBandId = bands.Single(x => x.Level == bestBandLevel).BandId;
                        }

                        var finalCriterion = new RubricMarkingAssessmentCriterionDto
                        {
                            AssessmentCriterionId = example.AssessmentCriterionId,
                            Bands = bands,
                            Name = example.Name,
                            // set the selected band in the final result to the best band from the submitted results
                            SelectedBandId = selectedBandId,
                            Label = example.Label,
                            Comments = exitingFinalCriterion?.Comments ?? string.Empty
                        };
                        finalCriteria.Add(finalCriterion);
                    }

                    finalCompetency.RubricMarkingAssessmentCriteria = finalCriteria;
                    finalCompentencies.Add(finalCompetency);
                }

                var exampleTask = taskGroup.First();
                var finalTask = new TestMarkingComponentDto
                {
                    ComponentNumber = exampleTask.ComponentNumber,
                    GroupNumber = exampleTask.GroupNumber,
                    Label = exampleTask.Label,
                    Mark = exampleTask.Mark,
                    MarkingResultTypeId = exampleTask.MarkingResultTypeId,
                    MinExaminerCommentLength = exampleTask.MinExaminerCommentLength,
                    MinNaatiCommentLength = exampleTask.MinNaatiCommentLength,
                    Name = exampleTask.Name,
                    TestComponentId = exampleTask.TestComponentId,
                    TestComponentTypeId = exampleTask.TestComponentTypeId,
                    TypeDescription = exampleTask.TypeDescription,
                    TypeLabel = exampleTask.TypeLabel,
                    TypeName = exampleTask.TypeName,
                    RubricMarkingCompentencies = finalCompentencies,
                    WasAttempted = taskGroup.All(x => x.WasAttempted == true)
                };
                result.Add(finalTask);
            }

            return result;
        }
    }
}