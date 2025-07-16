ALTER PROCEDURE [dbo].[ReportingSnapshop_EndorsedQualification]
	@Date DateTime
AS
BEGIN
	DECLARE @EndorsedQualificationHistory as table([EndorsedQualificationId] [int] NOT NULL,[InstitutionId] [int] NOT NULL,[NAATINumber] [int] NOT NULL,[InstitutionName] [nvarchar](100) NOT NULL,[Location] [nvarchar](200) NOT NULL,[Qualification] [nvarchar](200) NOT NULL,[CredentialTypeId] [int] NOT NULL,
												   [CredentialTypeInternalName] [nvarchar](50) NOT NULL,[CredentialTypeExternalName] [nvarchar](50) NOT NULL,[EndorsementPeriodFrom] [datetime] NOT NULL,[EndorsementPeriodTo] [datetime] NOT NULL,[Notes] [nvarchar](1000) NULL,[Active] [bit] NOT NULL,
												   [ModifiedDate] [datetime] NOT NULL,	[RowStatus] [nvarchar](50) NOT NULL)

	
	IF(@Date is NULL)
	SET @Date = GETDATE()
	SET @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @EndorsedQualificationHistory
	SELECT
		 CASE WHEN [Source].[EndorsedQualificationId] IS NULL THEN [EndorsedQualification].[EndorsedQualificationId] ELSE [Source].[EndorsedQualificationId] END AS [EndorsedQualificationId]
		,CASE WHEN [Source].[InstitutionId] IS NULL THEN [EndorsedQualification].[InstitutionId] ELSE [Source].[InstitutionId] END AS [InstitutionId]
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [EndorsedQualification].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[InstitutionName] IS NULL THEN [EndorsedQualification].[InstitutionName] ELSE [Source].[InstitutionName] END AS [InstitutionName]
		,CASE WHEN [Source].[Location] IS NULL THEN [EndorsedQualification].[Location] ELSE [Source].[Location] END AS [Location]
		,CASE WHEN [Source].[Qualification] IS NULL THEN [EndorsedQualification].[Qualification] ELSE [Source].[Qualification] END AS [Qualification]
		,CASE WHEN [Source].[CredentialTypeId] IS NULL THEN [EndorsedQualification].[CredentialTypeId] ELSE [Source].[CredentialTypeId] END AS [CredentialTypeId]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [EndorsedQualification].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [EndorsedQualification].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[EndorsementPeriodFrom] IS NULL THEN [EndorsedQualification].[EndorsementPeriodFrom] ELSE [Source].[EndorsementPeriodFrom] END AS [EndorsementPeriodFrom]
		,CASE WHEN [Source].[EndorsementPeriodTo] IS NULL THEN [EndorsedQualification].[EndorsementPeriodTo] ELSE [Source].[EndorsementPeriodTo] END AS [EndorsementPeriodTo]
		,CASE WHEN [Source].[Notes] IS NULL THEN [EndorsedQualification].[Notes] ELSE [Source].[Notes] END AS [Notes]
		,CASE WHEN [Source].[Active] IS NULL THEN [EndorsedQualification].[Active] ELSE [Source].[Active] END AS [Active]

		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[EndorsedQualificationId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
							
	FROM
	(
		SELECT 
		eq.EndorsedQualificationId AS [EndorsedQualificationId],
		eq.InstitutionId AS [InstitutionId],
		e.[NAATINumber] AS [NAATINumber],
		ina.[Name] AS [InstitutionName],
		eq.[Location] AS [Location],
		eq.Qualification AS [Qualification],
		eq.CredentialTypeId AS [CredentialTypeId],
		ct.InternalName AS [CredentialTypeInternalName],
		ct.ExternalName AS [CredentialTypeExternalName],
		eq.EndorsementPeriodFrom AS [EndorsementPeriodFrom],
		eq.EndorsementPeriodTo AS [EndorsementPeriodTo],
		eq.Notes AS [Notes],
		eq.Active AS [Active]

		FROM [naati_db]..tblEndorsedQualification eq
		LEFT JOIN [naati_db]..tblInstitution i ON eq.InstitutionId = i.InstitutionId
		LEFT JOIN [naati_db]..[tblEntity] e ON i.[EntityId] = e.[EntityId]
		LEFT JOIN [naati_db]..[vwDistinctInstitutionName] distinctIna ON i.[InstitutionId] = distinctIna.[InstitutionId]
		LEFT JOIN [naati_db]..[tblInstitutionName] ina ON distinctIna.[InstitutionNameId] = ina.[InstitutionNameId]
		LEFT JOIN [naati_db]..[tblCredentialType] ct on eq.CredentialTypeId = ct.CredentialTypeId
	) [Source]
	FULL OUTER JOIN [EndorsedQualification] ON [Source].[EndorsedQualificationId] = [EndorsedQualification].[EndorsedQualificationId]

	WHERE 
	(([Source].[EndorsedQualificationId] IS NOT NULL AND [EndorsedQualification].[EndorsedQualificationId] IS NULL) OR ([Source].[EndorsedQualificationId] IS NULL AND [EndorsedQualification].[EndorsedQualificationId] IS NOT NULL) OR (([Source].[EndorsedQualificationId] IS NOT NULL AND [EndorsedQualification].[EndorsedQualificationId] IS NOT NULL) AND [Source].[EndorsedQualificationId] != [EndorsedQualification].[EndorsedQualificationId]))
	OR (([Source].[InstitutionId] IS NOT NULL AND [EndorsedQualification].[InstitutionId] IS NULL) OR ([Source].[InstitutionId] IS NULL AND [EndorsedQualification].[InstitutionId] IS NOT NULL) OR (([Source].[InstitutionId] IS NOT NULL AND [EndorsedQualification].[InstitutionId] IS NOT NULL) AND [Source].[InstitutionId] != [EndorsedQualification].[InstitutionId]))
	OR (([Source].[NAATINumber] IS NOT NULL AND [EndorsedQualification].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [EndorsedQualification].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [EndorsedQualification].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [EndorsedQualification].[NAATINumber]))
	OR (([Source].[InstitutionName] IS NOT NULL AND [EndorsedQualification].[InstitutionName] IS NULL) OR ([Source].[InstitutionName] IS NULL AND [EndorsedQualification].[InstitutionName] IS NOT NULL) OR (([Source].[InstitutionName] IS NOT NULL AND [EndorsedQualification].[InstitutionName] IS NOT NULL) AND [Source].[InstitutionName] != [EndorsedQualification].[InstitutionName]))
	OR (([Source].[Location] IS NOT NULL AND [EndorsedQualification].[Location] IS NULL) OR ([Source].[Location] IS NULL AND [EndorsedQualification].[Location] IS NOT NULL) OR (([Source].[Location] IS NOT NULL AND [EndorsedQualification].[Location] IS NOT NULL) AND [Source].[Location] != [EndorsedQualification].[Location]))
	OR (([Source].[Qualification] IS NOT NULL AND [EndorsedQualification].[Qualification] IS NULL) OR ([Source].[Qualification] IS NULL AND [EndorsedQualification].[Qualification] IS NOT NULL) OR (([Source].[Qualification] IS NOT NULL AND [EndorsedQualification].[Qualification] IS NOT NULL) AND [Source].[Qualification] != [EndorsedQualification].[Qualification]))
	OR (([Source].[CredentialTypeId] IS NOT NULL AND [EndorsedQualification].[CredentialTypeId] IS NULL) OR ([Source].[CredentialTypeId] IS NULL AND [EndorsedQualification].[CredentialTypeId] IS NOT NULL) OR (([Source].[CredentialTypeId] IS NOT NULL AND [EndorsedQualification].[CredentialTypeId] IS NOT NULL) AND [Source].[CredentialTypeId] != [EndorsedQualification].[CredentialTypeId]))
	OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [EndorsedQualification].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [EndorsedQualification].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [EndorsedQualification].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [EndorsedQualification].[CredentialTypeInternalName]))
	OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [EndorsedQualification].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [EndorsedQualification].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [EndorsedQualification].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [EndorsedQualification].[CredentialTypeExternalName]))
	OR (([Source].[EndorsementPeriodFrom] IS NOT NULL AND [EndorsedQualification].[EndorsementPeriodFrom] IS NULL) OR ([Source].[EndorsementPeriodFrom] IS NULL AND [EndorsedQualification].[EndorsementPeriodFrom] IS NOT NULL) OR (([Source].[EndorsementPeriodFrom] IS NOT NULL AND [EndorsedQualification].[EndorsementPeriodFrom] IS NOT NULL) AND [Source].[EndorsementPeriodFrom] != [EndorsedQualification].[EndorsementPeriodFrom]))
	OR (([Source].[EndorsementPeriodTo] IS NOT NULL AND [EndorsedQualification].[EndorsementPeriodTo] IS NULL) OR ([Source].[EndorsementPeriodTo] IS NULL AND [EndorsedQualification].[EndorsementPeriodTo] IS NOT NULL) OR (([Source].[EndorsementPeriodTo] IS NOT NULL AND [EndorsedQualification].[EndorsementPeriodTo] IS NOT NULL) AND [Source].[EndorsementPeriodTo] != [EndorsedQualification].[EndorsementPeriodTo]))
	OR (([Source].[Notes] IS NOT NULL AND [EndorsedQualification].[Notes] IS NULL) OR ([Source].[Notes] IS NULL AND [EndorsedQualification].[Notes] IS NOT NULL) OR (([Source].[Notes] IS NOT NULL AND [EndorsedQualification].[Notes] IS NOT NULL) AND [Source].[Notes] != [EndorsedQualification].[Notes]))
	OR (([Source].[Active] IS NOT NULL AND [EndorsedQualification].[Active] IS NULL) OR ([Source].[Active] IS NULL AND [EndorsedQualification].[Active] IS NOT NULL) OR (([Source].[Active] IS NOT NULL AND [EndorsedQualification].[Active] IS NOT NULL) AND [Source].[Active] != [EndorsedQualification].[Active]))	

  --select * from @EndorsedQualificationHistory
    
  BEGIN TRANSACTION 
  
	   --Merge operation delete
		MERGE EndorsedQualificationHistory AS Target USING(select * from @EndorsedQualificationHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[EndorsedQualificationId] = Source.[EndorsedQualificationId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE EndorsedQualificationHistory AS Target USING(	select * from @EndorsedQualificationHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[EndorsedQualificationId] = Source.[EndorsedQualificationId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE EndorsedQualificationHistory AS Target USING(	select * from @EndorsedQualificationHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[EndorsedQualificationId] = Source.[EndorsedQualificationId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.[EndorsedQualificationId] = Source.[EndorsedQualificationId]
		  ,Target.[InstitutionId] = Source.[InstitutionId]
		  ,Target.[NAATINumber] = Source.[NAATINumber]
		  ,Target.[InstitutionName] = Source.[InstitutionName]
		  ,Target.[Location] = Source.[Location]
		  ,Target.[Qualification] = Source.[Qualification]      
		  ,Target.[CredentialTypeId] = Source.[CredentialTypeId]
		  ,Target.[CredentialTypeInternalName] = Source.[CredentialTypeInternalName]
		  ,Target.[CredentialTypeExternalName] = Source.[CredentialTypeExternalName]
		  ,Target.[EndorsementPeriodFrom] = Source.[EndorsementPeriodFrom]
		  ,Target.[EndorsementPeriodTo] = Source.[EndorsementPeriodTo]

		  ,Target.[ModifiedDate] = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 

		  INSERT([EndorsedQualificationId],[InstitutionId],[NAATINumber],[InstitutionName],[Location],[Qualification],[CredentialTypeId],[CredentialTypeInternalName],[CredentialTypeExternalName],
				 [EndorsementPeriodFrom],[EndorsementPeriodTo],[Notes],[Active],[ModifiedDate],[RowStatus])
		  
		  VALUES (Source.[EndorsedQualificationId],Source.[InstitutionId],Source.[NAATINumber],Source.[InstitutionName],Source.[Location],Source.[Qualification],Source.[CredentialTypeId],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],
				  Source.[EndorsementPeriodFrom],Source.[EndorsementPeriodTo],Source.[Notes],Source.[Active], @Date, 'Latest');

	     COMMIT TRANSACTION;

END
