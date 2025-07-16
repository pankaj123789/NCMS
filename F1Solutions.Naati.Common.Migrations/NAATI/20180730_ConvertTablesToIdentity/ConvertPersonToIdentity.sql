
ALTER TABLE [dbo].[tblPersonImage]
DROP CONSTRAINT [FK_tblPersonImage_tblPerson]
ALTER TABLE [dbo].[tblCredentialApplication]
DROP CONSTRAINT [FK_CredentialApplication_Person]
ALTER TABLE [dbo].[tblProfessionalDevelopmentActivity]
DROP CONSTRAINT [FK_ProfessionalDevelopmentActivity_Person]
ALTER TABLE [dbo].[tblPDInclusion]
DROP CONSTRAINT [FK_tblPDInclusion_tblPerson]
ALTER TABLE [dbo].[tblTestResultAttachment]
DROP CONSTRAINT [FK_tblTestResultAttachment_tblPerson_01]
ALTER TABLE [dbo].[tblTestResultAttachment]
DROP CONSTRAINT [FK_tblTestResultAttachment_tblPerson_02]
ALTER TABLE [dbo].[tblCertificationPeriod]
DROP CONSTRAINT [FK_CertificationPeriod_Person]
ALTER TABLE [dbo].[tblEmailBatchRecipient]
DROP CONSTRAINT [FK_tblEmailBatchRecipient_PersonId_tblPerson_PersonId]
ALTER TABLE [dbo].[tblPersonName]
DROP CONSTRAINT [FK_tblPersonName_tblPerson]
ALTER TABLE [dbo].[tblRevalidation]
DROP CONSTRAINT [FK_tblRevalidation_tblPerson]
ALTER TABLE [dbo].[tblWorkshopAttendance]
DROP CONSTRAINT [FK_tblWorkshopAttendance_tblPerson]
ALTER TABLE [dbo].[tblInstitutionContactPerson]
DROP CONSTRAINT [FK_tblInstitutionContactPerson_tblPerson]
ALTER TABLE [dbo].[tblPersonExpertise]
DROP CONSTRAINT [FK_tblPersonExpertise_tblPerson]
ALTER TABLE [dbo].[tblEventFacilitator]
DROP CONSTRAINT [FK_tblEventSupervisor_tblPerson]
ALTER TABLE [dbo].[tblExpressionOfInterest]
DROP CONSTRAINT [FK_tblExpressionOfInterest_tblPerson]
ALTER TABLE [dbo].[tblPanelMembership]
DROP CONSTRAINT [FK_tblPanelMembership_tblPerson]
ALTER TABLE [dbo].[tblCourseCoordinator]
DROP CONSTRAINT [FK_tblCourseCoordinator_tblPerson]
ALTER TABLE [dbo].[tblLanguageExperience]
DROP CONSTRAINT [FK_tblLanguageExperience_tblPerson]
ALTER TABLE [dbo].[tblPersonArchiveHistory]
DROP CONSTRAINT [FK_tblPersonArchiveHistory_tblPerson]
ALTER TABLE [dbo].[tblCredentialRecordLog]
DROP CONSTRAINT [FK_CredentialRecordLog_Person]
ALTER TABLE [dbo].[tblStoredFile]
DROP CONSTRAINT [FK_tblStoredFile_tblPerson]
ALTER TABLE [dbo].[tblSmsSend]
DROP CONSTRAINT [FK_tblSmsSend_tblPerson]
ALTER TABLE [dbo].[tblPerson]
DROP CONSTRAINT [FK_tblPerson_tblCountry]
ALTER TABLE [dbo].[tblPerson]
DROP CONSTRAINT [FK_tblPerson_tblInstitution]
ALTER TABLE [dbo].[tblPerson]
DROP CONSTRAINT [FK_tblPerson_tluEducationLevel]
ALTER TABLE [dbo].[tblPerson]
DROP CONSTRAINT [FK_tblPerson_tblEntity]
GO
DROP STATISTICS [dbo].[tblPerson].[_dta_stat_597577167_2_7_1]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000008_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000009_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_0000000E_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000011_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000012_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000013_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000014_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000015_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000016_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_00000017_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_0000001A_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_0000001B_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_0000001C_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_BirthCountryId_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_BirthDate_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_Deceased_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_DoNotInviteToDirectory_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_DoNotSendCorrespondence_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_EnteredDate_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_EntityId_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_ExpertiseFreeText_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_Gender_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_RowVersion_239E4DCF]
GO
DROP STATISTICS [dbo].[tblPerson].[_WA_Sys_SponsorInstitutionId_239E4DCF]
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[PK_tblPerson]',
	@newname  = N'tmp_fce8cb94fbaa4ddaa3c310115962cc2c',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[U_tblPerson_PractitionerNumber]',
	@newname  = N'tmp_98d6d98d91914ab1980afb0df815a6fb',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[DF_tblPerson_PersonId]',
	@newname  = N'tmp_d3820ba59412482b9c84ab349ec5351d',
	@objtype  = 'OBJECT'
GO
EXECUTE [sp_rename]
	@objname  = N'[dbo].[tblPerson]',
	@newname  = N'tmp_c40a08a2c8b84d8da440a1e8d818420b',
	@objtype  = 'OBJECT'
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[tblPerson] (
	[PersonId] int IDENTITY(1, 1),
	[EntityId] int NOT NULL,
	[Gender] char(1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[BirthDate] datetime NULL,
	[BirthCountryId] int NULL,
	[SponsorInstitutionId] int NULL,
	[Deceased] bit NOT NULL,
	[HighestEducationLevelId] int NULL,
	[ReleaseDetails] bit NOT NULL,
	[DoNotInviteToDirectory] bit NOT NULL,
	[EnteredDate] datetime NOT NULL,
	[ExpertiseFreeText] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[NameOnAccreditationProduct] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RowVersion] timestamp NULL,
	[DoNotSendCorrespondence] bit NOT NULL DEFAULT (0),
	[ScanRequired] bit NOT NULL DEFAULT (0),
	[IsEportalActive] bit NULL,
	[PersonalDetailsLastUpdatedOnEportal] datetime NULL,
	[WebAccountCreateDate] datetime NULL,
	[AllowVerifyOnline] bit NOT NULL DEFAULT ((1)),
	[ShowPhotoOnline] bit NOT NULL DEFAULT ((1)),
	[RevalidationScheme] bit NULL,
	[ExaminerSecurityCode] varchar(6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExaminerTrackingCategory] varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PractitionerNumber] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EthicalCompetency] bit NOT NULL DEFAULT ((0)),
	[InterculturalCompetency] bit NOT NULL DEFAULT ((0)),
	CONSTRAINT [PK_tblPerson] PRIMARY KEY([PersonId]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY],
	CONSTRAINT [U_tblPerson_PractitionerNumber] UNIQUE([PersonId], [PractitionerNumber]) WITH (FILLFACTOR=90,
		DATA_COMPRESSION = NONE) ON [PRIMARY]
)
CREATE INDEX [_dta_index_tblPerson_7_597577167__K1_2]
 ON [dbo].[tblPerson] ([PersonId])
INCLUDE ([EntityId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [_dta_index_tblPerson_7_597577167__K1_K2]
 ON [dbo].[tblPerson] ([PersonId], [EntityId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [_dta_index_tblPerson_9_597577167__K7_K1_K2]
 ON [dbo].[tblPerson] ([Deceased], [PersonId], [EntityId])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
CREATE INDEX [IX_EntityId_Person_BirthDate]
 ON [dbo].[tblPerson] ([EntityId])
INCLUDE ([PersonId], [BirthDate])
WITH (FILLFACTOR=100,
	DATA_COMPRESSION = NONE)
ON [PRIMARY]
ALTER INDEX [U_tblPerson_PractitionerNumber]
ON [dbo].[tblPerson]
DISABLE
ALTER INDEX [_dta_index_tblPerson_7_597577167__K1_2]
ON [dbo].[tblPerson]
DISABLE
ALTER INDEX [_dta_index_tblPerson_7_597577167__K1_K2]
ON [dbo].[tblPerson]
DISABLE
ALTER INDEX [_dta_index_tblPerson_9_597577167__K7_K1_K2]
ON [dbo].[tblPerson]
DISABLE
ALTER INDEX [IX_EntityId_Person_BirthDate]
ON [dbo].[tblPerson]
DISABLE
SET IDENTITY_INSERT [dbo].[tblPerson] ON
GO
INSERT INTO [dbo].[tblPerson] (
	[PersonId],
	[EntityId],
	[Gender],
	[BirthDate],
	[BirthCountryId],
	[SponsorInstitutionId],
	[Deceased],
	[HighestEducationLevelId],
	[ReleaseDetails],
	[DoNotInviteToDirectory],
	[EnteredDate],
	[ExpertiseFreeText],
	[NameOnAccreditationProduct],
	[DoNotSendCorrespondence],
	[ScanRequired],
	[IsEportalActive],
	[PersonalDetailsLastUpdatedOnEportal],
	[WebAccountCreateDate],
	[AllowVerifyOnline],
	[ShowPhotoOnline],
	[RevalidationScheme],
	[ExaminerSecurityCode],
	[ExaminerTrackingCategory],
	[PractitionerNumber],
	[EthicalCompetency],
	[InterculturalCompetency])
SELECT
	[PersonId],
	[EntityId],
	[Gender],
	[BirthDate],
	[BirthCountryId],
	[SponsorInstitutionId],
	[Deceased],
	[HighestEducationLevelId],
	[ReleaseDetails],
	[DoNotInviteToDirectory],
	[EnteredDate],
	[ExpertiseFreeText],
	[NameOnAccreditationProduct],
	[DoNotSendCorrespondence],
	[ScanRequired],
	[IsEportalActive],
	[PersonalDetailsLastUpdatedOnEportal],
	[WebAccountCreateDate],
	[AllowVerifyOnline],
	[ShowPhotoOnline],
	[RevalidationScheme],
	[ExaminerSecurityCode],
	[ExaminerTrackingCategory],
	[PractitionerNumber],
	[EthicalCompetency],
	[InterculturalCompetency]
FROM [dbo].[tmp_c40a08a2c8b84d8da440a1e8d818420b]
GO
SET IDENTITY_INSERT [dbo].[tblPerson] OFF
ALTER INDEX ALL
ON [dbo].[tblPerson]
REBUILD
GO
DROP TABLE [dbo].[tmp_c40a08a2c8b84d8da440a1e8d818420b]
GO
ALTER TABLE [dbo].[tblPersonImage]
 ADD CONSTRAINT [FK_tblPersonImage_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblCredentialApplication]
 ADD CONSTRAINT [FK_CredentialApplication_Person] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblProfessionalDevelopmentActivity]
 ADD CONSTRAINT [FK_ProfessionalDevelopmentActivity_Person] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPDInclusion]
 ADD CONSTRAINT [FK_tblPDInclusion_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblTestResultAttachment]
 ADD CONSTRAINT [FK_tblTestResultAttachment_tblPerson_01] FOREIGN KEY ([UploadedByPersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblTestResultAttachment]
 ADD CONSTRAINT [FK_tblTestResultAttachment_tblPerson_02] FOREIGN KEY ([ModifiedByPersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblCertificationPeriod]
 ADD CONSTRAINT [FK_CertificationPeriod_Person] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblEmailBatchRecipient]
 ADD CONSTRAINT [FK_tblEmailBatchRecipient_PersonId_tblPerson_PersonId] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPersonName]
 ADD CONSTRAINT [FK_tblPersonName_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblRevalidation]
 ADD CONSTRAINT [FK_tblRevalidation_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblWorkshopAttendance]
 ADD CONSTRAINT [FK_tblWorkshopAttendance_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblInstitutionContactPerson]
 ADD CONSTRAINT [FK_tblInstitutionContactPerson_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPersonExpertise]
 ADD CONSTRAINT [FK_tblPersonExpertise_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblEventFacilitator]
 ADD CONSTRAINT [FK_tblEventSupervisor_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblExpressionOfInterest]
 ADD CONSTRAINT [FK_tblExpressionOfInterest_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPanelMembership]
 ADD CONSTRAINT [FK_tblPanelMembership_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblCourseCoordinator]
 ADD CONSTRAINT [FK_tblCourseCoordinator_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblLanguageExperience]
 ADD CONSTRAINT [FK_tblLanguageExperience_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPersonArchiveHistory]
 ADD CONSTRAINT [FK_tblPersonArchiveHistory_tblPerson] FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblCredentialRecordLog]
 ADD CONSTRAINT [FK_CredentialRecordLog_Person] FOREIGN KEY ([PersonID])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblStoredFile]
 ADD CONSTRAINT [FK_tblStoredFile_tblPerson] FOREIGN KEY ([UploadedByPersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblSmsSend]
 ADD CONSTRAINT [FK_tblSmsSend_tblPerson] FOREIGN KEY ([ReceivePersonId])
		REFERENCES [dbo].[tblPerson] ([PersonId])
	
ALTER TABLE [dbo].[tblPerson]
 ADD CONSTRAINT [FK_tblPerson_tblCountry] FOREIGN KEY ([BirthCountryId])
		REFERENCES [dbo].[tblCountry] ([CountryId])
	
ALTER TABLE [dbo].[tblPerson]
 ADD CONSTRAINT [FK_tblPerson_tblInstitution] FOREIGN KEY ([SponsorInstitutionId])
		REFERENCES [dbo].[tblInstitution] ([InstitutionId])
	
ALTER TABLE [dbo].[tblPerson]
 ADD CONSTRAINT [FK_tblPerson_tluEducationLevel] FOREIGN KEY ([HighestEducationLevelId])
		REFERENCES [dbo].[tluEducationLevel] ([EducationLevelId])
	
ALTER TABLE [dbo].[tblPerson]
 ADD CONSTRAINT [FK_tblPerson_tblEntity] FOREIGN KEY ([EntityId])
		REFERENCES [dbo].[tblEntity] ([EntityId])
	
