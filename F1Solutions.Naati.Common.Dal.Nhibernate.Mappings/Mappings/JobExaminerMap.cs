using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class JobExaminerMap : IAutoMappingOverride<JobExaminer>
    {
        public void Override(AutoMapping<JobExaminer> mapping)
        {
            mapping.Id(x => x.Id).Column("JobExaminerID");

            mapping.References(x => x.ExaminerSentUser).Column("ExaminerSentUserID").NotFound.Ignore();
            mapping.References(x => x.ExaminerReceivedUser).Column("ExaminerReceivedUserID").NotFound.Ignore();
            mapping.References(x => x.ExaminerToPayrollUser).Column("ExaminerToPayrollUserID").NotFound.Ignore();
            mapping.References(x => x.ProductSpecification).Column("ProductSpecificationId").NotFound.Ignore();
            mapping.References(x => x.ProductSpecificationChangedUser).Column("ProductSpecificationChangedUserId").NotFound.Ignore();
            mapping.References(x => x.ValidatedUser).Column("ValidatedUserId").NotFound.Ignore();

            mapping.Map(x => x.PayrollStatusName).Formula("(select top 1 vwJobExaminerPayrollStatus.DisplayName from vwJobExaminerPayrollStatus where vwJobExaminerPayrollStatus.JobExaminerId = JobExaminerID)");

            mapping.HasMany(x => x.PayRolls)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.Markings)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.JobExaminerRubricTestComponentResults)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}