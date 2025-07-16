using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CertificationPeriodDetailsDto : CertificationPeriodDto
    {
        public CertificationPeriodRecertificationStatus RecertificationStatus { get; set; }
        public DateTime RecertificationEligibilityDate { get; set; }
        public bool Editable { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsDefault { get; set; }
        public int? SubmittedRecertificationApplicationId { get; set; }
        public DateTime? RecertificationEnteredDate { get; set; }
    }
}