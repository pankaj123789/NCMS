using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180613_RubricHistory
{
    [NaatiMigration(201806131355)]
    public class RubricHistory : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("TestResultRubricHistory")
                .WithColumn("TestResultId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("TestSittingId").AsInt32().Nullable()
                .WithColumn("TestSessionId").AsInt32().Nullable()
                .WithColumn("PersonId").AsInt32().Nullable()
                .WithColumn("CustomerNo").AsInt32().Nullable()
                .WithColumn("CandidateName").AsString(252).Nullable()
                .WithColumn("ResultDueDate").AsDateTime().Nullable()
                .WithColumn("Language1").AsString(50).Nullable()
                .WithColumn("Language1Code").AsString(10).Nullable()
                .WithColumn("Language1Group").AsString(100).Nullable()
                .WithColumn("Language2").AsString(50).Nullable()
                .WithColumn("Language2Code").AsString(10).Nullable()
                .WithColumn("Language2Group").AsString(100).Nullable()
                .WithColumn("Skill").AsString(100).Nullable()
                .WithColumn("PaidReview").AsBoolean().Nullable()
                .WithColumn("Supplementary").AsBoolean().Nullable()
                .WithColumn("OverallResult").AsString(50).Nullable()
                .WithColumn("ResultDate").AsDateTime().Nullable();

            Create.Table("MarkRubricHistory")
                .WithColumn("TestResultId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("TestComponentId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("JobExaminerId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("RubricAssessementCriterionResultId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("TestSittingId").AsInt32().Nullable()
                .WithColumn("TestSessionId").AsInt32().Nullable()
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("CustomerNo").AsInt32().Nullable()
                .WithColumn("CandidateName").AsString(252).Nullable()
                .WithColumn("PaidReview").AsBoolean().Nullable()
                .WithColumn("Supplementary").AsBoolean().Nullable()
                .WithColumn("TestTaskTypeLabel").AsString(100).Nullable()
                .WithColumn("TestTaskTypeName").AsString(50).Nullable()
                .WithColumn("TestTaskLabel").AsString(100).Nullable()
                .WithColumn("TestTaskName").AsString(100).Nullable()
                .WithColumn("TestTaskNumber").AsInt32().Nullable()
                .WithColumn("MarkType").AsString(100).Nullable()
                .WithColumn("ResultType").AsString(100).Nullable()
                .WithColumn("ExaminerCustomerNo").AsInt32().Nullable()
                .WithColumn("ExaminerName").AsString(252).Nullable()
                .WithColumn("ExaminerType").AsString(100).Nullable()
                .WithColumn("Cost").AsCurrency().Nullable()
                .WithColumn("ExaminerSubmittedDate").AsDateTime().Nullable()
                .WithColumn("IncludeMarks").AsBoolean().Nullable()
                .WithColumn("WasAttempted").AsBoolean().Nullable()
                .WithColumn("Successful").AsBoolean().Nullable()
                .WithColumn("RubricCompetencyLabel").AsString(100).Nullable()
                .WithColumn("RubricCompetencyName").AsString(50).Nullable()
                .WithColumn("RubricCriterionName").AsString(100).Nullable()
                .WithColumn("RubricCriterionLabel").AsString(100).Nullable()
                .WithColumn("RubricSelectedBandLabel").AsString(100).Nullable()
                .WithColumn("RubricSelectedBandLevel").AsInt32().Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
