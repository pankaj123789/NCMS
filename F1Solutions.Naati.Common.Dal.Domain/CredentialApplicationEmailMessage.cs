namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationEmailMessage : EntityBase
    {
        public virtual CredentialApplication CredentialApplication { get; set; }
        public virtual EmailMessage EmailMessage { get; set; }
    }
}
