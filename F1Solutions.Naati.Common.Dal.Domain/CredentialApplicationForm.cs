using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationForm : EntityBase
    {
        private IList<CredentialApplicationFormSection> mSections = new List<CredentialApplicationFormSection>();
        private IList<CredentialApplicationFormCredentialType> mFormCredentialTypes = new List<CredentialApplicationFormCredentialType>();

        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual CredentialApplicationFormUserType CredentialApplicationFormUserType { get; set; }
        public virtual string Url { get; set; }
        public virtual bool Inactive { get; set; }

		public virtual IEnumerable<CredentialApplicationFormSection> Sections => mSections;
        public virtual IEnumerable<CredentialApplicationFormCredentialType> FormCredentialTypes => mFormCredentialTypes;
    }
}
