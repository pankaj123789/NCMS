
ALTER TABLE [dbo].[tblRoleScreen]
DROP CONSTRAINT [FK_tblRoleScreen_tluPermission]
ALTER TABLE [dbo].[tblRoleScreen]
DROP CONSTRAINT [FK_tblRoleScreen_tluRole]
GO
DROP STATISTICS [dbo].[tblRoleScreen].[_WA_Sys_PermissionId_2A4B4B5E]
GO
DROP STATISTICS [dbo].[tblRoleScreen].[_WA_Sys_RoleId_2A4B4B5E]
GO
DROP STATISTICS [dbo].[tblRoleScreen].[_WA_Sys_RowVersion_2A4B4B5E]
GO
DROP STATISTICS [dbo].[tblRoleScreen].[_WA_Sys_ScreenId_2A4B4B5E]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblRoleScreen]',
	@newname  = N'tmp_d22018c82d76405cb14cc384462e6302',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblRoleScreen]',
	@newname  = N'tmp_0f19f8d0358f44da983f9166662b6ddb',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblRoleScreen] (
	[RoleScreenId] int IDENTITY(1, 1),
	[RoleId] int NOT NULL,
	[ScreenId] int NOT NULL,
	[PermissionId] int NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblRoleScreen] PRIMARY KEY([RoleScreenId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblRoleScreen] ON
GO
INSERT INTO [dbo].[tblRoleScreen] (
	[RoleScreenId],
	[RoleId],
	[ScreenId],
	[PermissionId])
SELECT
	[RoleScreenId],
	[RoleId],
	[ScreenId],
	[PermissionId]
FROM [dbo].[tmp_0f19f8d0358f44da983f9166662b6ddb]
GO
SET IDENTITY_INSERT [dbo].[tblRoleScreen] OFF
GO
DROP TABLE [dbo].[tmp_0f19f8d0358f44da983f9166662b6ddb]
GO
ALTER TABLE [dbo].[tblRoleScreen]
 ADD CONSTRAINT [FK_tblRoleScreen_tluPermission] FOREIGN KEY ([PermissionId])
		REFERENCES [dbo].[tluPermission] ([PermissionId])
	
ALTER TABLE [dbo].[tblRoleScreen]
 ADD CONSTRAINT [FK_tblRoleScreen_tluRole] FOREIGN KEY ([RoleId])
		REFERENCES [dbo].[tluRole] ([RoleId])
	
