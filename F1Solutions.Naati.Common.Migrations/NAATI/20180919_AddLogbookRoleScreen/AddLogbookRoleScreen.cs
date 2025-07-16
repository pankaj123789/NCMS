
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180919_AddLogbookRoleScreen
{
    [NaatiMigration(201809191200)]
    public class AddLogbookRoleScreen : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"INSERT INTO [dbo].[tblRoleScreen] (RoleId,ScreenId,PermissionId) VALUES ( 7, 136, 2)");
        }
    }
}
