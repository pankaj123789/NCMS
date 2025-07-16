using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialRequestDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string CredentialName { get; set; }
        public string ExternalCredentialName { get; set; }
        public string Direction { get; set; }
        public int DirectionTypeId { get; set; }
        public string Status { get; set; }
        public bool? AutoCreated { get; set; }
        public int CredentialApplicationId { get; set; }
        public string ApplicationTypeDisplayName { get; set; }
        public int StatusTypeId { get; set; }
        public int SkillId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public string ModifiedBy { get; set; }
        public int StatusChangeUserId { get; set; }
        public int CredentialTypeId { get; set; }
        public bool Supplementary { get; set; }
        public int CredentialRequestPathTypeId { get; set; }
        public int SkillLanguage1Id { get; set; }
        public CredentialTypeDto CredentialType { get; set; }
        public SkillDto Skill { get; set; }
        public IEnumerable<CredentialRequestFieldDataDto> Fields { get; set; }
        public IEnumerable<CredentialDto> Credentials { get; set; }
        public IEnumerable<CredentialWorkflowFeeDto> CredentialWorkflowFees { get; set; }
        public IEnumerable<CredentialRequestTestSessionDto> TestSessions { get; set; }
        public bool Certification { get; set; }
        public int? ConcededFromCredentialRequestId { get; set; }
        public IEnumerable<RefundDto> RefundRequests { get; set; }
        public IEnumerable<TestSittingHistoryItemDto> TestSittings { get; set; }
    }
}
