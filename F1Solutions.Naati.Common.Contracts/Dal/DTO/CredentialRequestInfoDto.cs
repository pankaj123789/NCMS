using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialRequestInfoDto
    {
        public int CredentialRequestId { get; set; }
        public CredentialRequestStatusTypeName CredentialRequestStatusType { get; set; }
    }
}