namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialTypeCrossSkill : EntityBase
    {
        public virtual CredentialType CredentialTypeFrom { get; set; }
        public virtual CredentialType CredentialTypeTo { get; set; }
    }
}
