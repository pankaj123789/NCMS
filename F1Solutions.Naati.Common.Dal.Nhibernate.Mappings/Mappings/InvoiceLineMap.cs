using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class InvoiceLineMap : IAutoMappingOverride<InvoiceLine>
    {
        public void Override(AutoMapping<InvoiceLine> mapping)
        {
            mapping.HasMany(p => p.PaymentAllocations)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Cascade.SaveUpdate()
               .Inverse();
        }
    }
}
