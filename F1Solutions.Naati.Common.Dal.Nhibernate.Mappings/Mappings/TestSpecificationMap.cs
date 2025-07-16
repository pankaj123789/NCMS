using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace TestSpecImporter.DataAccessLayer.Mappings
{
    public class TestSpecificationMap : IAutoMappingOverride<TestSpecification>
    {
        public void Override(AutoMapping<TestSpecification> mapping)
        {

            mapping.HasMany(m => m.TestComponents)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Cascade.AllDeleteOrphan()
               .Inverse();

            mapping.HasMany(x => x.TestComponentTypes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.TestSpecificationStandardMarkingSchemes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.RubricQuestionPassRules)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.RubricTestBandRules)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.RubricTestQuestionRules)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}
