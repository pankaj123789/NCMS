:setvar samDbName "NAATI_UAT"
:setvar domain "NAATI"
:setvar eportalServiceAccount "uateportalwebservice"
:setvar emailServiceAccount "NAATI_EMAIL_SVC"


USE [$(samDbName)]

declare @nextId int
declare @firstNewUserId int

EXEC	[dbo].[GetSingleKey] @TableName = N'User', @NextKey = @nextId OUTPUT
SET @firstNewUserId = @nextId -- store the id of the first new user we added

IF NOT EXISTS (SELECT * FROM tblUSer WHERE UserName = '$(domain)\$(eportalServiceAccount)')
BEGIN
	insert into tbluser (UserId, UserName, FullName, OfficeId, Note, Active, Email)
		values (@nextId, '$(domain)\$(eportalServiceAccount)', 'ePortal App Service', 2, 'Added by script', 1, null) 
END

IF NOT EXISTS (SELECT * FROM tblUSer WHERE UserName = '$(domain)\$(emailServiceAccount)')
BEGIN
	EXEC	[dbo].[GetSingleKey] @TableName = N'User', @NextKey = @nextId OUTPUT
	insert into tbluser (UserId, UserName, FullName, OfficeId, Note, Active, Email)
		values (@nextId, '$(domain)\$(emailServiceAccount)', 'Email Service', 2, 'Added by script', 1,  null)
END

IF NOT EXISTS (SELECT * FROM tblUSer WHERE UserName = '$(domain)\Murray.McCulloch')
BEGIN
	EXEC	[dbo].[GetSingleKey] @TableName = N'User', @NextKey = @nextId OUTPUT
	insert into tbluser (UserId, UserName, FullName, OfficeId, Note, Active, Email)
		values (@nextId, '$(domain)\Murray.McCulloch', 'Murray McCulloch', 2, 'Added by script', 1, null)
END

IF NOT EXISTS (SELECT * FROM tblUSer WHERE UserName = '$(domain)\Pasindu.Wijayatilaka')
BEGIN
	EXEC	[dbo].[GetSingleKey] @TableName = N'User', @NextKey = @nextId OUTPUT
	insert into tbluser (UserId, UserName, FullName, OfficeId, Note, Active, Email)
		values (@nextId, '$(domain)\Pasindu.Wijayatilaka', 'Pasindu Wijayatilaka', 2, 'Added by script', 1, null)
END

IF NOT EXISTS (SELECT * FROM tblUSer WHERE UserName = '$(domain)\June.Ngo')
BEGIN
	EXEC	[dbo].[GetSingleKey] @TableName = N'User', @NextKey = @nextId OUTPUT
	insert into tbluser (UserId, UserName, FullName, OfficeId, Note, Active, Email)
		values (@nextId, '$(domain)\June.Ngo', 'June Ngo', 2, 'Added by script', 1, null)
END

IF NOT EXISTS (SELECT * FROM tblUSer WHERE UserName = '$(domain)\Zachariah.McDevitt')
BEGIN
	EXEC	[dbo].[GetSingleKey] @TableName = N'User', @NextKey = @nextId OUTPUT
	insert into tbluser (UserId, UserName, FullName, OfficeId, Note, Active, Email)
		values (@nextId, '$(domain)\Zachariah.McDevitt', 'Zachariah McDevitt', 2, 'Added by script', 1, null)
END

IF NOT EXISTS (SELECT * FROM tblUSer WHERE UserName = '$(domain)\Zarthost.Boman')
BEGIN
	EXEC	[dbo].[GetSingleKey] @TableName = N'User', @NextKey = @nextId OUTPUT
	insert into tbluser (UserId, UserName, FullName, OfficeId, Note, Active, Email)
		values (@nextId, '$(domain)\Zarthost.Boman', 'Zarthost Boman', 2, 'Added by script', 1, null)
END


DECLARE roleCursor CURSOR FOR
	SELECT [UserId], [RoleId]
		FROM [tblUser], [tluRole]
		WHERE tluRole.RoleName NOT IN ('ReadOnly User', 'Regional Office')
			AND tblUser.UserId > @firstNewUserId
			
DECLARE @UserRoleId int, @UserId int, @RoleId int

OPEN roleCursor
FETCH NEXT FROM roleCursor INTO @UserId, @RoleId

WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC [dbo].[GetSingleKey] @TableName = N'UserRole', @NextKey = @UserRoleId OUTPUT	

	INSERT INTO [dbo].[tblUserRole] (UserRoleId, UserId, RoleId) VALUES (@UserRoleId, @UserId, @RoleId)

	FETCH NEXT FROM roleCursor INTO @UserId, @RoleId
END

CLOSE roleCursor
DEALLOCATE roleCursor

-- note that this part is UAT-specific
USE [NAATI_UAT]
CREATE USER [NAATI\UAT_Webservice] FOR LOGIN [NAATI\UAT_Webservice] WITH DEFAULT_SCHEMA=[dbo]
ALTER ROLE [db_owner] ADD MEMBER [NAATI\UAT_Webservice]
CREATE USER [NAATI\uateportalwebservice] FOR LOGIN [NAATI\uateportalwebservice] WITH DEFAULT_SCHEMA=[dbo]
ALTER ROLE [db_datareader] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [db_datawriter] ADD MEMBER [NAATI\uateportalwebservice]
GRANT EXECUTE ON [dbo].[NH_EntityInsert] TO [NAATI\uateportalwebservice]
CREATE USER [NAATI\NWS009_OctoClient_Sv] FOR LOGIN [NAATI\NWS009_OctoClient_Sv]
ALTER ROLE [db_owner] ADD MEMBER [NAATI\NWS009_OctoClient_Sv]
USE [NAATI_ePortal_UAT]
CREATE USER [NAATI\uateportalwebservice] FOR LOGIN [NAATI\uateportalwebservice]
ALTER ROLE [aspnet_Membership_BasicAccess] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [aspnet_Membership_FullAccess] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [aspnet_Membership_ReportingAccess] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [aspnet_Roles_BasicAccess] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [aspnet_Roles_FullAccess] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [aspnet_Roles_ReportingAccess] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [db_datareader] ADD MEMBER [NAATI\uateportalwebservice]
ALTER ROLE [db_datawriter] ADD MEMBER [NAATI\uateportalwebservice]CREATE USER [NAATI\NWS009_OctoClient_Sv] FOR LOGIN [NAATI\NWS009_OctoClient_Sv]
ALTER ROLE [db_owner] ADD MEMBER [NAATI\NWS009_OctoClient_Sv]


