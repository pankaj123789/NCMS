using F1Solutions.Naati.Common.Migrations.NAATI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.Reports._20221117_AddTestResultExaminerMarkingTables
{
    [NaatiMigration(202211170930)]
    public class AddTestResultExaminerMarkingTables : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("TestResultExaminerRubricMarkingHistory")
                .WithColumn("TestResultId").AsInt32().NotNullable()
                .WithColumn("TestSittingId").AsInt32().Nullable()
                .WithColumn("NaatiNumber").AsInt32().Nullable()
                .WithColumn("ExaminerName").AsString(350).Nullable()
                .WithColumn("TestSatDate").AsDateTime().Nullable()
                .WithColumn("CredentialType").AsString(50).Nullable()
                .WithColumn("Skill").AsString(256).Nullable()
                .WithColumn("MarksReceivedDate").AsDateTime().Nullable()
                .WithColumn("TestComponentType").AsString(600).Nullable()
                .WithColumn("Competency").AsString(50).Nullable()
                .WithColumn("Criteria").AsString(100).Nullable()
                .WithColumn("Band").AsString(50).Nullable()
                .WithColumn("ExaminerComments").AsString(4000).Nullable();

            Create.Table("TestResultExaminerStandardMarkingHistory")
                .WithColumn("TestResultId").AsInt32().NotNullable()
                .WithColumn("TestSittingId").AsInt32().Nullable()
                .WithColumn("NaatiNumber").AsInt32().Nullable()
                .WithColumn("ExaminerName").AsString(350).Nullable()
                .WithColumn("TestSatDate").AsDateTime().Nullable()
                .WithColumn("CredentialType").AsString(50).Nullable()
                .WithColumn("Skill").AsString(256).Nullable()
                .WithColumn("MarksSubmittedDate").AsDateTime().Nullable()
                .WithColumn("ExaminerComments").AsString(4000).Nullable();
        }
    }
}
