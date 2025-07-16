using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class EmailTemplateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public string FromAddress { get; set; }
        public IEnumerable<WorkflowResponse> WorkflowModels { get; set; }
    }
}