using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class JobMap : IAutoMappingOverride<Job>
    {
        public void Override(AutoMapping<Job> mapping)
        {
            mapping.References(x => x.SentUser).Column("SentUserID");

            mapping.HasMany(x => x.JobExaminers)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.TestResults)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m).KeyColumn("CurrentJobId")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
