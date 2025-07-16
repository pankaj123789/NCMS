using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class TestQueryHelper : QuerySearchHelper
    {
        private TestSearchResultDto mTestSearchResultDto => null;
        public IList<TestSearchResultDto> SearchTests(GetTestSearchRequest request)
        {
            var applicationFilterDictionary = new Dictionary<TestFilterType, Func<TestSearchCriteria, Junction, Junction>>
            {
                [TestFilterType.NaatiNumberIntList] = (criteria, previousJunction) => GetNaatiNumberFilter(criteria, previousJunction),
                [TestFilterType.CredentialRequestTypeIntList] = (criteria, previousJunction) => GetCredentialRequestTypeFilter(criteria, previousJunction),
                [TestFilterType.LanguageIntList] = (criteria, previousJunction) => GetLanguageFilter(criteria, previousJunction),
                [TestFilterType.ApplicationTypeIntList] = (criteria, previousJunction) => GetApplicationTypeFilter(criteria, previousJunction),
                [TestFilterType.TestOfficeIntList] = (criteria, previousJunction) => GetTestOfficeFilter(criteria, previousJunction),
                [TestFilterType.TestLocationIntList] = (criteria, previousJunction) => GetTestLocationFilter(criteria, previousJunction),
                [TestFilterType.AttendanceIdIntList] = (criteria, previousJunction) => GetAttendanceFilter(criteria, previousJunction),
                [TestFilterType.TestDateFromString] = (criteria, previousJunction) => GetTestSessionDateFromFilter(criteria, previousJunction),
                [TestFilterType.TestDateToString] = (criteria, previousJunction) => GetTestSessionDateToFilter(criteria, previousJunction),
                [TestFilterType.TestStatusTypeIntList] = (criteria, previousJunction) => GetTestStatusTypeFilter(criteria, previousJunction),
                [TestFilterType.ExaminerStatusTypeIntList] = (criteria, previousJunction) => GetExaminerStatusTypeFilter(criteria, previousJunction),
                [TestFilterType.AllMarksReceivedBoolean] = (criteria, previousJunction) => GetAllMarksReceivedFilter(criteria, previousJunction),
                [TestFilterType.SupplementaryBoolean] = (criteria, previousJunction) => GetIsSupplementaryTestFilter(criteria, previousJunction),
                [TestFilterType.AllowPaidReviewBoolean] = (criteria, previousJunction) => GetAllowPaidReviewFilter(criteria, previousJunction),
                [TestFilterType.AllowSupplementaryTestBoolean] = (criteria, previousJunction) => GetAllowSupplementaryTestFilter(criteria, previousJunction),
                [TestFilterType.CredentialRequestIdIntList] = (criteria, previousJunction) => GetCredentialRequestFilter(criteria, previousJunction),
                [TestFilterType.AllowConcededPassBoolean] = (criteria, previousJunction) => GetAllowConcededPassFilter(criteria, previousJunction),
                [TestFilterType.HasDowngradePathBoolean] = (criteria, previousJunction) => GetHasDowngradePathFilter(criteria, previousJunction),
                [TestFilterType.PendingToAssignPaidReviewersBoolean] = (criteria, previousJunction) => GetPendingToAssignPaidReviewersBooleanFilter(criteria, previousJunction),
                [TestFilterType.ReadyToIssueResultsBoolean] = (criteria, previousJunction) => GetReadyToIssueResultsFilter(criteria, previousJunction),
                [TestFilterType.AllowIssueBoolean] = (criteria, previousJunction) => GetAllowIssueFilter(criteria, previousJunction)
            };

            var examinerFilters = new Dictionary<TestFilterType, Func<TestSearchCriteria, Junction, Junction>>
            {
                [TestFilterType.JobExminerMembershipIdIntList] = (criteria, previousJunction) => GetJobExaminerMemberShipFilter(criteria, previousJunction),
                [TestFilterType.JobExaminerStatusIntList] = (criteria, previousJunction) => GetJobExaminerStatusFilter(criteria, previousJunction),
                [TestFilterType.JobExaminerSubmittedFromString] = (criteria, previousJunction) => GetJobExaminerSubmittedFromFilter(criteria, previousJunction),
                [TestFilterType.JobExaminerSubmittedToString] = (criteria, previousJunction) => GetJobExaminerSubmittedToFilter(criteria, previousJunction),
                [TestFilterType.JobExaminerDueByFromString] = (criteria, previousJunction) => GetJobExaminerDueByFromFilter(criteria, previousJunction),
                [TestFilterType.JobExaminerDueByToString] = (criteria, previousJunction) => GetJobExaminerDueByToFilter(criteria, previousJunction),
            };

            Junction junction = Restrictions.Conjunction();

            var reviewFromJobIdProjection = Projections.Property(() => Job.ReviewFromJobId);
            junction = junction.Add(Restrictions.IsNull(reviewFromJobIdProjection));

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            foreach (var criteria in validCriterias)
            {
                Func<TestSearchCriteria, Junction, Junction> junctionFunc;
                if (applicationFilterDictionary.TryGetValue(criteria.Filter, out junctionFunc))
                {
                    junction = junctionFunc(criteria, junction);
                }
            }

            junction = junction.Add(GetJobExaminerRestrictions(validCriterias, examinerFilters));

            var queryOver = BuildQuery();

            if (request.Skip.HasValue)
            {
                queryOver.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                queryOver.Take(request.Take.Value);
            }

            if (junction != null)
            {
                queryOver = queryOver.Where(junction);
            }

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections.ToArray());

            var searchResult = queryOver.TransformUsing(Transformers.AliasToBean<TestSearchResultDto>());

            var resultList = searchResult.List<TestSearchResultDto>();
            return resultList;
        }


        private IQueryOver<TestStatus, TestStatus> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => TestStatus)
                .Inner.JoinAlias(x => TestStatus.TestStatusType, () => TestStatusType)
                .Inner.JoinAlias(x => TestStatus.TestSitting, () => TestSitting)
                .Left.JoinAlias(x => TestSitting.TestResults, () => TestResult)
                .Left.JoinAlias(x => TestResult.CurrentJob, () => Job)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.TestSpecification, () => TestSpecification)
                .Inner.JoinAlias(x => TestSession.Venue, () => Venue)
                .Inner.JoinAlias(x => Venue.TestLocation, () => TestLocation)
                .Inner.JoinAlias(x => TestLocation.Office, () => TestOffice)
                .Inner.JoinAlias(x => TestOffice.Institution, () => Institution)
                .Inner.JoinAlias(x => Institution.LatestInstitutionName, () => LatestInstitutionName)
                .Left.JoinAlias(x => Institution.LatestInstitutionName.InstitutionName, () => InstitutionName)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                .Inner.JoinAlias(x => Skill.Language2, () => Language2)
                .Inner.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.CredentialApplicationType, () => CredentialApplicationType)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Left.JoinAlias(x => LatestPersonName.PersonName, () => PersonName);

            return queryOver;
        }

        private List<IProjection> BuildProjections()
        {
            return new List<IProjection>
            {
                Projections.Property(() => TestSitting.Id).WithAlias(() => mTestSearchResultDto.AttendanceId),
                Projections.Property(() => CredentialRequest.Id).WithAlias(() => mTestSearchResultDto.CredentialRequestId),
                Projections.Property(() => TestSession.Id).WithAlias(() => mTestSearchResultDto.TestSessionId),
                Projections.Property(() => Skill.Id).WithAlias(() => mTestSearchResultDto.SkillId),
                Projections.Property(() => TestOffice.Id).WithAlias(() => mTestSearchResultDto.TestOfficeId),
                Projections.Property(() => CredentialType.Id).WithAlias(() => mTestSearchResultDto.CredentialTypeId),
                GetDirectionProjection().WithAlias(() => mTestSearchResultDto.Skill),
                Projections.Property(() => Entity.NaatiNumber).WithAlias(() => mTestSearchResultDto.NaatiNumber),
                Projections.Property(() => TestSitting.Supplementary).WithAlias(() => mTestSearchResultDto.Supplementary),
                Projections.Property(() => Person.Gender).WithAlias(() => mTestSearchResultDto.PersonName),
                GetNameProjection().WithAlias(() => mTestSearchResultDto.PersonName),
                Projections.Property(() => TestSession.TestDateTime).WithAlias(() => mTestSearchResultDto.TestDate),
                Projections.Property(() => CredentialType.InternalName).WithAlias(() => mTestSearchResultDto.CredentialTypeInternalName),
                Projections.Property(() => InstitutionName.Name).WithAlias(() => mTestSearchResultDto.Office),
                Projections.Property(() => TestStatusType.DisplayName).WithAlias(() => mTestSearchResultDto.Status),
                Projections.Property(() => TestStatusType.Id).WithAlias(() => mTestSearchResultDto.StatusTypeId),
                Projections.Property(() => TestResult.Id).WithAlias(() => mTestSearchResultDto.TestResultId),
                Projections.Property(() => Job.Id).WithAlias(() => mTestSearchResultDto.JobId),
                Projections.Property(() => TestStatus.HasAssets).WithAlias(() => mTestSearchResultDto.HasAssets),
                Projections.Property(() => TestStatus.HasExaminers).WithAlias(() => mTestSearchResultDto.HasExaminers),
                Projections.Property(() => CredentialRequestStatusType.Id).WithAlias(() => mTestSearchResultDto.CredentialRequestStatusTypeId),
                GetBooleanProjectionFor(GetAllowSupplementaryRestriction()).WithAlias(() => mTestSearchResultDto.ElilgibleForSupplementary),
                GetBooleanProjectionFor(GetAllowPaidReviewRestriction()).WithAlias(() => mTestSearchResultDto.ElilgibleForPaidReview)
            };
        }

        protected Junction GetAttendanceFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<TestSitting>(c => TestSitting.Id.IsIn(typeList));
            return junction;
        }

        protected Junction GetTestOfficeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Office>(c => TestOffice.Id.IsIn(typeList));
            return junction;
        }


        private Junction GetTestSessionDateFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = DateTime.Now;
            }

            var testDate = GetDateProjectionFrom(Projections.Property(() => TestSession.TestDateTime));
            junction.Add(Restrictions.Ge(testDate, dateTime.Date));
            return junction;
        }


        private Junction GetTestSessionDateToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = new DateTime(1900, 12, 01);
            }

            var testDate = GetDateProjectionFrom(Projections.Property(() => TestSession.TestDateTime));
            junction.Add(Restrictions.Le(testDate, dateTime.Date));

            return junction;
        }

        private Junction GetTestLocationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<TestLocation>(at => TestLocation.Id.IsIn(typeList));
            return junction;
        }

        protected override Junction GetNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var naatiNumberList = criteria.ToList<S, int>();
            junction.Add<Entity>(
                e => Entity.NaatiNumber.IsIn(naatiNumberList));
            return junction;
        }

        protected Junction GetTestStatusTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<TestStatusType>(at => TestStatusType.Id.IsIn(typeList));
            return junction;
        }

        protected Junction GetExaminerStatusTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>().Distinct();
            var restriction = Restrictions.Disjunction();

            foreach (var typeId in typeList)
            {
                switch (typeId)
                {
                    case (int)ExaminerStatusTypeName.InProgress:
                        restriction.Add(Restrictions.Eq(Projections.Property(() => TestStatus.HasInProgressExaminers), true));
                        break;
                    case (int)ExaminerStatusTypeName.Submitted:
                        restriction.Add(Restrictions.Eq(Projections.Property(() => TestStatus.HasSubmittedExaminers), true));
                        break;
                    case (int)ExaminerStatusTypeName.Overdue:
                        restriction.Add(Restrictions.Eq(Projections.Property(() => TestStatus.HasOverdueExaminers), true));
                        break;
                    default:
                        throw new NotSupportedException($"Examiner Status Type {typeId} is not supported");
                }

                junction = junction.Add(restriction);
            }

            return junction;
        }

        protected Junction GetAllMarksReceivedFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var active = criteria.ToList<S, bool>().First();
            return junction.Add(Restrictions.Eq(Projections.Property(() => TestStatus.AllMarksReceived), active));
        }
        protected Junction GetIsSupplementaryTestFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var active = criteria.ToList<S, bool>().First();
            return junction.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), active));
        }

        public ICriterion GetJobExaminerRestrictions(List<TestSearchCriteria> criterias,
            Dictionary<TestFilterType, Func<TestSearchCriteria, Junction, Junction>> filters)
        {

            var subQuery = QueryOver.Of(() => JobExaminer)
                .Left.JoinAlias(x => JobExaminer.Markings, () => ExaminerMarking)
                .Left.JoinAlias(x => JobExaminer.Job, () => Job)
                .Left.JoinAlias(x => JobExaminer.PanelMembership, () => PanelMembership)
                .Left.JoinAlias(x => Job.TestResults, () => TestResult);

            Junction examinerJunction = Restrictions.Conjunction();
            var initialString = examinerJunction.ToString();
            foreach (var criteria in criterias)
            {
                Func<TestSearchCriteria, Junction, Junction> junctionFunc;
                if (filters.TryGetValue(criteria.Filter, out junctionFunc))
                {
                    examinerJunction = junctionFunc(criteria, examinerJunction);
                }
            }

            if (examinerJunction.ToString() == initialString)
            {
                return Restrictions.Gt(Projections.Property(() => TestSitting.Id), 0);
            }

            subQuery.Where(examinerJunction)
                    .Select(Projections.Property(() => TestResult.TestSitting.Id));

            return Subqueries.WhereProperty<TestResult>(e => TestSitting.Id).In(subQuery);
        }


        private Junction GetJobExaminerMemberShipFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            var membershipId = Projections.Property(() => PanelMembership.Id);
            var restriction = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(membershipId))
                .Add(Restrictions.In(membershipId, typeList));
            junction.Add(restriction);

            return junction;
        }

        private Junction GetJobExaminerStatusFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();

            var statusFilter = Restrictions.Disjunction();
            foreach (var filterValue in typeList)
            {

                switch (filterValue)
                {
                    case (int)ExaminerStatusTypeName.InProgress:
                        var progressFilter = Restrictions.Conjunction().Add(Restrictions.IsNull(Projections.Property(() => JobExaminer.ExaminerReceivedDate)))
                                                                       .Add(Restrictions.Ge(Projections.Property(() => Job.DueDate), DateTime.Now.Date));

                        statusFilter.Add(progressFilter);
                        break;
                    case (int)ExaminerStatusTypeName.Submitted:
                        var submittedFilter = Restrictions.IsNotNull(Projections.Property(() => JobExaminer.ExaminerReceivedDate));
                        statusFilter.Add(submittedFilter);
                        break;

                    case (int)ExaminerStatusTypeName.Overdue:
                        var overdueFilter = Restrictions.Conjunction().Add(Restrictions.Lt(Projections.Property(() => Job.DueDate), DateTime.Now.Date))
                                                                       .Add(Restrictions.IsNull(Projections.Property(() => JobExaminer.ExaminerReceivedDate)));
                        statusFilter.Add(overdueFilter);

                        break;
                }
            }

            junction.Add(statusFilter);

            return junction;
        }

        private Junction GetJobExaminerSubmittedFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = DateTime.Now;
            }

            var receivedDate = GetDateProjectionFrom(Projections.Property(() => JobExaminer.ExaminerReceivedDate));
            junction.Add(Restrictions.Ge(receivedDate, dateTime.Date));
            return junction;
        }

        private Junction GetJobExaminerDueByFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = DateTime.Now;
            }

            var dueDate = GetDateProjectionFrom(Projections.Property(() => Job.DueDate));
            junction.Add(Restrictions.Ge(dueDate, dateTime.Date));
            return junction;
        }


        private Junction GetJobExaminerSubmittedToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = new DateTime(1900, 12, 01);
            }

            var receivedDate = GetDateProjectionFrom(Projections.Property(() => JobExaminer.ExaminerReceivedDate));
            junction.Add(Restrictions.Le(receivedDate, dateTime.Date));
            return junction;
        }

        private Junction GetJobExaminerDueByToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = new DateTime(1900, 12, 01);
            }

            var dueDate = GetDateProjectionFrom(Projections.Property(() => Job.DueDate));
            junction.Add(Restrictions.Le(dueDate, dateTime.Date));
            return junction;
        }

        protected Junction GetAllowPaidReviewFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var allowReview = criteria.ToList<S, bool>().First();

            var allowFilter = GetAllowPaidReviewRestriction();
            if (!allowReview)
            {
                allowFilter = Restrictions.Not(allowFilter);
            }

            return junction.Add(allowFilter);
           
        }

        private ICriterion GetAllowPaidReviewRestriction()
        {
            CredentialApplicationTypeCredentialType mCredentialApplicationTypeCredentialType1 = null;
            CredentialType mCredentialType1 = null;
            CredentialApplicationType mCredentialApplicationType1 = null;
            CredentialRequest mCredentialRequest1 = null;
            CredentialApplication mCredentialApplication1 = null;
            CredentialApplicationType mCredentialApplicationType2 = null;
            CredentialRequestStatusType mCredentialRequestStatusType1 = null;
            TestSitting mTestSitting1 = null;

            var testSittingsWithAllowedPaidReview = QueryOver.Of(() => mTestSitting1)
                .Inner.JoinAlias(x => mTestSitting1.CredentialRequest, () => mCredentialRequest1)
                .Inner.JoinAlias(x => mCredentialRequest1.CredentialApplication, () => mCredentialApplication1)
                .Inner.JoinAlias(x => mCredentialRequest1.CredentialRequestStatusType, () => mCredentialRequestStatusType1)
                .Inner.JoinAlias(x => mCredentialApplication1.CredentialApplicationType, () => mCredentialApplicationType1)
                .Inner.JoinAlias(x => mCredentialRequest1.CredentialType, () => mCredentialType1)
                .Inner.JoinAlias(x => mCredentialType1.CredentialApplicationTypeCredentialTypes, () => mCredentialApplicationTypeCredentialType1)
                .Inner.JoinAlias(x => mCredentialApplicationTypeCredentialType1.CredentialApplicationType, () => mCredentialApplicationType2);

            var restriction = Restrictions.Conjunction()
                .Add(Restrictions.EqProperty(Projections.Property(() => mCredentialApplicationType1.Id), Projections.Property(() => mCredentialApplicationType2.Id)))
                .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationTypeCredentialType1.AllowPaidReview), true))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSitting1.Rejected), false))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSitting1.Sat), true))
                .Add(Restrictions.Eq(Projections.Property(() => mCredentialRequestStatusType1.Id), (int)CredentialRequestStatusTypeName.TestFailed));

            testSittingsWithAllowedPaidReview = testSittingsWithAllowedPaidReview.Where(restriction)
                .Select(Projections.ProjectionList().Add(Projections.Property(() => mTestSitting1.Id)));


            TestSitting mTestSitting2 = null;
            CredentialRequest mmCredentialRequest2 = null;
            TestStatus mTestStatus2 = null;

            var testSittingsWithPaidReviewExaminers = QueryOver.Of(() => mTestStatus2)
                .Inner.JoinAlias(x => mTestStatus2.TestSitting, () => mTestSitting2)
                .Inner.JoinAlias(x => mTestSitting2.CredentialRequest, () => mmCredentialRequest2)
                .Where(Restrictions.Conjunction()
                    .Add(Restrictions.Eq(Projections.Property(() => mTestStatus2.HasPaidReviewExaminers), true)))
                .Select(Projections.ProjectionList().Add(Projections.Distinct(Projections.Property(() => mTestSitting2.Id))));

            ICriterion allowFilter = Restrictions.Conjunction()
                .Add(Subqueries.WhereProperty<CredentialRequest>(x => TestSitting.Id).In(testSittingsWithAllowedPaidReview))
                .Add(Subqueries.WhereProperty<CredentialRequest>(x => TestSitting.Id).NotIn(testSittingsWithPaidReviewExaminers));

            return allowFilter;
        }

        protected Junction GetCredentialRequestFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var credentailRequestIds = criteria.ToList<S, int>();
            junction.Add<CredentialRequest>(x => CredentialRequest.Id.IsIn(credentailRequestIds));
            return junction;

        }
        
        protected Junction GetAllowSupplementaryTestFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var allowSupplementary = criteria.ToList<S, bool>().First();

            var allowFilter = GetAllowSupplementaryRestriction();

            if (!allowSupplementary)
            {
                allowFilter = Restrictions.Not(allowFilter);
               
            }

            return junction.Add(allowFilter);
           
        }

        private ICriterion GetAllowSupplementaryRestriction()
        {
            CredentialApplicationTypeCredentialType mCredentialApplicationTypeCredentialType = null;
            CredentialType mCredentialType = null;
            CredentialApplicationType mCredentialApplicationType = null;
            CredentialRequest mCredentialRequest = null;
            CredentialApplication mCredentialApplication = null;
            CredentialApplicationType mCredentialApplicationType2 = null;
            CredentialRequestStatusType mCredentialRequestStatusType = null;
            TestSitting mmTestSitting = null;
            TestResult mTestResult = null;
            ResultType mResultType = null;


            var testSittingWithAllowedSupplementaryTestQuery = QueryOver.Of(() => mCredentialRequest)
                .Inner.JoinAlias(x => mCredentialRequest.CredentialApplication, () => mCredentialApplication)
                .Inner.JoinAlias(x => mCredentialRequest.CredentialRequestStatusType, () => mCredentialRequestStatusType)
                .Inner.JoinAlias(x => mCredentialApplication.CredentialApplicationType, () => mCredentialApplicationType)
                .Inner.JoinAlias(x => mCredentialRequest.CredentialType, () => mCredentialType)
                .Inner.JoinAlias(x => mCredentialType.CredentialApplicationTypeCredentialTypes, () => mCredentialApplicationTypeCredentialType)
                .Inner.JoinAlias(x => mCredentialApplicationTypeCredentialType.CredentialApplicationType, () => mCredentialApplicationType2)
                .Inner.JoinAlias(x => mCredentialRequest.TestSittings, () => mmTestSitting)
                .Inner.JoinAlias(x => mmTestSitting.TestResults, () => mTestResult)
                .Inner.JoinAlias(x => mTestResult.ResultType, () => mResultType);

            var restriction = Restrictions.Conjunction()
                .Add(Restrictions.EqProperty(Projections.Property(() => mCredentialApplicationType.Id), Projections.Property(() => mCredentialApplicationType2.Id)))
                .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationTypeCredentialType.AllowSupplementary), true))
                .Add(Restrictions.Eq(Projections.Property(() => mResultType.Id), (int)TestResultStatusTypeName.Failed));

            testSittingWithAllowedSupplementaryTestQuery = testSittingWithAllowedSupplementaryTestQuery
                .Where(restriction)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Distinct(Projections.Property(() => mmTestSitting.Id))));


            TestSitting mTestSitting = null;
            CredentialRequest mmCredentialRequest = null;

            var testSittingWithSupplementaryTest = QueryOver.Of(() => mTestSitting)
                .Inner.JoinAlias(x => mTestSitting.CredentialRequest, () => mmCredentialRequest)
                .Where(Restrictions.Conjunction()
                    .Add(Restrictions.Eq(Projections.Property(() => mTestSitting.Supplementary), true))
                    .Add(Restrictions.Eq(Projections.Property(() => mTestSitting.Rejected), false)))
                .Select(Projections.ProjectionList().Add(Projections.Distinct(Projections.Property(() => mTestSitting.Id))));


            ICriterion allowSupplementary = Restrictions.Conjunction()
                .Add(Subqueries.WhereProperty<CredentialRequest>(x => TestSitting.Id).In(testSittingWithAllowedSupplementaryTestQuery))
                .Add(Subqueries.WhereProperty<CredentialRequest>(x => TestSitting.Id).NotIn(testSittingWithSupplementaryTest))
                .Add(Restrictions.Eq(Projections.Property(() => TestStatus.EligibleForSupplementary), true));

            return allowSupplementary;
        }


        protected Junction GetAllowConcededPassFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var allowConcedPass = criteria.ToList<S, bool>().First();
            var hasDowngradePath = GetHasDowngradePathCriterion();
            ICriterion filter = Restrictions.Conjunction()
                .Add(hasDowngradePath)
                .Add(Restrictions.Eq(Projections.Property(() => TestStatus.EligibleForConcededPass), true));
            if (!allowConcedPass)
            {
                filter = Restrictions.Not(filter);
            }
           
            return junction.Add(filter);
        }

        protected Junction GetHasDowngradePathFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var hasdowngradeValue = criteria.ToList<S, bool>().First();
            var filter = GetHasDowngradePathCriterion();

            if (!hasdowngradeValue)
            {
                filter = Restrictions.Not(filter);
            }

            return junction.Add(filter);
        }

        protected Junction GetPendingToAssignPaidReviewersBooleanFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var pendingReviewFlag = criteria.ToList<S, bool>().First();
            var filter = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => CredentialRequestStatusType.Id), (int)CredentialRequestStatusTypeName.UnderPaidTestReview))
                .Add(Restrictions.Eq(Projections.Property(() => TestStatus.HasPaidReviewExaminers), !pendingReviewFlag));

            return junction.Add(filter);
        }
        protected Junction GetReadyToIssueResultsFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var defaultOcurrenceDays = 3;
            var backGroundJobCron=  NHibernateSession.Current.Query<SystemValue>()
                .First(x => x.ValueKey == "IssueTestResultsAndCredentialsCheckingInterval")
                .Value;

            var cronExpression = Cronos.CronExpression.Parse(backGroundJobCron);
           
            var previousOccurrence = cronExpression.GetOccurrences(DateTime.SpecifyKind(DateTime.Now.AddDays(-defaultOcurrenceDays), DateTimeKind.Utc), DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)).LastOrDefault();
           
            if (previousOccurrence == default(DateTime))
            { // set the default previous ocurrence when not ocurrence was found
                previousOccurrence = DateTime.Now.AddDays(-defaultOcurrenceDays);
            }

            previousOccurrence = DateTime.SpecifyKind(previousOccurrence, DateTimeKind.Local);

            var nextOcurrence = cronExpression.GetNextOccurrence(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));

            if (nextOcurrence.HasValue)
            {
                nextOcurrence = DateTime.SpecifyKind(nextOcurrence.Value, DateTimeKind.Local);
            }

            var filterValue = criteria.ToList<S, bool>().First();

            var resultsNotIssued = Restrictions.Eq(Projections.Property(() => CredentialRequestStatusType.Id), (int)CredentialRequestStatusTypeName.TestSat);
            var hasExaminers = Restrictions.Eq(Projections.Property(() => TestStatus.HasExaminers), true);
            var allMarReceived = Restrictions.Eq(Projections.Property(() => TestStatus.AllMarksReceived), true);

            var submittedBeforePreviousOcurrence =Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.Eq(Projections.Property(()=> TestSpecification.AutomaticIssuing), true), Restrictions.Lt(Projections.Property(() => TestStatus.LastExaminerReceivedDate), previousOccurrence)))
                .Add(Restrictions.Eq(Projections.Property(()=> TestSpecification.AutomaticIssuing), false));

            if (nextOcurrence == null)
            {
                // Remove automatic issuing filter when there is not next ocurrence
                submittedBeforePreviousOcurrence = Restrictions.Disjunction();
            }

            ICriterion filter = Restrictions.Conjunction()
                .Add(resultsNotIssued)
                .Add(hasExaminers)
                .Add(allMarReceived)
                .Add(submittedBeforePreviousOcurrence);

            if (!filterValue)
            {
                filter = Restrictions.Not(filter);
            }

            return junction.Add(filter);
        }

        protected Junction GetAllowIssueFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var allowIssue = criteria.ToList<S, bool>().First();
            var filter = Restrictions.Eq(Projections.Property(() => TestStatus.AllowIssue), allowIssue);
            
            return junction.Add(filter);
        }

        private ICriterion GetHasDowngradePathCriterion()
        {
            CredentialTypeDowngradePath mCredentialTypeDowngradePath = null;
            CredentialType mCredentialTypeFrom = null;
            CredentialType mCredentialTypeTo = null;
            SkillType mSkillTypeTo = null;
            Skill mSkillTo = null;
            Skill mSkillFrom = null;
            TestSitting mTestSitting = null;
            CredentialRequest mCredentialRequest = null;
            Language mLanguage1From = null;
            Language mLanguage1To = null;
            Language mLanguage2From = null;
            Language mLanguage2To = null;
            DirectionType mDirectionTypeFrom = null;
            DirectionType mDirectionTypeTo = null;


            var testSittinsWithDowngradePath = QueryOver.Of(() => mTestSitting)
                .Inner.JoinAlias(x => mTestSitting.CredentialRequest, () => mCredentialRequest)
                .Inner.JoinAlias(x => mCredentialRequest.Skill, () => mSkillFrom)
                .Inner.JoinAlias(x => mSkillFrom.Language1, () => mLanguage1From)
                .Inner.JoinAlias(x => mSkillFrom.Language2, () => mLanguage2From)
                .Inner.JoinAlias(x => mSkillFrom.DirectionType, () => mDirectionTypeFrom)
                .Inner.JoinAlias(x => mCredentialRequest.CredentialType, () => mCredentialTypeFrom)
                .Inner.JoinAlias(x => mCredentialTypeFrom.DowngradePaths, () => mCredentialTypeDowngradePath)
                .Inner.JoinAlias(x => mCredentialTypeDowngradePath.CredentialTypeTo, () => mCredentialTypeTo)
                .Inner.JoinAlias(x => mCredentialTypeTo.SkillType, () => mSkillTypeTo)
                .Inner.JoinAlias(x => mSkillTypeTo.Skills, () => mSkillTo)
                .Inner.JoinAlias(x => mSkillTo.Language1, () => mLanguage1To)
                .Inner.JoinAlias(x => mSkillTo.Language2, () => mLanguage2To)
                .Inner.JoinAlias(x => mSkillTo.DirectionType, () => mDirectionTypeTo);

            var langueage1FromProperty = Projections.Property(() => mLanguage1From.Id);
            var langueage1ToProperty = Projections.Property(() => mLanguage1To.Id);
            var langueage2FromProperty = Projections.Property(() => mLanguage2From.Id);
            var langueage2ToProperty = Projections.Property(() => mLanguage2To.Id);
            var directionFromProperty = Projections.Property(() => mDirectionTypeFrom.Id);
            var directionToProperty = Projections.Property(() => mDirectionTypeTo.Id);

            var sameLanguagesInSameDirection = Restrictions.Conjunction()
                .Add(Restrictions.EqProperty(directionFromProperty, directionToProperty))
                .Add(Restrictions.EqProperty(langueage1FromProperty, langueage1ToProperty))
                .Add(Restrictions.EqProperty(langueage2FromProperty, langueage2ToProperty));

            var sameLanguagesInOpositeDirections = Restrictions.Conjunction();

            var invertedDirections = Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.Eq(directionFromProperty, 1), Restrictions.Eq(directionToProperty, 2)))
                .Add(Restrictions.And(Restrictions.Eq(directionFromProperty, 2), Restrictions.Eq(directionToProperty, 1)));

            var invertedLanguages = Restrictions.Conjunction()
                .Add(Restrictions.EqProperty(langueage1FromProperty, langueage2ToProperty))
                .Add(Restrictions.EqProperty(langueage2FromProperty, langueage1ToProperty));

            sameLanguagesInOpositeDirections.Add(invertedDirections).Add(invertedLanguages);

            testSittinsWithDowngradePath = testSittinsWithDowngradePath
                .Where(Restrictions.Or(sameLanguagesInSameDirection, sameLanguagesInOpositeDirections))
                .Select(Projections.Property(() => mTestSitting.Id));

           return Restrictions.Conjunction()
                .Add(Subqueries.WhereProperty<CredentialRequest>(x => TestSitting.Id).In(testSittinsWithDowngradePath));
        }

    }
}
