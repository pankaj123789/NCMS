using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateCertificationPeriodRequest
    {
        public int PersonId { get; set; }
        public int CredentialApplicationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime OriginalEndDate { get; set; }
    }
}