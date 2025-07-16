using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestMaterialLastUsedMap : IAutoMappingOverride<TestMaterialLastUsed>
    {
        public void Override(AutoMapping<TestMaterialLastUsed> mapping)
        {
            mapping.Table("vwTestMaterialLastUsed");
            mapping.ReadOnly();
            mapping.Id(x => x.Id).Column("TestMaterialId");
            mapping.References(x => x.TestMaterial).Column("TestMaterialId");
        }
    }
}
