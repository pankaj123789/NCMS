using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class TestEventMap : IAutoMappingOverride<TestEvent>
    {
        public void Override(AutoMapping<TestEvent> mapping)
        {
            mapping.HasMany(m => m.TestAttendances)
                .Inverse();
        }        
    }
}
