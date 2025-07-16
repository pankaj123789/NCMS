
ALTER TABLE [dbo].[tblOrder]
DROP CONSTRAINT [FK_Order_Country]
ALTER TABLE [dbo].[tblTestLocation]
DROP CONSTRAINT [FK_TestLocation_Country]
ALTER TABLE [dbo].[tblApplication]
DROP CONSTRAINT [FK_tblApplication_tblCountry]
ALTER TABLE [dbo].[tblPerson]
DROP CONSTRAINT [FK_tblPerson_tblCountry]
ALTER TABLE [dbo].[tluRegion]
DROP CONSTRAINT [FK_tluRegion_tblCountry]
GO
DROP STATISTICS [dbo].[tblCountry].[_WA_Sys_00000003_7C8480AE]
GO
DROP STATISTICS [dbo].[tblCountry].[_WA_Sys_Name_7C8480AE]
GO
DROP STATISTICS [dbo].[tblCountry].[_WA_Sys_RowVersion_7C8480AE]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblCountry]',
	@newname  = N'tmp_466a147610734cf3917efaaededa001f',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblCountry]',
	@newname  = N'tmp_758377afaf4248338d4d7cd6c180e25f',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblCountry] (
	[CountryId] int IDENTITY(1, 1),
	[Name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Code] varchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblCountry] PRIMARY KEY([CountryId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblCountry] ON
GO
INSERT INTO [dbo].[tblCountry] (
	[CountryId],
	[Name],
	[Code])
SELECT
	[CountryId],
	[Name],
	[Code]
FROM [dbo].[tmp_758377afaf4248338d4d7cd6c180e25f]
GO
SET IDENTITY_INSERT [dbo].[tblCountry] OFF
GO
DROP TABLE [dbo].[tmp_758377afaf4248338d4d7cd6c180e25f]
GO
ALTER TABLE [dbo].[tblOrder]
 ADD CONSTRAINT [FK_Order_Country] FOREIGN KEY ([CountryId])
		REFERENCES [dbo].[tblCountry] ([CountryId])
	
ALTER TABLE [dbo].[tblTestLocation]
 ADD CONSTRAINT [FK_TestLocation_Country] FOREIGN KEY ([CountryId])
		REFERENCES [dbo].[tblCountry] ([CountryId])
	
ALTER TABLE [dbo].[tblApplication]
 ADD CONSTRAINT [FK_tblApplication_tblCountry] FOREIGN KEY ([CountryOfTrainingId])
		REFERENCES [dbo].[tblCountry] ([CountryId])
	
ALTER TABLE [dbo].[tblPerson]
 ADD CONSTRAINT [FK_tblPerson_tblCountry] FOREIGN KEY ([BirthCountryId])
		REFERENCES [dbo].[tblCountry] ([CountryId])
	
ALTER TABLE [dbo].[tluRegion]
 ADD CONSTRAINT [FK_tluRegion_tblCountry] FOREIGN KEY ([CountryId])
		REFERENCES [dbo].[tblCountry] ([CountryId])
	
