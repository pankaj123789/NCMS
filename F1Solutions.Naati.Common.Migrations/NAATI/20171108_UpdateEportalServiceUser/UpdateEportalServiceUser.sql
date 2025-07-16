DECLARE @UserId     INT, 
        @RoleId     INT, 
        @UserRoleId INT 

SELECT @UserId = UserId 
FROM   tblUser 
WHERE  UserName = 'NAATI\EPORTALAPPSERVICE' 
        OR UserName = 'NAATI\UATEPORTALWEBSERVICE' 
        OR UserName = 'F1TEST\EPORTALAPPLICATIONAP' 

IF @UserId IS NOT NULL 
  BEGIN 
      SELECT @RoleId = RoleId 
      FROM   tluRole 
      WHERE  RoleName = 'Update User' 

      IF NOT EXISTS(SELECT * 
                    FROM   tblUserRole 
                    WHERE  UserId = @UserId 
                           AND RoleId = @RoleId) 
        BEGIN 
            EXEC Getsinglekey
              @TableName = N'UserRole', 
              @NextKey = @UserRoleId output 

            INSERT INTO tblUserRole
                        (UserRoleId, UserId, RoleId) 
            VALUES      (@UserRoleId, @UserId, @RoleId) 
        END 
 
      UPDATE tblUser 
      SET    FullName = 'My NAATI', OfficeId = 316 
      WHERE  UserId = @UserId 
  END 
  