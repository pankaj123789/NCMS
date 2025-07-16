ALTER PROCEDURE [dbo].[ReportingSnapshot_MaterialRequest]
	@Date DateTime
AS
BEGIN

DECLARE @MaterialRequestHistory as table([MaterialRequestId] [int],[PanelId] [int],[Panel] [nvarchar](100) NULL,[OutputMaterialId] [int] NOT NULL,[SourceMaterialId] [int] NULL,[SourceMaterialName] [nvarchar](255) NULL,[OutputMaterialName] [nvarchar](255) NULL,[CredentialType] [nvarchar](255) NULL,
										 [Skill] [nvarchar](4000) NULL,[Status] [nvarchar](255) NULL,[NumberOfRounds] [int] NOT NULL,[MaxBillableHours] [float] NULL,[ProductSpecificationName] [nvarchar](255) NOT NULL,[GLCode] [nvarchar](255) NOT NULL,[CostPerHour] [money] NOT NULL,
										 [CreatedDate] [datetime] NULL, [ModifiedDate] [datetime] NOT NULL,[CreatedBy] [nvarchar](255) NULL,[OwnedBy] [nvarchar](225) NULL,[RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	
	INSERT into @MaterialRequestHistory

	SELECT
		 CASE WHEN [Source].[MaterialRequestId] IS NULL THEN [MaterialRequest].[MaterialRequestId] ELSE [Source].[MaterialRequestId] END AS [MaterialRequestId]
		,CASE WHEN [Source].[PanelId] IS NULL THEN [MaterialRequest].[PanelId] ELSE [Source].[PanelId] END AS [PanelId]
		,CASE WHEN [Source].[Panel] IS NULL THEN [MaterialRequest].[Panel] ELSE [Source].[Panel] END AS [Panel]
		,CASE WHEN [Source].[OutputMaterialId] IS NULL THEN [MaterialRequest].[OutputMaterialId] ELSE [Source].[OutputMaterialId] END AS [OutputMaterialId]
		,CASE WHEN [Source].[SourceMaterialId] IS NULL THEN [MaterialRequest].[SourceMaterialId] ELSE [Source].[SourceMaterialId] END AS [SourceMaterialId]
		,CASE WHEN [Source].[SourceMaterialName] IS NULL THEN [MaterialRequest].[SourceMaterialName] ELSE [Source].[SourceMaterialName] END AS [SourceMaterialName]
		,CASE WHEN [Source].[OutputMaterialName] IS NULL THEN [MaterialRequest].[OutputMaterialName] ELSE [Source].[OutputMaterialName] END AS [OutputMaterialName]
		,CASE WHEN [Source].[CredentialType] IS NULL THEN [MaterialRequest].[CredentialType] ELSE [Source].[CredentialType] END AS [CredentialType]
		,CASE WHEN [Source].[Skill] IS NULL THEN [MaterialRequest].[Skill] ELSE [Source].[Skill] END AS [Skill]
		,CASE WHEN [Source].[Status] IS NULL THEN [MaterialRequest].[Status] ELSE [Source].[Status] END AS [Status]
		,CASE WHEN [Source].[NumberOfRounds] IS NULL THEN [MaterialRequest].[NumberOfRounds] ELSE [Source].[NumberOfRounds] END AS [NumberOfRounds]
		,CASE WHEN [Source].[MaxBillableHours] IS NULL THEN [MaterialRequest].[MaxBillableHours] ELSE [Source].[MaxBillableHours] END AS [MaxBillableHours]
		,CASE WHEN [Source].[ProductSpecificationName] IS NULL THEN [MaterialRequest].[ProductSpecificationName] ELSE [Source].[ProductSpecificationName] END AS [ProductSpecificationName]
		,CASE WHEN [Source].[GLCode] IS NULL THEN [MaterialRequest].[GLCode] ELSE [Source].[GLCode] END AS [GLCode]		
		,CASE WHEN [Source].[CostPerHour] IS NULL THEN [MaterialRequest].[CostPerHour] ELSE [Source].[CostPerHour] END AS [CostPerHour]
		,CASE WHEN [Source].[CreatedDate] IS NULL THEN [MaterialRequest].[CreatedDate] ELSE [Source].[CreatedDate] END AS [CreatedDate]

		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[CreatedBy] IS NULL THEN [MaterialRequest].[CreatedBy] ELSE [Source].[CreatedBy] END AS [CreatedBy]
		,CASE WHEN [Source].[OwnedBy] IS NULL THEN [MaterialRequest].[OwnedBy] ELSE [Source].[OwnedBy] END AS [OwnedBy]		
		
		,CASE WHEN [Source].[MaterialRequestId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]
		
	FROM
	(
		SELECT	
		
		     mr.[MaterialRequestId] as [MaterialRequestId]
			,p.[PanelId] as [PanelId]
			,p.[Name] as [Panel]
			,tms.[TestMaterialId] as [SourceMaterialId]
			,tms.[Title] as [SourceMaterialName]
			,tmo.TestMaterialId as [OutputMaterialId]
			,tmo.Title as [OutputMaterialName]
			,ct.InternalName as [CredentialType]			
		    ,REPLACE(REPLACE(dt.DisplayName, '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) AS [Skill]
			,mrst.[Name] as [Status]
			,mrr.[RoundNumber] as [NumberOfRounds]
			,mr.MaxBillableHours as [MaxBillableHours]
			,ps.[Name] as [ProductSpecificationName]
			,glc.Code as [GLCode]
			,ps.CostPerUnit as [CostPerHour]
			,mr.[CreatedDate] as [CreatedDate]
			,cu.[FullName] as [CreatedBy]
			,ou.[FullName] as [OwnedBy]				
						
		FROM [naati_db]..[tblMaterialRequest] mr
			LEFT JOIN [naati_db]..[tblPanel] p ON mr.PanelId = p.PanelId
			LEFT JOIN [naati_db]..[tblTestMaterial] tms ON mr.[SourceMaterialId] = tms.[TestMaterialId]
			LEFT JOIN [naati_db]..[tblTestMaterial] tmo ON mr.[OutputMaterialId] = tmo.[TestMaterialId]
			LEFT JOIN [naati_db]..[tblTestComponentType] tct on tmo.TestComponentTypeId = tct.TestComponentTypeId
			LEFT JOIN [naati_db]..[tblTestSpecification] ts on tct.TestSpecificationId = ts.TestSpecificationId
			LEFT JOIN [naati_db]..[tblCredentialType] ct on ts.CredentialTypeId = ct.CredentialTypeId
			LEFT JOIN [naati_db]..[tblMaterialRequestStatusType] mrst on mr.MaterialRequestStatusTypeId = mrst.MaterialRequestStatusTypeId
			LEFT JOIN [naati_db]..tblSkill s on tmo.SkillId = s.SkillId
		    LEFT JOIN [naati_db]..tblDirectionType dt on s.DirectionTypeId = dt.DirectionTypeId
			LEFT JOIN [naati_db]..tblUser cu on mr.CreatedByUserId = cu.UserId
			LEFT JOIN [naati_db]..tblUser ou on mr.OwnedByUserId = ou.UserId
			LEFT JOIN [naati_db]..[tblProductSpecification] ps on mr.ProductSpecificationId = ps.ProductSpecificationId
			LEFT JOIN [naati_db]..[tblGLCode] glc on ps.GLCodeId = glc.GLCodeId
			LEFT JOIN 
					(
				SELECT 
				 mrr.[MaterialRequestId]	
				,max(mrr.[RoundNumber]) as [RoundNumber]
				FROM [naati_db]..[tblMaterialRequestRound] mrr				
				group by mrr.MaterialRequestId
			) mrr ON mr.MaterialRequestId = mrr.MaterialRequestId
			INNER JOIN [naati_db]..tblLanguage l1 ON s.Language1Id = l1.LanguageId
			INNER JOIN [naati_db]..tblLanguage l2 ON s.Language2Id = l2.LanguageId		
						
	) [Source]
	FULL OUTER JOIN [MaterialRequest] ON [Source].[MaterialRequestId] = [MaterialRequest].[MaterialRequestId]

	WHERE 
		(([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequest].[MaterialRequestId] IS NULL) OR ([Source].[MaterialRequestId] IS NULL AND [MaterialRequest].[MaterialRequestId] IS NOT NULL) OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequest].[MaterialRequestId] IS NOT NULL) AND [Source].[MaterialRequestId] != [MaterialRequest].[MaterialRequestId]))
		OR (([Source].[PanelId] IS NOT NULL AND [MaterialRequest].[PanelId] IS NULL) OR ([Source].[PanelId] IS NULL AND [MaterialRequest].[PanelId] IS NOT NULL) OR (([Source].[PanelId] IS NOT NULL AND [MaterialRequest].[PanelId] IS NOT NULL) AND [Source].[PanelId] != [MaterialRequest].[PanelId]))
		OR (([Source].[Panel] IS NOT NULL AND [MaterialRequest].[Panel] IS NULL) OR ([Source].[Panel] IS NULL AND [MaterialRequest].[Panel] IS NOT NULL) OR (([Source].[Panel] IS NOT NULL AND [MaterialRequest].[Panel] IS NOT NULL) AND [Source].[Panel] != [MaterialRequest].[Panel]))
		OR (([Source].[SourceMaterialId] IS NOT NULL AND [MaterialRequest].[SourceMaterialId] IS NULL) OR ([Source].[SourceMaterialId] IS NULL AND [MaterialRequest].[SourceMaterialId] IS NOT NULL) OR (([Source].[SourceMaterialId] IS NOT NULL AND [MaterialRequest].[SourceMaterialId] IS NOT NULL) AND [Source].[SourceMaterialId] != [MaterialRequest].[SourceMaterialId]))
		OR (([Source].[SourceMaterialName] IS NOT NULL AND [MaterialRequest].[SourceMaterialName] IS NULL) OR ([Source].[SourceMaterialName] IS NULL AND [MaterialRequest].[SourceMaterialName] IS NOT NULL) OR (([Source].[SourceMaterialName] IS NOT NULL AND [MaterialRequest].[SourceMaterialName] IS NOT NULL) AND [Source].[SourceMaterialName] != [MaterialRequest].[SourceMaterialName]))
		OR (([Source].[OutputMaterialId] IS NOT NULL AND [MaterialRequest].[OutputMaterialId] IS NULL) OR ([Source].[OutputMaterialId] IS NULL AND [MaterialRequest].[OutputMaterialId] IS NOT NULL) OR (([Source].[OutputMaterialId] IS NOT NULL AND [MaterialRequest].[OutputMaterialId] IS NOT NULL) AND [Source].[OutputMaterialId] != [MaterialRequest].[OutputMaterialId]))
		OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequest].[OutputMaterialName] IS NULL) OR ([Source].[OutputMaterialName] IS NULL AND [MaterialRequest].[OutputMaterialName] IS NOT NULL) OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequest].[OutputMaterialName] IS NOT NULL) AND [Source].[OutputMaterialName] != [MaterialRequest].[OutputMaterialName]))
		OR (([Source].[CredentialType] IS NOT NULL AND [MaterialRequest].[CredentialType] IS NULL) OR ([Source].[CredentialType] IS NULL AND [MaterialRequest].[CredentialType] IS NOT NULL) OR (([Source].[CredentialType] IS NOT NULL AND [MaterialRequest].[CredentialType] IS NOT NULL) AND [Source].[CredentialType] != [MaterialRequest].[CredentialType]))
		OR (([Source].[Skill] IS NOT NULL AND [MaterialRequest].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [MaterialRequest].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [MaterialRequest].[Skill] IS NOT NULL) AND [Source].[Skill] != [MaterialRequest].[Skill]))
		OR (([Source].[Status] IS NOT NULL AND [MaterialRequest].[Status] IS NULL) OR ([Source].[Status] IS NULL AND [MaterialRequest].[Status] IS NOT NULL) OR (([Source].[Status] IS NOT NULL AND [MaterialRequest].[Status] IS NOT NULL) AND [Source].[Status] != [MaterialRequest].[Status]))
		OR (([Source].[NumberOfRounds] IS NOT NULL AND [MaterialRequest].[NumberOfRounds] IS NULL) OR ([Source].[NumberOfRounds] IS NULL AND [MaterialRequest].[NumberOfRounds] IS NOT NULL) OR (([Source].[NumberOfRounds] IS NOT NULL AND [MaterialRequest].[NumberOfRounds] IS NOT NULL) AND [Source].[NumberOfRounds] != [MaterialRequest].[NumberOfRounds]))

		OR (([Source].[MaxBillableHours] IS NOT NULL AND [MaterialRequest].[MaxBillableHours] IS NULL) OR ([Source].[MaxBillableHours] IS NULL AND [MaterialRequest].[MaxBillableHours] IS NOT NULL) OR (([Source].[MaxBillableHours] IS NOT NULL AND [MaterialRequest].[MaxBillableHours] IS NOT NULL) AND [Source].[MaxBillableHours] != [MaterialRequest].[MaxBillableHours]))
		OR (([Source].[ProductSpecificationName] IS NOT NULL AND [MaterialRequest].[ProductSpecificationName] IS NULL) OR ([Source].[ProductSpecificationName] IS NULL AND [MaterialRequest].[ProductSpecificationName] IS NOT NULL) OR (([Source].[ProductSpecificationName] IS NOT NULL AND [MaterialRequest].[ProductSpecificationName] IS NOT NULL) AND [Source].[ProductSpecificationName] != [MaterialRequest].[ProductSpecificationName]))
		OR (([Source].[GLCode] IS NOT NULL AND [MaterialRequest].[GLCode] IS NULL) OR ([Source].[GLCode] IS NULL AND [MaterialRequest].[GLCode] IS NOT NULL) OR (([Source].[GLCode] IS NOT NULL AND [MaterialRequest].[GLCode] IS NOT NULL) AND [Source].[GLCode] != [MaterialRequest].[GLCode]))
		OR (([Source].[CostPerHour] IS NOT NULL AND [MaterialRequest].[CostPerHour] IS NULL) OR ([Source].[CostPerHour] IS NULL AND [MaterialRequest].[CostPerHour] IS NOT NULL) OR (([Source].[CostPerHour] IS NOT NULL AND [MaterialRequest].[CostPerHour] IS NOT NULL) AND [Source].[CostPerHour] != [MaterialRequest].[CostPerHour]))

		OR (([Source].[CreatedDate] IS NOT NULL AND [MaterialRequest].[CreatedDate] IS NULL) OR ([Source].[CreatedDate] IS NULL AND [MaterialRequest].[CreatedDate] IS NOT NULL) OR (([Source].[CreatedDate] IS NOT NULL AND [MaterialRequest].[CreatedDate] IS NOT NULL) AND [Source].[CreatedDate] != [MaterialRequest].[CreatedDate]))
		OR (([Source].[CreatedBy] IS NOT NULL AND [MaterialRequest].[CreatedBy] IS NULL) OR ([Source].[CreatedBy] IS NULL AND [MaterialRequest].[CreatedBy] IS NOT NULL) OR (([Source].[CreatedBy] IS NOT NULL AND [MaterialRequest].[CreatedBy] IS NOT NULL) AND [Source].[CreatedBy] != [MaterialRequest].[CreatedBy]))
		OR (([Source].[OwnedBy] IS NOT NULL AND [MaterialRequest].[OwnedBy] IS NULL) OR ([Source].[OwnedBy] IS NULL AND [MaterialRequest].[OwnedBy] IS NOT NULL) OR (([Source].[OwnedBy] IS NOT NULL AND [MaterialRequest].[OwnedBy] IS NOT NULL) AND [Source].[OwnedBy] != [MaterialRequest].[OwnedBy]))
				
	--select * from @MaterialRequestHistory
		
	BEGIN TRANSACTION 
	
	   --Merge operation delete
		MERGE MaterialRequestHistory AS Target USING(select * from @MaterialRequestHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.MaterialRequestId = Source.MaterialRequestId AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE MaterialRequestHistory AS Target USING(	select * from @MaterialRequestHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.MaterialRequestId = Source.MaterialRequestId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE MaterialRequestHistory AS Target USING(	select * from @MaterialRequestHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.MaterialRequestId = Source.MaterialRequestId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.MaterialRequestId = Source.MaterialRequestId
		  ,Target.PanelId = Source.PanelId
		  ,Target.Panel = Source.Panel
		  ,Target.OutputMaterialId = Source.OutputMaterialId
		  ,Target.SourceMaterialId = Source.SourceMaterialId
		  ,Target.SourceMaterialName = Source.SourceMaterialName      
		  ,Target.OutputMaterialName = Source.OutputMaterialName
		  ,Target.CredentialType = Source.CredentialType
		  ,Target.Skill = Source.Skill
		  ,Target.[Status] = Source.[Status]

		  ,Target.NumberOfRounds = Source.NumberOfRounds
		  ,Target.MaxBillableHours = Source.MaxBillableHours
		  ,Target.ProductSpecificationName = Source.ProductSpecificationName
		  ,Target.GLCode = Source.GLCode
		  ,Target.CostPerHour = Source.CostPerHour
		  ,Target.CreatedDate = Source.CreatedDate		  
		  ,Target.CreatedBy = Source.CreatedBy
		  ,Target.OwnedBy = Source.OwnedBy	  
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([MaterialRequestId],[PanelId],[Panel],[OutputMaterialId],[SourceMaterialId],[SourceMaterialName],[OutputMaterialName],[CredentialType],
			 [Skill],[Status],[NumberOfRounds],[MaxBillableHours],[ProductSpecificationName],[GLCode],[CostPerHour],[CreatedDate],[ModifiedDate],[CreatedBy],[OwnedBy],[RowStatus])	  
	  
		 VALUES (Source.[MaterialRequestId],Source.[PanelId],Source.[Panel],Source.[OutputMaterialId],Source.[SourceMaterialId],Source.[SourceMaterialName],Source.[OutputMaterialName],Source.[CredentialType],
			 Source.[Skill],Source.[Status],Source.[NumberOfRounds],Source.[MaxBillableHours],Source.[ProductSpecificationName],Source.[GLCode],Source.[CostPerHour],Source.[CreatedDate],@Date,Source.[CreatedBy],Source.[OwnedBy], 'Latest');
	    	
	COMMIT TRANSACTION;	

END