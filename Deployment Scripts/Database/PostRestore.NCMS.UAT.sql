-- this script is for restoring an NCMS PROD database into UAT
-- it should be run before running migrations

:setvar ncms_db_name "NCMS_UAT1"
:setvar ncms_reporting_db_name "NCMS_Reporting_UATScripts3"

:setvar user_username "ncuatuser"
:setvar admin_username "ncuatadmin"

:setvar dev_username "Hector.Figueroa"
:setvar dev_username2 "Pasindu.Wijayatilaka"

USE [$(ncms_db_name)]

-- sanitise emails (don't want to send an email to a customer from UAT)
update tblEmail 
set Email =  substring(email, 1, charindex('@', email)-1) + '.' + cast(NaatiNumber as varchar(6)) + '@altf4solutions.com.au'
from tblEmail
join tblEntity ON tblEntity.EntityId = tblEmail.EntityId

update t 
set t.userName =  substring(t.userName, 1, charindex('@', t.userName)-1) + '.' + cast(u.NaatiNumber as varchar(6)) + '@altf4solutions.com.au',
t.LoweredUserName =  substring(t.LoweredUserName, 1, charindex('@', t.LoweredUserName)-1) + '.' + cast(u.NaatiNumber as varchar(6)) + '@altf4solutions.com.au'
from aspnet_users t
inner join tblmynaatiuser u ON u.AspUserId = t.UserId

update t 
set t.Email =  substring(t.email, 1, charindex('@', t.email)-1) + '.' + cast(u.NaatiNumber as varchar(6)) + '@altf4solutions.com.au',
t.LoweredEmail =  substring(t.LoweredEmail, 1, charindex('@', t.LoweredEmail)-1) + '.' + cast(u.NaatiNumber as varchar(6)) + '@altf4solutions.com.au'
from aspnet_Membership t
inner join tblmynaatiuser u ON u.AspUserId = t.UserId


update tblSystemValue set Value = 'SDEULQJ6LH2VUJGA8PBBBXN0ZB2PIH' where ValueKey = 'WiiseApiKey'
update tblSystemValue set Value = 'b44329e2-01db-4d3c-9420-cb14910fa69b' where ValueKey = 'WiisePaymentAccount'
update tblSystemValue set value = 'o8D4Q/o8dD2EGbfFeuQrqW+ZeMxeSQiW1woK9Cf5j74Wj1z2fQzEIwnEogMYa4cwFg2Wj4KAxfarRFXzJztMB7sDHid2O8fknRT9ndU9QvqVGlUzie5lPsxHtrdN8ugs3BADqECSHcXVsDkgiUXOUYdXDqZ1VTH9+SeHJ8UC1hh3sfVY' where ValueKey = 'MicrosoftGraphAppKey'

UPDATE tblSystemValue SET Value = 'Deprecated_DB' WHERE ValueKey = 'SamDatabaseName'

UPDATE tblApiAccess SET Inactive = 0

UPDATE tblEmailTemplate
SET FromAddress = 'uat.' + FromAddress

UPDATE tblEmailMessage 
SET FromAddress = 'uat.' + FromAddress 

insert into tbluser (UserName, FullName, OfficeId, Note, Active, Email)
	values ('Simon.Richardson@f1solutions.com.au', 'Simon Richardson', 2, 'Added by post-restore script', 1, 'Simon.Richardson@f1solutions.com.au')


insert into tblUserRole
(UserId,SecurityRoleId)
select UserId, 1 from tblUser where userName 
in('Simon.Richardson@f1solutions.com.au',
'Hector.Figueroa@f1solutions.com.au',
'Josh.Yip@f1solutions.com.au')

--SET BUILD VERSION TO ZERO IN NCMS and REPORTING DATABASES

USE [$(ncms_db_name)]
GO
update tblsystemvalue set Value = '0' where ValueKey = 'BuildVersion'
GO

USE [$(ncms_reporting_db_name)]
GO
update tblsystemvalue set Value = '0' where ValueKey = 'BuildVersion'
GO

USE [$(ncms_db_name)]
GO
INSERT INTO tblApiAccess 
(InstitutionId, CreatedDate, ModifiedDate, MODIFIEDUSERID, INACTIVE, PublicKey,PrivateKey,Permissions)
VALUES( 1303, GETDATE(), GETDATE(), 40, 0, '1ATB4B7041B7BBCB1C7CCCB040CB1687B17B4B7041B7BBCB1C7CCCB040CB11AT',
'1ATCCB0B1484B0704CCC4C7CB0E4CC41B4ECCB0B1484B0704CCC4C7CB0E4C1AT', 32767)


---ADD DB USERS

USE [$(ncms_db_name)]
GO
ALTER AUTHORIZATION ON SCHEMA::[MyNaatiHangFire] TO [dbo]
GO

USE [$(ncms_db_name)]
GO
DROP USER [$(admin_username)]
GO
CREATE USER [$(admin_username)] FOR LOGIN [$(admin_username)] WITH DEFAULT_SCHEMA=[dbo]
GO
DROP USER [$(user_username)]
GO
CREATE USER [$(user_username)] FOR LOGIN [$(user_username)] WITH DEFAULT_SCHEMA=[dbo]
GO
GRANT EXECUTE ON SCHEMA::[dbo] TO [$(user_username)]
GO
ALTER ROLE [db_datareader] ADD MEMBER [$(user_username)]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [$(user_username)]
GO
ALTER ROLE [db_owner] ADD MEMBER [$(admin_username)]
GO

--ADD Developer Access

USE [$(ncms_db_name)]
GO

DROP USER [$(dev_username)]
GO
CREATE USER [$(dev_username)] FOR LOGIN [$(dev_username)] WITH DEFAULT_SCHEMA=[dbo]
GO
GRANT EXECUTE ON SCHEMA::[dbo] TO [$(dev_username)]
GO
ALTER ROLE [db_datareader] ADD MEMBER [$(dev_username)]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [$(dev_username)]
GO

USE [$(ncms_db_name)]
GO

DROP USER [$(dev_username2)]
GO
CREATE USER [$(dev_username2)] FOR LOGIN [$(dev_username2)] WITH DEFAULT_SCHEMA=[dbo]
GO
GRANT EXECUTE ON SCHEMA::[dbo] TO [$(dev_username2)]
GO
ALTER ROLE [db_datareader] ADD MEMBER [$(dev_username2)]
GO
