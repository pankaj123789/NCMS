ALTER PROCEDURE [dbo].[ReportingSnapshot_Panel]
	@Date DateTime
AS
BEGIN
	DECLARE @PanelHistory as table([PanelId] [int] NOT NULL,[PanelName] [nvarchar](100) NULL,[PanelType] [nvarchar](50) NULL,[Language] [nvarchar](50) NULL,[CommissionedDate] [datetime] NULL,	ModifiedDate datetime not null DEFAULT GETDATE(), [RowStatus] nvarchar(50))   
	
	
	IF(@Date is NULL)
	set @Date = GETDATE()
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @PanelHistory
SELECT
		CASE WHEN [Source].[PanelId] IS NULL THEN [Panel].[PanelId] ELSE [Source].[PanelId] END AS [PanelId]
		,CASE WHEN [Source].[PanelId] IS NULL THEN [Panel].[PanelName] ELSE [Source].[PanelName] END AS [PanelName]
		,CASE WHEN [Source].[PanelId] IS NULL THEN [Panel].[PanelType] ELSE [Source].[PanelType] END AS [PanelType]
		,CASE WHEN [Source].[PanelId] IS NULL THEN [Panel].[Language] ELSE [Source].[Language] END AS [Language]
		,CASE WHEN [Source].[PanelId] IS NULL THEN [Panel].[CommissionedDate] ELSE [Source].[CommissionedDate] END AS [CommissionedDate]
		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[PanelId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
	FROM
	(
		SELECT
			p.[PanelId]
			,p.[Name] COLLATE Latin1_General_CI_AS  AS 'PanelName'
			,pt.[Name] COLLATE Latin1_General_CI_AS  AS 'PanelType'
			,l.[Name] COLLATE Latin1_General_CI_AS  AS 'Language'
			,p.[CommissionedDate] AS 'CommissionedDate'
			
		FROM [naati_db]..[tblPanel] p
		LEFT JOIN [naati_db]..[tluPanelType] pt ON p.[PanelTypeId] = pt.[PanelTypeId]
		LEFT JOIN [naati_db]..[tblLanguage] l ON p.[LanguageId] = l.[LanguageId]
	) [Source]
	FULL OUTER JOIN [Panel] ON [Source].[PanelId] = [Panel].[PanelId]
	WHERE (([Source].[PanelName] IS NOT NULL AND [Panel].[PanelName] IS NULL) OR ([Source].[PanelName] IS NULL AND [Panel].[PanelName] IS NOT NULL) OR (([Source].[PanelName] IS NOT NULL AND [Panel].[PanelName] IS NOT NULL) AND [Source].[PanelName] != [Panel].[PanelName]))
	OR (([Source].[PanelType] IS NOT NULL AND [Panel].[PanelType] IS NULL) OR ([Source].[PanelType] IS NULL AND [Panel].[PanelType] IS NOT NULL) OR (([Source].[PanelType] IS NOT NULL AND [Panel].[PanelType] IS NOT NULL) AND [Source].[PanelType] != [Panel].[PanelType]))
	OR (([Source].[Language] IS NOT NULL AND [Panel].[Language] IS NULL) OR ([Source].[Language] IS NULL AND [Panel].[Language] IS NOT NULL) OR (([Source].[Language] IS NOT NULL AND [Panel].[Language] IS NOT NULL) AND [Source].[Language] != [Panel].[Language]))
	OR (([Source].[CommissionedDate] IS NOT NULL AND [Panel].[CommissionedDate] IS NULL) OR ([Source].[CommissionedDate] IS NULL AND [Panel].[CommissionedDate] IS NOT NULL) OR (([Source].[CommissionedDate] IS NOT NULL AND [Panel].[CommissionedDate] IS NOT NULL) AND [Source].[CommissionedDate] != [Panel].[CommissionedDate]))
		
	--select * from @PanelHistory

	
	BEGIN TRANSACTION
	 
	   --Merge operation delete
		MERGE PanelHistory AS Target USING(select * from @PanelHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[PanelId] = Source.[PanelId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		 	 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE PanelHistory AS Target USING(	select * from @PanelHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[PanelId] = Source.[PanelId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
			 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE PanelHistory AS Target USING(	select * from @PanelHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[PanelId] = Source.[PanelId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.[PanelId] = Source.[PanelId]
		  ,Target.[PanelName] = Source.[PanelName]
		  ,Target.[PanelType] = Source.[PanelType]
		  ,Target.[Language] = Source.[Language]
		  ,Target.[CommissionedDate] = Source.[CommissionedDate]		  
		  ,Target.[ModifiedDate] = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		  INSERT(PanelId,PanelName,PanelType,[Language],CommissionedDate,ModifiedDate,[RowStatus]) VALUES (Source.PanelId,Source.PanelName,Source.PanelType,Source.[Language],Source.CommissionedDate,@Date, 'Latest');
		  	
	COMMIT TRANSACTION;		
	
END
