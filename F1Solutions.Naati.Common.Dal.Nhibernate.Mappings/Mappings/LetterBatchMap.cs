using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class LetterBatchMap : IAutoMappingOverride<LetterBatch>
    {
        public void Override(AutoMapping<LetterBatch> mapping)
        {
            mapping.Id(x => x.Id).Column("LetterBatchId");

            mapping.HasMany(x => x.MergeData)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
