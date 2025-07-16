ALTER TABLE [dbo].[tblJobExaminer]
DROP CONSTRAINT [FK_JobExaminer_PanelMembership]
ALTER TABLE [dbo].[tblPanelMembership]
DROP CONSTRAINT [FK_tblPanelMembership_tblPanel]
ALTER TABLE [dbo].[tblPanelMembership]
DROP CONSTRAINT [FK_tblPanelMembership_tblPerson]
ALTER TABLE [dbo].[tblPanelMembership]
DROP CONSTRAINT [FK_tblPanelMembership_tluRoleType]
GO
DROP STATISTICS [dbo].[tblPanelMembership].[_WA_Sys_EndDate_1BFD2C07]
GO
DROP STATISTICS [dbo].[tblPanelMembership].[_WA_Sys_PanelId_1BFD2C07]
GO
DROP STATISTICS [dbo].[tblPanelMembership].[_WA_Sys_PersonId_1BFD2C07]
GO
DROP STATISTICS [dbo].[tblPanelMembership].[_WA_Sys_RoleTypeId_1BFD2C07]
GO
DROP STATISTICS [dbo].[tblPanelMembership].[_WA_Sys_RowVersion_1BFD2C07]
GO
DROP STATISTICS [dbo].[tblPanelMembership].[_WA_Sys_StartDate_1BFD2C07]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblPanelMembership]',
	@newname  = N'tmp_18676c151d8b40e5ab58c3cb7252fdb3',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblPanelMembership]',
	@newname  = N'tmp_a436477a043047eb93c817ea20d96339',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblPanelMembership] (
	[PanelMembershipId] int IDENTITY(1, 1),
	[PersonId] int NOT NULL,
	[PanelId] int NOT NULL,
	[RoleTypeId] int NOT NULL,
	[StartDate] datetime NOT NULL,
	[EndDate] datetime NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblPanelMembership] PRIMARY KEY([PanelMembershipId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [_dta_index_tblPanelMembership_7_469576711__K1_K2]
 ON [dbo].[tblPanelMembership] ([PanelMembershipId], [PersonId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [_dta_index_tblPanelMembership_7_469576711__K1_K2]
ON [dbo].[tblPanelMembership]
DISABLE
SET IDENTITY_INSERT [dbo].[tblPanelMembership] ON
GO
INSERT INTO [dbo].[tblPanelMembership] (
	[PanelMembershipId],
	[PersonId],
	[PanelId],
	[RoleTypeId],
	[StartDate],
	[EndDate])
SELECT
	[PanelMembershipId],
	[PersonId],
	[PanelId],
	[RoleTypeId],
	[StartDate],
	[EndDate]
FROM [dbo].[tmp_a436477a043047eb93c817ea20d96339]
GO
SET IDENTITY_INSERT [dbo].[tblPanelMembership] OFF
ALTER INDEX ALL
ON [dbo].[tblPanelMembership]
REBUILD
GO
DROP TABLE [dbo].[tmp_a436477a043047eb93c817ea20d96339]
GO
ALTER TABLE [dbo].[tblJobExaminer]
 ADD CONSTRAINT [FK_JobExaminer_PanelMembership] FOREIGN KEY ([PanelMembershipId])
		REFERENCES [dbo].[tblPanelMembership] ([PanelMembershipId])
	
ALTER TABLE [dbo].[tblPanelMembership]
 ADD CONSTRAINT [FK_tblPanelMembership_tblPanel] FOREIGN KEY ([PanelId])
		REFERENCES [dbo].[tblPanel] ([PanelId])
	
ALTER TABLE [dbo].[tblPanelMembership]
 ADD CONSTRAINT [FK_tblPanelMembership_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPanelMembership]
 ADD CONSTRAINT [FK_tblPanelMembership_tluRoleType] FOREIGN KEY ([RoleTypeId])
		REFERENCES [dbo].[tluRoleType] ([RoleTypeId])
	
