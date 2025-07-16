using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialRequestBasicDto
    {
        public int Id { get; set; }
        public int StatusTypeId { get; set; }
        public string ApplicationTypeDisplayName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public bool HasAvailableTestSessions { get; set; }
        public int CredentialApplicationId { get; set; }
        public string SkillDisplayName { get; set; }
        public string Status { get; set; }
        public bool Supplementary { get; set; }
        public int? TestSessionId { get; set; }
        public DateTime? TestDate { get; set; }
        public string VenueName { get; set; }
    }
}
