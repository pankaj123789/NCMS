namespace F1Solutions.Naati.Common.Migrations.NAATI._20200324_AddAllowIssue
{
    [NaatiMigration(202003241056)]
    public class AddAllowIssue : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblTestResult")
                .AddColumn("AllowIssue").AsBoolean().NotNullable().WithDefaultValue(true);
        }
    }
}
