namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialTypeTemplate : EntityBase
    {
        public virtual CredentialType CredentialType { get; set; }
        public virtual StoredFile StoredFile { get; set; }
        public virtual  string DocumentNameTemplate { get; set; }
    }
}
