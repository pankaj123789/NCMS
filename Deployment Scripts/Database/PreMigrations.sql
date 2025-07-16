-- TEST  e.g.
--:setvar samDbName "SAM_TEST1"
--:setvar ncmsDbName "NCMS_TEST1"
--:setvar reportDbName "NCMS_REPORTING_TEST1"
--:setvar domain "f1test"
--:setvar eportalServiceAccount "EPORTALAPPLICATIONAP"

-- Add anything here you want to run after a DB reset but before migrations are run


-- add service account permissions to databases

DECLARE @AllUsers TABLE
(
	UserName NVARCHAR(250)
)
INSERT INTO @AllUsers (UserName) 
	SELECT '$(domain)\$(eportalServiceAccount)' UNION
	SELECT '$(domain)\$(samServiceAccount)' UNION
	SELECT '$(domain)\domain users'


DECLARE @UserSql NVARCHAR(MAX)

SELECT @UserSql = COALESCE (@UserSql + ' ', '') + N'IF(NOT EXISTS (SELECT * FROM sys.database_principals WHERE Name = ''' + UserName + '''))
	     BEGIN
			CREATE USER [' + UserName + '] FOR LOGIN [' + UserName + ']; 
			ALTER ROLE [db_owner] ADD MEMBER [' + UserName + ']; 
		 END'
		FROM @AllUsers

USE [$(ncmsDbName)] -- NCMS DATABASE
exec sp_executesql @UserSql
--update the VersionInfo for NCMSDb
update VersionInfo set [Version] = 201803021203 where [Version] = 2018030281200
update VersionInfo set [Version] = 201806091801 where [Version] = 20805091800
update VersionInfo set [Version] = 201806011002 where [Version] = 2018060101001
update VersionInfo set [Version] = 201806021101 where [Version] = 2018060251101
update VersionInfo set [Version] = 201905161006 where [Version] = 201916051005
update VersionInfo set [Version] = 201905171701 where [Version] = 201917051700
update VersionInfo set [Version] = 201803051501 where [Version] = 20803051500
update VersionInfo set [Version] = 201803021201 where [Version] = 2018030291200

USE [$(reportDbName)] -- REPORT DATABASE
exec sp_executesql @UserSql
--update the VersionInfo for NCMSDb
update VersionInfo set [Version] = 201902141001 where [Version] = 201914021000
update VersionInfo set [Version] = 201902131601 where [Version] = 201913021600
update VersionInfo set [Version] = 201901301701 where [Version] = 201930011700
update VersionInfo set [Version] = 201901311706 where [Version] = 201931011705

USE [$(ncmsDbName)] 
IF EXISTS (SELECT * FROM tblSystemValue WHERE ValueKey = 'SamDatabaseName')
  UPDATE tblSystemValue SET Value = '$(samDbName)' WHERE ValueKey = 'SamDatabaseName'
ELSE
  INSERT INTO tblSystemValue (ValueKey, Value) VALUES ('SamDatabaseName', '$(samDbName)')

