namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormAnswerOptionActionType : EntityBase
    {
        public virtual CredentialApplicationFormAnswerOption CredentialApplicationFormAnswerOption { get; set; }
        public virtual CredentialApplicationFormActionType CredentialApplicationFormActionType { get; set; }
        public virtual  string Parameter { get; set; }
        public virtual  int Order { get; set; }
    }
}
