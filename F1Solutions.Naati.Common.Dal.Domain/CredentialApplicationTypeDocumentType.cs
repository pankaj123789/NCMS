namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationTypeDocumentType : EntityBase
    {
        public virtual CredentialApplicationType CredentialApplicationType { get; set; }

        public virtual DocumentType DocumentType { get; set; }

        public virtual bool Mandatory { get; set; }
    }
}
