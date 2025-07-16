using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class ProfessionalDevelopmentCategoryMap: IAutoMappingOverride<ProfessionalDevelopmentCategory>
    {
        public void Override(AutoMapping<ProfessionalDevelopmentCategory> mapping)
        {

            mapping.HasMany(x => x.CategoryRequirements)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.ProfessionalDevelopmentSectionCategories)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
