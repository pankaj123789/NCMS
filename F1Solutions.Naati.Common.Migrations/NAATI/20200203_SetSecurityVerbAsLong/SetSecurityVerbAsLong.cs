namespace F1Solutions.Naati.Common.Migrations.NAATI._20200203_SetSecurityVerbAsLong
{
    [NaatiMigration(202002031403)]
    public class SetSecurityVerbAsLong :NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("SecurityVerbMask").OnTable("tblSecurityRule").AsInt64().NotNullable();
        }
    }
}
