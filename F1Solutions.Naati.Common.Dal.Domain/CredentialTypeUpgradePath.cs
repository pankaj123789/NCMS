namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialTypeUpgradePath : EntityBase
    {
        public virtual CredentialType CredentialTypeFrom { get; set; }
        public virtual CredentialType CredentialTypeTo { get; set; }
        public virtual bool MatchDirection { get; set; }
    }
}
