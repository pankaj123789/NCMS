using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RecertificationDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int CertificationPeriodId { get; set; }
        public CredentialApplicationStatusTypeName CredentialApplicationStatus { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}