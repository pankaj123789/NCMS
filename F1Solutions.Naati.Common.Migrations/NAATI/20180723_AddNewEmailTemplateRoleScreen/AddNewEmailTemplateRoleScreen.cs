
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180723_AddNewEmailTemplateRoleScreen
{
    [NaatiMigration(201807231700)]
    public class AddNewEmailTemplateRoleScreen : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(
                @"Declare @RoleScreenId int = 0
                  EXEC [dbo].[GetSingleKey] @TableName = N'RoleScreen', @NextKey = @RoleScreenId OUTPUT    
	              INSERT INTO [dbo].[tblRoleScreen] (RoleScreenId, RoleId, ScreenId, PermissionId) VALUES (@RoleScreenId,12,135,2)");
        }
    }
}
