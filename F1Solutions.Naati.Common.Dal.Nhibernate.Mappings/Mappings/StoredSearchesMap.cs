using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class StoredSearchesMap : IAutoMappingOverride<StoredSearches>
	{
        public void Override(AutoMapping<StoredSearches> mapping)
        {
            mapping.Id(x => x.Id).Column("SearchPrimaryId");
            mapping.Map(x => x.CriteriaXml).CustomType("StringClob").CustomSqlType("varchar(max)");
        }
	}
}