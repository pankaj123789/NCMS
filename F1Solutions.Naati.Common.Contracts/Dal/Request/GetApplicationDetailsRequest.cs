using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetApplicationDetailsRequest
    {
        public int ApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public int TestSittingId { get; set; }
        public IEnumerable<CredentialRequestStatusTypeName> ExcludedRequestStauses { get; set; }
    }
}