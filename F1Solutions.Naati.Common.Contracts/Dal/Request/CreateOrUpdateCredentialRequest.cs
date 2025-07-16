using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrUpdateCredentialRequest
    {
        public int CredentialId { get; set; }
        public int CredentialRequestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool ShowInOnlineDirectory { get; set; }
        public CertificationPeriodDto CertificationPeriod { get; set; }
    }
}