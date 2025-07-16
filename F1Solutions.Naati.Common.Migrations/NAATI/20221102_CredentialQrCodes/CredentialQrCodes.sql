BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblCredential SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.tblCredentialQrCode
    (
    CredentialQrCodeId int NOT NULL IDENTITY(1,1),
    CredentialId int NOT NULL,
    IssueDate date NOT NULL,
    QrCodeGuid uniqueidentifier NOT NULL
    )  ON [PRIMARY]
GO

ALTER TABLE dbo.tblCredentialQrCode ADD CONSTRAINT
	PK_tblCredentialQrCode PRIMARY KEY CLUSTERED 
	(
	CredentialQrCodeId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblCredentialQrCode ADD CONSTRAINT
	FK_tblCredentialQrCode_tblCredential FOREIGN KEY
	(
	CredentialId
	) REFERENCES dbo.tblCredential
	(
	CredentialId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 	
GO
ALTER TABLE dbo.tblCredentialQrCode SET (LOCK_ESCALATION = TABLE)
GO
COMMIT