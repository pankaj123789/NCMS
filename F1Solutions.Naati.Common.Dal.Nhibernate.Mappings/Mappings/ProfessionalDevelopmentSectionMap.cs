using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class ProfessionalDevelopmentSectionMap : IAutoMappingOverride<ProfessionalDevelopmentSection>
    {
        public void Override(AutoMapping<ProfessionalDevelopmentSection> mapping)
        {
            mapping.HasMany(x => x.SectionCategories)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
