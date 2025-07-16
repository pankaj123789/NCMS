
ALTER TABLE [dbo].[tblInstitutionName]
DROP CONSTRAINT [FK_tblInstitutionName_tblInstitution]
GO
DROP STATISTICS [dbo].[tblInstitutionName].[_WA_Sys_Abbreviation_08EA5793]
GO
DROP STATISTICS [dbo].[tblInstitutionName].[_WA_Sys_EffectiveDate_08EA5793]
GO
DROP STATISTICS [dbo].[tblInstitutionName].[_WA_Sys_InstitutionId_08EA5793]
GO
DROP STATISTICS [dbo].[tblInstitutionName].[_WA_Sys_Name_08EA5793]
GO
DROP STATISTICS [dbo].[tblInstitutionName].[_WA_Sys_RowVersion_08EA5793]
GO
DROP STATISTICS [dbo].[tblInstitutionName].[hind_149575571_1A_2A_4A]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblInstitutionName]',
	@newname  = N'tmp_35f8662337b24a40813a9596cc0d2880',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblInstitutionName]',
	@newname  = N'tmp_a6e7b170b4cc459cb0d74e2ee406460c',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblInstitutionName] (
	[InstitutionNameId] int IDENTITY(1, 1),
	[InstitutionId] int NOT NULL,
	[Name] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Abbreviation] varchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[EffectiveDate] datetime NOT NULL,
	[RowVersion] timestamp NULL,
	[TradingName] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	CONSTRAINT [PK_tblInstitutionName] PRIMARY KEY([InstitutionNameId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [_dta_index_tblInstitutionName_7_149575571__K1_K2_3]
 ON [dbo].[tblInstitutionName] ([InstitutionNameId], [InstitutionId])
INCLUDE ([Name])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [_dta_index_tblInstitutionName_7_149575571__K2_K1]
 ON [dbo].[tblInstitutionName] ([InstitutionId], [InstitutionNameId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [_dta_index_tblInstitutionName_7_149575571__K1_K2_3]
ON [dbo].[tblInstitutionName]
DISABLE
ALTER INDEX [_dta_index_tblInstitutionName_7_149575571__K2_K1]
ON [dbo].[tblInstitutionName]
DISABLE
SET IDENTITY_INSERT [dbo].[tblInstitutionName] ON
GO
INSERT INTO [dbo].[tblInstitutionName] (
	[InstitutionNameId],
	[InstitutionId],
	[Name],
	[Abbreviation],
	[EffectiveDate],
	[TradingName])
SELECT
	[InstitutionNameId],
	[InstitutionId],
	[Name],
	[Abbreviation],
	[EffectiveDate],
	[TradingName]
FROM [dbo].[tmp_a6e7b170b4cc459cb0d74e2ee406460c]
GO
SET IDENTITY_INSERT [dbo].[tblInstitutionName] OFF
ALTER INDEX ALL
ON [dbo].[tblInstitutionName]
REBUILD
GO
DROP TABLE [dbo].[tmp_a6e7b170b4cc459cb0d74e2ee406460c]
GO
ALTER TABLE [dbo].[tblInstitutionName]
 ADD CONSTRAINT [FK_tblInstitutionName_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
