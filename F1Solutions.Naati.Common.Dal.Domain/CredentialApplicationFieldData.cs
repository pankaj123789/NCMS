namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFieldData : EntityBase
    {
        public virtual CredentialApplication CredentialApplication { get; set; }
        public virtual CredentialApplicationField CredentialApplicationField { get; set; }
        public virtual string Value { get; set; }
        public virtual CredentialApplicationFieldOptionOption CredentialApplicationFieldOptionOption { get; set; }

        public override IAuditObject RootAuditObject => CredentialApplication;
    }
}
