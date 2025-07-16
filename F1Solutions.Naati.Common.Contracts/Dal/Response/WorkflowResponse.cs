using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class WorkflowResponse
    {
        public IEnumerable<LookupTypeDto> ApplicationTypes { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowDisplayName { get; set; }
        public LookupTypeDto EventType { get; set; }
        public int Id { get; set; }
        public IEnumerable<LookupTypeDto> EmailTemplateDetails { get; set; }
        public bool Active { get; set; }
    }
}