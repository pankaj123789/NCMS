ALTER PROCEDURE [dbo].[ReportingSnapshot_TestResult]
	@Date DateTime
AS
BEGIN
DECLARE @TestResultHistory as table([TestResultId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL,[PersonId] [int] NULL,[TestSittingId] [int] NULL,[ResultDueDate] [datetime] NULL,[LanguageName1] [nvarchar](50) NULL,[CandidateName] [nvarchar](252) NULL,
									[NAATINumber] [int] NULL,[NAATINumberDisplay] [nvarchar](50) NULL,[PaidReview] [bit] NULL,[TotalMarks] [float] NULL,[PassMark] [float] NULL,[TotalCost] [money] NULL,[GeneralComments] [nvarchar](500) NULL,[OverallResult] [nvarchar](50) NULL,
									[ResultDate] [datetime] NULL,[LanguageName2] [nvarchar](50) NOT NULL,[CredentialTypeInternalName] [nvarchar](100) NULL,[CredentialTypeExternalName] [nvarchar](100) NULL,[EligibleForSupplementary] [bit] NOT NULL,[EligibleForConcededPass] [bit] NOT NULL,
									[MarksOverridden] [bit] NOT NULL,[RowStatus] nvarchar(50))   
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @TestResultHistory

	SELECT
		 CASE WHEN [Source].[TestResultId] IS NULL THEN [TestResult].[TestResultId] ELSE [Source].[TestResultId] END AS [TestResultId]
		 
		 ,@Date AS [ModifiedDate]
		 
		,CASE WHEN [Source].[PersonId] IS NULL THEN [TestResult].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN [TestResult].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		,CASE WHEN [Source].[ResultDueDate] IS NULL THEN [TestResult].[ResultDueDate] ELSE [Source].[ResultDueDate] END AS [ResultDueDate]
		,CASE WHEN [Source].[LanguageName1] IS NULL THEN [TestResult].[LanguageName1] ELSE [Source].[LanguageName1] END AS [LanguageName1]
		,CASE WHEN [Source].[CandidateName] IS NULL THEN [TestResult].[CandidateName] ELSE [Source].[CandidateName] END AS [CandidateName]
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [TestResult].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[NaatiNumberDisplay] IS NULL THEN [TestResult].[NaatiNumberDisplay] ELSE [Source].[NaatiNumberDisplay] END AS [NaatiNumberDisplay]
		,CASE WHEN [Source].[PaidReview] IS NULL THEN [TestResult].[PaidReview] ELSE [Source].[PaidReview] END AS [PaidReview]
		,CASE WHEN [Source].[TotalMarks] IS NULL THEN [TestResult].[TotalMarks] ELSE [Source].[TotalMarks] END AS [TotalMarks]
		,CASE WHEN [Source].[PassMark] IS NULL THEN [TestResult].[PassMark] ELSE [Source].[PassMark] END AS [PassMark]
		,CASE WHEN [Source].[TotalCost] IS NULL THEN [TestResult].[TotalCost] ELSE [Source].[TotalCost] END AS [TotalCost]
		,CASE WHEN [Source].[GeneralComments] IS NULL THEN [TestResult].[GeneralComments] ELSE [Source].[GeneralComments] END AS [GeneralComments]
		,CASE WHEN [Source].[OverallResult] IS NULL THEN [TestResult].[OverallResult] ELSE [Source].[OverallResult] END AS [OverallResult]
		,CASE WHEN [Source].[ResultDate] IS NULL THEN [TestResult].[ResultDate] ELSE [Source].[ResultDate] END AS [ResultDate]
		,CASE WHEN [Source].[LanguageName2] IS NULL THEN [TestResult].[LanguageName2] ELSE [Source].[LanguageName2] END AS [LanguageName2]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [TestResult].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [TestResult].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[EligibleForSupplementary] IS NULL THEN [TestResult].[EligibleForSupplementary] ELSE [Source].[EligibleForSupplementary] END AS [EligibleForSupplementary]
		,CASE WHEN [Source].[EligibleForConcededPass] IS NULL THEN [TestResult].[EligibleForConcededPass] ELSE [Source].[EligibleForConcededPass] END AS [EligibleForConcededPass]
		,CASE WHEN [Source].[MarksOverridden] IS NULL THEN [TestResult].[MarksOverridden] ELSE [Source].[MarksOverridden] END AS [MarksOverridden]
		
		,CASE WHEN [Source].[TestResultId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
	FROM
	(
		SELECT
			tr.[TestResultId]
			,p.[PersonId]
			,ts.[TestSittingId] AS 'TestSittingId'
			,j.[DueDate] AS 'ResultDueDate'

			,ct.InternalName CredentialTypeInternalName
			,ct.ExternalName CredentialTypeExternalName

			,l1.[Name] COLLATE Latin1_General_CI_AS AS 'LanguageName1'
			,l2.[Name] COLLATE Latin1_General_CI_AS AS 'LanguageName2'
			,(
				SELECT TOP 1 CASE WHEN pn.TitleId IS NULL THEN [GivenName] + ' ' + [SurName] ELSE [Title] + ' ' + [GivenName] + ' ' + [SurName] END
				FROM [naati_db]..[tblPersonName] pn
				LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
				WHERE pn.[PersonId] = p.[PersonId]
				ORDER BY pn.[EffectiveDate] DESC
			) COLLATE Latin1_General_CI_AS AS 'CandidateName'
			,e.[NAATINumber]
			,CASE WHEN e.[EntityTypeId] = 2 THEN 'NC' + CONVERT(NVARCHAR(20), e.[NAATINumber]) ELSE CONVERT(NVARCHAR(20), e.[NAATINumber]) END AS 'NaatiNumberDisplay'
			,CAST(CASE WHEN j.[ReviewFromJobId] IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS 'PaidReview'
			,trom.[TestResultOverallMark] AS 'TotalMarks'
			,tssms.[OverallPassMark] 'PassMark'
			,j.[JobCost] AS 'TotalCost'
			,tr.[CommentsGeneral] COLLATE Latin1_General_CI_AS AS 'GeneralComments'
			,rt.[Result] COLLATE Latin1_General_CI_AS AS 'OverallResult'
			,tr.[ProcessedDate] AS 'ResultDate'
			,tr.EligibleForSupplementary AS 'EligibleForSupplementary'
			,tr.EligibleForConcededPass AS 'EligibleForConcededPass'
			,CASE WHEN tr.AllowCalculate = 1 THEN 0 ELSE 1 END 'MarksOverridden'

		FROM [naati_db]..[tblTestResult] tr
		LEFT JOIN [naati_db]..[tblTestSitting] ts ON tr.[TestSittingId] = ts.[TestSittingId]
		LEFT JOIN [naati_db]..[tblCredentialRequest] cr ON ts.[CredentialRequestId] = cr.[CredentialRequestId]
		LEFT JOIN [naati_db]..[tblSkill] s ON cr.[SkillId] = s.[SkillId]

		LEFT JOIN [naati_db]..[tblCredentialType] ct ON cr.CredentialTypeId = ct.CredentialTypeId

		LEFT JOIN [naati_db]..[tblLanguage] l1 ON s.[Language1Id] = l1.[LanguageId]
		LEFT JOIN [naati_db]..[tblLanguage] l2 ON s.[Language2Id] = l2.[LanguageId]
		LEFT JOIN [naati_db]..[tblCredentialApplication] a ON cr.[CredentialApplicationId] = a.[CredentialApplicationId]
		LEFT JOIN [naati_db]..[tblJob] j ON tr.[CurrentJobId] = j.[JobId]
		LEFT JOIN [naati_db]..[tblPerson] p ON a.[PersonId] = p.[PersonId]
		LEFT JOIN [naati_db]..[tblEntity] e ON p.[EntityId] = e.[EntityId]
		LEFT JOIN [naati_db]..[tluResultType] rt ON tr.[ResultTypeId] = rt.[ResultTypeId]
		LEFT JOIN [naati_db]..[tblTestSpecification] tss ON cr.[CredentialTypeId] = tss.[CredentialTypeId]
		LEFT JOIN [naati_db]..[tblTestSpecificationStandardMarkingScheme] tssms ON tss.[TestSpecificationId] = tssms.[TestSpecificationId]
		OUTER APPLY [dbo].[GetTestResultOverallMark](tr.[TestResultId]) trom
		WHERE 
		    tss.Active = 1 and --INC186019
            tssms.TestSpecificationStandardMarkingSchemeId is not null and --INC186019. This one knocks out test specs that dont have standard marking scheme

			-- JUST STANDARD MARKING
			(SELECT COUNT(1)
			FROM [naati_db]..[tblTestComponentType] tct
				JOIN [naati_db]..[tblRubricMarkingCompetency] rmc ON tct.[TestComponentTypeId] = rmc.[TestComponentTypeId]
			WHERE tct.[TestSpecificationId] = ts.[TestSpecificationId]) = 0
	) [Source]
	FULL OUTER JOIN [TestResult] ON [Source].[TestResultId] = [TestResult].[TestResultId]
	WHERE (([Source].[PersonId] IS NOT NULL AND [TestResult].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [TestResult].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [TestResult].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [TestResult].[PersonId]))
	OR (([Source].[TestSittingId] IS NOT NULL AND [TestResult].[TestSittingId] IS NULL) OR ([Source].[TestSittingId] IS NULL AND [TestResult].[TestSittingId] IS NOT NULL) OR (([Source].[TestSittingId] IS NOT NULL AND [TestResult].[TestSittingId] IS NOT NULL) AND [Source].[TestSittingId] != [TestResult].[TestSittingId]))
	OR (([Source].[ResultDueDate] IS NOT NULL AND [TestResult].[ResultDueDate] IS NULL) OR ([Source].[ResultDueDate] IS NULL AND [TestResult].[ResultDueDate] IS NOT NULL) OR (([Source].[ResultDueDate] IS NOT NULL AND [TestResult].[ResultDueDate] IS NOT NULL) AND [Source].[ResultDueDate] != [TestResult].[ResultDueDate]))

	OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestResult].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [TestResult].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestResult].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [TestResult].[CredentialTypeInternalName]))
	OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestResult].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [TestResult].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestResult].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [TestResult].[CredentialTypeExternalName]))
	
	OR (([Source].[LanguageName1] IS NOT NULL AND [TestResult].[LanguageName1] IS NULL) OR ([Source].[LanguageName1] IS NULL AND [TestResult].[LanguageName1] IS NOT NULL) OR (([Source].[LanguageName1] IS NOT NULL AND [TestResult].[LanguageName1] IS NOT NULL) AND [Source].[LanguageName1] != [TestResult].[LanguageName1]))
	OR (([Source].[LanguageName2] IS NOT NULL AND [TestResult].[LanguageName2] IS NULL) OR ([Source].[LanguageName2] IS NULL AND [TestResult].[LanguageName2] IS NOT NULL) OR (([Source].[LanguageName2] IS NOT NULL AND [TestResult].[LanguageName2] IS NOT NULL) AND [Source].[LanguageName2] != [TestResult].[LanguageName2]))
	OR (([Source].[CandidateName] IS NOT NULL AND [TestResult].[CandidateName] IS NULL) OR ([Source].[CandidateName] IS NULL AND [TestResult].[CandidateName] IS NOT NULL) OR (([Source].[CandidateName] IS NOT NULL AND [TestResult].[CandidateName] IS NOT NULL) AND [Source].[CandidateName] != [TestResult].[CandidateName]))
	OR (([Source].[NAATINumber] IS NOT NULL AND [TestResult].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [TestResult].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [TestResult].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [TestResult].[NAATINumber]))
	OR (([Source].[NaatiNumberDisplay] IS NOT NULL AND [TestResult].[NaatiNumberDisplay] IS NULL) OR ([Source].[NaatiNumberDisplay] IS NULL AND [TestResult].[NaatiNumberDisplay] IS NOT NULL) OR (([Source].[NaatiNumberDisplay] IS NOT NULL AND [TestResult].[NaatiNumberDisplay] IS NOT NULL) AND [Source].[NaatiNumberDisplay] != [TestResult].[NaatiNumberDisplay]))
	OR (([Source].[PaidReview] IS NOT NULL AND [TestResult].[PaidReview] IS NULL) OR ([Source].[PaidReview] IS NULL AND [TestResult].[PaidReview] IS NOT NULL) OR (([Source].[PaidReview] IS NOT NULL AND [TestResult].[PaidReview] IS NOT NULL) AND [Source].[PaidReview] != [TestResult].[PaidReview]))
	OR (([Source].[TotalMarks] IS NOT NULL AND [TestResult].[TotalMarks] IS NULL) OR ([Source].[TotalMarks] IS NULL AND [TestResult].[TotalMarks] IS NOT NULL) OR (([Source].[TotalMarks] IS NOT NULL AND [TestResult].[TotalMarks] IS NOT NULL) AND [Source].[TotalMarks] != [TestResult].[TotalMarks]))
	OR (([Source].[PassMark] IS NOT NULL AND [TestResult].[PassMark] IS NULL) OR ([Source].[PassMark] IS NULL AND [TestResult].[PassMark] IS NOT NULL) OR (([Source].[PassMark] IS NOT NULL AND [TestResult].[PassMark] IS NOT NULL) AND [Source].[PassMark] != [TestResult].[PassMark]))
	OR (([Source].[TotalCost] IS NOT NULL AND [TestResult].[TotalCost] IS NULL) OR ([Source].[TotalCost] IS NULL AND [TestResult].[TotalCost] IS NOT NULL) OR (([Source].[TotalCost] IS NOT NULL AND [TestResult].[TotalCost] IS NOT NULL) AND [Source].[TotalCost] != [TestResult].[TotalCost]))
	OR (([Source].[GeneralComments] IS NOT NULL AND [TestResult].[GeneralComments] IS NULL) OR ([Source].[GeneralComments] IS NULL AND [TestResult].[GeneralComments] IS NOT NULL) OR (([Source].[GeneralComments] IS NOT NULL AND [TestResult].[GeneralComments] IS NOT NULL) AND [Source].[GeneralComments] != [TestResult].[GeneralComments]))
	OR (([Source].[OverallResult] IS NOT NULL AND [TestResult].[OverallResult] IS NULL) OR ([Source].[OverallResult] IS NULL AND [TestResult].[OverallResult] IS NOT NULL) OR (([Source].[OverallResult] IS NOT NULL AND [TestResult].[OverallResult] IS NOT NULL) AND [Source].[OverallResult] != [TestResult].[OverallResult]))
	OR (([Source].[ResultDate] IS NOT NULL AND [TestResult].[ResultDate] IS NULL) OR ([Source].[ResultDate] IS NULL AND [TestResult].[ResultDate] IS NOT NULL) OR (([Source].[ResultDate] IS NOT NULL AND [TestResult].[ResultDate] IS NOT NULL) AND [Source].[ResultDate] != [TestResult].[ResultDate]))
	OR (([Source].[EligibleForConcededPass] IS NOT NULL AND [TestResult].[EligibleForConcededPass] IS NULL) OR ([Source].[EligibleForConcededPass] IS NULL AND [TestResult].[EligibleForConcededPass] IS NOT NULL) OR (([Source].[EligibleForConcededPass] IS NOT NULL AND [TestResult].[EligibleForConcededPass] IS NOT NULL) AND [Source].[EligibleForConcededPass] != [TestResult].[EligibleForConcededPass]))
	OR (([Source].[EligibleForSupplementary] IS NOT NULL AND [TestResult].[EligibleForSupplementary] IS NULL) OR ([Source].[EligibleForSupplementary] IS NULL AND [TestResult].[EligibleForSupplementary] IS NOT NULL) OR (([Source].[EligibleForSupplementary] IS NOT NULL AND [TestResult].[EligibleForSupplementary] IS NOT NULL) AND [Source].[EligibleForSupplementary] != [TestResult].[EligibleForSupplementary]))
	OR (([Source].[MarksOverridden] IS NOT NULL AND [TestResult].[MarksOverridden] IS NULL) OR ([Source].[MarksOverridden] IS NULL AND [TestResult].[MarksOverridden] IS NOT NULL) OR (([Source].[MarksOverridden] IS NOT NULL AND [TestResult].[MarksOverridden] IS NOT NULL) AND [Source].[MarksOverridden] != [TestResult].[MarksOverridden]))

	--select * from @TestResultHistory
		
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE TestResultHistory AS Target USING(select * from @TestResultHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[TestResultId] = Source.[TestResultId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestResultHistory AS Target USING(	select * from @TestResultHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestResultId] = Source.[TestResultId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestResultHistory AS Target USING(	select * from @TestResultHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestResultId] = Source.[TestResultId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestResultId = Source.TestResultId		  
		  ,Target.PersonId = Source.PersonId
		  ,Target.TestSittingId = Source.TestSittingId
		  ,Target.ResultDueDate = Source.ResultDueDate
		  ,Target.LanguageName1 = Source.LanguageName1      
		  ,Target.CandidateName = Source.CandidateName
		  ,Target.NAATINumber = Source.NAATINumber
		  ,Target.NAATINumberDisplay = Source.NAATINumberDisplay
		  ,Target.PaidReview = Source.PaidReview

		  ,Target.TotalMarks = Source.TotalMarks
		  ,Target.PassMark = Source.PassMark
		  ,Target.TotalCost = Source.TotalCost
		  ,Target.GeneralComments = Source.GeneralComments
		  ,Target.OverallResult = Source.OverallResult
		  ,Target.ResultDate = Source.ResultDate
		  ,Target.LanguageName2 = Source.LanguageName2
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName
		  ,Target.EligibleForSupplementary = Source.EligibleForSupplementary
		  ,Target.EligibleForConcededPass = Source.EligibleForConcededPass

		  ,Target.MarksOverridden = Source.MarksOverridden

		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN
		  INSERT([TestResultId],[ModifiedDate],[PersonId],[TestSittingId],[ResultDueDate],[LanguageName1],[CandidateName],[NAATINumber],[NAATINumberDisplay],[PaidReview],
			 [TotalMarks],[PassMark],[TotalCost],[GeneralComments],[OverallResult],[ResultDate],[LanguageName2],[CredentialTypeInternalName],[CredentialTypeExternalName],
			 [EligibleForSupplementary],[EligibleForConcededPass],[MarksOverridden],[RowStatus])
	  
		VALUES (Source.[TestResultId],@Date,[PersonId],Source.[TestSittingId],Source.[ResultDueDate],Source.[LanguageName1],Source.[CandidateName],Source.[NAATINumber],Source.[NAATINumberDisplay],
			  Source.[PaidReview],Source.[TotalMarks],Source.[PassMark],Source.[TotalCost],Source.[GeneralComments],Source.[OverallResult],Source.[ResultDate],Source.[LanguageName2],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],
			  Source.[EligibleForSupplementary],Source.[EligibleForConcededPass],Source.[MarksOverridden], 'Latest');
	    	
	COMMIT TRANSACTION;	

END
