namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialTypeDowngradePath : EntityBase
    {
        public virtual CredentialType CredentialTypeFrom { get; set; }
        public virtual CredentialType CredentialTypeTo { get; set; }
     
    }
}