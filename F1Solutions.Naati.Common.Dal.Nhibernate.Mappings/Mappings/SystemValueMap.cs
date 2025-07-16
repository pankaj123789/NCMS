using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class SystemValueMap : IAutoMappingOverride<SystemValue>
    {
        public void Override(AutoMapping<SystemValue> mapping)
        {
            mapping.Map(m => m.Value).CustomType("StringClob").CustomSqlType("nvarchar(max)");
        }
    }
}