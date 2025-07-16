using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace TestSpecImporter.DataAccessLayer.Mappings
{
    public class TestComponentMap : IAutoMappingOverride<TestComponent>
    {
        public void Override(AutoMapping<TestComponent> mapping)
        {
            mapping.HasMany(x => x.TestSittingTestMaterials)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}
