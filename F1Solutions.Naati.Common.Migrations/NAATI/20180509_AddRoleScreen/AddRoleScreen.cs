
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180509_AddRoleScreen
{
    [NaatiMigration(201805091600)]
    public class AddRoleScreen : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@" 
                        DECLARE @RoleScreenId int                        

                        EXEC [dbo].[GetSingleKey] @TableName = N'RoleScreen', @NextKey = @RoleScreenId OUTPUT						
                        INSERT INTO [dbo].[tblRoleScreen] (RoleScreenId,RoleId,ScreenId,PermissionId) VALUES (@RoleScreenId, 322, 134, 2)

                        EXEC [dbo].[GetSingleKey] @TableName = N'RoleScreen', @NextKey = @RoleScreenId OUTPUT						
                        INSERT INTO [dbo].[tblRoleScreen] (RoleScreenId,RoleId,ScreenId,PermissionId) VALUES (@RoleScreenId, 12, 134, 2)

                        EXEC [dbo].[GetSingleKey] @TableName = N'RoleScreen', @NextKey = @RoleScreenId OUTPUT						
                        INSERT INTO [dbo].[tblRoleScreen] (RoleScreenId,RoleId,ScreenId,PermissionId) VALUES (@RoleScreenId, 6, 134, 1)
                        ");
        }
    }
}
