ALTER PROCEDURE [dbo].[ReportingSnapshot_ApplicationCustomFields_Latest]
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'ApplicationCustomFieldsLatest')
		BEGIN
			SELECT * INTO ApplicationCustomFieldsLatest FROM ApplicationCustomFieldsHistory WHERE RowStatus = 'Latest'
		END
	ELSE
		BEGIN	
		BEGIN TRANSACTION


				MERGE ApplicationCustomFieldsLatest AS Target USING(	select * from ApplicationCustomFieldsHistory where [RowStatus] = 'Latest' ) AS Source ON Target.ApplicationCustomFieldId = Source.ApplicationCustomFieldId AND Target.RowStatus = 'Latest' 
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
				  ,Target.[RowStatus] = 'Latest'
				WHEN NOT MATCHED THEN 
				 INSERT([ApplicationCustomFieldId], [PersonId], [ApplicationId],[ApplicationType],[ApplicationStatus],[ApplicationStatusModifiedDate],[ApplicationEnteredDate],[Section],[FieldName],[Type],[Value], ModifiedDate, [RowStatus]) 
				 VALUES (Source.[ApplicationCustomFieldId],Source.[PersonId],Source.[ApplicationId],Source.[ApplicationType],Source.[ApplicationStatus],Source.[ApplicationStatusModifiedDate],Source.[ApplicationEnteredDate],Source.[Section],Source.[FieldName],Source.[Type],Source.[Value],GetDate(), 'Latest');
	 	  	
		COMMIT TRANSACTION;
		END
END
