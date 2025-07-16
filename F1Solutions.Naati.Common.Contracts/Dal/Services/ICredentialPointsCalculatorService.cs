using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.Services
{
    public interface ICredentialPointsCalculatorService
    {
        IEnumerable<WorkPracticeResponse> GetWorkPractices(int credentialId, int naatiNumber, int certificationPeriodId);
        IEnumerable<CredentialCertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber, int credentialId);
        WorkPracticeCredentialDto GetCertificationPeriodCredential(int naatiNumber, int certificationPeriodId, int credentialId);
        IEnumerable<WorkPracticeCredentialDto> GetDefaultPeriodCredentials(int naatiNumber);
    }
}
