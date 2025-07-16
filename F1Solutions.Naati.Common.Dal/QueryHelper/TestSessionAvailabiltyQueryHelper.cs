using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    public class TestSessionAvailabiltyQueryHelper : QuerySearchHelper
    {

        public bool IsAvailableTestSession(int credentialTypeId, int skillId, int testSessionId, int credentialStatusTypeId, int preferredTestLocationId)
        {
            var testFeePaid = credentialStatusTypeId == (int)CredentialRequestStatusTypeName.TestSessionAccepted || credentialStatusTypeId == (int)CredentialRequestStatusTypeName.TestAccepted;
            var availableTestSession = GetAllAvailableTestSessions(skillId, credentialTypeId, preferredTestLocationId, testFeePaid, testSessionId);

            return availableTestSession.Any();
        }

        public IList<AvailableTestSessionDto> GetAllAvailableTestSessions(int credentialRequestId, int testSessionId = 0)
        {
            var credentialRequest = NHibernateSession.Current.Load<CredentialRequest>(credentialRequestId);
            var skillId = credentialRequest.Skill.Id;
            var credentialType = NHibernateSession.Current.Get<CredentialType>(credentialRequest.CredentialType.Id);
            var preferredTestLocationId = credentialRequest.CredentialApplication.PreferredTestLocation?.Id ?? 0;
            var testFeePaid = credentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.TestSessionAccepted || credentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.TestAccepted;

            var allAvailableTestSessions = GetAllAvailableTestSessions(skillId, credentialType.Id, preferredTestLocationId, testFeePaid, testSessionId);

            if (allAvailableTestSessions.Any())
            {
                //filter based on NewCandidatesOnly flag
                var isResit = IsResit(credentialRequest);
                allAvailableTestSessions = allAvailableTestSessions.Where(x => (x.NewCandidatesOnly == 1 && !isResit) || x.NewCandidatesOnly == 0).ToList();
            }

            return allAvailableTestSessions;
        }
        public IList<AvailableTestSessionDto> GetAllAvailableTestSessions(int skillId, int credentialTypeId, int preferredTestLocationId, bool testFeePaid, int testSessionId = 0, DateTime? fromTestDate = null, DateTime? toTestDate = null)
        {
            var credentialType = NHibernateSession.Current.Load<CredentialType>(credentialTypeId);

            var testSessionBookingAvailabilityDays = credentialType.TestSessionBookingAvailabilityWeeks * 7;
            var testSessionBookingClosedDays = credentialType.TestSessionBookingClosedWeeks * 7;
            var testSesssionRejectHours = credentialType.TestSessionBookingRejectHours;

            var testFeePaidProperty = Projections.Constant(testFeePaid, NHibernateUtil.Boolean);
            var testSessionDateProperty = GetDateProjectionFrom(Projections.Property(() => TestSession.TestDateTime));
            var testDateTimeProperty = Projections.Property(() => TestSession.TestDateTime);


            var availabilityDate = DateTime.Now.Date.AddDays(testSessionBookingAvailabilityDays);
            var closingDate = DateTime.Now.Date.AddDays(testSessionBookingClosedDays);

            //Validate is the test session is available
            var isAvailableSessionDate = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => TestSession.AllowSelfAssign), true))
                .Add(Restrictions.Eq(Projections.Property(() => TestSession.Completed), false))
                .Add(Restrictions.Eq(Projections.Property(() => TestSession.CredentialType.Id), credentialTypeId))
                .Add(Restrictions.Ge(testSessionDateProperty, closingDate))
               .Add(Restrictions.Le(testSessionDateProperty, availabilityDate))
               .Add(Restrictions.Eq(Projections.Property(() => TestSession.IsActive), true));

            if (fromTestDate.HasValue)
            {
                isAvailableSessionDate.Add(Restrictions.Ge(testDateTimeProperty, fromTestDate.Value));
            }

            if (toTestDate.HasValue)
            {
                isAvailableSessionDate.Add(Restrictions.Le(testDateTimeProperty, toTestDate.Value));
            }

            if (testSessionId > 0)
            {
                isAvailableSessionDate = isAvailableSessionDate.Add(
                    (Restrictions.Eq(
                        Projections.Property(() => TestSession.Id),
                        testSessionId)));
            }

            //filter by skill id of the credential request
            var filter = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => Skill.Id), skillId));

            Skill mcredentialRequestSkill = null;

            var credentialSkillId = Projections.Property(() => mcredentialRequestSkill.Id);

            //Calculate the total os the sitting with the specific skill
            var skillsTestSittingCount = Projections.Count(
                Projections.Distinct(
                    Projections.Conditional(
                      Restrictions.Conjunction().Add(Restrictions.IsNotNull(credentialSkillId)).Add(Restrictions.Eq(credentialSkillId, skillId)),
                        Projections.Property(() => TestSitting.Id),
                        Projections.Constant(null, NHibernateUtil.Int32))));

            // Replace the total of test sitting for the skill with 0 when the count is null
            var totalSkillSittings = Projections.Conditional(Restrictions.IsNotNull(skillsTestSittingCount), skillsTestSittingCount, Projections.Constant(0, NHibernateUtil.Int32));

            var sessionTestSittingCount = Projections.Count(Projections.Distinct(Projections.Property(() => TestSitting.Id)));

            var totaltestSessionSittings = Projections.Conditional(Restrictions.IsNotNull(sessionTestSittingCount), sessionTestSittingCount, Projections.Constant(0, NHibernateUtil.Int32));

            var skillCapacityProperty = Projections.Property(() => TestSessionSkill.Capacity);

            var testSessionCapacityProjection = Projections.Conditional(
                Restrictions.Eq(Projections.Property(() => TestSession.OverrideVenueCapacity), true),
                Projections.Property(() => TestSession.Capacity),
                Projections.Property(() => Venue.Capacity));

            var testSessionCapacity = Projections.Max(testSessionCapacityProjection);

            var skillCapacity = Projections.Max(Projections.Conditional(
                 Restrictions.Eq(Projections.Property(() => Skill.Id), skillId),
                 Projections.Conditional(Restrictions.IsNotNull(skillCapacityProperty), skillCapacityProperty, testSessionCapacityProjection),
                 Projections.Constant(0, NHibernateUtil.Int32)));

            var availableSkillSeats = SumProjections(skillCapacity, MultiplyProjections(totalSkillSittings, Projections.Constant(-1, NHibernateUtil.Int32)));
            var availableSessionSeats = SumProjections(testSessionCapacity, MultiplyProjections(totaltestSessionSittings, Projections.Constant(-1, NHibernateUtil.Int32)));

            var hasSkillCapacity = Restrictions.Conjunction()
                .Add(Restrictions.Gt(availableSkillSeats, 0))
                .Add(Restrictions.Gt(availableSessionSeats, 0));

            var testLocationNameProperty = Projections.Max(Concatenate(Projections.Property(() => State.Abbreviation),
                Projections.Constant(" - ", NHibernateUtil.AnsiString), Projections.Property(() => TestLocation.Name)));

            var havingFilter = new CustomGroupDisjunction()
                .Add(hasSkillCapacity);

            var testLocationIdProjection = Projections.Max(Projections.Property(() => TestLocation.Id));
            var isPreferedLocationProjection = GetBooleanProjectionFor(Restrictions.Eq(testLocationIdProjection, preferredTestLocationId));

            var availableCapacity =
                Projections.Conditional(Restrictions.GtProperty(availableSkillSeats, availableSessionSeats),
                    availableSessionSeats, availableSkillSeats);

            var rejectionHoursProperty = Projections.Constant(testSesssionRejectHours, NHibernateUtil.Int32);
            AvailableTestSessionDto dto = null;
            var pojections = Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => TestSession.Id)).WithAlias(() => dto.TestSessionId))
                .Add(Projections.Max(Projections.Cast(NHibernateUtil.Int32, Projections.Property(() => TestSession.NewCandidatesOnly))).WithAlias(() => dto.NewCandidatesOnly))
                .Add(testLocationNameProperty.WithAlias(() => dto.TestLocation))
                .Add(Projections.Max(Projections.Property(() => TestSession.Name)).WithAlias(() => dto.Name)) 
                .Add(Projections.Max(Projections.Property(() => TestSession.TestDateTime)).WithAlias(() => dto.TestDateTime))
                .Add(Projections.Max(Projections.Property(() => TestSession.Duration)).WithAlias(() => dto.TestSessionDuration))
                .Add(Projections.Max(Projections.Property(() => Venue.Name)).WithAlias(() => dto.VenueName))
                .Add(Projections.Max(Projections.Property(() => Venue.Address)).WithAlias(() => dto.VenueAddress))
                .Add(rejectionHoursProperty.WithAlias(() => dto.BookingRejectHours))
                .Add(isPreferedLocationProjection.WithAlias(() => dto.IsPreferedLocation))
                .Add(testLocationIdProjection.WithAlias(() => dto.TestLocationId))
                .Add(availableCapacity.WithAlias(() => dto.AvailableSeats))
                .Add(testFeePaidProperty.WithAlias(() => dto.TestFeePaid));

            var query = NHibernateSession.Current.QueryOver(() => TestSessionSkill)
                 .Inner.JoinAlias(x => TestSessionSkill.Skill, () => Skill)
                 .Inner.JoinAlias(x => TestSessionSkill.TestSession, () => TestSession, isAvailableSessionDate)
                 .Inner.JoinAlias(x => TestSession.Venue, () => Venue)
                 .Inner.JoinAlias(x => Venue.TestLocation, () => TestLocation)
                 .Inner.JoinAlias(x => TestLocation.Office, () => Office)
                 .Inner.JoinAlias(x => Office.State, () => State)
                 .Inner.JoinAlias(x => TestSession.CredentialType, () => CredentialType)
                 .Left.JoinAlias(x => TestSession.TestSittings, () => TestSitting, Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false))
                 .Left.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                 .Left.JoinAlias(x => CredentialRequest.Skill, () => mcredentialRequestSkill)
                 .Where(filter)
                 .Where(havingFilter)
                 .Select(pojections);

            var results = query.TransformUsing(Transformers.AliasToBean<AvailableTestSessionDto>())
                .List<AvailableTestSessionDto>();

            return results;

        }

        public bool HasAllAvailableTestSessions(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Load<CredentialRequest>(credentialRequestId);
            var skillId = credentialRequest.Skill.Id;
            var credentialTypeId = credentialRequest.CredentialType.Id;
            var testSessionBookingAvailabilityDays = credentialRequest.CredentialType.TestSessionBookingAvailabilityWeeks * 7;
            var testSessionBookingClosedDays = credentialRequest.CredentialType.TestSessionBookingClosedWeeks * 7;

            var testSessionDateProperty = GetDateProjectionFrom(Projections.Property(() => TestSession.TestDateTime));


            var availabilityDate = DateTime.Now.Date.AddDays(testSessionBookingAvailabilityDays);
            var closingDate = DateTime.Now.Date.AddDays(testSessionBookingClosedDays);

            //Validate is the test session is available
            var isAvailableSessionDate = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => TestSession.AllowSelfAssign), true))
                .Add(Restrictions.Eq(Projections.Property(() => TestSession.Completed), false))
                .Add(Restrictions.Eq(Projections.Property(() => TestSession.CredentialType.Id), credentialTypeId))
                .Add(Restrictions.Ge(testSessionDateProperty, closingDate))
                .Add(Restrictions.Le(testSessionDateProperty, availabilityDate));

            //filter by skill id of the credential request
            var filter = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => Skill.Id), skillId));

            Skill mcredentialRequestSkill = null;

            var credentialSkillId = Projections.Property(() => mcredentialRequestSkill.Id);

            //Calculate the total os the sitting with the specific skill
            var skillsTestSittingCount = Projections.Count(
                Projections.Distinct(
                    Projections.Conditional(
                        Restrictions.Conjunction().Add(Restrictions.IsNotNull(credentialSkillId)).Add(Restrictions.Eq(credentialSkillId, skillId)),
                        Projections.Property(() => TestSitting.Id),
                        Projections.Constant(null, NHibernateUtil.Int32))));

            // Replace the total of test sitting for the skill with 0 when the count is null
            var totalSkillSittings = Projections.Conditional(Restrictions.IsNotNull(skillsTestSittingCount), skillsTestSittingCount, Projections.Constant(0, NHibernateUtil.Int32));

            var sessionTestSittingCount = Projections.Count(Projections.Distinct(Projections.Property(() => TestSitting.Id)));

            var totaltestSessionSittings = Projections.Conditional(Restrictions.IsNotNull(sessionTestSittingCount), sessionTestSittingCount, Projections.Constant(0, NHibernateUtil.Int32));


            var skillCapacityProperty = Projections.Property(() => TestSessionSkill.Capacity);

            var testSessionCapacityProjection = Projections.Conditional(
                Restrictions.Eq(Projections.Property(() => TestSession.OverrideVenueCapacity), true),
                Projections.Property(() => TestSession.Capacity),
                Projections.Property(() => Venue.Capacity));

            var testSessionCapacity = Projections.Max(testSessionCapacityProjection);

            var skillCapacity = Projections.Max(Projections.Conditional(
                Restrictions.Eq(Projections.Property(() => Skill.Id), skillId),
                Projections.Conditional(Restrictions.IsNotNull(skillCapacityProperty), skillCapacityProperty, testSessionCapacityProjection),
                Projections.Constant(0, NHibernateUtil.Int32)));

            var availableSkillSeats = SumProjections(skillCapacity, MultiplyProjections(totalSkillSittings, Projections.Constant(-1, NHibernateUtil.Int32)));
            var availableSessionSeats = SumProjections(testSessionCapacity, MultiplyProjections(totaltestSessionSittings, Projections.Constant(-1, NHibernateUtil.Int32)));

            var hasSkillCapacity = Restrictions.Conjunction()
                .Add(Restrictions.Gt(availableSkillSeats, 0))
                .Add(Restrictions.Gt(availableSessionSeats, 0));


            var havingFilter = new CustomGroupDisjunction()
                .Add(hasSkillCapacity);

            var query = NHibernateSession.Current.QueryOver(() => TestSessionSkill)
                .Inner.JoinAlias(x => TestSessionSkill.Skill, () => Skill)
                .Inner.JoinAlias(x => TestSessionSkill.TestSession, () => TestSession, isAvailableSessionDate)
                .Inner.JoinAlias(x => TestSession.Venue, () => Venue)
                .Inner.JoinAlias(x => Venue.TestLocation, () => TestLocation)
                .Inner.JoinAlias(x => TestLocation.Office, () => Office)
                .Inner.JoinAlias(x => Office.State, () => State)
                .Inner.JoinAlias(x => TestSession.CredentialType, () => CredentialType)
                .Left.JoinAlias(x => TestSession.TestSittings, () => TestSitting,
                    Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false))
                .Left.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Left.JoinAlias(x => CredentialRequest.Skill, () => mcredentialRequestSkill)
                .Where(filter)
                .Select(Projections.GroupProperty(Projections.Property(() => TestSession.Id)))
                .Where(havingFilter);


            var results = query.List<object>();

            var anyresults = results.Any();
            //sticking this here rather than trying to maintain the mass of code above. Task 171562
            if (anyresults)
            {
                anyresults = false;

                bool isResit = IsResit(credentialRequest);

                foreach (int testSessionId in results)
                {
                    if ((NHibernateSession.Current.Query<TestSession>().Any(x => (x.NewCandidatesOnly.HasValue && x.NewCandidatesOnly.Value && x.Id == testSessionId && !isResit)
                    || ((!x.NewCandidatesOnly.HasValue || !x.NewCandidatesOnly.Value) && x.Id == testSessionId))))
                    {
                        anyresults = true;
                        break;
                    }
                }
            }

            return anyresults;
        }

        private static bool IsResit(CredentialRequest credentialRequest)
        {
            var user = credentialRequest.CredentialApplication.Person;

            var allowedStatuses = new List<CredentialRequestStatusTypeName> {
                    CredentialRequestStatusTypeName.TestFailed,
                    CredentialRequestStatusTypeName.CertificationIssued,
                    CredentialRequestStatusTypeName.TestInvalidated
                };

            var pastCredentialRequests = NHibernateSession.Current.Query<CredentialRequest>().Any(x => x.CredentialApplication.Person.Id == user.Id
                && x.CredentialType.Id == credentialRequest.CredentialType.Id
                && x.Skill.Id == credentialRequest.Skill.Id
                && allowedStatuses.Contains((CredentialRequestStatusTypeName)x.CredentialRequestStatusType.Id));

            var isResit = pastCredentialRequests || credentialRequest.TestSittings.Any(x => x.Sat);
            return isResit;
        }

        public IList<TestSittingBacklog> GetTestSittingBacklog(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(credentialRequestId);
            var skillId = credentialRequest.Skill.Id;
            return GetTestSittingBacklog(skillId, credentialRequestId);
        }
        public IList<TestSittingBacklog> GetTestSittingBacklog(int skillId, int excludedCredentialRequestId)
        {
            TestSittingBacklog mTestSittingBacklog = null;

            var testSittingJoinRestriction = Restrictions.Conjunction()
                .Add(Restrictions.EqProperty(Projections.Property(() => CredentialRequest.Supplementary), Projections.Property(() => TestSitting.Supplementary)))
                .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));

            var activeRequest = GetActiveCredentialRequestStatusRestriction();
            var pendingToAssignTestSession = Restrictions.IsNull(Projections.Property(() => TestSitting.Id));
            var skillIdRestriction = Restrictions.Eq(Projections.Property(() => Skill.Id), skillId);
            var credentialRequestRestriction = Restrictions.Not(Restrictions.Eq(Projections.Property(() => CredentialRequest.Id), excludedCredentialRequestId));

            var applicationCredentialType = Projections.Property(() => CredentialApplicationTypeCredentialType.CredentialType.Id);

            var applicationTypeJoinRestriction = Restrictions.Conjunction()
                .Add(Restrictions.EqProperty(applicationCredentialType, Projections.Property(() => CredentialType.Id)))
                .Add(Restrictions.Eq(Projections.Property(() => CredentialApplicationTypeCredentialType.HasTest), true));

            var filter = Restrictions.Conjunction()
                .Add(activeRequest)
                .Add(credentialRequestRestriction)
                .Add(skillIdRestriction)
                .Add(pendingToAssignTestSession);

            var projections = Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => PreferredTestLocation.Id)).WithAlias(() => mTestSittingBacklog.TestLocationId))
                .Add(Projections.Count(Projections.Distinct(Projections.Property(() => CredentialRequest.Id))).WithAlias(() => mTestSittingBacklog.Backlog));

            var query = NHibernateSession.Current.QueryOver(() => CredentialRequest)
                .Inner.JoinAlias(() => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(() => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(() => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(() => CredentialApplication.CredentialApplicationType, () => CredentialApplicationType)
                .Inner.JoinAlias(() => CredentialApplicationType.CredentialApplicationTypeCredentialTypes, () => CredentialApplicationTypeCredentialType, applicationTypeJoinRestriction)
                .Inner.JoinAlias(() => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Inner.JoinAlias(() => CredentialApplication.PreferredTestLocation, () => PreferredTestLocation)
                .Left.JoinAlias(() => CredentialRequest.TestSittings, () => TestSitting, testSittingJoinRestriction)
                .Where(filter)
                .Select(projections);

            var result = query.TransformUsing(Transformers.AliasToBean<TestSittingBacklog>()).List<TestSittingBacklog>();
            return result;

        }

        private ICriterion GetActiveCredentialRequestStatusRestriction()
        {
            var statusIdProjection = Projections.Property(() => CredentialRequestStatusType.Id);
            var statuses = new List<int> { (int)CredentialRequestStatusTypeName.Draft, (int)CredentialRequestStatusTypeName.Cancelled, (int)CredentialRequestStatusTypeName.Deleted, (int)CredentialRequestStatusTypeName.Withdrawn, (int)CredentialRequestStatusTypeName.Rejected };
            return Restrictions.Not(Restrictions.In(statusIdProjection, statuses));
        }
    }
}
