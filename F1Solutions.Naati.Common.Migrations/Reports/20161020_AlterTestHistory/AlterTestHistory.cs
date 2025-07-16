using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161020_AlterTestHistory
{
    [NaatiMigration(201610201802)]
    public class AlterTestHistory : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql("DROP INDEX IX_Test_ObsoletedDate ON TestHistory");

            Create.Column("Venue").OnTable("TestHistory").AsString(50).Nullable();
            Create.Column("TestMaterialId").OnTable("TestHistory").AsInt32().Nullable();
            Create.Column("TestMaterialDescription").OnTable("TestHistory").AsString(500).Nullable();

            this.ExecuteSql(@"
CREATE NONCLUSTERED INDEX [IX_Test_ObsoletedDate] ON [dbo].[TestHistory]
(
    [ObsoletedDate] ASC
)
INCLUDE ( 	[TestInvitationId],
    [TestAttendanceId],
    [TestResultId],
    [TestKey],
    [ApplicationId],
    [PersonId],
    [ResponseDate],
    [Accepted],
    [WithdrawnDate],
    [ConfirmedDate],
    [TestEventId],
    [Sat],
    [ResultType],
    [ThirdExaminerRequired],
    [ProcessedDate],
    [SatDate],
    [ResultChecked],
    [Venue],
    [TestMaterialId],
    [TestMaterialDescription]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
