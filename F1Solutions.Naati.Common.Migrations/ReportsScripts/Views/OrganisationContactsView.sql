ALTER VIEW [dbo].[OrganisationContacts] AS
SELECT
	[ContactPersonId]
	,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]
	,[OrganisationId]
	,[Name]
	,[Email]
	,[Phone]
	,[Address]
	,[Description]
	,[Inactive]	
FROM [OrganisationContactsHistory] 
where [RowStatus] = 'Latest'
