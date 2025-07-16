using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionSkillInfoDto
    {
        public int TestSessionId { get; set; }
        public int SkillId { get; set; }
        public bool OverrideCapacity { get; set; }
        public int? OverridenCapacity { get; set; }
        public int? SkillCapacity { get; set; }
        public int VenueCapacity { get; set; }
        public int AllocatedSeatsForSkill { get; set; }
        public int TotalAllocatedSeats { get; set; }
        public DateTime TestDateTime { get; set; }
        public int? Duration { get; set; }
        public string VenueName { get; set; }
        public string CredentialTypeName { get; set; }
        public string SkillName { get; set; }
        public int TestLocationId { get; set; }
    }
}