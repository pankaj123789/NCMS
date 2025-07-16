using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialCertificationPeriodDetailsDto : CertificationPeriodDetailsDto
    {
        public IEnumerable<CredentialsDetailsDto> SubmittedCredentials { get; set; }
        public IEnumerable<CredentialsDetailsDto> Credentials { get; set; }
        public bool IsCredentialSubmitted { get; set; }
    }
}