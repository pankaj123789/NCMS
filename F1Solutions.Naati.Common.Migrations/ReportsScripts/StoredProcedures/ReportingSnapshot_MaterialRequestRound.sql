ALTER PROCEDURE [dbo].[ReportingSnapshot_MaterialRequestRound]
	@Date DateTime
AS
BEGIN
	DECLARE @MaterialRequestRoundHistory as table([MaterialRequestRoundId] [int] NOT NULL,[MaterialRequestId] [int] NOT NULL,[OutputMaterialName] [nvarchar](255) NULL,[DueDate] [datetime] NULL,[RequestedDate] [datetime] NULL,[SubmittedDate] [datetime] NULL,[Status] [nvarchar](255) NULL,
												  [ModifiedDate] [datetime] NOT NULL,[RowStatus] nvarchar(50))   
												  												  	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @MaterialRequestRoundHistory

	SELECT
		 CASE WHEN [Source].[MaterialRequestRoundId] IS NULL THEN [MaterialRequestRound].[MaterialRequestRoundId] ELSE [Source].[MaterialRequestRoundId] END AS [MaterialRequestRoundId]
		,CASE WHEN [Source].[MaterialRequestId] IS NULL THEN [MaterialRequestRound].[MaterialRequestId] ELSE [Source].[MaterialRequestId] END AS [MaterialRequestId]
		,CASE WHEN [Source].[OutputMaterialName] IS NULL THEN [MaterialRequestRound].[OutputMaterialName] ELSE [Source].[OutputMaterialName] END AS [OutputMaterialName]
		,CASE WHEN [Source].[DueDate] IS NULL THEN [MaterialRequestRound].[DueDate] ELSE [Source].[DueDate] END AS [DueDate]
		,CASE WHEN [Source].[RequestedDate] IS NULL THEN [MaterialRequestRound].[RequestedDate] ELSE [Source].[RequestedDate] END AS [RequestedDate]		
		,CASE WHEN [Source].[SubmittedDate] IS NULL THEN [MaterialRequestRound].[SubmittedDate] ELSE [Source].[SubmittedDate] END AS [SubmittedDate]
		,CASE WHEN [Source].[Status] IS NULL THEN [MaterialRequestRound].[Status] ELSE [Source].[Status] END AS [Status]
		
		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[MaterialRequestRoundId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
	FROM
	(
		SELECT			
		     mrr.[MaterialRequestRoundId] as [MaterialRequestRoundId]
			,mr.[MaterialRequestId] as [MaterialRequestId]			
			,tmo.[Title] as [OutputMaterialName]
			,mrr.[DueDate] as [DueDate]
			,mrr.[RequestedDate] as [RequestedDate]
			,mrr.[SubmittedDate] as [SubmittedDate]			
			,mrst.[Name] as [Status]							

		FROM [naati_db]..[tblMaterialRequestRound] mrr
			LEFT JOIN [naati_db]..[tblMaterialRequest] mr ON mrr.[MaterialRequestId] = mr.[MaterialRequestId]						
			LEFT JOIN [naati_db]..[tblTestMaterial] tmo ON mr.[OutputMaterialId] = tmo.[TestMaterialId]
			LEFT JOIN [naati_db]..[tblMaterialRequestStatusType] mrst on mr.MaterialRequestStatusTypeId = mrst.MaterialRequestStatusTypeId			
						
	) [Source]
	FULL OUTER JOIN [MaterialRequestRound] ON [Source].[MaterialRequestRoundId] = [MaterialRequestRound].[MaterialRequestRoundId]

	WHERE 
		(([Source].[MaterialRequestRoundId] IS NOT NULL AND [MaterialRequestRound].[MaterialRequestRoundId] IS NULL) OR ([Source].[MaterialRequestRoundId] IS NULL AND [MaterialRequestRound].[MaterialRequestRoundId] IS NOT NULL) OR (([Source].[MaterialRequestRoundId] IS NOT NULL AND [MaterialRequestRound].[MaterialRequestRoundId] IS NOT NULL) AND [Source].[MaterialRequestRoundId] != [MaterialRequestRound].[MaterialRequestRoundId]))
		OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestRound].[MaterialRequestId] IS NULL) OR ([Source].[MaterialRequestId] IS NULL AND [MaterialRequestRound].[MaterialRequestId] IS NOT NULL) OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestRound].[MaterialRequestId] IS NOT NULL) AND [Source].[MaterialRequestId] != [MaterialRequestRound].[MaterialRequestId]))
		OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequestRound].[OutputMaterialName] IS NULL) OR ([Source].[OutputMaterialName] IS NULL AND [MaterialRequestRound].[OutputMaterialName] IS NOT NULL) OR (([Source].[OutputMaterialName] IS NOT NULL AND [MaterialRequestRound].[OutputMaterialName] IS NOT NULL) AND [Source].[OutputMaterialName] != [MaterialRequestRound].[OutputMaterialName]))
		OR (([Source].[DueDate] IS NOT NULL AND [MaterialRequestRound].[DueDate] IS NULL) OR ([Source].[DueDate] IS NULL AND [MaterialRequestRound].[DueDate] IS NOT NULL) OR (([Source].[DueDate] IS NOT NULL AND [MaterialRequestRound].[DueDate] IS NOT NULL) AND [Source].[DueDate] != [MaterialRequestRound].[DueDate]))
		OR (([Source].[RequestedDate] IS NOT NULL AND [MaterialRequestRound].[RequestedDate] IS NULL) OR ([Source].[RequestedDate] IS NULL AND [MaterialRequestRound].[RequestedDate] IS NOT NULL) OR (([Source].[RequestedDate] IS NOT NULL AND [MaterialRequestRound].[RequestedDate] IS NOT NULL) AND [Source].[RequestedDate] != [MaterialRequestRound].[RequestedDate]))
		
		OR (([Source].[SubmittedDate] IS NOT NULL AND [MaterialRequestRound].[SubmittedDate] IS NULL) OR ([Source].[SubmittedDate] IS NULL AND [MaterialRequestRound].[SubmittedDate] IS NOT NULL) OR (([Source].[SubmittedDate] IS NOT NULL AND [MaterialRequestRound].[SubmittedDate] IS NOT NULL) AND [Source].[SubmittedDate] != [MaterialRequestRound].[SubmittedDate]))
		OR (([Source].[Status] IS NOT NULL AND [MaterialRequestRound].[Status] IS NULL) OR ([Source].[Status] IS NULL AND [MaterialRequestRound].[Status] IS NOT NULL) OR (([Source].[Status] IS NOT NULL AND [MaterialRequestRound].[Status] IS NOT NULL) AND [Source].[Status] != [MaterialRequestRound].[Status]))		
			
	--select * from @MaterialRequestRoundHistory
	
	BEGIN TRANSACTION	
	   --Merge operation delete
		MERGE MaterialRequestRoundHistory AS Target USING(select * from @MaterialRequestRoundHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[MaterialRequestRoundId] = Source.[MaterialRequestRoundId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE MaterialRequestRoundHistory AS Target USING(	select * from @MaterialRequestRoundHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MaterialRequestRoundId] = Source.[MaterialRequestRoundId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE MaterialRequestRoundHistory AS Target USING(	select * from @MaterialRequestRoundHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MaterialRequestRoundId] = Source.[MaterialRequestRoundId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.MaterialRequestRoundId = Source.MaterialRequestRoundId
		  ,Target.MaterialRequestId = Source.MaterialRequestId
		  ,Target.OutputMaterialName = Source.OutputMaterialName
		  ,Target.DueDate = Source.DueDate
		  ,Target.RequestedDate = Source.RequestedDate
		  ,Target.SubmittedDate = Source.SubmittedDate      
		  ,Target.Status = Source.Status	 

		  ,Target.ModifiedDate = @Date		  
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([MaterialRequestRoundId],[MaterialRequestId],[OutputMaterialName],[DueDate],[RequestedDate],[SubmittedDate],[Status],[ModifiedDate],[RowStatus])
			VALUES (Source.[MaterialRequestRoundId],Source.[MaterialRequestId],Source.[OutputMaterialName],Source.[DueDate],Source.[RequestedDate],Source.[SubmittedDate],Source.[Status],@Date,'Latest');
	 	  	
	COMMIT TRANSACTION;	

END