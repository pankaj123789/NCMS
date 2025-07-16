using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;

namespace Ncms.Contracts
{
    public class  ServiceEmailTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public string FromAddress { get; set; }
        public IEnumerable<WorkflowModel> WorkflowModels { get; set; }
    }

    public class WorkflowModel
    {
        public IEnumerable<LookupTypeModel> ApplicationTypes { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowDisplayName { get; set; }
        public int Id { get; set; }
        public IEnumerable<LookupTypeModel> EmailTemplateDetails { get; set; }
        public bool Active { get; set; }
        public LookupTypeModel EventType { get; set; }
    }

    public interface IEmailTemplateService
    {
        GenericResponse<ServiceEmailTemplateModel> Get(int id);
        void Save(ServiceEmailTemplateModel model);
        GenericResponse<IList<ServiceEmailTemplateModel>> GetAllEmailTemplates(SearchRequest reuqest);
    }
}

