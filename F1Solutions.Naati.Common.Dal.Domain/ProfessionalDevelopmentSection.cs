using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentSection : EntityBase
    {
        private IList<ProfessionalDevelopmentSectionCategory> mSectionCategories = new List<ProfessionalDevelopmentSectionCategory>();

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual double RequiredPointsPerYear { get; set; }

        public virtual IEnumerable<ProfessionalDevelopmentSectionCategory> SectionCategories => mSectionCategories;

    }
}
