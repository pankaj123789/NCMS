using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.ServiceContracts.DTO;
using NAATI.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using SharpArch.Data.NHibernate;

namespace F1Solutions.NAATI.SAM.WebService.ExposedServices
{
    public class TestSessionAvailabiltyQueryHelper : QuerySearchHelper
    {

        public IList<AvailableTestSessionDto> GetAllAvailableTestSessions(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Load<CredentialRequest>(credentialRequestId);
            var skillId = credentialRequest.Skill.Id;
            var credentialTypeId = credentialRequest.CredentialType.Id;
            var preferedTestLocationId = credentialRequest.CredentialApplication.PreferredTestLocation.Id;


            var testSessionBookingAvailabilityDays = Convert.ToInt32(NHibernateSession.Current.Query<SystemValue>()
                                                         .First(x => x.ValueKey == "TestSessionBookingAvailability").Value) * 7;

            var testSessionBookingClosedDays = Convert.ToInt32(NHibernateSession.Current.Query<SystemValue>()
                                                   .First(x => x.ValueKey == "TestSessionBookingClosed").Value) * 7;

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

            var testLocationNameProperty = Projections.Max(Concatenate(Projections.Property(() => State.Abbreviation),
                Projections.Constant(" - ", NHibernateUtil.AnsiString), Projections.Property(() => TestLocation.Name)));

            var havingFilter = new CustomGroupDisjunction()
                .Add(hasSkillCapacity);

            var testLocationIdProjection = Projections.Max(Projections.Property(() => TestLocation.Id));
            var isPreferedLocationProjection = GetBooleanProjectionFor(Restrictions.Eq(testLocationIdProjection, preferedTestLocationId));

            var availableCapacity =
                Projections.Conditional(Restrictions.GtProperty(availableSkillSeats, availableSessionSeats),
                    availableSessionSeats, availableSkillSeats);

            AvailableTestSessionDto dto = null;
            var pojections = Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => TestSession.Id)).WithAlias(() => dto.TestSessionId))
                .Add(testLocationNameProperty.WithAlias(() => dto.TestLocation))
                .Add(Projections.Max(Projections.Property(() => TestSession.Name)).WithAlias(() => dto.Name))
                .Add(Projections.Max(Projections.Property(() => TestSession.TestDateTime)).WithAlias(() => dto.TestDate))
                .Add(Projections.Max(Projections.Property(() => TestSession.Duration)).WithAlias(() => dto.TestSessionDuration))
                .Add(Projections.Max(Projections.Property(() => Venue.Name)).WithAlias(() => dto.VenueName))
                .Add(Projections.Max(Projections.Property(() => Venue.Address)).WithAlias(() => dto.VenueAddress))
                .Add(isPreferedLocationProjection.WithAlias(() => dto.IsPreferedLocation))
                .Add(testLocationIdProjection.WithAlias(() => dto.TestLocationId))
                .Add(availableCapacity.WithAlias(() => dto.AvailableSeats));

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


            var testSessionBookingAvailabilityDays = Convert.ToInt32(NHibernateSession.Current.Query<SystemValue>()
                                                         .First(x => x.ValueKey == "TestSessionBookingAvailability").Value) * 7;

            var testSessionBookingClosedDays = Convert.ToInt32(NHibernateSession.Current.Query<SystemValue>()
                                                   .First(x => x.ValueKey == "TestSessionBookingClosed").Value) * 7;

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
            return results.Any();
        }



    }
}
