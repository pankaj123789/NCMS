ALTER VIEW [dbo].[ApplicationCustomFields] AS
SELECT
	[ApplicationCustomFieldId] 
	,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]	
	,[PersonId]
	,[ApplicationId]
	,[ApplicationType]
	,[ApplicationStatus]
	,[ApplicationStatusModifiedDate]
	,[ApplicationEnteredDate]
	,[Section]
	,[FieldName]
	,[Type]
	,[Value]	
FROM [ApplicationCustomFieldsLatest] a
--FROM [ApplicationCustomFieldsHistory] a
--where [RowStatus] = 'Latest'