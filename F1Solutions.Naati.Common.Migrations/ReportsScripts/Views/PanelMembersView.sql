ALTER VIEW [dbo].[PanelMembers] AS
SELECT
    [PanelId]
	,[PanelMembersId]
	,[PersonId]
    ,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]
    ,[PersonName]
    ,[NAATINumber]
    ,[Role]
    ,[StartDate]
    ,[EndDate]	
FROM [PanelMembersHistory] a
where [RowStatus] = 'Latest'
