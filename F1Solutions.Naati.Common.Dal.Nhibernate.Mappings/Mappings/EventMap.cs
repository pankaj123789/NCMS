using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class EventMap : IAutoMappingOverride<Event>
    {
        public void Override(AutoMapping<Event> mapping)
        {
            mapping.HasMany(x => x.EventDates)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.EventFacilitators)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();          
        }
    }
}
