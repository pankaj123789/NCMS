ALTER TABLE [dbo].[tblPhone]
DROP CONSTRAINT [FK_tblPhone_tblEntity]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_00000003_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_00000004_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_00000005_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_00000007_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_0000000C_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_0000000D_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_0000000E_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_EntityId_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_IncludeInPD_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_Number_2D52A092]
GO
DROP STATISTICS [dbo].[tblPhone].[_WA_Sys_RowVersion_2D52A092]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblPhone]',
	@newname  = N'tmp_6598d244e58f456fa02272e285362289',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblPhone_CountryCode]',
	@newname  = N'tmp_10ebfa82bba544e3ac7049c27b35b573',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblPhone_AreaCode]',
	@newname  = N'tmp_b49ea40b45424aa1baf31698195627a6',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblPhone_LocalNumber]',
	@newname  = N'tmp_dc34f90b1ca24bc3acd583cb5983da4d',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[default_AllowSmsNotification]',
	@newname  = N'tmp_c373f85bd9654cdfb7d41aa910a1ec9f',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblPhone_ExaminerCorrespondence]',
	@newname  = N'tmp_aecd3a31ae6b4c57a926211c646f006c',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblPhone]',
	@newname  = N'tmp_f69ccab3f3f64546bc250f705357f285',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblPhone] (
	[PhoneId] int IDENTITY(1, 1),
	[EntityId] int NOT NULL,
	[CountryCode] varchar(4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	[AreaCode] varchar(4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	[LocalNumber] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	[Number] AS (rtrim(ltrim(([CountryCode] + ' ' + [AreaCode] + ' ' + case when (len([LocalNumber]) = 8) then (substring([LocalNumber],1,4) + ' ' + substring([LocalNumber],5,4)) else [LocalNumber] end)))),
	[Note] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IncludeInPD] bit NOT NULL,
	[RowVersion] timestamp NULL,
	[AllowSmsNotification] bit NOT NULL DEFAULT ((0)),
	[Invalid] bit NOT NULL DEFAULT ((0)),
	[PrimaryContact] bit NOT NULL DEFAULT ((0)),
	[ExaminerCorrespondence] bit NOT NULL DEFAULT ((0)),
	CONSTRAINT [PK_tblPhone] PRIMARY KEY([PhoneId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblPhone] ON
GO
INSERT INTO [dbo].[tblPhone] (
	[PhoneId],
	[EntityId],
	[CountryCode],
	[AreaCode],
	[LocalNumber],
	[Note],
	[IncludeInPD],
	[AllowSmsNotification],
	[Invalid],
	[PrimaryContact],
	[ExaminerCorrespondence])
SELECT
	[PhoneId],
	[EntityId],
	[CountryCode],
	[AreaCode],
	[LocalNumber],
	[Note],
	[IncludeInPD],
	[AllowSmsNotification],
	[Invalid],
	[PrimaryContact],
	[ExaminerCorrespondence]
FROM [dbo].[tmp_f69ccab3f3f64546bc250f705357f285]
GO
SET IDENTITY_INSERT [dbo].[tblPhone] OFF
GO
DROP TABLE [dbo].[tmp_f69ccab3f3f64546bc250f705357f285]
GO
ALTER TABLE [dbo].[tblPhone]
 ADD CONSTRAINT [FK_tblPhone_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
