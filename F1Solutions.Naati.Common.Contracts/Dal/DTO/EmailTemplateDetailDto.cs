using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EmailTemplateDetailDto : EmailTemplateDto
    {
        public string Content { get; set; }
        public string FromAddress { get; set; }
        public IEnumerable<EmailTemplateDetailTypeName> EmailTemplateDetails { get; set; }
        public virtual SystemActionEventTypeName SystemActionEventType { get; set; }
    }
}