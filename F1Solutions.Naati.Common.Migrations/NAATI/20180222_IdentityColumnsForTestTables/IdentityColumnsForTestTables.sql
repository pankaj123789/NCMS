-- drop FK and DF constraints

ALTER TABLE dbo.tblJob
	DROP CONSTRAINT FK_tblJob_tblUser

ALTER TABLE dbo.tblJob
	DROP CONSTRAINT FK_tblJob_tblUser1

ALTER TABLE dbo.tblJob
	DROP CONSTRAINT FK_tblJob_tblUser2

ALTER TABLE dbo.tblJob
	DROP CONSTRAINT FK_tblJob_tblLanguage

ALTER TABLE dbo.tblJob
	DROP CONSTRAINT DF__tblJob__JobCateg__7B712C3B

ALTER TABLE dbo.tblJob
	DROP CONSTRAINT DF__tblJob__ReviewFr__7D5974AD

ALTER TABLE dbo.tblTestResult
	DROP CONSTRAINT FK_tblTestResult_tblJob

ALTER TABLE dbo.tblWorkshopMaterial
	DROP CONSTRAINT FK_tblWorkshopMaterial_tblJob

ALTER TABLE dbo.tblJobCorrespondence
	DROP CONSTRAINT FK_tblJobCorrespondence_tblJob

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT FK_tblJobExaminer_tblJob

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT FK_tblJobExaminer_tblPanelMembership

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT FK_tblJobExaminer_tblUser1

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT FK_tblJobExaminer_tblUser2

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT FK_tblJobExaminer_tblProductSpecification

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT DF_tblJobExaminer_ExaminerPaperLost

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT DF__tblJobExa__Lette__2878DCDC

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT DF_tblJobExaminer_DateAllocated

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT DF_tblJobExaminer_Status

ALTER TABLE dbo.tblJobExaminer
	DROP CONSTRAINT DF_tblJobExaminer_PaidReviewer

ALTER TABLE dbo.tblExaminerMarking
	DROP CONSTRAINT FK_tblExaminerMarking_tblJobExaminer

ALTER TABLE dbo.tblJobExaminerPayroll
	DROP CONSTRAINT FK_tblJobExaminerPayroll_tblJobExaminer

ALTER TABLE dbo.tblTestResult
	DROP CONSTRAINT DF__tblTestRe__Resul__463E49ED

ALTER TABLE dbo.tblTestResult
	DROP CONSTRAINT DF__tblTestRe__Allow__47326E26

ALTER TABLE dbo.tblTestResultAttachment
	DROP CONSTRAINT FK_tblTestResultttachment_tblTestResult

ALTER TABLE dbo.tblTestComponentResult
	DROP CONSTRAINT FK_tblTestComponentResult_tblTestResult

ALTER TABLE dbo.tblExaminerMarking
	DROP CONSTRAINT FK_tblExaminerMarking_tblTestResult

ALTER TABLE dbo.tblTestResultFailureReason
	DROP CONSTRAINT FK_tblTestResultFailureReason_tblTestResult

ALTER TABLE dbo.tblTestComponent
	DROP CONSTRAINT FK_tblTestComponent_tluTestComponentType

ALTER TABLE dbo.tblTestComponent
	DROP CONSTRAINT FK_tblTestComponent_tblTestSpecification1

ALTER TABLE dbo.tblTestComponent
	DROP CONSTRAINT DF_tblTestComponent_Group

ALTER TABLE dbo.tblTestComponentResult
	DROP CONSTRAINT DF_tblTestComponentResult_ComponentNumber

ALTER TABLE dbo.tblTestComponentResult
	DROP CONSTRAINT DF_tblTestComponentResult_GroupNumber

ALTER TABLE dbo.tblExaminerMarking
	DROP CONSTRAINT DF__tblExamin__Count__10C14EDC

ALTER TABLE dbo.tblExaminerTestComponentResult
	DROP CONSTRAINT FK_tblExaminerTestComponentResult_tblExaminerMarking

ALTER TABLE dbo.tblExaminerTestComponentResult
	DROP CONSTRAINT FK_tblExaminerTestComponentResult_tluTestComponentType

ALTER TABLE dbo.tblTestAvailability
	DROP CONSTRAINT FK_tblTestAvailability_tblTestSpecification

ALTER TABLE dbo.tblTestAvailability
	DROP CONSTRAINT FK_tblTestAvailability_tblTestSpecification1

ALTER TABLE dbo.tblTestAvailability
	DROP CONSTRAINT FK_tblTestAvailability_tblTestSpecification2

ALTER TABLE dbo.tblTestAvailability
	DROP CONSTRAINT FK_tblTestAvailability_tblTestSpecification3

ALTER TABLE dbo.tblTestSpecificationMapping
	DROP CONSTRAINT FK_TestSpecificationMapping_TestSpecification

CREATE TABLE dbo.Tmp_tblJob
	(
	JobId int NOT NULL IDENTITY (1, 1),
	LanguageId int NULL,
	SentDate datetime NULL,
	SentUserID int NULL,
	DueDate datetime NULL,
	ReceivedDate datetime NULL,
	ReceivedUserId int NULL,
	SentToPayrollDate datetime NULL,
	SentToPayrollUserId int NULL,
	InitialJobId int NULL,
	Note varchar(1000) NULL,
	Name varchar(100) NOT NULL,
	SentToPanelMembershipId int NULL,
	SettingMaterialId int NULL,
	WorkshopId int NULL,
	NumberOfPapers int NULL,
	NumberOfItems char(10) NULL,
	JobCost money NULL,
	JobLost bit NULL,
	JobType varchar(50) NULL,
	JobCategory int NOT NULL,
	ReviewFromJobId int NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.Tmp_tblJob ADD CONSTRAINT
	DF_Job_JobCategory DEFAULT (0) FOR JobCategory

ALTER TABLE dbo.Tmp_tblJob ADD CONSTRAINT
	DF_Job_ReviewFromJobId DEFAULT (null) FOR ReviewFromJobId

SET IDENTITY_INSERT dbo.Tmp_tblJob ON

IF EXISTS(SELECT * FROM dbo.tblJob)
	 EXEC('INSERT INTO dbo.Tmp_tblJob (JobId, LanguageId, SentDate, SentUserID, DueDate, ReceivedDate, ReceivedUserId, SentToPayrollDate, SentToPayrollUserId, InitialJobId, Note, Name, SentToPanelMembershipId, SettingMaterialId, WorkshopId, NumberOfPapers, NumberOfItems, JobCost, JobLost, JobType, JobCategory, ReviewFromJobId)
		SELECT JobId, LanguageId, SentDate, SentUserID, DueDate, ReceivedDate, ReceivedUserId, SentToPayrollDate, SentToPayrollUserId, InitialJobId, Note, Name, SentToPanelMembershipId, SettingMaterialId, WorkshopId, NumberOfPapers, NumberOfItems, JobCost, JobLost, JobType, JobCategory, ReviewFromJobId FROM dbo.tblJob WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblJob OFF

DROP TABLE dbo.tblJob

EXECUTE sp_rename N'dbo.Tmp_tblJob', N'tblJob', 'OBJECT' 

ALTER TABLE dbo.tblJob ADD CONSTRAINT
	PK_tblJob PRIMARY KEY CLUSTERED 
	(
	JobId
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX _dta_index_tblJob_7_197575742__K14 ON dbo.tblJob
	(
	SettingMaterialId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX _dta_index_tblJob_7_197575742__K1_K14 ON dbo.tblJob
	(
	JobId,
	SettingMaterialId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

-- tblJobExaminer

CREATE TABLE dbo.Tmp_tblJobExaminer
	(
	JobExaminerID int NOT NULL IDENTITY (1, 1),
	JobId int NOT NULL,
	PanelMembershipId int NOT NULL,
	ThirdExaminer bit NOT NULL,
	ExaminerSentDate datetime NULL,
	ExaminerSentUserID int NULL,
	ExaminerReceivedDate datetime NULL,
	ExaminerReceivedUserID int NULL,
	ExaminerToPayrollDate datetime NULL,
	ExaminerToPayrollUserID int NULL,
	ExaminerCost money NULL,
	ExaminerPaperLost bit NOT NULL,
	LetterRecipient bit NOT NULL,
	DateAllocated datetime NOT NULL,
	Status char(1) NOT NULL,
	ExaminerPaperReceivedDate datetime NULL,
	PaidReviewer bit NULL,
	ProductSpecificationId int NULL,
	ProductSpecificationChangedDate datetime NULL,
	ProductSpecificationChangedUserId int NULL,
	ValidatedDate datetime NULL,
	ValidatedUserId int NULL,
	PrimaryContact bit NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.Tmp_tblJobExaminer ADD CONSTRAINT
	DF_JobExaminer_ExaminerPaperLost DEFAULT (0) FOR ExaminerPaperLost

ALTER TABLE dbo.Tmp_tblJobExaminer ADD CONSTRAINT
	DF_JobExaminer_LetterRecipient DEFAULT (0) FOR LetterRecipient

ALTER TABLE dbo.Tmp_tblJobExaminer ADD CONSTRAINT
	DF_JobExaminer_DateAllocated DEFAULT (getdate()) FOR DateAllocated

ALTER TABLE dbo.Tmp_tblJobExaminer ADD CONSTRAINT
	DF_JobExaminer_Status DEFAULT ('I') FOR Status

ALTER TABLE dbo.Tmp_tblJobExaminer ADD CONSTRAINT
	DF_JobExaminer_PaidReviewer DEFAULT ((0)) FOR PaidReviewer

SET IDENTITY_INSERT dbo.Tmp_tblJobExaminer ON

IF EXISTS(SELECT * FROM dbo.tblJobExaminer)
	 EXEC('INSERT INTO dbo.Tmp_tblJobExaminer (JobExaminerID, JobId, PanelMembershipId, ThirdExaminer, ExaminerSentDate, ExaminerSentUserID, ExaminerReceivedDate, ExaminerReceivedUserID, ExaminerToPayrollDate, ExaminerToPayrollUserID, ExaminerCost, ExaminerPaperLost, LetterRecipient, DateAllocated, Status, ExaminerPaperReceivedDate, PaidReviewer, ProductSpecificationId, ProductSpecificationChangedDate, ProductSpecificationChangedUserId, ValidatedDate, ValidatedUserId, PrimaryContact)
		SELECT JobExaminerID, JobId, PanelMembershipId, ThirdExaminer, ExaminerSentDate, ExaminerSentUserID, ExaminerReceivedDate, ExaminerReceivedUserID, ExaminerToPayrollDate, ExaminerToPayrollUserID, ExaminerCost, ExaminerPaperLost, LetterRecipient, DateAllocated, Status, ExaminerPaperReceivedDate, PaidReviewer, ProductSpecificationId, ProductSpecificationChangedDate, ProductSpecificationChangedUserId, ValidatedDate, ValidatedUserId, PrimaryContact FROM dbo.tblJobExaminer WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblJobExaminer OFF

DROP TABLE dbo.tblJobExaminer

EXECUTE sp_rename N'dbo.Tmp_tblJobExaminer', N'tblJobExaminer', 'OBJECT' 

ALTER TABLE dbo.tblJobExaminer ADD CONSTRAINT
	PK_tblJobExaminer PRIMARY KEY CLUSTERED 
	(
	JobExaminerID
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX _dta_index_tblJobExaminer_7_229575856__K1_K2_K3 ON dbo.tblJobExaminer
	(
	JobExaminerID,
	JobId,
	PanelMembershipId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

-- tblTestResult

CREATE TABLE dbo.Tmp_tblTestResult
	(
	TestResultId int NOT NULL IDENTITY (1, 1),
	TestSittingId int NOT NULL,
	CurrentJobId int NULL,
	ResultTypeId int NOT NULL,
	Comments1 varchar(500) NOT NULL,
	Comments2 varchar(500) NOT NULL,
	CommentsEthics varchar(500) NOT NULL,
	CommentsGeneral varchar(500) NOT NULL,
	ThirdExaminerRequired bit NOT NULL,
	ReviewInvoiceLineId int NULL,
	ProcessedDate datetime NULL,
	SatDate datetime NULL,
	ResultChecked bit NOT NULL,
	AllowCalculate bit NOT NULL,
	IncludePreviousMarks bit NULL,
	TestSpecificationMappingId int NOT NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.Tmp_tblTestResult ADD CONSTRAINT
	DF_TestResult_ResulChecked DEFAULT (0) FOR ResultChecked

ALTER TABLE dbo.Tmp_tblTestResult ADD CONSTRAINT
	DF_TestResult_AllowCalculate DEFAULT (0) FOR AllowCalculate

SET IDENTITY_INSERT dbo.Tmp_tblTestResult ON

IF EXISTS(SELECT * FROM dbo.tblTestResult)
	 EXEC('INSERT INTO dbo.Tmp_tblTestResult (TestResultId, TestSittingId, CurrentJobId, ResultTypeId, Comments1, Comments2, CommentsEthics, CommentsGeneral, ThirdExaminerRequired, ReviewInvoiceLineId, ProcessedDate, SatDate, ResultChecked, AllowCalculate, IncludePreviousMarks, TestSpecificationMappingId)
		SELECT TestResultId, TestSittingId, CurrentJobId, ResultTypeId, Comments1, Comments2, CommentsEthics, CommentsGeneral, ThirdExaminerRequired, ReviewInvoiceLineId, ProcessedDate, SatDate, ResultChecked, AllowCalculate, IncludePreviousMarks, TestSpecificationMappingId FROM dbo.tblTestResult WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblTestResult OFF

DROP TABLE dbo.tblTestResult

EXECUTE sp_rename N'dbo.Tmp_tblTestResult', N'tblTestResult', 'OBJECT' 

ALTER TABLE dbo.tblTestResult ADD CONSTRAINT
	PK_tblTestResult PRIMARY KEY CLUSTERED 
	(
	TestResultId
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX _dta_index_tblTestResult_7_901578250__K2_K3_K1_13 ON dbo.tblTestResult
	(
	TestSittingId,
	CurrentJobId,
	TestResultId
	) INCLUDE (SatDate) 
 WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

-- tblTestComponent

CREATE TABLE dbo.Tmp_tblTestComponent
	(
	TestComponentId int NOT NULL IDENTITY (1, 1),
	TestSpecificationId int NOT NULL,
	TypeId int NOT NULL,
	TotalMarks int NOT NULL,
	PassMark float(53) NOT NULL,
	ComponentNumber int NOT NULL,
	Name varchar(50) NOT NULL,
	GroupNumber int NOT NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.Tmp_tblTestComponent ADD CONSTRAINT
	DF_TestComponent_Group DEFAULT (1) FOR GroupNumber

SET IDENTITY_INSERT dbo.Tmp_tblTestComponent ON

IF EXISTS(SELECT * FROM dbo.tblTestComponent)
	 EXEC('INSERT INTO dbo.Tmp_tblTestComponent (TestComponentId, TestSpecificationId, TypeId, TotalMarks, PassMark, ComponentNumber, Name, GroupNumber)
		SELECT TestComponentId, TestSpecificationId, TypeId, TotalMarks, PassMark, ComponentNumber, Name, GroupNumber FROM dbo.tblTestComponent WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblTestComponent OFF

DROP TABLE dbo.tblTestComponent

EXECUTE sp_rename N'dbo.Tmp_tblTestComponent', N'tblTestComponent', 'OBJECT' 

ALTER TABLE dbo.tblTestComponent ADD CONSTRAINT
	PK_tblTestComponent PRIMARY KEY CLUSTERED 
	(
	TestComponentId
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

-- tblTestComponentResult

CREATE TABLE dbo.Tmp_tblTestComponentResult
	(
	TestComponentResultId int NOT NULL IDENTITY (1, 1),
	TestResultId int NOT NULL,
	Mark float(53) NOT NULL,
	TypeId int NULL,
	TotalMarks int NULL,
	PassMark float(53) NULL,
	ComponentNumber int NULL,
	GroupNumber int NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.Tmp_tblTestComponentResult ADD CONSTRAINT
	DF_TestComponentResult_ComponentNumber DEFAULT (1) FOR ComponentNumber

ALTER TABLE dbo.Tmp_tblTestComponentResult ADD CONSTRAINT
	DF_TestComponentResult_GroupNumber DEFAULT (1) FOR GroupNumber

SET IDENTITY_INSERT dbo.Tmp_tblTestComponentResult ON

IF EXISTS(SELECT * FROM dbo.tblTestComponentResult)
	 EXEC('INSERT INTO dbo.Tmp_tblTestComponentResult (TestComponentResultId, TestResultId, Mark, TypeId, TotalMarks, PassMark, ComponentNumber, GroupNumber)
		SELECT TestComponentResultId, TestResultId, Mark, TypeId, TotalMarks, PassMark, ComponentNumber, GroupNumber FROM dbo.tblTestComponentResult WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblTestComponentResult OFF

DROP TABLE dbo.tblTestComponentResult

EXECUTE sp_rename N'dbo.Tmp_tblTestComponentResult', N'tblTestComponentResult', 'OBJECT' 

ALTER TABLE dbo.tblTestComponentResult ADD CONSTRAINT
	PK_tblTestComponentResult PRIMARY KEY CLUSTERED 
	(
	TestComponentResultId
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

-- tblExaminerMarking

CREATE TABLE dbo.Tmp_tblExaminerMarking
	(
	ExaminerMarkingID int NOT NULL IDENTITY (1, 1),
	TestResultID int NOT NULL,
	JobExaminerID int NULL,
	CountMarks bit NOT NULL,
	Comments nvarchar(4000) NULL,
	ReasonsForPoorPerformance varchar(100) NULL,
	PrimaryReasonForFailure int NULL,
	Status char(1) NULL,
	SubmittedDate datetime NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.Tmp_tblExaminerMarking ADD CONSTRAINT
	DF_ExaminerMarking_CountMarks DEFAULT (0) FOR CountMarks

SET IDENTITY_INSERT dbo.Tmp_tblExaminerMarking ON

IF EXISTS(SELECT * FROM dbo.tblExaminerMarking)
	 EXEC('INSERT INTO dbo.Tmp_tblExaminerMarking (ExaminerMarkingID, TestResultID, JobExaminerID, CountMarks, Comments, ReasonsForPoorPerformance, PrimaryReasonForFailure, Status, SubmittedDate)
		SELECT ExaminerMarkingID, TestResultID, JobExaminerID, CountMarks, Comments, ReasonsForPoorPerformance, PrimaryReasonForFailure, Status, SubmittedDate FROM dbo.tblExaminerMarking WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblExaminerMarking OFF

DROP TABLE dbo.tblExaminerMarking

EXECUTE sp_rename N'dbo.Tmp_tblExaminerMarking', N'tblExaminerMarking', 'OBJECT' 

ALTER TABLE dbo.tblExaminerMarking ADD CONSTRAINT
	PK_tblExaminerMarking PRIMARY KEY CLUSTERED 
	(
	ExaminerMarkingID
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX ix_tblExaminerMarking_JobExaminerID ON dbo.tblExaminerMarking
	(
	JobExaminerID
	) INCLUDE (TestResultID) 
 WITH( PAD_INDEX = OFF, FILLFACTOR = 100, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX _dta_index_tblExaminerMarking_7_1034486764__K2_K1_K3_5_10 ON dbo.tblExaminerMarking
	(
	TestResultID,
	ExaminerMarkingID,
	JobExaminerID
	) INCLUDE (CountMarks, SubmittedDate) 
 WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

-- tblExaminerTestComponentResult

CREATE TABLE dbo.Tmp_tblExaminerTestComponentResult
	(
	ExaminerTestComponentResultID int NOT NULL IDENTITY (1, 1),
	ExaminerMarkingID int NOT NULL,
	Mark float(53) NOT NULL,
	TypeID int NULL,
	TotalMarks int NULL,
	PassMark float(53) NULL,
	ComponentNumber int NULL,
	GroupNumber int NULL
	)  ON [PRIMARY]

SET IDENTITY_INSERT dbo.Tmp_tblExaminerTestComponentResult ON

IF EXISTS(SELECT * FROM dbo.tblExaminerTestComponentResult)
	 EXEC('INSERT INTO dbo.Tmp_tblExaminerTestComponentResult (ExaminerTestComponentResultID, ExaminerMarkingID, Mark, TypeID, TotalMarks, PassMark, ComponentNumber, GroupNumber)
		SELECT ExaminerTestComponentResultID, ExaminerMarkingID, Mark, TypeID, TotalMarks, PassMark, ComponentNumber, GroupNumber FROM dbo.tblExaminerTestComponentResult WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblExaminerTestComponentResult OFF

DROP TABLE dbo.tblExaminerTestComponentResult

EXECUTE sp_rename N'dbo.Tmp_tblExaminerTestComponentResult', N'tblExaminerTestComponentResult', 'OBJECT' 

ALTER TABLE dbo.tblExaminerTestComponentResult ADD CONSTRAINT
	PK_tblExaminerTestComponentResult PRIMARY KEY CLUSTERED 
	(
	ExaminerTestComponentResultID
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

-- tblTestSpecification

CREATE TABLE dbo.Tmp_tblTestSpecification
	(
	TestSpecificationId int NOT NULL IDENTITY (1, 1),
	Description varchar(500) NOT NULL,
	OverallPassMark int NOT NULL
	)  ON [PRIMARY]

SET IDENTITY_INSERT dbo.Tmp_tblTestSpecification ON

IF EXISTS(SELECT * FROM dbo.tblTestSpecification)
	 EXEC('INSERT INTO dbo.Tmp_tblTestSpecification (TestSpecificationId, Description, OverallPassMark)
		SELECT TestSpecificationId, Description, OverallPassMark FROM dbo.tblTestSpecification WITH (HOLDLOCK TABLOCKX)')

SET IDENTITY_INSERT dbo.Tmp_tblTestSpecification OFF

DROP TABLE dbo.tblTestSpecification

EXECUTE sp_rename N'dbo.Tmp_tblTestSpecification', N'tblTestSpecification', 'OBJECT' 

ALTER TABLE dbo.tblTestSpecification ADD CONSTRAINT
	PK_tblTestSpecification PRIMARY KEY CLUSTERED 
	(
	TestSpecificationId
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

--- add FK constraints

ALTER TABLE dbo.tblJob ADD CONSTRAINT
	FK_Job_Language FOREIGN KEY
	(
	LanguageId
	) REFERENCES dbo.tblLanguage
	(
	LanguageId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJob ADD CONSTRAINT
	FK_Job_User1 FOREIGN KEY
	(
	SentUserID
	) REFERENCES dbo.tblUser
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJob ADD CONSTRAINT
	FK_Job_User2 FOREIGN KEY
	(
	ReceivedUserId
	) REFERENCES dbo.tblUser
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJob ADD CONSTRAINT
	FK_Job_User3 FOREIGN KEY
	(
	SentToPayrollUserId
	) REFERENCES dbo.tblUser
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJobExaminer ADD CONSTRAINT
	FK_JobExaminer_Job FOREIGN KEY
	(
	JobId
	) REFERENCES dbo.tblJob
	(
	JobId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJobCorrespondence ADD CONSTRAINT
	FK_JobCorrespondence_Job FOREIGN KEY
	(
	JobId
	) REFERENCES dbo.tblJob
	(
	JobId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblWorkshopMaterial ADD CONSTRAINT
	FK_WorkshopMaterial_Job FOREIGN KEY
	(
	JobId
	) REFERENCES dbo.tblJob
	(
	JobId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestResult ADD CONSTRAINT
	FK_TestResult_Job FOREIGN KEY
	(
	CurrentJobId
	) REFERENCES dbo.tblJob
	(
	JobId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJobExaminer ADD CONSTRAINT
	FK_JobExaminer_ProductSpecification FOREIGN KEY
	(
	ProductSpecificationId
	) REFERENCES dbo.tblProductSpecification
	(
	ProductSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJobExaminer ADD CONSTRAINT
	FK_JobExaminer_User1 FOREIGN KEY
	(
	ProductSpecificationChangedUserId
	) REFERENCES dbo.tblUser
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJobExaminer ADD CONSTRAINT
	FK_JobExaminer_User2 FOREIGN KEY
	(
	ValidatedUserId
	) REFERENCES dbo.tblUser
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJobExaminer ADD CONSTRAINT
	FK_JobExaminer_PanelMembership FOREIGN KEY
	(
	PanelMembershipId
	) REFERENCES dbo.tblPanelMembership
	(
	PanelMembershipId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblJobExaminerPayroll ADD CONSTRAINT
	FK_JobExaminerPayroll_JobExaminer FOREIGN KEY
	(
	JobExaminerId
	) REFERENCES dbo.tblJobExaminer
	(
	JobExaminerID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblExaminerMarking ADD CONSTRAINT
	FK_ExaminerMarking_JobExaminer FOREIGN KEY
	(
	JobExaminerID
	) REFERENCES dbo.tblJobExaminer
	(
	JobExaminerID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestResult ADD CONSTRAINT
	FK_TestResult_ResultType FOREIGN KEY
	(
	ResultTypeId
	) REFERENCES dbo.tluResultType
	(
	ResultTypeId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestResult ADD CONSTRAINT
	FK_TestResult_TestSitting FOREIGN KEY
	(
	TestSittingId
	) REFERENCES dbo.tblTestSitting
	(
	TestSittingId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestResultFailureReason ADD CONSTRAINT
	FK_TestResultFailureReason_TestResult FOREIGN KEY
	(
	TestResultId
	) REFERENCES dbo.tblTestResult
	(
	TestResultId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestComponentResult ADD CONSTRAINT
	FK_TestComponentResult_TestResult FOREIGN KEY
	(
	TestResultId
	) REFERENCES dbo.tblTestResult
	(
	TestResultId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestResultAttachment ADD CONSTRAINT
	FK_TestResultttachment_TestResult FOREIGN KEY
	(
	TestResultId
	) REFERENCES dbo.tblTestResult
	(
	TestResultId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestComponent ADD CONSTRAINT
	FK_TestComponent_TestSpecification1 FOREIGN KEY
	(
	TestSpecificationId
	) REFERENCES dbo.tblTestSpecification
	(
	TestSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestComponent ADD CONSTRAINT
	FK_TestComponent_TestComponentType FOREIGN KEY
	(
	TypeId
	) REFERENCES dbo.tluTestComponentType
	(
	TestComponentTypeId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblExaminerTestComponentResult ADD CONSTRAINT
	FK_ExaminerTestComponentResult_ExaminerMarking FOREIGN KEY
	(
	ExaminerMarkingID
	) REFERENCES dbo.tblExaminerMarking
	(
	ExaminerMarkingID
	) ON UPDATE  NO ACTION 
	 ON DELETE NO ACTION

ALTER TABLE dbo.tblExaminerTestComponentResult ADD CONSTRAINT
	FK_ExaminerTestComponentResult_tblExaminerMarking FOREIGN KEY
	(
	ExaminerMarkingID
	) REFERENCES dbo.tblExaminerMarking
	(
	ExaminerMarkingID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION

ALTER TABLE dbo.tblExaminerTestComponentResult ADD CONSTRAINT
	FK_ExaminerTestComponentResult_TestComponentType FOREIGN KEY
	(
	TypeID
	) REFERENCES dbo.tluTestComponentType
	(
	TestComponentTypeId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 
ALTER TABLE dbo.tblTestSpecificationMapping ADD CONSTRAINT
	FK_TestSpecificationMapping_TestSpecification FOREIGN KEY
	(
	TestSpecificationId
	) REFERENCES dbo.tblTestSpecification
	(
	TestSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestAvailability ADD CONSTRAINT
	FK_TestAvailability_TestSpecification1 FOREIGN KEY
	(
	ToEnglishSpecificationId
	) REFERENCES dbo.tblTestSpecification
	(
	TestSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestAvailability ADD CONSTRAINT
	FK_TestAvailability_TestSpecification2 FOREIGN KEY
	(
	FromEnglishSpecificationId
	) REFERENCES dbo.tblTestSpecification
	(
	TestSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestAvailability ADD CONSTRAINT
	FK_TestAvailability_TestSpecification3 FOREIGN KEY
	(
	BothSpecificationId
	) REFERENCES dbo.tblTestSpecification
	(
	TestSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.tblTestAvailability WITH NOCHECK ADD CONSTRAINT
	FK_TestAvailability_TestSpecification4 FOREIGN KEY
	(
	SupplementarySpecificationId
	) REFERENCES dbo.tblTestSpecification
	(
	TestSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

ALTER TABLE dbo.tblTestComponent ADD CONSTRAINT
	FK_TestComponent_TestSpecification FOREIGN KEY
	(
	TestSpecificationId
	) REFERENCES dbo.tblTestSpecification
	(
	TestSpecificationId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
