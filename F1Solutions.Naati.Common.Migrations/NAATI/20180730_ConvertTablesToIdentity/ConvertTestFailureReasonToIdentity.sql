
ALTER TABLE [dbo].[tblTestResultFailureReason]
DROP CONSTRAINT [FK_tblTestResultFailureReason_tluFailureReason]
ALTER TABLE [dbo].[tblTestResultFailureReason]
DROP CONSTRAINT [FK_TestResultFailureReason_TestResult]
GO
DROP STATISTICS [dbo].[tblTestResultFailureReason].[_WA_Sys_FailureReasonId_36B12243]
GO
DROP STATISTICS [dbo].[tblTestResultFailureReason].[_WA_Sys_RowVersion_36B12243]
GO
DROP STATISTICS [dbo].[tblTestResultFailureReason].[_WA_Sys_TestResultId_36B12243]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblTestResultFailureReason]',
	@newname  = N'tmp_7efd1f9fa8cd4c6c8bf4c23430700c2b',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblTestResultFailureReason]',
	@newname  = N'tmp_6ee36a71dd174248854098e378373d11',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblTestResultFailureReason] (
	[TestResultFailureReasonId] int IDENTITY(1, 1),
	[TestResultId] int NOT NULL,
	[FailureReasonId] int NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tblTestResultFailureReason] PRIMARY KEY([TestResultFailureReasonId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblTestResultFailureReason] ON
GO
INSERT INTO [dbo].[tblTestResultFailureReason] (
	[TestResultFailureReasonId],
	[TestResultId],
	[FailureReasonId])
SELECT
	[TestResultFailureReasonId],
	[TestResultId],
	[FailureReasonId]
FROM [dbo].[tmp_6ee36a71dd174248854098e378373d11]
GO
SET IDENTITY_INSERT [dbo].[tblTestResultFailureReason] OFF
GO
DROP TABLE [dbo].[tmp_6ee36a71dd174248854098e378373d11]
GO
ALTER TABLE [dbo].[tblTestResultFailureReason]
 ADD CONSTRAINT [FK_tblTestResultFailureReason_tluFailureReason] FOREIGN KEY ([FailureReasonId])
		REFERENCES [dbo].[tluFailureReason] ([FailureReasonId])
	
ALTER TABLE [dbo].[tblTestResultFailureReason]
 ADD CONSTRAINT [FK_TestResultFailureReason_TestResult] FOREIGN KEY ([TestResultId])
		REFERENCES [dbo].[tblTestResult] ([TestResultId])
	
