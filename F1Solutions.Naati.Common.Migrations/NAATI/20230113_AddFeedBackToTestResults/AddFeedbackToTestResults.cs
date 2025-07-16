

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230113_AddFeedBackToTestResults
{
    [NaatiMigration(202301131126)]
    public class AddFeedbackToTestResults : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("ALTER TABLE dbo.tblTestResult ADD Feedback varchar(2000) NULL");
        }
    }
}
