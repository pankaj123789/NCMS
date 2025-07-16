using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetSystemEmailTemplateRequest
    {
        public SystemActionTypeName[] Actions { get; set; }
        public IEnumerable<SystemActionEventTypeName> ActionEvents { get; set; }
    }
}