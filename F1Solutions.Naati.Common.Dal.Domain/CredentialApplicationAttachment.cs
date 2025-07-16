namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationAttachment : EntityBase
    {
        public virtual CredentialApplication CredentialApplication { get; set; }

        public virtual StoredFile StoredFile { get; set; }

        public virtual string Description { get; set; }

        public override IAuditObject RootAuditObject => CredentialApplication;
    }
}
