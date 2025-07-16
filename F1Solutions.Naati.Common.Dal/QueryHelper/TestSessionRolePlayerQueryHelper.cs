using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using Order = NHibernate.Criterion.Order;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    public class TestSessionRolePlayerQueryHelper : QuerySearchHelper
    {

        public IEnumerable<TestSessionSpecificationDetailsDto> GetTestSpecificationWithPendingRolePlayers(int testSessionId)
        {

            TestSession testSession = null;
            TestSpecification testSpecification = null;
            TestComponent testComponent = null;
            TestComponentType testComponentType = null;
            TestSessionRolePlayer testSessionRolePlayer = null;
            RolePlayer rolePlayer = null;
            TestSessionRolePlayerDetail testSessionRolePlayerDetail = null;
            CredentialType creentialType = null;
            TestComponent rolePlayerComponent = null;
            Skill skill = null;
            TestSessionSkill testSessionSkill = null;
            Skill rolePlayerSkill = null;

            var query = NHibernateSession.Current.QueryOver(() => testSession)
                .Inner.JoinAlias(x => testSession.TestSessionSkills, () => testSessionSkill)
                .Inner.JoinAlias(x => testSessionSkill.Skill, () => skill)
                .Inner.JoinAlias(x => testSession.CredentialType, () => creentialType)
                .Inner.JoinAlias(x => creentialType.TestSpecifications, () => testSpecification)
                .Inner.JoinAlias(x => testSpecification.TestComponents, () => testComponent)
                .Inner.JoinAlias(x => testComponent.Type, () => testComponentType)
                .Left.JoinAlias(x => testSession.TestSessionRolePlayers, () => testSessionRolePlayer , ()=> testSessionRolePlayer.Rejected == false)
                .Left.JoinAlias(x => testSessionRolePlayer.Details, () => testSessionRolePlayerDetail, () => skill.Id == testSessionRolePlayerDetail.Skill.Id)
                .Left.JoinAlias(x => testSessionRolePlayerDetail.TestComponent, () => rolePlayerComponent, y => testComponent.Id == rolePlayerComponent.Id )
                .Left.JoinAlias(x => testSessionRolePlayerDetail.Skill, () => rolePlayerSkill, y => skill.Id == rolePlayerSkill.Id)
                .Left.JoinAlias(x => testSessionRolePlayer.RolePlayer, () => rolePlayer);

            query = query
                .Where(x => testSession.Id == testSessionId)
                .Where(x => testComponentType.RoleplayersRequired)
                .Where(x => testSpecification.Active);
               

            query = query.Select(Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => testSpecification.Id)))
                .Add(Projections.GroupProperty(Projections.Property(() => skill.Id)))
                .Add(Projections.GroupProperty(Projections.Property(() => testComponent.Id)))
                .Add(Projections.Max(Projections.Property(() => testSpecification.Description)))
                .Add(Projections.Count(Projections.Distinct(Projections.Property(() => rolePlayerComponent.Id)))));

            var data = query.List<IList>();

            var result = data.GroupBy(x => x[0]).Select(g => new TestSessionSpecificationDetailsDto()
            {
                Id = (int)g.Key,
                Description = g.First()[3] as string,
                NumberOfTasksWithRequiredRolePlayers = g.Count(),
                TasksWithoutRolePlayers = g.Count(w => (int)w[4] == 0)
            });

            return result;
        }


        public IEnumerable<TestSessionSkillDetailsDto> GetTestSkillsWithPendingRolePlayers(int testSessionId, int testSpecificationId)
        {

            TestSession testSession = null;
            TestSpecification testSpecification = null;
            TestComponent testComponent = null;
            TestComponentType testComponentType = null;
            TestSessionRolePlayer testSessionRolePlayer = null;
            RolePlayer rolePlayer = null;
            TestSessionRolePlayerDetail testSessionRolePlayerDetail = null;
            TestSessionSpecificationDetailsDto dto = null;
            TestComponent rolePlayerComponent = null;
            Skill skill = null;
            Skill rolePlayerSkill = null;
            CredentialType credentialType = null;
            TestSessionSkill testSessionSkill = null;

            var query = NHibernateSession.Current.QueryOver(() => testSession)
                .Inner.JoinAlias(x => testSession.TestSessionSkills, () => testSessionSkill)
                .Inner.JoinAlias(x => testSessionSkill.Skill, () => skill)
                .Inner.JoinAlias(x => skill.Language1, () => Language1)
                .Inner.JoinAlias(x => skill.Language2, () => Language2)
                .Inner.JoinAlias(x => skill.DirectionType, () => DirectionType)
                .Inner.JoinAlias(x => testSession.CredentialType, () => credentialType)
                .Inner.JoinAlias(x => credentialType.TestSpecifications, () => testSpecification)
                .Inner.JoinAlias(x => testSpecification.TestComponents, () => testComponent)
                .Inner.JoinAlias(x => testComponent.Type, () => testComponentType)
                .Left.JoinAlias(x => testSession.TestSessionRolePlayers, () => testSessionRolePlayer, () =>  testSessionRolePlayer.Rejected == false)
                .Left.JoinAlias(x => testSessionRolePlayer.Details, () => testSessionRolePlayerDetail, () => skill.Id == testSessionRolePlayerDetail.Skill.Id)
                .Left.JoinAlias(x => testSessionRolePlayerDetail.TestComponent, () => rolePlayerComponent, y => testComponent.Id == rolePlayerComponent.Id)
                .Left.JoinAlias(x => testSessionRolePlayerDetail.Skill, () => rolePlayerSkill, y => skill.Id == rolePlayerSkill.Id)
                .Left.JoinAlias(x => testSessionRolePlayer.RolePlayer, () => rolePlayer);

            query = query
                .Where(x => testSession.Id == testSessionId)
                .Where(x => testComponentType.RoleplayersRequired)
                .Where(x => testSpecification.Id == testSpecificationId);
               


            query = query.Select(Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => skill.Id)))
                .Add(Projections.GroupProperty(Projections.Property(() => testComponent.Id)))
                .Add(Projections.Max(GetDirectionProjection()).WithAlias(() => dto.Description))
                .Add(Projections.Count(Projections.Distinct(Projections.Property(() => rolePlayerComponent.Id)))));

            var data = query.List<IList>();


            var result = data.GroupBy(x => x[0]).Select(g => new TestSessionSkillDetailsDto()
            {
                SkillId = (int)g.Key,
                Description = g.First()[2] as string,
                NumberOfTasksWithRequiredRolePlayers = g.Count(),
                TasksWithoutRolePlayers = g.Count(w => (int)w[3] == 0)
            });

            return result;
        }

        public IEnumerable<TestSessionRolePlayerAvailabilityDto> GetTestSessionRolePlayers(GetTestSessionRolePlayerRequest request)
        {
            var testSpecificationId = request.TestSpecificationId;
            var testSessionId = request.TestSessionId;
            var skillId = request.SkillId;
            var languageId = request.LanguageId;

            var session = NHibernateSession.Current.Get<TestSession>(testSessionId);

            // Start date of the session
            var sessionStartDateTime = Projections.Constant(session.TestDateTime, NHibernateUtil.DateTime);
            var sessionEndDateTime = Projections.Constant(session.TestDateTime.AddMinutes(Convert.ToDouble(session.Duration)), NHibernateUtil.DateTime);

            // The rehearsal date time
            var rehearsalDateTime = Projections.Constant(session.RehearsalDateTime, NHibernateUtil.DateTime);

            // Get session Test Location
            var sessionTestLocation = Projections.Constant(session.Venue.TestLocation.Id, NHibernateUtil.Int32);

            TestSessionRolePlayer rTestSessionRolePlayer = null;
            RolePlayer rRolePlayer = null;
            Person rPerson = null;
            RolePlayerLastAttendedTestSession rRolePlayerLastAttendedTestSession = null;
            TestSession rLasAttendedTestSession = null;
            NaatiEntity rEntity = null;
            ExaminerUnavailable rExaminerUnavailable = null;
            RolePlayerTestLocation rRoleplayerLocation = null;
            TestLocation rTestLocation = null;
    
            LatestPersonName rLatestPersonName = null;
            PersonName rPersonName = null;
            RolePlayerStatusType rRolePlayerStatus = null;
            RolePlayerRoleType rRolePlayerRoleType = null;
            TestComponent rRolePlayerComponent = null;
            TestSessionRolePlayerDetail rTestSessionRolePlayerDetail = null;

            var roleplayersQuery = NHibernateSession.Current.QueryOver(() => rTestSessionRolePlayer)
                .Inner.JoinAlias(x => rTestSessionRolePlayer.RolePlayer, () => rRolePlayer)
                .Inner.JoinAlias(x => rRolePlayer.Person, () => rPerson)
                .Left.JoinAlias(x => rRolePlayer.LastAttendedTestSessions, () => rRolePlayerLastAttendedTestSession)
                .Left.JoinAlias(x => rRolePlayerLastAttendedTestSession.LastAttendedTestSession, () => rLasAttendedTestSession)
                .Inner.JoinAlias(x => rPerson.Entity, () => rEntity)
                .Left.JoinAlias(x => rPerson.ExaminerUnavailable, () => rExaminerUnavailable)
                .Left.JoinAlias(x => rRolePlayer.Locations, () => rRoleplayerLocation)
                .Left.JoinAlias(x => rRoleplayerLocation.TestLocation, () => rTestLocation)
                .Inner.JoinAlias(x => rPerson.LatestPersonName, () => rLatestPersonName)
                .Inner.JoinAlias(x => rTestSessionRolePlayer.RolePlayerStatusType, () => rRolePlayerStatus)
                .Inner.JoinAlias(x => rTestSessionRolePlayerDetail.RolePlayerRoleType, () => rRolePlayerRoleType)
                .Inner.JoinAlias(x => rTestSessionRolePlayerDetail.TestComponent, () => rRolePlayerComponent)
                .Inner.JoinAlias(x => rTestSessionRolePlayer.Details, () => rTestSessionRolePlayerDetail)
                .Inner.JoinAlias(x => rLatestPersonName.PersonName, () => rPersonName);


            roleplayersQuery = roleplayersQuery.Where(x => rTestSessionRolePlayer.TestSession.Id == testSessionId)
                                                .Where(x => rTestSessionRolePlayerDetail.Skill.Id == skillId)
                                                .Where(x => rTestSessionRolePlayerDetail.Language.Id == languageId)
                                                .Where(x => rRolePlayerComponent.TestSpecification.Id == testSpecificationId);
            if (!request.IncludeRejected)
            {
                roleplayersQuery = roleplayersQuery.Where(x => rTestSessionRolePlayer.Rejected == false);
            }

            //Calculate if role-player is available or not
            var reherasalDateWithoutTime = ToDateTime(GetDateProjectionFrom(rehearsalDateTime));

            var rehearsalEndDate = AddMinutes(Projections.Constant(-1, NHibernateUtil.Int32),
                AddDays(Projections.Constant(1, NHibernateUtil.Int32), reherasalDateWithoutTime));

            // Gets the uvailability
            var unavailableStartDate = Projections.Property(() => rExaminerUnavailable.StartDate);

            var unavailableEndDate = ToDateTime(GetDateProjectionFrom(Projections.Property(() => rExaminerUnavailable.EndDate)));
            // Take the end of the unavailability and add 23 hours and 59 minutes
            var unavailableEndDateTime = AddMinutes(Projections.Constant(-1, NHibernateUtil.Int32), AddDays(Projections.Constant(1, NHibernateUtil.Int32), unavailableEndDate));

            var isThereUnavailability = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(unavailableStartDate))
                .Add(Restrictions.IsNotNull(unavailableEndDateTime));

            //Checks if unavailability overlaps with test session rehearsal
            var unavailabityOverlappedWithRehearsaldate = IsItemOverlappedRestriction(rehearsalDateTime,
                rehearsalEndDate, unavailableStartDate, unavailableEndDateTime);

            // Checks if unavailability overlaps with test session 
            var unavailabilityOverlappedWithTestSession = IsItemOverlappedRestriction(sessionStartDateTime,
                sessionEndDateTime, unavailableStartDate, unavailableEndDateTime);

            var isunavailabilityOverlapped = Restrictions.And(isThereUnavailability, Restrictions.Or(unavailabityOverlappedWithRehearsaldate, unavailabilityOverlappedWithTestSession));

            var isAvailableProperty = GetBooleanProjectionFor(Restrictions.Eq(Projections.Sum(GetIntValueProjectionFor(isunavailabilityOverlapped)), 0));

            var rolePlayerTestLocation = Projections.Property(() => rRoleplayerLocation.TestLocation.Id);
            var isInTestLocationRestriction = Restrictions.EqProperty(rolePlayerTestLocation, sessionTestLocation);

            var isInTestLocationProperty = GetBooleanProjectionFor(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(isInTestLocationRestriction)), 0));

            var isSeniorProperty = Restrictions.Eq(Projections.Max(GetIntValueProjectionFor(Restrictions.Eq(Projections.Property(() => rRolePlayer.Senior), true))), 1);
            var isAttendedProperty = Restrictions.Eq(Projections.Max(GetIntValueProjectionFor(Restrictions.Eq(Projections.Property(() => rTestSessionRolePlayer.Attended), true))), 1);
            var isRehearsedProperty = Restrictions.Eq(Projections.Max(GetIntValueProjectionFor(Restrictions.Eq(Projections.Property(() => rTestSessionRolePlayer.Rehearsed), true))), 1);
            var isRejectedProperty = Restrictions.Eq(Projections.Max(GetIntValueProjectionFor(Restrictions.Eq(Projections.Property(() => rTestSessionRolePlayer.Rejected), true))), 1);
            var sessionLimitProperty = Projections.Max(Projections.Property(() => rRolePlayer.SessionLimit));
            var ageProperty = GetAge(Projections.Max(Projections.Property(() => rPerson.BirthDate)));

            var rolePlayerWithValidSessionLimit = Restrictions.Gt(sessionLimitProperty, 0);
            var hasCapacityProperty = GetBooleanProjectionFor(
                Restrictions.Conjunction()
                    .Add(rolePlayerWithValidSessionLimit)
                    .Add(Subqueries.WhereProperty<RolePlayer>(x => rEntity.NaatiNumber).NotIn(GetRolePlayerNaatiNumberBookedWithItsMaxCapacity(session.TestDateTime, session.Id))));

            var testLocationSeparator = ',';
            var genderProperty = Projections.Max(Projections.Property(() => rPerson.Gender));
            var ratingProperty = Projections.Max(Projections.Property(() => rRolePlayer.Rating));
            roleplayersQuery = roleplayersQuery.Select(Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => rEntity.NaatiNumber)))
                .Add(Projections.GroupProperty(Projections.Property(() => rRolePlayerComponent.Id)))
                .Add(Projections.Max(Projections.Property(() => rPersonName.GivenName)))
                .Add(Projections.Max(Projections.Property(() => rPersonName.Surname)))
                .Add(genderProperty)
                .Add(GetBooleanProjectionFor(isSeniorProperty))
                .Add(sessionLimitProperty)
                .Add(ratingProperty)
                .Add(isAvailableProperty)
                .Add(isInTestLocationProperty)
                .Add(GetBooleanProjectionFor(isAttendedProperty))
                .Add(GetBooleanProjectionFor(isRehearsedProperty))
                .Add(GetBooleanProjectionFor(isRejectedProperty))
                .Add(Projections.Max(Projections.Property(() => rTestSessionRolePlayer.Id)))
                .Add(Projections.Max(Projections.Property(() => rRolePlayerStatus.Id)))
                .Add(Projections.Max(Projections.Property(() => rRolePlayerRoleType.Id)))
                .Add(Projections.Max(Projections.Property(() => rTestSessionRolePlayerDetail.Language.Id)))
                .Add(Projections.Max(Projections.Property(() => rRolePlayer.Id)))
                .Add(hasCapacityProperty)
                .Add(StringAgg(Projections.Property(() => rTestLocation.Name), testLocationSeparator.ToString()))
                .Add(StringAgg(Projections.Property(() => rTestLocation.Id), testLocationSeparator.ToString()))
                .Add(ageProperty)
                .Add(Projections.Max(Projections.Property(() => rLasAttendedTestSession.TestDateTime)))
                .Add(Projections.Max(Projections.Property(() => rLasAttendedTestSession.Id))));


            if (request.Skip.HasValue)
            {
                roleplayersQuery.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                roleplayersQuery.Take(request.Take.Value);
            }

            foreach (var sorting in request.Sorting)
            {
                IProjection sortProjection = null;
                switch (sorting.SortType)
                {
                    case RolePlayerSortingType.TestLocation:
                        sortProjection = isInTestLocationProperty;
                        break;
                    case RolePlayerSortingType.Availability:
                        sortProjection = isAvailableProperty;
                        break;
                    case RolePlayerSortingType.Capacity:
                        sortProjection = hasCapacityProperty;
                        break;
                    case RolePlayerSortingType.Gender:
                        sortProjection = genderProperty;
                        break;
                    case RolePlayerSortingType.Rating:
                        sortProjection = ratingProperty;
                        break;
                }

                var order = GetOrdering(sortProjection, sorting.Direction);
                roleplayersQuery.UnderlyingCriteria.AddOrder(order);
            }

            var data = roleplayersQuery.List<IList>();

            var dtos = data.GroupBy(x => x[0])
                .Select(y =>
                    new TestSessionRolePlayerAvailabilityDto
                    {
                        NaatiNumber = (int)y.Key,
                        Details = y.Select(w => new RolePlayerTaskDetailDto
                        {
                            TestComponentId = (int)w[1],
                            RolePlayerRoleTypeId = (int)w[15]
                        }),
                        GivenName = y.First()[2] as string,
                        Surname = y.First()[3] as string,
                        Gender = y.First()[4] as string,
                        Senior = (bool)y.First()[5],
                        SessionLimit = (int)y.First()[6],
                        Rating = (decimal?)y.First()[7],
                        Available = (bool)y.First()[8],
                        IsInTestLocation = (bool)y.First()[9],
                        Attended = (bool)y.First()[10],
                        Rehearsed = (bool)y.First()[11],
                        Rejected = (bool)y.First()[12],
                        TestSessionRolePlayerId = (int)y.First()[13],
                        RolePlayerStatusId = (int)y.First()[14],
                        LanguageId = (int)y.First()[16],
                        RolePlayerId = (int)y.First()[17],
                        HasCapacity = (bool)y.First()[18],
                        AvailableTestLocations = MapTestLocations((y.First()[20] as string), (y.First()[19] as string), testLocationSeparator),
                        Age = (int)y.First()[21],
                        LastAttendedTestSessionId = (int?)y.First()[23],
                        LastAttendedTestSessionDateTime = (DateTime?)y.First()[22],
                    });
            //var result = roleplayersQuery.TransformUsing(Transformers.AliasToBean<TestSessionRolePlayerDto>()).List<TestSessionRolePlayerDto>();
            return dtos;
        }

        private IEnumerable<LookupTypeDto> MapTestLocations(string testLocationIds, string testLocationNames, char separator)
        {
            var lookups = new List<LookupTypeDto>();
            if (string.IsNullOrWhiteSpace(testLocationIds))
            {
                return lookups;
            }

            var list = testLocationIds.Split(separator);
            var nameList = testLocationNames.Split(separator);
            for (var i = 0; i < list.Length; i++)
            {
                lookups.Add(new LookupTypeDto()
                {
                    Id = Convert.ToInt32(list[i]),
                    DisplayName = nameList[i]
                });
            }

            return lookups;
        }

        private Order GetOrdering(IProjection projection, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Descending)
            {
                return Order.Desc(projection);
            }

            return Order.Asc(projection);
        }

        private ICriterion IsItemOverlappedRestriction(IProjection startDate, IProjection endDate,
            IProjection itemStartDate, IProjection itemEndDate)
        {
            var startOverlapping = Restrictions.Conjunction()
                .Add(Restrictions.GeProperty(itemStartDate, startDate))
                .Add(Restrictions.LtProperty(itemStartDate, endDate));

            var endOverlapping = Restrictions.Conjunction()
                .Add(Restrictions.GtProperty(itemEndDate, startDate))
                .Add(Restrictions.LtProperty(itemStartDate, startDate));

            return Restrictions.Disjunction().Add(startOverlapping).Add(endOverlapping);
        }

        public IEnumerable<RolePlayerAvailabilityDto> GetAvailableRolePlayers(GetRolePlayerRequest request)
        {
            var testSessionId = request.TestSessionId;
            var testSpecificationId = request.TestSpecificationId;
            var skillId = request.SkillId;
            var languageId = request.LanguageId;
            var session = NHibernateSession.Current.Get<TestSession>(testSessionId);
            var sessionCredentialType = Projections.Constant(session.CredentialType.Id, NHibernateUtil.Int32);

            // Start date of the session
            var sessionStartDateTime = Projections.Constant(session.TestDateTime, NHibernateUtil.DateTime);
            var sessionEndDateTime = Projections.Constant(session.TestDateTime.AddMinutes(Convert.ToDouble(session.Duration)), NHibernateUtil.DateTime);

            // The rehearsal date time
            var rehearsalDateTime = Projections.Constant(session.RehearsalDateTime, NHibernateUtil.DateTime);

            // Get session Test Location
            var sessionTestLocation = Projections.Constant(session.Venue.TestLocation.Id, NHibernateUtil.Int32);

            // Calculate overelapped roleplayers
            TestSessionRolePlayer otestSessionRolePlayer = null;
            TestSessionRolePlayerDetail otestSessionRolePlayerDetail = null;
            TestComponent oTestComponent = null;
            TestSession otestSession = null;
            RolePlayer oRolePlayer = null;


            var startSession = Projections.Property(() => otestSession.TestDateTime);
            var endSession = AddMinutes(Projections.Property(() => otestSession.Duration), startSession);

            var sessionOverlapping = IsItemOverlappedRestriction(sessionStartDateTime, sessionEndDateTime, startSession, endSession);


            var testSessionProperty = Projections.Property(() => otestSession.Id);
            var notRejectedTestSession = Restrictions.Eq(Projections.Property(() => otestSessionRolePlayer.Rejected), false);
            var rejectedTestSession = Restrictions.Eq(Projections.Property(() => otestSessionRolePlayer.Rejected), true);
            var sameSkill = Restrictions.Eq(Projections.Property(() => otestSessionRolePlayerDetail.Skill.Id), skillId);
            var sameSession = Restrictions.Eq(testSessionProperty, testSessionId);

            var differentTestSession = Restrictions.Conjunction()
                .Add(Restrictions.Not(sameSession))
                .Add(notRejectedTestSession);

            var sameTestSessionAndDifferentSkill = Restrictions.Conjunction()
                .Add(sameSession)
                .Add(Restrictions.Not(sameSkill))
                .Add(notRejectedTestSession);

            var sameSessionAndSkillButRejected  = Restrictions.Conjunction()
                .Add(sameSession)
                .Add(sameSkill)
                .Add(rejectedTestSession);


            var filter = Restrictions.Disjunction().Add(differentTestSession).Add(sameTestSessionAndDifferentSkill).Add(sameSessionAndSkillButRejected);


            var overlappedPersonIds = QueryOver.Of(() => otestSessionRolePlayer)
                .Inner.JoinAlias(x => otestSessionRolePlayer.TestSession, () => otestSession)
                .Inner.JoinAlias(x => otestSessionRolePlayer.Details, () => otestSessionRolePlayerDetail)
                .Inner.JoinAlias(x => otestSessionRolePlayerDetail.TestComponent, () => oTestComponent)
                .Inner.JoinAlias(x => otestSessionRolePlayer.RolePlayer, () => oRolePlayer)
                .Where(filter)
                .Where(sessionOverlapping)
                .Select(x => oRolePlayer.Person.Id);

            Panel pPanel = null;
            PanelMembership pPanelMembership = null;
            PanelRole pPanelRole = null;
            PanelRoleCategory pPanelRoleCategory = null;
            PanelMembershipCredentialType pPanelMembmershipCredentialType = null;
            CredentialType pMembershipCredentialType = null;
            Person pPerson = null;

            var validRolePlayers = QueryOver.Of(() => pPanel)
                .Inner.JoinAlias(x => pPanel.PanelMemberships, () => pPanelMembership)
                .Inner.JoinAlias(x => pPanelMembership.PanelRole, () => pPanelRole)
                .Inner.JoinAlias(x => pPanelRole.PanelRoleCategory, () => pPanelRoleCategory, y => pPanelRoleCategory.Id == (int)PanelRoleCategoryName.RolePlayer)
                .Inner.JoinAlias(x => pPanelMembership.PanelMembershipCredentialTypes, () => pPanelMembmershipCredentialType)
                .Inner.JoinAlias(x => pPanelMembmershipCredentialType.CredentialType, () => pMembershipCredentialType, Restrictions.EqProperty(Projections.Property(() => pMembershipCredentialType.Id), sessionCredentialType))
                .Inner.JoinAlias(x => pPanelMembership.Person, () => pPerson)
                .Where(x => pPanel.Language.Id == languageId)
                .Where(x => pPanelMembership.StartDate <= DateTime.Now)
                .Where(x => pPanelMembership.EndDate > DateTime.Now.Date.AddDays(1))
                .Where(Subqueries.WhereProperty<Panel>(y => pPerson.Id).NotIn(overlappedPersonIds))
                .Select(x => pPerson.Id);

            RolePlayer rRolePlayer = null;
            RolePlayerLastAttendedTestSession rRolePlayerLastAttendedTestSession = null;
            TestSession rLasAttendedTestSession = null;
            Person rPerson = null;
            NaatiEntity rEntity = null;
            ExaminerUnavailable rExaminerUnavailable = null;
            RolePlayerTestLocation rRoleplayerLocation = null;
            TestLocation rTestLocation = null;
           

            LatestPersonName rLatestPersonName = null;
            PersonName rPersonName = null;

            var roleplayersQuery = NHibernateSession.Current.QueryOver(() => rRolePlayer)
                .Inner.JoinAlias(x => rRolePlayer.Person, () => rPerson, Subqueries.WhereProperty<RolePlayer>(x => rPerson.Id).In(validRolePlayers))
                .Left.JoinAlias(x => rRolePlayer.LastAttendedTestSessions, () => rRolePlayerLastAttendedTestSession)
                .Left.JoinAlias(x => rRolePlayerLastAttendedTestSession.LastAttendedTestSession, () => rLasAttendedTestSession)
                .Inner.JoinAlias(x => rPerson.Entity, () => rEntity)
                .Left.JoinAlias(x => rPerson.ExaminerUnavailable, () => rExaminerUnavailable)
                .Left.JoinAlias(x => rRolePlayer.Locations, () => rRoleplayerLocation)
                .Left.JoinAlias(x => rRoleplayerLocation.TestLocation, () => rTestLocation)
                .Inner.JoinAlias(x => rPerson.LatestPersonName, () => rLatestPersonName)
                .Inner.JoinAlias(x => rLatestPersonName.PersonName, () => rPersonName);

            //Calculate if role-player is available or not

            var reherasalDateWithoutTime = ToDateTime(GetDateProjectionFrom(rehearsalDateTime));

            var rehearsalEndDate = AddMinutes(Projections.Constant(-1, NHibernateUtil.Int32),
                AddDays(Projections.Constant(1, NHibernateUtil.Int32), reherasalDateWithoutTime));

            // Gets the uvailability
            var unavailableStartDate = Projections.Property(() => rExaminerUnavailable.StartDate);

            var unavailableEndDate = ToDateTime(GetDateProjectionFrom(Projections.Property(() => rExaminerUnavailable.EndDate)));
            // Take the end of the unavailability and add 23 hours and 59 minutes
            var unavailableEndDateTime = AddMinutes(Projections.Constant(-1, NHibernateUtil.Int32), AddDays(Projections.Constant(1, NHibernateUtil.Int32), unavailableEndDate));

            var isThereUnavailability = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(unavailableStartDate))
                .Add(Restrictions.IsNotNull(unavailableEndDateTime));

            //Checks if unavailability overlaps with test session rehearsal
            var unavailabityOverlappedWithRehearsaldate = IsItemOverlappedRestriction(rehearsalDateTime,
                rehearsalEndDate, unavailableStartDate, unavailableEndDateTime);

            // Checks if unavailability overlaps with test session 
            var unavailabilityOverlappedWithTestSession = IsItemOverlappedRestriction(sessionStartDateTime,
               sessionEndDateTime, unavailableStartDate, unavailableEndDateTime);

            var isunavailabilityOverlapped = Restrictions.And(isThereUnavailability, Restrictions.Or(unavailabityOverlappedWithRehearsaldate, unavailabilityOverlappedWithTestSession));

            var isAvailableProperty = GetBooleanProjectionFor(Restrictions.Eq(Projections.Sum(GetIntValueProjectionFor(isunavailabilityOverlapped)), 0));

            var rolePlayerTestLocation = Projections.Property(() => rRoleplayerLocation.TestLocation.Id);
            var isInTestLocationRestriction = Restrictions.EqProperty(rolePlayerTestLocation, sessionTestLocation);

            var isInTestLocationProperty = GetBooleanProjectionFor(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(isInTestLocationRestriction)), 0));

            var isSeniorProperty = Restrictions.Eq(Projections.Max(GetIntValueProjectionFor(Restrictions.Eq(Projections.Property(() => rRolePlayer.Senior), true))), 1);

            var sessionLimitProperty = Projections.Max(Projections.Property(() => rRolePlayer.SessionLimit));

            var rolePlayerWithValidSessionLimit = Restrictions.Gt(sessionLimitProperty, 0);
            var hasCapacityProperty = GetBooleanProjectionFor(
                Restrictions.Conjunction()
                .Add(rolePlayerWithValidSessionLimit)
                .Add(Subqueries.WhereProperty<RolePlayer>(x => rEntity.NaatiNumber).NotIn(GetRolePlayerNaatiNumberBookedWithItsMaxCapacity(session.TestDateTime, session.Id))));

            var ratingProperty = Projections.Max(Projections.Property(() => rRolePlayer.Rating));
            var genderProperty = Projections.Max(Projections.Property(() => rPerson.Gender));

            var ageProperty = GetAge(Projections.Max(Projections.Property(() => rPerson.BirthDate)));
            var testLocationSeparator = ',';
            roleplayersQuery = roleplayersQuery.Select(Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => rEntity.NaatiNumber)))
                .Add(Projections.Max(Projections.Property(() => rPersonName.GivenName)))
                .Add(Projections.Max(Projections.Property(() => rPersonName.Surname)))
                .Add(genderProperty)
                .Add(GetBooleanProjectionFor(isSeniorProperty))
                .Add(sessionLimitProperty)
                .Add(ratingProperty)
                .Add(isAvailableProperty)
                .Add(Projections.Max(Projections.Property(() => rRolePlayer.Id)))
                .Add(isInTestLocationProperty)
                .Add(hasCapacityProperty)
                .Add(StringAgg(Projections.Property(() => rTestLocation.Name), testLocationSeparator.ToString()))
                .Add(StringAgg(Projections.Property(() => rTestLocation.Id), testLocationSeparator.ToString()))
                .Add(Projections.Constant(languageId, NHibernateUtil.Int32))
                .Add(ageProperty)
                .Add(Projections.Max(Projections.Property(() => rLasAttendedTestSession.TestDateTime)))
                .Add(Projections.Max(Projections.Property(() => rLasAttendedTestSession.Id))));

            if (request.Skip.HasValue)
            {
                roleplayersQuery.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                roleplayersQuery.Take(request.Take.Value);
            }

            foreach (var sorting in request.Sorting)
            {
                IProjection sortProjection = null;
                switch (sorting.SortType)
                {
                    case RolePlayerSortingType.TestLocation:
                        sortProjection = isInTestLocationProperty;
                        break;
                    case RolePlayerSortingType.Availability:
                        sortProjection = isAvailableProperty;
                        break;
                    case RolePlayerSortingType.Capacity:
                        sortProjection = hasCapacityProperty;
                        break;
                    case RolePlayerSortingType.Gender:
                        sortProjection = genderProperty;
                        break;
                    case RolePlayerSortingType.Rating:
                        sortProjection = ratingProperty;
                        break;
                }

                var order = GetOrdering(sortProjection, sorting.Direction);
                roleplayersQuery.UnderlyingCriteria.AddOrder(order);
            }

            var data = roleplayersQuery.List<IList>().Select(x => new RolePlayerAvailabilityDto
            {
                NaatiNumber = (int) x[0],
                GivenName = x[1] as string,
                Surname = x[2] as string,
                Gender = x[3] as string,
                Senior = (bool) x[4],
                SessionLimit = (int) x[5],
                Rating = x[6] as decimal?,
                Available = (bool) x[7],
                RolePlayerId = (int) x[8],
                IsInTestLocation = (bool) x[9],
                HasCapacity = (bool) x[10],
                AvailableTestLocations = MapTestLocations(x[12] as string, x[11] as string, testLocationSeparator),
                LanguageId = (int) x[13],
                Age = (int) x[14],
                LastAttendedTestSessionId = (int?)x[16],
                LastAttendedTestSessionDateTime = (DateTime?)x[15],
            });
            

            return data;

        }


        private QueryOver<TestSessionRolePlayer, TestSessionRolePlayer> GetRolePlayerNaatiNumberBookedWithItsMaxCapacity(DateTime date, int testSessionId)
        {
            TestSessionRolePlayer cTestSesionRolePlayer = null;
            TestSession cTestSesion = null;
            RolePlayer cRolePlayer = null;
            Person cPerson = null;
            NaatiEntity cEntity = null;

            var dateProjection = GetDateProjectionFrom(Projections.Constant(date, NHibernateUtil.Date));
            var startRange = AddDays(Projections.Constant(-15, NHibernateUtil.Int32), dateProjection);
            var endRange = AddDays(Projections.Constant(15, NHibernateUtil.Int32), dateProjection);


            var sessiondate = GetDateProjectionFrom(Projections.Property(() => cTestSesion.TestDateTime));

            var totalSessionAllocation = MultiplyProjections(Projections.Count(
                Projections.Distinct(Projections.Property(() => cTestSesionRolePlayer.Id))), Projections.Constant(-1));

            var sessionLimit = Projections.Max(Projections.Property(() => cRolePlayer.SessionLimit));

            var outOfCapacityFilter = new CustomGroupConjuction().Add(Restrictions.Le(SumProjections(sessionLimit, totalSessionAllocation), 0));

            var query = QueryOver.Of(() => cTestSesionRolePlayer)
                .Inner.JoinAlias(x => cTestSesionRolePlayer.RolePlayer, () => cRolePlayer)
                .Inner.JoinAlias(x => cRolePlayer.Person, () => cPerson)
                .Inner.JoinAlias(x => cPerson.Entity, () => cEntity)
                .Inner.JoinAlias(x => cTestSesionRolePlayer.TestSession, () => cTestSesion)
                .Where(Restrictions.GeProperty(sessiondate, startRange))
                .Where(Restrictions.LeProperty(sessiondate, endRange))
                .Where(x => !cTestSesionRolePlayer.Rejected)
                .Where(x => cTestSesion.Id != testSessionId)
                .Select(Projections.GroupProperty(Projections.Property(() => cEntity.NaatiNumber)))
                .Where(outOfCapacityFilter);

            return query;

        }

    }
}
