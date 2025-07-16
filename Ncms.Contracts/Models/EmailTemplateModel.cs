using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models
{
    public class EmailTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public bool Active { get; set; }
        public string Content { get; set; }
        public string FromAddress { get; set; }
        public IEnumerable<EmailTemplateDetailTypeName> EmailTemplateDetails { get; set; }
        public virtual SystemActionEventTypeName SystemActionEventType { get; set; }
    }
}