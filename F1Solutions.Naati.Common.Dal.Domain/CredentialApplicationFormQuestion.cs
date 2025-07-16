using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormQuestion : EntityBase
    {
        private IList<CredentialApplicationFormQuestionAnswerOption> mQuestionAnswerOptions = new List<CredentialApplicationFormQuestionAnswerOption>();
        private IList<CredentialApplicationFormQuestionLogic> mQuestionLogics = new List<CredentialApplicationFormQuestionLogic>();
        public virtual CredentialApplicationFormSection CredentialApplicationFormSection { get; set; }
        public virtual CredentialApplicationFormQuestionType CredentialApplicationFormQuestionType { get; set; }
        public virtual CredentialApplicationField CredentialApplicationField { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual IEnumerable<CredentialApplicationFormQuestionAnswerOption> QuestionAnswerOptions =>
            mQuestionAnswerOptions;

        public virtual IEnumerable<CredentialApplicationFormQuestionLogic> QuestionLogics => mQuestionLogics;
    }
}
