GO
ALTER TABLE [dbo].[tblEmailBatchRecipient]
DROP CONSTRAINT [FK_tblEmailBatchRecipient_EmailId_tblEmail_EmailId]
ALTER TABLE [dbo].[tblEmail]
DROP CONSTRAINT [FK_tblEmail_tblEntity]
GO
DROP STATISTICS [dbo].[tblEmail].[_WA_Sys_00000004_00551192]
GO
DROP STATISTICS [dbo].[tblEmail].[_WA_Sys_00000009_00551192]
GO
DROP STATISTICS [dbo].[tblEmail].[_WA_Sys_Email_00551192]
GO
DROP STATISTICS [dbo].[tblEmail].[_WA_Sys_EntityId_00551192]
GO
DROP STATISTICS [dbo].[tblEmail].[_WA_Sys_IncludeInPD_00551192]
GO
DROP STATISTICS [dbo].[tblEmail].[_WA_Sys_IsPreferredEmail_00551192]
GO
DROP STATISTICS [dbo].[tblEmail].[_WA_Sys_RowVersion_00551192]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblEmail]',
	@newname  = N'tmp_0295a03acffc4901826bcf9b212e3dfb',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblEmail_ExaminerCorrespondence]',
	@newname  = N'tmp_60853c661bc14ef595ba3b67e0e9cbb2',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblEmail]',
	@newname  = N'tmp_214cefbcb2cf4db590078df3e0801df9',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblEmail] (
	[EmailId] int IDENTITY(1, 1),
	[EntityId] int NOT NULL,
	[Email] varchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Note] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IncludeInPD] bit NOT NULL,
	[RowVersion] timestamp NULL,
	[IsPreferredEmail] bit NOT NULL DEFAULT (0),
	[Invalid] bit NOT NULL DEFAULT ((0)),
	[ExaminerCorrespondence] bit NOT NULL DEFAULT ((0)),
	CONSTRAINT [PK_tblEmail] PRIMARY KEY([EmailId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [IX_EntityId]
 ON [dbo].[tblEmail] ([EntityId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [IX_EntityId]
ON [dbo].[tblEmail]
DISABLE
SET IDENTITY_INSERT [dbo].[tblEmail] ON
GO
INSERT INTO [dbo].[tblEmail] (
	[EmailId],
	[EntityId],
	[Email],
	[Note],
	[IncludeInPD],
	[IsPreferredEmail],
	[Invalid],
	[ExaminerCorrespondence])
SELECT
	[EmailId],
	[EntityId],
	[Email],
	[Note],
	[IncludeInPD],
	[IsPreferredEmail],
	[Invalid],
	[ExaminerCorrespondence]
FROM [dbo].[tmp_214cefbcb2cf4db590078df3e0801df9]
GO
SET IDENTITY_INSERT [dbo].[tblEmail] OFF
ALTER INDEX ALL
ON [dbo].[tblEmail]
REBUILD
GO
DROP TABLE [dbo].[tmp_214cefbcb2cf4db590078df3e0801df9]
GO
ALTER TABLE [dbo].[tblEmailBatchRecipient]
 ADD CONSTRAINT [FK_tblEmailBatchRecipient_EmailId_tblEmail_EmailId] FOREIGN KEY ([EmailId])
		REFERENCES [dbo].[tblEmail] ([EmailId])
	
ALTER TABLE [dbo].[tblEmail]
 ADD CONSTRAINT [FK_tblEmail_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	