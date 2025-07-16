using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestLocationMap : IAutoMappingOverride<TestLocation>
    {
        public void Override(AutoMapping<TestLocation> mapping)
        {
            mapping.HasMany(x => x.Venues)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.RolePlayerTestLocations)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}