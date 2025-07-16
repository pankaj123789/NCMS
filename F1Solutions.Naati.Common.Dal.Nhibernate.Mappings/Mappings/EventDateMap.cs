using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class EventDateMap : IAutoMappingOverride<EventDate>
    {
        public void Override(AutoMapping<EventDate> mapping)
        {
            mapping.Map(x => x.Date).Column("EventDate");
        }
    }
}
