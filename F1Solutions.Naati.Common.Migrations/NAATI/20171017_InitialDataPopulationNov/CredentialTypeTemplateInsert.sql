insert into tblStoredFile (
   ExternalFileId
  ,FileSize
  ,[FileName]
  ,UploadedByPersonId
  ,UploadedByUserId
  ,UploadedDateTime
  ,DocumentTypeId
) VALUES 
   (N'CredentialLetterTemplate\TranslatorCredentialLetterTemplate.docx',138075,'TranslatorCredentialLetterTemplate.docx',NULL,40,GETDATE(),22),
   (N'CredentialLetterTemplate\InterpreterCredentialLetterTemplate.docx',141385,'InterpreterCredentialLetterTemplate.docx',NULL,40,GETDATE(),22),
   (N'CertificateTemplate\CredentialCertificateTemplate.docx',240891,'CredentialCertificateTemplate.docx',NULL,40,GETDATE(),19)

declare @translatorLetterId int
declare @interpreterLetterId int
declare @certificateId int
declare @letterNameTemplate varchar(100) = N'[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Letter'
declare @certificateNameTemplate varchar(100) = N'[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Certificate'

select @translatorLetterId = StoredFileId FROM tblStoredFile WHERE [FileName] = 'TranslatorCredentialLetterTemplate.docx'
select @interpreterLetterId = StoredFileId FROM tblStoredFile WHERE [FileName] = 'InterpreterCredentialLetterTemplate.docx'
select @certificateId = StoredFileId FROM tblStoredFile WHERE [FileName] = 'CredentialCertificateTemplate.docx'

insert into tblCredentialTypeTemplate (
   CredentialTypeId
  ,StoredFileId
  ,DocumentNameTemplate
) VALUES 
(1, @translatorLetterId, @letterNameTemplate),
(1, @certificateId, @certificateNameTemplate),
(2, @translatorLetterId, @letterNameTemplate),
(2, @certificateId, @certificateNameTemplate),
(3, @translatorLetterId, @letterNameTemplate),
(3, @certificateId, @certificateNameTemplate),
(4, @translatorLetterId, @letterNameTemplate),
(4, @certificateId, @certificateNameTemplate),
(5, @interpreterLetterId, @letterNameTemplate),
(5, @certificateId, @certificateNameTemplate),
(6, @interpreterLetterId, @letterNameTemplate),
(6, @certificateId, @certificateNameTemplate),
(7, @interpreterLetterId, @letterNameTemplate),
(7, @certificateId, @certificateNameTemplate),
(8, @interpreterLetterId, @letterNameTemplate),
(8, @certificateId, @certificateNameTemplate),
(9, @interpreterLetterId, @letterNameTemplate),
(9, @certificateId, @certificateNameTemplate),
(10, @interpreterLetterId, @letterNameTemplate),
(10, @certificateId, @certificateNameTemplate),
(11, @interpreterLetterId, @letterNameTemplate),
(11, @certificateId, @certificateNameTemplate),
(12, @interpreterLetterId, @letterNameTemplate),
(12, @certificateId, @certificateNameTemplate),
(13, @interpreterLetterId, @letterNameTemplate),
(13, @certificateId, @certificateNameTemplate)