Alter VIEW [dbo].[TestSessionRolePlayerDetails] AS
SELECT
	 [TestSessionRolePlayerDetailId]
	,[TestSessionId]	
	,[TestSessionName]
	,[TestDate]	
	,[PersonId]
	,[CustomerNo]
	,[Status]
	,[TaskName]
    ,[Language]
    ,[Position]
	,[ModifiedDate]	
FROM [TestSessionRolePlayerDetailsHistory] c
where [RowStatus] = 'Latest'