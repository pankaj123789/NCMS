DECLARE @ID TABLE (ID INT) 

IF NOT EXISTS (SELECT * 
               FROM   tblStoredFile 
               WHERE  [FileName] = 'CLA-CredentialLetterTemplate.docx') 
BEGIN 
    DELETE @ID

    INSERT INTO tblStoredFile 
                (ExternalFileId,FileSize,[FileName],UploadedByPersonId,UploadedByUserId,UploadedDateTime,DocumentTypeId) 
    OUTPUT      INSERTED.StoredFileId INTO @ID 
    VALUES      (N'CredentialLetterTemplate\CLA-CredentialLetterTemplate.docx', 140389, 'CLA-CredentialLetterTemplate.docx', NULL, 40, Getdate(), 22) 

	INSERT INTO tblCredentialTypeTemplate 
				(CredentialTypeId, StoredFileId, DocumentNameTemplate) 
	VALUES		
		(15, (SELECT ID from @ID), '[[Credential Type]] [[Skill]] [[Document Number]] Letter')
END 
