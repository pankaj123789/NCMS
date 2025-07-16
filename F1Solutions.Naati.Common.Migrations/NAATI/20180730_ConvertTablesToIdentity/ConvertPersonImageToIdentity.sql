
ALTER TABLE [dbo].[tblPersonImage]
DROP CONSTRAINT [FK_tblPersonImage_tblPerson]
GO
DROP STATISTICS [dbo].[tblPersonImage].[_WA_Sys_00000003_1AE9D794]
GO
DROP STATISTICS [dbo].[tblPersonImage].[_WA_Sys_00000004_1AE9D794]
GO
DROP STATISTICS [dbo].[tblPersonImage].[_WA_Sys_00000005_1AE9D794]
GO
DROP STATISTICS [dbo].[tblPersonImage].[_WA_Sys_00000006_1AE9D794]
GO
DROP STATISTICS [dbo].[tblPersonImage].[_WA_Sys_00000008_1AE9D794]
GO
DROP STATISTICS [dbo].[tblPersonImage].[_WA_Sys_PersonId_1AE9D794]
GO
DROP STATISTICS [dbo].[tblPersonImage].[_WA_Sys_RowVersion_1AE9D794]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblPersonImage]',
	@newname  = N'tmp_b61f36b3a736466ba5b0d59d87a2d4e4',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblPersonImage]',
	@newname  = N'tmp_950c3d6506df4bc99a3ed3515442a9b3',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblPersonImage] (
	[PersonImageId] int IDENTITY(1, 1),
	[PersonId] int NOT NULL,
	[Photo] image NULL,
	[Signature] image NULL,
	[ApplicationFirstPage] image NULL,
	[ApplicationLastPage] image NULL,
	[RowVersion] timestamp NULL,
	[PhotoDate] datetime NULL,
	CONSTRAINT [PK_tblPersonImage] PRIMARY KEY([PersonImageId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblPersonImage] ON
GO
INSERT INTO [dbo].[tblPersonImage] (
	[PersonImageId],
	[PersonId],
	[Photo],
	[Signature],
	[ApplicationFirstPage],
	[ApplicationLastPage],
	[PhotoDate])
SELECT
	[PersonImageId],
	[PersonId],
	[Photo],
	[Signature],
	[ApplicationFirstPage],
	[ApplicationLastPage],
	[PhotoDate]
FROM [dbo].[tmp_950c3d6506df4bc99a3ed3515442a9b3]
GO
SET IDENTITY_INSERT [dbo].[tblPersonImage] OFF
GO
DROP TABLE [dbo].[tmp_950c3d6506df4bc99a3ed3515442a9b3]
GO
ALTER TABLE [dbo].[tblPersonImage]
 ADD CONSTRAINT [FK_tblPersonImage_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
