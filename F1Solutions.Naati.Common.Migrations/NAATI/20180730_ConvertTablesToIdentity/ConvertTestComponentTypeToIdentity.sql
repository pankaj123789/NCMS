
ALTER TABLE [dbo].[tblRubricMarkingCompetency]
DROP CONSTRAINT [FK_RubricMarkingCompetency_TestComponentType]
ALTER TABLE [dbo].[tblTestComponentTypeStandardMarkingScheme]
DROP CONSTRAINT [FK_TestComponentTypeStandardMarkingScheme_TestComponentType]
ALTER TABLE [dbo].[tblTestComponent]
DROP CONSTRAINT [FK_TestComponent_TestComponentType]
ALTER TABLE [dbo].[tblTestMaterial]
DROP CONSTRAINT [FK_TestMaterial_TestComponentType]
ALTER TABLE [dbo].[tblExaminerTestComponentResult]
DROP CONSTRAINT [FK_ExaminerTestComponentResult_TestComponentType]
ALTER TABLE [dbo].[tblTestSpecificationTestComponentType]
DROP CONSTRAINT [FK_TestSpecificationTestComponentType_TestComponentType]
ALTER TABLE [dbo].[tblTestComponentType]
DROP CONSTRAINT [FK_tblTestComponentType_TestComponentBaseTypeId_tblTestComponentBaseType_TestComponentBaseTypeId]
ALTER TABLE [dbo].[tblTestComponentType]
DROP CONSTRAINT [FK_TestComponentType_TestSpecification]
GO
DROP STATISTICS [dbo].[tblTestComponentType].[_WA_Sys_00000002_5812160E]
GO
DROP STATISTICS [dbo].[tblTestComponentType].[_WA_Sys_00000003_5812160E]
GO
DROP STATISTICS [dbo].[tblTestComponentType].[_WA_Sys_00000008_5812160E]
GO
DROP STATISTICS [dbo].[tblTestComponentType].[_WA_Sys_00000009_5812160E]
GO
DROP STATISTICS [dbo].[tblTestComponentType].[_WA_Sys_0000000A_5812160E]
GO
DROP STATISTICS [dbo].[tblTestComponentType].[_WA_Sys_RowVersion_5812160E]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tluTestComponentType]',
	@newname  = N'tmp_7107ad7018854bec8e7c99995610fbb8',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblTestComponentType_TestComponentBaseTypeId]',
	@newname  = N'tmp_d402f1d6945c48d1aebd28c7076b2b9b',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblTestComponentType]',
	@newname  = N'tmp_11ca8ebada364e138a80ebc038c437f8',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblTestComponentType] (
	[TestComponentTypeId] int IDENTITY(1, 1),
	[Name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	[TestComponentBaseTypeId] int NOT NULL DEFAULT ((1)),
	[Label] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TestSpecificationId] int NOT NULL,
	CONSTRAINT [PK_tluTestComponentType] PRIMARY KEY([TestComponentTypeId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblTestComponentType] ON
GO
INSERT INTO [dbo].[tblTestComponentType] (
	[TestComponentTypeId],
	[Name],
	[Description],
	[TestComponentBaseTypeId],
	[Label],
	[TestSpecificationId])
SELECT
	[TestComponentTypeId],
	[Name],
	[Description],
	[TestComponentBaseTypeId],
	[Label],
	[TestSpecificationId]
FROM [dbo].[tmp_11ca8ebada364e138a80ebc038c437f8]
GO
SET IDENTITY_INSERT [dbo].[tblTestComponentType] OFF
GO
DROP TABLE [dbo].[tmp_11ca8ebada364e138a80ebc038c437f8]
GO
ALTER TABLE [dbo].[tblRubricMarkingCompetency]
 ADD CONSTRAINT [FK_RubricMarkingCompetency_TestComponentType] FOREIGN KEY ([TestComponentTypeId])
		REFERENCES [dbo].[tblTestComponentType] ([TestComponentTypeId])
	
ALTER TABLE [dbo].[tblTestComponentTypeStandardMarkingScheme]
 ADD CONSTRAINT [FK_TestComponentTypeStandardMarkingScheme_TestComponentType] FOREIGN KEY ([TestComponentTypeId])
		REFERENCES [dbo].[tblTestComponentType] ([TestComponentTypeId])
	
ALTER TABLE [dbo].[tblTestComponent]
 ADD CONSTRAINT [FK_TestComponent_TestComponentType] FOREIGN KEY ([TypeId])
		REFERENCES [dbo].[tblTestComponentType] ([TestComponentTypeId])
	
ALTER TABLE [dbo].[tblTestMaterial]
 ADD CONSTRAINT [FK_TestMaterial_TestComponentType] FOREIGN KEY ([TestComponentTypeId])
		REFERENCES [dbo].[tblTestComponentType] ([TestComponentTypeId])
	
ALTER TABLE [dbo].[tblExaminerTestComponentResult]
 ADD CONSTRAINT [FK_ExaminerTestComponentResult_TestComponentType] FOREIGN KEY ([TypeID])
		REFERENCES [dbo].[tblTestComponentType] ([TestComponentTypeId])
	
ALTER TABLE [dbo].[tblTestSpecificationTestComponentType]
 ADD CONSTRAINT [FK_TestSpecificationTestComponentType_TestComponentType] FOREIGN KEY ([TestComponentTypeId])
		REFERENCES [dbo].[tblTestComponentType] ([TestComponentTypeId])
	
ALTER TABLE [dbo].[tblTestComponentType]
 ADD CONSTRAINT [FK_tblTestComponentType_TestComponentBaseTypeId_tblTestComponentBaseType_TestComponentBaseTypeId] FOREIGN KEY ([TestComponentBaseTypeId])
		REFERENCES [dbo].[tblTestComponentBaseType] ([TestComponentBaseTypeId])
	
ALTER TABLE [dbo].[tblTestComponentType]
 ADD CONSTRAINT [FK_TestComponentType_TestSpecification] FOREIGN KEY ([TestSpecificationId])
		REFERENCES [dbo].[tblTestSpecification] ([TestSpecificationId])
	
