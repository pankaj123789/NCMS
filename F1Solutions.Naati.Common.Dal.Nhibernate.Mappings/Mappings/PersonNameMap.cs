using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class PersonNameMap : IAutoMappingOverride<PersonName>
    {
        public void Override(AutoMapping<PersonName> mapping)
        {
            mapping.References(x => x.Title).Fetch.Join();
        }
    }
}
