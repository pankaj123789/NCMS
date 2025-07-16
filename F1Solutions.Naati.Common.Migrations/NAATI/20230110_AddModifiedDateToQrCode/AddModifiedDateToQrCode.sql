ALTER TABLE dbo.tblCredentialQrCode ADD
	ModifiedDate date NOT NULL CONSTRAINT DF_tblCredentialQrCode_ModifiedDate DEFAULT GetDate()

ALTER TABLE dbo.tblCredentialQrCode
	DROP CONSTRAINT DF_tblCredentialQrCode_ModifiedBy

ALTER TABLE dbo.tblCredentialQrCode ADD CONSTRAINT
	DF_tblCredentialQrCode_ModifiedBy DEFAULT 0 FOR ModifiedBy

ALTER TABLE dbo.tblCredentialQrCode SET (LOCK_ESCALATION = TABLE)