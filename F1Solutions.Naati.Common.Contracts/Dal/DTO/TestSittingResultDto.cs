using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSittingResultDto
    {
        public int TestSittingId { get; set; }
        public int? LastTestResultId { get; set; }
        public int CredentialRequestId { get; set; }
        public DateTime TestDate { get; set; }
        public string CredentialTypeDisplayName { get; set; }
        public string VenueName { get; set; }
        public string State { get; set; }
        public string TestLocationName { get; set; }
        public string SkillDisplayName { get; set; }
        public string OverallResult { get; set; }
        public int? ResultTypeId { get; set; }
        public bool EligibleForAPaidTestReview { get; set; }
        public bool EligibleForASupplementaryTest { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public bool Supplementary { get; set; }
        public int? MinStandardMarkForPaidReview { get; set; }
        public DateTime? ResultDate { get; set; }

        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }
    }
}