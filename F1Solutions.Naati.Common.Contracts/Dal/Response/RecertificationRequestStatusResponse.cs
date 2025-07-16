using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class RecertificationRequestStatusResponse
    {
        public CertificationPeriodDto CertificationPeriod { get; set; }
        public RecertificationDto RecertificationInfo { get; set; }
        public DateTime? CredentialTerminationDate { get; set; }
        public CredentialRequestInfoDto SubmitedRecertificationRequest { get; set; }
        public bool AllowRecertification { get; set; }
    }
}