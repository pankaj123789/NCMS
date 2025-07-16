using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestStatusMap : IAutoMappingOverride<TestStatus>
    {
        public void Override(AutoMapping<TestStatus> mapping)
        {
            mapping.Table("vwTestStatus");
            mapping.ReadOnly();
            mapping.Id(x => x.Id).Column("TestSittingId");
            mapping.References(x => x.TestStatusType).Column("TestStatusTypeId");           
            mapping.References(x => x.TestSitting).Column("TestSittingId");           
        }
    }
}
