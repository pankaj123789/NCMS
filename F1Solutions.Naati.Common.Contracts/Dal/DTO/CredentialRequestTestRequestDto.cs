using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialRequestTestRequestDto
    {
        public int Id { get; set; }        
        public string Status { get; set; }        
        public string CredentialTypeDisplayName { get; set; }
        public string SkillTypeDisplayName { get; set; }
        public string SkillDisplayName { get; set; }
        public string VenueName { get; set; }
        public int Capacity { get; set; }
        public string TestSessionName { get; set; }
        public DateTime TestDate { get; set; }
        public string ModifiedBy { get; set; }
        public int? Duration { get; set; }
        public int Attendees { get; set; }
        public int Allocated { get; set; }
        public int Accepted { get; set; }
        public bool Completed { get; set; }
        public int RejectedCount { get; set; }
        public bool TestSessionActive { get; set; }
    }
}
