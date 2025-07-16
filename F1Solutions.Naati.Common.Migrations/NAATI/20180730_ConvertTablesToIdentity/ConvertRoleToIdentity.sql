
ALTER TABLE [dbo].[tblRoleScreen]
DROP CONSTRAINT [FK_tblRoleScreen_tluRole]
ALTER TABLE [dbo].[tblUserRole]
DROP CONSTRAINT [FK_tblUserRole_tblRole]
GO
DROP STATISTICS [dbo].[tluRole].[_WA_Sys_00000002_52593CB8]
GO
DROP STATISTICS [dbo].[tluRole].[_WA_Sys_00000003_52593CB8]
GO
DROP STATISTICS [dbo].[tluRole].[_WA_Sys_00000004_52593CB8]
GO
DROP STATISTICS [dbo].[tluRole].[_WA_Sys_RowVersion_52593CB8]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblRole]',
	@newname  = N'tmp_a289f09621a54c43bc809b486100fc5d',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tluRole_SystemRole]',
	@newname  = N'tmp_c87ffb791b914b5fa07282088879a4d5',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluRole]',
	@newname  = N'tmp_47585605d184445a8634ab0d86fcbfa0',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluRole] (
	[RoleId] int IDENTITY(1, 1),
	[RoleName] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SystemRole] bit NOT NULL DEFAULT (0),
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblRole] PRIMARY KEY([RoleId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluRole] ON
GO
INSERT INTO [dbo].[tluRole] (
	[RoleId],
	[RoleName],
	[Description],
	[SystemRole])
SELECT
	[RoleId],
	[RoleName],
	[Description],
	[SystemRole]
FROM [dbo].[tmp_47585605d184445a8634ab0d86fcbfa0]
GO
SET IDENTITY_INSERT [dbo].[tluRole] OFF
GO
DROP TABLE [dbo].[tmp_47585605d184445a8634ab0d86fcbfa0]
GO
ALTER TABLE [dbo].[tblRoleScreen]
 ADD CONSTRAINT [FK_tblRoleScreen_tluRole] FOREIGN KEY ([RoleId])
		REFERENCES [dbo].[tluRole] ([RoleId])
	
ALTER TABLE [dbo].[tblUserRole]
 ADD CONSTRAINT [FK_tblUserRole_tblRole] FOREIGN KEY ([RoleId])
		REFERENCES [dbo].[tluRole] ([RoleId])
	
