namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialAttachment : EntityBase
    {
        public virtual Credential Credential { get; set; }
        public virtual StoredFile StoredFile { get; set; }
        public virtual string DocumentNumber { get; set; }
    }
}
