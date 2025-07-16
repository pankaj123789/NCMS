ALTER PROCEDURE [dbo].[ReportingSnapshot_MaterialRequestTaskHours]
	@Date DateTime
AS
BEGIN
DECLARE @MaterialRequestTaskHoursHistory as table([MaterialRequestPanelMembershipTaskId] [int] NOT NULL,[MaterialRequestId] [int] NOT NULL,[OutputMaterialName] [nvarchar](255) NULL,[PanelMemberShipId] [int] NOT NULL,[Task] [nvarchar](255) NULL,[Hours] [float] NOT NULL,
												  [ModifiedDate] [datetime] NOT NULL, [RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @MaterialRequestTaskHoursHistory
	SELECT		 
		 CASE WHEN [Source].[MaterialRequestPanelMembershipTaskId] IS NULL THEN [MaterialRequestTaskHours].[MaterialRequestPanelMembershipTaskId] ELSE [Source].[MaterialRequestPanelMembershipTaskId] END AS [MaterialRequestPanelMembershipTaskId]
		,CASE WHEN [Source].[MaterialRequestId] IS NULL THEN [MaterialRequestTaskHours].[MaterialRequestId] ELSE [Source].[MaterialRequestId] END AS [MaterialRequestId]
		,CASE WHEN [Source].[OutputMaterialName] IS NULL THEN [MaterialRequestTaskHours].[OutputMaterialName] ELSE [Source].[OutputMaterialName] END AS [OutputMaterialName]
		,CASE WHEN [Source].[PanelMemberShipId] IS NULL THEN [MaterialRequestTaskHours].[PanelMemberShipId] ELSE [Source].[PanelMemberShipId] END AS [PanelMemberShipId]
		,CASE WHEN [Source].[Task] IS NULL THEN [MaterialRequestTaskHours].[Task] ELSE [Source].[Task] END AS [Task]		
		,CASE WHEN [Source].[Hours] IS NULL THEN [MaterialRequestTaskHours].[Hours] ELSE [Source].[Hours] END AS [Hours]
		
		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[MaterialRequestPanelMembershipTaskId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
	FROM
	(
		SELECT
			 mrpmt.[MaterialRequestPanelMembershipTaskId] AS [MaterialRequestPanelMembershipTaskId]
			,mr.[MaterialRequestId] as [MaterialRequestId]			
			,tmo.[Title] as [OutputMaterialName]
			,mrpm.[PanelMemberShipId] as [PanelMemberShipId]
			,mrpmty.[Name]as [Task]			
			,mrpmt.[HoursSpent] as [Hours]
						
			FROM [naati_db]..[tblMaterialRequestPanelMembershipTask] mrpmt		
		    LEFT JOIN [naati_db]..[tblMaterialRequestPanelMembership] mrpm on mrpmt.MaterialRequestPanelMemberShipId = mrpm.MaterialRequestPanelMemberShipId			
			LEFT JOIN [naati_db]..[tblMaterialRequest] mr ON mrpm.MaterialRequestId = mr.MaterialRequestId
			LEFT JOIN [naati_db]..[tblTestMaterial] tmo ON mr.[OutputMaterialId] = tmo.[TestMaterialId]
			LEFT JOIN [naati_db]..[tblMaterialRequestPanelMembershipType] mrpmty ON mrpm.MaterialRequestPanelMembershipTypeId = mrpmty.MaterialRequestPanelMembershipTypeId
						
	) [Source]
	FULL OUTER JOIN [MaterialRequestTaskHours] ON ([Source].[MaterialRequestPanelMembershipTaskId] = [MaterialRequestTaskHours].[MaterialRequestPanelMembershipTaskId] )

	WHERE 		
		(([Source].[MaterialRequestPanelMembershipTaskId] IS NOT NULL AND [MaterialRequestTaskHours].[MaterialRequestPanelMembershipTaskId] IS NULL) OR ([Source].[MaterialRequestPanelMembershipTaskId] IS NULL AND [MaterialRequestTaskHours].[MaterialRequestPanelMembershipTaskId] IS NOT NULL) OR (([Source].[MaterialRequestPanelMembershipTaskId] IS NOT NULL AND [MaterialRequestTaskHours].[MaterialRequestPanelMembershipTaskId] IS NOT NULL) AND [Source].[MaterialRequestPanelMembershipTaskId] != [MaterialRequestTaskHours].[MaterialRequestPanelMembershipTaskId]))
		OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestTaskHours].[MaterialRequestId] IS NULL) OR ([Source].[MaterialRequestId] IS NULL AND [MaterialRequestTaskHours].[MaterialRequestId] IS NOT NULL) OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestTaskHours].[MaterialRequestId] IS NOT NULL) AND [Source].[MaterialRequestId] != [MaterialRequestTaskHours].[MaterialRequestId]))
		OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequestTaskHours].[OutputMaterialName] IS NULL) OR ([Source].[OutputMaterialName] IS NULL AND [MaterialRequestTaskHours].[OutputMaterialName] IS NOT NULL) OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequestTaskHours].[OutputMaterialName] IS NOT NULL) AND [Source].[OutputMaterialName] != [MaterialRequestTaskHours].[OutputMaterialName]))
		OR (([Source].[PanelMemberShipId] IS NOT NULL AND [MaterialRequestTaskHours].[PanelMemberShipId] IS NULL) OR ([Source].[PanelMemberShipId] IS NULL AND [MaterialRequestTaskHours].[PanelMemberShipId] IS NOT NULL) OR (([Source].[PanelMemberShipId] IS NOT NULL AND [MaterialRequestTaskHours].[PanelMemberShipId] IS NOT NULL) AND [Source].[PanelMemberShipId] != [MaterialRequestTaskHours].[PanelMemberShipId]))
		OR (([Source].[Task] IS NOT NULL AND [MaterialRequestTaskHours].[Task] IS NULL) OR ([Source].[Task] IS NULL AND [MaterialRequestTaskHours].[Task] IS NOT NULL) OR (([Source].[Task] IS NOT NULL AND [MaterialRequestTaskHours].[Task] IS NOT NULL) AND [Source].[Task] != [MaterialRequestTaskHours].[Task]))
		OR (([Source].[Hours] IS NOT NULL AND [MaterialRequestTaskHours].[Hours] IS NULL) OR ([Source].[Hours] IS NULL AND [MaterialRequestTaskHours].[Hours] IS NOT NULL) OR (([Source].[Hours] IS NOT NULL AND [MaterialRequestTaskHours].[Hours] IS NOT NULL) AND [Source].[Hours] != [MaterialRequestTaskHours].[Hours]))
			
	
		--select * from @MaterialRequestTaskHoursHistory
	
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE MaterialRequestTaskHoursHistory AS Target USING(select * from @MaterialRequestTaskHoursHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[MaterialRequestPanelMembershipTaskId] = Source.[MaterialRequestPanelMembershipTaskId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE MaterialRequestTaskHoursHistory AS Target USING(	select * from @MaterialRequestTaskHoursHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MaterialRequestPanelMembershipTaskId] = Source.[MaterialRequestPanelMembershipTaskId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ModifiedDate] = @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE MaterialRequestTaskHoursHistory AS Target USING(	select * from @MaterialRequestTaskHoursHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MaterialRequestPanelMembershipTaskId] = Source.[MaterialRequestPanelMembershipTaskId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.MaterialRequestPanelMembershipTaskId = Source.MaterialRequestPanelMembershipTaskId
		  ,Target.MaterialRequestId = Source.MaterialRequestId
		  ,Target.OutputMaterialName = Source.OutputMaterialName
		  ,Target.PanelMemberShipId = Source.PanelMemberShipId
		  ,Target.Task = Source.Task
		  ,Target.Hours = Source.Hours
		  
		  ,Target.ModifiedDate = @Date		  
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
			INSERT([MaterialRequestPanelMembershipTaskId],[MaterialRequestId],[OutputMaterialName],[PanelMemberShipId],[Task],[Hours],[ModifiedDate],[RowStatus])	  
			VALUES (Source.[MaterialRequestPanelMembershipTaskId],Source.[MaterialRequestId],Source.[OutputMaterialName],Source.[PanelMemberShipId],Source.[Task],Source.[Hours],@Date,'Latest');

	COMMIT TRANSACTION;	

END