
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180406_AddUserToTestProgramRole
{
    [NaatiMigration(201804061700)]
    public class AddUserToTestProgramRole : NaatiMigration
    {
        public override void Up()
        {   

            Execute.Sql(@"   
         
                        declare @UserRoleId int
                        declare @UserId int 
                        declare @RoleId int = 322

                        DECLARE MY_CURSOR CURSOR 
                          LOCAL STATIC READ_ONLY FORWARD_ONLY
                        FOR                         
                        SELECT DISTINCT UserId FROM tblUser where Email in (
                        'Kevin.Bleasdale@NAATI.com.au', 'Natalie.Richardson@NAATI.com.au', 
                        'renuka.ganesan@naati.com.au', 'Jacqui.Seidel@NAATI.com.au', 'Nora.Sautter@NAATI.com.au',
                        'maho.fukuno@NAATI.com.au', 'Lee.Yacoumis@NAATI.com.au', 'Sarah.Lattimore@NAATI.com.au')
                         and UserId not in (select UserId from tblUserRole where RoleId = @RoleId)

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
