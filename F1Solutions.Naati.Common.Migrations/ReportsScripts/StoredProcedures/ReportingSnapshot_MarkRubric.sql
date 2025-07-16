ALTER PROCEDURE [dbo].[ReportingSnapshot_MarkRubric]
	@Date DateTime
AS
BEGIN
	
	DECLARE @MarkRubricHistory as table([TestResultId] [int] NOT NULL,[TestComponentId] [int] NOT NULL,[JobExaminerId] [int] NOT NULL,[RubricAssessementCriterionResultId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL,[TestSittingId] [int] NULL,[TestSessionId] [int] NULL,[PersonId] [int] NOT NULL,
									[CustomerNo] [int] NULL,[CandidateName] [nvarchar](252) NULL,[PaidReview] [bit] NULL,[Supplementary] [bit] NULL,[TestTaskTypeLabel] [nvarchar](100) NULL,[TestTaskTypeName] [nvarchar](50) NULL,[TestTaskLabel] [nvarchar](100) NULL,[TestTaskName] [nvarchar](100) NULL,
									[TestTaskNumber] [int] NULL,[MarkType] [nvarchar](100) NULL,[ResultType] [nvarchar](100) NULL,[ExaminerCustomerNo] [int] NULL,[ExaminerName] [nvarchar](252) NULL,[ExaminerType] [nvarchar](100) NULL,[Cost] [money] NULL,[ExaminerSubmittedDate] [datetime] NULL,
									[IncludeMarks] [bit] NULL,[WasAttempted] [bit] NULL,[Successful] [bit] NULL,[RubricCompetencyLabel] [nvarchar](100) NULL,[RubricCompetencyName] [nvarchar](50) NULL,[RubricCriterionName] [nvarchar](100) NULL,[RubricCriterionLabel] [nvarchar](100) NULL,
									[RubricSelectedBandLabel] [nvarchar](100) NULL,[RubricSelectedBandLevel] [int] NULL, [RowStatus] nvarchar(50))

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @MarkRubricHistory

SELECT
		CASE WHEN [Source].[TestResultId] IS NULL THEN [MarkRubric].[TestResultId] ELSE [Source].[TestResultId] END AS [TestResultId]
		,CASE WHEN [Source].[TestComponentId] IS NULL THEN [MarkRubric].[TestComponentId] ELSE [Source].[TestComponentId] END AS [TestComponentId]
		,CASE WHEN [Source].[JobExaminerId] IS NULL THEN [MarkRubric].[JobExaminerId] ELSE [Source].[JobExaminerId] END AS [JobExaminerId]
		,CASE WHEN [Source].[RubricAssessementCriterionResultId] IS NULL THEN [MarkRubric].[RubricAssessementCriterionResultId] ELSE [Source].[RubricAssessementCriterionResultId] END AS [RubricAssessementCriterionResultId]
		
		,@Date AS [ModifiedDate]
		
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN [MarkRubric].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		,CASE WHEN [Source].[TestSessionId] IS NULL THEN [MarkRubric].[TestSessionId] ELSE [Source].[TestSessionId] END AS [TestSessionId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [MarkRubric].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNo] IS NULL THEN [MarkRubric].[CustomerNo] ELSE [Source].[CustomerNo] END AS [CustomerNo]
		,CASE WHEN [Source].[CandidateName] IS NULL THEN [MarkRubric].[CandidateName] ELSE [Source].[CandidateName] END AS [CandidateName]
		,CASE WHEN [Source].[PaidReview] IS NULL THEN [MarkRubric].[PaidReview] ELSE [Source].[PaidReview] END AS [PaidReview]
		,CASE WHEN [Source].[Supplementary] IS NULL THEN [MarkRubric].[Supplementary] ELSE [Source].[Supplementary] END AS [Supplementary]
		,CASE WHEN [Source].[TestTaskTypeLabel] IS NULL THEN [MarkRubric].[TestTaskTypeLabel] ELSE [Source].[TestTaskTypeLabel] END AS [TestTaskTypeLabel]
		,CASE WHEN [Source].[TestTaskTypeName] IS NULL THEN [MarkRubric].[TestTaskTypeName] ELSE [Source].[TestTaskTypeName] END AS [TestTaskTypeName]
		,CASE WHEN [Source].[TestTaskLabel] IS NULL THEN [MarkRubric].[TestTaskLabel] ELSE [Source].[TestTaskLabel] END AS [TestTaskLabel]
		,CASE WHEN [Source].[TestTaskName] IS NULL THEN [MarkRubric].[TestTaskName] ELSE [Source].[TestTaskName] END AS [TestTaskName]
		,CASE WHEN [Source].[TestTaskNumber] IS NULL THEN [MarkRubric].[TestTaskNumber] ELSE [Source].[TestTaskNumber] END AS [TestTaskNumber]
		,CASE WHEN [Source].[MarkType] IS NULL THEN [MarkRubric].[MarkType] ELSE [Source].[MarkType] END AS [MarkType]
		,CASE WHEN [Source].[ResultType] IS NULL THEN [MarkRubric].[ResultType] ELSE [Source].[ResultType] END AS [ResultType]
		,CASE WHEN [Source].[ExaminerCustomerNo] IS NULL THEN [MarkRubric].[ExaminerCustomerNo] ELSE [Source].[ExaminerCustomerNo] END AS [ExaminerCustomerNo]
		,CASE WHEN [Source].[ExaminerName] IS NULL THEN [MarkRubric].[ExaminerName] ELSE [Source].[ExaminerName] END AS [ExaminerName]
		,CASE WHEN [Source].[ExaminerType] IS NULL THEN [MarkRubric].[ExaminerType] ELSE [Source].[ExaminerType] END AS [ExaminerType]
		,CASE WHEN [Source].[Cost] IS NULL THEN [MarkRubric].[Cost] ELSE [Source].[Cost] END AS [Cost]
		,CASE WHEN [Source].[ExaminerSubmittedDate] IS NULL THEN [MarkRubric].[ExaminerSubmittedDate] ELSE [Source].[ExaminerSubmittedDate] END AS [ExaminerSubmittedDate]
		,CASE WHEN [Source].[IncludeMarks] IS NULL THEN [MarkRubric].[IncludeMarks] ELSE [Source].[IncludeMarks] END AS [IncludeMarks]
		,CASE WHEN [Source].[WasAttempted] IS NULL THEN [MarkRubric].[WasAttempted] ELSE [Source].[WasAttempted] END AS [WasAttempted]
		,CASE WHEN [Source].[Successful] IS NULL THEN [MarkRubric].[Successful] ELSE [Source].[Successful] END AS [Successful]
		,CASE WHEN [Source].[RubricCompetencyLabel] IS NULL THEN [MarkRubric].[RubricCompetencyLabel] ELSE [Source].[RubricCompetencyLabel] END AS [RubricCompetencyLabel]
		,CASE WHEN [Source].[RubricCompetencyName] IS NULL THEN [MarkRubric].[RubricCompetencyName] ELSE [Source].[RubricCompetencyName] END AS [RubricCompetencyName]
		,CASE WHEN [Source].[RubricCriterionName] IS NULL THEN [MarkRubric].[RubricCriterionName] ELSE [Source].[RubricCriterionName] END AS [RubricCriterionName]
		,CASE WHEN [Source].[RubricCriterionLabel] IS NULL THEN [MarkRubric].[RubricCriterionLabel] ELSE [Source].[RubricCriterionLabel] END AS [RubricCriterionLabel]
		,CASE WHEN [Source].[RubricSelectedBandLabel] IS NULL THEN [MarkRubric].[RubricSelectedBandLabel] ELSE [Source].[RubricSelectedBandLabel] END AS [RubricSelectedBandLabel]
		,CASE WHEN [Source].[RubricSelectedBandLevel] IS NULL THEN [MarkRubric].[RubricSelectedBandLevel] ELSE [Source].[RubricSelectedBandLevel] END AS [RubricSelectedBandLevel]
		
		,CASE WHEN [Source].[TestResultId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM
	(
		SELECT
			trm.[TestResultId]
			,trm.[TestComponentId]
			,ISNULL(je.JobExaminerId, 0) JobExaminerId
			,ISNULL(rtcr.[RubricAssessementCriterionResultId], 0) RubricAssessementCriterionResultId
			,trm.[TestSittingId] 
			,trm.[TestSessionId] 
			,trm.[PersonId]
			,trm.[CustomerNo]
			,trm.[CandidateName]
			,trm.[PaidReview]
			,trm.Supplementary
			,trm.TestTaskTypeLabel
			,trm.TestTaskTypeName
			,trm.TestTaskLabel
			,trm.TestTaskName
			,trm.TestTaskNumber
			,'Examiner' MarkType
			,
			CASE 
				WHEN rmrt.MarkingResultTypeId = 1 AND trm.Supplementary = 1 Then 'Supplementary Result'				
				ELSE rmrt.DisplayName
			END ResultType
			, (
				SELECT TOP 1
					[NAATINumber]
				FROM [naati_db]..[tblEntity]
				WHERE [EntityId] = 
				(
					SELECT TOP 1 EntityId
					FROM [naati_db]..[tblPerson]
					WHERE [PersonId] = pm.[PersonId]
				)
			) ExaminerCustomerNo
			,(
				SELECT TOP 1
					[Title] + ' ' + [GivenName] + ' ' + [SurName]
				FROM [naati_db]..[tblPersonName] pn
				LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
				WHERE pn.[PersonId] = pm.[PersonId]
				ORDER BY pn.[EffectiveDate] DESC
			) ExaminerName
			,CASE 
				WHEN je.ThirdExaminer = 1 THEN 'Third Marker'
				WHEN je.PaidReviewer = 1 THEN 'Paid Reviewer'
				ELSE 'Original'
			END ExaminerType
			,je.ExaminerCost Cost
			,em.SubmittedDate ExaminerSubmittedDate
			,0 IncludeMarks
			,rtcr.WasAttempted WasAttempted
			,rtcr.Successful Successful
			,rtcr.RubricCompetencyLabel
			,rtcr.RubricCompetencyName
			,rtcr.RubricCriterionName
			,rtcr.RubricCriterionLabel
			,rtcr.RubricSelectedBandLabel
			,rtcr.RubricSelectedBandLevel
			
		FROM TestRubricComponent trm 
			INNER JOIN [naati_db]..[tblJobExaminer] je ON trm.[JobId] = je.[JobId]
			INNER JOIN (
				[naati_db]..[tblJobExaminerRubricTestComponentResult] jertcr
				INNER JOIN RubricTestComponentResult rtcr ON jertcr.[RubricTestComponentResultId] = rtcr.[RubricTestComponentResultId]
				INNER JOIN [naati_db]..[tblMarkingResultType] rmrt on rtcr.MarkingResultTypeId = rmrt.MarkingResultTypeId
			) ON je.[JobExaminerID] = jertcr.[JobExaminerId]
				AND trm.[TestComponentId] = rtcr.[TestComponentId]
			LEFT JOIN [naati_db]..[tblExaminerMarking] em ON em.[JobExaminerId] = je.[JobExaminerId]
			LEFT JOIN [naati_db]..[tblPanelMembership] pm ON pm.[PanelMembershipId] = je.[PanelMembershipId]
		UNION
		SELECT 
			trm.[TestResultId]
			,trm.[TestComponentId]
			,0 JobExaminerId
			,ISNULL(rtcr.[RubricAssessementCriterionResultId], 0) RubricAssessementCriterionResultId
			,trm.[TestSittingId] 
			,trm.[TestSessionId] 
			,trm.[PersonId]
			,trm.[CustomerNo]
			,trm.[CandidateName]
			,trm.[PaidReview]
			,trm.Supplementary
			,trm.TestTaskTypeLabel
			,trm.TestTaskTypeName
			,trm.TestTaskLabel
			,trm.TestTaskName
			,trm.TestTaskNumber
			,'Final' MarkType
			,CASE 
				WHEN rmrt.MarkingResultTypeId = 1 AND trm.Supplementary = 1 Then 'Supplementary Result'				
				ELSE rmrt.DisplayName
			END ResultType
			,NULL ExaminerCustomerNo
			,NULL ExaminerName
			,NULL ExaminerType
			,NULL Cost
			,NULL ExaminerSubmittedDate
			,1 IncludeMarks
			,rtcr.WasAttempted WasAttempted
			,rtcr.Successful Successful
			,rtcr.RubricCompetencyLabel
			,rtcr.RubricCompetencyName
			,rtcr.RubricCriterionName
			,rtcr.RubricCriterionLabel
			,rtcr.RubricSelectedBandLabel
			,rtcr.RubricSelectedBandLevel
						
		FROM TestRubricComponent trm 
			LEFT JOIN (
				[naati_db]..[tblTestResultRubricTestComponentResult] trrtcr
				INNER JOIN RubricTestComponentResult rtcr ON trrtcr.[RubricTestComponentResultId] = rtcr.[RubricTestComponentResultId]
				INNER JOIN [naati_db]..[tblMarkingResultType] rmrt on rtcr.MarkingResultTypeId = rmrt.MarkingResultTypeId
			) ON trm.[TestResultId] = trrtcr.[TestResultId]
				AND trm.[TestComponentId] = rtcr.[TestComponentId]
	) [Source]
	FULL OUTER JOIN [MarkRubric] ON [Source].[TestResultId] = [MarkRubric].[TestResultId]
		AND [Source].[TestComponentId] = [MarkRubric].[TestComponentId]
		AND ISNULL([Source].[JobExaminerId], 0) = ISNULL([MarkRubric].[JobExaminerId], 0)
		AND ISNULL([Source].[RubricAssessementCriterionResultId], 0) = ISNULL([MarkRubric].[RubricAssessementCriterionResultId], 0)
	WHERE (([Source].[TestResultId] IS NOT NULL AND [MarkRubric].[TestResultId] IS NULL) OR ([Source].[TestResultId] IS NULL AND [MarkRubric].[TestResultId] IS NOT NULL) OR (([Source].[TestResultId] IS NOT NULL AND [MarkRubric].[TestResultId] IS NOT NULL) AND [Source].[TestResultId] != [MarkRubric].[TestResultId]))
		OR (([Source].[TestComponentId] IS NOT NULL AND [MarkRubric].[TestComponentId] IS NULL) OR ([Source].[TestComponentId] IS NULL AND [MarkRubric].[TestComponentId] IS NOT NULL) OR (([Source].[TestComponentId] IS NOT NULL AND [MarkRubric].[TestComponentId] IS NOT NULL) AND [Source].[TestComponentId] != [MarkRubric].[TestComponentId]))
		OR (([Source].[JobExaminerId] IS NOT NULL AND [MarkRubric].[JobExaminerId] IS NULL) OR ([Source].[JobExaminerId] IS NULL AND [MarkRubric].[JobExaminerId] IS NOT NULL) OR (([Source].[JobExaminerId] IS NOT NULL AND [MarkRubric].[JobExaminerId] IS NOT NULL) AND [Source].[JobExaminerId] != [MarkRubric].[JobExaminerId]))
		OR (([Source].[RubricAssessementCriterionResultId] IS NOT NULL AND [MarkRubric].[RubricAssessementCriterionResultId] IS NULL) OR ([Source].[RubricAssessementCriterionResultId] IS NULL AND [MarkRubric].[RubricAssessementCriterionResultId] IS NOT NULL) OR (([Source].[RubricAssessementCriterionResultId] IS NOT NULL AND [MarkRubric].[RubricAssessementCriterionResultId] IS NOT NULL) AND [Source].[RubricAssessementCriterionResultId] != [MarkRubric].[RubricAssessementCriterionResultId]))
		OR (([Source].[TestSittingId] IS NOT NULL AND [MarkRubric].[TestSittingId] IS NULL) OR ([Source].[TestSittingId] IS NULL AND [MarkRubric].[TestSittingId] IS NOT NULL) OR (([Source].[TestSittingId] IS NOT NULL AND [MarkRubric].[TestSittingId] IS NOT NULL) AND [Source].[TestSittingId] != [MarkRubric].[TestSittingId]))
		OR (([Source].[TestSessionId] IS NOT NULL AND [MarkRubric].[TestSessionId] IS NULL) OR ([Source].[TestSessionId] IS NULL AND [MarkRubric].[TestSessionId] IS NOT NULL) OR (([Source].[TestSessionId] IS NOT NULL AND [MarkRubric].[TestSessionId] IS NOT NULL) AND [Source].[TestSessionId] != [MarkRubric].[TestSessionId]))
		OR (([Source].[PersonId] IS NOT NULL AND [MarkRubric].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [MarkRubric].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [MarkRubric].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [MarkRubric].[PersonId]))
		OR (([Source].[CustomerNo] IS NOT NULL AND [MarkRubric].[CustomerNo] IS NULL) OR ([Source].[CustomerNo] IS NULL AND [MarkRubric].[CustomerNo] IS NOT NULL) OR (([Source].[CustomerNo] IS NOT NULL AND [MarkRubric].[CustomerNo] IS NOT NULL) AND [Source].[CustomerNo] != [MarkRubric].[CustomerNo]))
		OR (([Source].[CandidateName] IS NOT NULL AND [MarkRubric].[CandidateName] IS NULL) OR ([Source].[CandidateName] IS NULL AND [MarkRubric].[CandidateName] IS NOT NULL) OR (([Source].[CandidateName] IS NOT NULL AND [MarkRubric].[CandidateName] IS NOT NULL) AND [Source].[CandidateName] != [MarkRubric].[CandidateName]))
		OR (([Source].[PaidReview] IS NOT NULL AND [MarkRubric].[PaidReview] IS NULL) OR ([Source].[PaidReview] IS NULL AND [MarkRubric].[PaidReview] IS NOT NULL) OR (([Source].[PaidReview] IS NOT NULL AND [MarkRubric].[PaidReview] IS NOT NULL) AND [Source].[PaidReview] != [MarkRubric].[PaidReview]))
		OR (([Source].[Supplementary] IS NOT NULL AND [MarkRubric].[Supplementary] IS NULL) OR ([Source].[Supplementary] IS NULL AND [MarkRubric].[Supplementary] IS NOT NULL) OR (([Source].[Supplementary] IS NOT NULL AND [MarkRubric].[Supplementary] IS NOT NULL) AND [Source].[Supplementary] != [MarkRubric].[Supplementary]))
		OR (([Source].[TestTaskTypeLabel] IS NOT NULL AND [MarkRubric].[TestTaskTypeLabel] IS NULL) OR ([Source].[TestTaskTypeLabel] IS NULL AND [MarkRubric].[TestTaskTypeLabel] IS NOT NULL) OR (([Source].[TestTaskTypeLabel] IS NOT NULL AND [MarkRubric].[TestTaskTypeLabel] IS NOT NULL) AND [Source].[TestTaskTypeLabel] != [MarkRubric].[TestTaskTypeLabel]))
		OR (([Source].[TestTaskTypeName] IS NOT NULL AND [MarkRubric].[TestTaskTypeName] IS NULL) OR ([Source].[TestTaskTypeName] IS NULL AND [MarkRubric].[TestTaskTypeName] IS NOT NULL) OR (([Source].[TestTaskTypeName] IS NOT NULL AND [MarkRubric].[TestTaskTypeName] IS NOT NULL) AND [Source].[TestTaskTypeName] != [MarkRubric].[TestTaskTypeName]))
		OR (([Source].[TestTaskLabel] IS NOT NULL AND [MarkRubric].[TestTaskLabel] IS NULL) OR ([Source].[TestTaskLabel] IS NULL AND [MarkRubric].[TestTaskLabel] IS NOT NULL) OR (([Source].[TestTaskLabel] IS NOT NULL AND [MarkRubric].[TestTaskLabel] IS NOT NULL) AND [Source].[TestTaskLabel] != [MarkRubric].[TestTaskLabel]))
		OR (([Source].[TestTaskName] IS NOT NULL AND [MarkRubric].[TestTaskName] IS NULL) OR ([Source].[TestTaskName] IS NULL AND [MarkRubric].[TestTaskName] IS NOT NULL) OR (([Source].[TestTaskName] IS NOT NULL AND [MarkRubric].[TestTaskName] IS NOT NULL) AND [Source].[TestTaskName] != [MarkRubric].[TestTaskName]))
		OR (([Source].[TestTaskNumber] IS NOT NULL AND [MarkRubric].[TestTaskNumber] IS NULL) OR ([Source].[TestTaskNumber] IS NULL AND [MarkRubric].[TestTaskNumber] IS NOT NULL) OR (([Source].[TestTaskNumber] IS NOT NULL AND [MarkRubric].[TestTaskNumber] IS NOT NULL) AND [Source].[TestTaskNumber] != [MarkRubric].[TestTaskNumber]))
		OR (([Source].[MarkType] IS NOT NULL AND [MarkRubric].[MarkType] IS NULL) OR ([Source].[MarkType] IS NULL AND [MarkRubric].[MarkType] IS NOT NULL) OR (([Source].[MarkType] IS NOT NULL AND [MarkRubric].[MarkType] IS NOT NULL) AND [Source].[MarkType] != [MarkRubric].[MarkType]))
		OR (([Source].[ResultType] IS NOT NULL AND [MarkRubric].[ResultType] IS NULL) OR ([Source].[ResultType] IS NULL AND [MarkRubric].[ResultType] IS NOT NULL) OR (([Source].[ResultType] IS NOT NULL AND [MarkRubric].[ResultType] IS NOT NULL) AND [Source].[ResultType] != [MarkRubric].[ResultType]))
		OR (([Source].[ExaminerCustomerNo] IS NOT NULL AND [MarkRubric].[ExaminerCustomerNo] IS NULL) OR ([Source].[ExaminerCustomerNo] IS NULL AND [MarkRubric].[ExaminerCustomerNo] IS NOT NULL) OR (([Source].[ExaminerCustomerNo] IS NOT NULL AND [MarkRubric].[ExaminerCustomerNo] IS NOT NULL) AND [Source].[ExaminerCustomerNo] != [MarkRubric].[ExaminerCustomerNo]))
		OR (([Source].[ExaminerName] IS NOT NULL AND [MarkRubric].[ExaminerName] IS NULL) OR ([Source].[ExaminerName] IS NULL AND [MarkRubric].[ExaminerName] IS NOT NULL) OR (([Source].[ExaminerName] IS NOT NULL AND [MarkRubric].[ExaminerName] IS NOT NULL) AND [Source].[ExaminerName] != [MarkRubric].[ExaminerName]))
		OR (([Source].[ExaminerType] IS NOT NULL AND [MarkRubric].[ExaminerType] IS NULL) OR ([Source].[ExaminerType] IS NULL AND [MarkRubric].[ExaminerType] IS NOT NULL) OR (([Source].[ExaminerType] IS NOT NULL AND [MarkRubric].[ExaminerType] IS NOT NULL) AND [Source].[ExaminerType] != [MarkRubric].[ExaminerType]))
		OR (([Source].[Cost] IS NOT NULL AND [MarkRubric].[Cost] IS NULL) OR ([Source].[Cost] IS NULL AND [MarkRubric].[Cost] IS NOT NULL) OR (([Source].[Cost] IS NOT NULL AND [MarkRubric].[Cost] IS NOT NULL) AND [Source].[Cost] != [MarkRubric].[Cost]))
		OR (([Source].[ExaminerSubmittedDate] IS NOT NULL AND [MarkRubric].[ExaminerSubmittedDate] IS NULL) OR ([Source].[ExaminerSubmittedDate] IS NULL AND [MarkRubric].[ExaminerSubmittedDate] IS NOT NULL) OR (([Source].[ExaminerSubmittedDate] IS NOT NULL AND [MarkRubric].[ExaminerSubmittedDate] IS NOT NULL) AND [Source].[ExaminerSubmittedDate] != [MarkRubric].[ExaminerSubmittedDate]))
		OR (([Source].[IncludeMarks] IS NOT NULL AND [MarkRubric].[IncludeMarks] IS NULL) OR ([Source].[IncludeMarks] IS NULL AND [MarkRubric].[IncludeMarks] IS NOT NULL) OR (([Source].[IncludeMarks] IS NOT NULL AND [MarkRubric].[IncludeMarks] IS NOT NULL) AND [Source].[IncludeMarks] != [MarkRubric].[IncludeMarks]))
		OR (([Source].[WasAttempted] IS NOT NULL AND [MarkRubric].[WasAttempted] IS NULL) OR ([Source].[WasAttempted] IS NULL AND [MarkRubric].[WasAttempted] IS NOT NULL) OR (([Source].[WasAttempted] IS NOT NULL AND [MarkRubric].[WasAttempted] IS NOT NULL) AND [Source].[WasAttempted] != [MarkRubric].[WasAttempted]))
		OR (([Source].[Successful] IS NOT NULL AND [MarkRubric].[Successful] IS NULL) OR ([Source].[Successful] IS NULL AND [MarkRubric].[Successful] IS NOT NULL) OR (([Source].[Successful] IS NOT NULL AND [MarkRubric].[Successful] IS NOT NULL) AND [Source].[Successful] != [MarkRubric].[Successful]))
		OR (([Source].[RubricCompetencyLabel] IS NOT NULL AND [MarkRubric].[RubricCompetencyLabel] IS NULL) OR ([Source].[RubricCompetencyLabel] IS NULL AND [MarkRubric].[RubricCompetencyLabel] IS NOT NULL) OR (([Source].[RubricCompetencyLabel] IS NOT NULL AND [MarkRubric].[RubricCompetencyLabel] IS NOT NULL) AND [Source].[RubricCompetencyLabel] != [MarkRubric].[RubricCompetencyLabel]))
		OR (([Source].[RubricCompetencyName] IS NOT NULL AND [MarkRubric].[RubricCompetencyName] IS NULL) OR ([Source].[RubricCompetencyName] IS NULL AND [MarkRubric].[RubricCompetencyName] IS NOT NULL) OR (([Source].[RubricCompetencyName] IS NOT NULL AND [MarkRubric].[RubricCompetencyName] IS NOT NULL) AND [Source].[RubricCompetencyName] != [MarkRubric].[RubricCompetencyName]))
		OR (([Source].[RubricCriterionName] IS NOT NULL AND [MarkRubric].[RubricCriterionName] IS NULL) OR ([Source].[RubricCriterionName] IS NULL AND [MarkRubric].[RubricCriterionName] IS NOT NULL) OR (([Source].[RubricCriterionName] IS NOT NULL AND [MarkRubric].[RubricCriterionName] IS NOT NULL) AND [Source].[RubricCriterionName] != [MarkRubric].[RubricCriterionName]))
		OR (([Source].[RubricCriterionLabel] IS NOT NULL AND [MarkRubric].[RubricCriterionLabel] IS NULL) OR ([Source].[RubricCriterionLabel] IS NULL AND [MarkRubric].[RubricCriterionLabel] IS NOT NULL) OR (([Source].[RubricCriterionLabel] IS NOT NULL AND [MarkRubric].[RubricCriterionLabel] IS NOT NULL) AND [Source].[RubricCriterionLabel] != [MarkRubric].[RubricCriterionLabel]))
		OR (([Source].[RubricSelectedBandLabel] IS NOT NULL AND [MarkRubric].[RubricSelectedBandLabel] IS NULL) OR ([Source].[RubricSelectedBandLabel] IS NULL AND [MarkRubric].[RubricSelectedBandLabel] IS NOT NULL) OR (([Source].[RubricSelectedBandLabel] IS NOT NULL AND [MarkRubric].[RubricSelectedBandLabel] IS NOT NULL) AND [Source].[RubricSelectedBandLabel] != [MarkRubric].[RubricSelectedBandLabel]))
		OR (([Source].[RubricSelectedBandLevel] IS NOT NULL AND [MarkRubric].[RubricSelectedBandLevel] IS NULL) OR ([Source].[RubricSelectedBandLevel] IS NULL AND [MarkRubric].[RubricSelectedBandLevel] IS NOT NULL) OR (([Source].[RubricSelectedBandLevel] IS NOT NULL AND [MarkRubric].[RubricSelectedBandLevel] IS NOT NULL) AND [Source].[RubricSelectedBandLevel] != [MarkRubric].[RubricSelectedBandLevel]))

		
		--select * from @MarkRubricHistory

		BEGIN TRANSACTION
		   --Merge operation delete
			MERGE MarkRubricHistory AS Target USING(select * from @MarkRubricHistory where [RowStatus] = 'Deleted') AS Source ON (Target.[TestResultId] = Source.[TestResultId] And Target.[TestComponentId] = Source.[TestComponentId] AND Target.[JobExaminerId] = Source.[JobExaminerId] AND Target.[RubricAssessementCriterionResultId] = Source.[RubricAssessementCriterionResultId] AND Target.RowStatus = 'Latest')
			WHEN MATCHED THEN 
			  UPDATE Set     
			  Target.[ObsoletedDate] =  @Date,
			  Target.[RowStatus] = 'Deleted';

			--Merge operation Obsolete
			MERGE MarkRubricHistory AS Target USING(select * from @MarkRubricHistory where [RowStatus] = 'NewOrModified') AS Source ON (Target.[TestResultId] = Source.[TestResultId] AND Target.[TestComponentId] = Source.[TestComponentId] AND Target.[JobExaminerId] = Source.[JobExaminerId] AND Target.[RubricAssessementCriterionResultId] = Source.[RubricAssessementCriterionResultId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
			WHEN MATCHED THEN 
			  UPDATE Set     		  
			  Target.[ObsoletedDate] =  @Date,
			  Target.[RowStatus] = 'Obsolete';

			--Merge operation Latest
			MERGE MarkRubricHistory AS Target USING(select * from @MarkRubricHistory where [RowStatus] = 'NewOrModified') AS Source ON (Target.[TestResultId] = Source.[TestResultId] AND Target.[TestComponentId] = Source.[TestComponentId] AND Target.[JobExaminerId] = Source.[JobExaminerId] AND Target.[RubricAssessementCriterionResultId] = Source.[RubricAssessementCriterionResultId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
			WHEN MATCHED THEN 
			  UPDATE Set     
		       Target.TestResultId = Source.TestResultId
			  ,Target.TestComponentId = Source.TestComponentId
			  ,Target.JobExaminerId = Source.JobExaminerId
			  ,Target.RubricAssessementCriterionResultId = Source.RubricAssessementCriterionResultId			  
			  ,Target.TestSittingId = Source.TestSittingId      
			  ,Target.TestSessionId = Source.TestSessionId
			  ,Target.PersonId = Source.PersonId
			  ,Target.CustomerNo = Source.CustomerNo
			  ,Target.CandidateName = Source.CandidateName
	  
			  ,Target.PaidReview = Source.PaidReview
			  ,Target.Supplementary = Source.Supplementary
			  ,Target.TestTaskTypeLabel = Source.TestTaskTypeLabel
			  ,Target.TestTaskTypeName = Source.TestTaskTypeName
			  ,Target.TestTaskLabel = Source.TestTaskLabel
			  ,Target.TestTaskName = Source.TestTaskName
			  ,Target.TestTaskNumber = Source.TestTaskNumber
			  ,Target.MarkType = Source.MarkType
			  ,Target.ResultType = Source.ResultType
			  ,Target.ExaminerCustomerNo = Source.ExaminerCustomerNo
			  ,Target.ExaminerName = Source.ExaminerName
			  ,Target.ExaminerType = Source.ExaminerType
	  
			  ,Target.Cost = Source.Cost
			  ,Target.ExaminerSubmittedDate = Source.ExaminerSubmittedDate
			  ,Target.IncludeMarks = Source.IncludeMarks
			  ,Target.WasAttempted = Source.WasAttempted
			  ,Target.Successful = Source.Successful
			  ,Target.RubricCompetencyLabel = Source.RubricCompetencyLabel
			  ,Target.RubricCompetencyName = Source.RubricCompetencyName
			  ,Target.RubricCriterionName = Source.RubricCriterionName
			  ,Target.RubricCriterionLabel = Source.RubricCriterionLabel
	  
			  ,Target.RubricSelectedBandLabel = Source.RubricSelectedBandLabel
			  ,Target.RubricSelectedBandLevel = Source.RubricSelectedBandLevel

			  ,Target.ModifiedDate = @Date
			  ,Target.[RowStatus] = 'Latest'
			WHEN NOT MATCHED THEN 
			 INSERT([TestResultId],[TestComponentId],[JobExaminerId],[RubricAssessementCriterionResultId],[ModifiedDate],[TestSittingId],[TestSessionId],[PersonId],[CustomerNo],[CandidateName],[PaidReview],
			 [Supplementary],[TestTaskTypeLabel],[TestTaskTypeName],[TestTaskLabel],[TestTaskName],[TestTaskNumber],[MarkType],[ResultType],[ExaminerCustomerNo],[ExaminerName],[ExaminerType],[Cost],[ExaminerSubmittedDate],
			 [IncludeMarks],[WasAttempted],[Successful],[RubricCompetencyLabel],[RubricCompetencyName],[RubricCriterionName],[RubricCriterionLabel],[RubricSelectedBandLabel],[RubricSelectedBandLevel],[RowStatus])
	  	  
			 VALUES (Source.[TestResultId],Source.[TestComponentId],Source.[JobExaminerId],Source.[RubricAssessementCriterionResultId], @Date, Source.[TestSittingId],Source.[TestSessionId],Source.[PersonId],Source.[CustomerNo],
			  Source.[CandidateName],[PaidReview],Source.[Supplementary],Source.[TestTaskTypeLabel],Source.[TestTaskTypeName],Source.[TestTaskLabel],Source.[TestTaskName],Source.[TestTaskNumber],Source.[MarkType],Source.[ResultType],
			  Source.[ExaminerCustomerNo],Source.[ExaminerName],Source.[ExaminerType],Source.[Cost],Source.[ExaminerSubmittedDate],Source.[IncludeMarks],Source.[WasAttempted],Source.[Successful],Source.[RubricCompetencyLabel],
			  Source.[RubricCompetencyName],Source.[RubricCriterionName],Source.[RubricCriterionLabel],Source.[RubricSelectedBandLabel],Source.[RubricSelectedBandLevel], 'Latest');

	COMMIT TRANSACTION;	
		
END