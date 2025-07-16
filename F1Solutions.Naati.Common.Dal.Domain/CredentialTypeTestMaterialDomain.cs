namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialTypeTestMaterialDomain : EntityBase
    {
        public virtual CredentialType CredentialType { get; set; }
        public virtual TestMaterialDomain TestMaterialDomain { get; set; }
    }
}
