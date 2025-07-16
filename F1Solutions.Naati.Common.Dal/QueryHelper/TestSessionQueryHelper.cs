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
    internal class TestSessionQueryHelper : QuerySearchHelper
    {
        private TestSessionSearchResultDto mTestSessionSearchResultDto => null;
        public IList<TestSessionSearchResultDto> SearchTestSessions(GetTestSessionSearchRequest request)
        {
            var applicationFilterDictionary = new Dictionary<TestSessionFilterType, Func<TestSessionSearchCriteria, Junction, Junction>>
            {
                [TestSessionFilterType.TestDateFromString] = (criteria, previousJunction) => GetTestSessionDateFromFilter(criteria, previousJunction),
                [TestSessionFilterType.TestDateToString] = (criteria, previousJunction) => GetTestSessionDateToFilter(criteria, previousJunction),
                [TestSessionFilterType.CredentialIntList] = (criteria, previousJunction) => GetCredentialTypeFilter(criteria, previousJunction),
                [TestSessionFilterType.CredentialSkillIntList] = (criteria, previousJunction) => GetCredentialTypeSkillFilter(criteria, previousJunction),
                [TestSessionFilterType.TestLocationIntList] = (criteria, previousJunction) => GetTestLocationFilter(criteria, previousJunction),
                [TestSessionFilterType.TestVenueIntList] = (criteria, previousJunction) => GetTestVenueFilter(criteria, previousJunction),
                [TestSessionFilterType.SessionNameString] = (criteria, previousJunction) => previousJunction.Add<TestSession>(e => TestSession.Name.IsLike(criteria.Values.FirstOrDefault())),
                [TestSessionFilterType.IncludeCompletedSessionsBoolean] = (criteria, previousJunction) => GetAllSessionsFilter(criteria, previousJunction),
                [TestSessionFilterType.TestSessionIntList] = (criteria, previousJunction) => GetTestSessionFilter(criteria, previousJunction),
                [TestSessionFilterType.RolePlayersRequiredBoolean] = (criteria, previousJunction) => GetRequireRolePlayersFilter(criteria, previousJunction),
                [TestSessionFilterType.TestSpecificationIntList] = (criteria, previousJunction) => GetTestSpecificationFilter(criteria, previousJunction),
                [TestSessionFilterType.AllowSelfAssignBoolean] = (criteria, previousJunction) => GetAllowSelfAssignFilter(criteria, previousJunction),
                [TestSessionFilterType.NewCandidatesOnlyBoolean] = (criteria, previousJunction) => GetNewCandidatesOnlyFilter(criteria, previousJunction),
                [TestSessionFilterType.IsActiveBoolean] = (criteria, previousJunction) => IsActiveFilter(criteria, previousJunction)
            };

            Junction junction = Restrictions.Conjunction();


            if (request.Filters.FirstOrDefault(x => x.Filter == TestSessionFilterType.IncludeCompletedSessionsBoolean) == null)
            {
                junction = junction.Add(Restrictions.Eq(Projections.Property(() => TestSession.Completed), false));
            }

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            foreach (var criteria in validCriterias)
            {
                Func<TestSessionSearchCriteria, Junction, Junction> junctionFunc;
                if (applicationFilterDictionary.TryGetValue(criteria.Filter, out junctionFunc))
                {
                    junction = junctionFunc(criteria, junction);
                }
            }

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

            var searchResult = queryOver.TransformUsing(Transformers.AliasToBean<TestSessionSearchResultDto>());

            var resultList = searchResult.List<TestSessionSearchResultDto>();

            return resultList;
        }

        private Junction GetAllowSelfAssignFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var allowSelfAssign = criteria.ToList<S, bool>().First();
            junction.Add(Restrictions.Eq(Projections.Property(() => TestSession.AllowSelfAssign), allowSelfAssign));
            return junction;
        }

        private Junction GetNewCandidatesOnlyFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var newCandidatesOnly = criteria.ToList<S, bool>().First();
            if (!newCandidatesOnly)
            {
                junction.Add(Restrictions.Or(Restrictions.Eq(Projections.Property(() => TestSession.NewCandidatesOnly), newCandidatesOnly),
                    Restrictions.IsNull(Projections.Property(() => TestSession.NewCandidatesOnly))));
            }
            else
            {
                junction.Add(Restrictions.Eq(Projections.Property(() => TestSession.NewCandidatesOnly), newCandidatesOnly));
            }
            return junction;
        }

        private Junction IsActiveFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var isActive = criteria.ToList<S, bool>().First();

            junction.Add(Restrictions.Eq(Projections.Property(() => TestSession.IsActive), isActive));

            return junction;
        }

        private IQueryOver<TestSession, TestSession> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => TestSession)
                .Left.JoinAlias(x => TestSession.Venue, () => Venue)
                .Left.JoinAlias(x => Venue.TestLocation, () => TestLocation)
                .Left.JoinAlias(x => TestLocation.Office, () => Office)
                .Left.JoinAlias(x => Office.State, () => State)
                .Left.JoinAlias(x => TestSession.TestSittings, () => TestSitting)
                .Left.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Left.JoinAlias(x => TestSession.CredentialType, () => CredentialType)
                .Left.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Left.JoinAlias(x => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Left.JoinAlias(x => Skill.Language1, () => Language1)
                .Left.JoinAlias(x => Skill.Language2, () => Language2)
                .Left.JoinAlias(x => Skill.DirectionType, () => DirectionType);
            return queryOver;
        }

        private List<IProjection> BuildProjections()
        {
            var rejectTestSittingProperty = Projections.Property(() => TestSitting.Rejected);
            var credentialRequestStatusProperty = Projections.Property(() => CredentialRequestStatusType.Id);
            var capacityProperty = Projections.Conditional(Restrictions.Eq(Projections.Property(() => TestSession.OverrideVenueCapacity), true),
                Projections.Property(() => TestSession.Capacity),
                Projections.Property(() => Venue.Capacity));
            var testSessionCompletedProperty = Projections.Property(() => TestSession.Completed);
            return new List<IProjection>
            {
                Projections.Group(() => TestSession.Id).WithAlias(() => mTestSessionSearchResultDto.TestSessionId),
                Projections.Group(() => TestSession.Completed).WithAlias(() => mTestSessionSearchResultDto.Completed),
                Projections.Group(() => TestSession.IsActive).WithAlias(() => mTestSessionSearchResultDto.IsActive),
                Projections.Max(() => TestSession.TestDateTime).WithAlias(() => mTestSessionSearchResultDto.TestDate),
                Projections.Max(() => TestSession.Name).WithAlias(() => mTestSessionSearchResultDto.SessionName),
                Projections.Max(() => TestSession.Duration).WithAlias(() => mTestSessionSearchResultDto.Duration),

                Projections.Max(() => TestLocation.Name).WithAlias(() => mTestSessionSearchResultDto.TestLocationName),
                Projections.Max(() => State.Abbreviation).WithAlias(() => mTestSessionSearchResultDto.TestLocationStateName),

                Projections.SqlFunction("concat",
                                        NHibernateUtil.String,
                                        Projections.Max(() => Venue.Name),
                                        Projections.Max(Projections.Conditional(
                                             Restrictions.Eq(Projections.Property(() => Venue.Inactive), true),
                                             Projections.Constant(" (Inactive)", NHibernateUtil.String),
                                             Projections.Constant("", NHibernateUtil.String)))
                                        ).WithAlias(() => mTestSessionSearchResultDto.Venue),

            Projections.Max(capacityProperty).WithAlias(() => mTestSessionSearchResultDto.Capacity),

                Projections.Sum(
                    Projections.Conditional(Restrictions.Conjunction()
                .Add(Restrictions.Eq(rejectTestSittingProperty, false ))
                .Add(Restrictions.Not(Restrictions.Eq(credentialRequestStatusProperty, (int)CredentialRequestStatusTypeName.AwaitingTestPayment ))),
                Projections.Constant(1,NHibernateUtil.Int32),
                Projections.Constant(0,NHibernateUtil.Int32)))
                .WithAlias(() => mTestSessionSearchResultDto.Accepted),

                Projections.Sum(
                        Projections.Conditional(Restrictions.Conjunction()
                                .Add(Restrictions.Eq(rejectTestSittingProperty, false ))
                                .Add(Restrictions.Eq(credentialRequestStatusProperty, (int)CredentialRequestStatusTypeName.AwaitingTestPayment)),
                            Projections.Constant(1,NHibernateUtil.Int32),
                            Projections.Constant(0,NHibernateUtil.Int32)))
                    .WithAlias(() => mTestSessionSearchResultDto.PendingToAccept),

                Projections.Sum(Projections.Conditional(Restrictions.Eq(rejectTestSittingProperty, false ), Projections.Constant(1,NHibernateUtil.Int32), Projections.Constant(0,NHibernateUtil.Int32))).WithAlias(() => mTestSessionSearchResultDto.Allocated),

                Projections.Sum(Projections.Conditional(Restrictions.Eq(rejectTestSittingProperty, true ) , Projections.Constant(1,NHibernateUtil.Int32), Projections.Constant(0,NHibernateUtil.Int32))).WithAlias(() => mTestSessionSearchResultDto.Rejected),

               // Projections.Max(() => CredentialRequest.Id).WithAlias(() => mTestSessionSearchResultDto.CredentialRequestId),
                //Projections.Max(() => Skill.Id).WithAlias(() => mTestSessionSearchResultDto.SkillId),
                Projections.Max(GetDirectionProjection()).WithAlias(() => mTestSessionSearchResultDto.SkillDisplayName),
                Projections.Max(() => CredentialType.Id).WithAlias(() => mTestSessionSearchResultDto.CredentialTypeId),
                Projections.Max(() => CredentialType.InternalName).WithAlias(() => mTestSessionSearchResultDto.CredentialTypeInternalName),
                Projections.Max(() => CredentialType.ExternalName).WithAlias(() => mTestSessionSearchResultDto.CredentialTypeExternalName),
        };
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

        private Junction GetCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialType>(at => CredentialType.Id.IsIn(typeList));
            return junction;
        }

        private Junction GetCredentialTypeSkillFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();

            var filter = Subqueries.WhereProperty<TestSession>(x => TestSession.Id).In(GetTestSessionIdsWithSkillIds(typeList.ToArray()));
            junction.Add(filter);
            return junction;
        }

        private Junction GetTestSpecificationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();

            junction.Add<TestSession>(at => TestSession.DefaultTestSpecification.Id.IsIn(typeList));

            return junction;
        }

        private Junction GetTestLocationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<TestLocation>(at => TestLocation.Id.IsIn(typeList));
            return junction;
        }
        private Junction GetTestSessionFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<TestSession>(at => TestSession.Id.IsIn(typeList));
            return junction;
        }

        private Junction GetTestVenueFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Venue>(at => Venue.Id.IsIn(typeList));
            return junction;
        }

        private Junction GetAllSessionsFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var active = criteria.ToList<S, bool>().First();

            if (!active)
            {
                return junction.Add(Restrictions.Eq(Projections.Property(() => TestSession.Completed), false));
            }
            return junction;
        }
        private Junction GetRequireRolePlayersFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            //Todo: This filter needs to need modified when Test SPECIFICATION Is moved to Test session.
            var required = criteria.ToList<S, bool>().First();

            var filter = Subqueries.WhereProperty<TestSession>(x => CredentialType.Id)
                .In(GetCredentialTypesWithRequiredRolePlayesrs());
            if (!required)
            {
                filter = Restrictions.Not(filter);
            }

            return junction.Add(filter);
        }


        private QueryOver<TestComponent, TestComponent> GetCredentialTypesWithRequiredRolePlayesrs()
        {
            TestComponent fTestComponent = null;
            TestComponentType fTestComponentType = null;
            TestSpecification fTestSpecification = null;

            var query = QueryOver.Of(() => fTestComponent)
                .Inner.JoinAlias(x => fTestComponent.TestSpecification, () => fTestSpecification)
                .Inner.JoinAlias(x => fTestComponent.Type, () => fTestComponentType)
                .Where(x => fTestComponentType.RoleplayersRequired)
                .Where(x => fTestSpecification.Active)
                .Select(Projections.Distinct(Projections.Property(() => fTestSpecification.CredentialType.Id)));
            return query;
        }

        private QueryOver<TestSessionSkill, TestSessionSkill> GetTestSessionIdsWithSkillIds(int[] skillIds)
        {
            TestSession mTestSession = null;
            Skill mSkill = null;
            var query = QueryOver.Of(() => TestSessionSkill)
                .Inner.JoinAlias(x => TestSessionSkill.TestSession, () => mTestSession)
                .Inner.JoinAlias(x => TestSessionSkill.Skill, () => mSkill)
                .Where(Restrictions.In(Projections.Property(() => mSkill.Id), skillIds))
                .Select(Projections.Distinct(Projections.Property(() => mTestSession.Id)));

            return query;
        }
    }
}

