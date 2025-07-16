ALTER PROCEDURE [dbo].[ReportingSnapshot_MaterialRequestPanelMember]
	@Date DateTime
AS
BEGIN

DECLARE @MaterialRequestPanelMemberHistory as table([MaterialRequestPanelMembershipId] [int] NOT NULL,[MaterialRequestId] [int] NOT NULL,[OutputMaterialName] [nvarchar](255) NULL,[PanelMemberShipId] [int] NOT NULL,[MemberType] [nvarchar](255) NULL,
													[ModifiedDate] [datetime] NOT NULL,	[RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @MaterialRequestPanelMemberHistory

	SELECT		 
		 CASE WHEN [Source].[MaterialRequestPanelMembershipId] IS NULL THEN [MaterialRequestPanelMember].[MaterialRequestPanelMembershipId] ELSE [Source].[MaterialRequestPanelMembershipId] END AS [MaterialRequestPanelMembershipId]
		,CASE WHEN [Source].[MaterialRequestId] IS NULL THEN [MaterialRequestPanelMember].[MaterialRequestId] ELSE [Source].[MaterialRequestId] END AS [MaterialRequestId]
		,CASE WHEN [Source].[OutputMaterialName] IS NULL THEN [MaterialRequestPanelMember].[OutputMaterialName] ELSE [Source].[OutputMaterialName] END AS [OutputMaterialName]
		,CASE WHEN [Source].[PanelMemberShipId] IS NULL THEN [MaterialRequestPanelMember].[PanelMemberShipId] ELSE [Source].[PanelMemberShipId] END AS [PanelMemberShipId]
		,CASE WHEN [Source].[MemberType] IS NULL THEN [MaterialRequestPanelMember].[MemberType] ELSE [Source].[MemberType] END AS [MemberType]	
		
		,@Date AS [ModifiedDate]
		,CASE WHEN [Source].[MaterialRequestPanelMembershipId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM
	(
		SELECT
			 mrpm.[MaterialRequestPanelMembershipId] AS [MaterialRequestPanelMembershipId]
			,mrpm.[MaterialRequestId] as [MaterialRequestId]			
			,tmo.[Title] as [OutputMaterialName]
			,mrpm.[PanelMemberShipId] as [PanelMemberShipId]
			,mrpmty.[Name]as [MemberType]
						
		FROM [naati_db]..[tblMaterialRequestPanelMembership] mrpm		    
			LEFT JOIN [naati_db]..[tblMaterialRequest] mr ON mrpm.MaterialRequestId = mr.MaterialRequestId			
			LEFT JOIN [naati_db]..[tblTestMaterial] tmo ON mr.[OutputMaterialId] = tmo.[TestMaterialId]
			LEFT JOIN [naati_db]..[tblMaterialRequestPanelMembershipType] mrpmty ON mrpm.MaterialRequestPanelMembershipTypeId = mrpmty.MaterialRequestPanelMembershipTypeId	
						
	) [Source]
	FULL OUTER JOIN [MaterialRequestPanelMember] ON ([Source].[MaterialRequestPanelMembershipId] = [MaterialRequestPanelMember].[MaterialRequestPanelMembershipId])

	WHERE 		
		(([Source].[MaterialRequestPanelMembershipId] IS NOT NULL AND [MaterialRequestPanelMember].[MaterialRequestPanelMembershipId] IS NULL) OR ([Source].[MaterialRequestPanelMembershipId] IS NULL AND [MaterialRequestPanelMember].[MaterialRequestPanelMembershipId] IS NOT NULL) OR (([Source].[MaterialRequestPanelMembershipId] IS NOT NULL AND [MaterialRequestPanelMember].[MaterialRequestPanelMembershipId] IS NOT NULL) AND [Source].[MaterialRequestPanelMembershipId] != [MaterialRequestPanelMember].[MaterialRequestPanelMembershipId]))
		OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestPanelMember].[MaterialRequestId] IS NULL) OR ([Source].[MaterialRequestId] IS NULL AND [MaterialRequestPanelMember].[MaterialRequestId] IS NOT NULL) OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestPanelMember].[MaterialRequestId] IS NOT NULL) AND [Source].[MaterialRequestId] != [MaterialRequestPanelMember].[MaterialRequestId]))
		OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequestPanelMember].[OutputMaterialName] IS NULL) OR ([Source].[OutputMaterialName] IS NULL AND [MaterialRequestPanelMember].[OutputMaterialName] IS NOT NULL) OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequestPanelMember].[OutputMaterialName] IS NOT NULL) AND [Source].[OutputMaterialName] != [MaterialRequestPanelMember].[OutputMaterialName]))
		OR (([Source].[PanelMemberShipId] IS NOT NULL AND [MaterialRequestPanelMember].[PanelMemberShipId] IS NULL) OR ([Source].[PanelMemberShipId] IS NULL AND [MaterialRequestPanelMember].[PanelMemberShipId] IS NOT NULL) OR (([Source].[PanelMemberShipId] IS NOT NULL AND [MaterialRequestPanelMember].[PanelMemberShipId] IS NOT NULL) AND [Source].[PanelMemberShipId] != [MaterialRequestPanelMember].[PanelMemberShipId]))
		OR (([Source].[MemberType] IS NOT NULL AND [MaterialRequestPanelMember].[MemberType] IS NULL) OR ([Source].[MemberType] IS NULL AND [MaterialRequestPanelMember].[MemberType] IS NOT NULL) OR (([Source].[MemberType] IS NOT NULL AND [MaterialRequestPanelMember].[MemberType] IS NOT NULL) AND [Source].[MemberType] != [MaterialRequestPanelMember].[MemberType]))
				
	 --select * from @MaterialRequestPanelMemberHistory
	 	 
	 BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE MaterialRequestPanelMemberHistory AS Target USING(select * from @MaterialRequestPanelMemberHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.MaterialRequestPanelMembershipId = Source.MaterialRequestPanelMembershipId AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE MaterialRequestPanelMemberHistory AS Target USING(select * from @MaterialRequestPanelMemberHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.MaterialRequestPanelMembershipId = Source.MaterialRequestPanelMembershipId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE MaterialRequestPanelMemberHistory AS Target USING(select * from @MaterialRequestPanelMemberHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.MaterialRequestPanelMembershipId = Source.MaterialRequestPanelMembershipId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.MaterialRequestPanelMembershipId = Source.MaterialRequestPanelMembershipId
		  ,Target.MaterialRequestId = Source.MaterialRequestId
		  ,Target.OutputMaterialName = Source.OutputMaterialName
		  ,Target.PanelMemberShipId = Source.PanelMemberShipId
		  ,Target.MemberType = Source.MemberType		  
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		  INSERT([MaterialRequestPanelMembershipId],[MaterialRequestId],[OutputMaterialName],[PanelMemberShipId],[MemberType],[ModifiedDate],[RowStatus])	  	  
			VALUES (Source.[MaterialRequestPanelMembershipId],Source.[MaterialRequestId],Source.[OutputMaterialName],Source.[PanelMemberShipId],Source.[MemberType],@Date, 'Latest');
		  		  	
	COMMIT TRANSACTION;	

END