namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialRequestCredentialRequest : EntityBase
    {
        public virtual CredentialRequest OriginalCredentialRequest { get; set; }
        public virtual CredentialRequest AssociatedCredentialRequest { get; set; }
        public virtual CredentialRequestAssociationType AssociationType { get; set; }
    }
}
