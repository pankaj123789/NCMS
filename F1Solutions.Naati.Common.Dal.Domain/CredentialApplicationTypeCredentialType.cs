namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationTypeCredentialType : EntityBase
    {
        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual CredentialType CredentialType { get; set; }
        public virtual bool HasTest { get; set; }
        public virtual bool AllowSupplementary { get; set; }
        public virtual bool AllowPaidReview { get; set; }
    }
}
