using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CertificationPeriodDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime OriginalEndDate { get; set; }
        public CertificationPeriodStatus CertificationPeriodStatus { get; set; }
        public int UserId { get; set; }
        public int NaatiNumber { get; set; }
        public string Notes { get; set; }
        public int CredentialApplicationId { get; set; }
    }
}