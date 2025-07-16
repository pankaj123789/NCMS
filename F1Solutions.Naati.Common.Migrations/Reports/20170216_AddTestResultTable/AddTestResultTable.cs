using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170216_AddTestResultTable
{
    [NaatiMigration(201702161811)]
    public class AddTestResultTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("TestResultHistory")
                .WithColumn("TestResultId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("PersonId").AsInt32().Nullable()
                .WithColumn("TestId").AsInt32().Nullable()
                .WithColumn("ResultDueDate").AsDateTime().Nullable()
                .WithColumn("LanguageName").AsString(50).Nullable()
                .WithColumn("AccreditationLevel").AsString(100).Nullable()
                .WithColumn("AccreditationCategoryDescription").AsString(100).Nullable()
                .WithColumn("Direction").AsString(20).Nullable()
                .WithColumn("CandidateName").AsString(252).Nullable()
                .WithColumn("NAATINumber").AsInt32().Nullable()
                .WithColumn("NAATINumberDisplay").AsString(50).Nullable()
                .WithColumn("PaidReview").AsBoolean().Nullable()
                .WithColumn("TotalMarks").AsInt32().Nullable()
                .WithColumn("PassMark").AsFloat().Nullable()
                .WithColumn("TotalCost").AsCurrency().Nullable()
                .WithColumn("GeneralComments").AsString(500).Nullable()
                .WithColumn("OverallResult").AsString(50).Nullable()
                .WithColumn("ResultDate").AsDateTime().Nullable();

            this.CreateObsoletedDateIndex("TestResult");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
