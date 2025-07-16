namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialFeeProduct : EntityBase
    {
        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual CredentialType CredentialType { get; set; }
        public virtual FeeType FeeType { get; set; }
        public virtual ProductSpecification ProductSpecification { get; set; }
        public virtual CredentialApplicationRefundPolicy CredentialApplicationRefundPolicy { get; set; }
    }
}
