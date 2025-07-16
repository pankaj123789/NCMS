using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSearchResultDto
    {
        public int AttendanceId { get; set; }
        public int? TestResultId { get; set; }
        public int CredentialRequestId { get; set; }
        public int TestSessionId { get; set; }
        public int SkillId { get; set; }
        public int TestOfficeId { get; set; }
        public int CredentialTypeId { get; set; }
        public int StatusTypeId { get; set; }
        public int? JobId { get; set; }
        public string Skill { get; set; }
        public bool HasAssets { get; set; }
        public bool HasExaminers { get; set; }
        public bool Supplementary { get; set; }
        public int NaatiNumber { get; set; }
        public string Status { get; set; }
        public string Office { get; set; }
        public string PersonName { get; set; }
        public  DateTime TestDate { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public bool ElilgibleForSupplementary { get; set; }
        public bool ElilgibleForPaidReview { get; set; }
    }
}
