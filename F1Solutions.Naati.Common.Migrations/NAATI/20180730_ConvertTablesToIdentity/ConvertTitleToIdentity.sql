ALTER TABLE [dbo].[tblPersonName]
DROP CONSTRAINT [FK_tblPersonName_tluTitle1]
GO
DROP STATISTICS [dbo].[tluTitle].[_WA_Sys_00000003_59063A47]
GO
DROP STATISTICS [dbo].[tluTitle].[_WA_Sys_RowVersion_59063A47]
GO
DROP STATISTICS [dbo].[tluTitle].[_WA_Sys_Title_59063A47]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tluTitle]',
	@newname  = N'tmp_aa0513a56cdb404482ddefaf86f61bea',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluTitle]',
	@newname  = N'tmp_c28a1c4fa6034629a054d65aaa8a18bc',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluTitle] (
	[TitleId] int IDENTITY(1, 1),
	[Title] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StandardTitle] bit NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tluTitle] PRIMARY KEY([TitleId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluTitle] ON
GO
INSERT INTO [dbo].[tluTitle] (
	[TitleId],
	[Title],
	[StandardTitle])
SELECT
	[TitleId],
	[Title],
	[StandardTitle]
FROM [dbo].[tmp_c28a1c4fa6034629a054d65aaa8a18bc]
GO
SET IDENTITY_INSERT [dbo].[tluTitle] OFF
GO
DROP TABLE [dbo].[tmp_c28a1c4fa6034629a054d65aaa8a18bc]
GO
ALTER TABLE [dbo].[tblPersonName]
 ADD CONSTRAINT [FK_tblPersonName_tluTitle1] FOREIGN KEY ([TitleId])
		REFERENCES [dbo].[tluTitle] ([TitleId])
	
