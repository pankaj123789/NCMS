ALTER TABLE [dbo].[tblMergeData]
DROP CONSTRAINT [FK_tblMergeData_tblAddress]
ALTER TABLE [dbo].[tblMailingListAddress]
DROP CONSTRAINT [FK_tblMailingListAddress_tblAddress]
ALTER TABLE [dbo].[tblAddressMailCategory]
DROP CONSTRAINT [FK_tblAddressMailCategory_tblAddress]
ALTER TABLE [dbo].[tblAddress]
DROP CONSTRAINT [FK_tblAddress_tblEntity]
ALTER TABLE [dbo].[tblAddress]
DROP CONSTRAINT [FK_Address_ODAddressVisibilityType]
ALTER TABLE [dbo].[tblAddress]
DROP CONSTRAINT [FK_tblAddress_tblPostcode]
GO
DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_4_9]
GO
DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_5_4_2]
GO
DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_9_2_5_4]
GO
DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_9_5_4]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_00000003_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_00000013_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_ContactPerson_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_CountryId_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_EndDate_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_EntityId_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_Invalid_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_Note_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_PostcodeId_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_PrimaryContact_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_RowVersion_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_StartDate_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_SubscriptionExpiryDate_77BFCB91]
GO
DROP STATISTICS [dbo].[tblAddress].[_WA_Sys_SubscriptionRenewSentDate_77BFCB91]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblAddress]',
	@newname  = N'tmp_53d1d021156c4503b7bb1edfa31dffae',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblAddress_EndDate]',
	@newname  = N'tmp_04c87a84d8f644d2b152cd65b36c341f',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblAddress_PrimaryContact]',
	@newname  = N'tmp_5153e46c4338475991e2f507772d108a',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblAddress_ExaminerCorrespondence]',
	@newname  = N'tmp_08c6523da17c4b899a8c897eb82c5474',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblAddress]',
	@newname  = N'tmp_4d403b5601c0417581dceb7d36301abf',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblAddress] (
	[AddressId] int IDENTITY(1, 1),
	[EntityId] int NOT NULL,
	[StreetDetails] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PostcodeId] int NULL,
	[CountryId] int NOT NULL,
	[Note] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StartDate] datetime NOT NULL,
	[EndDate] datetime NULL DEFAULT ('1978/05/05'),
	[PrimaryContact] bit NOT NULL DEFAULT (1),
	[Invalid] bit NOT NULL,
	[SubscriptionExpiryDate] datetime NULL,
	[RowVersion] timestamp NULL,
	[SubscriptionRenewSentDate] datetime NULL DEFAULT (null),
	[ContactPerson] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ValidateInExternalTool] bit NULL,
	[ExaminerCorrespondence] bit NOT NULL DEFAULT ((0)),
	[ODAddressVisibilityTypeId] int NOT NULL,
	CONSTRAINT [PK_tblAddress] PRIMARY KEY([AddressId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [IX_EntityId_PrimaryContact]
 ON [dbo].[tblAddress] ([EntityId], [PrimaryContact])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [IX_EntityId_PrimaryContact]
ON [dbo].[tblAddress]
DISABLE
SET IDENTITY_INSERT [dbo].[tblAddress] ON
GO
INSERT INTO [dbo].[tblAddress] (
	[AddressId],
	[EntityId],
	[StreetDetails],
	[PostcodeId],
	[CountryId],
	[Note],
	[StartDate],
	[EndDate],
	[PrimaryContact],
	[Invalid],
	[SubscriptionExpiryDate],
	[SubscriptionRenewSentDate],
	[ContactPerson],
	[ValidateInExternalTool],
	[ExaminerCorrespondence],
	[ODAddressVisibilityTypeId])
SELECT
	[AddressId],
	[EntityId],
	[StreetDetails],
	[PostcodeId],
	[CountryId],
	[Note],
	[StartDate],
	[EndDate],
	[PrimaryContact],
	[Invalid],
	[SubscriptionExpiryDate],
	[SubscriptionRenewSentDate],
	[ContactPerson],
	[ValidateInExternalTool],
	[ExaminerCorrespondence],
	[ODAddressVisibilityTypeId]
FROM [dbo].[tmp_4d403b5601c0417581dceb7d36301abf]
GO
SET IDENTITY_INSERT [dbo].[tblAddress] OFF
ALTER INDEX ALL
ON [dbo].[tblAddress]
REBUILD
GO
DROP TABLE [dbo].[tmp_4d403b5601c0417581dceb7d36301abf]
GO
ALTER TABLE [dbo].[tblMergeData]
 ADD CONSTRAINT [FK_tblMergeData_tblAddress] FOREIGN KEY ([AddressId])
		REFERENCES [dbo].[tblAddress] ([AddressId])
	
ALTER TABLE [dbo].[tblMailingListAddress]
 ADD CONSTRAINT [FK_tblMailingListAddress_tblAddress] FOREIGN KEY ([AddressId])
		REFERENCES [dbo].[tblAddress] ([AddressId])
	
ALTER TABLE [dbo].[tblAddressMailCategory]
 ADD CONSTRAINT [FK_tblAddressMailCategory_tblAddress] FOREIGN KEY ([AddressId])
		REFERENCES [dbo].[tblAddress] ([AddressId])
	
ALTER TABLE [dbo].[tblAddress]
 ADD CONSTRAINT [FK_tblAddress_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblAddress]
 ADD CONSTRAINT [FK_Address_ODAddressVisibilityType] FOREIGN KEY ([ODAddressVisibilityTypeId])
		REFERENCES [dbo].[tblODAddressVisibilityType] ([ODAddressVisibilityTypeId])
	
ALTER TABLE [dbo].[tblAddress]
 ADD CONSTRAINT [FK_tblAddress_tblPostcode] FOREIGN KEY ([PostcodeId])
		REFERENCES [dbo].[tblPostcode] ([PostcodeId])