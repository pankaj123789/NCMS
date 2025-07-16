using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionDto
    {
        public int TestSessionId { get; set; }
        public string Name { get; set; }
        public int VenueId { get; set; }
        public DateTime TestDate { get; set; }
        public int? ArrivalTime { get; set; }
        public int? Duration { get; set; }
        public string VenueName { get; set; }
        public int VenueCapacity { get; set; }
        public string VenueAddress { get; set; }
        public string PublicNotes { get; set; }
        public string RehearsalNotes { get; set; }
        public DateTime? RehearsalDateTime { get; set; }
        public bool OverrideCapacity { get; set; }
        public bool NewCandidatesOnly { get; set; }
        public bool IsActive { get; set; }

        public int? OverridenCapacity { get; set; }
        public int CredentialTypeId { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public bool Completed { get; set; }
        public IEnumerable<TestSessionApplicantDto> TestSessionApplicants { get; set; }
        public bool AllowSelfAssign { get; set; }
        public List<SkillDto> Skills { get; set; }
        public bool HasRolePlayers { get; set; }
        public int DefaultTestSpecificationId { get; set; }
        public int MarkingSchemaTypeId { get; set; }
    }
}