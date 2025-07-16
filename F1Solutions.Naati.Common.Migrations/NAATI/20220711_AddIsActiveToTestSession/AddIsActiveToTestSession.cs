namespace F1Solutions.Naati.Common.Migrations.NAATI._20220711_AddIsActiveToTestSession
{
    [NaatiMigration(202207110853)]
    public class AddIsActiveToTestSession : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql("ALTER TABLE dbo.tblTestSession ADD IsActive bit NOT NULL CONSTRAINT DF_tblTestSession_IsActive DEFAULT 1");
        }
    }
}
