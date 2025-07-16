using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestSittingMap : IAutoMappingOverride<TestSitting>
    {
        public void Override(AutoMapping<TestSitting> mapping)
        {

            mapping.HasMany(x => x.TestResults)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(m => m.TestSittingDocuments)
             .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
             .Cascade.AllDeleteOrphan()
             .Inverse();

            mapping.HasMany(m => m.TestSittingNotes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(m => m.TestSittingTestMaterials)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.References(prop => prop.TestStatus).Column("TestSittingId").ReadOnly().NotFound.Ignore();
        }
    }
}
