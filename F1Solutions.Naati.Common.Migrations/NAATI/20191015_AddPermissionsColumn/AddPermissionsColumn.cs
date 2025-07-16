
namespace F1Solutions.Naati.Common.Migrations.NAATI._20191015_AddPermissionsColumn
{
    [NaatiMigration(201910151200)]
    public class AddPermissionsColumn : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblApiAccess").AddColumn("Permissions").AsInt32().Nullable();

            Execute.Sql("Update tblApiAccess Set Permissions = 1");

            Alter.Table("tblApiAccess").AlterColumn("Permissions").AsInt32().NotNullable();
        }
    }
}
