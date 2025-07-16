using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentCategory : EntityBase
    {
        private IList<ProfessionalDevelopmentCategoryRequirement> mCategoryRequirements = new List<ProfessionalDevelopmentCategoryRequirement>();
        private IList<ProfessionalDevelopmentSectionCategory> mProfessionalDevelopmentSectionCategories = new List<ProfessionalDevelopmentSectionCategory>();

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual ProfessionalDevelopmentCategoryGroup ProfessionalDevelopmentCategoryGroup { get; set; }

        public virtual IEnumerable<ProfessionalDevelopmentSectionCategory> ProfessionalDevelopmentSectionCategories =>
            mProfessionalDevelopmentSectionCategories;

        public virtual IEnumerable<ProfessionalDevelopmentCategoryRequirement> CategoryRequirements =>
            mCategoryRequirements;




    }
}
