
ALTER TABLE [dbo].[tblTestResultFailureReason]
DROP CONSTRAINT [FK_tblTestResultFailureReason_tluFailureReason]
ALTER TABLE [dbo].[tblAccreditationResult]
DROP CONSTRAINT [FK_tblAccreditationResult_tluFailureReason]
GO
DROP STATISTICS [dbo].[tluFailureReason].[_WA_Sys_00000002_48CFD27E]
GO
DROP STATISTICS [dbo].[tluFailureReason].[_WA_Sys_00000003_48CFD27E]
GO
DROP STATISTICS [dbo].[tluFailureReason].[_WA_Sys_RowVersion_48CFD27E]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tluFailureReason]',
	@newname  = N'tmp_1927b0e17f544cc2a9f50e9a02fa9670',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluFailureReason]',
	@newname  = N'tmp_0a9e58e423d24646a28729e1b20aa9a8',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluFailureReason] (
	[FailureReasonId] int IDENTITY(1, 1),
	[Name] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tluFailureReason] PRIMARY KEY([FailureReasonId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluFailureReason] ON
GO
INSERT INTO [dbo].[tluFailureReason] (
	[FailureReasonId],
	[Name],
	[Description])
SELECT
	[FailureReasonId],
	[Name],
	[Description]
FROM [dbo].[tmp_0a9e58e423d24646a28729e1b20aa9a8]
GO
SET IDENTITY_INSERT [dbo].[tluFailureReason] OFF
GO
DROP TABLE [dbo].[tmp_0a9e58e423d24646a28729e1b20aa9a8]
GO
ALTER TABLE [dbo].[tblTestResultFailureReason]
 ADD CONSTRAINT [FK_tblTestResultFailureReason_tluFailureReason] FOREIGN KEY ([FailureReasonId])
		REFERENCES [dbo].[tluFailureReason] ([FailureReasonId])
	
ALTER TABLE [dbo].[tblAccreditationResult]
 ADD CONSTRAINT [FK_tblAccreditationResult_tluFailureReason] FOREIGN KEY ([FailureReasonId])
		REFERENCES [dbo].[tluFailureReason] ([FailureReasonId])
	
