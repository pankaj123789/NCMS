using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestResultMap : IAutoMappingOverride<TestResult>
    {
        public void Override(AutoMapping<TestResult> mapping)
        {
            mapping.HasMany(x => x.TestComponentResults)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.ExaminerMarkings)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.TestResultRubricTestComponentResults)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.IgnoreProperty(x => x.CurrentJobId);
        }
    }
}
