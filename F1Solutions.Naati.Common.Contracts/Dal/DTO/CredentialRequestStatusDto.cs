using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialRequestStatusDto
    {
        public int SkillId { get; set; }
        public int CredentialTypeId { get; set; }
        public int CredentialRequestId { get; set; }
        public CredentialRequestStatusTypeName Status { get; set; }
        public CredentialRequestPathTypeName Path { get; set; }
        public CredentialApplicationStatusTypeName ApplicationStatus { get; set; }
    }
}