namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class IssuedCredentialCredentialRequest : EntityBase
    {
        public virtual Credential Credential { get; set; }
        public virtual CredentialRequest CredentialRequest { get; set; }
    }
}
