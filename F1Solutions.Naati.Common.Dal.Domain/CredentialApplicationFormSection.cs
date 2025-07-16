using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFormSection : EntityBase
    {
        private IList<CredentialApplicationFormQuestion> mQuestions = new List<CredentialApplicationFormQuestion>();
        public virtual CredentialApplicationForm CredentialApplicationForm { get; set; }
        public virtual string Name { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual string Description { get; set; }
        public virtual IEnumerable<CredentialApplicationFormQuestion> Questions => mQuestions;
    }
}
