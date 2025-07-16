ALTER VIEW [dbo].[MaterialRequestPanelMember] AS
SELECT	
     [MaterialRequestPanelMembershipId]
    ,[MaterialRequestId]
	,[OutputMaterialName]
	,[PanelMemberShipId]
	,[MemberType]		
FROM [MaterialRequestPanelMemberHistory] tr
where [RowStatus] = 'Latest'