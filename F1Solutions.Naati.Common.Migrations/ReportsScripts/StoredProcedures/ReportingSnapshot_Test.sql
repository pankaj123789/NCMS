ALTER PROCEDURE ReportingSnapshot_Test
	@Date DateTime
AS
BEGIN
DECLARE @TestHistory as table([TestSittingId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL,[CredentialRequestId] [int] NULL,[TestResultId] [int] NULL,[CredentialApplicationId] [int] NOT NULL,[PersonId] [int] NOT NULL,[Rejected] [bit] NOT NULL,[Sat] [bit] NULL,[ResultType] [varchar](50) NULL,
							  [ThirdExaminerRequired] [bit] NULL,[ProcessedDate] [datetime] NULL,[SatDate] [datetime] NULL,[ResultChecked] [bit] NULL,[Venue] [nvarchar](100) NOT NULL,[CandidateName] [nvarchar](252) NULL,[LanguageName1] [nvarchar](50) NULL,[LanguageName2] [nvarchar](50) NOT NULL,
							  [CredentialTypeInternalName] [nvarchar](50) NULL,[CredentialTypeExternalName] [nvarchar](50) NULL,[Skill] [nvarchar](100) NULL,[ApplicationType] [nvarchar](50) NULL,[SupplementaryTest] [bit] NOT NULL, [RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @TestHistory
	SELECT 
	     DISTINCT
		 CASE WHEN [Source].[TestSittingId] IS NULL THEN [Test].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		 
		 ,@Date AS [ModifiedDate]
		 		 
		,CASE WHEN [Source].[CredentialRequestId] IS NULL THEN [Test].[CredentialRequestId] ELSE [Source].[CredentialRequestId] END AS [CredentialRequestId]
		,CASE WHEN [Source].[TestResultId] IS NULL THEN [Test].[TestResultId] ELSE [Source].[TestResultId] END AS [TestResultId]
		,CASE WHEN [Source].[CredentialApplicationId] IS NULL THEN [Test].[CredentialApplicationId] ELSE [Source].[CredentialApplicationId] END AS [CredentialApplicationId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [Test].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[Rejected] IS NULL THEN [Test].[Rejected] ELSE [Source].[Rejected] END AS [Rejected]
		,CASE WHEN [Source].[Sat] IS NULL THEN [Test].[Sat] ELSE [Source].[Sat] END AS [Sat]
		,CASE WHEN [Source].[ResultType] IS NULL THEN [Test].[ResultType] ELSE [Source].[ResultType] END AS [ResultType]
		,CASE WHEN [Source].[ThirdExaminerRequired] IS NULL THEN [Test].[ThirdExaminerRequired] ELSE [Source].[ThirdExaminerRequired] END AS [ThirdExaminerRequired]
		,CASE WHEN [Source].[ProcessedDate] IS NULL THEN [Test].[ProcessedDate] ELSE [Source].[ProcessedDate] END AS [ProcessedDate]
		,CASE WHEN [Source].[SatDate] IS NULL THEN [Test].[SatDate] ELSE [Source].[SatDate] END AS [SatDate]
		,CASE WHEN [Source].[ResultChecked] IS NULL THEN [Test].[ResultChecked] ELSE [Source].[ResultChecked] END AS [ResultChecked]
		,CASE WHEN [Source].[Venue] IS NULL THEN [Test].[Venue] ELSE [Source].[Venue] END AS [Venue]
		,CASE WHEN [Source].[CandidateName] IS NULL THEN [Test].[CandidateName] ELSE [Source].[CandidateName] END AS [CandidateName]
		,CASE WHEN [Source].[LanguageName1] IS NULL THEN [Test].[LanguageName1] ELSE [Source].[LanguageName1] END AS [LanguageName1]
		,CASE WHEN [Source].[LanguageName2] IS NULL THEN [Test].[LanguageName2] ELSE [Source].[LanguageName2] END AS [LanguageName2]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [Test].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [Test].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[Skill] IS NULL THEN [Test].[Skill] ELSE [Source].[Skill] END AS [Skill]
		,CASE WHEN [Source].[ApplicationType] IS NULL THEN [Test].[ApplicationType] ELSE [Source].[ApplicationType] END AS [ApplicationType]
		,CASE WHEN [Source].[SupplementaryTest] IS NULL THEN [Test].[SupplementaryTest] ELSE [Source].[SupplementaryTest] END AS [SupplementaryTest]
		
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM
	(
		SELECT
			ts.[TestSittingId]
			,tr.[TestResultId]
			,cr.[CredentialRequestId]
			,ca.[CredentialApplicationId]
			,ca.[PersonId]
			,(
				SELECT TOP 1 CASE WHEN pn.TitleId IS NULL THEN [GivenName] + ' ' + [SurName] ELSE [Title] + ' ' + [GivenName] + ' ' + [SurName] END AS [CandidateName]
				FROM [naati_db]..[tblPersonName] pn
				LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
				WHERE pn.[PersonId] = ca.[PersonId]
				ORDER BY pn.[EffectiveDate] DESC
			) COLLATE Latin1_General_CI_AS AS 'CandidateName'
			,l1.[Name] COLLATE Latin1_General_CI_AS AS 'LanguageName1'
			,l2.[Name] COLLATE Latin1_General_CI_AS AS 'LanguageName2'
			,ts.[Rejected]
			,ts.[Sat]
			,rt.[Result] COLLATE Latin1_General_CI_AS AS 'ResultType'
			,tr.[ThirdExaminerRequired]
			,tr.[ProcessedDate]
			,CASE 
				WHEN ts.[Sat] = 1 Then tss.[TestDateTime]				
				ELSE NULL
			END 'SatDate'
			,tr.[ResultChecked]
			,tv.[Name] COLLATE Latin1_General_CI_AS AS 'Venue'

			,ct.InternalName as 'CredentialTypeInternalName'
			,ct.ExternalName as 'CredentialTypeExternalName'
			,REPLACE(REPLACE(d.[DisplayName], '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) COLLATE Latin1_General_CI_AS as 'Skill'
			,cat.DisplayName ApplicationType
			
			,ts.[Supplementary] SupplementaryTest			

		FROM [naati_db]..[tblTestSitting] ts
		LEFT JOIN [naati_db]..[tblTestSession] tss ON ts.[TestSessionId] = tss.[TestSessionId]
		LEFT JOIN [naati_db]..[tblVenue] tv ON tss.[VenueId] = tv.[VenueId]
		LEFT JOIN [naati_db]..[tblTestResult] tr ON tr.[TestSittingId] = ts.[TestSittingId]
		LEFT JOIN [naati_db]..[tblJob] j ON tr.[CurrentJobId] = j.[JobId]
		LEFT JOIN [naati_db]..[tluResultType] rt ON rt.[ResultTypeId] = tr.[ResultTypeId]
		LEFT JOIN [naati_db]..[tblCredentialRequest] cr ON ts.[CredentialRequestId] = cr.[CredentialRequestId]
		LEFT JOIN [naati_db]..[tblCredentialApplication] ca ON cr.[CredentialApplicationId] = ca.[CredentialApplicationId]
		LEFT JOIN [naati_db]..[tblCredentialApplicationType] cat ON ca.CredentialApplicationTypeId = cat.CredentialApplicationTypeId
		LEFT JOIN [naati_db]..[tblCredentialType] ct ON cr.CredentialTypeId = ct.CredentialTypeId
		LEFT JOIN [naati_db]..[tblSkill] s ON cr.[SkillId] = s.[SkillId]
		LEFT JOIN [naati_db]..[tblSkillType] st ON s.SkillTypeId = st.SkillTypeId
		LEFT JOIN [naati_db]..[tblLanguage] l1 ON l1.[LanguageId] = s.Language1Id
		LEFT JOIN [naati_db]..[tblLanguage] l2 ON l2.[LanguageId] = s.Language2Id
		LEFT JOIN [naati_db]..[tblDirectionType] d ON d.[DirectionTypeId] = s.[DirectionTypeId]
	) [Source]
	FULL OUTER JOIN [TestHistory] AS [Test] ON 	[Source].[TestSittingId] = [Test].[TestSittingId] AND [Source].[CredentialRequestId] = [Test].[CredentialRequestId]	AND ISNULL([Source].[TestResultId],0) = ISNULL([Test].[TestResultId],0)

	WHERE ([Test].[RowStatus] = 'Latest' OR [Test].[RowStatus] IS NULL)
	AND (
			([Source].[CredentialRequestId] IS NOT NULL AND [Test].[CredentialRequestId] IS NULL) OR 
			([Source].[CredentialRequestId] IS NULL AND [Test].[CredentialRequestId] IS NOT NULL) OR 
			(([Source].[CredentialRequestId] IS NOT NULL AND [Test].[CredentialRequestId] IS NOT NULL) AND ([Source].[CredentialRequestId] != [Test].[CredentialRequestId])) 
			
			OR

			(([Source].[TestResultId] IS NOT NULL AND [Test].[TestResultId] IS NULL) OR ([Source].[TestResultId] IS NULL AND [Test].[TestResultId] IS NOT NULL) OR 
			(([Source].[TestResultId] IS NOT NULL AND [Test].[TestResultId] IS NOT NULL) AND [Source].[TestResultId] != [Test].[TestResultId])) 
			
			OR

			(([Source].[CredentialApplicationId] IS NOT NULL AND [Test].[CredentialApplicationId] IS NULL) OR ([Source].[CredentialApplicationId] IS NULL AND [Test].[CredentialApplicationId] IS NOT NULL) OR 
			(([Source].[CredentialApplicationId] IS NOT NULL AND [Test].[CredentialApplicationId] IS NOT NULL) AND [Source].[CredentialApplicationId] != [Test].[CredentialApplicationId])) 
			
			OR

			(([Source].[PersonId] IS NOT NULL AND [Test].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [Test].[PersonId] IS NOT NULL) OR 
			(([Source].[PersonId] IS NOT NULL AND [Test].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [Test].[PersonId]))
	
			OR 
	
			(([Source].[CandidateName] IS NOT NULL AND [Test].[CandidateName] IS NULL) OR ([Source].[CandidateName] IS NULL AND [Test].[CandidateName] IS NOT NULL) OR 
			(([Source].[CandidateName] IS NOT NULL AND [Test].[CandidateName] IS NOT NULL) AND [Source].[CandidateName] != [Test].[CandidateName]))

			OR 
	
			(([Source].[LanguageName1] IS NOT NULL AND [Test].[LanguageName1] IS NULL) OR ([Source].[LanguageName1] IS NULL AND [Test].[LanguageName1] IS NOT NULL) OR 
			(([Source].[LanguageName1] IS NOT NULL AND [Test].[LanguageName1] IS NOT NULL) AND [Source].[LanguageName1] != [Test].[LanguageName1]))

			OR 
	
			(([Source].[LanguageName2] IS NOT NULL AND [Test].[LanguageName2] IS NULL) OR ([Source].[LanguageName2] IS NULL AND [Test].[LanguageName2] IS NOT NULL) OR 
			(([Source].[LanguageName2] IS NOT NULL AND [Test].[LanguageName2] IS NOT NULL) AND [Source].[LanguageName2] != [Test].[LanguageName2]))
	
			OR 
	
			(([Source].[Rejected] IS NOT NULL AND [Test].[Rejected] IS NULL) OR ([Source].[Rejected] IS NULL AND [Test].[Rejected] IS NOT NULL) OR (([Source].[Rejected] IS NOT NULL AND [Test].[Rejected] IS NOT NULL) AND [Source].[Rejected] != [Test].[Rejected]))
	OR 
	
			(([Source].[Sat] IS NOT NULL AND [Test].[Sat] IS NULL) OR ([Source].[Sat] IS NULL AND [Test].[Sat] IS NOT NULL) OR 
			(([Source].[Sat] IS NOT NULL AND [Test].[Sat] IS NOT NULL) AND [Source].[Sat] != [Test].[Sat]))
	OR 
	
	(([Source].[ResultType] IS NOT NULL AND [Test].[ResultType] IS NULL) OR ([Source].[ResultType] IS NULL AND [Test].[ResultType] IS NOT NULL) OR (([Source].[ResultType] IS NOT NULL AND [Test].[ResultType] IS NOT NULL) AND [Source].[ResultType] != [Test].[ResultType]))
	OR 
	
	(([Source].[ThirdExaminerRequired] IS NOT NULL AND [Test].[ThirdExaminerRequired] IS NULL) OR ([Source].[ThirdExaminerRequired] IS NULL AND [Test].[ThirdExaminerRequired] IS NOT NULL) OR 
	(([Source].[ThirdExaminerRequired] IS NOT NULL AND [Test].[ThirdExaminerRequired] IS NOT NULL) AND [Source].[ThirdExaminerRequired] != [Test].[ThirdExaminerRequired]))
	OR 
	
	(([Source].[ProcessedDate] IS NOT NULL AND [Test].[ProcessedDate] IS NULL) OR ([Source].[ProcessedDate] IS NULL AND [Test].[ProcessedDate] IS NOT NULL) OR 
	(([Source].[ProcessedDate] IS NOT NULL AND [Test].[ProcessedDate] IS NOT NULL) AND [Source].[ProcessedDate] != [Test].[ProcessedDate]))
	
	OR 
	
	(([Source].[SatDate] IS NOT NULL AND [Test].[SatDate] IS NULL) OR ([Source].[SatDate] IS NULL AND [Test].[SatDate] IS NOT NULL) OR 
	(([Source].[SatDate] IS NOT NULL AND [Test].[SatDate] IS NOT NULL) AND [Source].[SatDate] != [Test].[SatDate]))
	
	OR 
	
	(([Source].[ResultChecked] IS NOT NULL AND [Test].[ResultChecked] IS NULL) OR ([Source].[ResultChecked] IS NULL AND [Test].[ResultChecked] IS NOT NULL) OR 
	(([Source].[ResultChecked] IS NOT NULL AND [Test].[ResultChecked] IS NOT NULL) AND [Source].[ResultChecked] != [Test].[ResultChecked]))
	
	OR 
	
	(([Source].[Venue] IS NOT NULL AND [Test].[Venue] IS NULL) OR ([Source].[Venue] IS NULL AND [Test].[Venue] IS NOT NULL) OR 
	(([Source].[Venue] IS NOT NULL AND [Test].[Venue] IS NOT NULL) AND [Source].[Venue] != [Test].[Venue]))

	OR 
	
	(([Source].[CredentialTypeInternalName] IS NOT NULL AND [Test].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [Test].[CredentialTypeInternalName] IS NOT NULL) OR
	(([Source].[CredentialTypeInternalName] IS NOT NULL AND [Test].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [Test].[CredentialTypeInternalName]))
	
	OR 
	
	(([Source].[CredentialTypeExternalName] IS NOT NULL AND [Test].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [Test].[CredentialTypeExternalName] IS NOT NULL) OR 
	(([Source].[CredentialTypeExternalName] IS NOT NULL AND [Test].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [Test].[CredentialTypeExternalName]))
	
	OR 
	
	(([Source].[Skill] IS NOT NULL AND [Test].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [Test].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [Test].[Skill] IS NOT NULL) AND 
	[Source].[Skill] != [Test].[Skill]))
	
	
	OR 
	
	(([Source].[ApplicationType] IS NOT NULL AND [Test].[ApplicationType] IS NULL) OR ([Source].[ApplicationType] IS NULL AND [Test].[ApplicationType] IS NOT NULL) OR 
	(([Source].[ApplicationType] IS NOT NULL AND [Test].[ApplicationType] IS NOT NULL) AND [Source].[ApplicationType] != [Test].[ApplicationType]))
	
	OR 
	
	(([Source].[SupplementaryTest] IS NOT NULL AND [Test].[SupplementaryTest] IS NULL) OR 
	([Source].[SupplementaryTest] IS NULL AND [Test].[SupplementaryTest] IS NOT NULL) OR (([Source].[SupplementaryTest] IS NOT NULL AND 
	[Test].[SupplementaryTest] IS NOT NULL) AND [Source].[SupplementaryTest] != [Test].[SupplementaryTest]))
	
	)

	--select * from @TestHistory

	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE TestHistory AS Target USING(select * from @TestHistory where [RowStatus] = 'Deleted' ) AS Source ON (Target.[TestSittingId] = Source.[TestSittingId] AND Target.[CredentialRequestId] = Source.[CredentialRequestId] And (ISNULL(Target.[TestResultId],0) = ISNULL(Source.[TestResultId],0)) AND Target.RowStatus = 'Latest')
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestHistory AS Target USING(	select * from @TestHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.[TestSittingId] = Source.[TestSittingId] AND Target.[CredentialRequestId] = Source.[CredentialRequestId] And (ISNULL(Target.[TestResultId],0) = ISNULL(Source.[TestResultId],0)) AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestHistory AS Target USING(	select * from @TestHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.[TestSittingId] = Source.[TestSittingId] AND Target.[CredentialRequestId] = Source.[CredentialRequestId] And (ISNULL(Target.[TestResultId],0) = ISNULL(Source.[TestResultId],0)) AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestSittingId = Source.TestSittingId		  
		  ,Target.CredentialRequestId = Source.CredentialRequestId
		  ,Target.TestResultId = Source.TestResultId
		  ,Target.CredentialApplicationId = Source.CredentialApplicationId
		  ,Target.PersonId = Source.PersonId      
		  ,Target.Rejected = Source.Rejected
		  ,Target.Sat = Source.Sat
		  ,Target.ResultType = Source.ResultType
		  ,Target.ThirdExaminerRequired = Source.ThirdExaminerRequired

		  ,Target.ProcessedDate = Source.ProcessedDate
		  ,Target.SatDate = Source.SatDate
		  ,Target.ResultChecked = Source.ResultChecked
		  ,Target.Venue = Source.Venue
		  ,Target.CandidateName = Source.CandidateName
		  ,Target.LanguageName1 = Source.LanguageName1
		  ,Target.LanguageName2 = Source.LanguageName2
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName
		  ,Target.Skill = Source.Skill

		  ,Target.ApplicationType = Source.ApplicationType
		  ,Target.SupplementaryTest = Source.SupplementaryTest

		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([TestSittingId],[ModifiedDate],[CredentialRequestId],[TestResultId],[CredentialApplicationId],[PersonId],[Rejected],[Sat],[ResultType],[ThirdExaminerRequired],[ProcessedDate],[SatDate],[ResultChecked],[Venue],[CandidateName],
			 [LanguageName1],[LanguageName2],[CredentialTypeInternalName],[CredentialTypeExternalName],[Skill],[ApplicationType],[SupplementaryTest],[RowStatus])
	  
		 VALUES (Source.[TestSittingId],@Date,Source.[CredentialRequestId],Source.[TestResultId],Source.[CredentialApplicationId],Source.[PersonId],Source.[Rejected],Source.[Sat],Source.[ResultType],Source.[ThirdExaminerRequired],
			  Source.[ProcessedDate],Source.[SatDate],Source.[ResultChecked],Source.[Venue],Source.[CandidateName],Source.[LanguageName1],Source.[LanguageName2],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],
			  Source.[Skill],Source.[ApplicationType],Source.[SupplementaryTest], 'Latest');

	COMMIT TRANSACTION;	

END