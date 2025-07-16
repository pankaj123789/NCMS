using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class ExaminerMarkingMap : IAutoMappingOverride<ExaminerMarking>

    {
        public void Override(AutoMapping<ExaminerMarking> mapping)
        {
            mapping.Id(x => x.Id).Column("ExaminerMarkingID");

            mapping.References(x => x.TestResult).Column("TestResultID");
            mapping.References(x => x.JobExaminer).Column("JobExaminerID");

            mapping.HasMany(x => x.ExaminerTestComponentResults).KeyColumn("ExaminerMarkingID")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
