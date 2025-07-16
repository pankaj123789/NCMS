namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationNote : EntityBase
    {
        public virtual CredentialApplication CredentialApplication { get; set; }
        public virtual Note Note { get; set; }

        public override IAuditObject RootAuditObject => CredentialApplication;
    }
}
