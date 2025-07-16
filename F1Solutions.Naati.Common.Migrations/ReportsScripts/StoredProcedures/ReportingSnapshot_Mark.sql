ALTER PROCEDURE [dbo].[ReportingSnapshot_Mark]
	@Date DateTime
AS
BEGIN
	
	DECLARE @MarkHistory as table([PersonId] [int] NOT NULL,[TestSittingId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL,[ExaminerNAATINumber] [int] NULL,[ExaminerName] [nvarchar](252) NULL,[IncludeMarks] [bit] NULL,[MarkId] [int] NOT NULL,[Mark] [float] NULL,[TotalMark] [int] NULL,[OverallMark] [float] NULL,
							  [TestResultId] [int] NULL,[ComponentName] [nvarchar](100) NULL,[ExaminerType] [nvarchar](50) NULL,[Cost] [money] NULL,[PaperLost] [bit] NULL,[PaperReceived] [datetime] NULL,[SentToPayroll] [datetime] NULL,[PassMark] [float] NULL,[OverallPassMark] [int] NULL,[OverallTotalMark] [int] NULL,
							  [PrimaryFailureReason] [nvarchar](50) NULL,[PoorPerformanceReasons] [nvarchar](100) NULL,[MarkerComments] [nvarchar](4000) NULL,[SubmittedDate] [datetime] NULL,[RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

INSERT into @MarkHistory

SELECT
		 CASE WHEN [Source].[PersonId] IS NULL THEN [Mark].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN [Mark].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		
		,@Date AS [ModifiedDate]
		
		,CASE WHEN [Source].[ExaminerNAATINumber] IS NULL THEN [Mark].[ExaminerNAATINumber] ELSE [Source].[ExaminerNAATINumber] END AS [ExaminerNAATINumber]
		,CASE WHEN [Source].[ExaminerName] IS NULL THEN [Mark].[ExaminerName] ELSE [Source].[ExaminerName] END AS [ExaminerName]
		,CASE WHEN [Source].[IncludeMarks] IS NULL THEN [Mark].[IncludeMarks] ELSE [Source].[IncludeMarks] END AS [IncludeMarks]
		,CASE WHEN [Source].[MarkId] IS NULL THEN [Mark].[MarkId] ELSE [Source].[MarkId] END AS [MarkId]
		,CASE WHEN [Source].[Mark] IS NULL THEN [Mark].[Mark] ELSE [Source].[Mark] END AS [Mark]
		,CASE WHEN [Source].[TotalMark] IS NULL THEN [Mark].[TotalMark] ELSE [Source].[TotalMark] END AS [TotalMark]
		,CASE WHEN [Source].[OverallMark] IS NULL THEN [Mark].[OverallMark] ELSE [Source].[OverallMark] END AS [OverallMark]
		,CASE WHEN [Source].[TestResultId] IS NULL THEN [Mark].[TestResultId] ELSE [Source].[TestResultId] END AS [TestResultId]
		,CASE WHEN [Source].[ComponentName] IS NULL THEN [Mark].[ComponentName] ELSE [Source].[ComponentName] END AS [ComponentName]
		,CASE WHEN [Source].[ExaminerType] IS NULL THEN [Mark].[ExaminerType] ELSE [Source].[ExaminerType] END AS [ExaminerType]
		,CASE WHEN [Source].[Cost] IS NULL THEN [Mark].[Cost] ELSE [Source].[Cost] END AS [Cost]
		,CASE WHEN [Source].[PaperLost] IS NULL THEN [Mark].[PaperLost] ELSE [Source].[PaperLost] END AS [PaperLost]
		,CASE WHEN [Source].[PaperReceived] IS NULL THEN [Mark].[PaperReceived] ELSE [Source].[PaperReceived] END AS [PaperReceived]
		,CASE WHEN [Source].[SentToPayroll] IS NULL THEN [Mark].[SentToPayroll] ELSE [Source].[SentToPayroll] END AS [SentToPayroll]
		,CASE WHEN [Source].[PassMark] IS NULL THEN [Mark].[PassMark] ELSE [Source].[PassMark] END AS [PassMark]
		,CASE WHEN [Source].[OverallPassMark] IS NULL THEN [Mark].[OverallPassMark] ELSE [Source].[OverallPassMark] END AS [OverallPassMark]
		,CASE WHEN [Source].[OverallTotalMark] IS NULL THEN [Mark].[OverallTotalMark] ELSE [Source].[OverallTotalMark] END AS [OverallTotalMark]
		,CASE WHEN [Source].[PrimaryFailureReason] IS NULL THEN [Mark].[PrimaryFailureReason] ELSE [Source].[PrimaryFailureReason] END AS [PrimaryFailureReason]
		,CASE WHEN [Source].[PoorPerformanceReasons] IS NULL THEN [Mark].[PoorPerformanceReasons] ELSE [Source].[PoorPerformanceReasons] END AS [PoorPerformanceReasons]
		,CASE WHEN [Source].[MarkerComments] IS NULL THEN [Mark].[MarkerComments] ELSE [Source].[MarkerComments] END AS [MarkerComments]
		,CASE WHEN [Source].[SubmittedDate] IS NULL THEN [Mark].[SubmittedDate] ELSE [Source].[SubmittedDate] END AS [SubmittedDate]
				
		,CASE WHEN [Source].[MarkId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM
	(
		SELECT
			etcr.[ExaminerTestComponentResultId] AS 'MarkId'
			,pm.[PersonId]
			,ts.[TestSittingId] AS 'TestSittingId'
			,tr.[TestResultId]
			,tct.[Name] COLLATE Latin1_General_CI_AS AS 'ComponentName'
			,(
				SELECT TOP 1
					[NAATINumber]
				FROM [naati_db]..[tblEntity]
				WHERE [EntityId] = 
				(
					SELECT TOP 1 EntityId
					FROM [naati_db]..[tblPerson]
					WHERE [PersonId] = pm.[PersonId]
				)
			) AS 'ExaminerNAATINumber'
			,(
				SELECT TOP 1
					[Title] + ' ' + [GivenName] + ' ' + [SurName]
				FROM [naati_db]..[tblPersonName] pn
				LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
				WHERE pn.[PersonId] = pm.[PersonId]
				ORDER BY pn.[EffectiveDate] DESC
			) COLLATE Latin1_General_CI_AS AS 'ExaminerName'
			,CASE
				WHEN je.[ThirdExaminer] = 1 THEN 'Third Examiner'
				WHEN j.[ReviewFromJobId] IS NOT NULL THEN 'Paid Reviewer'
				ELSE 'Original'
			END AS 'ExaminerType'
			,je.[ExaminerCost] AS 'Cost'
			,je.[ExaminerPaperLost] AS 'PaperLost'
			,je.[ExaminerPaperReceivedDate] AS 'PaperReceived'
			,je.[ExaminerToPayrollDate] AS 'SentToPayroll'
			,em.[CountMarks] AS 'IncludeMarks'
			,etcr.[Mark]
			,tctsms.[PassMark]
			,tctsms.[TotalMarks] AS 'TotalMark'
			,eom.[ExaminerOverallMark] AS 'OverallMark'
			,tssms.[OverallPassMark] 'OverallPassMark'
			,eom.[ExaminerTotalMarks] AS 'OverallTotalMark'
			,CASE em.[PrimaryReasonForFailure]
				WHEN 0 THEN ''
				WHEN 1 THEN 'Lack of proficiency in English'
				WHEN 2 THEN 'Lack of proficiency in LOTE'
				WHEN 3 THEN 'Lack of translating skills'
			END COLLATE Latin1_General_CI_AS AS 'PrimaryFailureReason'
			,em.[ReasonsForPoorPerformance] COLLATE Latin1_General_CI_AS AS 'PoorPerformanceReasons'
			,em.[Comments] COLLATE Latin1_General_CI_AS AS 'MarkerComments'
			,em.[SubmittedDate]
			
		FROM [naati_db]..[tblExaminerTestComponentResult] etcr
		LEFT JOIN [naati_db]..[tblExaminerMarking] em ON etcr.[ExaminerMarkingId] = em.[ExaminerMarkingId]
		LEFT JOIN [naati_db]..[tblJobExaminer] je ON em.[JobExaminerId] = je.[JobExaminerId]
		LEFT JOIN [naati_db]..[tblJob] j ON je.[JobId] = j.[JobId]
		LEFT JOIN [naati_db]..[tblPanelMembership] pm ON pm.[PanelMembershipId] = je.[PanelMembershipId]
		LEFT JOIN [naati_db]..[tblTestResult] tr ON tr.[TestResultId] = em.[TestResultId]
		LEFT JOIN [naati_db]..[tblTestSitting] ts ON tr.[TestSittingId] = ts.[TestSittingId]
		LEFT JOIN [naati_db]..[tblTestComponent] tct ON etcr.[TypeId] = tct.[TypeId] 
			AND etcr.[ComponentNumber] = tct.[ComponentNumber]
			AND ts.[TestSpecificationId] = tct.[TestSpecificationId]
		LEFT JOIN [naati_db]..[tblTestComponentTypeStandardMarkingScheme] tctsms ON tct.[TypeId] = tctsms.[TestComponentTypeId]
		LEFT JOIN [naati_db]..[tblCredentialRequest] cr ON ts.[CredentialRequestId] = cr.[CredentialRequestId]
		LEFT JOIN [naati_db]..[tblCredentialApplication] a ON cr.[CredentialApplicationId] = a.[CredentialApplicationId]
		--LEFT JOIN [naati_db]..[tblTestSpecification] tss ON cr.[CredentialTypeId] = tss.[CredentialTypeId]
		LEFT JOIN [naati_db]..[tblTestSpecification] tss ON tct.[TestSpecificationId] = tss.[TestSpecificationId] -- put this in to stop multiple matches
			AND tss.Active = 1
		LEFT JOIN [naati_db]..[tblTestSpecificationStandardMarkingScheme] tssms ON tss.[TestSpecificationId] = tssms.[TestSpecificationId]
		OUTER APPLY [dbo].[GetExaminerOverallMark](tr.[TestResultId], em.[JobExaminerID]) eom
		WHERE 
			tr.testresultid is not null and --there is no FK relationship between Examainer Marking and TestResult. So if there is a manual deletion it wont get picked up and this wouold have broken
			-- JUST STANDARD MARKING
			(SELECT COUNT(1)
			FROM [naati_db]..[tblTestComponentType] tct
				JOIN [naati_db]..[tblRubricMarkingCompetency] rmc ON tct.[TestComponentTypeId] = rmc.[TestComponentTypeId]
			WHERE tct.[TestSpecificationId] = ts.[TestSpecificationId]) = 0
	) [Source]
	FULL OUTER JOIN [Mark] ON [Source].[MarkId] = [Mark].[MarkId]
	WHERE (([Source].[PersonId] IS NOT NULL AND [Mark].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [Mark].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [Mark].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [Mark].[PersonId]))
	OR (([Source].[TestSittingId] IS NOT NULL AND [Mark].[TestSittingId] IS NULL) OR ([Source].[TestSittingId] IS NULL AND [Mark].[TestSittingId] IS NOT NULL) OR (([Source].[TestSittingId] IS NOT NULL AND [Mark].[TestSittingId] IS NOT NULL) AND [Source].[TestSittingId] != [Mark].[TestSittingId]))
	OR (([Source].[TestResultId] IS NOT NULL AND [Mark].[TestResultId] IS NULL) OR ([Source].[TestResultId] IS NULL AND [Mark].[TestResultId] IS NOT NULL) OR (([Source].[TestResultId] IS NOT NULL AND [Mark].[TestResultId] IS NOT NULL) AND [Source].[TestResultId] != [Mark].[TestResultId]))
	OR (([Source].[ComponentName] IS NOT NULL AND [Mark].[ComponentName] IS NULL) OR ([Source].[ComponentName] IS NULL AND [Mark].[ComponentName] IS NOT NULL) OR (([Source].[ComponentName] IS NOT NULL AND [Mark].[ComponentName] IS NOT NULL) AND [Source].[ComponentName] != [Mark].[ComponentName]))
	OR (([Source].[ExaminerNAATINumber] IS NOT NULL AND [Mark].[ExaminerNAATINumber] IS NULL) OR ([Source].[ExaminerNAATINumber] IS NULL AND [Mark].[ExaminerNAATINumber] IS NOT NULL) OR (([Source].[ExaminerNAATINumber] IS NOT NULL AND [Mark].[ExaminerNAATINumber] IS NOT NULL) AND [Source].[ExaminerNAATINumber] != [Mark].[ExaminerNAATINumber]))
	OR (([Source].[ExaminerName] IS NOT NULL AND [Mark].[ExaminerName] IS NULL) OR ([Source].[ExaminerName] IS NULL AND [Mark].[ExaminerName] IS NOT NULL) OR (([Source].[ExaminerName] IS NOT NULL AND [Mark].[ExaminerName] IS NOT NULL) AND [Source].[ExaminerName] != [Mark].[ExaminerName]))
	OR (([Source].[ExaminerType] IS NOT NULL AND [Mark].[ExaminerType] IS NULL) OR ([Source].[ExaminerType] IS NULL AND [Mark].[ExaminerType] IS NOT NULL) OR (([Source].[ExaminerType] IS NOT NULL AND [Mark].[ExaminerType] IS NOT NULL) AND [Source].[ExaminerType] != [Mark].[ExaminerType]))
	OR (([Source].[Cost] IS NOT NULL AND [Mark].[Cost] IS NULL) OR ([Source].[Cost] IS NULL AND [Mark].[Cost] IS NOT NULL) OR (([Source].[Cost] IS NOT NULL AND [Mark].[Cost] IS NOT NULL) AND [Source].[Cost] != [Mark].[Cost]))
	OR (([Source].[PaperLost] IS NOT NULL AND [Mark].[PaperLost] IS NULL) OR ([Source].[PaperLost] IS NULL AND [Mark].[PaperLost] IS NOT NULL) OR (([Source].[PaperLost] IS NOT NULL AND [Mark].[PaperLost] IS NOT NULL) AND [Source].[PaperLost] != [Mark].[PaperLost]))
	OR (([Source].[PaperReceived] IS NOT NULL AND [Mark].[PaperReceived] IS NULL) OR ([Source].[PaperReceived] IS NULL AND [Mark].[PaperReceived] IS NOT NULL) OR (([Source].[PaperReceived] IS NOT NULL AND [Mark].[PaperReceived] IS NOT NULL) AND [Source].[PaperReceived] != [Mark].[PaperReceived]))
	OR (([Source].[SentToPayroll] IS NOT NULL AND [Mark].[SentToPayroll] IS NULL) OR ([Source].[SentToPayroll] IS NULL AND [Mark].[SentToPayroll] IS NOT NULL) OR (([Source].[SentToPayroll] IS NOT NULL AND [Mark].[SentToPayroll] IS NOT NULL) AND [Source].[SentToPayroll] != [Mark].[SentToPayroll]))
	OR (([Source].[IncludeMarks] IS NOT NULL AND [Mark].[IncludeMarks] IS NULL) OR ([Source].[IncludeMarks] IS NULL AND [Mark].[IncludeMarks] IS NOT NULL) OR (([Source].[IncludeMarks] IS NOT NULL AND [Mark].[IncludeMarks] IS NOT NULL) AND [Source].[IncludeMarks] != [Mark].[IncludeMarks]))
	OR (([Source].[Mark] IS NOT NULL AND [Mark].[Mark] IS NULL) OR ([Source].[Mark] IS NULL AND [Mark].[Mark] IS NOT NULL) OR (([Source].[Mark] IS NOT NULL AND [Mark].[Mark] IS NOT NULL) AND [Source].[Mark] != [Mark].[Mark]))
	OR (([Source].[PassMark] IS NOT NULL AND [Mark].[PassMark] IS NULL) OR ([Source].[PassMark] IS NULL AND [Mark].[PassMark] IS NOT NULL) OR (([Source].[PassMark] IS NOT NULL AND [Mark].[PassMark] IS NOT NULL) AND [Source].[PassMark] != [Mark].[PassMark]))
	OR (([Source].[TotalMark] IS NOT NULL AND [Mark].[TotalMark] IS NULL) OR ([Source].[TotalMark] IS NULL AND [Mark].[TotalMark] IS NOT NULL) OR (([Source].[TotalMark] IS NOT NULL AND [Mark].[TotalMark] IS NOT NULL) AND [Source].[TotalMark] != [Mark].[TotalMark]))
	OR (([Source].[OverallMark] IS NOT NULL AND [Mark].[OverallMark] IS NULL) OR ([Source].[OverallMark] IS NULL AND [Mark].[OverallMark] IS NOT NULL) OR (([Source].[OverallMark] IS NOT NULL AND [Mark].[OverallMark] IS NOT NULL) AND [Source].[OverallMark] != [Mark].[OverallMark]))
	OR (([Source].[OverallPassMark] IS NOT NULL AND [Mark].[OverallPassMark] IS NULL) OR ([Source].[OverallPassMark] IS NULL AND [Mark].[OverallPassMark] IS NOT NULL) OR (([Source].[OverallPassMark] IS NOT NULL AND [Mark].[OverallPassMark] IS NOT NULL) AND [Source].[OverallPassMark] != [Mark].[OverallPassMark]))
	OR (([Source].[OverallTotalMark] IS NOT NULL AND [Mark].[OverallTotalMark] IS NULL) OR ([Source].[OverallTotalMark] IS NULL AND [Mark].[OverallTotalMark] IS NOT NULL) OR (([Source].[OverallTotalMark] IS NOT NULL AND [Mark].[OverallTotalMark] IS NOT NULL) AND [Source].[OverallTotalMark] != [Mark].[OverallTotalMark]))
	OR (([Source].[PrimaryFailureReason] IS NOT NULL AND [Mark].[PrimaryFailureReason] IS NULL) OR ([Source].[PrimaryFailureReason] IS NULL AND [Mark].[PrimaryFailureReason] IS NOT NULL) OR (([Source].[PrimaryFailureReason] IS NOT NULL AND [Mark].[PrimaryFailureReason] IS NOT NULL) AND [Source].[PrimaryFailureReason] != [Mark].[PrimaryFailureReason]))
	OR (([Source].[PoorPerformanceReasons] IS NOT NULL AND [Mark].[PoorPerformanceReasons] IS NULL) OR ([Source].[PoorPerformanceReasons] IS NULL AND [Mark].[PoorPerformanceReasons] IS NOT NULL) OR (([Source].[PoorPerformanceReasons] IS NOT NULL AND [Mark].[PoorPerformanceReasons] IS NOT NULL) AND [Source].[PoorPerformanceReasons] != [Mark].[PoorPerformanceReasons]))
	OR (([Source].[MarkerComments] IS NOT NULL AND [Mark].[MarkerComments] IS NULL) OR ([Source].[MarkerComments] IS NULL AND [Mark].[MarkerComments] IS NOT NULL) OR (([Source].[MarkerComments] IS NOT NULL AND [Mark].[MarkerComments] IS NOT NULL) AND [Source].[MarkerComments] != [Mark].[MarkerComments]))
	OR (([Source].[SubmittedDate] IS NOT NULL AND [Mark].[SubmittedDate] IS NULL) OR ([Source].[SubmittedDate] IS NULL AND [Mark].[SubmittedDate] IS NOT NULL) OR (([Source].[SubmittedDate] IS NOT NULL AND [Mark].[SubmittedDate] IS NOT NULL) AND [Source].[SubmittedDate] != [Mark].[SubmittedDate]))
		
	--select * from @MarkHistory
	
	BEGIN TRANSACTION 
	
	   --Merge operation delete
		MERGE MarkHistory AS Target USING(select * from @MarkHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[MarkId] = Source.[MarkId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE MarkHistory AS Target USING(	select * from @MarkHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MarkId] = Source.[MarkId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE MarkHistory AS Target USING(	select * from @MarkHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MarkId] = Source.[MarkId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.PersonId = Source.PersonId
		  ,Target.TestSittingId = Source.TestSittingId		  
		  ,Target.ExaminerNAATINumber = Source.ExaminerNAATINumber
		  ,Target.ExaminerName = Source.ExaminerName
		  ,Target.IncludeMarks = Source.IncludeMarks      
		  ,Target.MarkId = Source.MarkId
		  ,Target.Mark = Source.Mark
		  ,Target.TotalMark = Source.TotalMark
		  ,Target.OverallMark = Source.OverallMark

		  ,Target.TestResultId = Source.TestResultId
		  ,Target.ComponentName = Source.ComponentName
		  ,Target.ExaminerType = Source.ExaminerType
		  ,Target.Cost = Source.Cost
		  ,Target.PaperLost = Source.PaperLost
		  ,Target.PaperReceived = Source.PaperReceived
		  ,Target.SentToPayroll = Source.SentToPayroll
		  ,Target.PassMark = Source.PassMark
		  ,Target.OverallPassMark = Source.OverallPassMark
		  ,Target.OverallTotalMark = Source.OverallTotalMark

		  ,Target.PrimaryFailureReason = Source.PrimaryFailureReason
		  ,Target.PoorPerformanceReasons = Source.PoorPerformanceReasons
		  ,Target.MarkerComments = Source.MarkerComments
		  ,Target.SubmittedDate = Source.SubmittedDate

		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'

		WHEN NOT MATCHED THEN 
		 INSERT([PersonId],[TestSittingId],[ModifiedDate],[ExaminerNAATINumber],[ExaminerName],[IncludeMarks],[MarkId],[Mark],[TotalMark],[OverallMark],[TestResultId],[ComponentName],[ExaminerType],
			 [Cost],[PaperLost],[PaperReceived],[SentToPayroll],[PassMark],[OverallPassMark],[OverallTotalMark],[PrimaryFailureReason],[PoorPerformanceReasons],[MarkerComments],[SubmittedDate],[RowStatus])	  
		 VALUES (Source.[PersonId],Source.[TestSittingId],@Date,Source.[ExaminerNAATINumber],Source.[ExaminerName],Source.[IncludeMarks],Source.[MarkId],Source.[Mark],Source.[TotalMark],Source.[OverallMark],Source.[TestResultId],Source.[ComponentName],Source.[ExaminerType],
			  Source.[Cost],Source.[PaperLost],Source.[PaperReceived],Source.[SentToPayroll],Source.[PassMark],Source.[OverallPassMark],Source.[OverallTotalMark],Source.[PrimaryFailureReason],Source.[PoorPerformanceReasons],Source.[MarkerComments],Source.[SubmittedDate], 'Latest');
	    	
	COMMIT TRANSACTION;	

END