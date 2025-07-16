using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161021_AddMarkTable
{
    [NaatiMigration(201610211431)]
    public class AddMarkTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("MarkHistory")
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("TestId").AsInt32().NotNullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("ExaminerNAATINumber").AsInt32().Nullable()
                .WithColumn("ExaminerName").AsString(252).Nullable()
                .WithColumn("CountMarks").AsBoolean().Nullable()
                .WithColumn("Identifier").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("Mark").AsFloat().Nullable()
                .WithColumn("Total").AsInt32().Nullable()
                .WithColumn("TotalMarks").AsInt32().Nullable()
                .WithColumn("CodesGroup1").AsString(500).Nullable()
                .WithColumn("CodesGroup2").AsString(500).Nullable()
                .WithColumn("EthicsSocialCulturalComments").AsString(500).Nullable()
                .WithColumn("GeneralComments").AsString(500).Nullable()
                .WithColumn("OverallResult").AsString(50).Nullable()
                .WithColumn("ResultDate").AsDateTime().Nullable();

            this.ExecuteSql(@"
CREATE NONCLUSTERED INDEX [IX_Mark_ObsoletedDate] ON [dbo].[MarkHistory]
(
    [ObsoletedDate] ASC
)
INCLUDE ( 	[PersonId],
    [TestId],
    [ExaminerNAATINumber],
    [ExaminerName],
    [CountMarks],
    [Identifier],
    [Mark],
    [Total],
    [TotalMarks],
    [CodesGroup1],
    [CodesGroup2],
    [EthicsSocialCulturalComments],
    [GeneralComments],
    [OverallResult],
    [ResultDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
