-- TEST  e.g.
:setvar ncmsDbName "NCMS_TEST1"

USE [$(ncmsDbName)]

declare @nextId int
--declare @eportalUserId int
declare @ncmsUserId int
declare @userRoleId int
declare @lastAdminUserId int
Declare @userId int;

update tblSystemValue set Value = 'f78c5fb3-1765-4be2-99e7-0e511a868e25' where ValueKey = 'WiisePaymentAccount'

insert into tbluser (UserName, FullName, OfficeId, Note, Active, Email)
values ('Brendan.Cooke@f1solutions.com.au', 'Brendan Cooke', 1, 0, 1, N'Brendan.Cooke@f1solutions.com.au')

insert into tbluser (UserName, FullName, OfficeId, Note, Active, Email)
values ('Hector.Figueroa@f1solutions.com.au', 'Hector Figueroa', 1, 0, 1, N'Hector.Figueroa@f1solutions.com.au')

insert into tbluser (UserName, FullName, OfficeId, Note, Active, Email)
values ('Henrique.Bovareto@f1solutions.com.au', 'Henrique Bovareto', 1, 0, 1, N'Henrique.Bovareto@f1solutions.com.au') 

insert into tbluser (UserName, FullName, OfficeId, Note, Active, Email)
values ('Simon.Richardson@f1solutions.com.au', 'Simon Richardson', 1, 0, 1, N'Simon.Richardson@f1solutions.com.au')

insert into tbluser (UserName, FullName, OfficeId, Note, Active, Email)
values ('Mananu.Gunawardana@f1solutions.com.au', 'Mananu Gunawardana', 1, 0, 1, N'Mananu.Gunawardana@f1solutions.com.au')

insert into tbluser (UserName, FullName, OfficeId, Note, Active, Email)
values ('Josh.Yip@f1solutions.com.au', 'Josh Yip', 1, 0, 1, N'Josh.Yip@f1solutions.com.au')

insert into tbluserrole (UserId, SecurityRoleId)
select userId, 1 from tblUser where UserName like '%@f1solutions.com.au'
insert into tbluserrole (UserId, SecurityRoleId)
select userId, 2 from tblUser where UserName like '%@f1solutions.com.au'
insert into tbluserrole (UserId, SecurityRoleId)
select userId, 9 from tblUser where UserName like '%@f1solutions.com.au'

update tblEmailTemplate set FromAddress = 'test.'+replace(fromAddress, 'naati.com.au', 'altf4solutions.onmicrosoft.com')
where FromAddress not like 'test%'


---Create Automation User for Test Environment
INSERT INTO [dbo].[tblUser] ([UserName],[FullName],[OfficeId],[Note],[Active],[Email],[SystemUser],[NonWindowsUser],[Password],[LastPasswordChangeDate],[FailedPasswordAttemptCount],[IsLockedOut],[LastLockoutDate]) VALUES ('AUTOMATION.USER','automation.user',1,NULL,1,'automation.user@altf4solutions.com.au',0,1,'jBA79J6axWCZ+Suonzlw4bTlerbaPdBDPXdiLOTa6Dda14nG',NULL,0,0,NULL)

select @userId = UserId from tblUser where USERNAME = 'AUTOMATION.USER'

insert into tbluserrole (UserId, SecurityRoleId)
VALUES
(@userId,1),
(@userId,9)

 --update EmailAddress for automation users in NCMS
declare @EntityId INT

--select @EntityId =  EntityId from tblEntity where NAATINumber = 232180
--update tblEmail set Email = 'Automation.Test.100@altf4solutions.com.au' where EmailId in(select Max(EmailId) EmailId from tblEmail where EntityId = @EntityId and IsPreferredEmail = 1 and Invalid = 0)

--select @EntityId =  EntityId from tblEntity where NAATINumber = 226570
--update tblEmail set Email = 'Automation.Test.101@altf4solutions.com.au' where EmailId in( select Max(EmailId) EmailId from tblEmail where EntityId = @EntityId and IsPreferredEmail = 1 and Invalid = 0)

--select @EntityId =  EntityId from tblEntity where NAATINumber = 2952
--update tblEmail set Email = 'Automation.Test.102@altf4solutions.com.au' where EmailId in( select Max(EmailId) EmailId from tblEmail where EntityId = @EntityId and IsPreferredEmail = 1 and Invalid = 0)

--select @EntityId =  EntityId from tblEntity where NAATINumber = 2724
--update tblEmail set Email = 'Automation.Test.103@altf4solutions.com.au' where EmailId in( select Max(EmailId) EmailId from tblEmail where EntityId = @EntityId and IsPreferredEmail = 1 and Invalid = 0)

--select @EntityId =  EntityId from tblEntity where NAATINumber = 42701
--update tblEmail set Email = 'Automation.Test.104@altf4solutions.com.au' where EmailId in( select Max(EmailId) EmailId from tblEmail where EntityId = @EntityId and IsPreferredEmail = 1 and Invalid = 0)

--select @EntityId =  EntityId from tblEntity where NAATINumber = 6964
--update tblEmail set Email = 'Automation.Test.19@altf4solutions.com.au' where EmailId in( select Max(EmailId) EmailId from tblEmail where EntityId = @EntityId and IsPreferredEmail = 1 and Invalid = 0)

--select @EntityId =  EntityId from tblEntity where NAATINumber = 31943
--update tblEmail set Email = 'Automation.Test.112@altf4solutions.com.au' where EmailId in( select Max(EmailId) EmailId from tblEmail where EntityId = @EntityId and IsPreferredEmail = 1 and Invalid = 0)


--USE [$(ncmsDbName)]

--delete FROM [dbo].[aspnet_UsersInRoles]  where USERId in (N'fe6405e7-7573-4e9d-8954-43ea2676167f',  N'ed52e80d-997f-47b2-9c79-ae2d1a9a6590', N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'3acec270-7e61-473b-ae22-99991dde8c3b', N'062cee94-42be-4d49-b0eb-e084fbf7e318', N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'E639A773-224A-4B31-AD95-85C1502BF048')
--delete FROM [dbo].[aspnet_Membership] where USERId in (N'fe6405e7-7573-4e9d-8954-43ea2676167f',  N'ed52e80d-997f-47b2-9c79-ae2d1a9a6590', N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'3acec270-7e61-473b-ae22-99991dde8c3b', N'062cee94-42be-4d49-b0eb-e084fbf7e318', N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'E639A773-224A-4B31-AD95-85C1502BF048')
--delete from [dbo].[tblMyNaatiUser] where AspUserId in (N'fe6405e7-7573-4e9d-8954-43ea2676167f',  N'ed52e80d-997f-47b2-9c79-ae2d1a9a6590', N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'3acec270-7e61-473b-ae22-99991dde8c3b', N'062cee94-42be-4d49-b0eb-e084fbf7e318', N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'E639A773-224A-4B31-AD95-85C1502BF048')
--delete FROM [dbo].[aspnet_Users] where USERId in (N'fe6405e7-7573-4e9d-8954-43ea2676167f',  N'ed52e80d-997f-47b2-9c79-ae2d1a9a6590', N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'3acec270-7e61-473b-ae22-99991dde8c3b', N'062cee94-42be-4d49-b0eb-e084fbf7e318', N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'E639A773-224A-4B31-AD95-85C1502BF048')
--GO

--INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'fe6405e7-7573-4e9d-8954-43ea2676167f', N'Automation.Test.100@altf4solutions.com.au', N'automation.test.100@altf4solutions.com.au', NULL, 0, CAST(N'2019-08-16T01:40:24.253' AS DateTime))
--GO
--INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'ed52e80d-997f-47b2-9c79-ae2d1a9a6590', N'Automation.Test.101@altf4solutions.com.au', N'automation.test.101@altf4solutions.com.au', NULL, 0, CAST(N'2019-08-16T01:42:23.433' AS DateTime))
--GO
--INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'Automation.Test.102@altf4solutions.com.au', N'automation.test.102@altf4solutions.com.au', NULL, 0, CAST(N'2019-08-16T02:19:25.423' AS DateTime))
--GO
--INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'3acec270-7e61-473b-ae22-99991dde8c3b', N'Automation.Test.103@altf4solutions.com.au', N'automation.test.103@altf4solutions.com.au', NULL, 0, CAST(N'2019-08-16T02:32:18.597' AS DateTime))
--GO
--INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'062cee94-42be-4d49-b0eb-e084fbf7e318', N'Automation.Test.104@altf4solutions.com.au', N'automation.test.104@altf4solutions.com.au', NULL, 0, CAST(N'2019-08-16T02:02:17.890' AS DateTime))
--GO
--INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'Automation.Test.19@altf4solutions.com.au', N'automation.test.19@altf4solutions.com.au', NULL, 0, CAST(N'2019-08-16T02:02:17.890' AS DateTime))
--GO
--INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'E639A773-224A-4B31-AD95-85C1502BF048', N'Automation.Test.112@altf4solutions.com.au', N'automation.test.112@altf4solutions.com.au', NULL, 0, CAST(N'2019-08-16T02:02:17.890' AS DateTime))
--GO

--INSERT [dbo].[tblMyNaatiUser] ([AspUserId], [NaatiNumber]) VALUES (N'fe6405e7-7573-4e9d-8954-43ea2676167f', 232180)
--GO
--INSERT [dbo].[tblMyNaatiUser] ([AspUserId], [NaatiNumber]) VALUES (N'ed52e80d-997f-47b2-9c79-ae2d1a9a6590', 226570)
--GO
--INSERT [dbo].[tblMyNaatiUser] ([AspUserId], [NaatiNumber]) VALUES (N'062cee94-42be-4d49-b0eb-e084fbf7e318', 42701)
--GO
--INSERT [dbo].[tblMyNaatiUser] ([AspUserId], [NaatiNumber]) VALUES (N'50960fcb-aef5-4eec-be7e-a440ce853e77', 2952)
--GO
--INSERT [dbo].[tblMyNaatiUser] ([AspUserId], [NaatiNumber]) VALUES (N'3acec270-7e61-473b-ae22-99991dde8c3b', 2724)
--GO
--INSERT [dbo].[tblMyNaatiUser] ([AspUserId], [NaatiNumber]) VALUES (N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', 6964)
--GO
--INSERT [dbo].[tblMyNaatiUser] ([AspUserId], [NaatiNumber]) VALUES (N'E639A773-224A-4B31-AD95-85C1502BF048', 31943)
--GO

--INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'fe6405e7-7573-4e9d-8954-43ea2676167f', N'4n7xnDkikbJXUnfv4/dpfVwP4ZE=', 1, N'5k34D/IzGQ98Ni1PXWT5Cg==', NULL, N'Automation.Test.100@altf4solutions.com.au', N'automation.test.100@altf4solutions.com.au', N'Uo7CL!aaTC', NULL, 1, 0, CAST(N'2019-08-16T00:36:09.000' AS DateTime), CAST(N'2019-08-16T01:40:23.930' AS DateTime), CAST(N'2019-08-16T01:39:26.687' AS DateTime), CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), NULL)
--GO
--INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'ed52e80d-997f-47b2-9c79-ae2d1a9a6590', N'WxvvOh+vQRZzXd/nvL6Ye8f0Aks=', 1, N'KUKQIlBvQAH8222Pf2ZqKw==', NULL, N'Automation.Test.101@altf4solutions.com.au', N'automation.test.101@altf4solutions.com.au', N'@bRG3Zos$?', NULL, 1, 0, CAST(N'2019-08-16T01:41:27.000' AS DateTime), CAST(N'2019-08-16T01:42:01.483' AS DateTime), CAST(N'2019-08-16T01:42:19.697' AS DateTime), CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), NULL)
--GO
--INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'T4VcQeA/1me3OIfSQX6fcAuhdV0=', 1, N'JCR697ZfxE9Iccp6jXF1WQ==', NULL, N'Automation.Test.102@altf4solutions.com.au', N'automation.test.102@altf4solutions.com.au', N'UYnqWu9@UL', NULL, 1, 0, CAST(N'2019-08-16T02:18:51.000' AS DateTime), CAST(N'2019-08-16T02:19:11.990' AS DateTime), CAST(N'2019-08-16T02:19:25.237' AS DateTime), CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), NULL)
--GO
--INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'3acec270-7e61-473b-ae22-99991dde8c3b', N'FGEHB8jcmkU9djDoMlPX5oS7O9o=', 1, N'x2h2fynoKEskYMLkAsRLqw==', NULL, N'Automation.Test.103@altf4solutions.com.au', N'automation.test.103@altf4solutions.com.au', N'D0!wGWvgNC', NULL, 1, 0, CAST(N'2019-08-16T02:31:41.000' AS DateTime), CAST(N'2019-08-16T02:32:05.417' AS DateTime), CAST(N'2019-08-16T02:32:15.510' AS DateTime), CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), NULL)
--GO
--INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'062cee94-42be-4d49-b0eb-e084fbf7e318', N'/7HJXHZ3ikThTQ5UFXJUxNYDezA=', 1, N'1mPy6gTswy65Q/N3/8kGIw==', NULL, N'Automation.Test.104@altf4solutions.com.au', N'automation.test.104@altf4solutions.com.au', N'6ZQhn@sD25', NULL, 1, 0, CAST(N'2019-08-16T02:01:33.000' AS DateTime), CAST(N'2019-08-16T02:02:00.347' AS DateTime), CAST(N'2019-08-16T02:02:15.553' AS DateTime), CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), NULL)
--GO
--INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'B/gD98+AQNrKcZsl5DEqgTEM0KQ=', 1, N'bktjoStWyyXYN9+7ZNxbzA==', NULL, N'Automation.Test.19@altf4solutions.com.au', N'automation.test.19@altf4solutions.com.au', N'6eTKN$xf23', NULL, 1, 0, CAST(N'2019-08-16T02:01:33.000' AS DateTime), CAST(N'2019-08-16T02:02:00.347' AS DateTime), CAST(N'2019-08-16T02:02:15.553' AS DateTime), CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), NULL)
--GO
--INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'a4b7c679-ed79-491e-841d-34a65938d621', N'E639A773-224A-4B31-AD95-85C1502BF048', N'Z8hdnVactf7syyNB4vyhmkBLXxw=', 1, N'/z/KSZAi+k8DbG9xh7ObIg==', NULL, N'Automation.Test.112@altf4solutions.com.au', N'automation.test.112@altf4solutions.com.au', N'0pRhKX?f02', NULL, 1, 0, CAST(N'2019-08-16T02:01:33.000' AS DateTime), CAST(N'2019-08-16T02:02:00.347' AS DateTime), CAST(N'2019-08-16T02:02:15.553' AS DateTime), CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), 0, CAST(N'1754-01-01T00:00:00.000' AS DateTime), NULL)
--GO



--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'd0a20302-2206-4577-b6d7-2bf4c4f3cf56')
--GO
--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'FD21FF59-A862-4E46-8C64-049D0741D43F')
--GO
--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'50960fcb-aef5-4eec-be7e-a440ce853e77', N'fdf9b749-730e-4399-875b-b2f11efda845')
--GO
--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'062cee94-42be-4d49-b0eb-e084fbf7e318', N'FD21FF59-A862-4E46-8C64-049D0741D43F')
--GO
--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'FD21FF59-A862-4E46-8C64-049D0741D43F')
--GO
--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'5F593FB1-D090-4E63-BCA4-D54642B4A34A', N'ABA1BC3D-E1A1-40AF-B96E-B2568E574CDD')
--GO

--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'E639A773-224A-4B31-AD95-85C1502BF048', N'D0A20302-2206-4577-B6D7-2BF4C4F3CF56')
--GO
--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'E639A773-224A-4B31-AD95-85C1502BF048', N'FDF9B749-730E-4399-875B-B2F11EFDA845')
--GO
--INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'E639A773-224A-4B31-AD95-85C1502BF048', N'FD21FF59-A862-4E46-8C64-049D0741D43F')
--GO