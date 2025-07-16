namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormQuestionAnswerOption : EntityBase
    {
        public virtual CredentialApplicationFormQuestion CredentialApplicationFormQuestion { get; set; }
        public virtual CredentialApplicationFormAnswerOption CredentialApplicationFormAnswerOption { get; set; }
        public virtual bool DefaultAnswer { get; set; }
        public virtual CredentialApplicationField CredentialApplicationField { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual CredentialApplicationFieldOptionOption CredentialApplicationFieldOptionOption { get; set; }
        public virtual string FieldData { get; set; }
    }
}
