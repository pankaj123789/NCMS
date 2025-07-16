using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20220818_AddAutoCreatedToApplicationHistoryCredentialRequestHistory
{
    [NaatiMigration(202208181011)]
    class ReportingDb_AddAutoCreatedFieldToTblApplicationHistoryAndTblCredentialRequestHistory : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddAutoCreatedToApplicationHistoryCredentialRequestHistory);
        }
    }
}
