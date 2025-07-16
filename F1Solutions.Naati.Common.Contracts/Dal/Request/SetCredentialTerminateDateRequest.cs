using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SetCredentialTerminateDateRequest
    {
        public int CredentialId { get; set; }
        public DateTime? NewTerminationDate { get; set; }
        public string Notes { get; set; }
        public int UserId { get; set; }
        public DateTime? CertificationPeriodEndDate { get; set; }
    }
}