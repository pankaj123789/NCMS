using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class InvoiceBatchMap : IAutoMappingOverride<InvoiceBatch>
    {
        public void Override(AutoMapping<InvoiceBatch> mapping)
        {
            mapping.HasMany(x => x.InvoiceBatchInvoices)
             .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
             .Cascade.SaveUpdate()
             .Inverse();
        }
    }
}
