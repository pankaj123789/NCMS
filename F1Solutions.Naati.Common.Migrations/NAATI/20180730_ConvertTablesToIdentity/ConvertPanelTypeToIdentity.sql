
ALTER TABLE [dbo].[tblPanel]
DROP CONSTRAINT [FK_tblPanel_tluPanelType]
GO
DROP STATISTICS [dbo].[tluPanelType].[_WA_Sys_00000002_4BAC3F29]
GO
DROP STATISTICS [dbo].[tluPanelType].[_WA_Sys_00000003_4BAC3F29]
GO
DROP STATISTICS [dbo].[tluPanelType].[_WA_Sys_RowVersion_4BAC3F29]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tluPanelType]',
	@newname  = N'tmp_c16f6fd1d9d4449f86e45cca7b88c0e1',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluPanelType]',
	@newname  = N'tmp_9d5e491a17b94affbf8b4cdb5e2e2ad9',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluPanelType] (
	[PanelTypeId] int IDENTITY(1, 1),
	[Name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tluPanelType] PRIMARY KEY([PanelTypeId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluPanelType] ON
GO
INSERT INTO [dbo].[tluPanelType] (
	[PanelTypeId],
	[Name],
	[Description])
SELECT
	[PanelTypeId],
	[Name],
	[Description]
FROM [dbo].[tmp_9d5e491a17b94affbf8b4cdb5e2e2ad9]
GO
SET IDENTITY_INSERT [dbo].[tluPanelType] OFF
GO
DROP TABLE [dbo].[tmp_9d5e491a17b94affbf8b4cdb5e2e2ad9]
GO
ALTER TABLE [dbo].[tblPanel]
 ADD CONSTRAINT [FK_tblPanel_tluPanelType] FOREIGN KEY ([PanelTypeId])
		REFERENCES [dbo].[tluPanelType] ([PanelTypeId])
	
