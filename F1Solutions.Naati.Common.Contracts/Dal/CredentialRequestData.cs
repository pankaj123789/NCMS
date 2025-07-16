using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CredentialRequestData
    {
        public int Id { get; set; }
        public int CredentialTypeId { get; set; }
        public int StatusTypeId { get; set; }
        public int SkillId { get; set; }
        public int? CredentialId { get; set; }
        public int? ConcededFromCredentialRequestId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public string ModifiedBy { get; set; }
        public int CredentialRequestPathTypeId { get; set; }
        public bool Supplementary { get; set; }
        public int StatusChangeUserId { get; set; }
        public IEnumerable<CredentialFieldData> Fields { get; set; }
        public IEnumerable<CredentialDto> Credentials { get; set; }
        public IEnumerable<CredentialRequestTestSessionDto> TestSessions { get; set; }
        public IEnumerable<WorkPracticeData> WorkPractices { get; set; }
        public IEnumerable<CandidateBriefDto> Briefs { get; set; }
        public IList<RefundDto> RefundRequests { get; set; }
        public bool? AutoCreated { get; set; }
    }
}