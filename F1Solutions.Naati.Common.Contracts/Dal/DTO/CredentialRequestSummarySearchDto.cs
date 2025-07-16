using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialRequestSummarySearchDto
    {
        public int CredentialApplicationTypeId { get; set; }
        public bool? AutoCreated { get; set; }
        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }
        public int TestLocationId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public DateTime EnteredDate { get; set; }
        public string ApplicationType { get; set; }
        public string CredentialType { get; set; }
        public string Skill { get; set; }
        public string PreferredTestLocation { get; set; }
        public int NumberOfApplicants { get; set; }
        public string CredentialRequestStatus { get; set; }
        public string Language1Name { get; set; }
        public string Language2Name { get; set; }
        public string DirectionDisplayName { get; set; }
        public string StateAbbr { get; set; }
        public bool AllowSelfAssign { get; set; }
        public DateTime EarliestApplicationEnteredDate { get; set; }
        public DateTime LastApplicationEnteredDate { get; set; }
    }
}
