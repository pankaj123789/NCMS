ALTER VIEW [dbo].[Organisation] AS
SELECT
	[OrganisationId]
	,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]
	,[NAATINumber]
	,[Name]
	,[PrimaryAddress]
	,[Suburb]
	,[Country]
	,[PrimaryPhone]
	,[PrimaryEmail]
	,[TrustedPayer]	
FROM [OrganisationHistory] a
where [RowStatus] = 'Latest'
