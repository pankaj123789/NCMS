using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationType : EntityBase, IDynamicLookupType
    {
        private IList<CredentialApplicationTypeCredentialType> mCredentialApplicationTypeCredentialTypes = new List<CredentialApplicationTypeCredentialType>();
        private IList<CredentialApplicationField> mCredentialApplicationFields = new List<CredentialApplicationField>();
        private IList<CredentialApplicationTypeDocumentType> mCredentialApplicationTypeDocumentTypes = new List<CredentialApplicationTypeDocumentType>();
		private IList<CredentialApplicationTypeTestLocation> mLocations = new List<CredentialApplicationTypeTestLocation>();
		private IList<SkillApplicationType> mSkills = new List<SkillApplicationType>();

		public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool Online { get; set; }
        public virtual bool DisplayBills { get; set; }
        public virtual bool RequiresChecking { get; set; }
        public virtual bool RequiresAssessment { get; set; }
        public virtual bool BackOffice { get; set; }
        public virtual bool PendingAllowed { get; set; }
        public virtual bool AssessmentReviewAllowed { get; set; }
        public virtual bool AllowMultiple { get; set; }

        public virtual IEnumerable<CredentialApplicationTypeCredentialType> CredentialApplicationTypeCredentialTypes =>
            mCredentialApplicationTypeCredentialTypes;

        public virtual IEnumerable<CredentialApplicationField> CredentialApplicationFields => mCredentialApplicationFields;

        public virtual IEnumerable<CredentialApplicationTypeDocumentType> CredentialApplicationTypeDocumentTypes =>
            mCredentialApplicationTypeDocumentTypes;

		public virtual IEnumerable<CredentialApplicationTypeTestLocation> Locations => mLocations;

		public virtual IEnumerable<SkillApplicationType> Skills => mSkills;

        public virtual CredentialApplicationTypeCategory CredentialApplicationTypeCategory { get; set; }
    }
}
