using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCertificationPeriodsRequest
    {
        public int? PersonId { get; set; }
        public int? NaatiNumber { get; set; }
        public string PractitionerNumber { get; set; }
        public IEnumerable<CertificationPeriodStatus> CertificationPeriodStatus { get; set; }
    }
}