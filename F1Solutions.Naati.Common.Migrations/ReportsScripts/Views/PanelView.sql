ALTER VIEW [dbo].[Panel] AS
SELECT
    [PanelId]
    ,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]
    ,[PanelName]
    ,[PanelType]
    ,[Language]
    ,[CommissionedDate]	
FROM [PanelHistory] a
where [RowStatus] = 'Latest'