
ALTER TABLE [dbo].[tblUserRole]
DROP CONSTRAINT [FK_tblUserRole_tblRole]
ALTER TABLE [dbo].[tblUserRole]
DROP CONSTRAINT [FK_tblUserRole_tblUser]
GO
DROP STATISTICS [dbo].[tblUserRole].[_WA_Sys_RoleId_3A81B327]
GO
DROP STATISTICS [dbo].[tblUserRole].[_WA_Sys_RowVersion_3A81B327]
GO
DROP STATISTICS [dbo].[tblUserRole].[_WA_Sys_UserId_3A81B327]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblUserRole]',
	@newname  = N'tmp_4172bdaafdc747af989464be5ff093c9',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblUserRole]',
	@newname  = N'tmp_7a37e40271214d88832f77667ebed3e5',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblUserRole] (
	[UserRoleId] int IDENTITY(1, 1),
	[UserId] int NOT NULL,
	[RoleId] int NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblUserRole] PRIMARY KEY([UserRoleId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblUserRole] ON
GO
INSERT INTO [dbo].[tblUserRole] (
	[UserRoleId],
	[UserId],
	[RoleId])
SELECT
	[UserRoleId],
	[UserId],
	[RoleId]
FROM [dbo].[tmp_7a37e40271214d88832f77667ebed3e5]
GO
SET IDENTITY_INSERT [dbo].[tblUserRole] OFF
GO
DROP TABLE [dbo].[tmp_7a37e40271214d88832f77667ebed3e5]
GO
ALTER TABLE [dbo].[tblUserRole]
 ADD CONSTRAINT [FK_tblUserRole_tblRole] FOREIGN KEY ([RoleId])
		REFERENCES [dbo].[tluRole] ([RoleId])
	
ALTER TABLE [dbo].[tblUserRole]
 ADD CONSTRAINT [FK_tblUserRole_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
