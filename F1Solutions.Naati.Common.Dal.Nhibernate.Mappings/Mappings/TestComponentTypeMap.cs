using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace TestSpecImporter.DataAccessLayer.Mappings
{
    public class TestComponentTypeMap : IAutoMappingOverride<TestComponentType>
    {
        public void Override(AutoMapping<TestComponentType> mapping)
        {
            mapping.HasMany(m => m.TestComponentTypeStandardMarkingSchemes)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.AllDeleteOrphan()
              .Inverse();

            mapping.HasMany(m => m.TestMaterials)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.AllDeleteOrphan()
              .Inverse();

            mapping.HasMany(m => m.RubricMarkingCompetencies)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}