using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Institution : EntityBase
    {
        public Institution()
        {
            mInstitutionNames = new List<InstitutionName>();
            mContactPersons = new List<ContactPerson>();
            mLatestInstitutionName = new LatestInstitutionName();
        }

        private IList<InstitutionName> mInstitutionNames;
        private LatestInstitutionName mLatestInstitutionName;
        private IList<ContactPerson> mContactPersons;

        public virtual IEnumerable<InstitutionContactPerson> InstitutionContactPersons { get; set; }
        public virtual LatestInstitutionName LatestInstitutionName => mLatestInstitutionName;
        public virtual IEnumerable<ContactPerson> ContactPersons => mContactPersons;

        public virtual IEnumerable<InstitutionName> InstitutionNames
        {
            get
            {
                return mInstitutionNames;
            }
        }

        public virtual void AddName(InstitutionName name)
        {
            name.Institution = this;
            mInstitutionNames.Add(name);
        }

        public virtual NaatiEntity Entity { get; set; }

        // Formula fields
        public virtual string InstitutionAbberviation { get; set; }
        public virtual string InstitutionName { get; set; }
        public virtual string TradingName { get; set; }
        public virtual bool TrustedPayer { get; set; }

        /// <summary>
        /// Returns the current name (most recent effective date) or null if there are no names.
        /// </summary>
        public virtual InstitutionName CurrentName
        {
            get
            {
                return ((from n in InstitutionNames select n).OrderByDescending(n => n.EffectiveDate)).FirstOrDefault();
            }
        }

        public override IAuditObject RootAuditObject
        {
            get { return this.Entity.RootAuditObject; }
        }


        public virtual bool IsManageCoursesAndQualification { get; set; }
        public virtual bool IsGoThroughApprovalProcess { get; set; }
        public virtual bool IsUniversity { get; set; }
        public virtual bool IsVetRto { get; set; }
        public virtual string RtoNumber { get; set; }
        public virtual bool IsOfferCourseToStudentVisa { get; set; }
        public virtual string CricosProviderCode { get; set; }

        public override string ToString()
        {
            return CurrentName.Name;
        }

    }
}
