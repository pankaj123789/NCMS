ALTER PROCEDURE [dbo].[ReportingSnapshot_ApplicationCustomFIelds]
	@Date DateTime
AS
BEGIN
DECLARE @ApplicationCustomFieldsHistory as table([ApplicationCustomFieldId] [int] NOT NULL,[PersonId] [int] NOT NULL,[ApplicationId] [int] NOT NULL,[ApplicationType] [nvarchar](50) NOT NULL,[ApplicationStatus] [nvarchar](50) NOT NULL,
		[ApplicationStatusModifiedDate] [datetime] NOT NULL,[ApplicationEnteredDate] [datetime] NOT NULL,[Section] [nvarchar](100) NULL,[FieldName] [nvarchar](50) NOT NULL,[Type] [nvarchar](50) NOT NULL,	[Value] [nvarchar](max) NULL,
		ModifiedDate datetime not null DEFAULT GETDATE(), [RowStatus] nvarchar(50))   
				
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @ApplicationCustomFieldsHistory

	SELECT
		CASE WHEN [Source].[ApplicationCustomFieldId] IS NULL THEN [ApplicationCustomFields].[ApplicationCustomFieldId] ELSE [Source].[ApplicationCustomFieldId] END AS [ApplicationCustomFieldId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [ApplicationCustomFields].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [ApplicationCustomFields].[ApplicationId] ELSE [Source].[ApplicationId] END AS [ApplicationId]
		,CASE WHEN [Source].[ApplicationType] IS NULL THEN [ApplicationCustomFields].[ApplicationType] ELSE [Source].[ApplicationType] END AS [ApplicationType]
		,CASE WHEN [Source].[ApplicationStatus] IS NULL THEN [ApplicationCustomFields].[ApplicationStatus] ELSE [Source].[ApplicationStatus] END AS [ApplicationStatus]
		,CASE WHEN [Source].[ApplicationStatusModifiedDate] IS NULL THEN [ApplicationCustomFields].[ApplicationStatusModifiedDate] ELSE [Source].[ApplicationStatusModifiedDate] END AS [ApplicationStatusModifiedDate]
		,CASE WHEN [Source].[ApplicationEnteredDate] IS NULL THEN [ApplicationCustomFields].[ApplicationEnteredDate] ELSE [Source].[ApplicationEnteredDate] END AS [ApplicationEnteredDate]
		,CASE WHEN [Source].[Section] IS NULL THEN [ApplicationCustomFields].[Section] ELSE [Source].[Section] END AS [Section]
		,CASE WHEN [Source].[FieldName] IS NULL THEN [ApplicationCustomFields].[FieldName] ELSE [Source].[FieldName] END AS [FieldName]
		,CASE WHEN [Source].[Type] IS NULL THEN [ApplicationCustomFields].[Type] ELSE [Source].[Type] END AS [Type]
		,CASE WHEN [Source].[Value] IS NULL THEN [ApplicationCustomFields].[Value] ELSE [Source].[Value] END AS [Value]	
		
		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[ApplicationCustomFieldId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM
	(
		SELECT
			afd.[CredentialApplicationFieldDataId] as 'ApplicationCustomFieldId'
			,ap.[PersonId]
			,afd.[CredentialApplicationId] as 'ApplicationId'
			,apt.[DisplayName] as 'ApplicationType'
			,aps.[DisplayName] as 'ApplicationStatus'
			,ap.[StatusChangeDate] as 'ApplicationStatusModifiedDate'
			,ap.[EnteredDate] as 'ApplicationEnteredDate'
			,aft.[Section] 
			,aft.[Name] as 'FieldName' 
			,dt.[DisplayName] as 'Type' 
			,afd.[Value] as 'Value' 
			
		FROM [naati_db]..[tblCredentialApplicationFieldData] afd
		INNER JOIN [naati_db]..[tblCredentialApplication] ap ON afd.CredentialApplicationId = ap.CredentialApplicationId
		INNER JOIN [naati_db]..[tblCredentialApplicationType] apt ON ap.CredentialApplicationTypeId = apt.CredentialApplicationTypeId
		INNER JOIN [naati_db]..[tblCredentialApplicationStatusType] aps ON ap.CredentialApplicationStatusTypeId = aps.CredentialApplicationStatusTypeId
		INNER JOIN [naati_db]..[tblCredentialApplicationField] aft ON afd.CredentialApplicationFieldId = aft.CredentialApplicationFieldId
		INNER JOIN [naati_db]..[tblDataType] dt ON aft.DataTypeId = dt.DataTypeId
		WHERE afT.Reportable = 1
	) [Source]
	FULL OUTER JOIN [ApplicationCustomFields] ON [Source].[ApplicationCustomFieldId] = [ApplicationCustomFields].[ApplicationCustomFieldId]
	WHERE (([Source].[PersonId] IS NOT NULL AND [ApplicationCustomFields].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [ApplicationCustomFields].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [ApplicationCustomFields].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [ApplicationCustomFields].[PersonId]))
	OR (([Source].[ApplicationId] IS NOT NULL AND [ApplicationCustomFields].[ApplicationId] IS NULL) OR ([Source].[ApplicationId] IS NULL AND [ApplicationCustomFields].[ApplicationId] IS NOT NULL) OR (([Source].[ApplicationId] IS NOT NULL AND [ApplicationCustomFields].[ApplicationId] IS NOT NULL) AND [Source].[ApplicationId] != [ApplicationCustomFields].[ApplicationId]))
	OR (([Source].[ApplicationType] IS NOT NULL AND [ApplicationCustomFields].[ApplicationType] IS NULL) OR ([Source].[ApplicationType] IS NULL AND [ApplicationCustomFields].[ApplicationType] IS NOT NULL) OR (([Source].[ApplicationType] IS NOT NULL AND [ApplicationCustomFields].[ApplicationType] IS NOT NULL) AND [Source].[ApplicationType] != [ApplicationCustomFields].[ApplicationType]))
	OR (([Source].[ApplicationStatus] IS NOT NULL AND [ApplicationCustomFields].[ApplicationStatus] IS NULL) OR ([Source].[ApplicationStatus] IS NULL AND [ApplicationCustomFields].[ApplicationStatus] IS NOT NULL) OR (([Source].[ApplicationStatus] IS NOT NULL AND [ApplicationCustomFields].[ApplicationStatus] IS NOT NULL) AND [Source].[ApplicationStatus] != [ApplicationCustomFields].[ApplicationStatus]))	
	OR (([Source].[ApplicationStatusModifiedDate] IS NOT NULL AND [ApplicationCustomFields].[ApplicationStatusModifiedDate] IS NULL) OR ([Source].[ApplicationStatusModifiedDate] IS NULL AND [ApplicationCustomFields].[ApplicationStatusModifiedDate] IS NOT NULL) OR (([Source].[ApplicationStatusModifiedDate] IS NOT NULL AND [ApplicationCustomFields].[ApplicationStatusModifiedDate] IS NOT NULL) AND [Source].[ApplicationStatusModifiedDate] != [ApplicationCustomFields].[ApplicationStatusModifiedDate]))
	OR (([Source].[ApplicationEnteredDate] IS NOT NULL AND [ApplicationCustomFields].[ApplicationEnteredDate] IS NULL) OR ([Source].[ApplicationEnteredDate] IS NULL AND [ApplicationCustomFields].[ApplicationEnteredDate] IS NOT NULL) OR (([Source].[ApplicationEnteredDate] IS NOT NULL AND [ApplicationCustomFields].[ApplicationEnteredDate] IS NOT NULL) AND [Source].[ApplicationEnteredDate] != [ApplicationCustomFields].[ApplicationEnteredDate]))
	OR (([Source].[Section] IS NOT NULL AND [ApplicationCustomFields].[Section] IS NULL) OR ([Source].[Section] IS NULL AND [ApplicationCustomFields].[Section] IS NOT NULL) OR (([Source].[Section] IS NOT NULL AND [ApplicationCustomFields].[Section] IS NOT NULL) AND [Source].[Section] != [ApplicationCustomFields].[Section]))
	OR (([Source].[FieldName] IS NOT NULL AND [ApplicationCustomFields].[FieldName] IS NULL) OR ([Source].[FieldName] IS NULL AND [ApplicationCustomFields].[FieldName] IS NOT NULL) OR (([Source].[FieldName] IS NOT NULL AND [ApplicationCustomFields].[FieldName] IS NOT NULL) AND [Source].[FieldName] != [ApplicationCustomFields].[FieldName]))
	OR (([Source].[Type] IS NOT NULL AND [ApplicationCustomFields].[Type] IS NULL) OR ([Source].[Type] IS NULL AND [ApplicationCustomFields].[Type] IS NOT NULL) OR (([Source].[Type] IS NOT NULL AND [ApplicationCustomFields].[Type] IS NOT NULL) AND [Source].[Type] != [ApplicationCustomFields].[Type]))
	OR (([Source].[Value] IS NOT NULL AND [ApplicationCustomFields].[Value] IS NULL) OR ([Source].[Value] IS NULL AND [ApplicationCustomFields].[Value] IS NOT NULL) OR (([Source].[Value] IS NOT NULL AND [ApplicationCustomFields].[Value] IS NOT NULL) AND [Source].[Value] != [ApplicationCustomFields].[Value]))
		
	--select * from @ApplicationCustomFieldsHistory

	-----------------------------------------------
	BEGIN TRANSACTION
	  --Merge operation delete
		MERGE ApplicationCustomFieldsHistory AS Target USING(select * from @ApplicationCustomFieldsHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.ApplicationCustomFieldId = Source.ApplicationCustomFieldId AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE ApplicationCustomFieldsHistory AS Target USING(	select * from @ApplicationCustomFieldsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.ApplicationCustomFieldId = Source.ApplicationCustomFieldId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE ApplicationCustomFieldsHistory AS Target USING(	select * from @ApplicationCustomFieldsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.ApplicationCustomFieldId = Source.ApplicationCustomFieldId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.[ApplicationCustomFieldId] = Source.[ApplicationCustomFieldId]
		  ,Target.[PersonId] = Source.[PersonId]
		  ,Target.[ApplicationId] = Source.[ApplicationId]
		  ,Target.[ApplicationType] = Source.[ApplicationType]
		  ,Target.[ApplicationStatus] = Source.[ApplicationStatus]
		  ,Target.[ApplicationStatusModifiedDate] = Source.[ApplicationStatusModifiedDate]      
		  ,Target.[ApplicationEnteredDate] = Source.[ApplicationEnteredDate]
		  ,Target.[Section] = Source.[Section]
		  ,Target.[FieldName] = Source.[FieldName]
		  ,Target.[Type] = Source.[Type]
		  ,Target.[Value] = Source.[Value]

		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([ApplicationCustomFieldId], [PersonId], [ApplicationId],[ApplicationType],[ApplicationStatus],[ApplicationStatusModifiedDate],[ApplicationEnteredDate],[Section],[FieldName],[Type],[Value], ModifiedDate, [RowStatus]) 
		 VALUES (Source.[ApplicationCustomFieldId],Source.[PersonId],Source.[ApplicationId],Source.[ApplicationType],Source.[ApplicationStatus],Source.[ApplicationStatusModifiedDate],Source.[ApplicationEnteredDate],Source.[Section],Source.[FieldName],Source.[Type],Source.[Value],@Date, 'Latest');
	 	  	
	COMMIT TRANSACTION;

END
