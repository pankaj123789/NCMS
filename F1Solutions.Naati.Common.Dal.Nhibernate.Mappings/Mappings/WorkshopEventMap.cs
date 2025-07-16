using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class WorkshopEventMap : IAutoMappingOverride<WorkshopEvent>
    {
        public void Override(AutoMapping<WorkshopEvent> mapping)
        {
            mapping.HasMany(x => x.WorkshopAttendances)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.WorkshopMaterials)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

        }
    }
}
