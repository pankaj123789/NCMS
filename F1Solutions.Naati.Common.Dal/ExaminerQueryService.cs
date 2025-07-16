using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.Extensions;
using NHibernate;

namespace F1Solutions.Naati.Common.Dal
{
    
    public class ExaminerQueryService : IExaminerQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ExaminerQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public GetExaminersResponse GetExaminers(GetExaminersRequest request)
        {
            var filterJobExaminer = request.JobExaminerId != null && request.JobExaminerId.Any();
            var filterJob = request.JobId != null && request.JobId.Any();
            var filterTest = request.TestAttendanceId != null && request.TestAttendanceId.Any();
            var joinTest = filterTest || request.JoinTestResult;
            var joinJob = joinTest || filterJob || filterJobExaminer || request.JoinJobExaminer;

           
            var session = NHibernateSession.Current;
            var panelQuery = session.Query<Panel>();
            var testResultQuery = session.Query<TestResult>();
            var jobExaminerQuery = session.Query<JobExaminer>();

            var examinerQuery =
                from panel in panelQuery
                from panelMember in panel.PanelMemberships
                where panelMember.PanelRole.Id ==(int)PanelRoleCategoryName.Examiner
                select new
                {
                    panelMember.Person,
                    PanelId = panel.Id,
                    PanelLanguageId = panel.Language != null ? panel.Language.Id : (int?)null,
                    PanelMembership = panelMember,
                    JobExaminer = null as JobExaminer,
                    TestResult = null as TestResult
                };

            if (joinJob)
            {
                examinerQuery =
                    from examiner in examinerQuery
                    join jobExaminer in jobExaminerQuery on examiner.PanelMembership.Id equals jobExaminer.PanelMembership.Id
                    select new
                    {
                        examiner.Person,
                        examiner.PanelId,
                        examiner.PanelLanguageId,
                        examiner.PanelMembership,
                        JobExaminer = jobExaminer,
                        TestResult = null as TestResult
                    };
            }

            if (joinTest)
            {
                examinerQuery =
                    from examiner in examinerQuery
                    join testResult in testResultQuery on examiner.JobExaminer.Job.Id equals testResult.CurrentJobId
                    select new
                    {
                        examiner.Person,
                        examiner.PanelId,
                        examiner.PanelLanguageId,
                        examiner.PanelMembership,
                        examiner.JobExaminer,
                        TestResult = testResult
                    };
            }

            if (request.ActiveExaminersOnly.GetValueOrDefault(true))
            {
                var now = DateTime.Now.Date;
                examinerQuery = examinerQuery.Where(x => x.PanelMembership.StartDate <= now && now <= x.PanelMembership.EndDate);
            }

            if (request.LanguageId != null && request.LanguageId.Any())
                examinerQuery = examinerQuery.Where(x => x.PanelLanguageId != null && request.LanguageId.Contains(x.PanelLanguageId.Value));

            if (request.PanelId != null && request.PanelId.Any())
                examinerQuery = examinerQuery.Where(x => request.PanelId.Contains(x.PanelId));

            if (filterTest)
                examinerQuery = examinerQuery.Where(x => x.TestResult != null && x.TestResult.TestSitting != null && request.TestAttendanceId.Contains(x.TestResult.TestSitting.Id));

            if (filterJob)
                examinerQuery = examinerQuery.Where(x => x.JobExaminer != null && x.JobExaminer.Job != null && request.JobId.Contains(x.JobExaminer.Job.Id));

            if (filterJobExaminer)
                examinerQuery = examinerQuery.Where(x => x.JobExaminer != null && request.JobExaminerId.Contains(x.JobExaminer.Id));

            var examiners = examinerQuery.ToArray();

            return new GetExaminersResponse
            {
                Examiners = examiners.Select(x =>
                {
                    
                    var personName = x.Person.PersonNames.OrderByDescending(p => p.EffectiveDate).First();
                    var examinerSentUser = x.JobExaminer != null ? x.JobExaminer.ExaminerSentUser : null;
                    var examinerReceivedUser = x.JobExaminer != null ? x.JobExaminer.ExaminerReceivedUser : null;
                    var examinerReceivedDate = x.JobExaminer != null ? x.JobExaminer.ExaminerReceivedDate : null;
                    var examinerToPayrollUser = x.JobExaminer != null ? x.JobExaminer.ExaminerToPayrollUser : null;
                    var productSpecification = x.JobExaminer != null ? x.JobExaminer.ProductSpecification : null;

                    return new ExaminerDto
                    {
                        EntityId = x.Person.Entity.Id,
                        Name = x.Person.FullName,
                        LastName = personName.Surname,
                        FirstName = personName.GivenName,
                        PersonName = personName.Surname + ", " + personName.GivenName,
                        NaatiNumber = x.Person.Entity.NaatiNumber,
                        LanguageId = x.PanelLanguageId ?? new Language().Id,
                        IsChair = x.PanelMembership.PanelRole.Chair,
                        TestResultId = x.TestResult != null ? x.TestResult.Id : (int?)null,
                        JobExaminerId = x.JobExaminer != null ? x.JobExaminer.Id : (int?)null,
                        JobId = x.JobExaminer != null && x.JobExaminer.Job != null ? x.JobExaminer.Job.Id : (int?)null,
                        PersonId = x.Person.Id,
                        PanelId = x.PanelId,
                        PanelMembershipId = x.PanelMembership.Id,
                        StartDate = x.PanelMembership.StartDate,
                        EndDate = x.PanelMembership.EndDate,
                        PanelRoleId = x.PanelMembership.PanelRole.Id,
                        ThirdExaminer = x.JobExaminer != null ? x.JobExaminer.ThirdExaminer : (bool?)null,
                        NaatiNumberDisplay = x.Person.Entity.NaatiNumberDisplay,
                        ExaminerSentDate = x.JobExaminer != null ? x.JobExaminer.ExaminerSentDate : null,
                        ExaminerSentUserId = examinerSentUser != null ? examinerSentUser.Id : (int?)null,
                        ExaminerSentUser = examinerSentUser != null ? examinerSentUser.FullName : null,
                        ExaminerReceivedDate = examinerReceivedDate,
                        ExaminerReceivedUserId = examinerReceivedUser != null ? examinerReceivedUser.Id : (int?)null,
                        ExaminerReceivedUser = examinerReceivedUser != null ? examinerReceivedUser.FullName : null,
                        ExaminerToPayrollDate = x.JobExaminer != null ? x.JobExaminer.ExaminerToPayrollDate : null,
                        ExaminerToPayrollUserId = examinerToPayrollUser != null ? examinerToPayrollUser.Id : (int?)null,
                        ExaminerToPayrollUser = examinerToPayrollUser != null ? examinerToPayrollUser.FullName : null,
                        ExaminerCost = x.JobExaminer != null
                            ? (double?)(x.JobExaminer.ExaminerCost.HasValue ? (decimal?)x.JobExaminer.ExaminerCost.Value : (decimal?)null)
                            : (double?)null,
                        ExaminerPaperLost = x.JobExaminer != null ? x.JobExaminer.ExaminerPaperLost : (bool?)null,
                        LetterRecipient = x.JobExaminer != null ? x.JobExaminer.LetterRecipient : (bool?)null,
                        DateAllocated = x.JobExaminer != null ? x.JobExaminer.DateAllocated : (DateTime?)null,
                        ExaminerPaperReceivedDate = x.JobExaminer != null ? x.JobExaminer.ExaminerPaperReceivedDate : null,
                        PaidReviewer = x.JobExaminer != null ? x.JobExaminer.PaidReviewer : (bool?)null,
                        ProductSpecificationId = productSpecification != null ? productSpecification.Id : (int?)null,
                        ProductSpecificationChangedDate = x.JobExaminer != null ? x.JobExaminer.ProductSpecificationChangedDate : null,
                        ProductSpecificationChangedUserId = x.JobExaminer != null && x.JobExaminer.ProductSpecificationChangedUser != null
                            ? x.JobExaminer.ProductSpecificationChangedUser.Id
                            : (int?)null,
                        ProductSpecificationCode = productSpecification != null ? productSpecification.Code : null,
                        PayrollStatusName = x.JobExaminer != null ? x.JobExaminer.PayrollStatusName : null,
                        PrimaryContact = x.JobExaminer != null ? x.JobExaminer.PrimaryContact : null,
                        DueDate = x.JobExaminer != null ? x.JobExaminer.Job.DueDate.GetValueOrDefault().Date : (DateTime?)null,
                        MarkerStatus = x.TestResult != null && examinerReceivedDate == null && (x.TestResult.ExaminerMarkings == null || !x.TestResult.ExaminerMarkings.Any())
                        ? x.JobExaminer.Job != null && x.JobExaminer.Job.DueDate.GetValueOrDefault().Date < DateTime.Now
                            ? "Overdue"
                            : "Pending"
                        : "Submitted"
                    };
                }).ToArray(),
                Extended = joinTest || joinJob
            };
        }

        public GetMarksResponse GetMarks(GetMarksRequest request)
        {
            var result = new GetMarksResponse();
            var jobExaminer = NHibernateSession.Current.Load<JobExaminer>(request.JobExaminerId);
            if (jobExaminer == null)
                throw new NullReferenceException(string.Format("JobExaminer {0} not found.", request.JobExaminerId));

            var examinerService = new ExaminerToolsService(_autoMapperHelper);
            var details = examinerService.GetTestDetails(new GetTestDetailsRequest { NaatiNumber = jobExaminer.PanelMembership.Person.Entity.NaatiNumber, TestResultId = request.TestResultId });

            result.Components = details.Components;

            var queryExaminerMarking = NHibernateSession.Current.Query<ExaminerMarking>();
            var examinerMarking = queryExaminerMarking.FirstOrDefault(em => em.JobExaminer.Id == request.JobExaminerId && em.TestResult.Id == request.TestResultId);
            if (examinerMarking != null)
            {
                foreach (var c in details.Components)
                {
                    var componentResult = examinerMarking.ExaminerTestComponentResults.FirstOrDefault(cr => cr.ComponentNumber == c.ComponentNumber);
                    if (componentResult == null)
                    {
                        continue;
                    }

                    c.Mark = componentResult.Mark;
                }
            }

            result.OverAllPassMark = details.OverAllPassMark;

            return result;
        }

        public SaveMarksResponse SaveMarks(SaveExaminerMarksRequest request)
        {
            var jobExaminer = NHibernateSession.Current.Load<JobExaminer>(request.JobExaminerId);
            if (jobExaminer == null)
                throw new NullReferenceException(string.Format("JobExaminer {0} not found.", request.JobExaminerId));

            var testResult = NHibernateSession.Current.Load<TestResult>(request.TestResultId);
            if (testResult == null)
                throw new NullReferenceException(string.Format("TestResult {0} not found.", request.TestResultId));

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                try
                {
                    var componentTypes = NHibernateSession.Current.Query<TestComponentType>().ToList();
                    var examinerMarkings = testResult.ExaminerMarkings;
                    var previousTestResult = request.IncludePreviousMarks ? GetPreviousTestResult(testResult) : null;

                    var examinerMarking = examinerMarkings.FirstOrDefault(em => em.JobExaminer.Id == request.JobExaminerId && em.TestResult.Id == request.TestResultId);
                    if (examinerMarking == null)
                    {
                        examinerMarking = new ExaminerMarking
                        {
                            CountMarks = true,
                            JobExaminer = jobExaminer,
                            TestResult = testResult,
                        };

                        testResult.AddExaminerMarking(examinerMarking);
                    }

                    examinerMarking.Status = ExaminerMarkingStatus.Submitted;
                    NHibernateSession.Current.Save(examinerMarking);

                    var calculateAverageResult = testResult.AllowCalculate || !testResult.TestComponentResults.Any();

                    foreach (var c in request.Components)
                    {
                        var type = componentTypes.FirstOrDefault(ct => ct.Id == c.TypeId);
                        var examinerTestComponentResult = examinerMarking.ExaminerTestComponentResults.FirstOrDefault(cr => cr.ExaminerMarking.Id == examinerMarking.Id && cr.ComponentNumber == c.ComponentNumber);
                        if (examinerTestComponentResult == null)
                        {
                            examinerTestComponentResult = new ExaminerTestComponentResult
                            {
                                ComponentNumber = c.ComponentNumber,
                                ExaminerMarking = examinerMarking,
                                GroupNumber = c.GroupNumber,
                                Mark = c.Mark.GetValueOrDefault(),
                                PassMark = c.PassMark,
                                TotalMarks = c.TotalMarks,
                                Type = type,
                            };

                            examinerMarking.AddExaminerTestComponentResult(examinerTestComponentResult);
                            NHibernateSession.Current.Save(examinerTestComponentResult);
                        }
                        else
                        {
                            examinerTestComponentResult.Mark = c.Mark.GetValueOrDefault();
                            NHibernateSession.Current.Save(examinerTestComponentResult);
                        }

                        if (calculateAverageResult)
                        {
                            CalculateComponentAverage(testResult, examinerTestComponentResult, previousTestResult);
                        }
                    }

                    testResult.IncludePreviousMarks = request.IncludePreviousMarks;
                    NHibernateSession.Current.Save(testResult);

                    if (!jobExaminer.ExaminerReceivedDate.HasValue)
                    {
                        jobExaminer.ExaminerReceivedDate = DateTime.Now;
                        NHibernateSession.Current.Save(jobExaminer);
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

            return new SaveMarksResponse();
        }

        private static TestResult GetPreviousTestResult(TestResult testResult)
        {
            var originalJob = GetOriginalJob(testResult.CurrentJobId.GetValueOrDefault(), false);
            if (originalJob == null)
                return null;

            var queryResult = NHibernateSession.Current.Query<TestResult>();
            return queryResult.First(r => r.CurrentJob.Id == originalJob.Id && r.TestSitting.Id == testResult.TestSitting.Id);
        }

        private static Job GetOriginalJob(int jobId, bool itSelfAsOriginal)
        {
            var job = NHibernateSession.Current.Load<Job>(jobId);
            if (!job.ReviewFromJobId.HasValue)
                return itSelfAsOriginal ? job : null;

            return GetOriginalJob(job.ReviewFromJobId.Value, true);
        }

        private static void CalculateComponentAverage(TestResult testResult, ExaminerTestComponentResult examinerComponentResult, TestResult previousTestResult)
        {
            var componentResult = testResult.TestComponentResults.FirstOrDefault(cr => cr.ComponentNumber == examinerComponentResult.ComponentNumber)
            ?? new TestComponentResult()
            {
                TestResult = testResult,
                Mark = examinerComponentResult.Mark,
                Type = examinerComponentResult.Type,
                TotalMarks = examinerComponentResult.TotalMarks,
                PassMark = examinerComponentResult.PassMark,
                ComponentNumber = examinerComponentResult.ComponentNumber,
                GroupNumber = examinerComponentResult.GroupNumber,
                MarkingResultType = NHibernateSession.Current.Get<MarkingResultType> ((int)MarkingResultTypeName.Original)
            };

            if (componentResult.MarkingResultType.Id ==(int)MarkingResultTypeName.FromOriginal)
            {
                return;
            }

            var sumMark = 0d;
            var countMark = 0;

            SumMarkAndCount(testResult, examinerComponentResult, ref sumMark, ref countMark);

            if (previousTestResult != null)
            {
                SumMarkAndCount(previousTestResult, examinerComponentResult, ref sumMark, ref countMark);
            }

            componentResult.Mark = countMark > 0 ? Math.Round(sumMark / countMark, 2).RoundOffForMidWay() : 0;
            //TODO componentResult.TestComponentResultType = 

            NHibernateSession.Current.Save(componentResult);
        }

        private static void SumMarkAndCount(TestResult testResult, ExaminerTestComponentResult examinerComponentResult, ref double sumMark, ref int countMark)
        {
            foreach (var em in testResult.ExaminerMarkings.Where(em => em.CountMarks))
            {
                var examinerResult = em.ExaminerTestComponentResults.FirstOrDefault(cr => cr.ComponentNumber == examinerComponentResult.ComponentNumber);

                if (examinerResult == null)
                {
                    continue;
                }

                sumMark += examinerResult.Mark;
                countMark++;
            }
        }

        internal UpdateCountMarksResponse UpdateCountMarks(UpdateCountMarksRequest request, ITransaction transaction)
        {
            var testResult = NHibernateSession.Current.Load<TestResult>(request.TestResultId);
            if (testResult == null)
                throw new NullReferenceException(string.Format("TestResult {0} not found.", request.TestResultId));

            var jobExaminers = request.JobExaminersId ?? new int[0];
            foreach (var examiner in testResult.ExaminerMarkings)
            {
                examiner.CountMarks = jobExaminers.Contains(examiner.JobExaminer.Id);
                NHibernateSession.Current.Save(examiner);
            }

            testResult.IncludePreviousMarks = request.IncludePreviousMarks;
            NHibernateSession.Current.Save(testResult);

            if (!testResult.AllowCalculate)
            {
                return new UpdateCountMarksResponse();
            }

            List<ExaminerMarking> originalExaminerMarkings = null;
            if (testResult.IncludePreviousMarks)
            {
                var originalTestResult = GetPreviousTestResult(testResult);
                originalExaminerMarkings = originalTestResult.ExaminerMarkings.ToList();
            }

            foreach (var tcr in testResult.TestComponentResults)
            {
                UpdateComponentAverage(tcr, originalExaminerMarkings);
                NHibernateSession.Current.Save(tcr);
            }

            return new UpdateCountMarksResponse();
        }

        public UpdateCountMarksResponse UpdateCountMarks(UpdateCountMarksRequest request)
        {
            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                UpdateCountMarksResponse response;
                try
                {
                    response = UpdateCountMarks(request, transaction);
                    transaction.Commit();
                }
                catch 
                {
                    transaction.Rollback();
                    throw;
                }

                return response;
            }
        }

        public GetJobExaminersResponse GetJobExaminers(GetJobExaminersRequest request)
        {
            var testResults = NHibernateSession.Current.Query<TestResult>()
                .Where(x => request.TestAttendanceIds.Contains(x.TestSitting.Id))
                .ToList();

            var dtos = new List<JobExaminerDto>();
            foreach (var testResult in testResults)
            {
                var jobExaminers = testResult.CurrentJob.JobExaminers;
                foreach (var jobExaminer in jobExaminers)
                {
                    var dto = MapJobExaminer(jobExaminer, request.IncludeExaminerMarkings, testResult.Id);
                    dtos.Add(dto);
                }
            }


            return new GetJobExaminersResponse { Results = dtos };
        }
 
        public bool HasPaidReviewExaminers(int credentialRequestId)
        {
            var activeTestSitting = NHibernateSession.Current.Query<TestSitting>().FirstOrDefault(x => x.CredentialRequest.Id == credentialRequestId && !x.Rejected);
            if (activeTestSitting != null)
            {
                var jobExaminers = NHibernateSession.Current.Query<TestResult>().Where(x => x.TestSitting.Id == activeTestSitting.Id).SelectMany(x=>x.CurrentJob.JobExaminers).ToList();

                if (jobExaminers.Any(x => x.PaidReviewer))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanWithdrawApplicationUnderPaidReview(int credentialRequestId)
        {
            bool hasPaidReviewExaminers = false;
            bool hasMoreThanOneTestResults = false;

            var activeTestSitting = NHibernateSession.Current.Query<TestSitting>().FirstOrDefault(x => x.CredentialRequest.Id == credentialRequestId && !x.Rejected);
            if (activeTestSitting != null)
            {
                var jobExaminers = NHibernateSession.Current.Query<TestResult>().Where(x => x.TestSitting.Id == activeTestSitting.Id).SelectMany(x => x.CurrentJob.JobExaminers).ToList();

                if (jobExaminers.Any(x => x.PaidReviewer))
                {
                    hasPaidReviewExaminers = true;
                }

                hasMoreThanOneTestResults = NHibernateSession.Current.Query<TestResult>().Where(x => x.TestSitting.Id == activeTestSitting.Id).ToList().Count > 1;
                
            }

            if (hasPaidReviewExaminers || hasMoreThanOneTestResults)
                return false;


            return true;
        }

        private JobExaminerDto MapJobExaminer(JobExaminer jobExaminer, bool includeExaminerMarkings, int testResultId)
        {
            var dto = new JobExaminerDto
            {
                JobExaminerId = jobExaminer.Id,
                TestResultId = testResultId,
                DateAllocated = jobExaminer.DateAllocated,
                JobDueDate = jobExaminer.Job.DueDate,
                JobId = jobExaminer.Job.Id,
                ExaminerReceivedDate = jobExaminer.ExaminerReceivedDate,
                PayRollStatusId = jobExaminer.PayRolls.OrderByDescending(x => x.Payroll.Id).FirstOrDefault()?.Payroll.PayrollStatus.Id,
                PayRollStatus = jobExaminer.PayRolls.OrderByDescending(x => x.Payroll.Id).FirstOrDefault()?.Payroll.PayrollStatus.DisplayName,
                ExaminerCost = jobExaminer.ExaminerCost,
                PaidReviewer = jobExaminer.PaidReviewer,
                ThirdExaminer = jobExaminer.ThirdExaminer,
                ProductSpecificationId = jobExaminer.ProductSpecification.Id,
                ProductSpecificationCode = jobExaminer.ProductSpecification.Code,
                ExaminerPaperReceivedDate = jobExaminer.ExaminerPaperReceivedDate,
                ExaminerSentDate = jobExaminer.ExaminerSentDate,
                Examiner = MapPersonExaminer(jobExaminer.PanelMembership.Person, jobExaminer.PanelMembership),
                ExaminerMarkings = includeExaminerMarkings ? jobExaminer.Markings.Select(MapExaminerMarking).ToList() : Enumerable.Empty<ExaminerMarkingDto>(),
                ExaminerPaperLost = jobExaminer.ExaminerPaperLost,
                ProductSpecificationChangedDate = jobExaminer.ProductSpecificationChangedDate,
                ProductSpecificationChangedUserId = jobExaminer.ProductSpecificationChangedUser?.Id,
                ExaminerReceivedUserId = jobExaminer.ExaminerReceivedUser?.Id,
                ExaminerSentUserId = jobExaminer.ExaminerSentUser?.Id,
                ExaminerToPayrollUserId = jobExaminer.ExaminerToPayrollUser?.Id,
                ExaminerToPayrollDate = jobExaminer.ExaminerToPayrollDate

            };

            return dto;
        }

        private ExaminerMarkingDto MapExaminerMarking(ExaminerMarking marking)
        {
            var dto = _autoMapperHelper.Mapper.Map<ExaminerMarkingDto>(marking);

            return dto;
        }

        public GetJobExaminerResponse GetJobExaminerById(GetJobExaminerRequest request)
        {
            var jobExaminer = NHibernateSession.Current.Get<JobExaminer>(request.JobExaminerId);

            var testResult = NHibernateSession.Current.Query<TestResult>().First(x => x.CurrentJob.Id == jobExaminer.Job.Id);
            JobExaminerDto dto = null;
            if (jobExaminer != null)
            {
                dto = MapJobExaminer(jobExaminer, request.IncludeExaminerMarks, testResult.Id);
            }

            return new GetJobExaminerResponse {Result = dto};
        }


        public PersonExaminerDto MapPersonExaminer(Person person, PanelMembership panelMembership)
        {
          
            var examiner = new PersonExaminerDto
            {
                PanelMembershipId = panelMembership.Id,
                EntityId = person.Entity.Id,
                PersonId = person.Id,
                PersonName = $"{person.GivenName} {person.Surname}",
                NaatiNumber = person.Entity.NaatiNumber,
                IsChair = panelMembership.PanelRole.Chair,
                MembershipEndDate = panelMembership.EndDate,
                MembershipStartDate = panelMembership.StartDate,
                PanelName = panelMembership.Panel.Name
            };

            return examiner;
        }


        public ServiceResponse<IEnumerable<RolePlayerDto>> GetRolePlayersForTestSitting(int testSittingId)
        {
            var testSitting = NHibernateSession.Current.Get<TestSitting>(testSittingId);
            var testSessionId = testSitting.TestSession.Id;
            var skillId = testSitting.CredentialRequest.Skill.Id;

            var testSittingRolePlayersIds = NHibernateSession.Current.Query<TestSessionRolePlayerDetail>()
                .Where(x => x.Skill.Id == skillId)
                .Where(x => x.TestSessionRolePlayer.TestSession.Id == testSessionId)
                .Where(x => !x.TestSessionRolePlayer.Rejected)
                .Select(y => y.TestSessionRolePlayer.Id)
                .Distinct()
                .ToList();

            var testSessionRolePlayers = NHibernateSession.Current.Query<TestSessionRolePlayer>()
                .Where(x => testSittingRolePlayersIds.Contains(x.Id))
                .Select(y => new RolePlayerDto
                {
                    NaatiNumber = y.RolePlayer.Person.Entity.NaatiNumber,
                    RolePlayerId = y.RolePlayer.Id
                }).ToList();

            var respoonse = new ServiceResponse<IEnumerable<RolePlayerDto>> {Data = testSessionRolePlayers};
            return respoonse;
        }


       public GetPersonExaminersResponse GetActiveExaminersByLanguageAndCredentialType(GetPersonExaminersByLanguageRequest request)
       {
           var query = NHibernateSession.Current.Query<PanelMembershipCredentialType>()
               .Where(x => x.CredentialType.Id == request.CredentialTypeId &&
                           x.PanelMembership.StartDate <= DateTime.Now
                           && x.PanelMembership.EndDate >= DateTime.Now.AddDays(1).Date &&
                           x.PanelMembership.PanelRole.PanelRoleCategory.Id == (int)PanelRoleCategoryName.Examiner && (x.PanelMembership.Panel.Language.Id == request.Language1Id || x.PanelMembership.Panel.Language.Id == request.Language2Id));

            var result = query.Select(v => MapPersonExaminer(v.PanelMembership.Person, v.PanelMembership)).ToList();

           return new GetPersonExaminersResponse { Results = result };
       }

        public void RemoveExaminers(RemoveExaminersRequest request)
        {
            var examiners = NHibernateSession.Current.Query<JobExaminer>()
                .Where(x => request.JobExaminersIds.Contains(x.Id)).ToList();

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                var rubricTestComponentResults = new List<RubricTestComponentResult>();
                foreach (var examiner in examiners)
                {
                    var rubricJobExaminerResults = examiner.JobExaminerRubricTestComponentResults.ToList();
                    rubricTestComponentResults.AddRange(rubricJobExaminerResults.Select(x=> x.RubricTestComponentResult));
                    NHibernateSession.Current.DeleteList(rubricJobExaminerResults);
                }
                rubricTestComponentResults.ForEach(x=> NHibernateSession.Current.DeleteList(x.RubricAssessementCriterionResults.ToList()));
                NHibernateSession.Current.DeleteList(rubricTestComponentResults);

                var markings = examiners.SelectMany(x => x.Markings).ToList();
                markings.ForEach(x=> NHibernateSession.Current.DeleteList(x.ExaminerTestComponentResults.ToList()));
                NHibernateSession.Current.DeleteList(markings);
                NHibernateSession.Current.DeleteList(examiners);
                transaction.Commit();
            }
          
        }

        public virtual void UpdateComponentAverage(TestComponentResult testComponentResult, List<ExaminerMarking> originalExaminerMarkings)
        {
            var examinerMarkings = testComponentResult.TestResult.ExaminerMarkings;
            var sumMark = 0d;
            var countMark = 0;
            var hasOriginalExaminerMarkings = originalExaminerMarkings != null;

            foreach (var em in examinerMarkings)
            {
                if (em.CountMarks)
                {
                    var examinerResult = em.ExaminerTestComponentResults.FirstOrDefault(cr => cr.ComponentNumber == testComponentResult.ComponentNumber);

                    if (examinerResult != null)
                    {
                        sumMark += examinerResult.Mark;
                        countMark++;
                    }
                }
                
            }

            if (hasOriginalExaminerMarkings)
            {
                var originalMarks =
                (from om in originalExaminerMarkings
                    where om.CountMarks
                    from cr in om.ExaminerTestComponentResults
                    where cr.ComponentNumber == testComponentResult.ComponentNumber
                    select cr.Mark).ToList();

                sumMark += originalMarks.Sum();
                countMark += originalMarks.Count;
            }

            testComponentResult.Mark = countMark == 0 ? 0 : Math.Round(sumMark / countMark, 2).RoundOffForMidWay();
        }
    }
}
