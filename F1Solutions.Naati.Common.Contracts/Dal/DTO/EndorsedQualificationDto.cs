using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EndorsedQualificationDto
    {
        public int EndorsedQualificationId { get; set; }
        public int InstitutionId { get; set; }
        public int CredentialTypeId { get; set; }
        public string Qualification { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public DateTime EndorsementPeriodFrom { get; set; }
        public DateTime EndorsementPeriodTo { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public bool Active { get; set; }
    }
}
