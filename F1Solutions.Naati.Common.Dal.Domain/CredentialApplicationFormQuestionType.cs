namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormQuestionType : EntityBase
    {
        public virtual  string Text { get; set; }
        public virtual CredentialApplicationFormAnswerType CredentialApplicationFormAnswerType { get; set; }
        public virtual string Description { get; set; }
    }
}
