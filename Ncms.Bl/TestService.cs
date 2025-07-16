using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using Ncms.Bl.Rubrics;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.Test;
using TestComponentModel = Ncms.Contracts.Models.Test.TestComponentModel;

namespace Ncms.Bl
{
    public class TestService : ITestService
    {
        private readonly IUserService _userService;
        private readonly ITestQueryService _testQueryService;
        private readonly ITestResultQueryService _testResultQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public TestService(IUserService userService, ITestQueryService testQueryService, ITestResultQueryService testResultQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _userService = userService;
            _testQueryService = testQueryService;
            _testResultQueryService = testResultQueryService;
            _autoMapperHelper = autoMapperHelper;
        }
        public GenericResponse<TestDataModel> GetTestSittingByCredentailRequestId(int credentialRequestId, bool supplementary)
        {
            var response =
                _testQueryService.GetTestSittingByCredentailRequestId(new GetTestSittingRequest
                {
                    CredentialRequestId = credentialRequestId,
                    Supplementary = supplementary
                });
            return new TestDataModel { TestAttendanceId = response.TestSitting?.TestSittingId };
        }
        public GenericResponse<AddExaminerResponseModel> AddExaminers(AddExaminerRequestModel request)
        {
            var existingExaminers = ServiceLocator.Resolve<IExaminerService>()
                 .GetTestExaminers(new GetJobExaminersRequestModel
                 {
                     IncludeExaminerMarkings = false,
                     TestAttendanceIds = request.TestDataModels.Select(x => x.TestAttendanceId.GetValueOrDefault())
                 });

            var examinersMembershipsIdsToAdd = request.Examiners.Select(x => new { x.PanelMembershipId, x.PaidReviewer });

            var repeatedExaminers = existingExaminers.Data.Where(x => examinersMembershipsIdsToAdd.Any(y => y.PanelMembershipId == x.Examiner.PanelMembershipId && y.PaidReviewer == x.PaidReviewer)).ToList();
            if (repeatedExaminers.Any())
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.Test.ExaminerAlreadyExists, string.Join(",", repeatedExaminers.Select(x => x.Examiner.PersonName))));
            }

            var examinerRequests = MapExaminerRequests(request.Examiners);

            var hasPaidReview = request.Examiners.Any(x => x.PaidReviewer);
            var testDatRequests = request.TestDataModels.Select(x => GetTestDataRequest(x, hasPaidReview, request.DueDate)).ToList();

            var response = _testQueryService.AddTestExaminers(new AddTestExaminersRequest { Examiners = examinerRequests, TestDataRequests = testDatRequests });
            return _autoMapperHelper.Mapper.Map<AddExaminerResponseModel>(response);
        }

        private TestDataRequest GetTestDataRequest(TestDataModel dataModel, bool hasPaidReview, DateTime dueDate)
        {
            var testSummary = GetTestSummary(dataModel.TestAttendanceId.GetValueOrDefault()).Data;
            var hasExistingPaidReview = dataModel.TestAttendanceId.HasValue && testSummary.LastReviewTestResultId != null;
            var createTestResult = (hasPaidReview && !hasExistingPaidReview) || !dataModel.JobId.HasValue;
            var dataRequest = new TestDataRequest
            {
                JobId = dataModel.JobId,
                TestAttendanceId = dataModel.TestAttendanceId,
                CreateTestResult = createTestResult,
                DueDate = dueDate,
                AllowCalculate = testSummary.MarkingSchemaTypeId == (int)MarkingSchemaTypeName.Standard,
                IncludePreviousMarks = testSummary.MarkingSchemaTypeId == (int)MarkingSchemaTypeName.Rubric,
                UserId = _userService.Get().Id,
                EligibleForSupplementary = testSummary.EligibleForSupplementary.GetValueOrDefault(),
                EligibleForConcededPass = testSummary.EligibleForConcededPass.GetValueOrDefault()
            };

            return dataRequest;
        }

        public GenericResponse<AddExaminerResponseModel> UpdateExaminers(UpdateExaminersRequestModel request)
        {
            var examinerRequests = MapExaminerRequests(request.Examiners);
            var response =
                _testQueryService.UpdateJobExaminers(new UpdateJobExaminersRequest { Examiners = examinerRequests });
            return _autoMapperHelper.Mapper.Map<AddExaminerResponseModel>(response);
        }

        private IEnumerable<ExaminerRequest> MapExaminerRequests(IEnumerable<ExaminerRequestModel> examiners)
        {
            var user = _userService.Get();
            return examiners.Select(x => new ExaminerRequest
            {
                JobExaminerId = x.JobExaminerId,
                PanelMemberShipId = x.PanelMembershipId,
                ThirdExaminer = x.ThirdExaminer,
                ExaminerSentDate = x.ExaminerSentDate,
                ExaminerSentUserId = x.ExaminerSentDateChanged ? user.Id : x.ExaminerSentUserId,
                ExaminerReceivedDate = x.ExaminerReceivedDate,
                ExaminerReceivedUserId = x.ExaminerReceivedDateChanged ? user.Id : x.ExaminerReceivedUserId,
                ExaminerToPayrollDate = x.ExaminerToPayrollDate,
                ExaminerToPayrollUserId = x.ExaminerToPayrollDateChanged ? user.Id : x.ExaminerToPayrollUserId,
                ExaminerCost = x.ExaminerCost,
                ExaminerPaperLost = x.ExaminerPaperLost,
                LetterRecipient = false,
                ExaminerPaperReceivedDate = x.ExaminerPaperReceivedDate,
                PaidReviewer = x.PaidReviewer,
                ProductSpecificationId = x.ProductSpecificationId,
                ProductSpecificationChangedDate = x.ProductSpecificationChanged ? DateTime.Now : x.ProductSpecificationChangedDate,
                ProductSpecificationChangedUserId = x.ProductSpecificationChanged ? user.Id : x.ProductSpecificationChangedUserId,
                ValidatedDate = null,
                ValidateUserId = null,
                PrimaryContact = true
            }).ToList();
        }

        public GenericResponse<IEnumerable<TestSearchResultModel>> List(TestSearchRequest request)
        {
            var getRequest = new GetTestSearchRequest
            {
                Take = request.Take,
                Skip = request.Skip,
                Filters = request.Filter.ToFilterList<TestSearchCriteria, TestFilterType>()
            };

            var result = _testQueryService.Search(getRequest);
            var models = result.Tests.Select(_autoMapperHelper.Mapper.Map<TestSearchResultModel>).ToList();

            var response = new GenericResponse<IEnumerable<TestSearchResultModel>>(models);

            if (request.Take.HasValue && models.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }

            return response;
        }

        public bool TestReadyForAssets(int testAttendanceId)
        {
            var readyResponse = false;
            readyResponse = _testQueryService.TestReadyForAssets(testAttendanceId);

            return readyResponse;
        }

        public GenericResponse<IEnumerable<VenueModel>> GetVenues(int testLocationId, bool activeOnly = false)
        {
            var result = _testQueryService.GetVenues(new GetVenuesRequest { TestLocationId = testLocationId, ActiveOnly = activeOnly });

            return new GenericResponse<IEnumerable<VenueModel>>(result.Result.Select(_autoMapperHelper.Mapper.Map<VenueModel>));
        }

        public GenericResponse<IEnumerable<VenueModel>> GetVenuesShowingInactive(int testLocationId, bool activeOnly = false)
        {
            var result = _testQueryService.GetVenuesShowingActive(new GetVenuesRequest { TestLocationId = testLocationId, ActiveOnly = activeOnly });

            return new GenericResponse<IEnumerable<VenueModel>>(result.Result.Select(_autoMapperHelper.Mapper.Map<VenueModel>));
        }

        public GenericResponse<VenueModel> GetVenueById(int venueId)
        {
            var result = _testQueryService.GetVenueById(venueId);
            return _autoMapperHelper.Mapper.Map<VenueModel>(result.Result);
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetAllVenues()
        {
            var serviceReponse = _testQueryService.GetVenues(new GetVenuesRequest { TestLocationId = null, ActiveOnly = false });

            var getAllVenues = new GenericResponse<IEnumerable<LookupTypeModel>>(serviceReponse.Result.Select(
                x => new LookupTypeModel
                {
                    Id = x.VenueId,
                    DisplayName = x.Name
                }));

            return getAllVenues;
        }

        public GenericResponse<TestSummaryModel> GetTestSummary(int testAttendanceId)
        {
            var response =
                _testQueryService.GetTestSummary(new GetTestSummaryRequest { TestAttendanceId = testAttendanceId });

            if (response.Result == null)
            {
                return null;
            }

            var model = _autoMapperHelper.Mapper.Map<TestSummaryModel>(response.Result);
            model.TestResultStatus = response.Result.LastTestResultStatus;
            model.TestResultStatusId = response.Result.LastTestResultStatusTypeId;
            model.Actions = ServiceLocator.Resolve<IApplicationWizardLogicService>()
                .GetValidCredentialRequestActions(response.Result.CredentialRequestStatusTypeId, response.Result.CredentialRequestId);
            return model;
        }

        public GenericResponse<TestRubricModel> GetTestResultRubricMarking(int testResultId)
        {
            var rubricResult = _testResultQueryService.GetTestResultMarkingResult(
                    new TestMarkingResultRequest { TestResultId = testResultId });

            var testDetails = _testQueryService.GetTestSummary(new GetTestSummaryRequest { TestAttendanceId = rubricResult.AttendanceId }).Result;

            var rubricModel = _autoMapperHelper.Mapper.Map<TestRubricModel>(rubricResult);

            rubricModel = _autoMapperHelper.Mapper.Map(testDetails, rubricModel);

            ComputeRubricResultEligibility(rubricModel, rubricResult.TestComponents, rubricResult.TestSpecificationId);

            return rubricModel;
        }

        public GenericResponse<TestRubricModel> GetExaminerRubricMarking(int jobExaminerId)
        {
            var rubricDto =
                _testResultQueryService.GetJobExaminerMarkingResult(
                    new JobExaminerMarkingResultRequest { JobExaminerId = jobExaminerId });

            var testDetails = _testQueryService.GetTestSummary(new GetTestSummaryRequest { TestAttendanceId = rubricDto.AttendanceId }).Result;
            
            var rubricModel = _autoMapperHelper.Mapper.Map<TestRubricModel>(rubricDto);

            rubricModel = _autoMapperHelper.Mapper.Map(testDetails, rubricModel);

            ComputeRubricResultEligibility(rubricModel, rubricDto.TestComponents, rubricDto.TestSpecificationId);

            return rubricModel;
        }

        public void SaveExaminerRubricMarking(TestRubricModel model)
        {
            var maxLength = 2000;
            ValidateMarks(model.TestComponents, maxLength);

            var markingDto = _autoMapperHelper.Mapper.Map<JobExaminerMarkingDto>(model);

            if (model.ResultAutoCalculation)
            {
                ComputeRubricQuestionResults(markingDto.TestComponents, model.AttendanceId);
            }

            var request = new SaveExaminerMarkingRequest
            {
                JobExaminerMarking = markingDto
            };

            if (request.JobExaminerMarking?.ReceivedDate == null && request.JobExaminerMarking?.SubmittedDate != null)
            {
                request.JobExaminerMarking.SubmittedDate = null;
            }

            var response = _testResultQueryService.SaveJobExaminerMarkingResult(request);
            if (!String.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }
        }

        public void SaveTestResultRubricMarking(TestRubricModel model)
        {
            var maxLength = 4000;
            ValidateMarks(model.TestComponents, maxLength);

            var markingDto = _autoMapperHelper.Mapper.Map<TestResultMarkingDto>(model);

            ComputeRubricQuestionResults(markingDto.TestComponents, markingDto.AttendanceId);

            var request = new SaveTestMarkingRequest
            {
                TestResultMarking = markingDto,
                UserId = _userService.Get().Id
            };

            var response = _testResultQueryService.SaveTestMarkingResult(request);
            if (!String.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }
        }

        public GenericResponse<bool> CheckIfAllowSupplementaryTest(int credentialRequestId)
        {
            var attendanceId = GetLastTestAttendanceId(credentialRequestId);
            var searchRequest = new GetTestSearchRequest
            {
                Filters = new[]
                {
                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.AttendanceIdIntList,
                        Values = new[] { attendanceId.ToString() }
                    },

                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.AllowSupplementaryTestBoolean,
                        Values = new[] {true.ToString()}
                    },
                }
            };

            var response = _testQueryService.Search(searchRequest).Tests;
            return response.Any();
        }


        private int GetLastTestAttendanceId(int credentialRequestId)
        {
            var searchRequest = new GetTestSearchRequest
            {
                Filters = new[]
                {
                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.CredentialRequestIdIntList,
                        Values = new[] { credentialRequestId.ToString() }
                    },
                }
            };
            return _testQueryService.Search(searchRequest).Tests.OrderByDescending(x => x.AttendanceId).FirstOrDefault()?.AttendanceId ?? 0;
        }
        public GenericResponse<bool> CheckIfAllowConcededPass(int credentialRequestId)
        {
            var attendanceId = GetLastTestAttendanceId(credentialRequestId);
            var searchRequest = new GetTestSearchRequest
            {
                Filters = new[]
                {
                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.AttendanceIdIntList,
                        Values = new[] { attendanceId.ToString() }
                    },

                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.AllowConcededPassBoolean,
                        Values = new[] {true.ToString()}
                    },
                }
            };

            var response = _testQueryService.Search(searchRequest).Tests;
            return response.Any();
        }


        public GenericResponse<bool> CheckIfAllowPaidTestReview(int credentialRequestId)
        {
            var attendanceId = GetLastTestAttendanceId(credentialRequestId);
            var searchRequest = new GetTestSearchRequest
            {
                Filters = new[]
                {
                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.AttendanceIdIntList,
                        Values = new[] { attendanceId.ToString() }
                    },

                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.AllowPaidReviewBoolean,
                        Values = new[] {true.ToString()}
                    },
                }
            };

            var response = _testQueryService.Search(searchRequest).Tests.Any();
            return response;
        }


        private void ValidateMarks(IEnumerable<TestComponentModel> testcomponents, int maxLength)
        {
            foreach (var testComponent in testcomponents)
            {
                if (testComponent.Competencies.SelectMany(x => x.Assessments).Any(a => (a.Comment ?? string.Empty).Length > maxLength))
                {
                    throw new UserFriendlySamException($"Comments can not be longer than {maxLength} characters");
                }
            }
        }

        public GenericResponse<TestDocumentsModel> GetTestDocuments(int testId)
        {
            throw new NotImplementedException();
        }

        public GenericResponse<TestPaidReviewModel> GetTestPaidReview(int testId)
        {
            throw new NotImplementedException();
        }

        public GenericResponse<TestAssetsModel> GetTestAssets(int testId)
        {
            throw new NotImplementedException();
        }

        public GenericResponse<TestNotesModel> GetTestNotes(int testId)
        {
            throw new NotImplementedException();
        }

        public void ComputeRubricResults(int jobExaminerId)
        {
            JobExaminerMarkingDto markingDto = _testResultQueryService.GetJobExaminerMarkingResult(new JobExaminerMarkingResultRequest
            {
                JobExaminerId = jobExaminerId
            });


            ComputeRubricQuestionResults(markingDto.TestComponents, markingDto.AttendanceId);

            var request = new SaveExaminerMarkingRequest
            {
                JobExaminerMarking = markingDto
            };

            var response = _testResultQueryService.SaveJobExaminerMarkingResult(request);
            if (!String.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }
        }

        public GenericResponse<TestRubricModel> ComputeFinalRubric(int testResultId)
        {
            var response = new GenericResponse<TestRubricModel>();
            var rubricResult = _testResultQueryService.GetTestResultMarkingResult(
                    new TestMarkingResultRequest { TestResultId = testResultId });

            IEnumerable<JobExaminerMarkingDto> markingResults = _testResultQueryService.GetAllExaminerMarkingResults(testResultId, true).Data.ToList();

            if (!markingResults.Any() || markingResults.Count() > 2)
            {
                throw new UserFriendlySamException(Naati.Resources.Test.OneOrTwoExaminerRequired);
            }

            var totalExaminers = markingResults.Count();
            var testComponents = new List<TestComponentModel>();

            foreach (var examinerId in markingResults.Select(x => x.JobExaminerId))
            {
                var examinerComponents = GetExaminerRubricMarking(examinerId).Data.TestComponents;
                testComponents.AddRange(examinerComponents);
            }

            var groupedComponents = testComponents
                .GroupBy(x => x.Id)
                .ToList();

            foreach (var componentGroup in groupedComponents)
            {
                if (componentGroup.Count() != totalExaminers)
                {
                    throw new UserFriendlySamException(Naati.Resources.Test.RubricMismatch);
                }

                var firstComponent = componentGroup.First();
                for (var i = 1; i < totalExaminers; i++)
                {
                    var nextComponent = componentGroup.ElementAt(i);
                    if (firstComponent.WasAttempted != nextComponent.WasAttempted || firstComponent.Successful != nextComponent.Successful)
                    {
                        throw new UserFriendlySamException(Naati.Resources.Test.RubricMismatch);
                    }
                }
            }

            rubricResult.TestComponents = RubricResultHelper.ComputeFinalRubric(markingResults, rubricResult.TestComponents.ToList());

            ComputeRubricQuestionResults(rubricResult.TestComponents, rubricResult.AttendanceId);

            var testDetails = _testQueryService.GetTestSummary(new GetTestSummaryRequest { TestAttendanceId = rubricResult.AttendanceId }).Result;

            // a false pass occurs when both examiners fail a task, but the aggregation of the best bands from each examiner equate to a pass
            var falsePasses = groupedComponents
                .Where(x => x.First().Successful == false && rubricResult.TestComponents.Single(y => y.TestComponentId == x.Key).Successful == true)
                .ToList();

            if (falsePasses.Any())
            {
                var messagePart = $"task{(falsePasses.Count > 1 ? "s" : "")} {string.Join(" and ", falsePasses.Select(x => $"{x.First().TypeLabel}{x.First().Label}"))}";
                throw new UserFriendlySamException(String.Format(Naati.Resources.Test.RubricFalsePassDetected, $"{messagePart}"));
            }

            var rubricModel = _autoMapperHelper.Mapper.Map<TestRubricModel>(rubricResult);

            rubricModel = _autoMapperHelper.Mapper.Map(testDetails, rubricModel);

            ComputeRubricResultEligibility(rubricModel, rubricResult.TestComponents, rubricResult.TestSpecificationId);

            response.Data = rubricModel;
            return response;
        }

        public GenericResponse<FeedbackResponseModel> Feedback(int? testAttendanceId)
        {
            if (testAttendanceId.IsNull())
            {
                return new FeedbackResponseModel();
            }

           var response = _testQueryService.GetFeedback(testAttendanceId);

            if (!response.Success)
            {
                return new GenericResponse<FeedbackResponseModel>(null)
                {
                    Success = false,
                    Errors = response.Errors
                };
            }

            if (response.Data.IsNull() || !response.Data.ExaminerFeedback.Any())
            {
                return new GenericResponse<FeedbackResponseModel>(null);
            }

            var examinerFeedback = response.Data.ExaminerFeedback;

            var examinerFeedbackModels = new List<ExaminerFeedbackModel>();

            foreach(var feedback in examinerFeedback)
            {
                examinerFeedbackModels.Add(
                    new ExaminerFeedbackModel{
                        ExaminerName = feedback.ExaminerName,
                        NaatiNumber = feedback.NaatiNumber,
                        Feedback = feedback.Feedback
                    }
                );
            }

            var model = new FeedbackResponseModel()
            {
                ExaminerFeedback = examinerFeedbackModels
            };

            return model;
        }

        /// <summary>
        /// Calculates whether the test result is eligible for Pass, Conceded Pass, and Supplementary Test
        /// </summary>
        private void ComputeRubricResultEligibility(TestRubricModel rubricModel, IEnumerable<TestMarkingComponentDto> questionResults, int testSpecificationId)
        {
            var testSummary = _testQueryService.GetTestSummary(
                    new GetTestSummaryRequest() { TestAttendanceId = rubricModel.AttendanceId }).Result;
            var rules = _testResultQueryService.GetRubricRules(testSpecificationId).Data;

            if (rules.ResultAutoCalculationEnabled && (rules.TestResultBandRules.Any() || rules.TestResultQuestionRules.Any() || rules.QuestionPassRules.Any()))
            {
                var resultHelper = new RubricResultHelper(questionResults, rules, _autoMapperHelper);
                rubricModel.ComputedEligibleForPass = resultHelper.ComputeExaminerRubricResultEligibility(TestResultEligibilityTypeName.Pass);
                rubricModel.ComputedEligibleForConcededPass = !rubricModel.ComputedEligibleForPass && 
                    testSummary.HasDowngradePaths && resultHelper.ComputeExaminerRubricResultEligibility(TestResultEligibilityTypeName.ConcededPass);
                rubricModel.ComputedEligibleForSupplementary = !rubricModel.ComputedEligibleForPass && 
                    testSummary.AllowSupplementary && !testSummary.Supplementary && resultHelper.ComputeExaminerRubricResultEligibility(TestResultEligibilityTypeName.SupplementaryTest);
            }
        }

        /// <summary>
        /// Calculates whether each question in the result was successful or not
        /// </summary>
        private void ComputeRubricQuestionResults(IEnumerable<TestMarkingComponentDto> questionResults, int testSittingId)
        {
            var results = questionResults.ToList();
            var rules = _testResultQueryService.GetRubricRulesForTestSitting(testSittingId).Data;

            if (rules.ResultAutoCalculationEnabled && (rules.TestResultBandRules.Any() || rules.TestResultQuestionRules.Any() || rules.QuestionPassRules.Any()))
            {
                var resultHelper = new RubricResultHelper(results, rules,_autoMapperHelper);
                foreach (var questionResult in results)
                {
                    questionResult.Successful = resultHelper.ComputeQuestionPass(questionResult.TestComponentId);
                }
            }
        }
    }
}
