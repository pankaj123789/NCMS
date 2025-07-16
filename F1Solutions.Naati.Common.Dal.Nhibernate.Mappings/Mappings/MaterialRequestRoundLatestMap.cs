using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class MaterialRequestRoundLatestMap : IAutoMappingOverride<MaterialRequestRoundLatest>
    {
        public void Override(AutoMapping<MaterialRequestRoundLatest> mapping)
        {
            mapping.Table("vwTestMaterialRequestRoundLatest");
            mapping.ReadOnly();
            mapping.Id(x => x.Id).Column("MaterialRequestId");
            mapping.References(x => x.MaterialRequestRound).Column("LatestMaterialRoundId");
        }
    }
}