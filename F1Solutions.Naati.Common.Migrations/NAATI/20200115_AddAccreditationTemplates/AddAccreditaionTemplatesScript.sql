Declare @ENTITYID Int

SET IDENTITY_INSERT [dbo].[tblCredentialCategory] ON 

insert into tblCredentialCategory (CredentialCategoryId, [Name], DisplayName, WorkPracticePoints, WorkPracticeUnits, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (11, 'Accreditation', 'Accreditation', null, null, 0, GETDATE(), 40)

SET IDENTITY_INSERT [dbo].[tblCredentialCategory] OFF 

SET IDENTITY_INSERT [dbo].[tblCredentialType] ON 

insert into tblCredentialType (CredentialTypeId,CredentialCategoryId,InternalName,ExternalName,DisplayOrder,Simultaneous,SkillTypeId,Certification,DefaultExpiry,
ModifiedByNaati,ModifiedDate,ModifiedUser,AllowBackdating,[Level],TestSessionBookingAvailabilityWeeks,TestSessionBookingClosedWeeks,TestSessionBookingRejectWeeks)
values (33, 11, 'Para-professional Interpreter', 'Para-professional Interpreter', 33, 0, 33, 0, 3, 0, GETDATE(), 40, 0, 0, 52, 5, 5),
       (34, 11, 'Professional Interpreter', 'Professional Interpreter', 34, 0, 34, 0, 3, 0, GETDATE(), 40, 0, 0, 52, 5, 5),
       (35, 11, 'Professional Translator', 'Professional Translator', 35, 0, 35, 0, 3, 0, GETDATE(), 40, 0, 0, 52, 5, 5)

SET IDENTITY_INSERT [dbo].[tblCredentialType] OFF 

insert into tblStoredFile (ExternalFileId, FileSize, [FileName], UploadedByPersonId, UploadedByUserId, UploadedDateTime, DocumentTypeId)
values (N'CertificateTemplate\AccreditationCertificateTemplate.docx', 241664, 'AccreditationCertificateTemplate.docx', null, 40, GetDate(), 19)
SELECT @ENTITYID= SCOPE_IDENTITY();
insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (35, @ENTITYID, '[[Credential Type]] [[Skill]] [[Document Number]] Certificate', 0, GETDATE(), 40),
	   (34, @ENTITYID, '[[Credential Type]] [[Skill]] [[Document Number]] Certificate', 0, GETDATE(), 40),
	   (33, @ENTITYID, '[[Credential Type]] [[Skill]] [[Document Number]] Certificate', 0, GETDATE(), 40)


insert into tblStoredFile (ExternalFileId, FileSize, [FileName], UploadedByPersonId, UploadedByUserId, UploadedDateTime, DocumentTypeId)
values (N'CredentialLetterTemplate\Accreditation - Professional Translator Letter.doc', 110592, 'Accreditation - Professional Translator Letter.doc', null, 40, GetDate(), 22)
SELECT @ENTITYID= SCOPE_IDENTITY();
insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (35, @ENTITYID, '[[Credential Type]] [[Skill]] [[Document Number]] Letter', 0, GETDATE(), 40)

insert into tblStoredFile (ExternalFileId, FileSize, [FileName], UploadedByPersonId, UploadedByUserId, UploadedDateTime, DocumentTypeId)
values (N'CredentialLetterTemplate\Accreditation - Professional Interpreter Letter.doc', 110592, 'Accreditation - Professional Interpreter Letter.doc', null, 40, GetDate(), 22)
SELECT @ENTITYID= SCOPE_IDENTITY();
insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (34, @ENTITYID, '[[Credential Type]] [[Skill]] [[Document Number]] Letter', 0, GETDATE(), 40)

insert into tblStoredFile (ExternalFileId, FileSize, [FileName], UploadedByPersonId, UploadedByUserId, UploadedDateTime, DocumentTypeId)
values (N'CredentialLetterTemplate\Accreditation - Paraprofessional Interpreter Letter.doc', 110592, 'Accreditation - Paraprofessional Interpreter Letter.doc', null, 40, GetDate(), 22)
SELECT @ENTITYID= SCOPE_IDENTITY();
insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (33, @ENTITYID, '[[Credential Type]] [[Skill]] [[Document Number]] Letter', 0, GETDATE(), 40)