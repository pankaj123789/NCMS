DECLARE @ID TABLE (ID INT) 

IF NOT EXISTS (SELECT * 
               FROM   tblStoredFile 
               WHERE  [FileName] = 'EthicsInterculturalCertificateTemplate.docx') 
BEGIN 
    DELETE @ID

    INSERT INTO tblStoredFile 
                (ExternalFileId,FileSize,[FileName],UploadedByPersonId,UploadedByUserId,UploadedDateTime,DocumentTypeId) 
    OUTPUT      INSERTED.StoredFileId INTO @ID 
    VALUES      (N'CertificateTemplate\EthicsInterculturalCertificateTemplate.docx',217629,'EthicsInterculturalCertificateTemplate.docx',NULL,40,Getdate(),22) 

	INSERT INTO tblCredentialTypeTemplate 
				(CredentialTypeId, StoredFileId, DocumentNameTemplate) 
	VALUES		
		(16, (SELECT ID from @ID), '[[NAATI No]] [[Credential Type]] [[Document Number]] Certificate'),
		(17, (SELECT ID from @ID), '[[NAATI No]] [[Credential Type]] [[Document Number]] Certificate')
END 
