using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialType : EntityBase, IDynamicLookupType
    {
        private IList<CredentialApplicationTypeCredentialType> mCredentialApplicationTypeCredentialTypes = new List<CredentialApplicationTypeCredentialType>();
        private IList<CredentialTypeTemplate> mCredentialTypeTemplates = new List<CredentialTypeTemplate>();
        private IList<TestSpecification> mTestSpecifications = new List<TestSpecification>();
        private IList<CredentialTypeDowngradePath> mDowngradePaths = new List<CredentialTypeDowngradePath>();
        public virtual CredentialCategory CredentialCategory { get; set; }
        public virtual string ExternalName { get; set; }
        public virtual string InternalName { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual bool Simultaneous { get; set; }
        public virtual SkillType SkillType { get; set; }
        public virtual bool Certification { get; set; }
        public virtual int? DefaultExpiry { get; set; }
        public virtual IEnumerable<CredentialApplicationTypeCredentialType> CredentialApplicationTypeCredentialTypes =>
            mCredentialApplicationTypeCredentialTypes;

        public virtual IEnumerable<CredentialTypeTemplate> CredentialTypeTemplates => mCredentialTypeTemplates;
        public virtual IEnumerable<TestSpecification> TestSpecifications => mTestSpecifications;
        public virtual IEnumerable<CredentialTypeDowngradePath> DowngradePaths => mDowngradePaths;

        public virtual TestSpecification ActiveTestSpecification => TestSpecifications.FirstOrDefault(t => t.Active);

        public virtual bool AllowBackdating { get; set; }
        public virtual int Level { get; set; }

        public virtual int TestSessionBookingAvailabilityWeeks { get; set; }
        public virtual int TestSessionBookingClosedWeeks { get; set; }
        public virtual int TestSessionBookingRejectHours { get; set; }
        public virtual bool AllowAvailabilityNotice { get; set; }

        public virtual string DisplayName
        {
            get { return InternalName; }
            set { InternalName = value; }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
