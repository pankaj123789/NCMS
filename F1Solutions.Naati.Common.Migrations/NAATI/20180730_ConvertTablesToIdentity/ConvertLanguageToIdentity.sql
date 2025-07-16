
ALTER TABLE [dbo].[tblWorkshopEvent]
DROP CONSTRAINT [FK_tblWorkshopEvent_tblLanguage]
ALTER TABLE [dbo].[tblPanel]
DROP CONSTRAINT [FK_tblPanel_tblLanguage]
ALTER TABLE [dbo].[tblAlternateLanguageName]
DROP CONSTRAINT [FK_AlternateLanguageName_Language]
ALTER TABLE [dbo].[tblWorkshopAttendanceLanguage]
DROP CONSTRAINT [FK_tblWorkshopAttendanceLanguage_tblLanguage]
ALTER TABLE [dbo].[tblTestMaterial]
DROP CONSTRAINT [FK_TestMaterial_Language]
ALTER TABLE [dbo].[tblSkill]
DROP CONSTRAINT [FK_Skill_Language1]
ALTER TABLE [dbo].[tblSkill]
DROP CONSTRAINT [FK_Skill_Language2]
ALTER TABLE [dbo].[tblTestAvailability]
DROP CONSTRAINT [FK_tblTestAvailability_tblLanguage]
ALTER TABLE [dbo].[tblExpressionOfInterest]
DROP CONSTRAINT [FK_tblExpressionOfInterest_tblLanguage]
ALTER TABLE [dbo].[tblCourseApproval]
DROP CONSTRAINT [FK_tblCourseApproval_tblLanguage]
ALTER TABLE [dbo].[tblCourseApproval]
DROP CONSTRAINT [FK_tblCourseApprovalLanguage_tblLanguage]
ALTER TABLE [dbo].[tblLanguageExperience]
DROP CONSTRAINT [FK_tblLanguageExperience_tblLanguage2]
ALTER TABLE [dbo].[tblJob]
DROP CONSTRAINT [FK_Job_Language]
ALTER TABLE [dbo].[tblEoiLanguage]
DROP CONSTRAINT [FK_tblEoiLanguage_tblLanguage]
ALTER TABLE [dbo].[tblLanguage]
DROP CONSTRAINT [FK_Language_LanguageGroup]
GO
DROP STATISTICS [dbo].[tblLanguage].[_WA_Sys_00000007_0EA330E9]
GO
DROP STATISTICS [dbo].[tblLanguage].[_WA_Sys_Code_0EA330E9]
GO
DROP STATISTICS [dbo].[tblLanguage].[_WA_Sys_Name_0EA330E9]
GO
DROP STATISTICS [dbo].[tblLanguage].[_WA_Sys_RowVersion_0EA330E9]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblLanguage]',
	@newname  = N'tmp_ea87e9ddffa24de09a2f92141b25d95e',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblLanguage]',
	@newname  = N'tmp_8e9ba04b3e1b422c8fd241a4e49cd96b',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblLanguage] (
	[LanguageId] int IDENTITY(1, 1),
	[Name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Code] varchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	[LanguageGroupId] int NULL,
	CONSTRAINT [PK_tblLanguage] PRIMARY KEY([LanguageId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
SET IDENTITY_INSERT [dbo].[tblLanguage] ON
GO
INSERT INTO [dbo].[tblLanguage] (
	[LanguageId],
	[Name],
	[Code],
	[LanguageGroupId])
SELECT
	[LanguageId],
	[Name],
	[Code],
	[LanguageGroupId]
FROM [dbo].[tmp_8e9ba04b3e1b422c8fd241a4e49cd96b]
GO
SET IDENTITY_INSERT [dbo].[tblLanguage] OFF
GO
DROP TABLE [dbo].[tmp_8e9ba04b3e1b422c8fd241a4e49cd96b]
GO
ALTER TABLE [dbo].[tblWorkshopEvent]
 ADD CONSTRAINT [FK_tblWorkshopEvent_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblPanel]
 ADD CONSTRAINT [FK_tblPanel_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblAlternateLanguageName]
 ADD CONSTRAINT [FK_AlternateLanguageName_Language] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblWorkshopAttendanceLanguage]
 ADD CONSTRAINT [FK_tblWorkshopAttendanceLanguage_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblTestMaterial]
 ADD CONSTRAINT [FK_TestMaterial_Language] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblSkill]
 ADD CONSTRAINT [FK_Skill_Language1] FOREIGN KEY ([Language1Id])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblSkill]
 ADD CONSTRAINT [FK_Skill_Language2] FOREIGN KEY ([Language2Id])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblTestAvailability]
 ADD CONSTRAINT [FK_tblTestAvailability_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblExpressionOfInterest]
 ADD CONSTRAINT [FK_tblExpressionOfInterest_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblCourseApproval]
 ADD CONSTRAINT [FK_tblCourseApproval_tblLanguage] FOREIGN KEY ([ToLanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblCourseApproval]
 ADD CONSTRAINT [FK_tblCourseApprovalLanguage_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblLanguageExperience]
 ADD CONSTRAINT [FK_tblLanguageExperience_tblLanguage2] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblJob]
 ADD CONSTRAINT [FK_Job_Language] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblEoiLanguage]
 ADD CONSTRAINT [FK_tblEoiLanguage_tblLanguage] FOREIGN KEY ([LanguageId])
		REFERENCES [dbo].[tblLanguage] ([LanguageId])
	
ALTER TABLE [dbo].[tblLanguage]
 ADD CONSTRAINT [FK_Language_LanguageGroup] FOREIGN KEY ([LanguageGroupId])
		REFERENCES [dbo].[tblLanguageGroup] ([LanguageGroupId])
	
