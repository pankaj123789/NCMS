using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class BoardDecisionMap : IAutoMappingOverride<BoardDecision>
    {
        public void Override(AutoMapping<BoardDecision> mapping)
        {
           mapping.Table("tluBoardDecision");
        }
    }
}
