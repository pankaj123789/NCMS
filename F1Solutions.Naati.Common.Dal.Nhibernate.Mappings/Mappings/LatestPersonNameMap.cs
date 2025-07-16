using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class LatestPersonNameNameMap : IAutoMappingOverride<LatestPersonName>
    {
        public void Override(AutoMapping<LatestPersonName> mapping)
        {
            mapping.Table("vwDistinctPersonName");
            mapping.ReadOnly();
            mapping.Id(x => x.Id).Column("PersonId");
        }
    }
}
