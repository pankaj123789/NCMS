using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetEmailTemplateRequest
    {
        public SystemActionTypeName Action { get; set; }
        public CredentialApplicationTypeName ApplicationType { get; set; }
        public IEnumerable<SystemActionEventTypeName> ActionEvents { get; set; }
    }
}