using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CertificationPeriodDetailsResponse
    {
        public CertificationPeriodDto CertificationPeriod { get; set; }
        public IList<RecertificationDto> Recertifications { get; set; }
    }
}