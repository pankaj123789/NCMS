using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170217_ModifyMarkTable
{
    [NaatiMigration(201702170959)]
    public class ModifyMarkTable : NaatiMigration
    {
        public override void Up()
        {
            using (new ReportMigrationContainer(this, "Mark"))
            {
                //Create.Column("MarkId").OnTable("MarkHistory").AsInt32().NotNullable();
                Create.Column("TestResultId").OnTable("MarkHistory").AsInt32().Nullable();
                Create.Column("ComponentName").OnTable("MarkHistory").AsString(50).Nullable();
                Create.Column("ExaminerType").OnTable("MarkHistory").AsString(50).Nullable();
                Create.Column("Cost").OnTable("MarkHistory").AsCurrency().Nullable();
                Create.Column("PaperLost").OnTable("MarkHistory").AsBoolean().Nullable();
                Create.Column("PaperReceived").OnTable("MarkHistory").AsDateTime().Nullable();
                Create.Column("SentToPayroll").OnTable("MarkHistory").AsDateTime().Nullable();
                //Create.Column("IncludeMarks").OnTable("MarkHistory").AsBoolean().Nullable();
                Create.Column("PassMark").OnTable("MarkHistory").AsFloat().Nullable();
                //Create.Column("TotalMark").OnTable("MarkHistory").AsInt32().Nullable();
                //Create.Column("OverallMark").OnTable("MarkHistory").AsFloat().Nullable();
                Create.Column("OverallPassMark").OnTable("MarkHistory").AsInt32().Nullable();
                Create.Column("OverallTotalMark").OnTable("MarkHistory").AsInt32().Nullable();
                Create.Column("PrimaryFailureReason").OnTable("MarkHistory").AsString(50).Nullable();
                Create.Column("PoorPerformanceReasons").OnTable("MarkHistory").AsString(100).Nullable();
                Create.Column("MarkerComments").OnTable("MarkHistory").AsString(4000).Nullable();
                Create.Column("SubmittedDate").OnTable("MarkHistory").AsDateTime().Nullable();

                Rename.Column("Identifier").OnTable("MarkHistory").To("MarkId");
                Rename.Column("CountMarks").OnTable("MarkHistory").To("IncludeMarks");
                Rename.Column("Total").OnTable("MarkHistory").To("TotalMark");
                Rename.Column("TotalMarks").OnTable("MarkHistory").To("OverallMark");

                // Renaming the primary key column [Identifier] to [MarkId]
                // Copy data from [Identifier] to [MarkId] and update primary key constraint
                //            this.ExecuteSql(@"
                //UPDATE
                //    [MarkHistory_A]
                //SET
                //    [MarkHistory_A].[MarkId] = [MarkHistory_B].[Identifier]
                //FROM
                //    [MarkHistory] AS [MarkHistory_A]
                //    INNER JOIN [MarkHistory] AS [MarkHistory_B]
                //        ON [MarkHistory_A].[Identifier] = [MarkHistory_B].[Identifier]
                //        AND [MarkHistory_A].[ModifiedDate] = [MarkHistory_B].[ModifiedDate]
                //
                //ALTER TABLE [MarkHistory]
                //DROP CONSTRAINT [ModifiedDate_Identifier_PK]
                //
                //ALTER TABLE [MarkHistory]
                //ADD CONSTRAINT [MarkId_ModifiedDate_PK] PRIMARY KEY ([MarkId], [ModifiedDate])");

                //Delete.Column("Identifier").FromTable("MarkHistory");
                //Delete.Column("CountMarks").FromTable("MarkHistory");
                //Delete.Column("Total").FromTable("MarkHistory");
                //Delete.Column("TotalMarks").FromTable("MarkHistory");
                Delete.Column("CodesGroup1").FromTable("MarkHistory");
                Delete.Column("CodesGroup2").FromTable("MarkHistory");
                Delete.Column("EthicsSocialCulturalComments").FromTable("MarkHistory");
                Delete.Column("GeneralComments").FromTable("MarkHistory");
                Delete.Column("OverallResult").FromTable("MarkHistory");
                Delete.Column("ResultDate").FromTable("MarkHistory");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
