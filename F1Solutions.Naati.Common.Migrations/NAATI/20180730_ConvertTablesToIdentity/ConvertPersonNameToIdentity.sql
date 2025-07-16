
ALTER TABLE [dbo].[tblPersonName]
DROP CONSTRAINT [FK_tblPersonName_tblPerson]
ALTER TABLE [dbo].[tblPersonName]
DROP CONSTRAINT [FK_tblPersonName_tluTitle1]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_1_3_4_5_6]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_1_3_4_5_7_6]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_1_5_2_4_3_6_7]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_2_1_7_5_3]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_2_1_7_6_3_4]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_3_6_1]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_4_6_1_2_3]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_5_3_4_2_1_7]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_6_1_5_2_4]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_6_2_1_3]
GO
DROP STATISTICS [dbo].[tblPersonName].[_dta_stat_629577281_7_2_6]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_00000009_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_0000000A_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_EffectiveDate_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_GivenName_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_OtherNames_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_PersonId_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_RowVersion_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_Surname_25869641]
GO
DROP STATISTICS [dbo].[tblPersonName].[_WA_Sys_TitleId_25869641]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblPersonName]',
	@newname  = N'tmp_a32d9546088e4c30ac359b9bd6356940',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblPersonName]',
	@newname  = N'tmp_958d49cf65504b6d8f40c64c9a6766bf',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblPersonName] (
	[PersonNameId] int IDENTITY(1, 1),
	[PersonId] int NOT NULL,
	[GivenName] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OtherNames] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Surname] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[TitleId] int NULL,
	[EffectiveDate] datetime NOT NULL,
	[RowVersion] timestamp NULL,
	[AlternativeGivenName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	[AlternativeSurname] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	CONSTRAINT [PK_tblPersonName] PRIMARY KEY([PersonNameId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [_dta_index_tblPersonName_7_629577281__K1_K2_3_5]
 ON [dbo].[tblPersonName] ([PersonNameId], [PersonId])
INCLUDE ([GivenName], [Surname])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [_dta_index_tblPersonName_7_629577281__K2_K1_3_5]
 ON [dbo].[tblPersonName] ([PersonId], [PersonNameId])
INCLUDE ([GivenName], [Surname])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [_dta_index_tblPersonName_9_629577281__K2_K1_K5_K3_K4_6]
 ON [dbo].[tblPersonName] ([PersonId], [PersonNameId], [Surname], [GivenName], [OtherNames])
INCLUDE ([TitleId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [_dta_index_tblPersonName_9_629577281__K2_K7]
 ON [dbo].[tblPersonName] ([PersonId], [EffectiveDate])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [IX_PersonNameId_GivenName_OtherNames_Surname]
 ON [dbo].[tblPersonName] ([PersonId])
INCLUDE ([PersonNameId], [GivenName], [OtherNames], [Surname])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [IX_PersonNameId_Person_EffectiveDate]
 ON [dbo].[tblPersonName] ([PersonNameId], [PersonId], [EffectiveDate])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [_dta_index_tblPersonName_7_629577281__K1_K2_3_5]
ON [dbo].[tblPersonName]
DISABLE
ALTER INDEX [_dta_index_tblPersonName_7_629577281__K2_K1_3_5]
ON [dbo].[tblPersonName]
DISABLE
ALTER INDEX [_dta_index_tblPersonName_9_629577281__K2_K1_K5_K3_K4_6]
ON [dbo].[tblPersonName]
DISABLE
ALTER INDEX [_dta_index_tblPersonName_9_629577281__K2_K7]
ON [dbo].[tblPersonName]
DISABLE
ALTER INDEX [IX_PersonNameId_GivenName_OtherNames_Surname]
ON [dbo].[tblPersonName]
DISABLE
ALTER INDEX [IX_PersonNameId_Person_EffectiveDate]
ON [dbo].[tblPersonName]
DISABLE
SET IDENTITY_INSERT [dbo].[tblPersonName] ON
GO
INSERT INTO [dbo].[tblPersonName] (
	[PersonNameId],
	[PersonId],
	[GivenName],
	[OtherNames],
	[Surname],
	[TitleId],
	[EffectiveDate],
	[AlternativeGivenName],
	[AlternativeSurname])
SELECT
	[PersonNameId],
	[PersonId],
	[GivenName],
	[OtherNames],
	[Surname],
	[TitleId],
	[EffectiveDate],
	[AlternativeGivenName],
	[AlternativeSurname]
FROM [dbo].[tmp_958d49cf65504b6d8f40c64c9a6766bf]
GO
SET IDENTITY_INSERT [dbo].[tblPersonName] OFF
ALTER INDEX ALL
ON [dbo].[tblPersonName]
REBUILD
GO
DROP TABLE [dbo].[tmp_958d49cf65504b6d8f40c64c9a6766bf]
GO
ALTER TABLE [dbo].[tblPersonName]
 ADD CONSTRAINT [FK_tblPersonName_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPersonName]
 ADD CONSTRAINT [FK_tblPersonName_tluTitle1] FOREIGN KEY ([TitleId])
		REFERENCES [dbo].[tluTitle] ([TitleId])
	
