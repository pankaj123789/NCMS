Set IDENTITY_INSERT tblStoredFileStatusType ON
INSERT INTO tblStoredFileStatusType ([StoredFileStatusTypeId],Name,DisplayName,DisplayOrder,ModifiedUser,ModifiedByNaati,ModifiedDate) values(1,'Current', 'Current', 1,40,0,GetDate())
Set IDENTITY_INSERT tblStoredFileStatusType OFF

UPDATE tblStoredFile
SET StoredFileStatusTypeId = 1,
StoredFileStatusChangeDate = GETDATE()