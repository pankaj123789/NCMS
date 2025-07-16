using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SystemActionEmailTemplate : EntityBase
    {
        private IList<SystemActionEmailTemplateDetail> mActionEmailTemplateDetails = new List<SystemActionEmailTemplateDetail>();
        private IList<CredentialWorkflowActionEmailTemplate> mCredentialWorkflowActionEmailTemplates = new List<CredentialWorkflowActionEmailTemplate>();
      
        public virtual SystemActionType SystemActionType { get; set; }
        public virtual SystemActionEventType SystemActionEventType { get; set; }
        public virtual EmailTemplate EmailTemplate { get; set; }

        public virtual IEnumerable<SystemActionEmailTemplateDetail> ActionEmailTemplateDetails =>
            mActionEmailTemplateDetails;

        public virtual IEnumerable<EmailTemplateDetailType> EmailTemplateDetails => ActionEmailTemplateDetails.Select(x=> x.EmailTemplateDetailType);

        public virtual IEnumerable<CredentialWorkflowActionEmailTemplate> CredentialWorkflowActionEmailTemplates => mCredentialWorkflowActionEmailTemplates;
        public  virtual bool Active { get; set; }
    }
}