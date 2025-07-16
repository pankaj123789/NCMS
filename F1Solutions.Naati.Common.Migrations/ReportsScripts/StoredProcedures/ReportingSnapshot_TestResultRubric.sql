ALTER PROCEDURE [dbo].[ReportingSnapshot_TestResultRubric]
	@Date DateTime
AS
BEGIN
DECLARE @TestResultRubricHistory as table([TestResultId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL,[TestSittingId] [int] NULL,[TestSessionId] [int] NULL,[PersonId] [int] NULL,[CustomerNo] [int] NULL,[CandidateName] [nvarchar](252) NULL,[ResultDueDate] [datetime] NULL,
											  [Language1] [nvarchar](50) NULL,[Language1Code] [nvarchar](10) NULL,[Language1Group] [nvarchar](100) NULL,[Language2] [nvarchar](50) NULL,[Language2Code] [nvarchar](10) NULL,[Language2Group] [nvarchar](100) NULL,[Skill] [nvarchar](100) NULL,
											  [PaidReview] [bit] NULL,[Supplementary] [bit] NULL,[OverallResult] [nvarchar](50) NULL,[ResultDate] [datetime] NULL,[CredentialTypeInternalName] [nvarchar](100) NULL,[CredentialTypeExternalName] [nvarchar](100) NULL,[EligibleForSupplementary] [bit] NOT NULL,
											  [EligibleForConcededPass] [bit] NOT NULL, [RowStatus] nvarchar(50))   

		
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @TestResultRubricHistory
	
	SELECT
		 CASE WHEN [Source].[TestResultId] IS NULL THEN [TestResultRubric].[TestResultId] ELSE [Source].[TestResultId] END AS [TestResultId]
		 
		 ,@Date AS [ModifiedDate]
		 
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN [TestResultRubric].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		,CASE WHEN [Source].[TestSessionId] IS NULL THEN [TestResultRubric].[TestSessionId] ELSE [Source].[TestSessionId] END AS [TestSessionId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [TestResultRubric].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNo] IS NULL THEN [TestResultRubric].[CustomerNo] ELSE [Source].[CustomerNo] END AS [CustomerNo]
		,CASE WHEN [Source].[CandidateName] IS NULL THEN [TestResultRubric].[CandidateName] ELSE [Source].[CandidateName] END AS [CandidateName]
		,CASE WHEN [Source].[ResultDueDate] IS NULL THEN [TestResultRubric].[ResultDueDate] ELSE [Source].[ResultDueDate] END AS [ResultDueDate]
		,CASE WHEN [Source].[Language1] IS NULL THEN [TestResultRubric].[Language1] ELSE [Source].[Language1] END AS [Language1]
		,CASE WHEN [Source].[Language1Code] IS NULL THEN [TestResultRubric].[Language1Code] ELSE [Source].[Language1Code] END AS [Language1Code]
		,CASE WHEN [Source].[Language1Group] IS NULL THEN [TestResultRubric].[Language1Group] ELSE [Source].[Language1Group] END AS [Language1Group]
		,CASE WHEN [Source].[Language2] IS NULL THEN [TestResultRubric].[Language2] ELSE [Source].[Language2] END AS [Language2]
		,CASE WHEN [Source].[Language2Code] IS NULL THEN [TestResultRubric].[Language2Code] ELSE [Source].[Language2Code] END AS [Language2Code]
		,CASE WHEN [Source].[Language2Group] IS NULL THEN [TestResultRubric].[Language2Group] ELSE [Source].[Language2Group] END AS [Language2Group]
		,CASE WHEN [Source].[Skill] IS NULL THEN [TestResultRubric].[Skill] ELSE [Source].[Skill] END AS [Skill]
		,CASE WHEN [Source].[PaidReview] IS NULL THEN [TestResultRubric].[PaidReview] ELSE [Source].[PaidReview] END AS [PaidReview]
		,CASE WHEN [Source].[Supplementary] IS NULL THEN [TestResultRubric].[Supplementary] ELSE [Source].[Supplementary] END AS [Supplementary]
		,CASE WHEN [Source].[OverallResult] IS NULL THEN [TestResultRubric].[OverallResult] ELSE [Source].[OverallResult] END AS [OverallResult]
		,CASE WHEN [Source].[ResultDate] IS NULL THEN [TestResultRubric].[ResultDate] ELSE [Source].[ResultDate] END AS [ResultDate]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [TestResultRubric].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [TestResultRubric].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[EligibleForSupplementary] IS NULL THEN [TestResultRubric].[EligibleForSupplementary] ELSE [Source].[EligibleForSupplementary] END AS [EligibleForSupplementary]
		,CASE WHEN [Source].[EligibleForConcededPass] IS NULL THEN [TestResultRubric].[EligibleForConcededPass] ELSE [Source].[EligibleForConcededPass] END AS [EligibleForConcededPass]
		
		,CASE WHEN [Source].TestResultId IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
	FROM
	(
		SELECT
			 tr.[TestResultId]
			,tr.[TestSittingId]
			,ts.[TestSessionId]
			,p.[PersonId]
			,e.[NaatiNumber] CustomerNo
			,(
				SELECT TOP 1 CASE WHEN pn.TitleId IS NULL THEN [GivenName] + ' ' + [SurName] ELSE [Title] + ' ' + [GivenName] + ' ' + [SurName] END
				FROM [naati_db]..[tblPersonName] pn
				LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
				WHERE pn.[PersonId] = p.[PersonId]
				ORDER BY pn.[EffectiveDate] DESC
			) 'CandidateName'
			,j.[DueDate] ResultDueDate

			,ct.InternalName CredentialTypeInternalName
			,ct.ExternalName CredentialTypeExternalName

			,l1.[Name] Language1
			,l1.[Code] Language1Code
			,lg1.[Name] Language1Group
			,l2.[Name] Language2
			,l2.[Code] Language2Code
			,lg2.[Name] Language2Group
			,REPLACE(REPLACE(d.[DisplayName], '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) Skill
			,CAST(CASE WHEN j.[ReviewFromJobId] IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS  PaidReview
			,ts.Supplementary
			,rt.[Result] OverallResult
			,tr.[ProcessedDate] ResultDate
			,tr.EligibleForSupplementary AS 'EligibleForSupplementary'
			,tr.EligibleForConcededPass AS 'EligibleForConcededPass'

		FROM [naati_db]..[tblTestResult] tr
			LEFT JOIN [naati_db]..[tblTestSitting] ts ON tr.[TestSittingId] = ts.[TestSittingId]
			LEFT JOIN [naati_db]..[tblCredentialRequest] cr ON ts.[CredentialRequestId] = cr.[CredentialRequestId]

			LEFT JOIN [naati_db]..[tblCredentialType] ct ON cr.CredentialTypeId = ct.CredentialTypeId

			LEFT JOIN [naati_db]..[tblSkill] s ON cr.[SkillId] = s.[SkillId]
			LEFT JOIN [naati_db]..[tblDirectionType] d ON d.[DirectionTypeId] = s.[DirectionTypeId]
			LEFT JOIN [naati_db]..[tblLanguage] l1 ON s.[Language1Id] = l1.[LanguageId]
			LEFT JOIN [naati_db]..[tblLanguageGroup] lg1 ON l1.[LanguageGroupId] = lg1.[LanguageGroupId]
			LEFT JOIN [naati_db]..[tblLanguage] l2 ON s.[Language2Id] = l2.[LanguageId]
			LEFT JOIN [naati_db]..[tblLanguageGroup] lg2 ON l2.[LanguageGroupId] = lg2.[LanguageGroupId]
			LEFT JOIN [naati_db]..[tblCredentialApplication] a ON cr.[CredentialApplicationId] = a.[CredentialApplicationId]
			LEFT JOIN [naati_db]..[tblJob] j ON tr.[CurrentJobId] = j.[JobId]
			LEFT JOIN [naati_db]..[tblPerson] p ON a.[PersonId] = p.[PersonId]
			LEFT JOIN [naati_db]..[tblEntity] e ON p.[EntityId] = e.[EntityId]
			LEFT JOIN [naati_db]..[tluResultType] rt ON tr.[ResultTypeId] = rt.[ResultTypeId]
			OUTER APPLY [dbo].[GetTestResultOverallMark](tr.[TestResultId]) trom
		WHERE
			-- JUST RUBRIC MARKING
			(SELECT COUNT(1)
			FROM [naati_db]..[tblTestComponentType] tct
				JOIN [naati_db]..[tblRubricMarkingCompetency] rmc ON tct.[TestComponentTypeId] = rmc.[TestComponentTypeId]
			WHERE tct.[TestSpecificationId] = ts.[TestSpecificationId]) > 0
	) [Source]
	FULL OUTER JOIN [TestResultRubric] ON [Source].[TestResultId] = [TestResultRubric].[TestResultId]
	WHERE (([Source].[TestResultId] IS NOT NULL AND [TestResultRubric].[TestResultId] IS NULL) OR ([Source].[TestResultId] IS NULL AND [TestResultRubric].[TestResultId] IS NOT NULL) OR (([Source].[TestResultId] IS NOT NULL AND [TestResultRubric].[TestResultId] IS NOT NULL) AND [Source].[TestResultId] != [TestResultRubric].[TestResultId]))
		OR (([Source].[TestSittingId] IS NOT NULL AND [TestResultRubric].[TestSittingId] IS NULL) OR ([Source].[TestSittingId] IS NULL AND [TestResultRubric].[TestSittingId] IS NOT NULL) OR (([Source].[TestSittingId] IS NOT NULL AND [TestResultRubric].[TestSittingId] IS NOT NULL) AND [Source].[TestSittingId] != [TestResultRubric].[TestSittingId]))
		OR (([Source].[TestSessionId] IS NOT NULL AND [TestResultRubric].[TestSessionId] IS NULL) OR ([Source].[TestSessionId] IS NULL AND [TestResultRubric].[TestSessionId] IS NOT NULL) OR (([Source].[TestSessionId] IS NOT NULL AND [TestResultRubric].[TestSessionId] IS NOT NULL) AND [Source].[TestSessionId] != [TestResultRubric].[TestSessionId]))
		OR (([Source].[PersonId] IS NOT NULL AND [TestResultRubric].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [TestResultRubric].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [TestResultRubric].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [TestResultRubric].[PersonId]))
		OR (([Source].[CustomerNo] IS NOT NULL AND [TestResultRubric].[CustomerNo] IS NULL) OR ([Source].[CustomerNo] IS NULL AND [TestResultRubric].[CustomerNo] IS NOT NULL) OR (([Source].[CustomerNo] IS NOT NULL AND [TestResultRubric].[CustomerNo] IS NOT NULL) AND [Source].[CustomerNo] != [TestResultRubric].[CustomerNo]))
		OR (([Source].[CandidateName] IS NOT NULL AND [TestResultRubric].[CandidateName] IS NULL) OR ([Source].[CandidateName] IS NULL AND [TestResultRubric].[CandidateName] IS NOT NULL) OR (([Source].[CandidateName] IS NOT NULL AND [TestResultRubric].[CandidateName] IS NOT NULL) AND [Source].[CandidateName] != [TestResultRubric].[CandidateName]))

		OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestResultRubric].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [TestResultRubric].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestResultRubric].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [TestResultRubric].[CredentialTypeInternalName]))
		OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestResultRubric].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [TestResultRubric].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestResultRubric].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [TestResultRubric].[CredentialTypeExternalName]))

		OR (([Source].[Language1] IS NOT NULL AND [TestResultRubric].[Language1] IS NULL) OR ([Source].[Language1] IS NULL AND [TestResultRubric].[Language1] IS NOT NULL) OR (([Source].[Language1] IS NOT NULL AND [TestResultRubric].[Language1] IS NOT NULL) AND [Source].[Language1] != [TestResultRubric].[Language1]))
		OR (([Source].[Language1Code] IS NOT NULL AND [TestResultRubric].[Language1Code] IS NULL) OR ([Source].[Language1Code] IS NULL AND [TestResultRubric].[Language1Code] IS NOT NULL) OR (([Source].[Language1Code] IS NOT NULL AND [TestResultRubric].[Language1Code] IS NOT NULL) AND [Source].[Language1Code] != [TestResultRubric].[Language1Code]))
		OR (([Source].[Language1Group] IS NOT NULL AND [TestResultRubric].[Language1Group] IS NULL) OR ([Source].[Language1Group] IS NULL AND [TestResultRubric].[Language1Group] IS NOT NULL) OR (([Source].[Language1Group] IS NOT NULL AND [TestResultRubric].[Language1Group] IS NOT NULL) AND [Source].[Language1Group] != [TestResultRubric].[Language1Group]))
		OR (([Source].[Language2] IS NOT NULL AND [TestResultRubric].[Language2] IS NULL) OR ([Source].[Language2] IS NULL AND [TestResultRubric].[Language2] IS NOT NULL) OR (([Source].[Language2] IS NOT NULL AND [TestResultRubric].[Language2] IS NOT NULL) AND [Source].[Language2] != [TestResultRubric].[Language2]))
		OR (([Source].[Language2Code] IS NOT NULL AND [TestResultRubric].[Language2Code] IS NULL) OR ([Source].[Language2Code] IS NULL AND [TestResultRubric].[Language2Code] IS NOT NULL) OR (([Source].[Language2Code] IS NOT NULL AND [TestResultRubric].[Language2Code] IS NOT NULL) AND [Source].[Language2Code] != [TestResultRubric].[Language2Code]))
		OR (([Source].[Language2Group] IS NOT NULL AND [TestResultRubric].[Language2Group] IS NULL) OR ([Source].[Language2Group] IS NULL AND [TestResultRubric].[Language2Group] IS NOT NULL) OR (([Source].[Language2Group] IS NOT NULL AND [TestResultRubric].[Language2Group] IS NOT NULL) AND [Source].[Language2Group] != [TestResultRubric].[Language2Group]))
		OR (([Source].[Skill] IS NOT NULL AND [TestResultRubric].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [TestResultRubric].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [TestResultRubric].[Skill] IS NOT NULL) AND [Source].[Skill] != [TestResultRubric].[Skill]))
		OR (([Source].[PaidReview] IS NOT NULL AND [TestResultRubric].[PaidReview] IS NULL) OR ([Source].[PaidReview] IS NULL AND [TestResultRubric].[PaidReview] IS NOT NULL) OR (([Source].[PaidReview] IS NOT NULL AND [TestResultRubric].[PaidReview] IS NOT NULL) AND [Source].[PaidReview] != [TestResultRubric].[PaidReview]))
		OR (([Source].[Supplementary] IS NOT NULL AND [TestResultRubric].[Supplementary] IS NULL) OR ([Source].[Supplementary] IS NULL AND [TestResultRubric].[Supplementary] IS NOT NULL) OR (([Source].[Supplementary] IS NOT NULL AND [TestResultRubric].[Supplementary] IS NOT NULL) AND [Source].[Supplementary] != [TestResultRubric].[Supplementary]))
		OR (([Source].[OverallResult] IS NOT NULL AND [TestResultRubric].[OverallResult] IS NULL) OR ([Source].[OverallResult] IS NULL AND [TestResultRubric].[OverallResult] IS NOT NULL) OR (([Source].[OverallResult] IS NOT NULL AND [TestResultRubric].[OverallResult] IS NOT NULL) AND [Source].[OverallResult] != [TestResultRubric].[OverallResult]))
		OR (([Source].[ResultDate] IS NOT NULL AND [TestResultRubric].[ResultDate] IS NULL) OR ([Source].[ResultDate] IS NULL AND [TestResultRubric].[ResultDate] IS NOT NULL) OR (([Source].[ResultDate] IS NOT NULL AND [TestResultRubric].[ResultDate] IS NOT NULL) AND [Source].[ResultDate] != [TestResultRubric].[ResultDate]))
		OR (([Source].[ResultDueDate] IS NOT NULL AND [TestResultRubric].[ResultDueDate] IS NULL) OR ([Source].[ResultDueDate] IS NULL AND [TestResultRubric].[ResultDueDate] IS NOT NULL) OR (([Source].[ResultDueDate] IS NOT NULL AND [TestResultRubric].[ResultDueDate] IS NOT NULL) AND [Source].[ResultDueDate] != [TestResultRubric].[ResultDueDate]))
		OR (([Source].[EligibleForConcededPass] IS NOT NULL AND [TestResultRubric].[EligibleForConcededPass] IS NULL) OR ([Source].[EligibleForConcededPass] IS NULL AND [TestResultRubric].[EligibleForConcededPass] IS NOT NULL) OR (([Source].[EligibleForConcededPass] IS NOT NULL AND [TestResultRubric].[EligibleForConcededPass] IS NOT NULL) AND [Source].[EligibleForConcededPass] != [TestResultRubric].[EligibleForConcededPass]))
		OR (([Source].[EligibleForSupplementary] IS NOT NULL AND [TestResultRubric].[EligibleForSupplementary] IS NULL) OR ([Source].[EligibleForSupplementary] IS NULL AND [TestResultRubric].[EligibleForSupplementary] IS NOT NULL) OR (([Source].[EligibleForSupplementary] IS NOT NULL AND [TestResultRubric].[EligibleForSupplementary] IS NOT NULL) AND [Source].[EligibleForSupplementary] != [TestResultRubric].[EligibleForSupplementary]))

	--select * from @TestResultRubricHistory
		
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE TestResultRubricHistory AS Target USING(select * from @TestResultRubricHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[TestResultId] = Source.[TestResultId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestResultRubricHistory AS Target USING(	select * from @TestResultRubricHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestResultId] = Source.[TestResultId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestResultRubricHistory AS Target USING(	select * from @TestResultRubricHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestResultId] = Source.[TestResultId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestResultId = Source.TestResultId		  
		  ,Target.TestSittingId = Source.TestSittingId
		  ,Target.TestSessionId = Source.TestSessionId
		  ,Target.PersonId = Source.PersonId
		  ,Target.CustomerNo = Source.CustomerNo      
		  ,Target.CandidateName = Source.CandidateName
		  ,Target.ResultDueDate = Source.ResultDueDate
		  ,Target.Language1 = Source.Language1
		  ,Target.Language1Code = Source.Language1Code

		  ,Target.Language1Group = Source.Language1Group
		  ,Target.Language2 = Source.Language2
		  ,Target.Language2Code = Source.Language2Code
		  ,Target.Language2Group = Source.Language2Group
		  ,Target.Skill = Source.Skill
		  ,Target.PaidReview = Source.PaidReview
		  ,Target.Supplementary = Source.Supplementary
		  ,Target.OverallResult = Source.OverallResult
		  ,Target.ResultDate = Source.ResultDate
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName

		  ,Target.EligibleForSupplementary = Source.EligibleForSupplementary
		  ,Target.EligibleForConcededPass = Source.EligibleForConcededPass
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		  INSERT([TestResultId],[ModifiedDate],[TestSittingId],[TestSessionId],[PersonId],[CustomerNo],[CandidateName],[ResultDueDate],[Language1],[Language1Code],[Language1Group],[Language2],[Language2Code],[Language2Group],[Skill],
			 [PaidReview],[Supplementary],[OverallResult],[ResultDate],[CredentialTypeInternalName],[CredentialTypeExternalName],[EligibleForSupplementary],[EligibleForConcededPass],[RowStatus])

		  VALUES (Source.[TestResultId],@Date,Source.[TestSittingId],Source.[TestSessionId],Source.[PersonId],Source.[CustomerNo],Source.[CandidateName],Source.[ResultDueDate],Source.[Language1],Source.[Language1Code],
			  Source.[Language1Group],Source.[Language2],Source.[Language2Code],Source.[Language2Group],Source.[Skill],Source.[PaidReview],Source.[Supplementary],Source.[OverallResult],Source.[ResultDate],
			  Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],Source.[EligibleForSupplementary],Source.[EligibleForConcededPass], 'Latest');

	COMMIT TRANSACTION;	
	
END
