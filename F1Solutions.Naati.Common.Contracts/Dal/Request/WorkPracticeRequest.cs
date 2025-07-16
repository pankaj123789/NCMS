using System;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class WorkPracticeRequest
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        [LegalCharacters('\n', '\r', '\t')]
        public string Description { get; set; }
        public decimal Points { get; set; }
        public int CredentialId { get; set; }
        public int CertificationPeriodId { get; set; }
        public int NaatiNumber { get; set; }
        public int CredentialApplicationId { get; set; }
    }
}