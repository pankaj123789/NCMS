
ALTER PROCEDURE [dbo].[ReportingSnapshop_TestMaterial]
	@Date DateTime
AS
BEGIN

DECLARE @TestMaterialHistory as table([TestMaterialId] [int] NOT NULL,[Title] [nvarchar](255) NOT NULL,[LanguageId] [int] NULL,[Language] [nvarchar](50) NULL,[SkillId] [int] NULL,[Skill] [nvarchar](max) NULL,[Available] [bit] NOT NULL,[MaterialRequestId] [int] NULL,
									  [TestMaterialTypeId] [int] NOT NULL,[TestMaterialType] [nvarchar](255) NOT NULL,[TestMaterialDomainId] [int] NOT NULL,[TestMaterialDomain] [nvarchar](255) NOT NULL,
									  [ModifiedDate] [datetime] NOT NULL,[RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @TestMaterialHistory
	SELECT

		 CASE WHEN [Source].[TestMaterialId] IS NULL THEN [TestMaterial].[TestMaterialId] ELSE [Source].[TestMaterialId] END AS [TestMaterialId]
		,CASE WHEN [Source].[Title] IS NULL THEN [TestMaterial].[Title] ELSE [Source].[Title] END AS [Title]
		
		,CASE WHEN [Source].[LanguageId] IS NULL THEN [TestMaterial].[LanguageId] ELSE [Source].[LanguageId] END AS [LanguageId]
		,CASE WHEN [Source].[Language] IS NULL THEN [TestMaterial].[Language] ELSE [Source].[Language] END AS [Language]
		,CASE WHEN [Source].[SkillId] IS NULL THEN [TestMaterial].[SkillId] ELSE [Source].[SkillId] END AS [SkillId]
		,CASE WHEN [Source].[Skill] IS NULL THEN [TestMaterial].[Skill] ELSE [Source].[Skill] END AS [Skill]

		,CASE WHEN [Source].[Available] IS NULL THEN [TestMaterial].[Available] ELSE [Source].[Available] END AS [Available]
		,CASE WHEN [Source].[MaterialRequestId] IS NULL THEN [TestMaterial].[MaterialRequestId] ELSE [Source].[MaterialRequestId] END AS [MaterialRequestId]		

		,CASE WHEN [Source].[TestMaterialTypeId] IS NULL THEN [TestMaterial].[TestMaterialTypeId] ELSE [Source].[TestMaterialTypeId] END AS [TestMaterialTypeId]
		,CASE WHEN [Source].[TestMaterialType] IS NULL THEN [TestMaterial].[TestMaterialType] ELSE [Source].[TestMaterialType] END AS [TestMaterialType]

		,CASE WHEN [Source].[TestMaterialDomainId] IS NULL THEN [TestMaterial].[TestMaterialDomainId] ELSE [Source].[TestMaterialDomainId] END AS [TestMaterialDomainId]
		,CASE WHEN [Source].[TestMaterialDomain] IS NULL THEN [TestMaterial].[TestMaterialDomain] ELSE [Source].[TestMaterialDomain] END AS [TestMaterialDomain]
						
		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[TestMaterialId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
	FROM
	(
		SELECT			
		     tm.[TestMaterialId] as [TestMaterialId]
			,tm.[Title] as [Title]			
			,tm.[LanguageId] as [LanguageId]
			,l.[Name] as [Language]
			,tm.[SkillId] as [SkillId]
			,REPLACE(REPLACE(dt.DisplayName, '[Language 1]', l1.[Name]), '[Language 2]', l2.Name) AS [Skill]
			,tm.[Available] as [Available]
			,mr.[MaterialRequestId] as [MaterialRequestId]
			,tm.[TestMaterialTypeId] as [TestMaterialTypeId]
			,tmt.[Name] as [TestMaterialType]
			,tm.[TestMaterialDomainId] as [TestMaterialDomainId]	
			,tmd.[DisplayName] as [TestMaterialDomain]		
			
		FROM [naati_db]..[tblTestMaterial] tm
			LEFT JOIN [naati_db]..tblLanguage l on tm.LanguageId = l.LanguageId
			LEFT JOIN [naati_db]..tblSkill s on tm.SkillId = s.SkillId
			LEFT JOIN [naati_db]..tblDirectionType dt on s.DirectionTypeId = dt.DirectionTypeId
			LEFT JOIN [naati_db]..tblLanguage l1 ON s.Language1Id = l1.LanguageId
		    LEFT JOIN [naati_db]..tblLanguage l2 ON s.Language2Id = l2.LanguageId
			LEFT JOIN [naati_db]..[tblMaterialRequest] mr ON tm.TestMaterialId = mr.OutputMaterialId	
			LEFT JOIN [naati_db]..[tblTestMaterialType] tmt on tm.TestMaterialTypeId = tmt.TestMaterialTypeId	
			LEFT JOIN [naati_db]..[tblTestMaterialDomain] tmd on tm.TestMaterialDomainId = tmd.TestMaterialDomainId				
						
	) [Source]
	FULL OUTER JOIN [TestMaterial] ON [Source].[TestMaterialId] = [TestMaterial].[TestMaterialId]	

	WHERE 
	   (([Source].[TestMaterialId] IS NOT NULL AND [TestMaterial].[TestMaterialId] IS NULL) OR ([Source].[TestMaterialId] IS NULL AND [TestMaterial].[TestMaterialId] IS NOT NULL) OR (([Source].[TestMaterialId] IS NOT NULL AND [TestMaterial].[TestMaterialId] IS NOT NULL) AND [Source].[TestMaterialId] != [TestMaterial].[TestMaterialId]))
	OR (([Source].[Title] IS NOT NULL AND [TestMaterial].[Title] IS NULL) OR ([Source].[Title] IS NULL AND [TestMaterial].[Title] IS NOT NULL) OR (([Source].[Title] IS NOT NULL AND [TestMaterial].[Title] IS NOT NULL) AND [Source].[Title] != [TestMaterial].[Title]))

	OR (([Source].[LanguageId] IS NOT NULL AND [TestMaterial].[LanguageId] IS NULL) OR ([Source].[LanguageId] IS NULL AND [TestMaterial].[LanguageId] IS NOT NULL) OR (([Source].[LanguageId] IS NOT NULL AND [TestMaterial].[LanguageId] IS NOT NULL) AND [Source].[LanguageId] != [TestMaterial].[LanguageId]))
	OR (([Source].[Language] IS NOT NULL AND [TestMaterial].[Language] IS NULL) OR ([Source].[Language] IS NULL AND [TestMaterial].[Language] IS NOT NULL) OR (([Source].[Language] IS NOT NULL AND [TestMaterial].[Language] IS NOT NULL) AND [Source].[Language] != [TestMaterial].[Language]))
	OR (([Source].[SkillId] IS NOT NULL AND [TestMaterial].[SkillId] IS NULL) OR ([Source].[SkillId] IS NULL AND [TestMaterial].[SkillId] IS NOT NULL) OR (([Source].[SkillId] IS NOT NULL AND [TestMaterial].[SkillId] IS NOT NULL) AND [Source].[SkillId] != [TestMaterial].[SkillId]))
	OR (([Source].[Skill] IS NOT NULL AND [TestMaterial].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [TestMaterial].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [TestMaterial].[Skill] IS NOT NULL) AND [Source].[Skill] != [TestMaterial].[Skill]))

	OR (([Source].[Available] IS NOT NULL AND [TestMaterial].[Available] IS NULL) OR ([Source].[Available] IS NULL AND [TestMaterial].[Available] IS NOT NULL) OR (([Source].[Available] IS NOT NULL AND [TestMaterial].[Available] IS NOT NULL) AND [Source].[Available] != [TestMaterial].[Available]))

	OR (([Source].[MaterialRequestId] IS NOT NULL AND [TestMaterial].[MaterialRequestId] IS NULL) OR ([Source].[MaterialRequestId] IS NULL AND [TestMaterial].[MaterialRequestId] IS NOT NULL) OR (([Source].[MaterialRequestId] IS NOT NULL AND [TestMaterial].[MaterialRequestId] IS NOT NULL) AND [Source].[MaterialRequestId] != [TestMaterial].[MaterialRequestId]))
	
	OR (([Source].[TestMaterialTypeId] IS NOT NULL AND [TestMaterial].[TestMaterialTypeId] IS NULL) OR ([Source].[TestMaterialTypeId] IS NULL AND [TestMaterial].[TestMaterialTypeId] IS NOT NULL) OR (([Source].[TestMaterialTypeId] IS NOT NULL AND [TestMaterial].[TestMaterialTypeId] IS NOT NULL) AND [Source].[TestMaterialTypeId] != [TestMaterial].[TestMaterialTypeId]))
	OR (([Source].[TestMaterialType] IS NOT NULL AND [TestMaterial].[TestMaterialType] IS NULL) OR ([Source].[TestMaterialType] IS NULL AND [TestMaterial].[TestMaterialType] IS NOT NULL) OR (([Source].[TestMaterialType] IS NOT NULL AND [TestMaterial].[TestMaterialType] IS NOT NULL) AND [Source].[TestMaterialType] != [TestMaterial].[TestMaterialType]))
	OR (([Source].[TestMaterialDomainId] IS NOT NULL AND [TestMaterial].[TestMaterialDomainId] IS NULL) OR ([Source].[TestMaterialDomainId] IS NULL AND [TestMaterial].[TestMaterialDomainId] IS NOT NULL) OR (([Source].[TestMaterialDomainId] IS NOT NULL AND [TestMaterial].[TestMaterialDomainId] IS NOT NULL) AND [Source].[TestMaterialDomainId] != [TestMaterial].[TestMaterialDomainId]))
	OR (([Source].[TestMaterialDomain] IS NOT NULL AND [TestMaterial].[TestMaterialDomain] IS NULL) OR ([Source].[TestMaterialDomain] IS NULL AND [TestMaterial].[TestMaterialDomain] IS NOT NULL) OR (([Source].[TestMaterialDomain] IS NOT NULL AND [TestMaterial].[TestMaterialDomain] IS NOT NULL) AND [Source].[TestMaterialDomain] != [TestMaterial].[TestMaterialDomain]))
			
	--select * from @TestMaterialHistory
		
	BEGIN TRANSACTION 
	
	   --Merge operation delete
		MERGE TestMaterialHistory AS Target USING(select * from @TestMaterialHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[TestMaterialId] = Source.[TestMaterialId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestMaterialHistory AS Target USING(select * from @TestMaterialHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestMaterialId] = Source.[TestMaterialId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestMaterialHistory AS Target USING(	select * from @TestMaterialHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[TestMaterialId] = Source.[TestMaterialId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestMaterialId = Source.TestMaterialId
		  ,Target.Title = Source.Title
		  ,Target.LanguageId = Source.LanguageId
		  ,Target.Language = Source.Language
		  ,Target.SkillId = Source.SkillId
		  ,Target.Skill = Source.Skill      
		  ,Target.Available = Source.Available
		  ,Target.MaterialRequestId = Source.MaterialRequestId
		  ,Target.TestMaterialTypeId = Source.TestMaterialTypeId
		  ,Target.TestMaterialType = Source.TestMaterialType

		  ,Target.TestMaterialDomainId = Source.TestMaterialDomainId      
		  ,Target.TestMaterialDomain = Source.TestMaterialDomain   
		  ,Target.ModifiedDate = @Date

		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([TestMaterialId],[Title],[LanguageId],[Language],[SkillId],[Skill],[Available],[MaterialRequestId],[TestMaterialTypeId],[TestMaterialType],[TestMaterialDomainId],[TestMaterialDomain],[ModifiedDate],[RowStatus])
	  
		  VALUES (Source.[TestMaterialId],Source.[Title],Source.[LanguageId],Source.[Language],Source.[SkillId],Source.[Skill],Source.[Available],Source.[MaterialRequestId],Source.[TestMaterialTypeId],Source.[TestMaterialType],
			  Source.[TestMaterialDomainId],Source.[TestMaterialDomain],@Date, 'Latest');		  
	
	 COMMIT TRANSACTION;
	
	
END
