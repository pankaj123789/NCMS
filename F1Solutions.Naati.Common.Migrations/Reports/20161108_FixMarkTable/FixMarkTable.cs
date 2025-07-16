using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161108_FixMarkTable
{
    [NaatiMigration(201611081558)]
    public class FixMarkTable : NaatiMigration
    {
        public override void Up()
        {            
            // Fix the Mark and TotalMarks columns to use the correct datatypes

            this.ExecuteSql(@"
DROP INDEX [IX_Mark_ObsoletedDate] ON [MarkHistory]

ALTER TABLE [MarkHistory]
	ALTER COLUMN Mark float null

ALTER TABLE [MarkHistory]
	ALTER COLUMN TotalMarks float null

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
