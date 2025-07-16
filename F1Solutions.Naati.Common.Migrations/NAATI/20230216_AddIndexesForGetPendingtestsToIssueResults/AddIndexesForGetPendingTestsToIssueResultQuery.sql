IF NOT EXISTS 
	(
		SELECT * 
		FROM sys.indexes 
		WHERE name='IX_CredentialRequest_CredentialRequestStatusType_Includes_CredentialApplication' AND object_id = OBJECT_ID('[dbo].[tblCredentialRequest]')
	)
BEGIN
	CREATE NONCLUSTERED INDEX 
		IX_CredentialRequest_CredentialRequestStatusType_Includes_CredentialApplication 
	ON 
		[dbo].[tblCredentialRequest]
			(    
				[CredentialRequestStatusTypeId] ASC
			)
	INCLUDE
		(
			[CredentialApplicationId]
		) 
	WITH 
		(
			PAD_INDEX = OFF, 
			STATISTICS_NORECOMPUTE = OFF, 
			SORT_IN_TEMPDB = OFF,
			DROP_EXISTING = OFF,
			ONLINE = ON, 
			ALLOW_ROW_LOCKS = ON, 
			ALLOW_PAGE_LOCKS = ON, 
			OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
		) 
	ON 
		[PRIMARY] 
END


IF NOT EXISTS 
	(
		SELECT * 
		FROM sys.indexes 
		WHERE name='IX_JobExaminer_Job_Includes_ThirdExaminer_ReceivedDate_PaidReviewer' AND object_id = OBJECT_ID('[dbo].[tblJobExaminer]')
	)
BEGIN
	CREATE NONCLUSTERED INDEX 
		IX_JobExaminer_Job_Includes_ThirdExaminer_ReceivedDate_PaidReviewer 
	ON 
		[dbo].[tblJobExaminer]
			(    
				[JobId] ASC
			)
	INCLUDE
		(    
			[ThirdExaminer],    
			[ExaminerReceivedDate],   
			[PaidReviewer]
		) 
	WITH 
		(
			PAD_INDEX = OFF, 
			STATISTICS_NORECOMPUTE = OFF, 
			SORT_IN_TEMPDB = OFF,
			DROP_EXISTING = OFF, 
			ONLINE = ON, 
			ALLOW_ROW_LOCKS = ON, 
			ALLOW_PAGE_LOCKS = ON, 
			OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
		) 
	ON 
		[PRIMARY] 
END
	
IF NOT EXISTS 
	(
		SELECT * 
		FROM sys.indexes 
		WHERE name='IX_TestResult_ResultChecked_AllowIssue_AutmoticIssuing_Includes_TestSitting_CurrentJob' AND object_id = OBJECT_ID('[dbo].[tblTestResult]')
	)
BEGIN
	CREATE NONCLUSTERED INDEX 
		IX_TestResult_ResultChecked_AllowIssue_AutmoticIssuing_Includes_TestSitting_CurrentJob 
	ON 
		[dbo].[tblTestResult]
			(    
				[ResultChecked] ASC,    
				[AllowIssue] ASC,   
				[AutomaticIssuingExaminer] ASC
			)
	INCLUDE
		(    
			[TestSittingId],    
			[CurrentJobId]
		) 
	WITH 
		(
			PAD_INDEX = OFF,
			STATISTICS_NORECOMPUTE = OFF, 
			SORT_IN_TEMPDB = OFF,
			DROP_EXISTING = OFF,
			ONLINE = ON,
			ALLOW_ROW_LOCKS = ON, 
			ALLOW_PAGE_LOCKS = ON,
			OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
		) 
	ON 
		[PRIMARY]
END