namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormAnswerOptionDocumentType : EntityBase
    {
        public virtual CredentialApplicationFormAnswerOption CredentialApplicationFormAnswerOption { get; set; }
        public virtual DocumentType DocumentType { get; set; }
    }
}
