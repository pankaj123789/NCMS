namespace F1Solutions.Naati.Common.Migrations.NAATI._20200519_AlterGlCode
{
    [NaatiMigration(202005191030)]
    public class AlterGlCode : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblGlCode").AlterColumn("Code").AsString(12).NotNullable();
            Execute.Sql("update tblGlCode set code = trim(code)");
        }
    }
}
