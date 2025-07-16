ALTER VIEW [PanelMembershipCredentialTypes] AS
SELECT
	 [PanelMemberShipCredentialTypeId]
	,[PanelMembershipId]
	,[PanelId]	
	,[PanelName]
	,[Role]
	,[PersonId]
	,[PersonName]
	,[NaatiNumber]	
	,[CredentialTypeInternalName]
	,[CredentialTypeExternalName]	
	,[ModifiedDate]	
FROM [PanelMembershipCredentialTypesHistory] c
where [RowStatus] = 'Latest'