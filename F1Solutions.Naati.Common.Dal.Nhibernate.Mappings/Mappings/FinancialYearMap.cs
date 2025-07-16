using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class FinancialYearMap : IAutoMappingOverride<FinancialYear>
    {
        public void Override(AutoMapping<FinancialYear> mapping)
        {
            mapping.Map(x => x.Year).Column("FinancialYear");
        }
    }
}
