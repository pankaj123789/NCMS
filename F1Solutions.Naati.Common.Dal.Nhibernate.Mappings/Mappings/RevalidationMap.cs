using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class RevalidationMap : IAutoMappingOverride<Revalidation>
    {
        public void Override(AutoMapping<Revalidation> mapping)
        {
            mapping.HasMany(m => m.ApplicationRevalidations)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}