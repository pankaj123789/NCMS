using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Services
{
    public interface IActivityPointsCalculatorService
    {
        List<CertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber);
        IEnumerable<ProfessionalDevelopmentActivityDto> GetAllCertificationPeriodActivities(int naatiNumber, int certificationPeriodId);
        PdActivityPoints CaluculatePointsForDefaultPeriod(int naatiNumber);
        PdActivityPoints CaluculatePointsFor(int naatiNumber, int certificationPeriodId);
    }
}
