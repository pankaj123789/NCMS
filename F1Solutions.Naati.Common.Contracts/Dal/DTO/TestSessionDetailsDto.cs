using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime TestDate { get; set; }
        public string ApplicationType { get; set; }
        public string CredentialType { get; set; }
        public int? ArrivalTime { get; set; }
        public int? Duration { get; set; }
        public string VenueName { get; set; }
        public int Capacity { get; set; }
        public int Attendees { get; set; }
        public bool Completed { get; set; }
        public string VenueAddress { get; set; }
        public int TestLocationId { get; set; }
        public int VenueId { get; set; }
        public int ApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public bool AllowSelfAssign { get; set; }
        public string PublicNote { get; set; }
        public IEnumerable<TestSessionApplicantDto> TestSessionApplicants { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public bool OverrideVenueCapacity { get; set; }
        public bool NewCandidatesOnly { get; set; }
        public virtual DateTime? RehearsalDateTime { get; set; }
        public virtual string RehearsalNotes { get; set; }
        public bool RolePlayersRequired { get; set; }
        public bool HasRolePLayers { get; set; }
        public bool IsActive { get; set; }


        public int DefaultTestSpecificationId { get; set; }
        public string TestSpecificationDescription { get; set; }
        public bool IsActiveTestSpecification { get; set; }
    }
}