ALTER TABLE [tblTestNote] 
  DROP CONSTRAINT [FK_TestNote_TestSessionCredentialRequest] 

ALTER TABLE [tblTestAttendanceDocument] 
  DROP CONSTRAINT [FK_TestAttendanceDocument_TestSessionCredentialRequest] 

ALTER TABLE [tblTestResult] 
  DROP CONSTRAINT [FK_TestResult_TestSessionCredentialRequest] 

ALTER TABLE [tblTestSessionCredentialRequest] 
  DROP CONSTRAINT [PK_tblTestSessionCredentialRequest] 

-- tblTestSessionCredentialRequest
EXEC sys.SP_RENAME 
  @objname = N'[tblTestSessionCredentialRequest]', 
  @newname = [tblTestSitting], 
  @objtype = N'OBJECT'; 
  
EXEC sys.SP_RENAME 
  @objname = N'[tblTestSitting].[TestSessionCredentialRequestId]', 
  @newname = N'TestSittingId', 
  @objtype = 'COLUMN' 

EXEC sys.SP_RENAME 
  N'[FK_TestSessionCredentialRequest_TestSession]', 
  N'FK_TestSitting_TestSession', 
  N'OBJECT' 

EXEC sys.SP_RENAME 
  N'[FK_TestSessionCredentialRequest_CredentialRequest]', 
  N'FK_TestSitting_CredentialRequest', 
  N'OBJECT' 

ALTER TABLE [tblTestSitting] 
  ADD CONSTRAINT [PK_tblTestSitting] PRIMARY KEY CLUSTERED ([TestSittingId] ASC) 

-- tblTestNote
ALTER TABLE [tblTestNote] 
  DROP CONSTRAINT [PK_tblTestNote] 
  
EXEC sys.SP_RENAME 
  @objname = N'[tblTestNote]', 
  @newname = [tblTestSittingNote], 
  @objtype = N'OBJECT'; 

EXEC sys.SP_RENAME 
  @objname = N'[tblTestSittingNote].[TestNoteId]', 
  @newname = N'TestSittingNoteId', 
  @objtype = 'COLUMN' 

EXEC sys.SP_RENAME 
  @objname = N'[tblTestSittingNote].[TestSessionCredentialRequestId]', 
  @newname = N'TestSittingId', 
  @objtype = 'COLUMN' 

ALTER TABLE [tblTestSittingNote] 
  ADD CONSTRAINT [PK_tblTestSittingNote] PRIMARY KEY CLUSTERED ([TestSittingNoteId] ASC) 

ALTER TABLE [tblTestSittingNote] 
  ADD CONSTRAINT [FK_tblTestSittingNote_TestSitting] FOREIGN KEY ([TestSittingId]) REFERENCES [tblTestSitting] ( [TestSittingId] ) 

-- tblTestAttendanceDocument
ALTER TABLE [tblTestAttendanceDocument] 
  DROP CONSTRAINT [PK_tblTestAttendanceDocument] 

EXEC sys.SP_RENAME 
  @objname = N'[tblTestAttendanceDocument]', 
  @newname = [tblTestSittingDocument], 
  @objtype = N'OBJECT'; 

EXEC sys.SP_RENAME 
  @objname = N'[tblTestSittingDocument].[TestAttendanceDocumentId]', 
  @newname = N'TestSittingDocumentId', 
  @objtype = 'COLUMN' 

EXEC sys.SP_RENAME 
  @objname = N'[tblTestSittingDocument].[TestSessionCredentialRequestId]', 
  @newname = N'TestSittingId', 
  @objtype = 'COLUMN' 

ALTER TABLE [tblTestSittingDocument] 
  ADD CONSTRAINT [PK_tblTestSittingDocument] PRIMARY KEY CLUSTERED ([TestSittingDocumentId] ASC)

ALTER TABLE [tblTestSittingDocument] 
  ADD CONSTRAINT [FK_TestSittingDocument_TestSitting] FOREIGN KEY ([TestSittingId]) REFERENCES [tblTestSitting] ( [TestSittingId] ) 

-- tblTestResult
EXEC sys.SP_RENAME 
  @objname = N'[tblTestResult].[TestSessionCredentialRequestId]', 
  @newname = N'TestSittingId', 
  @objtype = 'COLUMN' 

ALTER TABLE [tblTestResult] 
  ADD FOREIGN KEY ([TestSittingId]) REFERENCES [tblTestSitting] ( [TestSittingId] ) 

