IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'tblCredentialPrerequisite'))
BEGIN
    DROP TABLE dbo.tblCredentialPrerequisite
END


GO

CREATE TABLE dbo.tblCredentialPrerequisite
	(
	CredentialPrerequisiteId int NOT NULL IDENTITY (1, 1),
	CredentialTypeId int NOT NULL,
	CredentialApplicationTypeId int NOT NULL,
	SkillMatch bit NOT NULL,
	ModifiedByNaati bit NOT NULL,
	ModifiedDate datetime NOT NULL,
	ModifiedUser int NOT NULL,
	CredentialTypePrerequisiteId int NOT NULL
	)  ON [PRIMARY]

GO

ALTER TABLE dbo.tblCredentialPrerequisite ADD CONSTRAINT
	FK_tblCredentialPrerequisite_tblUser FOREIGN KEY
	(
	ModifiedUser
	) REFERENCES dbo.tblUser
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tblCredentialPrerequisite ADD CONSTRAINT
	FK_tblCredentialPrerequisite_tblCredentialType FOREIGN KEY
	(
	CredentialTypeId
	) REFERENCES dbo.tblCredentialType
	(
	CredentialTypeId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.tblCredentialPrerequisite ADD CONSTRAINT
	FK_tblCredentialPrerequisite_tblCredentialApplicationType FOREIGN KEY
	(
	CredentialApplicationTypeId
	) REFERENCES dbo.tblCredentialApplicationType
	(
	CredentialApplicationTypeId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

