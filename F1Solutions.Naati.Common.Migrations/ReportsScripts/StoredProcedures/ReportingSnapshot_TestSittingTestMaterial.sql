ALTER PROCEDURE [dbo].[ReportingSnapshot_TestSittingTestMaterial]
		@Date DateTime
AS
BEGIN

DECLARE @TestSittingTestMaterialHistory as table([TestSittingTestMaterialId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL,[TestSittingId] [int] NULL,[TestSessionId] [int] NULL,[TestMaterialId] [int] NULL,[SatDate] [datetime] NULL,[TestLocationName] [nvarchar](1000) NULL,
												 [TestLocationState] [nvarchar](50) NULL,[CredentialRequestId] [int] NULL,[CredentialApplicationId] [int] NULL,[PersonId] [int] NULL,[CustomerNo] [int] NULL,[CandidateName] [nvarchar](252) NULL,[Language1] [nvarchar](50) NULL,
												 [Language1Code] [nvarchar](10) NULL,[Language1Group] [nvarchar](100) NULL,[Language2] [nvarchar](50) NULL,[Language2Code] [nvarchar](10) NULL,[Language2Group] [nvarchar](100) NULL,[Skill] [nvarchar](100) NULL,
												 [TestMaterialTitle] [nvarchar](510) NULL,[TestTaskTypeLabel] [nvarchar](100) NULL,[TestTaskTypeName] [nvarchar](50) NULL,[TestTaskLabel] [nvarchar](100) NULL,[TestTaskName] [nvarchar](100) NULL,
												 [RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @TestSittingTestMaterialHistory

	SELECT
		 CASE WHEN [Source].[TestSittingTestMaterialId] IS NULL THEN [TestSittingTestMaterial].[TestSittingTestMaterialId] ELSE [Source].[TestSittingTestMaterialId] END AS [TestSittingTestMaterialId]
		 
		 ,@Date AS [ModifiedDate]
		 
		 
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN [TestSittingTestMaterial].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		,CASE WHEN [Source].[TestSessionId] IS NULL THEN [TestSittingTestMaterial].[TestSessionId] ELSE [Source].[TestSessionId] END AS [TestSessionId]
		,CASE WHEN [Source].[TestMaterialId] IS NULL THEN [TestSittingTestMaterial].[TestMaterialId] ELSE [Source].[TestMaterialId] END AS [TestMaterialId]
		,CASE WHEN [Source].[SatDate] IS NULL THEN [TestSittingTestMaterial].[SatDate] ELSE [Source].[SatDate] END AS [SatDate]
		,CASE WHEN [Source].[TestLocationName] IS NULL THEN [TestSittingTestMaterial].[TestLocationName] ELSE [Source].[TestLocationName] END AS [TestLocationName]
		,CASE WHEN [Source].[TestLocationState] IS NULL THEN [TestSittingTestMaterial].[TestLocationState] ELSE [Source].[TestLocationState] END AS [TestLocationState]
		,CASE WHEN [Source].[CredentialRequestId] IS NULL THEN [TestSittingTestMaterial].[CredentialRequestId] ELSE [Source].[CredentialRequestId] END AS [CredentialRequestId]
		,CASE WHEN [Source].[CredentialApplicationId] IS NULL THEN [TestSittingTestMaterial].[CredentialApplicationId] ELSE [Source].[CredentialApplicationId] END AS [CredentialApplicationId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [TestSittingTestMaterial].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNo] IS NULL THEN [TestSittingTestMaterial].[CustomerNo] ELSE [Source].[CustomerNo] END AS [CustomerNo]
		,CASE WHEN [Source].[CandidateName] IS NULL THEN [TestSittingTestMaterial].[CandidateName] ELSE [Source].[CandidateName] END AS [CandidateName]
		,CASE WHEN [Source].[Language1] IS NULL THEN [TestSittingTestMaterial].[Language1] ELSE [Source].[Language1] END AS [Language1]
		,CASE WHEN [Source].[Language1Code] IS NULL THEN [TestSittingTestMaterial].[Language1Code] ELSE [Source].[Language1Code] END AS [Language1Code]
		,CASE WHEN [Source].[Language1Group] IS NULL THEN [TestSittingTestMaterial].[Language1Group] ELSE [Source].[Language1Group] END AS [Language1Group]
		,CASE WHEN [Source].[Language2] IS NULL THEN [TestSittingTestMaterial].[Language2] ELSE [Source].[Language2] END AS [Language2]
		,CASE WHEN [Source].[Language2Code] IS NULL THEN [TestSittingTestMaterial].[Language2Code] ELSE [Source].[Language2Code] END AS [Language2Code]
		,CASE WHEN [Source].[Language2Group] IS NULL THEN [TestSittingTestMaterial].[Language2Group] ELSE [Source].[Language2Group] END AS [Language2Group]
		,CASE WHEN [Source].[Skill] IS NULL THEN [TestSittingTestMaterial].[Skill] ELSE [Source].[Skill] END AS [Skill]
		,CASE WHEN [Source].[TestMaterialTitle] IS NULL THEN [TestSittingTestMaterial].[TestMaterialTitle] ELSE [Source].[TestMaterialTitle] END AS [TestMaterialTitle]
		,CASE WHEN [Source].[TestTaskTypeLabel] IS NULL THEN [TestSittingTestMaterial].[TestTaskTypeLabel] ELSE [Source].[TestTaskTypeLabel] END AS [TestTaskTypeLabel]
		,CASE WHEN [Source].[TestTaskTypeName] IS NULL THEN [TestSittingTestMaterial].[TestTaskTypeName] ELSE [Source].[TestTaskTypeName] END AS [TestTaskTypeName]
		,CASE WHEN [Source].[TestTaskLabel] IS NULL THEN [TestSittingTestMaterial].[TestTaskLabel] ELSE [Source].[TestTaskLabel] END AS [TestTaskLabel]
		,CASE WHEN [Source].[TestTaskName] IS NULL THEN [TestSittingTestMaterial].[TestTaskName] ELSE [Source].[TestTaskName] END AS [TestTaskName]

		,CASE WHEN [Source].[TestSittingTestMaterialId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
	FROM
	(
		SELECT
			tsm.[TestSittingTestMaterialId]
			,tsm.[TestSittingId]
			,ts.[TestSessionId]
			,tsm.[TestMaterialId]
			,CASE 
				WHEN ts.[Sat] = 1 Then tss.[TestDateTime]				
				ELSE NULL
			END 'SatDate'
			,tl.[Name] TestLocationName
			,s.[Name] TestLocationState
			,cr.[CredentialRequestId]
			,cr.[CredentialApplicationId]
			,p.[PersonId]
			,e.[NaatiNumber] CustomerNo
			,(
				SELECT TOP 1 CASE WHEN pn.TitleId IS NULL THEN [GivenName] + ' ' + [SurName] ELSE [Title] + ' ' + [GivenName] + ' ' + [SurName] END
				FROM [naati_db]..[tblPersonName] pn
				LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
				WHERE pn.[PersonId] = p.[PersonId]
				ORDER BY pn.[EffectiveDate] DESC
			) CandidateName
			,l1.[Name] Language1
			,l1.[Code] Language1Code
			,lg1.[Name] Language1Group
			,l2.[Name] Language2
			,l2.[Code] Language2Code
			,lg2.[Name] Language2Group
			,REPLACE(REPLACE(d.[DisplayName], '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) Skill
			,tm.[Title] [TestMaterialTitle]
			,tct.[Label] TestTaskTypeLabel
			,tct.[Name] TestTaskTypeName
			,tc.[ComponentNumber] TestTaskLabel
			,tc.[Name] TestTaskName

		FROM [naati_db]..[tblTestSittingTestMaterial] tsm
			LEFT JOIN [naati_db]..[tblTestSitting] ts ON tsm.[TestSittingId] = ts.[TestSittingId]
			LEFT JOIN [naati_db]..[tblTestSession] tss ON ts.[TestSessionId] = tss.[TestSessionId]
			LEFT JOIN [naati_db]..[tblVenue] v ON tss.[VenueId] = v.[VenueId]
			LEFT JOIN [naati_db]..[tblTestLocation] tl ON v.[TestLocationId] = tl.[TestLocationId]
			LEFT JOIN [naati_db]..[tblOffice] o ON tl.[OfficeId] = o.[OfficeId]
			LEFT JOIN [naati_db]..[tluState] s ON o.[StateId] = s.[StateId]
			LEFT JOIN [naati_db]..[tblCredentialRequest] cr ON ts.[CredentialRequestId] = cr.[CredentialRequestId]
			LEFT JOIN [naati_db]..[tblCredentialApplication] ca ON cr.CredentialApplicationId = ca.CredentialApplicationId
			LEFT JOIN [naati_db]..[tblPerson] p ON ca.PersonId = p.PersonId
			LEFT JOIN [naati_db]..[tblEntity] e ON p.EntityId = e.EntityId
			LEFT JOIN [naati_db]..[tblSkill] sk ON cr.[SkillId] = sk.[SkillId]
			LEFT JOIN [naati_db]..[tblDirectionType] d ON sk.[DirectionTypeId] = d.[DirectionTypeId]
			LEFT JOIN [naati_db]..[tblLanguage] l1 ON sk.[Language1Id] = l1.[LanguageId]
			LEFT JOIN [naati_db]..[tblLanguageGroup] lg1 ON l1.[LanguageGroupId] = lg1.[LanguageGroupId]
			LEFT JOIN [naati_db]..[tblLanguage] l2 ON sk.[Language2Id] = l2.[LanguageId]
			LEFT JOIN [naati_db]..[tblLanguageGroup] lg2 ON l2.[LanguageGroupId] = lg2.[LanguageGroupId]
			LEFT JOIN [naati_db]..[tblTestComponent] tc ON tsm.[TestComponentId] = tc.[TestComponentId]
			LEFT JOIN [naati_db]..[tblTestComponentType] tct ON tc.[TypeId] = tct.[TestComponentTypeId] 
			LEFT JOIN [naati_db]..[tblTestMaterial] tm ON tsm.[TestMaterialId] = tm.[TestMaterialId] 
	) [Source]
	FULL OUTER JOIN [TestSittingTestMaterial] ON [Source].[TestSittingTestMaterialId] = [TestSittingTestMaterial].[TestSittingTestMaterialId]
	WHERE 
		(([Source].[TestSittingTestMaterialId] IS NOT NULL AND [TestSittingTestMaterial].[TestSittingTestMaterialId] IS NULL) OR ([Source].[TestSittingTestMaterialId] IS NULL AND [TestSittingTestMaterial].[TestSittingTestMaterialId] IS NOT NULL) OR (([Source].[TestSittingTestMaterialId] IS NOT NULL AND [TestSittingTestMaterial].[TestSittingTestMaterialId] IS NOT NULL) AND [Source].[TestSittingTestMaterialId] != [TestSittingTestMaterial].[TestSittingTestMaterialId]))
		OR (([Source].[TestSittingId] IS NOT NULL AND [TestSittingTestMaterial].[TestSittingId] IS NULL) OR ([Source].[TestSittingId] IS NULL AND [TestSittingTestMaterial].[TestSittingId] IS NOT NULL) OR (([Source].[TestSittingId] IS NOT NULL AND [TestSittingTestMaterial].[TestSittingId] IS NOT NULL) AND [Source].[TestSittingId] != [TestSittingTestMaterial].[TestSittingId]))
		OR (([Source].[TestSessionId] IS NOT NULL AND [TestSittingTestMaterial].[TestSessionId] IS NULL) OR ([Source].[TestSessionId] IS NULL AND [TestSittingTestMaterial].[TestSessionId] IS NOT NULL) OR (([Source].[TestSessionId] IS NOT NULL AND [TestSittingTestMaterial].[TestSessionId] IS NOT NULL) AND [Source].[TestSessionId] != [TestSittingTestMaterial].[TestSessionId]))
		OR (([Source].[TestMaterialId] IS NOT NULL AND [TestSittingTestMaterial].[TestMaterialId] IS NULL) OR ([Source].[TestMaterialId] IS NULL AND [TestSittingTestMaterial].[TestMaterialId] IS NOT NULL) OR (([Source].[TestMaterialId] IS NOT NULL AND [TestSittingTestMaterial].[TestMaterialId] IS NOT NULL) AND [Source].[TestMaterialId] != [TestSittingTestMaterial].[TestMaterialId]))
		OR (([Source].[SatDate] IS NOT NULL AND [TestSittingTestMaterial].[SatDate] IS NULL) OR ([Source].[SatDate] IS NULL AND [TestSittingTestMaterial].[SatDate] IS NOT NULL) OR (([Source].[SatDate] IS NOT NULL AND [TestSittingTestMaterial].[SatDate] IS NOT NULL) AND [Source].[SatDate] != [TestSittingTestMaterial].[SatDate]))
		OR (([Source].[TestLocationName] IS NOT NULL AND [TestSittingTestMaterial].[TestLocationName] IS NULL) OR ([Source].[TestLocationName] IS NULL AND [TestSittingTestMaterial].[TestLocationName] IS NOT NULL) OR (([Source].[TestLocationName] IS NOT NULL AND [TestSittingTestMaterial].[TestLocationName] IS NOT NULL) AND [Source].[TestLocationName] != [TestSittingTestMaterial].[TestLocationName]))
		OR (([Source].[TestLocationState] IS NOT NULL AND [TestSittingTestMaterial].[TestLocationState] IS NULL) OR ([Source].[TestLocationState] IS NULL AND [TestSittingTestMaterial].[TestLocationState] IS NOT NULL) OR (([Source].[TestLocationState] IS NOT NULL AND [TestSittingTestMaterial].[TestLocationState] IS NOT NULL) AND [Source].[TestLocationState] != [TestSittingTestMaterial].[TestLocationState]))
		OR (([Source].[CredentialRequestId] IS NOT NULL AND [TestSittingTestMaterial].[CredentialRequestId] IS NULL) OR ([Source].[CredentialRequestId] IS NULL AND [TestSittingTestMaterial].[CredentialRequestId] IS NOT NULL) OR (([Source].[CredentialRequestId] IS NOT NULL AND [TestSittingTestMaterial].[CredentialRequestId] IS NOT NULL) AND [Source].[CredentialRequestId] != [TestSittingTestMaterial].[CredentialRequestId]))
		OR (([Source].[CredentialApplicationId] IS NOT NULL AND [TestSittingTestMaterial].[CredentialApplicationId] IS NULL) OR ([Source].[CredentialApplicationId] IS NULL AND [TestSittingTestMaterial].[CredentialApplicationId] IS NOT NULL) OR (([Source].[CredentialApplicationId] IS NOT NULL AND [TestSittingTestMaterial].[CredentialApplicationId] IS NOT NULL) AND [Source].[CredentialApplicationId] != [TestSittingTestMaterial].[CredentialApplicationId]))
		OR (([Source].[PersonId] IS NOT NULL AND [TestSittingTestMaterial].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [TestSittingTestMaterial].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [TestSittingTestMaterial].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [TestSittingTestMaterial].[PersonId]))
		OR (([Source].[CustomerNo] IS NOT NULL AND [TestSittingTestMaterial].[CustomerNo] IS NULL) OR ([Source].[CustomerNo] IS NULL AND [TestSittingTestMaterial].[CustomerNo] IS NOT NULL) OR (([Source].[CustomerNo] IS NOT NULL AND [TestSittingTestMaterial].[CustomerNo] IS NOT NULL) AND [Source].[CustomerNo] != [TestSittingTestMaterial].[CustomerNo]))
		OR (([Source].[CandidateName] IS NOT NULL AND [TestSittingTestMaterial].[CandidateName] IS NULL) OR ([Source].[CandidateName] IS NULL AND [TestSittingTestMaterial].[CandidateName] IS NOT NULL) OR (([Source].[CandidateName] IS NOT NULL AND [TestSittingTestMaterial].[CandidateName] IS NOT NULL) AND [Source].[CandidateName] != [TestSittingTestMaterial].[CandidateName]))
		OR (([Source].[Language1] IS NOT NULL AND [TestSittingTestMaterial].[Language1] IS NULL) OR ([Source].[Language1] IS NULL AND [TestSittingTestMaterial].[Language1] IS NOT NULL) OR (([Source].[Language1] IS NOT NULL AND [TestSittingTestMaterial].[Language1] IS NOT NULL) AND [Source].[Language1] != [TestSittingTestMaterial].[Language1]))
		OR (([Source].[Language1Code] IS NOT NULL AND [TestSittingTestMaterial].[Language1Code] IS NULL) OR ([Source].[Language1Code] IS NULL AND [TestSittingTestMaterial].[Language1Code] IS NOT NULL) OR (([Source].[Language1Code] IS NOT NULL AND [TestSittingTestMaterial].[Language1Code] IS NOT NULL) AND [Source].[Language1Code] != [TestSittingTestMaterial].[Language1Code]))
		OR (([Source].[Language1Group] IS NOT NULL AND [TestSittingTestMaterial].[Language1Group] IS NULL) OR ([Source].[Language1Group] IS NULL AND [TestSittingTestMaterial].[Language1Group] IS NOT NULL) OR (([Source].[Language1Group] IS NOT NULL AND [TestSittingTestMaterial].[Language1Group] IS NOT NULL) AND [Source].[Language1Group] != [TestSittingTestMaterial].[Language1Group]))
		OR (([Source].[Language2] IS NOT NULL AND [TestSittingTestMaterial].[Language2] IS NULL) OR ([Source].[Language2] IS NULL AND [TestSittingTestMaterial].[Language2] IS NOT NULL) OR (([Source].[Language2] IS NOT NULL AND [TestSittingTestMaterial].[Language2] IS NOT NULL) AND [Source].[Language2] != [TestSittingTestMaterial].[Language2]))
		OR (([Source].[Language2Code] IS NOT NULL AND [TestSittingTestMaterial].[Language2Code] IS NULL) OR ([Source].[Language2Code] IS NULL AND [TestSittingTestMaterial].[Language2Code] IS NOT NULL) OR (([Source].[Language2Code] IS NOT NULL AND [TestSittingTestMaterial].[Language2Code] IS NOT NULL) AND [Source].[Language2Code] != [TestSittingTestMaterial].[Language2Code]))
		OR (([Source].[Language2Group] IS NOT NULL AND [TestSittingTestMaterial].[Language2Group] IS NULL) OR ([Source].[Language2Group] IS NULL AND [TestSittingTestMaterial].[Language2Group] IS NOT NULL) OR (([Source].[Language2Group] IS NOT NULL AND [TestSittingTestMaterial].[Language2Group] IS NOT NULL) AND [Source].[Language2Group] != [TestSittingTestMaterial].[Language2Group]))
		OR (([Source].[Skill] IS NOT NULL AND [TestSittingTestMaterial].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [TestSittingTestMaterial].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [TestSittingTestMaterial].[Skill] IS NOT NULL) AND [Source].[Skill] != [TestSittingTestMaterial].[Skill]))
		OR (([Source].[TestMaterialTitle] IS NOT NULL AND [TestSittingTestMaterial].[TestMaterialTitle] IS NULL) OR ([Source].[TestMaterialTitle] IS NULL AND [TestSittingTestMaterial].[TestMaterialTitle] IS NOT NULL) OR (([Source].[TestMaterialTitle] IS NOT NULL AND [TestSittingTestMaterial].[TestMaterialTitle] IS NOT NULL) AND [Source].[TestMaterialTitle] != [TestSittingTestMaterial].[TestMaterialTitle]))
		OR (([Source].[TestTaskTypeLabel] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskTypeLabel] IS NULL) OR ([Source].[TestTaskTypeLabel] IS NULL AND [TestSittingTestMaterial].[TestTaskTypeLabel] IS NOT NULL) OR (([Source].[TestTaskTypeLabel] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskTypeLabel] IS NOT NULL) AND [Source].[TestTaskTypeLabel] != [TestSittingTestMaterial].[TestTaskTypeLabel]))
		OR (([Source].[TestTaskTypeName] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskTypeName] IS NULL) OR ([Source].[TestTaskTypeName] IS NULL AND [TestSittingTestMaterial].[TestTaskTypeName] IS NOT NULL) OR (([Source].[TestTaskTypeName] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskTypeName] IS NOT NULL) AND [Source].[TestTaskTypeName] != [TestSittingTestMaterial].[TestTaskTypeName]))
		OR (([Source].[TestTaskLabel] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskLabel] IS NULL) OR ([Source].[TestTaskLabel] IS NULL AND [TestSittingTestMaterial].[TestTaskLabel] IS NOT NULL) OR (([Source].[TestTaskLabel] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskLabel] IS NOT NULL) AND [Source].[TestTaskLabel] != [TestSittingTestMaterial].[TestTaskLabel]))
		OR (([Source].[TestTaskName] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskName] IS NULL) OR ([Source].[TestTaskName] IS NULL AND [TestSittingTestMaterial].[TestTaskName] IS NOT NULL) OR (([Source].[TestTaskName] IS NOT NULL AND [TestSittingTestMaterial].[TestTaskName] IS NOT NULL) AND [Source].[TestTaskName] != [TestSittingTestMaterial].[TestTaskName]))

	  --select * from @TestSittingTestMaterialHistory
	
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE TestSittingTestMaterialHistory AS Target USING(select * from @TestSittingTestMaterialHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[TestSittingTestMaterialId] = Source.[TestSittingTestMaterialId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestSittingTestMaterialHistory AS Target USING(	select * from @TestSittingTestMaterialHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestSittingTestMaterialId] = Source.[TestSittingTestMaterialId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestSittingTestMaterialHistory AS Target USING(	select * from @TestSittingTestMaterialHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestSittingTestMaterialId] = Source.[TestSittingTestMaterialId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestSittingTestMaterialId = Source.TestSittingTestMaterialId		  
		  ,Target.TestSittingId = Source.TestSittingId
		  ,Target.TestSessionId = Source.TestSessionId
		  ,Target.TestMaterialId = Source.TestMaterialId
		  ,Target.SatDate = Source.SatDate      
		  ,Target.TestLocationName = Source.TestLocationName
		  ,Target.TestLocationState = Source.TestLocationState
		  ,Target.CredentialRequestId = Source.CredentialRequestId
		  ,Target.CredentialApplicationId = Source.CredentialApplicationId

		  ,Target.PersonId = Source.PersonId      
		  ,Target.CustomerNo = Source.CustomerNo      
		  ,Target.CandidateName = Source.CandidateName      
		  ,Target.Language1 = Source.Language1      
		  ,Target.Language1Code = Source.Language1Code      
		  ,Target.Language1Group = Source.Language1Group      
		  ,Target.Language2 = Source.Language2      
		  ,Target.Language2Code = Source.Language2Code      
		  ,Target.Language2Group = Source.Language2Group      
		  ,Target.Skill = Source.Skill      
		  ,Target.TestMaterialTitle = Source.TestMaterialTitle      

		  ,Target.TestTaskTypeLabel = Source.TestTaskTypeLabel
		  ,Target.TestTaskTypeName = Source.TestTaskTypeName
		  ,Target.TestTaskLabel = Source.TestTaskLabel
		  ,Target.TestTaskName = Source.TestTaskName
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		INSERT([TestSittingTestMaterialId],[ModifiedDate],[TestSittingId],[TestSessionId],[TestMaterialId],[SatDate],[TestLocationName],[TestLocationState],[CredentialRequestId],[CredentialApplicationId],[PersonId],[CustomerNo],
			 [CandidateName],[Language1],[Language1Code],[Language1Group],[Language2],[Language2Code],[Language2Group],[Skill],[TestMaterialTitle],[TestTaskTypeLabel],[TestTaskTypeName],[TestTaskLabel],[TestTaskName],
			 [RowStatus])
	  
		VALUES (Source.[TestSittingTestMaterialId],@Date,Source.[TestSittingId],Source.[TestSessionId],Source.[TestMaterialId],Source.[SatDate],Source.[TestLocationName],Source.[TestLocationState],Source.[CredentialRequestId],
			  Source.[CredentialApplicationId],Source.[PersonId],Source.[CustomerNo],Source.[CandidateName],Source.[Language1],Source.[Language1Code],Source.[Language1Group],Source.[Language2],Source.[Language2Code],Source.[Language2Group],
			  Source.[Skill],Source.[TestMaterialTitle],Source.[TestTaskTypeLabel],Source.[TestTaskTypeName],Source.[TestTaskLabel],Source.[TestTaskName], 'Latest');
	    	
	COMMIT TRANSACTION;	
	
END