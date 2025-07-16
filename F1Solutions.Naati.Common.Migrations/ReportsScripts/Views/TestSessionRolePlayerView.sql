ALTER VIEW [dbo].[TestSessionRolePlayer] AS
SELECT
	 [TestSessionRolePlayerId]
	,[TestSessionId]	
	,[TestSessionName]
	,[TestLocationState]
	,[TestLocationCountry]
	,[TestLocationName]
	,[TestDate]	
	,[CredentialTypeInternalName]
	,[CredentialTypeExternalName]
	,[PersonId]
	,[CustomerNo]
	,[Status]
	,[Rehearsed]
	,[Attended]
	,[Rejected]	
	,[ModifiedDate]		
FROM [TestSessionRolePlayerHistory] c
where [RowStatus] = 'Latest'