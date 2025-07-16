using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class DashboardReportMap : IAutoMappingOverride<DashboardReport>
    {
        public void Override(AutoMapping<DashboardReport> mapping)
        {
            mapping.Map(x => x.DashboardXml).CustomType("StringClob").CustomSqlType("nvarchar(max)");
        }
    }
}