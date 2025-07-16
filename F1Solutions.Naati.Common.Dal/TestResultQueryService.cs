using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using MarkingSchemaTypeName = F1Solutions.Naati.Common.Dal.Domain.MarkingSchemaTypeName;

namespace F1Solutions.Naati.Common.Dal
{
    public class TestResultQueryService : ITestResultQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public TestResultQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public GetTestResultResponse GetTestResultById(int testResultId)
        {
            var result = NHibernateSession.Current.Load<TestResult>(testResultId);

            var dto = _autoMapperHelper.Mapper.Map<TestResultDto>(result);
            dto.DueDate = result.CurrentJob.DueDate;

            return new GetTestResultResponse { Result = dto };
        }


        private string ValidateRubricTestResultExaminers(TestResult currentResult)
        {
            var examiners = new List<string>();
            foreach (var examiner in currentResult.CurrentJob.JobExaminers.Where(x => x.ExaminerReceivedDate == null))
            {
                var examinerMarking = examiner.Markings.FirstOrDefault(m => m.CountMarks);
                if (examinerMarking != null)
                {
                    examiners.Add(examiner.PanelMembership.Person.FullName);
                }
            }

            if (examiners.Any())
            {
                return
                    $"Received Date has not been set for examiner(s) {string.Join(", ", examiners)}. All included examiners must have a Received Date.";
            }

            return null;
        }


        public SaveTestResultResponse UpdateTestResult(UpdateTestResultRequest request)
        {
            var testResultDto = request.TestResult;
            if (testResultDto.TestResultId == 0)
            {
                throw new Exception("Test Result Id was not defined.");
            }

            var currentResult = NHibernateSession.Current.Get<TestResult>(testResultDto.TestResultId);
            var response = new SaveTestResultResponse
            {
                TestResultId = currentResult.Id
            };

            var markingSchema = currentResult.TestSitting.TestSpecification.MarkingSchemaType();
            if (testResultDto.ResultChecked && markingSchema == MarkingSchemaTypeName.Rubric)
            {

                response.ErrorMessage = ValidateRubricResults(currentResult, request.MaxCommentLength);
                if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    return response;
                }
                response.ErrorMessage = ValidateRubricTestResultExaminers(currentResult);
                if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    return response;
                }
            }
            var updateMarks = testResultDto.AllowCalculate != currentResult.AllowCalculate;
            var testResult = _autoMapperHelper.Mapper.Map(testResultDto, currentResult);

            testResult.ResultType = NHibernateSession.Current.Get<ResultType>(testResultDto.ResultTypeId);

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.SaveOrUpdate(testResult);

                if (updateMarks)
                {
                    UpdateMarks(testResult, transaction);
                }

                transaction.Commit();
            }


            if (testResultDto.DueDate.GetValueOrDefault().Date != testResult.CurrentJob.DueDate.GetValueOrDefault().Date)
            {
                var updateDueDateRequest = new UpdateDueDateRequest
                {
                    DueDate = testResultDto.DueDate.GetValueOrDefault(),
                    JobIds = new List<int> { testResult.CurrentJob.Id }
                };

                var testQueryService = new TestQueryService(_autoMapperHelper);
                testQueryService.UpdateDueDate(updateDueDateRequest);
            }
            return response;
        }

        private void UpdateMarks(TestResult result, ITransaction transaction)
        {
            var examinerQueryService = new ExaminerQueryService(_autoMapperHelper);
            var examinerIds = result.ExaminerMarkings.Where(x => x.CountMarks).Select(y => y.JobExaminer.Id);
            examinerQueryService.UpdateCountMarks(new UpdateCountMarksRequest()
            {
                IncludePreviousMarks = result.IncludePreviousMarks,
                JobExaminersId = examinerIds.ToArray(),
                TestResultId = result.Id
            }, transaction);
        }

        private string ValidateExaminerRubricResults(TestResult currentResult, JobExaminer examiner, bool isExaminerRequest, int maxLength)
        {
            string validationError = null;

            Func<TestComponentType, int> minLengthFunction = testType => testType.MinNaatiCommentLength;
            if (isExaminerRequest)
            {
                minLengthFunction = testType => testType.MinExaminerCommentLength;
            }

            foreach (var testComponent in currentResult.TestSitting.TestSpecification.TestComponents)
            {
                var assessments = testComponent.Type.RubricMarkingCompetencies.SelectMany(a => a.RubricMarkingAssessmentCriteria);
                foreach (var assessment in assessments)
                {
                    var minLength = minLengthFunction(testComponent.Type);
                    validationError = ValidateMandatoryRubricResult(maxLength,
                        minLength, examiner.RubricTestComponentResults,
                        assessment, testComponent,
                        $"Please ensure that for all Attempted Tasks have bands selected and that each criterion contains comments (between {minLength} and {maxLength} characters).");
                    if (!string.IsNullOrWhiteSpace(validationError))
                    {
                        return validationError;
                    }
                }
            }

            return validationError;
        }



        private string ValidateRubricResults(TestResult currentResult, int maxCommentLength)
        {
            string validationError = null;
            foreach (var testComponent in currentResult.TestSitting.TestSpecification.TestComponents)
            {
                var minLength = testComponent.Type.MinNaatiCommentLength;
                var assessments = testComponent.Type.RubricMarkingCompetencies.SelectMany(a => a.RubricMarkingAssessmentCriteria);
                foreach (var assessment in assessments)
                {

                    validationError = ValidateMandatoryRubricResult(maxCommentLength, minLength, currentResult.RubricTestComponentResults, assessment, testComponent, $"Please select a band and enter a comment ( Min. {minLength}, Max. {maxCommentLength} characters) for each assessment in the attempted test tasks.");
                    if (!string.IsNullOrWhiteSpace(validationError))
                    {
                        return validationError;
                    }
                }
            }

            return validationError;
        }

        private static string ValidateMandatoryRubricResult(int maxLength, int minLength, IEnumerable<RubricTestComponentResult> rubricTestComponentResults, RubricMarkingAssessmentCriterion assessment, TestComponent testComponent, string selectedBandOrCommentNotFoundMessage)
        {
            var rubricTestComponentResult = rubricTestComponentResults?.FirstOrDefault(a => a.TestComponent.Id == testComponent.Id);
            if (rubricTestComponentResult == null)
            {
                return selectedBandOrCommentNotFoundMessage;
            }

            if (!rubricTestComponentResult.WasAttempted || rubricTestComponentResult.MarkingResultType.Id == (int)MarkingResultTypeName.FromOriginal)
            {
                return null;
            }

            var rubric = rubricTestComponentResult.RubricAssessementCriterionResults?.FirstOrDefault(r => r.RubricMarkingAssessmentCriterion.Id == assessment.Id);
            if (rubric == null || rubric.RubricMarkingBand == null || (rubric.Comments ?? String.Empty).Length < minLength)
            {
                return selectedBandOrCommentNotFoundMessage;
            }

            if ((rubric.Comments ?? String.Empty).Length > maxLength)
            {
                return selectedBandOrCommentNotFoundMessage;
            }

            if ((rubric.Comments ?? String.Empty).Length > maxLength)
            {
                return selectedBandOrCommentNotFoundMessage;
            }

            return null;
        }

        public DeleteDocumentResponse DeleteDocument(DeleteDocumentRequest request)
        {
            var document = NHibernateSession.Current.Load<TestSittingDocument>(request.TestSittingDocumentId);
            document.Deleted = true;
            NHibernateSession.Current.Save(document);
            return new DeleteDocumentResponse();
        }

        public GetTestSittingDocumentResponse GetDocument(GetDocumentRequest request)
        {
            var document = NHibernateSession.Current.Load<TestSittingDocument>(request.TestSittingDocumentId);
            return new GetTestSittingDocumentResponse
            {
                Document = MapDocument(document)
            };
        }

        public GetDocumentsResponse GetDocuments(GetDocumentsRequest request)
        {
            var documents = NHibernateSession.Current.Load<TestSitting>(request.TestSittingId).TestSittingDocuments;
            var documentTypeIds = request.DocumentTypes.ToList();
            if (documentTypeIds.Any())
            {
                documents = documents.Where(x => documentTypeIds.Contains((StoredFileType)x.StoredFile.DocumentType.Id)).ToList();
            }

            if (request.ExaminerDocuments != null)
            {
                documents = documents.Where(x => x.EportalDownload == request.ExaminerDocuments.Value).ToList();
            }

            return new GetDocumentsResponse
            {
                Documents = documents.Select(MapDocument).ToList()
            };
        }

        private TestSittingDocumentDto MapDocument(TestSittingDocument document)
        {
            var storedFile = document.StoredFile;
            var personName = storedFile.UploadedByPerson?.LatestPersonName.PersonName;

            return new TestSittingDocumentDto
            {
                TestSittingDocumentId = document.Id,
                StoredFileId = storedFile.Id,
                DocumentTypeId = storedFile.DocumentType.Id,
                FileSize = storedFile.FileSize,
                FileName = storedFile.FileName,
                Title = document.Title,
                DocumentTypeDisplayName = storedFile.DocumentType.DisplayName,
                UploadedByPersonName = storedFile.UploadedByPerson?.FullName,
                UploadedByUserName = storedFile.UploadedByUser?.FullName,
                UploadedDateTime = storedFile.UploadedDateTime,
                EportalDownload = document.EportalDownload,
                StoredFileStatusChangeDate = storedFile.StoredFileStatusChangeDate,
                StoredFileStatusTypeId = storedFile.StoredFileStatusType.Id
            };
        }

        public CreateOrUpdateDocumentResponse CreateOrUpdateDocument(CreateOrUpdateDocumentRequest request)
        {
            if (request.Document.Id.HasValue)
            {
                UpdateTestAttendanceAsset(request.Document);
            }
            else
            {
                CreateTestAttendanceAsset(request.Document);
            }

            return new CreateOrUpdateDocumentResponse();
        }

        public JobExaminerMarkingDto GetJobExaminerMarkingResult(JobExaminerMarkingResultRequest request)
        {
            var examiner = NHibernateSession.Current.Get<JobExaminer>(request.JobExaminerId);
            var testResult = examiner.Job.TestResults.First();
            var testSitting = testResult.TestSitting;
            var testSpecification = testSitting.TestSpecification;
            var testComponents = testSpecification.TestComponents.ToList();
            var testComponentDtos = testComponents.Select(MapComponent).ToList();

            var rubricResults = NHibernateSession.Current.Query<JobExaminerRubricTestComponentResult>()
                .Where(x => x.JobExaminer.Id == request.JobExaminerId).Select(x => x.RubricTestComponentResult).ToList();

            SetRubricResults(testComponentDtos, rubricResults);

            var finalResults = testResult.RubricTestComponentResults;

            foreach (var testComponentDto in testComponentDtos)
            {
                var componentResult = finalResults.FirstOrDefault(x => x.TestComponent.Id == testComponentDto.TestComponentId);
                testComponentDto.MarkingResultTypeId = componentResult?.MarkingResultType.Id;
                testComponentDto.ReadOnly = testComponentDto.MarkingResultTypeId == (int)MarkingResultTypeName.FromOriginal;
            }

            return new JobExaminerMarkingDto
            {
                TestSpecificationId = testSpecification.Id,
                JobExaminerId = examiner.Id,
                AttendanceId = testSitting.Id,
                ExaminerName = examiner.PanelMembership.Person.FullName,
                TestComponents = testComponentDtos,
                SubmittedDate = testResult.ExaminerMarkings.FirstOrDefault(x => x.JobExaminer.Id == request.JobExaminerId)?.SubmittedDate,
                ReceivedDate = testResult.CurrentJob.JobExaminers.FirstOrDefault(x => x.Id == request.JobExaminerId)?.ExaminerReceivedDate,
                TestResultStatusTypeId = testResult.ResultType.Id,
                ResultAutoCalculation = testSpecification.ResultAutoCalculation,
                Feedback = examiner.Feedback
            };
        }

        public JobExaminerMarkingDto GetExaminerMarkingResult(int naatiNumber, int testResultId)
        {
            var examinerJobId = NHibernateSession.Current.Get<TestResult>(testResultId).CurrentJobId;
            var examiner = NHibernateSession.Current.Get<Job>(examinerJobId)
                .JobExaminers.First(x => x.PanelMembership.Person.Entity.NaatiNumber == naatiNumber);

            return GetJobExaminerMarkingResult(
                new JobExaminerMarkingResultRequest
                {
                    JobExaminerId = examiner.Id
                });

        }

        public ServiceResponse<IEnumerable<JobExaminerMarkingDto>> GetAllExaminerMarkingResults(int testResultId, bool includedOnly)
        {
            var examinerJobId = NHibernateSession.Current.Get<TestResult>(testResultId).CurrentJobId;
            var examiners = NHibernateSession.Current.Query<ExaminerMarking>()
                .Where(x => x.JobExaminer.Job.Id == examinerJobId && (x.CountMarks == includedOnly || !includedOnly))
                .Select(y => y.JobExaminer);

            var dtos = examiners.Select(examiner => GetJobExaminerMarkingResult(
                new JobExaminerMarkingResultRequest
                {
                    JobExaminerId = examiner.Id
                }))
                .ToList();

            return new ServiceResponse<IEnumerable<JobExaminerMarkingDto>> { Data = dtos };
        }

        public TestResultMarkingDto GetTestResultMarkingResult(TestMarkingResultRequest request)
        {
            var testResult = NHibernateSession.Current.Get<TestResult>(request.TestResultId);
            var testComponents = testResult.TestSitting.TestSpecification.TestComponents;
            var testComponentDtos = testComponents.Select(MapComponent).ToList();
            var examinerResults = testResult.CurrentJob.JobExaminers.Where(x => x.Markings.FirstOrDefault(y => y.CountMarks) != null).ToList();

            if (testResult.IncludePreviousMarks && testResult.CurrentJob.ReviewFromJobId.HasValue)
            {
                var previousJob = NHibernateSession.Current.Get<Job>(testResult.CurrentJob.ReviewFromJobId);
                var previousExaminers = previousJob.JobExaminers.Where(x => x.Markings.FirstOrDefault(y => y.CountMarks) != null).ToList();

                examinerResults = examinerResults.Concat(previousExaminers).ToList();
            }

            var rubricResults = NHibernateSession.Current.Query<TestResultRubricTestComponentResult>()
                .Where(x => x.TestResult.Id == request.TestResultId).Select(x => x.RubricTestComponentResult).ToList();

            // SetTestComponentResults(testComponentDtos, testResult.TestComponentResults.ToList());
            SetRubricResults(testComponentDtos, rubricResults);
            SetExaminerResults(testComponentDtos, examinerResults);

            return new TestResultMarkingDto
            {
                TestSpecificationId = testResult.TestSitting.TestSpecification.Id,
                TestResultId = testResult.Id,
                AttendanceId = testResult.TestSitting.Id,
                TestComponents = testComponentDtos,
                TestResultStatusTypeId = testResult.ResultType.Id,
                ResultAutoCalculation = testResult.TestSitting.TestSpecification.ResultAutoCalculation
            };
        }

        public SaveExaminerMarkingResponse SaveJobExaminerMarkingResult(SaveExaminerMarkingRequest request)
        {
            var dataToUpsert = new List<object>();

            var examiner = GetExaminerToUpdate(request.JobExaminerMarking);

            if (examiner.Job.TestResults.First().ResultType.Id != (int)TestResultStatusTypeName.AwaitingResults)
            {
                return new SaveExaminerMarkingResponse { ErrorMessage = "The rubric result can't be modified because this test already has a result." };
            }

            examiner.Feedback = request.JobExaminerMarking.Feedback;

            dataToUpsert.Add(examiner);
            var examinerMarking = GetExaminerMarkingToUupdate(examiner, request.JobExaminerMarking);
            dataToUpsert.Add(examinerMarking);

            var examinerComponentResults = GetExaminerComponentResultToUpsert(examiner);
            dataToUpsert.AddRange(examinerComponentResults.Select(x => x.RubricTestComponentResult));
            dataToUpsert.AddRange(examinerComponentResults);

            SaveRubricResults(dataToUpsert, request.JobExaminerMarking.TestComponents,
                examinerComponentResults.Select(x => x.RubricTestComponentResult).ToList(), request.ClearNotAttempted);

            return new SaveExaminerMarkingResponse();

        }



        public SaveTestResultResponse SaveJobExaminerMarkingResultWithNaatiNumber(SaveExaminerMarkingRequest request)
        {
            var response = new SaveTestResultResponse();
            var testResult = NHibernateSession.Current.Get<TestResult>(request.JobExaminerMarking.TestResultId);

            var examinerJobId = testResult.CurrentJobId;
            var examiner = NHibernateSession.Current.Get<Job>(examinerJobId)
                .JobExaminers.First(x => x.PanelMembership.Person.Entity.NaatiNumber == request.JobExaminerMarking.NaatiNumber);

            request.JobExaminerMarking.JobExaminerId = examiner.Id;
            if (request.JobExaminerMarking.SubmittedDate != null)
            {
                response.ErrorMessage = ValidateExaminerRubricResults(testResult, examiner, request.IsExaminerRequest, request.MaxCommentsLength);
            }
            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                return response;
            }
            SaveJobExaminerMarkingResult(request);
            return response;
        }

        private void ClearNotAttemptedResults(IEnumerable<RubricTestComponentResult> rubricTestComponentResults)
        {
            foreach (var rubricTestComponentResult in rubricTestComponentResults)
            {
                if (!rubricTestComponentResult.WasAttempted)
                {
                    foreach (var criterionResult in rubricTestComponentResult.RubricAssessementCriterionResults)
                    {
                        criterionResult.Comments = string.Empty;
                        criterionResult.RubricMarkingBand = null;
                        criterionResult.CommentDate = null;
                    }
                }
            }
        }

        public SaveTestMarkingResponse SaveTestMarkingResult(SaveTestMarkingRequest request)
        {
            var dataToUpsert = new List<object>();

            var testResult = GetTestResultToUpdate(request.TestResultMarking);

            if (testResult.ResultType.Id != (int)TestResultStatusTypeName.AwaitingResults)
            {
                return new SaveTestMarkingResponse { ErrorMessage = "The rubric result can't be modified because this test already has a result." };
            }

            dataToUpsert.Add(testResult);

            var testResultComponentResults = GetTestComponentResultToUpsert(testResult);
            dataToUpsert.AddRange(testResultComponentResults.Select(x =>
            {
                var componentDto = request.TestResultMarking.TestComponents.FirstOrDefault(
                        y => y.TestComponentId == x.RubricTestComponentResult.TestComponent.Id);
                if (componentDto != null)
                {
                    x.ModifiedDate = request.TestResultMarking.ModifiedDate ?? DateTime.Now;
                    x.ModifiedUser = NHibernateSession.Current.Get<User>(request.UserId);
                }

                return x.RubricTestComponentResult;
            }));

            dataToUpsert.AddRange(testResultComponentResults);
            SaveRubricResults(dataToUpsert, request.TestResultMarking.TestComponents, testResultComponentResults.Select(x => x.RubricTestComponentResult).ToList(), request.ClearNotAttempted);

            return new SaveTestMarkingResponse();
        }

        private void SaveRubricResults(IList<object> dataToUpsert, IEnumerable<TestMarkingComponentDto> testComponentDtos, IList<RubricTestComponentResult> rubricComponentResults, bool clearNotAttemptedResults)
        {
            foreach (var component in testComponentDtos)
            {
                var rubricComponentResult = rubricComponentResults.First(x => x.TestComponent.Id == component.TestComponentId);
                rubricComponentResult.WasAttempted = component.WasAttempted.GetValueOrDefault();
                rubricComponentResult.Successful = component.Successful;

                foreach (var competency in component.RubricMarkingCompentencies)
                {
                    foreach (var assessmentCriterion in competency.RubricMarkingAssessmentCriteria)
                    {
                        var assessmentCriterionResult = GetAssessmentCriterionToUpsert(rubricComponentResult, assessmentCriterion);
                        if (assessmentCriterionResult != null)
                        {
                            dataToUpsert.Add(assessmentCriterionResult);
                        }

                    }
                }
            }

            if (clearNotAttemptedResults)
            {
                ClearNotAttemptedResults(rubricComponentResults);
            }
            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                Util.ForEach(dataToUpsert, NHibernateSession.Current.SaveOrUpdate);
                transaction.Commit();
            }
        }

        private JobExaminer GetExaminerToUpdate(JobExaminerMarkingDto request)
        {
            var examiner = NHibernateSession.Current.Get<JobExaminer>(request.JobExaminerId);
            examiner.ExaminerReceivedDate = request.ReceivedDate;
            return examiner;
        }

        private ExaminerMarking GetExaminerMarkingToUupdate(JobExaminer examiner, JobExaminerMarkingDto request)
        {
            var testResult = examiner.Job.TestResults.First();
            var marking = NHibernateSession.Current.Query<ExaminerMarking>()
                .FirstOrDefault(x => x.TestResult.Id == testResult.Id
                && x.JobExaminer.Id == examiner.Id) ?? new ExaminerMarking { TestResult = testResult, JobExaminer = examiner, Comments = string.Empty, ReasonsForPoorPerformance = string.Empty, PrimaryReasonForFailure = 0, Status = "S" };
            marking.SubmittedDate = request.SubmittedDate;

            return marking;
        }
        private TestResult GetTestResultToUpdate(TestResultMarkingDto request)
        {
            var testResult = NHibernateSession.Current.Get<TestResult>(request.TestResultId);
            return testResult;
        }

        private IList<JobExaminerRubricTestComponentResult> GetExaminerComponentResultToUpsert(JobExaminer examiner)
        {
            var results = examiner.JobExaminerRubricTestComponentResults.ToList();
            if (!results.Any())
            {
                var originalMarkingType =
                    NHibernateSession.Current.Get<MarkingResultType>((int)MarkingResultTypeName.Original);
                var testComponents = examiner.Job.TestResults.First().TestSitting.TestSpecification.TestComponents;
                foreach (var testComponent in testComponents)
                {
                    var examinerComponentResult = new JobExaminerRubricTestComponentResult
                    {
                        JobExaminer = examiner,
                        RubricTestComponentResult = new RubricTestComponentResult
                        {
                            TestComponent = testComponent,
                            MarkingResultType = originalMarkingType
                        }
                    };

                    results.Add(examinerComponentResult);
                }
            }

            return results;
        }



        private IList<TestResultRubricTestComponentResult> GetTestComponentResultToUpsert(TestResult testResult)
        {
            var results = testResult.TestResultRubricTestComponentResults.ToList();
            if (!results.Any())
            {
                var originalMarkingType =
                    NHibernateSession.Current.Get<MarkingResultType>((int)MarkingResultTypeName.Original);
                var testComponents = testResult.TestSitting.TestSpecification.TestComponents;
                foreach (var testComponent in testComponents)
                {
                    var examinerComponentResult = new TestResultRubricTestComponentResult
                    {
                        TestResult = testResult,
                        RubricTestComponentResult = new RubricTestComponentResult
                        {
                            TestComponent = testComponent,
                            MarkingResultType = originalMarkingType
                        }
                    };

                    results.Add(examinerComponentResult);
                }
            }

            return results;
        }

        private RubricAssessementCriterionResult GetAssessmentCriterionToUpsert(RubricTestComponentResult rubricComponentResult, RubricMarkingAssessmentCriterionDto assessmentCriterionDto)
        {
            var criterionResult = rubricComponentResult.RubricAssessementCriterionResults?.FirstOrDefault(
                x => x.RubricMarkingAssessmentCriterion.Id == assessmentCriterionDto.AssessmentCriterionId);

            if (criterionResult == null)
            {
                criterionResult = new RubricAssessementCriterionResult
                {
                    RubricTestComponentResult = rubricComponentResult,
                    RubricMarkingAssessmentCriterion =
                        NHibernateSession.Current.Get<RubricMarkingAssessmentCriterion>(assessmentCriterionDto.AssessmentCriterionId)
                };

            }

            var band = criterionResult.RubricMarkingAssessmentCriterion.RubricMarkingBands.FirstOrDefault(
                x => x.Id == assessmentCriterionDto.SelectedBandId.GetValueOrDefault());
            if (band == null && assessmentCriterionDto.SelectedBandId.GetValueOrDefault() > 0)
            {
                throw new Exception($"band {assessmentCriterionDto.SelectedBandId} was not found on criterion { criterionResult.RubricMarkingAssessmentCriterion.Id}");
            }
            criterionResult.RubricMarkingBand = NHibernateSession.Current.Get<RubricMarkingBand>(assessmentCriterionDto.SelectedBandId.GetValueOrDefault());
            criterionResult.Comments = assessmentCriterionDto.Comments;
            criterionResult.CommentDate = DateTime.Now;


            return criterionResult;
        }


        private void SetExaminerResults(IEnumerable<TestMarkingComponentDto> testComponentDtos, IList<JobExaminer> examiners)
        {
            foreach (var testComponentDto in testComponentDtos)
            {
                var componentResults = examiners.Select(a => new
                {
                    JobexaminerId = a.Id,
                    Initials = $"{a.PanelMembership.Person.GivenName?.FirstOrDefault()} {a.PanelMembership.Person.Surname?.FirstOrDefault()}",
                    componentResult = a.RubricTestComponentResults.FirstOrDefault(r => r.TestComponent.Id == testComponentDto.TestComponentId)
                }).Where(k => k.componentResult != null).ToList();

                foreach (var competencyDto in testComponentDto.RubricMarkingCompentencies)
                {
                    foreach (var criterionDto in competencyDto.RubricMarkingAssessmentCriteria)
                    {

                        var examinerResults = componentResults.Select(x => new
                        {
                            x.JobexaminerId,
                            x.Initials,
                            criterion = x.componentResult.RubricAssessementCriterionResults.FirstOrDefault(y => y.RubricMarkingAssessmentCriterion.Id == criterionDto.AssessmentCriterionId),
                        }).Where(z => z.criterion?.RubricMarkingBand?.Id != null && z.criterion.RubricTestComponentResult.WasAttempted);

                        criterionDto.ExaminerResults = examinerResults.Select(x => new ExaminerResultDto
                        {
                            JobExaminerId = x.JobexaminerId,
                            Initials = x.Initials,
                            BandLevel = x.criterion.RubricMarkingBand.Level,
                            Comment = x.criterion.Comments
                        }).ToList();

                    }
                }
            }
        }

        private void SetRubricResults(IEnumerable<TestMarkingComponentDto> testComponentDtos, IList<RubricTestComponentResult> results)
        {
            foreach (var testComponentDto in testComponentDtos)
            {
                var componentResult = results.FirstOrDefault(x => x.TestComponent.Id == testComponentDto.TestComponentId);

                testComponentDto.WasAttempted = componentResult?.WasAttempted;
                testComponentDto.Successful = componentResult?.Successful;
                testComponentDto.MarkingResultTypeId = componentResult?.MarkingResultType.Id;
                testComponentDto.ReadOnly = testComponentDto.MarkingResultTypeId == (int)MarkingResultTypeName.FromOriginal;
                testComponentDto.RubricTestComponentResultId = componentResult?.Id;

                var testComponentResultLookup = componentResult?.RubricAssessementCriterionResults.ToDictionary(x => x.RubricMarkingAssessmentCriterion.Id, y => y)
                    ?? new Dictionary<int, RubricAssessementCriterionResult>();

                foreach (var competencyDto in testComponentDto.RubricMarkingCompentencies)
                {
                    foreach (var criterionDto in competencyDto.RubricMarkingAssessmentCriteria)
                    {
                        RubricAssessementCriterionResult selectedResult;

                        if (testComponentResultLookup.TryGetValue(criterionDto.AssessmentCriterionId, out selectedResult))
                        {
                            criterionDto.SelectedBandId = selectedResult.RubricMarkingBand?.Id;
                            criterionDto.Comments = selectedResult.Comments;
                        }

                    }
                }
            }
        }

        public static TestMarkingComponentDto MapComponent(TestComponent component)
        {
            // automapper was causing some weird issues here, so we're doing it manually
            return new TestMarkingComponentDto
            {
                TestComponentId = component.Id,
                TestComponentTypeId = component.Type.Id,
                ComponentNumber = component.ComponentNumber,
                GroupNumber = component.GroupNumber,
                RubricMarkingCompentencies = GetRubricMarkingCompetencies(component),
                Name = component.Name,
                Label = component.Label,
                TypeName = component.Type.Name,
                TypeLabel = component.Type.Label,
                TypeDescription = component.Type.Description,
                PassMark = 1.0,
                TotalMarks = 1,
                MinExaminerCommentLength = component.Type.MinExaminerCommentLength,
                MinNaatiCommentLength = component.Type.MinNaatiCommentLength
            };
        }

        private static IEnumerable<RubricMarkingCompetencyDto> GetRubricMarkingCompetencies(TestComponent component)
        {
            return component.Type.RubricMarkingCompetencies
                .Select(x =>
                    new RubricMarkingCompetencyDto
                    {
                        CompetencyId = x.Id,
                        Name = x.Name,
                        Label = x.Label,
                        DisplayOrder = x.DisplayOrder,
                        RubricMarkingAssessmentCriteria = GetRubricMarkingCriteria(x),
                    })
                .ToList();
        }

        private static IEnumerable<RubricMarkingAssessmentCriterionDto> GetRubricMarkingCriteria(RubricMarkingCompetency competency)
        {
            return competency.RubricMarkingAssessmentCriteria
                .Select(x =>
                    new RubricMarkingAssessmentCriterionDto
                    {
                        AssessmentCriterionId = x.Id,
                        Name = x.Name,
                        Label = x.Label,
                        ExaminerResults = Enumerable.Empty<ExaminerResultDto>(),
                        DisplayOrder = x.DisplayOrder,
                        Bands = GetRubricMarkingBands(x),
                    })
                .ToList();
        }


        private static IEnumerable<RubricMarkingBandDto> GetRubricMarkingBands(RubricMarkingAssessmentCriterion criterion)
        {
            return criterion.RubricMarkingBands
                .Select(x =>
                    new RubricMarkingBandDto
                    {
                        BandId = x.Id,
                        Label = x.Label,
                        Level = x.Level,
                        Description = x.Description
                    })
                .ToList();
        }

        private CreateOrUpdateTestAttendanceAssetResponse CreateTestAttendanceAsset(CreateOrReplaceTestSittingDocumentDto request)
        {
            var type = (StoredFileType)Enum.Parse(typeof(StoredFileType), request.Type);
            var file = new FileSystemFileStorageService(_autoMapperHelper);

            var response = file.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                FilePath = request.FilePath,
                UpdateFileName = request.Title,
                Type = type,
                StoragePath = request.StoragePath,
                UploadedByUserId = request.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
            });

            request.StoredFileId = response.StoredFileId;

            var asset = new TestSittingDocument();
            UpdateAndSaveTestAttendanceAsset(asset, request);

            return new CreateOrUpdateTestAttendanceAssetResponse();
        }

        private CreateOrUpdateTestAttendanceAssetResponse UpdateTestAttendanceAsset(CreateOrReplaceTestSittingDocumentDto request)
        {
            var asset = NHibernateSession.Current.Get<TestSittingDocument>(request.Id);

            if (asset == null)
            {
                asset = NHibernateSession.Current.Query<TestSittingDocument>()
                    .SingleOrDefault(x => x.StoredFile.Id == request.StoredFileId);
            }

            if (asset == null)
            {
                throw new Exception("Asset does not exist: " + request.Id);
            }

            var type = (StoredFileType)Enum.Parse(typeof(StoredFileType), request.Type);
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                Type = type,
                StoragePath = request.StoragePath,
                UploadedByUserId = request.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = request.FilePath,
                UpdateStoredFileId = request.StoredFileId,
                UpdateFileName = request.StoredFileId != 0 ? request.Title : null,
            });

            request.StoredFileId = response.StoredFileId;

            UpdateAndSaveTestAttendanceAsset(asset, request);

            return new CreateOrUpdateTestAttendanceAssetResponse();
        }

        private void UpdateAndSaveTestAttendanceAsset(TestSittingDocument asset, CreateOrReplaceTestSittingDocumentDto request)
        {
            if (asset.StoredFile == null || request.StoredFileId != asset.StoredFile.Id)
            {
                var storedFile = NHibernateSession.Current.Get<StoredFile>(request.StoredFileId);
                if (storedFile == null)
                {
                    throw new Exception("Referenced StoredFile does not exist: " + request.StoredFileId);
                }
                asset.StoredFile = storedFile;
            }

            if (asset.TestSitting == null || request.TestSittingId != asset.TestSitting.Id)
            {
                var testSitting = NHibernateSession.Current.Get<TestSitting>(request.TestSittingId);
                if (testSitting == null)
                {
                    throw new Exception("Referenced TestSitting does not exist: " + request.TestSittingId);
                }
                asset.TestSitting = testSitting;
            }

            asset.Title = request.Title.Replace("[UniqueID]", asset.StoredFile.Id.ToString());
            asset.EportalDownload = request.EportalDownload;

            NHibernateSession.Current.Save(asset);
            NHibernateSession.Current.Flush();
        }

        public ServiceResponse<RubricPassRulesDto> GetRubricRules(int testSpecificationId)
        {
            var testSpec = NHibernateSession.Current.Get<TestSpecification>(testSpecificationId)
                .NotNull($"Test Specification with ID {testSpecificationId} not found");

            var result = new RubricPassRulesDto
            {
                TestSpecificationId = testSpecificationId,
                ResultAutoCalculationEnabled = testSpec.ResultAutoCalculation,
                QuestionPassRules = NHibernateSession.Current.Query<RubricQuestionPassRule>()
                    .Where(x => x.TestSpecificationId == testSpecificationId)
                    .Select(x => new QuestionPassRuleDto
                    {
                        MaximumBandLevel = x.MaximumBandLevel,
                        RubricMarkingAssessmentCriterionId = x.RubricMarkingAssessmentCriterionId,
                        RuleGroup = x.RuleGroup
                    })
                    .ToList(),
                TestResultBandRules = NHibernateSession.Current.Query<RubricTestBandRule>()
                    .Where(x => x.TestSpecificationId == testSpecificationId)
                    .Select(x => new TestResultBandRuleDto
                    {
                        MaximumBandLevel = x.MaximumBandLevel,
                        NumberOfQuestions = x.NumberOfQuestions,
                        RubricMarkingAssessmentCriterionId = x.RubricMarkingAssessmentCriterionId,
                        RuleGroup = x.RuleGroup,
                        TestComponentTypeId = x.TestComponentTypeId,
                        TestResultEligibilityType = (TestResultEligibilityTypeName)x.TestResultEligibilityType.Id,
                    })
                    .ToList(),
                TestResultQuestionRules = NHibernateSession.Current.Query<RubricTestQuestionRule>()
                    .Where(x => x.TestSpecificationId == testSpecificationId)
                    .Select(x => new TestResultQuestionRuleDto
                    {
                        MinimumQuestionsAttempted = x.MinimumQuestionsAttempted,
                        MinimumQuestionsPassed = x.MinimumQuestionsPassed,
                        RuleGroup = x.RuleGroup,
                        TestComponentTypeId = x.TestComponentTypeId,
                        TestResultEligibilityType = (TestResultEligibilityTypeName)x.TestResultEligibilityType.Id,
                    })
                    .ToList()
            };

            return new ServiceResponse<RubricPassRulesDto> { Data = result };
        }

        public ServiceResponse<RubricPassRulesDto> GetRubricRulesForTestSitting(int testSittingId)
        {
            var sitting = NHibernateSession.Current.Get<TestSitting>(testSittingId)
                .NotNull($"Test Sitting with ID {testSittingId} not found");

            return GetRubricRules(sitting.TestSpecification.Id);
        }

        public ServiceResponse<IEnumerable<TestResultInfoDto>> GetPendingTestsToIssueResult()
        {
            TestResultInfoDto dto = null;
            JobExaminer jobExaminer = null;
            TestResult testResult = null;
            TestSitting testSitting = null;
            CredentialRequest credentialRequest = null;
            CredentialApplication credentialApplication = null;
            CredentialRequestStatusType credentialRequestStatusType = null;
            TestSpecification testSpecification = null;
            Job job = null;

            var totalExaminers = Projections.Count(Projections.Distinct(Projections.Property(() => jobExaminer.Id)));

            var totalPaidReviewers = Projections.Sum(Projections.Conditional(
                Restrictions.Eq(Projections.Property(() => jobExaminer.PaidReviewer), true),
                Projections.Constant(1, NHibernateUtil.Int32), Projections.Constant(0, NHibernateUtil.Int32)));

            var totalOriginalExaminers = Projections.Sum(Projections.Conditional(
                Restrictions.And(Restrictions.Eq(Projections.Property(() => jobExaminer.PaidReviewer), false), Restrictions.Eq(Projections.Property(() => jobExaminer.ThirdExaminer), false)),
                Projections.Constant(1, NHibernateUtil.Int32), Projections.Constant(0, NHibernateUtil.Int32)));

            var submittedExaminers = Projections.Count(
                Projections.Distinct(
                    Projections.Conditional(
                        Restrictions.IsNotNull(Projections.Property(() => jobExaminer.ExaminerReceivedDate)),
                    Projections.Property(() => jobExaminer.Id), Projections.Constant(null, NHibernateUtil.Int32))));

            // Query will make use of the new indexes added to improve query performance:
            // IX_CredentialRequest_CredentialRequestStatusType_Includes_CredentialApplication
            // IX_JobExaminer_Job_Includes_ThirdExaminer_ReceivedDate_PaidReviewer
            // IX_TestResult_ResultChecked_AllowIssue_AutmoticIssuing_Includes_TestSitting_CurrentJob
            // If the query is updated, it may be worth extracting the query via tracer, estimating the execution plan,
            // and making any changes to the indexes as required. (Execution Plan will recommend indexes, you should combine with existing indexes if applicable)
            var query = NHibernateSession.Current.QueryOver(() => testResult)
                .Inner.JoinAlias(x => testResult.CurrentJob, () => job)
                .Inner.JoinAlias(x => job.JobExaminers, () => jobExaminer)
                .Inner.JoinAlias(x => testResult.TestSitting, () => testSitting)
                .Inner.JoinAlias(x => testSitting.TestSpecification, () => testSpecification)
                .Inner.JoinAlias(x => testSitting.CredentialRequest, () => credentialRequest)
                .Inner.JoinAlias(x => credentialRequest.CredentialApplication, () => credentialApplication)
                .Inner.JoinAlias(x => credentialRequest.CredentialRequestStatusType, () => credentialRequestStatusType)
                .Where(x => credentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.TestSat)
                .Where(Restrictions.Eq(Projections.Property(() => testSpecification.AutomaticIssuing), true))
                .Where(Restrictions.Eq(Projections.Property(() => testResult.ResultChecked), false))
                .Where(Restrictions.Eq(Projections.Property(() => testResult.AllowIssue), true))
                .Where(Restrictions.Eq(Projections.Property(() => testResult.AutomaticIssuingExaminer), true))
                .Select(Projections.ProjectionList()
                    .Add(Projections.GroupProperty(Projections.Property(() => testResult.Id)).WithAlias(() => dto.TestResultId))
                    .Add(Projections.GroupProperty(Projections.Property(() => credentialRequest.Id)).WithAlias(() => dto.CredentialRequestId))
                    .Add(Projections.GroupProperty(Projections.Property(() => credentialApplication.Id)).WithAlias(() => dto.CredentialApplicationId))
                    .Add(totalOriginalExaminers.WithAlias(() => dto.TotalOriginalExaminers)))
                .Where(new CustomGroupConjuction()
                .Add(Restrictions.Eq(totalPaidReviewers, 0))
                .Add(Restrictions.EqProperty(submittedExaminers, totalExaminers)));

            var result = query.TransformUsing(Transformers.AliasToBean<TestResultInfoDto>()).List<TestResultInfoDto>();

            return new ServiceResponse<IEnumerable<TestResultInfoDto>> { Data = result };
        }

        public ServiceResponse<bool> UpdateTestResultAutomaticIssuingExaminer(int testSittingId, bool? automaticIssuingExaminer)
        {
            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                try
                {
                    transaction.Begin();

                    var testResult = GetTestResultAutomaticIssuingExaminer(testSittingId);

                    if (testResult == null)
                    {
                        throw new NullReferenceException($"Could not update automatic issuer. There is no test result for the test sitting id {testSittingId}");
                    }

                    testResult.AutomaticIssuingExaminer = automaticIssuingExaminer;

                    transaction.Commit();

                    return new ServiceResponse<bool>() { Data = true, Error = false };
                }
                catch(NullReferenceException ex)
                {
                    transaction.Rollback();
                    LoggingHelper.LogError(ex.Message);
                    return new ServiceResponse<bool>() { Data = false, Error = true, ErrorMessage = ex.Message };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LoggingHelper.LogError(ex.ToString());
                    return new ServiceResponse<bool>() { Data = false, Error = true, ErrorMessage = ex.ToString() };
                }
                
            }
        }

        public bool? GetAutomaticIssuingExaminer(int testSittingId)
        {
            var testResult = GetTestResultAutomaticIssuingExaminer(testSittingId);

            if (testResult == null)
            {
                return null;
            }

            return testResult.AutomaticIssuingExaminer;
        }

        private TestResult GetTestResultAutomaticIssuingExaminer(int testSittingId)
        {
            var testResult = (from testResults in NHibernateSession.Current.Query<TestResult>()
                              where testResults.TestSitting.Id == testSittingId
                              select testResults).FirstOrDefault();

            return testResult;
        }

        public bool GetTestSpecificationAutomaticIssuingByTestSittingId(int testSittingId)
        {
            var testSpecification = (from testSittings in NHibernateSession.Current.Query<TestSitting>()
                                     where testSittings.Id == testSittingId
                                     select testSittings.TestSpecification).First();

            var automaticIssuing = testSpecification.AutomaticIssuing;
            return automaticIssuing;
        }

        public GenericResponse<bool> IsCclPracticeTest(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(credentialRequestId);

            if (credentialRequest.IsNull())
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = new List<string>() { $"Could not get credential request for id {credentialRequestId}." }
                };
            }

            var isCclPractice = credentialRequest.CredentialType.Id == 38;

            return isCclPractice;
        }

        public GenericResponse<bool> UpdateTestResultToIssuePracticeResult(int testResultId)
        {
            var testResult = NHibernateSession.Current.Get<TestResult>(testResultId);

            testResult.ResultType = NHibernateSession.Current.Get<ResultType>(3);
            testResult.ResultChecked = true;
            testResult.ProcessedDate = DateTime.Now;
            testResult.AllowCalculate = true;

            NHibernateSession.Current.SaveOrUpdate(testResult);
            NHibernateSession.Current.Flush();

            return new GenericResponse<bool>(true);
        }
    }
}
