using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class AddressMap : IAutoMappingOverride<Address>
    {
        public void Override(AutoMapping<Address> mapping)
        {
            mapping.IgnoreProperty(x => x.SuburbOrCountry);
        }
    }
}
