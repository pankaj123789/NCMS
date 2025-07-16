
ALTER TABLE [dbo].[tblTestLocation]
DROP CONSTRAINT [FK_TestLocation_Office]
ALTER TABLE [dbo].[tblUser]
DROP CONSTRAINT [FK_tblUser_tblOffice]
ALTER TABLE [dbo].[tblCredentialApplication]
DROP CONSTRAINT [FK_CredentialApplication_Office]
ALTER TABLE [dbo].[tblTransaction]
DROP CONSTRAINT [FK_tblTransaction_tblOffice]
ALTER TABLE [dbo].[tblApplication]
DROP CONSTRAINT [FK_tblApplication_tblOffice]
ALTER TABLE [dbo].[tblEventVenue]
DROP CONSTRAINT [FK_tblEventVenue_tblOffice]
ALTER TABLE [dbo].[tblTestInvitation]
DROP CONSTRAINT [FK_tblTestInvitation_tblOffice]
ALTER TABLE [dbo].[tblBankDeposit]
DROP CONSTRAINT [FK_tblBankDeposit_tblOffice]
ALTER TABLE [dbo].[tblTestEvent]
DROP CONSTRAINT [FK_TestEvent_Office]
ALTER TABLE [dbo].[tblOffice]
DROP CONSTRAINT [FK_tblOffice_tblInstitution]
ALTER TABLE [dbo].[tblOffice]
DROP CONSTRAINT [FK_tblOffice_tluState]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_00000005_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_00000006_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_00000007_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_00000008_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_DefaultVenueId_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_Regional_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_RowVersion_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[_WA_Sys_StateId_1920BF5C]
GO
DROP STATISTICS [dbo].[tblOffice].[hind_421576540_1A_2A]
GO
DROP STATISTICS [dbo].[tblOffice].[hind_421576540_2A_1A]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[IX_tblOffice]',
	@newname  = N'tmp_a71742e8487e45a7b7c53ea6a4c11c7d',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblOffice]',
	@newname  = N'tmp_b71e01b7c0f5483cbc45a4dbde3c9c7c',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblOffice]',
	@newname  = N'tmp_59b0ba5de33c44239ced01e29c3cff9e',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblOffice] (
	[OfficeId] int IDENTITY(1, 1),
	[InstitutionId] int NOT NULL,
	[StateId] int NULL,
	[Regional] bit NOT NULL,
	[MYOBCustomerName] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CashChequeCode] varchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[EFTCode] varchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MergeLetterOutputFolder] varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[RowVersion] timestamp NULL,
	[DefaultVenueId] int NULL,
	CONSTRAINT [IX_tblOffice] UNIQUE([InstitutionId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY],
	CONSTRAINT [PK_tblOffice] PRIMARY KEY([OfficeId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
ALTER INDEX [IX_tblOffice]
ON [dbo].[tblOffice]
DISABLE
SET IDENTITY_INSERT [dbo].[tblOffice] ON
GO
INSERT INTO [dbo].[tblOffice] (
	[OfficeId],
	[InstitutionId],
	[StateId],
	[Regional],
	[MYOBCustomerName],
	[CashChequeCode],
	[EFTCode],
	[MergeLetterOutputFolder],
	[DefaultVenueId])
SELECT
	[OfficeId],
	[InstitutionId],
	[StateId],
	[Regional],
	[MYOBCustomerName],
	[CashChequeCode],
	[EFTCode],
	[MergeLetterOutputFolder],
	[DefaultVenueId]
FROM [dbo].[tmp_59b0ba5de33c44239ced01e29c3cff9e]
GO
SET IDENTITY_INSERT [dbo].[tblOffice] OFF
ALTER INDEX ALL
ON [dbo].[tblOffice]
REBUILD
GO
DROP TABLE [dbo].[tmp_59b0ba5de33c44239ced01e29c3cff9e]
GO
ALTER TABLE [dbo].[tblTestLocation]
 ADD CONSTRAINT [FK_TestLocation_Office] FOREIGN KEY ([OfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblUser]
 ADD CONSTRAINT [FK_tblUser_tblOffice] FOREIGN KEY ([OfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblCredentialApplication]
 ADD CONSTRAINT [FK_CredentialApplication_Office] FOREIGN KEY ([ReceivingOfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblTransaction]
 ADD CONSTRAINT [FK_tblTransaction_tblOffice] FOREIGN KEY ([OfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblApplication]
 ADD CONSTRAINT [FK_tblApplication_tblOffice] FOREIGN KEY ([ReceivingOfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblEventVenue]
 ADD CONSTRAINT [FK_tblEventVenue_tblOffice] FOREIGN KEY ([OfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblTestInvitation]
 ADD CONSTRAINT [FK_tblTestInvitation_tblOffice] FOREIGN KEY ([ReportOfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblBankDeposit]
 ADD CONSTRAINT [FK_tblBankDeposit_tblOffice] FOREIGN KEY ([OfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblTestEvent]
 ADD CONSTRAINT [FK_TestEvent_Office] FOREIGN KEY ([CreatedByOfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
ALTER TABLE [dbo].[tblOffice]
 ADD CONSTRAINT [FK_tblOffice_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblOffice]
 ADD CONSTRAINT [FK_tblOffice_tluState] FOREIGN KEY ([StateId])
		REFERENCES [dbo].[tluState] ([StateId])
	
