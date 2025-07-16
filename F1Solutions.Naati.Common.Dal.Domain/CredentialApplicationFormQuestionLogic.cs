namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormQuestionLogic : EntityBase
    {
        public  virtual CredentialApplicationFormQuestionAnswerOption CredentialApplicationFormQuestionAnswerOption { get; set; }
        public virtual CredentialType CredentialType { get; set; }
     
        public virtual CredentialRequestPathType CredentialRequestPathType { get; set; }
        public virtual CredentialApplicationFormQuestion CredentialApplicationFormQuestion { get; set; }
        public  virtual bool Not { get; set; }
        public  virtual bool And { get; set; }
        public virtual int Group { get; set; }

        public virtual int Order { get; set; }
        public virtual bool? PdPointsMet { get; set; }
        public virtual bool? WorkPracticeMet { get; set; }

        public virtual Skill Skill { get; set; }
    }
}
