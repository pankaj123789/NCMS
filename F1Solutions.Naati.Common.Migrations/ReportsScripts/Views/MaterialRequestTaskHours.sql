ALTER VIEW [dbo].[MaterialRequestTaskHours] AS
SELECT
	 [MaterialRequestPanelMembershipTaskId]
	,[MaterialRequestId]
	,[OutputMaterialName]
	,[PanelMemberShipId]
	,[Task]
	,[Hours]	
FROM [MaterialRequestTaskHoursHistory] tr
where [RowStatus] = 'Latest'