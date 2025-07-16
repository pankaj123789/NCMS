
ALTER TABLE [dbo].[tblAddress]
DROP CONSTRAINT [FK_tblAddress_tblPostcode]
ALTER TABLE [dbo].[tblPostcode]
DROP CONSTRAINT [FK_tblPostcode_tblSuburb]
GO
DROP STATISTICS [dbo].[tblPostcode].[_dta_stat_661577395_2_3]
GO
DROP STATISTICS [dbo].[tblPostcode].[_dta_stat_661577395_3_1_2]
GO
DROP STATISTICS [dbo].[tblPostcode].[_WA_Sys_Postcode_276EDEB3]
GO
DROP STATISTICS [dbo].[tblPostcode].[_WA_Sys_RowVersion_276EDEB3]
GO
DROP STATISTICS [dbo].[tblPostcode].[_WA_Sys_SuburbId_276EDEB3]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblPostcode]',
	@newname  = N'tmp_c065310a45b0483990226265e66b3be5',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblPostcode]',
	@newname  = N'tmp_cfab647c42244d2989b427c4edcc444f',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblPostcode] (
	[PostcodeId] int IDENTITY(1, 1),
	[SuburbId] int NOT NULL,
	[Postcode] char(4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblPostcode] PRIMARY KEY([PostcodeId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [IX_tblPostcode_Postcode]
 ON [dbo].[tblPostcode] ([Postcode])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [IX_tblPostcode_SuburbId]
 ON [dbo].[tblPostcode] ([SuburbId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [IX_tblPostcode_Postcode]
ON [dbo].[tblPostcode]
DISABLE
SET IDENTITY_INSERT [dbo].[tblPostcode] ON
GO
INSERT INTO [dbo].[tblPostcode] (
	[PostcodeId],
	[SuburbId],
	[Postcode])
SELECT
	[PostcodeId],
	[SuburbId],
	[Postcode]
FROM [dbo].[tmp_cfab647c42244d2989b427c4edcc444f]
GO
SET IDENTITY_INSERT [dbo].[tblPostcode] OFF
ALTER INDEX [IX_tblPostcode_Postcode]
ON [dbo].[tblPostcode]
REBUILD
GO
DROP TABLE [dbo].[tmp_cfab647c42244d2989b427c4edcc444f]
GO
ALTER TABLE [dbo].[tblAddress]
 ADD CONSTRAINT [FK_tblAddress_tblPostcode] FOREIGN KEY ([PostcodeId])
		REFERENCES [dbo].[tblPostcode] ([PostcodeId])
	
ALTER TABLE [dbo].[tblPostcode]
 ADD CONSTRAINT [FK_tblPostcode_tblSuburb] FOREIGN KEY ([SuburbId])
		REFERENCES [dbo].[tblSuburb] ([SuburbId])
	
