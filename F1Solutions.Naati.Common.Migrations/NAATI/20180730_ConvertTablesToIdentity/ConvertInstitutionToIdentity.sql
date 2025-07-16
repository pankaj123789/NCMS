
ALTER TABLE [dbo].[tblCourseLocation]
DROP CONSTRAINT [FK_tblCourseLocation_tblInstitution]
ALTER TABLE [dbo].[tblProject]
DROP CONSTRAINT [FK_tblProject_tblInstitution]
ALTER TABLE [dbo].[tblContactPerson]
DROP CONSTRAINT [FK_ContactPerson_Institution]
ALTER TABLE [dbo].[tblCredentialApplication]
DROP CONSTRAINT [FK_CredentialApplication_Institution]
ALTER TABLE [dbo].[tblApplication]
DROP CONSTRAINT [FK_tblApplication_SponsorInstitutionId_tblInstitution_InstitutionId]
ALTER TABLE [dbo].[tblInstitutionName]
DROP CONSTRAINT [FK_tblInstitutionName_tblInstitution]
ALTER TABLE [dbo].[tblInstitutionContactPerson]
DROP CONSTRAINT [FK_tblInstitutionContactPerson_tblInstitution]
ALTER TABLE [dbo].[tblOffice]
DROP CONSTRAINT [FK_tblOffice_tblInstitution]
ALTER TABLE [dbo].[tblApiAccess]
DROP CONSTRAINT [FK_ApiAccess_Institution]
ALTER TABLE [dbo].[tblPerson]
DROP CONSTRAINT [FK_tblPerson_tblInstitution]
ALTER TABLE [dbo].[tblCourse]
DROP CONSTRAINT [FK_tblCourse_tblInstitution]
ALTER TABLE [dbo].[tblInstitution]
DROP CONSTRAINT [FK_tblInstitution_tblEntity]
GO
DROP STATISTICS [dbo].[tblInstitution].[_WA_Sys_0000000C_07F6335A]
GO
DROP STATISTICS [dbo].[tblInstitution].[_WA_Sys_EntityId_07F6335A]
GO
DROP STATISTICS [dbo].[tblInstitution].[_WA_Sys_RowVersion_07F6335A]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblInstitution]',
	@newname  = N'tmp_c6997815eff247d49c9a5979a4e2f5f5',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblInstitution]',
	@newname  = N'tmp_1bc5e541e9dc442b86faf975b15ce2ac',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblInstitution] (
	[InstitutionId] int IDENTITY(1, 1),
	[EntityId] int NOT NULL,
	[RowVersion] timestamp NULL,
	[IsManageCoursesAndQualification] bit NOT NULL DEFAULT ((1)),
	[IsGoThroughApprovalProcess] bit NOT NULL DEFAULT ((1)),
	[IsUniversity] bit NOT NULL DEFAULT ((0)),
	[IsVetRto] bit NOT NULL DEFAULT ((0)),
	[RtoNumber] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	[IsOfferCourseToStudentVisa] bit NOT NULL DEFAULT ((0)),
	[CricosProviderCode] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT (''),
	[TrustedPayer] bit NOT NULL DEFAULT ((0)),
	CONSTRAINT [PK_tblInstitution] PRIMARY KEY([InstitutionId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [_dta_index_tblInstitution_7_133575514__K2_1]
 ON [dbo].[tblInstitution] ([EntityId])
INCLUDE ([InstitutionId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [_dta_index_tblInstitution_7_133575514__K2_1]
ON [dbo].[tblInstitution]
DISABLE
SET IDENTITY_INSERT [dbo].[tblInstitution] ON
GO
INSERT INTO [dbo].[tblInstitution] (
	[InstitutionId],
	[EntityId],
	[IsManageCoursesAndQualification],
	[IsGoThroughApprovalProcess],
	[IsUniversity],
	[IsVetRto],
	[RtoNumber],
	[IsOfferCourseToStudentVisa],
	[CricosProviderCode],
	[TrustedPayer])
SELECT
	[InstitutionId],
	[EntityId],
	[IsManageCoursesAndQualification],
	[IsGoThroughApprovalProcess],
	[IsUniversity],
	[IsVetRto],
	[RtoNumber],
	[IsOfferCourseToStudentVisa],
	[CricosProviderCode],
	[TrustedPayer]
FROM [dbo].[tmp_1bc5e541e9dc442b86faf975b15ce2ac]
GO
SET IDENTITY_INSERT [dbo].[tblInstitution] OFF
ALTER INDEX ALL
ON [dbo].[tblInstitution]
REBUILD
GO
DROP TABLE [dbo].[tmp_1bc5e541e9dc442b86faf975b15ce2ac]
GO
ALTER TABLE [dbo].[tblCourseLocation]
 ADD CONSTRAINT [FK_tblCourseLocation_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblProject]
 ADD CONSTRAINT [FK_tblProject_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblContactPerson]
 ADD CONSTRAINT [FK_ContactPerson_Institution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblCredentialApplication]
 ADD CONSTRAINT [FK_CredentialApplication_Institution] FOREIGN KEY ([SponsorInstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblApplication]
 ADD CONSTRAINT [FK_tblApplication_SponsorInstitutionId_tblInstitution_InstitutionId] FOREIGN KEY ([SponsorInstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblInstitutionName]
 ADD CONSTRAINT [FK_tblInstitutionName_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblInstitutionContactPerson]
 ADD CONSTRAINT [FK_tblInstitutionContactPerson_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblOffice]
 ADD CONSTRAINT [FK_tblOffice_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblApiAccess]
 ADD CONSTRAINT [FK_ApiAccess_Institution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblPerson]
 ADD CONSTRAINT [FK_tblPerson_tblInstitution] FOREIGN KEY ([SponsorInstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblCourse]
 ADD CONSTRAINT [FK_tblCourse_tblInstitution] FOREIGN KEY ([InstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblInstitution]
 ADD CONSTRAINT [FK_tblInstitution_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
