using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionSearchResultDto
    {
        public virtual DateTime? TestDate { get; set; }
        public string Venue { get; set; }
        public virtual int TestSessionId { get; set; }
        public virtual int CredentialTypeId { get; set; }
        public virtual string CredentialTypeInternalName { get; set; }
        public virtual string CredentialTypeExternalName { get; set; }
        public virtual int Allocated { get; set; }
        public virtual int Accepted { get; set; }
        public virtual int PendingToAccept { get; set; }
        public virtual int Rejected { get; set; }
        public virtual int Capacity { get; set; }
        public virtual bool Completed { get; set; }
        public virtual string TestLocationName { get; set; }
        public virtual string TestLocationStateName { get; set; }
        public virtual string SessionName { get; set; }
        public virtual int? Duration { get; set; }
        public virtual string SkillDisplayName { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
