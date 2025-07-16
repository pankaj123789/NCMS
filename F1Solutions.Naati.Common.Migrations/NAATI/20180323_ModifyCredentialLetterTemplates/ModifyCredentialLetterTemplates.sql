DECLARE @ID TABLE (ID INT) 

IF NOT EXISTS (SELECT * 
               FROM   tblStoredFile 
               WHERE  [FileName] = 'RPI-CPI-RPT-IND-CredentialLetterTemplate.docx') 
BEGIN 
    UPDATE tblStoredFile 
    SET    
      ExternalFileId = 'CredentialLetterTemplate\RPI-CPI-RPT-IND-CredentialLetterTemplate.docx',
      FileSize = 141199,
      [FileName] = 'RPI-CPI-RPT-IND-CredentialLetterTemplate.docx', 
      UploadedDateTime = Getdate() 
    OUTPUT INSERTED.StoredFileId INTO @ID 
    WHERE  [FileName] = 'CertifiedProvisionalInterpreterCredentialLetterTemplate.docx' 

    UPDATE tblCredentialTypeTemplate 
    SET    StoredFileId = ID
    FROM @ID
    WHERE  CredentialTypeTemplateId IN ( 1, 9, 11, 39, 41, 43 )  
END 

IF NOT EXISTS (SELECT * 
               FROM   tblStoredFile 
               WHERE  [FileName] = 'Auslan-CredentialLetterTemplate.docx') 
BEGIN 
    DELETE @ID

    INSERT INTO tblStoredFile 
                (ExternalFileId,FileSize,[FileName],UploadedByPersonId,UploadedByUserId,UploadedDateTime,DocumentTypeId) 
    OUTPUT      INSERTED.StoredFileId INTO @ID 
    VALUES      (N'CredentialLetterTemplate\Auslan-CredentialLetterTemplate.docx',141213,'Auslan-CredentialLetterTemplate.docx',NULL,40,Getdate(),22) 

    UPDATE tblCredentialTypeTemplate 
    SET    StoredFileId = ID 
    FROM   @ID 
    WHERE  CredentialTypeTemplateId IN ( 23, 25, 27, 29, 31, 33, 35, 37 ) 
END 

IF NOT EXISTS (SELECT * 
               FROM   tblStoredFile 
               WHERE  [FileName] = 'CCL-CredentialLetterTemplate.docx') 
BEGIN 
    DELETE @ID

    INSERT INTO tblStoredFile 
                (ExternalFileId,FileSize,[FileName],UploadedByPersonId,UploadedByUserId,UploadedDateTime,DocumentTypeId) 
    OUTPUT      INSERTED.StoredFileId INTO @ID 
    VALUES      (N'CredentialLetterTemplate\CCL-CredentialLetterTemplate.docx',140282,'CCL-CredentialLetterTemplate.docx',NULL,40,Getdate(),22) 

	INSERT INTO tblCredentialTypeTemplate 
				(CredentialTypeId, StoredFileId, DocumentNameTemplate) 
	VALUES		(14, (SELECT ID from @ID), '[[Credential Type]] [[Skill]] [[Document Number]] Letter')
END 
