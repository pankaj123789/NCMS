
ALTER TABLE [dbo].[tblSidebar]
DROP CONSTRAINT [FK_tblSidebar_tblUser]
ALTER TABLE [dbo].[tblUserSearch]
DROP CONSTRAINT [FK_tblUserSearch_tblUser]
ALTER TABLE [dbo].[tblCredentialApplication]
DROP CONSTRAINT [FK_CredentialApplication_User]
ALTER TABLE [dbo].[tblCredentialApplication]
DROP CONSTRAINT [FK_CredentialApplication_User2]
ALTER TABLE [dbo].[tblCredentialApplication]
DROP CONSTRAINT [FK_CredentialApplication_User3]
ALTER TABLE [dbo].[tblPayroll]
DROP CONSTRAINT [FK_tblPayroll_tblUser]
ALTER TABLE [dbo].[tblEmailBatch]
DROP CONSTRAINT [EmailBatchUser]
ALTER TABLE [dbo].[tblReminder]
DROP CONSTRAINT [FK_tblReminder_tblUser]
ALTER TABLE [dbo].[tblTestResultRubricTestComponentResult]
DROP CONSTRAINT [FK_TestResultRubricTestComponentResult_User]
ALTER TABLE [dbo].[tblNote]
DROP CONSTRAINT [FK_tblNote_tblUser]
ALTER TABLE [dbo].[tblJobExaminer]
DROP CONSTRAINT [FK_JobExaminer_User1]
ALTER TABLE [dbo].[tblJobExaminer]
DROP CONSTRAINT [FK_JobExaminer_User2]
ALTER TABLE [dbo].[tblUserRole]
DROP CONSTRAINT [FK_tblUserRole_tblUser]
ALTER TABLE [dbo].[tblLetterBatch]
DROP CONSTRAINT [FK_tblLetterBatch_tblUser]
ALTER TABLE [dbo].[tblExternalAccountingOperation]
DROP CONSTRAINT [FK_ExternalAccountingOperation_User]
ALTER TABLE [dbo].[tblApiAccess]
DROP CONSTRAINT [FK_ApiAccess_User]
ALTER TABLE [dbo].[tblCredentialRecordLog]
DROP CONSTRAINT [FK_CredentialRecordLog_User]
ALTER TABLE [dbo].[tblJob]
DROP CONSTRAINT [FK_Job_User1]
ALTER TABLE [dbo].[tblJob]
DROP CONSTRAINT [FK_Job_User2]
ALTER TABLE [dbo].[tblJob]
DROP CONSTRAINT [FK_Job_User3]
ALTER TABLE [dbo].[tblInvoiceBatch]
DROP CONSTRAINT [FK_tblInvoiceBatch_tblUser]
ALTER TABLE [dbo].[tblCredentialRequest]
DROP CONSTRAINT [FK_CredentialRequest_User]
ALTER TABLE [dbo].[tblStoredFile]
DROP CONSTRAINT [FK_tblStoredFile_tblUser]
ALTER TABLE [dbo].[tblDashboardReport]
DROP CONSTRAINT [FK_tblDashboardReport_tblUser]
ALTER TABLE [dbo].[tblUser]
DROP CONSTRAINT [FK_tblUser_tblOffice]
GO
DROP STATISTICS [dbo].[tblUser].[_WA_Sys_00000008_398D8EEE]
GO
DROP STATISTICS [dbo].[tblUser].[_WA_Sys_Active_398D8EEE]
GO
DROP STATISTICS [dbo].[tblUser].[_WA_Sys_FullName_398D8EEE]
GO
DROP STATISTICS [dbo].[tblUser].[_WA_Sys_Note_398D8EEE]
GO
DROP STATISTICS [dbo].[tblUser].[_WA_Sys_OfficeId_398D8EEE]
GO
DROP STATISTICS [dbo].[tblUser].[_WA_Sys_RowVersion_398D8EEE]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblUser]',
	@newname  = N'tmp_8f292ef87c2548c5aa80e0ed8781a5e0',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblUser_SystemUser]',
	@newname  = N'tmp_ad8a7eb2101f48f5b395c9b9c1f9341b',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblUser]',
	@newname  = N'tmp_e66d3b5a1c8c47dd9563d41977ebe3d2',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblUser] (
	[UserId] int IDENTITY(1, 1),
	[UserName] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FullName] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OfficeId] int NOT NULL,
	[Note] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Active] bit NOT NULL,
	[RowVersion] timestamp NULL,
	[Email] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SystemUser] bit NOT NULL DEFAULT ((0)),
	CONSTRAINT [PK_tblUser] PRIMARY KEY([UserId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE UNIQUE INDEX [AK_tblUser]
 ON [dbo].[tblUser] ([UserName])
WITH (FILLFACTOR=90,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [AK_tblUser]
ON [dbo].[tblUser]
DISABLE
SET IDENTITY_INSERT [dbo].[tblUser] ON
GO
INSERT INTO [dbo].[tblUser] (
	[UserId],
	[UserName],
	[FullName],
	[OfficeId],
	[Note],
	[Active],
	[Email],
	[SystemUser])
SELECT
	[UserId],
	[UserName],
	[FullName],
	[OfficeId],
	[Note],
	[Active],
	[Email],
	[SystemUser]
FROM [dbo].[tmp_e66d3b5a1c8c47dd9563d41977ebe3d2]
GO
SET IDENTITY_INSERT [dbo].[tblUser] OFF
ALTER INDEX ALL
ON [dbo].[tblUser]
REBUILD
GO
DROP TABLE [dbo].[tmp_e66d3b5a1c8c47dd9563d41977ebe3d2]
GO
ALTER TABLE [dbo].[tblSidebar]
 ADD CONSTRAINT [FK_tblSidebar_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblUserSearch]
 ADD CONSTRAINT [FK_tblUserSearch_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblCredentialApplication]
 ADD CONSTRAINT [FK_CredentialApplication_User] FOREIGN KEY ([EnteredUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblCredentialApplication]
 ADD CONSTRAINT [FK_CredentialApplication_User2] FOREIGN KEY ([StatusChangeUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblCredentialApplication]
 ADD CONSTRAINT [FK_CredentialApplication_User3] FOREIGN KEY ([OwnedByUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblPayroll]
 ADD CONSTRAINT [FK_tblPayroll_tblUser] FOREIGN KEY ([ModifiedUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblEmailBatch]
 ADD CONSTRAINT [EmailBatchUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblReminder]
 ADD CONSTRAINT [FK_tblReminder_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblTestResultRubricTestComponentResult]
 ADD CONSTRAINT [FK_TestResultRubricTestComponentResult_User] FOREIGN KEY ([ModifiedUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblNote]
 ADD CONSTRAINT [FK_tblNote_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblJobExaminer]
 ADD CONSTRAINT [FK_JobExaminer_User1] FOREIGN KEY ([ProductSpecificationChangedUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblJobExaminer]
 ADD CONSTRAINT [FK_JobExaminer_User2] FOREIGN KEY ([ValidatedUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblUserRole]
 ADD CONSTRAINT [FK_tblUserRole_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblLetterBatch]
 ADD CONSTRAINT [FK_tblLetterBatch_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblExternalAccountingOperation]
 ADD CONSTRAINT [FK_ExternalAccountingOperation_User] FOREIGN KEY ([RequestedByUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblApiAccess]
 ADD CONSTRAINT [FK_ApiAccess_User] FOREIGN KEY ([ModifiedUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblCredentialRecordLog]
 ADD CONSTRAINT [FK_CredentialRecordLog_User] FOREIGN KEY ([UserID])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblJob]
 ADD CONSTRAINT [FK_Job_User1] FOREIGN KEY ([SentUserID])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblJob]
 ADD CONSTRAINT [FK_Job_User2] FOREIGN KEY ([ReceivedUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblJob]
 ADD CONSTRAINT [FK_Job_User3] FOREIGN KEY ([SentToPayrollUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblInvoiceBatch]
 ADD CONSTRAINT [FK_tblInvoiceBatch_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblCredentialRequest]
 ADD CONSTRAINT [FK_CredentialRequest_User] FOREIGN KEY ([StatusChangeUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblStoredFile]
 ADD CONSTRAINT [FK_tblStoredFile_tblUser] FOREIGN KEY ([UploadedByUserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblDashboardReport]
 ADD CONSTRAINT [FK_tblDashboardReport_tblUser] FOREIGN KEY ([UserId])
		REFERENCES [dbo].[tblUser] ([UserId])
	
ALTER TABLE [dbo].[tblUser]
 ADD CONSTRAINT [FK_tblUser_tblOffice] FOREIGN KEY ([OfficeId])
		REFERENCES [dbo].[tblOffice] ([OfficeId])
	
