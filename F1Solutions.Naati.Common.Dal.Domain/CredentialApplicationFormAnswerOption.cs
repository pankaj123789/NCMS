using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormAnswerOption : EntityBase
    {
        private IList<CredentialApplicationFormAnswerOptionActionType> mActions = new List<CredentialApplicationFormAnswerOptionActionType>();
        private IList<CredentialApplicationFormAnswerOptionDocumentType> mDocuments = new List<CredentialApplicationFormAnswerOptionDocumentType>();
        public virtual string Option { get; set; }
        public virtual CredentialApplicationFormQuestionType CredentialApplicationFormQuestionType { get; set; }
        public virtual string Description { get; set; }
        public virtual IEnumerable<CredentialApplicationFormAnswerOptionActionType> Actions => mActions;
        public virtual IEnumerable<CredentialApplicationFormAnswerOptionDocumentType> Documents => mDocuments;
    }
}
