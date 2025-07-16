using System;

namespace Ncms.Contracts.Models.Test
{
    public class TestSessionDetails
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime? TestDate { get; set; }
        public string TestTime { get; set; }
        public string ApplicationType { get; set; }
        public string CredentialType { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string TestSpecificationDescription { get; set; }
        public bool IsActiveTestSpecification { get; set; }
        public string ArrivalTime { get; set; }
        public string TestEnd { get; set; }
        public string VenueName { get; set; }
        public int Capacity { get; set; }
        public int Attendees { get; set; }
        public bool IsCompletedStatus { get; set; }
        public string VenueAddress { get; set; }

        public int ConfirmedAttendees { get; set; }
        public int AwaitingPaymentAttendees { get; set; }
        public int ProcessingInvoiceAttendees { get; set; }
        public int SatAttendees { get; set; }
        public int CheckedInAttendees { get; set; }

        public int TestLocationId { get; set; }
        public int? VenueId { get; set; }
        public int ApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public bool AllowSelfAssign { get; set; }
        public int? PreparationTime { get; set; }
        public int? SessionDuration { get; set; }
        public string PublicNote { get; set; }
		public bool OverrideVenueCapacity { get; set; }
        public bool NewCandidatesOnly { get; set; }
        public bool IsActive { get; set; }

        public string RehearsalTime { get; set; }
        public DateTime? RehearsalDate { get; set; }
        public string RehearsalNotes { get; set; }
        public bool RolePlayersRequired { get; set; }
        public bool HasRolePLayers { get; set; }
        public int DefaultTestSpecificationId { get; set; }
        public bool IsPastTestSession { get; set; }
    }

    public class TestSessionRequest
    {
        public int[] SkillIds { get; set; }

        public TestSessionSkillModel[] Skills { get; set; }
        public int[] CredentialRequestIds { get; set; }

        public int CredentialTypeId { get; set; }
        public int ApplicationTypeId { get; set; }
        public int TestLocationId { get; set; }
    }

    public class TestSessionSkillValidationRequest : TestSessionRequest
    {
        public int VenueId { get; set; }

        public int TestSessionId { get; set; }

        public int Capacity { get; set; }

        public DateTime? RehearsalDate { get; set; }
        public string RehearsalTime { get; set; }
    }

    public class SessionStepValidationModel
    {
        public int StepId { get; set; }
        public int TestSessionId { get; set; }
        public int CredentialTypeId { get; set; }
    }

    public class RolePlayerAssignmentRequest
    {
        public int TestSessionId { get; set; }
        public int TestSpecificationId { get; set; }
        public int SkillId { get; set; }

        public TestSessionRolePlayerAvailabilityModel[] RolePlayers { get; set; }

    }

    public class AllocateTestSessionRequest
    {
        public int CredentialRequestId { get; set; }
        public int TestSessionId { get; set; }
        public int UserId { get; set; }
    }

}
