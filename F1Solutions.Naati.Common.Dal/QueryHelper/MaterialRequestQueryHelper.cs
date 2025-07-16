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
    internal class MaterialRequestQueryHelper : QuerySearchHelper
    {
        private TestMaterialRequestSearchDto mTestMaterialRequestSearchDto => null;

        public int SearchCount(TestMaterialRequestSearchRequest request)
        {
            var queryOver = GetSearchQuery(request);

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections);

            var result = queryOver.RowCount();
            return result;
        }

        public IList<TestMaterialRequestSearchDto> Search(TestMaterialRequestSearchRequest request)
        {
            var queryOver = GetSearchQuery(request);

            if (request.Skip.HasValue)
            {
                queryOver.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                queryOver.Take(request.Take.Value);
            }

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections);

            return queryOver.Select(projections)
                .TransformUsing(Transformers.AliasToBean<TestMaterialRequestSearchDto>())
                .List<TestMaterialRequestSearchDto>();
        }

        private IQueryOver<MaterialRequestRoundLatest, MaterialRequestRoundLatest> GetSearchQuery(TestMaterialRequestSearchRequest request)
        {
            var testMaterialRequestFiltersDictionary = new Dictionary<TestMaterialRequestFilterType, Func<TestMaterialRequestSearchCriteria, Junction, Junction>>
            {
                [TestMaterialRequestFilterType.TitleString] = GetTitleFilter,
                [TestMaterialRequestFilterType.PanelIntList] = GetPanelFilter,
                [TestMaterialRequestFilterType.TestMaterialTypeIntList] = GetTestMaterialTypeFilter,
                [TestMaterialRequestFilterType.CredentialTypeIntList] = GetCredentialTypeFilter,
                [TestMaterialRequestFilterType.TestTaskTypeIntList] = GetTestTaskTypeFilter,
                [TestMaterialRequestFilterType.LanguageIntList] = GetLanguageSkillFilter,
                [TestMaterialRequestFilterType.TestMaterialRequestStatusIntList] = GetTestMaterialRequestStatusFilter,
                [TestMaterialRequestFilterType.RoundStatusIntList] = GetRoundStatusFilter,
                [TestMaterialRequestFilterType.DueDateFromString] = GetDueDateFromFilter,
                [TestMaterialRequestFilterType.DueDateToString] = GetDueDateToFilter,
                [TestMaterialRequestFilterType.SourceTestMaterialIdIntList] = GetSourceTestMaterialFilter,
                [TestMaterialRequestFilterType.OutputTestMaterialIdIntList] = GetOutputTestMaterialFilter,
                [TestMaterialRequestFilterType.CoordinatorNaatiNumberIntList] = GetCoordinatorNaatiNumberFilter,
                [TestMaterialRequestFilterType.MaterialRequestIdIntList] = GetMaterialRequestIdFilter,
                [TestMaterialRequestFilterType.LatestRoundIdIntList] = GetLatestRoundIdFilter,
                [TestMaterialRequestFilterType.OverdueBoolean] = GetOverdueFilter,
                [TestMaterialRequestFilterType.OwnerUserIntList] = GetOwnerUserFilter,
                [TestMaterialRequestFilterType.CreatedDateFromString] = GetCreatedDateFromFilter,
                [TestMaterialRequestFilterType.CreatedDateToString] = GetCreatedDateToFilter,
                [TestMaterialRequestFilterType.HasMembersToApprove] = GetHasPendingMembersToApprove,
                [TestMaterialRequestFilterType.HasMembersToPay] = GetHasPendingMembersToPay,
                [TestMaterialRequestFilterType.RoundNumberIntList] = GetLatestRoundNumberFilter,
                [TestMaterialRequestFilterType.MemberNaatiNumberIntList] = GetMemberNaatiNumberFilter,
                [TestMaterialRequestFilterType.TestMaterialDomainIntList] = GetTestMaterialDomainFilter
            };

            Junction junction = Restrictions.Conjunction();
            ;

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                foreach (var criteria in validCriterias)
                {
                    var junctionFunc = testMaterialRequestFiltersDictionary[criteria.Filter];
                    junction = junctionFunc(criteria, junction);
                }
            }

            var queryOver = BuildQuery();
            queryOver = queryOver.Where(junction);

            return queryOver;
        }
        protected Junction GetTitleFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var stringValue = criteria.Values.FirstOrDefault()?.Trim();

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return junction;
            }

            var filter = Restrictions.InsensitiveLike(Projections.Property(() => OutputMaterial.Title), stringValue, MatchMode.Anywhere);
            return junction.Add(filter);
        }

        protected Junction GetPanelFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => Panel.Id), filterValues);
            return junction.Add(filter);
        }

        protected Junction GetTestMaterialDomainFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterIds = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => TestMaterialDomain.Id), filterIds);

            return junction.Add(filter);
        }

        protected Junction GetTestMaterialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => TestMaterialType.Id), filterValues);
            return junction.Add(filter);
        }

        protected Junction GetCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => CredentialType.Id), filterValues);
            return junction.Add(filter);
        }

        protected Junction GetTestTaskTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => TestComponentType.Id), filterValues);
            return junction.Add(filter);
        }

        protected Junction GetLanguageSkillFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var languageFilter = Restrictions.In(Projections.Property(() => Language.Id), filterValues);
            var language1Filter = Restrictions.In(Projections.Property(() => Language1.Id), filterValues);
            var language2Filter = Restrictions.In(Projections.Property(() => Language2.Id), filterValues);

            var filter = Restrictions.Disjunction().Add(languageFilter).Add(language1Filter).Add(language2Filter);
            return junction.Add(filter);
        }

        protected Junction GetTestMaterialRequestStatusFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => MaterialRequestStatusType.Id), filterValues);
            return junction.Add(filter);
        }

        protected Junction GetRoundStatusFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => MaterialRequestRoundStatusType.Id), filterValues);
            return junction.Add(filter);
        }

        protected Junction GetSourceTestMaterialFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => SourceMaterial.Id), filterValues);
            return junction.Add(filter);
        }
        protected Junction GetOutputTestMaterialFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => OutputMaterial.Id), filterValues);
            return junction.Add(filter);
        }
        protected Junction GetCoordinatorNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => Entity.NaatiNumber), filterValues);
            return junction.Add(filter);
        }

        private Junction GetDueDateFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var dueDate = GetDateProjectionFrom(Projections.Property(() => MaterialRequestRound.DueDate));
            junction.Add(Restrictions.Ge(dueDate, dateTime.Date));

            return junction;
        }

        private Junction GetDueDateToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var dueDate = GetDateProjectionFrom(Projections.Property(() => MaterialRequestRound.DueDate));
            junction.Add(Restrictions.Le(dueDate, dateTime));

            return junction;
        }

        private Junction GetCreatedDateFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var dueDate = GetDateProjectionFrom(Projections.Property(() => MaterialRequest.CreatedDate));
            junction.Add(Restrictions.Ge(dueDate, dateTime.Date));

            return junction;
        }

        private Junction GetCreatedDateToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var dueDate = GetDateProjectionFrom(Projections.Property(() => MaterialRequest.CreatedDate));
            junction.Add(Restrictions.Le(dueDate, dateTime));

            return junction;
        }

        protected Junction GetOwnerUserFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => MaterialRequest.OwnedByUser.Id), filterValues);
            return junction.Add(filter);
        }

        private Junction GetOverdueFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;
            }
            bool overdue;
            if (!Boolean.TryParse(criteria.Values.FirstOrDefault(), out overdue))
            {
                overdue = false;
            }
            var dueDate = GetDateProjectionFrom(Projections.Property(() => MaterialRequestRound.DueDate));
            var overdueRoundStatus = (int)MaterialRequestRoundStatusTypeName.SentForDevelopment;

            ICriterion filter = Restrictions.Conjunction()
                .Add(Restrictions.Lt(dueDate, DateTime.Today.Date))
                .Add(Restrictions.Eq(Projections.Property(() => MaterialRequestRoundStatusType.Id), overdueRoundStatus));

            if (!overdue)
            {
                filter = Restrictions.Not(filter);
            }

            return junction.Add(filter);
        }
        
        private Junction GetMaterialRequestIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => MaterialRequest.Id), filterValues);
            return junction.Add(filter);
        }

        private Junction GetLatestRoundIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => MaterialRequestRound.Id), filterValues);
            return junction.Add(filter);
        }
        private Junction GetLatestRoundNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => MaterialRequestRound.RoundNumber), filterValues);
            return junction.Add(filter);
        }
        private Junction GetMemberNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();
            
            Domain.MaterialRequest mMaterialRequest = null;
            MaterialRequestPanelMembership mMaterialRequestPanelMembership = null;
            PanelMembership mPanelMembership = null;
            Person mPerson = null;
            NaatiEntity mEntity = null;

            var subQuery = QueryOver.Of(() => mMaterialRequestPanelMembership)
                .Inner.JoinAlias(x => mMaterialRequestPanelMembership.MaterialRequest, () => mMaterialRequest)
                .Inner.JoinAlias(x => mMaterialRequestPanelMembership.PanelMembership, () => mPanelMembership)
                .Inner.JoinAlias(x => mPanelMembership.Person, () => mPerson)
                .Inner.JoinAlias(x => mPerson.Entity, () => mEntity);
            
            ICriterion filter = Restrictions.In(Projections.Property(() => mEntity.NaatiNumber), filterValues);

            subQuery = subQuery.Where(filter).Select(Projections.Distinct(Projections.Property(() => mMaterialRequest.Id)));

            var filteredRequests = Subqueries.WhereProperty<Domain.MaterialRequest>(e => MaterialRequest.Id).In(subQuery);

            return junction.Add(filteredRequests);
        }

        private Junction GetHasPendingMembersToApprove<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;
            }
            bool hasPendingToApprove;
            if (!Boolean.TryParse(criteria.Values.FirstOrDefault(), out hasPendingToApprove))
            {
                hasPendingToApprove = false;
            }

            Domain.MaterialRequest mMaterialRequest = null;
            MaterialRequestStatusType mMaterialRequestStatusType = null;
            MaterialRequestPayroll mMaterialRequestPayroll = null;
            MaterialRequestPanelMembership mMaterialRequestPanelMembership = null;

            var subquery = QueryOver.Of(() => mMaterialRequestPayroll)
                .Right.JoinAlias(x => mMaterialRequestPayroll.MaterialRequestPanelMembership, () => mMaterialRequestPanelMembership)
                .Inner.JoinAlias(x => mMaterialRequestPanelMembership.MaterialRequest, () => mMaterialRequest)
                .Inner.JoinAlias(x => mMaterialRequestPanelMembership.MaterialRequestPanelMembershipTasks, () => MaterialRequestPanelMembershipTask)
                .Inner.JoinAlias(x => mMaterialRequest.MaterialRequestStatusType, () => mMaterialRequestStatusType);
            
            ICriterion filter = Restrictions.Conjunction()
                .Add(Restrictions.In(Projections.Property(() => mMaterialRequestStatusType.Id),
                    new[]
                    {
                        (int) MaterialRequestStatusTypeName.AwaitingFinalisation,
                        (int) MaterialRequestStatusTypeName.Finalised, (int) MaterialRequestStatusTypeName.Cancelled
                    }))
                .Add(Restrictions.IsNull(Projections.Property(() => mMaterialRequestPayroll.ApprovedDate)))
                .Add(Restrictions.IsNotNull(Projections.Property(() => MaterialRequestPanelMembershipTask.Id)))
                .Add(Restrictions.Gt(Projections.Property(() => MaterialRequestPanelMembershipTask.HoursSpent), 0));

            if (!hasPendingToApprove)
            {
                filter = Restrictions.Not(filter);
            }

            subquery = subquery.Where(filter).Select(Projections.Distinct(Projections.Property(() => mMaterialRequest.Id)));

            var filteredRequests = Subqueries.WhereProperty<Domain.MaterialRequest>(e => MaterialRequest.Id).In(subquery);

            return junction.Add(filteredRequests);
        }
        private Junction GetHasPendingMembersToPay<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;
            }
            bool hasPendingToApprove;
            if (!Boolean.TryParse(criteria.Values.FirstOrDefault(), out hasPendingToApprove))
            {
                hasPendingToApprove = false;
            }
      
            Domain.MaterialRequest mMaterialRequest = null;
            MaterialRequestStatusType mMaterialRequestStatusType = null;
            MaterialRequestPayroll mMaterialRequestPayroll =null;
            MaterialRequestPanelMembership mMaterialRequestPanelMembership = null;
            MaterialRequestPanelMembershipType mMaterialRequestPanelMembershipType = null;

            var subquery = QueryOver.Of(() => mMaterialRequestPayroll)
                .Inner.JoinAlias(x => mMaterialRequestPayroll.MaterialRequestPanelMembership,() => mMaterialRequestPanelMembership)
                .Inner.JoinAlias(x => mMaterialRequestPanelMembership.MaterialRequestPanelMembershipType, () => mMaterialRequestPanelMembershipType)
                .Inner.JoinAlias(x => mMaterialRequestPanelMembership.MaterialRequest, () => mMaterialRequest)
                .Inner.JoinAlias(x => mMaterialRequestPanelMembership.MaterialRequestPanelMembershipTasks, () => MaterialRequestPanelMembershipTask)
                .Inner.JoinAlias(x => mMaterialRequest.MaterialRequestStatusType, () => mMaterialRequestStatusType);

            MaterialRequestPayroll mMaterialRequestPayroll2 = null;

            // INC-203030: There are member request payroll rows that have "duplicates" which are clogging up the initial query for the update material request payment screen
            // causing expected data to not be picked up
            var getRedundantDataQuery = QueryOver.Of(() => mMaterialRequestPayroll2)
                .Select(
                   Projections.GroupProperty(
                       Projections.Property(() => mMaterialRequestPayroll2.MaterialRequestPanelMembership.Id)
                   )
                 )
                 .Where(Restrictions.Gt(Projections.Count(Projections.Property(() => mMaterialRequestPayroll2.MaterialRequestPanelMembership.Id)), 1));

            var hasTasksWithHours = Restrictions.Gt(
                Projections.Property(() => MaterialRequestPanelMembershipTask.HoursSpent),
                0);

            var isCoordinator = Restrictions.Eq(Projections.Property(() => mMaterialRequestPanelMembershipType.Id), (int)MaterialRequestPanelMembershipTypeName.Coordinator);

            ICriterion filter = Restrictions.Conjunction()
                .Add(Restrictions.In(Projections.Property(() => mMaterialRequestStatusType.Id),
                    new[]
                    {
                        (int) MaterialRequestStatusTypeName.AwaitingFinalisation,
                        (int) MaterialRequestStatusTypeName.Finalised, (int) MaterialRequestStatusTypeName.Cancelled
                    }))
                .Add(Restrictions.IsNull(Projections.Property(() => mMaterialRequestPayroll.PaymentDate)))
                .Add(Restrictions.IsNotNull(Projections.Property(() => mMaterialRequestPayroll.ApprovedDate)))
                .Add(Restrictions.IsNotNull(Projections.Property(() => MaterialRequestPanelMembershipTask.Id)))
                .Add(Restrictions.Or(hasTasksWithHours, isCoordinator))
                .Add(Restrictions.Not(Restrictions.In(Projections.Property(() => mMaterialRequestPanelMembership.Id), getRedundantDataQuery.GetExecutableQueryOver(NHibernateSession.Current).List<int>().ToList())));

            if (!hasPendingToApprove)
            {
                filter = Restrictions.Not(filter);
            }

            subquery = subquery.Where(filter).Select(Projections.Property(() => mMaterialRequest.Id));

            var filteredRequests = Subqueries.WhereProperty<Domain.MaterialRequest>(e => MaterialRequest.Id).In(subquery);

            return junction.Add(filteredRequests);
        }

        private IQueryOver<MaterialRequestRoundLatest, MaterialRequestRoundLatest> BuildQuery()
        {
            var coordinatorFilter = Restrictions.Eq(Projections.Property(() => MaterialRequestPanelMembership.MaterialRequestPanelMembershipType.Id),
                (int) MaterialRequestPanelMembershipTypeName.Coordinator);

            var queryOver = NHibernateSession.Current.QueryOver(() => MaterialRequestRoundLatest)
                .Inner.JoinAlias(x => MaterialRequestRoundLatest.MaterialRequestRound, () => MaterialRequestRound)
                .Inner.JoinAlias(x => MaterialRequestRound.MaterialRequest, () => MaterialRequest)
                .Inner.JoinAlias(x => MaterialRequest.MaterialRequestRoundPanelMemberships, () => MaterialRequestPanelMembership, coordinatorFilter)
                .Inner.JoinAlias(x => MaterialRequestPanelMembership.PanelMembership, () => PanelMembership)
                .Inner.JoinAlias(x => PanelMembership.Person, () => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName)
                .Inner.JoinAlias(x => MaterialRequestRound.MaterialRequestRoundStatusType, () => MaterialRequestRoundStatusType)
                .Inner.JoinAlias(x => MaterialRequest.OutputMaterial, () => OutputMaterial)
                .Inner.JoinAlias(x => MaterialRequest.MaterialRequestStatusType, () => MaterialRequestStatusType)
                .Inner.JoinAlias(x => OutputMaterial.TestMaterialType, () => TestMaterialType)
                .Inner.JoinAlias(x => OutputMaterial.TestMaterialDomain, () => TestMaterialDomain)
                .Inner.JoinAlias(x => OutputMaterial.TestComponentType, () => TestComponentType)
                .Inner.JoinAlias(x => TestComponentType.TestSpecification, () => TestSpecification)
                .Inner.JoinAlias(x => TestSpecification.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => MaterialRequest.Panel, () => Panel)
                .Inner.JoinAlias(x => Panel.Language, () => PanelLanguage)
                .Left.JoinAlias(x => OutputMaterial.Skill, () => Skill)
                .Left.JoinAlias(x => Skill.Language1, () => Language1)
                .Left.JoinAlias(x => Skill.Language2, () => Language2)
                .Left.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Left.JoinAlias(x => OutputMaterial.Language, () => Language)
                .Left.JoinAlias(x => MaterialRequest.SourceMaterial, () => SourceMaterial)
                .OrderBy(x => x.MaterialRequestRound.MaterialRequestRoundStatusType.Id).Asc
                .ThenBy(x => x.MaterialRequestRound.DueDate).Asc;

            return queryOver;
        }

        private ProjectionList BuildProjections()
        {
            var givenName = Projections.Property(() => PersonName.GivenName);
            var space = Projections.Constant(" ", NHibernateUtil.String);
            var surName = Projections.Property(() => PersonName.Surname);
            var fullName = Concatenate(givenName, space, surName);



            var languageName = Projections.Conditional(
                Restrictions.IsNotNull(Projections.Property(() => Language.Id)),
                Projections.Property(() => Language.Name), GetDirectionProjection());
            return Projections.ProjectionList()
                .Add(Projections.Property(() => MaterialRequest.Id).WithAlias(() => mTestMaterialRequestSearchDto.Id))
                .Add(Projections.Property(() => TestComponentType.Name).WithAlias(() => mTestMaterialRequestSearchDto.TestTaskType))
                .Add(languageName.WithAlias(() => mTestMaterialRequestSearchDto.Language))
                .Add(Projections.Property(() => Panel.Name).WithAlias(() => mTestMaterialRequestSearchDto.Panel))
                .Add(Projections.Property(() => PanelLanguage.Name).WithAlias(() => mTestMaterialRequestSearchDto.PanelLanguageName))
                .Add(Projections.Property(() => MaterialRequestRound.RoundNumber).WithAlias(() => mTestMaterialRequestSearchDto.Round))
                .Add(Projections.Property(() => MaterialRequestRoundStatusType.Id).WithAlias(() => mTestMaterialRequestSearchDto.RoundStatusTypeId))
                .Add(fullName.WithAlias(() => mTestMaterialRequestSearchDto.Coordinator))
                .Add(Projections.Property(() => MaterialRequestStatusType.Id).WithAlias(() => mTestMaterialRequestSearchDto.RequestStatusTypeId))
                .Add(Projections.Property(() => CredentialType.InternalName).WithAlias(() => mTestMaterialRequestSearchDto.CredentialType))
                .Add(Projections.Property(() => OutputMaterial.Title).WithAlias(() => mTestMaterialRequestSearchDto.RequestTitle))
                .Add(Projections.Property(() => TestMaterialType.DisplayName).WithAlias(() => mTestMaterialRequestSearchDto.RequestType))
                .Add(Projections.Property(() => SourceMaterial.Id).WithAlias(() => mTestMaterialRequestSearchDto.SourceMaterialId))
                .Add(Projections.Property(() => MaterialRequest.CreatedDate).WithAlias(() => mTestMaterialRequestSearchDto.CreatedDate))
                .Add(Projections.Property(() => OutputMaterial.Id).WithAlias(() => mTestMaterialRequestSearchDto.OutputMaterialId))
                .Add(Projections.Property(() => MaterialRequestRound.DueDate).WithAlias(() => mTestMaterialRequestSearchDto.DueDate))
                .Add(Projections.Property(() => MaterialRequestRound.Id).WithAlias(() => mTestMaterialRequestSearchDto.LatestRoundId))
                .Add(Projections.Property(() => Entity.NaatiNumber).WithAlias(() => mTestMaterialRequestSearchDto.CoordinatorNaatiNumber));
        }

    }
}
