using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20171003_CleanReportingDatabase
{
    [NaatiMigration(201710031700)]
    public class CleanReportingDatabase : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                truncate table ApplicationHistory
                truncate table CredentialRequestsHistory
                truncate table CredentialsHistory
                truncate table MarkHistory
                truncate table PanelHistory
                truncate table PanelMembersHistory
                truncate table PersonHistory
                truncate table TestHistory
                truncate table TestResultHistory");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
