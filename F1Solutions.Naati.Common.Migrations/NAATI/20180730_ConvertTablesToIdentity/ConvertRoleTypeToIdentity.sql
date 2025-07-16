
ALTER TABLE [dbo].[tblPanelMembership]
DROP CONSTRAINT [FK_tblPanelMembership_tluRoleType]
GO
DROP STATISTICS [dbo].[tluRoleType].[_WA_Sys_00000003_534D60F1]
GO
DROP STATISTICS [dbo].[tluRoleType].[_WA_Sys_00000005_534D60F1]
GO
DROP STATISTICS [dbo].[tluRoleType].[_WA_Sys_Name_534D60F1]
GO
DROP STATISTICS [dbo].[tluRoleType].[_WA_Sys_RowVersion_534D60F1]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tluRoleType]',
	@newname  = N'tmp_0f6adf47c190478d84f25386208f1586',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluRoleType]',
	@newname  = N'tmp_4e7000e67fc04b668c3fba961aac9839',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluRoleType] (
	[RoleTypeId] int IDENTITY(1, 1),
	[Name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	[RoleVisible] bit NOT NULL DEFAULT (1),
	CONSTRAINT [PK_tluRoleType] PRIMARY KEY([RoleTypeId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluRoleType] ON
GO
INSERT INTO [dbo].[tluRoleType] (
	[RoleTypeId],
	[Name],
	[Description],
	[RoleVisible])
SELECT
	[RoleTypeId],
	[Name],
	[Description],
	[RoleVisible]
FROM [dbo].[tmp_4e7000e67fc04b668c3fba961aac9839]
GO
SET IDENTITY_INSERT [dbo].[tluRoleType] OFF
GO
DROP TABLE [dbo].[tmp_4e7000e67fc04b668c3fba961aac9839]
GO
ALTER TABLE [dbo].[tblPanelMembership]
 ADD CONSTRAINT [FK_tblPanelMembership_tluRoleType] FOREIGN KEY ([RoleTypeId])
		REFERENCES [dbo].[tluRoleType] ([RoleTypeId])
	
