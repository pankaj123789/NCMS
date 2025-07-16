using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrUpdateTestSessionRequest
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; }
        public DateTime TestDateTime { get; set; }
        public DateTime? RehearsalDateTime { get; set; }
        public string RehearsalNotes { get; set; }
        public int? ArrivalTime { get; set; }
        public int? Duration { get; set; }
        public int CredentialTypeId { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public bool Completed { get; set; }
        public string PublicNote { get; set; }
        public bool AllowSelfAssign { get; set; }
        public TestSessionSkillDto[] Skills { get; set; }
        public bool OverrideVenueCapacity { get; set; }
        public bool NewCandidatesOnly { get; set; }
        public int Capacity { get; set; }
        public int DefaultTestSpecificationId { get; set; }
        public bool AllowAvailabilityNotice { get; set; }
        public bool IsActive { get; set; }

    }
}