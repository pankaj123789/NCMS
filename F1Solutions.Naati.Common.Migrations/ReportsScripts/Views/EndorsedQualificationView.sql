ALTER VIEW [dbo].[EndorsedQualification] AS
SELECT
	[EndorsedQualificationId]
	,[InstitutionId]
	,[NAATINumber]
	,[InstitutionName]
	,[Location]
	,[Qualification]
	,[CredentialTypeId]
	,[CredentialTypeInternalName]
	,[CredentialTypeExternalName]
	,[EndorsementPeriodFrom]
	,[EndorsementPeriodTo]
	,[Notes]
	,[Active]
	
FROM [EndorsedQualificationHistory] a
where [RowStatus] = 'Latest'
