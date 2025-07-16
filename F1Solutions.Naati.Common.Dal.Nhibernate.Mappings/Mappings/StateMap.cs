using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class StateMap : IAutoMappingOverride<State>
    {
        public void Override(AutoMapping<State> mapping)
        {
            mapping.Map(x => x.Abbreviation).Column("State");
        }
    }
}
