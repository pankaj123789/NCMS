ALTER TABLE [dbo].[tblMergeData]
DROP CONSTRAINT [FK_tblMergeData_tblEntity]
ALTER TABLE [dbo].[tblPhone]
DROP CONSTRAINT [FK_tblPhone_tblEntity]
ALTER TABLE [dbo].[tblTransaction]
DROP CONSTRAINT [FK_tblTransaction_tblEntity]
ALTER TABLE [dbo].[tblReminder]
DROP CONSTRAINT [FK_tblReminder_tblEntity]
ALTER TABLE [dbo].[tblEmailMessage]
DROP CONSTRAINT [FK_EmailMessage_Entity]
ALTER TABLE [dbo].[tblEventVenue]
DROP CONSTRAINT [FK_tblEventVenue_tblEntity]
ALTER TABLE [dbo].[tblAddress]
DROP CONSTRAINT [FK_tblAddress_tblEntity]
ALTER TABLE [dbo].[tblEntityCorrespondence]
DROP CONSTRAINT [FK_tblEntityCorrespondence_tblEntity]
ALTER TABLE [dbo].[tblChangeRequest]
DROP CONSTRAINT [FK_tblChangeRequest_EmailToEntity]
ALTER TABLE [dbo].[tblChangeRequest]
DROP CONSTRAINT [FK_tblChangeRequest_LetterToEntity]
ALTER TABLE [dbo].[tblEntityNote]
DROP CONSTRAINT [FK_EntityNote_Entity]
ALTER TABLE [dbo].[tblPerson]
DROP CONSTRAINT [FK_tblPerson_tblEntity]
ALTER TABLE [dbo].[tblEmail]
DROP CONSTRAINT [FK_tblEmail_tblEntity]
ALTER TABLE [dbo].[tblInstitution]
DROP CONSTRAINT [FK_tblInstitution_tblEntity]
ALTER TABLE [dbo].[tblInstitutionMergeLog]
DROP CONSTRAINT [FK_tblInstitutionMergeLog_tblEntity]
GO
DROP STATISTICS [dbo].[tblEntity].[_dta_stat_708913597_2_6]
GO
DROP STATISTICS [dbo].[tblEntity].[_dta_stat_708913597_8_1]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_00000003_2A4129BD]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_00000004_2A4129BD]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_00000005_2A4129BD]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_0000000A_2A4129BD]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_GSTApplies_2A4129BD]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_RowVersion_2A4129BD]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_WebsiteInPD_2A4129BD]
GO
DROP STATISTICS [dbo].[tblEntity].[_WA_Sys_WebsiteURL_2A4129BD]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblEntity]',
	@newname  = N'tmp_5764c9f6a381419c9fe48bdd679d3d5d',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[IX_NAATINumber]',
	@newname  = N'tmp_4b3ffe49ed0546e29e4e365dcba606f7',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblEntity]',
	@newname  = N'tmp_96254df7e2354518ae5771020d560a42',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblEntity] (
	[EntityId] int IDENTITY(1, 1),
	[WebsiteURL] varchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ABN] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Note] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UseEmail] bit NOT NULL,
	[WebsiteInPD] bit NOT NULL,
	[GSTApplies] bit NOT NULL,
	[NAATINumber] int NULL,
	[RowVersion] timestamp NULL,
	[EntityTypeId] int NULL,
	[AccountNumber] varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT [PK_tblEntity] PRIMARY KEY([EntityId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY],
	CONSTRAINT [IX_NAATINumber] UNIQUE([NAATINumber]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [_dta_index_tblEntity_7_708913597__K1_K8]
 ON [dbo].[tblEntity] ([EntityId], [NAATINumber])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [_dta_index_tblEntity_9_708913597__K6_K1_K2]
 ON [dbo].[tblEntity] ([WebsiteInPD], [EntityId], [WebsiteURL])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [IX_NAATINumber]
ON [dbo].[tblEntity]
DISABLE
ALTER INDEX [_dta_index_tblEntity_7_708913597__K1_K8]
ON [dbo].[tblEntity]
DISABLE
ALTER INDEX [_dta_index_tblEntity_9_708913597__K6_K1_K2]
ON [dbo].[tblEntity]
DISABLE
SET IDENTITY_INSERT [dbo].[tblEntity] ON
GO
INSERT INTO [dbo].[tblEntity] (
	[EntityId],
	[WebsiteURL],
	[ABN],
	[Note],
	[UseEmail],
	[WebsiteInPD],
	[GSTApplies],
	[NAATINumber],
	[EntityTypeId],
	[AccountNumber])
SELECT
	[EntityId],
	[WebsiteURL],
	[ABN],
	[Note],
	[UseEmail],
	[WebsiteInPD],
	[GSTApplies],
	[NAATINumber],
	[EntityTypeId],
	[AccountNumber]
FROM [dbo].[tmp_96254df7e2354518ae5771020d560a42]
GO
SET IDENTITY_INSERT [dbo].[tblEntity] OFF
ALTER INDEX ALL
ON [dbo].[tblEntity]
REBUILD
GO
DROP TABLE [dbo].[tmp_96254df7e2354518ae5771020d560a42]
GO
ALTER TABLE [dbo].[tblMergeData]
 ADD CONSTRAINT [FK_tblMergeData_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblPhone]
 ADD CONSTRAINT [FK_tblPhone_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblTransaction]
 ADD CONSTRAINT [FK_tblTransaction_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblReminder]
 ADD CONSTRAINT [FK_tblReminder_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblEmailMessage]
 ADD CONSTRAINT [FK_EmailMessage_Entity] FOREIGN KEY ([RecipientEntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblEventVenue]
 ADD CONSTRAINT [FK_tblEventVenue_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblAddress]
 ADD CONSTRAINT [FK_tblAddress_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblEntityCorrespondence]
 ADD CONSTRAINT [FK_tblEntityCorrespondence_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblChangeRequest]
 ADD CONSTRAINT [FK_tblChangeRequest_EmailToEntity] FOREIGN KEY ([EmailToEntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblChangeRequest]
 ADD CONSTRAINT [FK_tblChangeRequest_LetterToEntity] FOREIGN KEY ([LetterToEntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblEntityNote]
 ADD CONSTRAINT [FK_EntityNote_Entity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblPerson]
 ADD CONSTRAINT [FK_tblPerson_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblEmail]
 ADD CONSTRAINT [FK_tblEmail_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblInstitution]
 ADD CONSTRAINT [FK_tblInstitution_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
ALTER TABLE [dbo].[tblInstitutionMergeLog]
 ADD CONSTRAINT [FK_tblInstitutionMergeLog_tblEntity] FOREIGN KEY ([PrimaryEntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
