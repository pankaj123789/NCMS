namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialWorkflowActionEmailTemplate : EntityBase
    {

        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual SystemActionEmailTemplate SystemActionEmailTemplate { get; set; }
    }
}
