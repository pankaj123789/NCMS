namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormCredentialType : EntityBase
    {
        public virtual CredentialApplicationForm CredentialApplicationForm { get; set; }
        public virtual CredentialType CredentialType { get; set; }
    }
}
