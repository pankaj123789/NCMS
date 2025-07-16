SET IDENTITY_INSERT [dbo].[tblCredentialType] ON 
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (19,2,'Recognised Practising Interpreter - Auslan','Recognised Practising Interpreter',100,0,19,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (20,2,'Certified Provisional Interpreter - Auslan','Certified Provisional Interpreter',200,0,20,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (21,2,'Certified Interpreter - Auslan','Certified Interpreter',300,1,21,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (22,2,'Certified Specialist Interpreter - Health - Auslan','Certified Specialist Interpreter - Health',400,1,23,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (23,2,'Certified Specialist Interpreter - Legal - Auslan','Certified Specialist Interpreter - Legal',400,1,24,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (24,2,'Certified Conference Interpreter - Auslan','Certified Conference Interpreter',400,1,22,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (25,2,'Recognised Practising Interpreter - Indigenous','Recognised Practising Interpreter',100,0,25,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (26,2,'Certified Provisional Interpreter - Indigenous','Certified Provisional Interpreter',200,0,26,1)
INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[UpgradePath],[Simultaneous],[SkillTypeId],[Certification]) VALUES  (27,2,'Certified Interpreter - Indigenous','Certified Interpreter',400,0,27,1)

SET IDENTITY_INSERT [dbo].[tblCredentialType] OFF 

UPDATE TBLCREDENTIALTYPE SET UpgradePath = 300 WHERE CredentialTypeId = 2
UPDATE TBLCREDENTIALTYPE SET UpgradePath = 400 WHERE CredentialTypeId IN (3,4)

DECLARE @translatorLetterId INT 
DECLARE @interpreterLetterId INT 
DECLARE @cpInterpreterLetterId int
DECLARE @certificateId INT 
DECLARE @letterNameTemplate VARCHAR(100) = N'[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Letter'
DECLARE @certificateNameTemplate VARCHAR(100) = N'[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Certificate'

insert into tblStoredFile (
   ExternalFileId
  ,FileSize
  ,[FileName]
  ,UploadedByPersonId
  ,UploadedByUserId
  ,UploadedDateTime
  ,DocumentTypeId
) VALUES 
   (N'CredentialLetterTemplate\CertifiedProvisionalInterpreterCredentialLetterTemplate.docx',138332,'CertifiedProvisionalInterpreterCredentialLetterTemplate.docx',NULL,40,GETDATE(),22)

SELECT @translatorLetterId = StoredFileId 
FROM   tblStoredFile 
WHERE  [FileName] = 'TranslatorCredentialLetterTemplate.docx' 

SELECT @interpreterLetterId = StoredFileId 
FROM   tblStoredFile 
WHERE  [FileName] = 'InterpreterCredentialLetterTemplate.docx' 

SELECT @certificateId = StoredFileId 
FROM   tblStoredFile 
WHERE  [FileName] = 'CredentialCertificateTemplate.docx' 

SELECT @cpInterpreterLetterId = StoredFileId 
FROM   tblStoredFile 
WHERE  [FileName] = 'CertifiedProvisionalInterpreterCredentialLetterTemplate.docx' 

update tblCredentialTypeTemplate SET StoredFileId = @cpInterpreterLetterId WHERE CredentialTypeTemplateId = 11
update tblCredentialTypeTemplate SET StoredFileId = @cpInterpreterLetterId WHERE CredentialTypeTemplateId = 29

INSERT INTO tblCredentialTypeTemplate 
            (CredentialTypeId, StoredFileId, DocumentNameTemplate) 
VALUES      (19, @interpreterLetterId, @letterNameTemplate),  
            (19, @certificateId, @certificateNameTemplate),  
            (20, @translatorLetterId, @letterNameTemplate),  
            (20, @certificateId, @certificateNameTemplate),  
            (21, @interpreterLetterId, @letterNameTemplate),  
            (21, @certificateId, @certificateNameTemplate),  
            (22, @interpreterLetterId, @letterNameTemplate),  
            (22, @certificateId, @certificateNameTemplate),  
            (23, @interpreterLetterId, @letterNameTemplate),  
            (23, @certificateId, @certificateNameTemplate),  
            (24, @interpreterLetterId, @letterNameTemplate),  
            (24, @certificateId, @certificateNameTemplate),  
            (25, @interpreterLetterId, @letterNameTemplate),  
            (25, @certificateId, @certificateNameTemplate),  
            (26, @cpInterpreterLetterId, @letterNameTemplate),  
            (26, @certificateId, @certificateNameTemplate),  
            (27, @interpreterLetterId, @letterNameTemplate),  
            (27, @certificateId, @certificateNameTemplate)


