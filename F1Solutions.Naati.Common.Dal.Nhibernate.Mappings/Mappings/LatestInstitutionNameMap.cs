using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class LatestInstitutionNameMap : IAutoMappingOverride<LatestInstitutionName>
    {
        public void Override(AutoMapping<LatestInstitutionName> mapping)
        {
            mapping.Table("vwDistinctInstitutionName");
            mapping.ReadOnly();
            mapping.Id(x => x.Id).Column("InstitutionId");}
    }
}
