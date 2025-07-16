using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class InvoiceMap : IAutoMappingOverride<Invoice>
    {
        public void Override(AutoMapping<Invoice> mapping)
        {
            mapping.HasMany(x => x.InvoiceLines)
            .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
            .Cascade.AllDeleteOrphan()
            .Inverse();

            mapping.References(x => x.Transaction)
                .Cascade.Delete();
        }
    }
}

