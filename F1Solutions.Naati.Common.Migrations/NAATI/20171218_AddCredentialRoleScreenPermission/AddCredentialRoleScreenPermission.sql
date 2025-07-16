DECLARE @RoleScreenIdForRead  INT, @RoleScreenIdForUpdate  INT


      EXEC Getsinglekey  @TableName = N'RoleScreen', @NextKey = @RoleScreenIdForRead output      

	  INSERT INTO [dbo].[tblRoleScreen]
           ([RoleScreenId]
           ,[RoleId]
           ,[ScreenId]
           ,[PermissionId])
     VALUES
           (@RoleScreenIdForRead
           ,6
           ,133
           ,1)
       
      EXEC Getsinglekey  @TableName = N'RoleScreen', @NextKey = @RoleScreenIdForUpdate output 

	  	  INSERT INTO [dbo].[tblRoleScreen]
           ([RoleScreenId]
           ,[RoleId]
           ,[ScreenId]
           ,[PermissionId])
     VALUES
           (@RoleScreenIdForUpdate
           ,7
           ,133
           ,2)

         
  