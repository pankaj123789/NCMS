ALTER VIEW [dbo].[MaterialRequestRound] AS
SELECT
	 [MaterialRequestRoundId]
    ,[MaterialRequestId]
	,[OutputMaterialName]
	,[DueDate]
	,[RequestedDate]
	,[SubmittedDate]
	,[Status]		
FROM [MaterialRequestRoundHistory] tr
where [RowStatus] = 'Latest'