ALTER TABLE [dbo].[tblTestResult]
DROP CONSTRAINT [FK_TestResult_ResultType]
ALTER TABLE [dbo].[tblAccreditationResult]
DROP CONSTRAINT [FK_tblAccreditationResult_tluResultType]
GO
DROP STATISTICS [dbo].[tluResultType].[_WA_Sys_Result_5165187F]
GO
DROP STATISTICS [dbo].[tluResultType].[_WA_Sys_RowVersion_5165187F]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tluResultType]',
	@newname  = N'tmp_fe98b49e6fc54d0ea682063c9cdf81b1',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluResultType]',
	@newname  = N'tmp_a30286a2a4ac4c18a58a62283ceeb2c8',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluResultType] (
	[ResultTypeId] int IDENTITY(1, 1),
	[Result] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tluResultType] PRIMARY KEY([ResultTypeId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluResultType] ON
GO
INSERT INTO [dbo].[tluResultType] (
	[ResultTypeId],
	[Result])
SELECT
	[ResultTypeId],
	[Result]
FROM [dbo].[tmp_a30286a2a4ac4c18a58a62283ceeb2c8]
GO
SET IDENTITY_INSERT [dbo].[tluResultType] OFF
GO
DROP TABLE [dbo].[tmp_a30286a2a4ac4c18a58a62283ceeb2c8]
GO
ALTER TABLE [dbo].[tblTestResult]
 ADD CONSTRAINT [FK_TestResult_ResultType] FOREIGN KEY ([ResultTypeId])
		REFERENCES [dbo].[tluResultType] ([ResultTypeId])
	
ALTER TABLE [dbo].[tblAccreditationResult]
 ADD CONSTRAINT [FK_tblAccreditationResult_tluResultType] FOREIGN KEY ([ResultTypeId])
		REFERENCES [dbo].[tluResultType] ([ResultTypeId])
	
