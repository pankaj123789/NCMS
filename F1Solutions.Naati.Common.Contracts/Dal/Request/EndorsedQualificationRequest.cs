using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class EndorsedQualificationRequest
    {
        public int EndorsedQualificationId { get; set; }
        public int InstitutionId { get; set; }
        public int CredentialTypeId { get; set; }
        public string Location { get; set; }
        public string Qualification { get; set; }
        public DateTime EndorsementPeriodFrom { get; set; }
        public DateTime EndorsementPeriodTo { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
    }
}
