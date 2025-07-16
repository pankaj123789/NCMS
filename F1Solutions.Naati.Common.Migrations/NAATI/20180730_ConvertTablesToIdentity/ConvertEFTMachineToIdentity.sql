
DROP STATISTICS [dbo].[tluEFTMachine].[_WA_Sys_00000004_6E814571]
GO
DROP STATISTICS [dbo].[tluEFTMachine].[_WA_Sys_OfficeId_6E814571]
GO
DROP STATISTICS [dbo].[tluEFTMachine].[_WA_Sys_RowVersion_6E814571]
GO
DROP STATISTICS [dbo].[tluEFTMachine].[_WA_Sys_TerminalNo_6E814571]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tluEFTMachine]',
	@newname  = N'tmp_26ab43b7ba174104b0094ff95af3ba03',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tluEFTMachine_Visible]',
	@newname  = N'tmp_339c23d9aaa8446d8990fe4219d5d2b0',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tluEFTMachine]',
	@newname  = N'tmp_a818e7369d6a40169374365ebca9c663',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tluEFTMachine] (
	[EFTMachineId] int IDENTITY(1, 1),
	[TerminalNo] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OfficeId] int NOT NULL,
	[Visible] bit NOT NULL DEFAULT (1),
	[RowVersion] timestamp NULL,
	CONSTRAINT [PK_tluEFTMachine] PRIMARY KEY([EFTMachineId]) WITH (FILLFACTOR=100,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tluEFTMachine] ON
GO
INSERT INTO [dbo].[tluEFTMachine] (
	[EFTMachineId],
	[TerminalNo],
	[OfficeId],
	[Visible])
SELECT
	[EFTMachineId],
	[TerminalNo],
	[OfficeId],
	[Visible]
FROM [dbo].[tmp_a818e7369d6a40169374365ebca9c663]
GO
SET IDENTITY_INSERT [dbo].[tluEFTMachine] OFF
GO
DROP TABLE [dbo].[tmp_a818e7369d6a40169374365ebca9c663]
GO
