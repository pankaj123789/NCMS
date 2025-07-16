
ALTER TABLE [dbo].[tblOrder]
DROP CONSTRAINT [FK_Order_Suburb]
ALTER TABLE [dbo].[tblPostcode]
DROP CONSTRAINT [FK_tblPostcode_tblSuburb]
ALTER TABLE [dbo].[tblSuburb]
DROP CONSTRAINT [FK_tblSuburb_tluState]
GO
DROP STATISTICS [dbo].[tblSuburb].[_dta_stat_741577680_3_2_1]
GO
DROP STATISTICS [dbo].[tblSuburb].[_WA_Sys_RowVersion_2C3393D0]
GO
DROP STATISTICS [dbo].[tblSuburb].[_WA_Sys_StateId_2C3393D0]
GO
DROP STATISTICS [dbo].[tblSuburb].[_WA_Sys_Suburb_2C3393D0]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblSuburb]',
	@newname  = N'tmp_de148d62bf024bc09158f073494abefc',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblSuburb]',
	@newname  = N'tmp_77c1ebe237e24d528da16a48349a68fc',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblSuburb] (
	[SuburbId] int IDENTITY(1, 1),
	[Suburb] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StateId] int NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblSuburb] PRIMARY KEY([SuburbId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblSuburb] ON
GO
INSERT INTO [dbo].[tblSuburb] (
	[SuburbId],
	[Suburb],
	[StateId])
SELECT
	[SuburbId],
	[Suburb],
	[StateId]
FROM [dbo].[tmp_77c1ebe237e24d528da16a48349a68fc]
GO
SET IDENTITY_INSERT [dbo].[tblSuburb] OFF
GO
DROP TABLE [dbo].[tmp_77c1ebe237e24d528da16a48349a68fc]
GO
ALTER TABLE [dbo].[tblOrder]
 ADD CONSTRAINT [FK_Order_Suburb] FOREIGN KEY ([SuburbId])
		REFERENCES [dbo].[tblSuburb] ([SuburbId])
	
ALTER TABLE [dbo].[tblPostcode]
 ADD CONSTRAINT [FK_tblPostcode_tblSuburb] FOREIGN KEY ([SuburbId])
		REFERENCES [dbo].[tblSuburb] ([SuburbId])
	
ALTER TABLE [dbo].[tblSuburb]
 ADD CONSTRAINT [FK_tblSuburb_tluState] FOREIGN KEY ([StateId])
		REFERENCES [dbo].[tluState] ([StateId])
	
