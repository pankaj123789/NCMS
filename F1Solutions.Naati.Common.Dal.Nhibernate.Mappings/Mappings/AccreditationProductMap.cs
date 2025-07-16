using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class AccreditationProductMap : IAutoMappingOverride<AccreditationProduct>
    {
        public void Override(AutoMapping<AccreditationProduct> mapping)
        {
            mapping.References(x => x.InvoiceLine).NotFound.Ignore();
                //.Cascade.Delete();

        }
    }
}

