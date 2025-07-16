
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180328_InsertNewRole
{
    [NaatiMigration(201803021203)]
    public class InsertNewRole : NaatiMigration
    {
        public override void Up()
        {
            Insert.IntoTable("tluRole")
                .Row(
                    new
                    {
                        RoleName = "TestProgram",
                        Description = "Can create or edit test materials",
                        SystemRole = false,
                        RoleId = 322
                    });

            Execute.Sql(@"   
                        update tblRoleScreen set PermissionId = 1 where RoleId = 7 and ScreenId = 128
                     
                        declare @RoleScreenId int                        

                        EXEC [dbo].[GetSingleKey] @TableName = N'RoleScreen', @NextKey = @RoleScreenId OUTPUT						
                        INSERT INTO [dbo].[tblRoleScreen] (RoleScreenId,RoleId,ScreenId,PermissionId) VALUES (@RoleScreenId, 322, 128, 1)

                        EXEC [dbo].[GetSingleKey] @TableName = N'RoleScreen', @NextKey = @RoleScreenId OUTPUT    
                        INSERT INTO [dbo].[tblRoleScreen] (RoleScreenId,RoleId,ScreenId,PermissionId) VALUES (@RoleScreenId, 322, 128, 2)

                        declare @UserRoleId int
                        declare @UserId int 
                        declare @RoleId int = 322

                        DECLARE MY_CURSOR CURSOR 
                          LOCAL STATIC READ_ONLY FORWARD_ONLY
                        FOR 
                        SELECT DISTINCT userId 
                        FROM tblUserRole
                        where roleId = 12

                        OPEN MY_CURSOR
                        FETCH NEXT FROM MY_CURSOR INTO @UserId
                        WHILE @@FETCH_STATUS = 0
                        BEGIN 
	                        EXEC [dbo].[GetSingleKey] @TableName = N'UserRole', @NextKey = @UserRoleId OUTPUT    
	                        INSERT INTO [dbo].[tblUserRole] (UserRoleId, UserId, RoleId) VALUES (@UserRoleId, @UserId, @RoleId)
                            FETCH NEXT FROM MY_CURSOR INTO @UserId
                        END
                        CLOSE MY_CURSOR
                        DEALLOCATE MY_CURSOR");
        }
    }
}
