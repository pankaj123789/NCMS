
ALTER TABLE [dbo].[tblOffice]
DROP CONSTRAINT [FK_tblOffice_tluState]
ALTER TABLE [dbo].[tblSuburb]
DROP CONSTRAINT [FK_tblSuburb_tluState]
ALTER TABLE [dbo].[tluRegion]
DROP CONSTRAINT [FK_tluRegion_tluState]
GO
DROP STATISTICS [dbo].[tluState].[_WA_Sys_Name_5535A963]
GO
DROP STATISTICS [dbo].[tluState].[_WA_Sys_RowVersion_5535A963]
GO
DROP STATISTICS [dbo].[tluState].[_WA_Sys_State_5535A963]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblState]',
	@newname  = N'tmp_bec112096cbe44d0bc7ab03eb0aeb718',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluState]',
	@newname  = N'tmp_f4060057f53941da8a2df98d68c92dae',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluState] (
	[StateId] int IDENTITY(1, 1),
	[State] char(3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblState] PRIMARY KEY([StateId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluState] ON
GO
INSERT INTO [dbo].[tluState] (
	[StateId],
	[State],
	[Name])
SELECT
	[StateId],
	[State],
	[Name]
FROM [dbo].[tmp_f4060057f53941da8a2df98d68c92dae]
GO
SET IDENTITY_INSERT [dbo].[tluState] OFF
GO
DROP TABLE [dbo].[tmp_f4060057f53941da8a2df98d68c92dae]
GO
ALTER TABLE [dbo].[tblOffice]
 ADD CONSTRAINT [FK_tblOffice_tluState] FOREIGN KEY ([StateId])
		REFERENCES [dbo].[tluState] ([StateId])
	
ALTER TABLE [dbo].[tblSuburb]
 ADD CONSTRAINT [FK_tblSuburb_tluState] FOREIGN KEY ([StateId])
		REFERENCES [dbo].[tluState] ([StateId])
	
ALTER TABLE [dbo].[tluRegion]
 ADD CONSTRAINT [FK_tluRegion_tluState] FOREIGN KEY ([StateId])
		REFERENCES [dbo].[tluState] ([StateId])
	
