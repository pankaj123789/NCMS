namespace F1Solutions.Naati.Common.Migrations.NAATI._20220729_MfaFields
{
    [NaatiMigration(202207291040)]
    public class MfaFields : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("ALTER TABLE dbo.tblPerson ADD MfaCode nvarchar(50) NULL, MfaExpireStartDate datetime NULL");
        }
    }
}
