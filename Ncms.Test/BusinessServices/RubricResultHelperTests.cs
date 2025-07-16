using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.AutoMappingProfiles;
using Ncms.Bl.AutoMappingProfiles;
using Ncms.Bl.Rubrics;
using Ncms.Bl.Rubrics.Evaluators;
using Ncms.Bl.Rubrics.Rules;
using Ncms.Test.AutoMappingProfiles;
using Ncms.Test.Utils;
using Xunit;

namespace Ncms.Test.BusinessServices
{
    public class RubricResultHelperTests //: IClassFixture<AutoMapperFixture>
    {
        /// <summary>
        /// Tests the RubricQuestionBandRuleEvaluator class.
        /// </summary>
        [Theory]
        [MemberData(nameof(T01_CreateRubricQuestionBandRuleEvaluatorTestData))]
        public void T01_TestRubricQuestionBandRuleEvaluator(string testId, IList<RubricQuestionPassRule> rules, IList<RubricMarkingAssessmentCriterionDto> criteria, bool shouldPass)
        {
            // arrange
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            }; 

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);

            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);
            ServiceLocator.Configure(serviceLocator);
            var evaluator = new RubricQuestionBandRuleEvaluator();

            // act
            var pass = evaluator.Evaluate(rules, criteria, TestResultEligibilityTypeName.Pass);

            // assert
            Assert.True(shouldPass == pass, $"TestId: {testId}");
        }

        /// <summary>
        /// Tests the RubricTestBandRuleEvaluator class.
        /// </summary>
        [Theory]
        [MemberData(nameof(T02_CreateRubricTestBandRuleEvaluatorTestData))]
        public void T02_TestRubricTestBandRuleEvaluator(string testId, IList<RubricTestResultBandRule> rules, IList<TestMarkingComponentDto> markingResults, bool shouldPass)
        {
            // arrange
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };
            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);

            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);
            var evaluator = new RubricTestBandRuleEvaluator();

            // act
            var pass = evaluator.Evaluate(rules, markingResults, TestResultEligibilityTypeName.Pass);

            // assert
            Assert.True(shouldPass == pass, $"TestId: {testId}");
        }

        /// <summary>
        /// Runs the same evaluator test as T02, but executes them via the RubricResultHelper 
        /// </summary>
        [Theory]
        [MemberData(nameof(T02A_CreateRubricTestBandRuleHelperTestData))]       
        public void T02A_TestRubricTestBandRuleEvaluatorViaHelper(string testId, IList<TestResultBandRuleDto> rules, IList<TestMarkingComponentDto> markingResults, bool shouldPass)
        {
            // arrange
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };
            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);

            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);

            var helper = new RubricResultHelper(markingResults, new RubricPassRulesDto
                                                                {
                                                                    QuestionPassRules = new List<QuestionPassRuleDto>(),
                                                                    TestResultBandRules = rules,
                                                                    TestResultQuestionRules = new List<TestResultQuestionRuleDto>(),
                                                                }, autoMapperHelper);

            // act
            var pass = helper.ComputeExaminerRubricResultEligibility(TestResultEligibilityTypeName.Pass);

            // assert
            Assert.True(shouldPass == pass, $"TestId: {testId}");
        }

        /// <summary>
        /// Tests the RubricTestQuestionRuleEvaluator class.
        /// </summary>
        [Theory]
        [MemberData(nameof(T03_CreateRubricTestQuestionRuleEvaluatorTestData))]
        public void T03_TestRubricTestQuestionRuleEvaluator(string testId, IList<RubricTestResultQuestionRule> rules, IList<(int TaskTypeId, bool Attempted, bool Passed)> taskResults, bool shouldPass)
        {
            // arrange
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);
         
            var evaluator = new RubricTestQuestionRuleEvaluator();

            // act
            var pass = evaluator.Evaluate(rules, taskResults, TestResultEligibilityTypeName.Pass);

            // assert
            Assert.True(shouldPass == pass, $"TestId: {testId}");
        }

        public static IEnumerable<object[]> T01_CreateRubricQuestionBandRuleEvaluatorTestData()
        {
            return new[]
                   {
                       // 1 criterion - fail
                       new object[] 
                       {
                           "01",
                           new[] { GetQuestionPassRule(1, 2) },
                           new[] { GetCriterionResult_(1, 3) },
                           false
                       },
                       // 1 criterion - pass
                       new object[] 
                       {
                           "02",
                           new[] { GetQuestionPassRule(1, 3) },
                           new[] { GetCriterionResult_(1, 3) },
                           true
                       },
                       // 2 criteria, no grouping - fail 
                       new object[]
                       {
                           "03",
                           new[] { GetQuestionPassRule(1, 2), GetQuestionPassRule(2, 2) },
                           new[] { GetCriterionResult_(1, 3), GetCriterionResult_(2, 2) },
                           false
                       },
                       // 2 criteria, no grouping - pass 
                       new object[]
                       {
                           "04",
                           new[] { GetQuestionPassRule(1, 3), GetQuestionPassRule(2, 2) },
                           new[] { GetCriterionResult_(1, 3), GetCriterionResult_(2, 2) },
                           true
                       },
                       // 2 criteria, grouped (both fail) - fail
                       new object[]
                       {
                           "05",
                           new[] { GetQuestionPassRule(1, 2, "1"), GetQuestionPassRule(2, 2, "1") },
                           new[] { GetCriterionResult_(1, 3),      GetCriterionResult_(2, 3) },
                           false
                       },
                       // 2 criteria, grouped (one pass, one fail) - pass
                       new object[]
                       {
                           "06",
                           new[] { GetQuestionPassRule(1, 2, "1"), GetQuestionPassRule(2, 2, "1") },
                           new[] { GetCriterionResult_(1, 3),      GetCriterionResult_(2, 2) },
                           true
                       },
                       // 2 criteria ungrouped and 2 criteria grouped - pass 
                       new object[]
                       {
                           "07",
                           new[] { GetQuestionPassRule(1, 3), GetQuestionPassRule(2, 2), GetQuestionPassRule(3, 2, "1"), GetQuestionPassRule(4, 2, "1") },
                           new[] { GetCriterionResult_(1, 3), GetCriterionResult_(2, 2), GetCriterionResult_(3, 3),      GetCriterionResult_(4, 2) },
                           true
                       },
                       // 2 criteria ungrouped and 2 criteria grouped - fail due to ungrouped rule
                       new object[]
                       {
                           "08",
                           new[] { GetQuestionPassRule(1, 3), GetQuestionPassRule(2, 2), GetQuestionPassRule(3, 2, "1"), GetQuestionPassRule(4, 2, "1") },
                           new[] { GetCriterionResult_(1, 3), GetCriterionResult_(2, 3), GetCriterionResult_(3, 3),      GetCriterionResult_(4, 2) },
                           false
                       },
                       // 2 criteria ungrouped and 2 criteria grouped - fail due to grouped rule
                       new object[]
                       {
                           "09",
                           new[] { GetQuestionPassRule(1, 3), GetQuestionPassRule(2, 2), GetQuestionPassRule(3, 2, "1"), GetQuestionPassRule(4, 2, "1") },
                           new[] { GetCriterionResult_(1, 3), GetCriterionResult_(2, 2), GetCriterionResult_(3, 3),      GetCriterionResult_(4, 3) },
                           false
                       },
                       // 4 criteria in 2 groups - pass
                       new object[]
                       {
                           "10",
                           new[] { GetQuestionPassRule(1, 1, "1"), GetQuestionPassRule(2, 2, "1"), GetQuestionPassRule(3, 3, "2"), GetQuestionPassRule(4, 4, "2") },
                           new[] { GetCriterionResult_(1, 2),      GetCriterionResult_(2, 2),      GetCriterionResult_(3, 3),      GetCriterionResult_(4, 3) },
                           true
                       },
                       // 4 criteria in 2 groups - fail
                       new object[]
                       {
                           "11",
                           new[] { GetQuestionPassRule(1, 1, "1"), GetQuestionPassRule(2, 2, "1"), GetQuestionPassRule(3, 3, "2"), GetQuestionPassRule(4, 4, "2") },
                           new[] { GetCriterionResult_(1, 2),      GetCriterionResult_(2, 3),      GetCriterionResult_(3, 3),      GetCriterionResult_(4, 3) },
                           false
                       },
                   };
        }

        public static IEnumerable<object[]> T02_CreateRubricTestBandRuleEvaluatorTestData()
        {
            return new[]
                   {
                       // 1 question, 1 criterion - fail due to band level
                       new object[]
                       {
                           "01",
                           new[] { GetTestResultBandRule(1, 1, 1, 1) },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 2)
                                                          })
                           },
                           false
                       },

                       // 1 question, 1 criterion - pass
                       new object[]
                       {
                           "02",
                           new[] { GetTestResultBandRule(1, 1, 1, 1) },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1)
                                                          })
                           },
                           true
                       },

                       // 1 question, 1 criterion - fail due to unattempted question
                       new object[]
                       {
                           "03",
                           new[] { GetTestResultBandRule(1, 1, 1, 1) },
                           new[]
                           {
                               GetQuestionResult(1, false, null)
                           },
                           false
                       },

                       // 2 question, 1 criterion - fail due to too few successful questions
                       new object[]
                       {
                           "04",
                           new[] { GetTestResultBandRule(1, 1, 1, 2) },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1) // pass
                                                          }),
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 2) // fail
                                                          }),
                           },
                           false
                       },

                       // 2 question, 1 criterion - pass
                       new object[]
                       {
                           "05",
                           new[] { GetTestResultBandRule(1, 1, 1, 2) },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1) // pass
                                                          }),
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1) // pass
                                                          }),
                           },
                           true
                       },

                       // 2 question (1 required to pass), 1 criterion - pass
                       new object[]
                       {
                           "06",
                           new[] { GetTestResultBandRule(1, 1, 1, 1) },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 2) // fail
                                                          }),
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1) // pass
                                                          }),
                           },
                           true
                       },

                       // 2 tasks, 2 rules - fail because one rule fails
                       new object[]
                       {
                           "07",
                           new[]
                           {
                               GetTestResultBandRule(1, 1, 2, 2),
                               GetTestResultBandRule(2, 1, 2, 2),
                           },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 2) // pass
                                                          }),
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1) // pass
                                                          }),
                               GetQuestionResult(2, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 2) // pass
                                                          }),
                               GetQuestionResult(2, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 3) // fail
                                                          }),
                           },
                           false
                       },

                       // 1 question, 2 critera - fail
                       new object[]
                       {
                           "08",
                           new[]
                           {
                               GetTestResultBandRule(1, 1, 1, 1),
                               GetTestResultBandRule(1, 2, 1, 1),
                           },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1), // pass
                                                              GetCriterionResult_(2, 2), // fail
                                                          }),
                           },
                           false
                       },

                       // 1 question, 2 criteria, grouped - pass
                       new object[]
                       {
                           "09",
                           new[]
                           {
                               GetTestResultBandRule(1, 1, 1, 1, "1"),
                               GetTestResultBandRule(1, 2, 1, 1, "1"),
                           },
                           new[]
                           {
                               GetQuestionResult(1, true, new[]
                                                          {
                                                              GetCriterionResult_(1, 1), // pass
                                                              GetCriterionResult_(2, 2), // fail
                                                          }),
                           },
                           true // because only one rule in the group has to pass
                       },

                       // based on the rules for the Ethics test - fail
                       new object[]
                       {
                           "10",
                           new[]
                           {
                               GetTestResultBandRule(12, 19, 2, 3),
                               GetTestResultBandRule(12, 20, 3, 3),
                               GetTestResultBandRule(12, 21, 3, 5),
                               GetTestResultBandRule(12, 22, 3, 5),
                           },
                           new[]
                           {
                               // question 1
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 2
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 3
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 4
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 5
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 4), // FAIL
                                                           }),
                           },
                           false // because criteria 22 requires 5 correct, but there are only 4
                       },

                       // based on the rules for the Ethics test - pass
                       new object[]
                       {
                           "11",
                           new[]
                           {
                               GetTestResultBandRule(12, 19, 2, 3),
                               GetTestResultBandRule(12, 20, 3, 3),
                               GetTestResultBandRule(12, 21, 3, 5),
                               GetTestResultBandRule(12, 22, 3, 5),
                           },
                           new[]
                           {
                               // question 1
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 4), // FAIL
                                                               GetCriterionResult_(20, 4), // FAIL
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 2
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 4), // FAIL
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 3
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 4), // FAIL
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 4
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                               // question 5
                               GetQuestionResult(12, true, new[]
                                                           {
                                                               GetCriterionResult_(19, 1), // pass
                                                               GetCriterionResult_(20, 2), // pass
                                                               GetCriterionResult_(21, 3), // pass
                                                               GetCriterionResult_(22, 3), // pass
                                                           }),
                           },
                           true
                       },
                   };
        }

        public static IEnumerable<object[]> T02A_CreateRubricTestBandRuleHelperTestData()
        {
            return T02_CreateRubricTestBandRuleEvaluatorTestData()
                .Select(itemIn => new[]
                                  {
                                      itemIn[0],
                                      (itemIn[1] as RubricTestResultBandRule[]).Select(MapTestResultBandRuleDto).ToArray(),
                                      itemIn[2],
                                      itemIn[3],
                                  }).ToList();
        }

        private static TestResultBandRuleDto MapTestResultBandRuleDto(RubricTestResultBandRule model)
        {
            return new TestResultBandRuleDto
            {
                MaximumBandLevel = model.MaximumBandLevel,
                NumberOfQuestions = model.NumberOfQuestions,
                RubricMarkingAssessmentCriterionId = model.RubricMarkingAssessmentCriterionId,
                RuleGroup = model.RuleGroup,
                TestComponentTypeId = model.TestComponentTypeId,
                TestResultEligibilityType = model.TestResultEligibilityType,
            };
        }

        public static IEnumerable<object[]> T03_CreateRubricTestQuestionRuleEvaluatorTestData()
        {
            return new[]
                   {
                       new object[]
                       {
                           // 1 pass required, 0 passed - fail
                           "01",
                           new[] { GetTestResultQuestionRule(1, 1, 1) },
                           new[] { (1, true, false), (2, true, true) },
                           false
                       },
                       new object[]
                       {
                           // 1 pass required, 0 attempted - fail
                           "02",
                           new[] { GetTestResultQuestionRule(1, 1, 1) },
                           new[] { (1, false, false), (2, true, true) },
                           false
                       },
                       new object[]
                       {
                           // 1 pass required, 1 passed - pass
                           "03",
                           new[] { GetTestResultQuestionRule(1, 1, 1) },
                           new[] { (1, true, true), (2, true, true) },
                           true
                       },
                       new object[]
                       {
                           // 2 pass required, 2 attempted, 1 failed - fail
                           "04",
                           new[] { GetTestResultQuestionRule(1, 2, 2) },
                           new[] { (1, true, true), (1, true, false), (2, true, true) },
                           false
                       },
                       new object[]
                       {
                           // 2 pass required, 2 attempted, 2 passed - pass
                           "05",
                           new[] { GetTestResultQuestionRule(2, 2, 2) },
                           new[] { (1, false, false), (2, true, true), (2, true, true) },
                           true
                       },
                       new object[]
                       {
                           // 2 pass required, 3 attempted, 1 passed - fail
                           "06",
                           new[] { GetTestResultQuestionRule(2, 2, 2) },
                           new[] { (2, true, false), (2, true, false), (2, true, true) },
                           false
                       },
                       new object[]
                       {
                           // 2 pass required, 3 attempted, 2 passed - pass
                           "07",
                           new[] { GetTestResultQuestionRule(2, 2, 2) },
                           new[] { (1, true, true), (2, true, true), (3, false, false), (2, true, false), (2, true, true) },
                           true
                       },
                       new object[]
                       {
                           // 0 pass required, 2 attempted required, 1 attempted - fail
                           "08",
                           new[] { GetTestResultQuestionRule(2, 2, 0) },
                           new[] { (1, true, true), (2, true, true), (3, false, false), (2, false, false), (2, false, false) },
                           false
                       },
                       new object[]
                       {
                           // 0 pass required, 2 attempted required, 2 attempted - pass
                           "09",
                           new[] { GetTestResultQuestionRule(2, 2, 0) },
                           new[] { (1, true, true), (2, true, true), (3, false, false), (2, false, false), (2, true, false) },
                           true
                       },
                   };
        }

        private static readonly List<RubricMarkingBandDto> Bands = new List<RubricMarkingBandDto>
                        {
                            new RubricMarkingBandDto { BandId = 101, Level = 1 },
                            new RubricMarkingBandDto { BandId = 102, Level = 2 },
                            new RubricMarkingBandDto { BandId = 103, Level = 3 },
                            new RubricMarkingBandDto { BandId = 104, Level = 4 },
                            new RubricMarkingBandDto { BandId = 105, Level = 5 },
                        };
        
        private static RubricMarkingAssessmentCriterionDto GetCriterionResult_(int criterionId, int bandLevel)
        {
            return new RubricMarkingAssessmentCriterionDto
                   {
                       AssessmentCriterionId = criterionId,
                       Bands = Bands,
                       SelectedBandId = Bands.Single(x => x.Level == bandLevel).BandId
                   };
        }

        private static TestMarkingComponentDto GetQuestionResult(int taskTypeId, bool attemped, IEnumerable<RubricMarkingAssessmentCriterionDto> criteria)
        {
            return new TestMarkingComponentDto
            {
                TestComponentTypeId = taskTypeId,
                RubricMarkingCompentencies = new List<RubricMarkingCompetencyDto>
                                                    {
                                                        new RubricMarkingCompetencyDto
                                                        {
                                                            RubricMarkingAssessmentCriteria = criteria ?? new List<RubricMarkingAssessmentCriterionDto>(),
                                                        }
                                                    },
                WasAttempted = attemped
            };
        }

        private static RubricQuestionPassRule GetQuestionPassRule(int criterionId, int maxBandLevel, string group = null)
        {
            return new RubricQuestionPassRule
                   {
                       RubricMarkingAssessmentCriterionId = criterionId,
                       TestResultEligibilityType = TestResultEligibilityTypeName.Pass,
                       MaximumBandLevel = maxBandLevel,
                       RuleGroup = group
                   };
        }

        private static RubricTestResultBandRule GetTestResultBandRule(int taskTypeId, int criterionId, int maxBandLevel, int numberOfQuestions, string group = null)
        {
            return new RubricTestResultBandRule
            {
                TestComponentTypeId = taskTypeId,
                RubricMarkingAssessmentCriterionId = criterionId,
                TestResultEligibilityType = TestResultEligibilityTypeName.Pass,
                MaximumBandLevel = maxBandLevel,
                NumberOfQuestions = numberOfQuestions,
                RuleGroup = group,
            };
        }

        private static RubricTestResultQuestionRule GetTestResultQuestionRule(int taskTypeId, int minQuestionsAttempted, int minQuestionsPassed, string group = null)
        {
            return new RubricTestResultQuestionRule
                   {
                       TestComponentTypeId = taskTypeId,
                       TestResultEligibilityType = TestResultEligibilityTypeName.Pass,
                       MinimumQuestionsAttempted = minQuestionsAttempted,
                       MinimumQuestionsPassed = minQuestionsPassed,
                       RuleGroup = group
                   };
        }
    }
}