
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190109_AddAllowMultipleColumn
{
    [NaatiMigration(201901091435)]
    public class AddAllowMultipleColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("AllowMultiple").OnTable("tblCredentialApplicationType").AsBoolean().Nullable();
            Update.Table("tblCredentialApplicationType").Set(new { AllowMultiple = false}).AllRows();
            Alter.Column("AllowMultiple").OnTable("tblCredentialApplicationType").AsBoolean().NotNullable();
        }
    }
}
