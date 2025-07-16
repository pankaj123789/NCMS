UPDATE 
	tblStoredFile
SET
	StoredFileStatusTypeId = 1
WHERE
	StoredFileStatusTypeId IS NULL

UPDATE
	tblStoredFile
SET
	StoredFileStatusChangeDate = UploadedDateTime
WHERE
	StoredFileStatusChangeDate IS NULL

ALTER TABLE
	tblStoredFile
ALTER COLUMN
	StoredFileStatusTypeId
		int NOT NULL;

ALTER TABLE
	tblStoredFile
ALTER COLUMN
	StoredFileStatusChangeDate
		datetime NOT NULL;

