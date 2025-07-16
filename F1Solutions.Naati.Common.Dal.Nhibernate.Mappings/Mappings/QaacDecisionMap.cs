using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class QaacDecisionMap : IAutoMappingOverride<QaacDecision>
    {
        public void Override(AutoMapping<QaacDecision> mapping)
        {
            mapping.Table("tluQaacDecision");
        }
    }
}
