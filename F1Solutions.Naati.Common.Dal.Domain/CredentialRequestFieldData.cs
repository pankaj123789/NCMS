namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialRequestFieldData : EntityBase
    {
        public virtual CredentialRequest CredentialRequest { get; set; }
        public virtual CredentialApplicationField CredentialApplicationField { get; set; }
        public virtual string Value { get; set; }
    }
}
