using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using NHibernate.Util;
using TestStatus = F1Solutions.Naati.Common.Dal.Domain.TestStatus;

namespace F1Solutions.Naati.Common.Dal
{
    public class TestQueryService : ITestQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public TestQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        private void CopyJobExaminer(TestResult testResult, Job src, Job dst)
        {
            foreach (var e in src.JobExaminers)
            {
                var newExaminer = new JobExaminer();
                _autoMapperHelper.Mapper.Map(e, newExaminer);
                newExaminer.Job = dst;

                NHibernateSession.Current.Save(newExaminer);
                CopyExaminerMarking(testResult, e, newExaminer);
            }
        }

        private void CopyExaminerMarking(TestResult testResult, JobExaminer src, JobExaminer dst)
        {
            var markings = NHibernateSession.Current.Query<ExaminerMarking>().Where(em => em.JobExaminer.Id == src.Id && em.TestResult.Id == testResult.Id).ToList();
            foreach (var m in markings)
            {
                var newMarking = new ExaminerMarking();
                _autoMapperHelper.Mapper.Map(m, newMarking);
                newMarking.JobExaminer = dst;
                newMarking.TestResult = testResult;

                NHibernateSession.Current.Save(newMarking);
                CopyExaminerMarkingComponentResult(m, newMarking);
            }
        }

        private void CopyExaminerMarkingComponentResult(ExaminerMarking src, ExaminerMarking dst)
        {
            foreach (var cr in src.ExaminerTestComponentResults)
            {
                var newComponentResult = new ExaminerTestComponentResult();
                _autoMapperHelper.Mapper.Map(cr, newComponentResult);
                newComponentResult.ExaminerMarking = dst;

                NHibernateSession.Current.Save(newComponentResult);
            }
        }

        public void UpdateDueDate(UpdateDueDateRequest request)
        {
            var jobs = NHibernateSession.Current.Query<Job>().Where(j => request.JobIds.Contains(j.Id));
            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                try
                {
                    foreach (var j in jobs)
                    {
                        UpdateLastReview(j, request.DueDate);
                    }
                    NHibernateSession.Current.Flush();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        private void UpdateLastReview(Job originalJob, DateTime dueDate)
        {
            var review = NHibernateSession.Current.Query<Job>().FirstOrDefault(j => j.ReviewFromJobId == originalJob.Id);
            if (review == null)
            {
                originalJob.DueDate = dueDate;
                NHibernateSession.Current.Save(originalJob);
            }
            else
            {
                UpdateLastReview(review, dueDate);
            }
        }

        public bool TestReadyForAssets(int testAttendanceId)
        {
            return NHibernateSession.Current.Query<TestSitting>().First(ta => ta.Id == testAttendanceId).Sat;
        }

        public SaveMarksResponse SaveMarks(SaveMarksRequest request)
        {
            var testResult = NHibernateSession.Current.Load<TestResult>(request.TestResultId);
            if (testResult == null)
            {
                throw new NullReferenceException(string.Format("TestResult {0} not found.", request.TestResultId));
            }
            var specification = testResult.TestSitting.TestSpecification;
            var components = specification.TestComponents.ToList();

            foreach (var componentMark in request.Components.ToDictionary(x => x.ComponentNumber, x => x.Mark))
            {
                var componentResult = testResult.TestComponentResults.FirstOrDefault(x => x.ComponentNumber == componentMark.Key);

                if (!componentMark.Value.HasValue)
                {
                    continue;
                }

                if (componentResult == null)
                {
                    var component = components.FirstOrDefault(x => x.ComponentNumber == componentMark.Key);

                    if (component == null)
                    {
                        continue;
                    }

                    var testComponenteTypeStandardMarkingScheme = component.Type.ActiveTestComponentTypeStandardMarkingScheme;

                    componentResult = new TestComponentResult
                    {
                        TestResult = testResult,
                        Type = component.Type,
                        TotalMarks = testComponenteTypeStandardMarkingScheme?.TotalMarks ?? 0,
                        PassMark = testComponenteTypeStandardMarkingScheme?.PassMark ?? 0,
                        ComponentNumber = component.ComponentNumber,
                        GroupNumber = component.GroupNumber
                    };
                }

                componentResult.Mark = componentMark.Value.Value;

                NHibernateSession.Current.Save(componentResult);
            }

            NHibernateSession.Current.Flush();

            return new SaveMarksResponse();
        }

        public GetApplicationIdResponse GetApplicationId(GetApplicationIdRequest request)
        {
            var testAttendance = NHibernateSession.Current.Load<TestSitting>(request.TestAttendanceId);
            return new GetApplicationIdResponse
            {
                ApplicationId = testAttendance.CredentialRequest.CredentialApplication.Id
            };
        }

        public GetVenueResponse GetVenueById(int venueId)
        {
            var result = NHibernateSession.Current.Get<Venue>(venueId);
            return new GetVenueResponse { Result = _autoMapperHelper.Mapper.Map<VenueDto>(result) };
        }

        public GetTestSummaryResponse GetTestSummaryFromTestResultId(int testResultId)
        {
            var testSittingId = NHibernateSession.Current.Get<TestResult>(testResultId).TestSitting.Id;
            return GetTestSummary(new GetTestSummaryRequest { TestAttendanceId = testSittingId });

        }

        public GetTestSummaryResponse GetTestSummary(GetTestSummaryRequest request)
        {
            var testStatus = NHibernateSession.Current.Get<TestStatus>(request.TestAttendanceId);

            if (testStatus == null)
            {
                return new GetTestSummaryResponse();
            }

            var testSessionRequest =
                NHibernateSession.Current.Get<TestSitting>(request.TestAttendanceId);
            var testResult = testSessionRequest.TestResults.OrderBy(x => x.Id).FirstOrDefault();


            var testMaterials = testSessionRequest.TestSittingTestMaterials.Select(x => x.TestMaterial).ToList();

            var hasSupplementaryOptionsEnabled = NHibernateSession.Current.Query<CredentialApplicationTypeCredentialType>()
                .First(x => x.CredentialApplicationType.Id == testSessionRequest.CredentialRequest.CredentialApplication
                                .CredentialApplicationType.Id
                            && x.CredentialType.Id == testSessionRequest.CredentialRequest.CredentialType.Id).AllowSupplementary;

            var hasDowngradePath = new TestQueryHelper().SearchTests(new GetTestSearchRequest
            {
                Filters = new[]
                {
                    new TestSearchCriteria()
                    {
                        Filter = TestFilterType.AttendanceIdIntList,
                        Values = new[] {request.TestAttendanceId.ToString()}
                    },
                    new TestSearchCriteria()
                    {
                        Filter = TestFilterType.HasDowngradePathBoolean,
                        Values = new[] {true.ToString()}
                    },
                }
            }).Any();

            // Get if test result has feedback
            var hasFeedback = testResult?.CurrentJob?.JobExaminers.Any(x => x.Feedback.IsNotNullOrEmpty());

            return new GetTestSummaryResponse
            {
                Result = new TestSummaryDto
                {
                    TestAttendanceId = testSessionRequest.Id,
                    ApplicationId = testSessionRequest.CredentialRequest.CredentialApplication.Id,
                    ApplicationReference = testSessionRequest.CredentialRequest.CredentialApplication.Reference,
                    ApplicationTypeId = testSessionRequest.CredentialRequest.CredentialApplication
                        .CredentialApplicationType.Id,
                    SkillId = testSessionRequest.CredentialRequest.Skill.Id,
                    ApplicationType = testSessionRequest.CredentialRequest.CredentialApplication
                        .CredentialApplicationType.DisplayName,
                    CredentialTypeId = testSessionRequest.CredentialRequest.CredentialType.Id,
                    CredentialTypeInternalName = testSessionRequest.CredentialRequest.CredentialType.InternalName,
                    Skill = testSessionRequest.CredentialRequest.Skill.DisplayName,
                    ResultId = testResult?.Id,
                    CurrentJobId = testResult?.CurrentJob?.Id,
                    LastReviewTestResultId = testSessionRequest.TestResults.Where(y => y.CurrentJob.ReviewFromJobId != null).OrderByDescending(x => x.Id).FirstOrDefault()?.Id,
                    LastTestResultStatusTypeId = testSessionRequest.TestResults.OrderByDescending(x => x.Id).FirstOrDefault()?.ResultType.Id,
                    LastTestResultStatus = testSessionRequest.TestResults.OrderByDescending(x => x.Id).FirstOrDefault()?.ResultType.Result,
                    Language1Id = testSessionRequest.CredentialRequest.Skill.Language1.Id,
                    Language2Id = testSessionRequest.CredentialRequest.Skill.Language2.Id,
                    TestSessionId = testSessionRequest.TestSession.Id,
                    TestDate = testSessionRequest.TestSession.TestDateTime,
                    TestStatusTypeId = testStatus.TestStatusType.Id,
                    TestStatus = testStatus.TestStatusType.DisplayName,
                    TestLocationId = testSessionRequest.TestSession.Venue.TestLocation.Id,
                    TestLocation = testSessionRequest.TestSession.Venue.TestLocation.Name,
                    VenueId = testSessionRequest.TestSession.Venue.Id,
                    Venue = testSessionRequest.TestSession.Venue.Name,
                    PersonId = testSessionRequest.CredentialRequest.CredentialApplication.Person.Id,
                    TestMaterialIds = testMaterials.Select(x => x.Id).ToList(),
                    TestMaterialNames = testMaterials.Select(x => x.Title).ToList(),
                    NaatiNumber = testSessionRequest.CredentialRequest.CredentialApplication.Person.Entity.NaatiNumber,
                    Supplementary = testSessionRequest.Supplementary,
                    CredentialRequestStatusTypeId = testSessionRequest.CredentialRequest.CredentialRequestStatusType.Id,
                    CredentialRequestId = testSessionRequest.CredentialRequest.Id,
                    ApplicationStatusTypeId = testSessionRequest.CredentialRequest.CredentialApplication.CredentialApplicationStatusType.Id,
                    AllowSupplementary = hasSupplementaryOptionsEnabled,
                    HasDowngradePaths = hasDowngradePath,
                    SupplementaryCredentialRequest = testSessionRequest.CredentialRequest.Supplementary,
                    MarkingSchemaTypeId = (int)testSessionRequest.TestSpecification.MarkingSchemaType(),
                    EligibleForSupplementary = testResult?.EligibleForSupplementary,
                    EligibleForConcededPass = testResult?.EligibleForConcededPass,
                    CredentialTypeExternalName = testSessionRequest.CredentialRequest.CredentialType.ExternalName,
                    State = testSessionRequest.TestSession.Venue.TestLocation.Office.Institution.InstitutionAbberviation,
                    DefaultTestSpecification = testSessionRequest.HasDefaultSpecification(),
                    HasFeedback = hasFeedback.IsNotNull() ? hasFeedback.Value : false
                }
            };
        }

        private TestSitting GetPreviousTestSitting(TestSitting testSitting)
        {
            var credentialRequestId = testSitting.CredentialRequest.Id;
            var previousTestSitting = NHibernateSession.Current.Query<TestSitting>()
                .Where(x => !x.Rejected && x.Sat && x.CredentialRequest.Id == credentialRequestId && x.Id != testSitting.Id)
                .OrderByDescending(y => y.Id)
                .FirstOrDefault();
            return previousTestSitting;
        }

        private IList<TestComponentResult> CreateTestComponentResults(TestSitting testSitting)
        {
            var lastTestResult = testSitting?.TestResults.OrderByDescending(x => x.Id).FirstOrDefault();

            var results =
                lastTestResult?.TestComponentResults ?? Enumerable.Empty<TestComponentResult>();

            var fromOrignalType =
                NHibernateSession.Current.Load<MarkingResultType>((int)MarkingResultTypeName.FromOriginal);

            var originalType =
           NHibernateSession.Current.Load<MarkingResultType>((int)MarkingResultTypeName.Original);
            return results.Select(x => new TestComponentResult
            {
                TotalMarks = x.TotalMarks,
                ComponentNumber = x.ComponentNumber,
                GroupNumber = x.GroupNumber,
                Mark = x.MarkingResultType.Id == (int)MarkingResultTypeName.EligableForSupplementary ? x.Mark : 0,
                PassMark = x.PassMark,
                MarkingResultType = x.MarkingResultType.Id == (int)MarkingResultTypeName.EligableForSupplementary ? fromOrignalType : originalType,
                Type = x.Type
            }).ToList();
        }

        private IList<object> CreateRubricComponentResults(TestSitting testSitting, TestResult testResult, User user)
        {
            var lastTestResult = testSitting?.TestResults.OrderByDescending(x => x.Id).FirstOrDefault();

            var fromOrignalType =
                NHibernateSession.Current.Load<MarkingResultType>((int)MarkingResultTypeName.FromOriginal);

            var originalType =
                NHibernateSession.Current.Load<MarkingResultType>((int)MarkingResultTypeName.Original);

            var testRubricResults = (lastTestResult?.TestResultRubricTestComponentResults ?? Enumerable.Empty<TestResultRubricTestComponentResult>()).ToList();

            var data = new List<object>();
            foreach (var testRubricResult in testRubricResults)
            {
                var copyResults = testRubricResult.RubricTestComponentResult.MarkingResultType.Id ==
                                  (int)MarkingResultTypeName.EligableForSupplementary;

                var newResult = new TestResultRubricTestComponentResult
                {
                    ModifiedDate = DateTime.Now,
                    ModifiedUser = user,
                    TestResult = testResult,
                    RubricTestComponentResult = new RubricTestComponentResult
                    {
                        WasAttempted = false,
                        Successful = false,
                        MarkingResultType = originalType,
                        TestComponent = testRubricResult.RubricTestComponentResult.TestComponent
                    }
                };
                data.Add(newResult.RubricTestComponentResult);
                data.Add(newResult);
                if (copyResults)
                {
                    newResult.RubricTestComponentResult.MarkingResultType = fromOrignalType;
                    newResult.RubricTestComponentResult.WasAttempted = testRubricResult.RubricTestComponentResult.WasAttempted;
                    newResult.RubricTestComponentResult.Successful = testRubricResult.RubricTestComponentResult.Successful;
                    foreach (var criterrionResult in testRubricResult.RubricTestComponentResult.RubricAssessementCriterionResults)
                    {
                        data.Add(new RubricAssessementCriterionResult
                        {
                            RubricTestComponentResult = newResult.RubricTestComponentResult,
                            Comments = criterrionResult.Comments,
                            RubricMarkingAssessmentCriterion = criterrionResult.RubricMarkingAssessmentCriterion,
                            RubricMarkingBand = criterrionResult.RubricMarkingBand,
                            CommentDate = DateTime.Now
                        });
                    }
                }

            }
            return data;
        }


        private TestResult GetNewTestResult(bool istestReview, bool allowCalculate, bool includePreviousMarks, Job job, TestSitting testSitting, bool eligibleForSupplementary, bool eligibleForConcededPass)
        {
            var result = new TestResult
            {
                CommentsGeneral = string.Empty,
                ResultType = NHibernateSession.Current.Load<ResultType>((int)ResultTypeEnum.NotKnown),
                ReviewInvoiceLineId = istestReview ? (int?)1 : null,
                AllowCalculate = allowCalculate,
                ThirdExaminerRequired = false,
                CurrentJob = job,
                TestSitting = testSitting,
                IncludePreviousMarks = includePreviousMarks,
                EligibleForSupplementary = eligibleForSupplementary,
                EligibleForConcededPass = eligibleForConcededPass,
                AllowIssue = true,
                AutomaticIssuingExaminer = testSitting.TestSpecification.CredentialType.Id == 14 ? true : null,
            };

            return result;

        }

        public JobExaminer CreateJobExaminer(ExaminerRequest request, Job job)
        {
            var jobExaminer = new JobExaminer
            {
                Job = job,
                PanelMembership = NHibernateSession.Current.Load<PanelMembership>(request.PanelMemberShipId),
                ThirdExaminer = request.ThirdExaminer,
                ExaminerSentDate = request.ExaminerSentDate,
                ExaminerSentUser = request.ExaminerSentUserId.HasValue ? NHibernateSession.Current.Get<User>(request.ExaminerSentUserId) : null,
                ExaminerReceivedDate = request.ExaminerReceivedDate,
                ExaminerReceivedUser = request.ExaminerReceivedUserId.HasValue ? NHibernateSession.Current.Get<User>(request.ExaminerReceivedUserId) : null,
                ExaminerToPayrollDate = request.ExaminerToPayrollDate,
                ExaminerToPayrollUser = request.ExaminerToPayrollUserId.HasValue ? NHibernateSession.Current.Get<User>(request.ExaminerToPayrollUserId) : null,
                ExaminerCost = Convert.ToDecimal(request.ExaminerCost),
                ExaminerPaperLost = request.ExaminerPaperLost,
                ExaminerPaperReceivedDate = request.ExaminerPaperReceivedDate,
                PaidReviewer = request.PaidReviewer,
                ProductSpecification = NHibernateSession.Current.Load<ProductSpecification>(request.ProductSpecificationId),
                ProductSpecificationChangedDate = request.ProductSpecificationChangedDate,
                ProductSpecificationChangedUser = request.ProductSpecificationChangedUserId.HasValue ? NHibernateSession.Current.Get<User>(request.ProductSpecificationChangedUserId) : null,
                ValidatedDate = request.ValidatedDate,
                ValidatedUser = request.ValidateUserId.HasValue ? NHibernateSession.Current.Get<User>(request.ValidateUserId) : null,
                PrimaryContact = request.PrimaryContact,
                Status = "I"
            };
            return jobExaminer;
        }

        private ExaminerMarking CreateExaminerMarking(JobExaminer examiner, TestResult testResult)
        {
            return new ExaminerMarking
            {
                JobExaminer = examiner,
                Comments = string.Empty,
                CountMarks = false,
                TestResult = testResult
            };
        }


        private Job GetNewJob(Language language, DateTime dueDate)
        {
            var newJob = new Job
            {
                Name = string.Empty,
                Language = language,
                JobCategory = 1, //Marking
                SentDate = DateTime.Now,
                DueDate = dueDate,
                Note = string.Empty,
                JobLost = false
            };

            return newJob;
        }

        private IList<object> AddExaminersToNewTestResult(TestDataRequest request, IList<ExaminerRequest> examinerRequests)
        {
            var testSitting = NHibernateSession.Current.Get<TestSitting>(request.TestAttendanceId);
            var previousTestSitting = GetPreviousTestSitting(testSitting);
            var testComponentResults = CreateTestComponentResults(previousTestSitting);
            var user = NHibernateSession.Current.Get<User>(request.UserId);

            var paidReview = examinerRequests.Any(x => x.PaidReviewer);
            var language = testSitting.CredentialRequest.Skill.Language1;

            var job = GetNewJob(language, request.DueDate);
            job.ReviewFromJobId = request.JobId;
            var testResult = GetNewTestResult(paidReview, request.AllowCalculate, request.IncludePreviousMarks, job, testSitting, request.EligibleForSupplementary, request.EligibleForConcededPass);
            foreach(var testComponentResult in testComponentResults)
            {
                testComponentResult.TestResult = testResult;
            }

            var rubricsResultsData = CreateRubricComponentResults(previousTestSitting, testResult, user);
            var examinersList = new List<JobExaminer>();
            var examinerMarkingList = new List<ExaminerMarking>();
            foreach (var examinerRequest in examinerRequests)
            {
                var jobExaminer = CreateJobExaminer(examinerRequest, job);
                var examinerMarking = CreateExaminerMarking(jobExaminer, testResult);
                jobExaminer.DateAllocated = DateTime.Now;
                examinersList.Add(jobExaminer);
                examinerMarkingList.Add(examinerMarking);
            }

            var dataToUpsert = new List<object>();
            dataToUpsert.Add(job);
            dataToUpsert.Add(testResult);
            dataToUpsert.AddRange(examinersList);
            dataToUpsert.AddRange(examinerMarkingList);
            dataToUpsert.AddRange(testComponentResults);
            dataToUpsert.AddRange(rubricsResultsData);
            return dataToUpsert;
        }


        private IList<object> AddExaminerToExistingTestResult(TestDataRequest request, IList<ExaminerRequest> examinerRequests)
        {
            var testSitting = NHibernateSession.Current.Get<TestSitting>(request.TestAttendanceId);

            var lastestExistingTestResult = testSitting.TestResults.OrderByDescending(x => x.Id).First();
            var latestJob = lastestExistingTestResult.CurrentJob;
            latestJob.DueDate = request.DueDate;

            var examinersList = new List<JobExaminer>();
            var examinerMarkingList = new List<ExaminerMarking>();
            foreach (var examinerRequest in examinerRequests)
            {
                var jobExaminer = CreateJobExaminer(examinerRequest, latestJob);
                var examinerMarking = CreateExaminerMarking(jobExaminer, lastestExistingTestResult);
                jobExaminer.DateAllocated = DateTime.Now;
                examinersList.Add(jobExaminer);
                examinerMarkingList.Add(examinerMarking);
            }
            var dataToUpsert = new List<object>();
            dataToUpsert.Add(lastestExistingTestResult);
            dataToUpsert.AddRange(examinersList);
            dataToUpsert.AddRange(examinerMarkingList);
            return dataToUpsert;

        }


        public AddOrUpdateTestExaminersResponse AddTestExaminers(AddTestExaminersRequest request)
        {
            var dataToUpsert = new List<object>();
            var examinersRequest = request.Examiners.ToList();

            foreach (var testData in request.TestDataRequests)
            {
                var data = testData.CreateTestResult ? AddExaminersToNewTestResult(testData, examinersRequest) :
                    AddExaminerToExistingTestResult(testData, examinersRequest);
                dataToUpsert.AddRange(data);
            }

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                dataToUpsert.ForEach(NHibernateSession.Current.SaveOrUpdate);
                transaction.Commit();
            }

            return new AddOrUpdateTestExaminersResponse
            {
                JobExaminersIds = dataToUpsert.OfType<JobExaminer>().Select(x => x.Id),
                JobIds = dataToUpsert.OfType<TestResult>().Select(x => x.CurrentJob.Id)
            };
        }
        
        public AddOrUpdateTestExaminersResponse UpdateJobExaminers(UpdateJobExaminersRequest request)
        {
            var examinersDictionary = request.Examiners.ToDictionary(x => x.JobExaminerId, y => y);
            var examiners = NHibernateSession.Current.Query<JobExaminer>()
                .Where(x => examinersDictionary.Keys.Contains(x.Id));

            var mappedExaminers = new List<JobExaminer>();

            foreach (var x in examiners)
            {
                var source = examinersDictionary[x.Id];
                //x.PanelMembership = NHibernateSession.Current.Get<PanelMembership>(source.PanelMemberShipId);
                x.ThirdExaminer = source.ThirdExaminer;
                x.ExaminerSentDate = source.ExaminerSentDate;
                x.ExaminerSentUser = NHibernateSession.Current.Get<User>(source.ExaminerSentUserId.GetValueOrDefault());
                x.ExaminerReceivedDate = source.ExaminerReceivedDate;
                x.ExaminerReceivedUser = NHibernateSession.Current.Get<User>(source.ExaminerReceivedUserId.GetValueOrDefault());
                x.ExaminerToPayrollDate = source.ExaminerToPayrollDate;
                x.ExaminerToPayrollUser = NHibernateSession.Current.Get<User>(source.ExaminerToPayrollUserId.GetValueOrDefault());
                x.ExaminerCost = Convert.ToDecimal(source.ExaminerCost);
                x.LetterRecipient = source.LetterRecipient;
                x.ExaminerPaperLost = source.ExaminerPaperLost;
                x.ExaminerPaperReceivedDate = source.ExaminerPaperReceivedDate;
                x.PaidReviewer = source.PaidReviewer;
                x.ProductSpecification = NHibernateSession.Current.Get<ProductSpecification>(source.ProductSpecificationId.GetValueOrDefault());
                x.ProductSpecificationChangedDate = source.ProductSpecificationChangedDate;
                x.ProductSpecificationChangedUser = NHibernateSession.Current.Get<User>(source.ProductSpecificationChangedUserId.GetValueOrDefault());
                x.ValidatedDate = source.ValidatedDate;
                x.ValidatedUser = NHibernateSession.Current.Get<User>(source.ValidateUserId.GetValueOrDefault());
                x.PrimaryContact = source.PrimaryContact;
                mappedExaminers.Add(x);
            }
           

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                mappedExaminers.ForEach(NHibernateSession.Current.SaveOrUpdate);
                transaction.Commit();
            }

            return new AddOrUpdateTestExaminersResponse
            {
                JobExaminersIds = mappedExaminers.Select(x => x.Id),
                JobIds = new[] { mappedExaminers.FirstOrDefault()?.Job.Id ?? 0 }
            };

        }


        public GetVenuesResponse GetVenues(GetVenuesRequest request)
        {
            var query = NHibernateSession.Current.Query<Venue>();
            if (request.TestLocationId.HasValue)
            {
                query = query.Where(v => v.TestLocation.Id == request.TestLocationId.GetValueOrDefault()
                && (request.ActiveOnly == false || v.Inactive == !request.ActiveOnly));
            }

            var result = query.Select(x => new VenueDto
            {
                VenueId = x.Id,
                Address = x.Address,
                Capacity = x.Capacity,
                Name = !x.Inactive ? x.Name : $"{x.Name} (Inactive)",
                PublicNotes = x.PublicNotes,
                TestLocationId = x.TestLocation.Id
            });

            return new GetVenuesResponse { Result = result.ToList() };
        }

        public GetVenuesResponse GetVenuesShowingActive(GetVenuesRequest request)
        {
            var inactiveTag = " (Inactive)";

            var query = NHibernateSession.Current.Query<Venue>();
            if (request.TestLocationId.HasValue)
            {
                query = query.Where(v => v.TestLocation.Id == request.TestLocationId.GetValueOrDefault()
                && (request.ActiveOnly == false || v.Inactive == !request.ActiveOnly));
            }

            var result = query.Select(x => new VenueDto
            {
                VenueId = x.Id,
                Address = x.Address,
                Capacity = x.Capacity,
                Name = !x.Inactive ? x.Name : $"{x.Name} (Inactive)",
                PublicNotes = x.PublicNotes,
                TestLocationId = x.TestLocation.Id
            }).ToList();

            var orderedCurrentVenues = result.Where(x => !x.Name.Contains(inactiveTag)).Select(x => x).OrderBy(x => x.Name).ToList();
            var orderedInactiveVenues = result.Where(x => x.Name.Contains(inactiveTag)).Select(x => x).OrderBy(x => x.Name).ToList();
            orderedCurrentVenues.AddRange(orderedInactiveVenues);

            return new GetVenuesResponse { Result = orderedCurrentVenues };
        }

    public CreateOrUpdateTestSessionResponse CreateOrUpdateTestSession(CreateOrUpdateTestSessionRequest request)
        {
            var testSession = _autoMapperHelper.Mapper.Map<TestSession>(request);
            testSession.CredentialType = NHibernateSession.Current.Load<CredentialType>(request.CredentialTypeId);
            testSession.Venue = NHibernateSession.Current.Load<Venue>(request.VenueId);
            testSession.DefaultTestSpecification = NHibernateSession.Current.Load<TestSpecification>(request.DefaultTestSpecificationId);


            NHibernateSession.Current.SaveOrUpdate(testSession);

            return new CreateOrUpdateTestSessionResponse { TestSessionId = testSession.Id };
        }

        private TestSessionDto MapTestSessionDto(TestSession session)
        {
            var dto = new TestSessionDto
            {
                TestSessionId = session.Id,
                Name = session.Name,
                TestDate = session.TestDateTime,
                ArrivalTime = session.ArrivalTime,
                Duration = session.Duration,
                VenueId = session.Venue.Id,
                VenueName = session.Venue.Name,
                VenueAddress = session.Venue.Address,
                VenueCapacity = session.Venue.Capacity,
                CredentialTypeId = session.CredentialType.Id,
                PublicNotes = session.PublicNote,
                Completed = session.Completed,
                AllowSelfAssign = session.AllowSelfAssign,
                RehearsalNotes = session.RehearsalNotes,
                RehearsalDateTime = session.RehearsalDateTime,
                OverrideCapacity = session.OverrideVenueCapacity,
                NewCandidatesOnly = session.NewCandidatesOnly.HasValue ? session.NewCandidatesOnly.Value : false,
                OverridenCapacity = session.Capacity,
                DefaultTestSpecificationId = session.DefaultTestSpecification.Id,
                MarkingSchemaTypeId = (int)session.DefaultTestSpecification.MarkingSchemaType()
            };

            dto.TestSessionApplicants = GetTestSessionApplicants(dto.TestSessionId);
            return dto;
        }

        private IList<TestSessionApplicantDto> GetTestSessionApplicants(int testSessionId)
        {
            var query = from credentialRequest in NHibernateSession.Current.Query<CredentialRequest>()
                        join testSitting in NHibernateSession.Current.Query<TestSitting>() on credentialRequest.Id equals testSitting.CredentialRequest.Id
                        join testSession in NHibernateSession.Current.Query<TestSession>() on testSitting.TestSession.Id equals testSession.Id
                        join credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id
                        join person in NHibernateSession.Current.Query<Person>() on credentialApplication.Person.Id equals person.Id
                        where testSession.Id == testSessionId
                        select new TestSessionApplicantDto
                        {
                            CredentialRequestId = credentialRequest.Id,
                            ApplicationId = credentialRequest.CredentialApplication.Id,
                            CustomerNo = person.Entity.NaatiNumber,
                            PersonId = person.Id,
                            Name = $"{person.GivenName} {person.Surname}",
                            ApplicationReference = credentialApplication.Reference,
                            Status = credentialRequest.CredentialRequestStatusType.DisplayName,
                            StatusId = credentialRequest.CredentialRequestStatusType.Id,
                            ApplicationSubmittedDate = credentialApplication.EnteredDate,
                            Rejected = testSitting.Rejected,
                            StatusModifiedDate = credentialRequest.StatusChangeDate,
                            Sat = testSitting.Sat,
                            Language1 = credentialRequest.Skill.Language1.Name,
                            Language2 = credentialRequest.Skill.Language2.Name,
                            DirectionDisplayName = credentialRequest.Skill.DirectionType.DisplayName
                        };

            return query.ToList();
        }

        private TestSessionDetailsDto MapTestSessionDetailsDto(TestSession testSessionDetials, IList<TestSitting> testSessionAttendees)
        {
            return new TestSessionDetailsDto
            {
                Id = testSessionDetials.Id,
                Name = testSessionDetials.Name,
                TestDate = testSessionDetials.TestDateTime,
                CredentialType = testSessionDetials.CredentialType.DisplayName,
                ArrivalTime = testSessionDetials.ArrivalTime,
                Duration = testSessionDetials.Duration,
                VenueName = testSessionDetials.Venue.Name,
                Capacity = testSessionDetials.Venue.Capacity,
                Attendees = testSessionAttendees.Count,
                Completed = testSessionDetials.Completed,
                VenueAddress = testSessionDetials.Venue.Address,
                TestSessionApplicants = GetTestSessionApplicants(testSessionDetials.Id)
            };
        }

        public TestSearchResponse Search(GetTestSearchRequest request)
        {
            var queryHelper = new TestQueryHelper();
            var result = queryHelper.SearchTests(request);

            return new TestSearchResponse
            {
                Tests = result
            };
        }

        public GetTestSittingResponse GetTestSittingByCredentailRequestId(GetTestSittingRequest request)
        {
            var testSitting =
                NHibernateSession.Current
                    .Query<TestSitting>()
                    .SingleOrDefault(x => x.CredentialRequest.Id == request.CredentialRequestId &&
                                          x.Supplementary == request.Supplementary && !x.Rejected && !x.Sat);
            if (testSitting == null)
            {
                return new GetTestSittingResponse();
            }
            return new GetTestSittingResponse
            {
                TestSitting = MapTestSitting(testSitting)
            };
        }

        public GetTestSittingResponse GetTestSitting(GetTestSittingRequest request)
        {
            var testSitting = NHibernateSession.Current.Load<TestSitting>(request.TestSittingId);
            if (testSitting == null)
            {
                throw new NullReferenceException($"TestSitting {request.TestSittingId} does not exists.");
            }

            return new GetTestSittingResponse
            {
                TestSitting = MapTestSitting(testSitting)
            };
        }

        public GetTestResultsResponse GetTestResults(GetTestResultsRequest request)
        {
            var testSittingQuery = NHibernateSession.Current.Query<TestSitting>();
            var testResults = testSittingQuery.Where(ts => ts.CredentialRequest.CredentialApplication.Person.Entity.NaatiNumber == request.NaatiNumber && ts.Sat).ToList();

            var testRequest = new GetTestSearchRequest
            {
                Filters = new[]
                                            {
                                                new TestSearchCriteria
                                                {
                                                    Filter = TestFilterType.NaatiNumberIntList,
                                                    Values = new[] { request.NaatiNumber.ToString() }
                                                }
                                            }
            };

            var testsResponse = Search(testRequest);

            return new GetTestResultsResponse
            {
                Results = testResults.Select(r =>
                    {
                        var test = testsResponse.Tests.FirstOrDefault(t => t.AttendanceId == r.Id);
                        return test != null ? MapTestResult(r, test) : null;
                    })
                           .Where(x => x != null)
                           .ToList()
            };
        }

        public GenericResponse<FeedbackDalResponse> GetFeedback(int? testAttendanceId)
        {
            var testSitting = NHibernateSession.Current.Get<TestSitting>(testAttendanceId);

            var jobExaminers = new List<JobExaminer>();

            foreach(var testResult in testSitting.TestResults)
            {
                jobExaminers.AddRange(testResult.CurrentJob.JobExaminers);
            }
                
            if (!jobExaminers.Any())
            {
                return new GenericResponse<FeedbackDalResponse>(null);
            }

            var examinerFeedback = new List<ExaminerFeedback>();

            foreach(var jobExaminer in jobExaminers)
            {
                var hasFeedback = jobExaminer.Feedback.IsNotNullOrEmpty();

                if (hasFeedback)
                {
                    examinerFeedback.Add(new ExaminerFeedback()
                        {
                            Feedback = jobExaminer.Feedback,
                            ExaminerName = jobExaminer.PanelMembership.Person.FullName,
                            NaatiNumber = jobExaminer.PanelMembership.Person.Entity.NaatiNumber
                        }
                    );
                }
            }

            var response = new FeedbackDalResponse() { ExaminerFeedback = examinerFeedback };

            return response;
        }

        private TestSittingDto MapTestSitting(TestSitting testSitting)
        {
            return new TestSittingDto
            {
                TestSittingId = testSitting.Id,
                Rejected = testSitting.Rejected,
                CredentialRequestId = testSitting.CredentialRequest.Id,
                CredentialRequestStatusTypeId = testSitting.CredentialRequest.CredentialRequestStatusType.Id
            };
        }

        private TestSittingResultDto MapTestResult(TestSitting testSitting, TestSearchResultDto test)
        {
            var testResult = testSitting.TestResults.LastOrDefault();
            var resultType = testResult?.ResultType;
            var testLocation = testSitting.TestSession.Venue?.TestLocation;
            var resultDate = testSitting.TestResults.LastOrDefault()?.ProcessedDate;

            return new TestSittingResultDto
            {
                TestSittingId = testSitting.Id,
                LastTestResultId = testResult?.Id,
                TestDate = testSitting.TestSession.TestDateTime,
                CredentialTypeDisplayName = testSitting.CredentialRequest.CredentialType.ExternalName,
                CredentialRequestStatusTypeId = testSitting.CredentialRequest.CredentialRequestStatusType.Id,
                VenueName = testSitting.TestSession.Venue.Name,
                SkillDisplayName = testSitting.CredentialRequest.Skill.DisplayName,
                OverallResult = resultType?.Result,
                ResultTypeId = resultType?.Id,
                CredentialRequestId = testSitting.CredentialRequest.Id,
                EligibleForAPaidTestReview = test.ElilgibleForPaidReview && testSitting.CredentialRequest.CredentialApplication.SponsorInstitution == null,
                EligibleForASupplementaryTest = test.ElilgibleForSupplementary && testSitting.CredentialRequest.CredentialApplication.SponsorInstitution == null,
                State = testLocation?.Office?.State?.Abbreviation,
                TestLocationName = testLocation?.Name,
                Supplementary = testSitting.Supplementary,
                ResultDate = resultDate,
                MinStandardMarkForPaidReview = testSitting.TestSpecification.ActiveTestSpecificationStandardMarkingScheme?.MinOverallMarkForPaidReview,
                CredentialTypeId = test.CredentialTypeId,
                SkillId = test.SkillId
            };
        }
    }
}
