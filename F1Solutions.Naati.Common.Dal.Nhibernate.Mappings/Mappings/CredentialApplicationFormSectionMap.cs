using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class CredentialApplicationFormSectionMap : IAutoMappingOverride<CredentialApplicationFormSection>
    {
        public void Override(AutoMapping<CredentialApplicationFormSection> mapping)
        {
            mapping.HasMany(x => x.Questions)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
