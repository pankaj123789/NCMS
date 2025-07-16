ALTER VIEW [dbo].[MaterialRequest] AS
SELECT
	 [MaterialRequestId]    
	,[PanelId]	
	,[Panel]
	,[SourceMaterialId]
	,[SourceMaterialName]
	,[OutputMaterialId]
	,[OutputMaterialName]
	,[CredentialType]
	,[Skill]
	,[Status]
	,[NumberOfRounds]
	,[MaxBillableHours]
	,[ProductSpecificationName]
	,[GLCode]
	,[CostPerHour]
	,[CreatedDate]
	,[CreatedBy]
	,[OwnedBy]	
FROM [MaterialRequestHistory] tr
where [RowStatus] = 'Latest'