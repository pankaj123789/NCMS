
ALTER TABLE [dbo].[tblPanelMembership]
DROP CONSTRAINT [FK_tblPanelMembership_tblPanel]
ALTER TABLE [dbo].[tblPanelNote]
DROP CONSTRAINT [FK_tblPanelNote_tblPanel]
ALTER TABLE [dbo].[tblPanel]
DROP CONSTRAINT [FK_tblPanel_tblLanguage]
ALTER TABLE [dbo].[tblPanel]
DROP CONSTRAINT [FK_tblPanel_tluPanelType]
GO
DROP STATISTICS [dbo].[tblPanel].[_WA_Sys_00000004_1B0907CE]
GO
DROP STATISTICS [dbo].[tblPanel].[_WA_Sys_00000006_1B0907CE]
GO
DROP STATISTICS [dbo].[tblPanel].[_WA_Sys_00000008_1B0907CE]
GO
DROP STATISTICS [dbo].[tblPanel].[_WA_Sys_LanguageId_1B0907CE]
GO
DROP STATISTICS [dbo].[tblPanel].[_WA_Sys_Name_1B0907CE]
GO
DROP STATISTICS [dbo].[tblPanel].[_WA_Sys_PanelTypeId_1B0907CE]
GO
DROP STATISTICS [dbo].[tblPanel].[_WA_Sys_RowVersion_1B0907CE]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblPanel]',
	@newname  = N'tmp_a3de57a1bc234f25bc3f74c76d286a1a',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[df_VisibleInEportal]',
	@newname  = N'tmp_5667d3bd258f49fd8c69de1447400929',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblPanel]',
	@newname  = N'tmp_fdcf97e317db422dba5b2fd69b56254d',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblPanel] (
	[PanelId] int IDENTITY(1, 1),
	[LanguageId] int NULL,
	[Name] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Note] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PanelTypeId] int NOT NULL,
	[CommissionedDate] datetime NOT NULL,
	[RowVersion] timestamp NULL,
	[VisibleInEportal] bit NOT NULL DEFAULT ((1)),
	CONSTRAINT [PK_tblPanel] PRIMARY KEY([PanelId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblPanel] ON
GO
INSERT INTO [dbo].[tblPanel] (
	[PanelId],
	[LanguageId],
	[Name],
	[Note],
	[PanelTypeId],
	[CommissionedDate],
	[VisibleInEportal])
SELECT
	[PanelId],
	[LanguageId],
	[Name],
	[Note],
	[PanelTypeId],
	[CommissionedDate],
	[VisibleInEportal]
FROM [dbo].[tmp_fdcf97e317db422dba5b2fd69b56254d]
GO
SET IDENTITY_INSERT [dbo].[tblPanel] OFF
GO
DROP TABLE [dbo].[tmp_fdcf97e317db422dba5b2fd69b56254d]
GO
ALTER TABLE [dbo].[tblPanelMembership]
 ADD CONSTRAINT [FK_tblPanelMembership_tblPanel] FOREIGN KEY ([PanelId])
		REFERENCES [dbo].[tblPanel] ([PanelId])
	
ALTER TABLE [dbo].[tblPanelNote]
 ADD CONSTRAINT [FK_tblPanelNote_tblPanel] FOREIGN KEY ([PanelId])
		REFERENCES [dbo].[tblPanel] ([PanelId])
	
ALTER TABLE [dbo].[tblPanel]
 ADD CONSTRAINT [FK_tblPanel_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblPanel]
 ADD CONSTRAINT [FK_tblPanel_tluPanelType] FOREIGN KEY ([PanelTypeId])
		REFERENCES [dbo].[tluPanelType] ([PanelTypeId])
	
