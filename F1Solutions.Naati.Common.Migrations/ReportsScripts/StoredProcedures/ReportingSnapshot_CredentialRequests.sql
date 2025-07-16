ALTER PROCEDURE [dbo].[ReportingSnapshot_CredentialRequests]
	@Date DateTime
AS
BEGIN
	DECLARE @CredentialRequestsHistory as table([CredentialRequestId] [int] NOT NULL,[PersonId] [int] NOT NULL,[Title] [nvarchar](50) NULL,[GivenName] [nvarchar](100) NULL,[OtherNames] [nvarchar](100) NULL,[FamilyName] [nvarchar](100) NULL,[PrimaryAddress] [nvarchar](500) NULL,[Country] [nvarchar](50) NULL,
													   [PrimaryEmail] [nvarchar](200) NULL,[PrimaryPhone] [nvarchar](60) NULL,[ApplicationId] [int] NOT NULL,[ApplicationType] [nvarchar](50) NOT NULL,[ApplicationReference] [nvarchar](50) NOT NULL,[ApplicationOwner] [nvarchar](100) NULL,
													   [CredentialTypeInternalName] [nvarchar](50) NOT NULL,[CredentialTypeExternalName] [nvarchar](50) NOT NULL,[Certification] [bit] NOT NULL,[Language1] [nvarchar](50) NOT NULL,[Language2] [nvarchar](50) NOT NULL,[DirectionDisplayName] [nvarchar](105) NOT NULL,
													   [ModifiedDate] [datetime] NOT NULL,[StatusDateModified] [datetime] NULL,[StatusModifiedUser] [nvarchar](100) NOT NULL,[Status] [nvarchar](50) NOT NULL,[NAATINumber] [int] NULL,[PractitionerNumber] [varchar](50) NULL,[Language1Code] [nvarchar](10) NULL,
													   [Language1Group] [nvarchar](50) NULL,[Language2Code] [nvarchar](10) NULL,[Language2Group] [nvarchar](50) NULL,[State] [varchar](3) NULL,[Postcode] [varchar](4) NULL,[CredentialId] [int] NULL,[LinkedCredentialRequestId] [int] NULL,
													   [LinkedCredentialRequestReason] [nvarchar](50) NULL,[RowStatus] nvarchar(50))   
		
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @CredentialRequestsHistory

	SELECT
		CASE WHEN [Source].[CredentialRequestId] IS NULL THEN [CredentialRequests].[CredentialRequestId] ELSE [Source].[CredentialRequestId] END AS [CredentialRequestId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [CredentialRequests].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[Title] IS NULL THEN [CredentialRequests].[Title] ELSE [Source].[Title] END AS [Title]
		,CASE WHEN [Source].[GivenName] IS NULL THEN [CredentialRequests].[GivenName] ELSE [Source].[GivenName] END AS [GivenName]
		,CASE WHEN [Source].[OtherNames] IS NULL THEN [CredentialRequests].[OtherNames] ELSE [Source].[OtherNames] END AS [OtherNames]
		,CASE WHEN [Source].[FamilyName] IS NULL THEN [CredentialRequests].[FamilyName] ELSE [Source].[FamilyName] END AS [FamilyName]
		,CASE WHEN [Source].[PrimaryAddress] IS NULL THEN [CredentialRequests].[PrimaryAddress] ELSE [Source].[PrimaryAddress] END AS [PrimaryAddress]
		,CASE WHEN [Source].[Country] IS NULL THEN [CredentialRequests].[Country] ELSE [Source].[Country] END AS [Country]
		,CASE WHEN [Source].[PrimaryEmail] IS NULL THEN [CredentialRequests].[PrimaryEmail] ELSE [Source].[PrimaryEmail] END AS [PrimaryEmail]
		,CASE WHEN [Source].[PrimaryPhone] IS NULL THEN [CredentialRequests].[PrimaryPhone] ELSE [Source].[PrimaryPhone] END AS [PrimaryPhone]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [CredentialRequests].[ApplicationId] ELSE [Source].[ApplicationId] END AS [ApplicationId]
		,CASE WHEN [Source].[ApplicationType] IS NULL THEN [CredentialRequests].[ApplicationType] ELSE [Source].[ApplicationType] END AS [ApplicationType]
		,CASE WHEN [Source].[ApplicationReference] IS NULL THEN [CredentialRequests].[ApplicationReference] ELSE [Source].[ApplicationReference] END AS [ApplicationReference]
		,CASE WHEN [Source].[ApplicationOwner] IS NULL THEN [CredentialRequests].[ApplicationOwner] ELSE [Source].[ApplicationOwner] END AS [ApplicationOwner]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [CredentialRequests].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [CredentialRequests].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[Certification] IS NULL THEN [CredentialRequests].[Certification] ELSE [Source].[Certification] END AS [Certification]
		,CASE WHEN [Source].[Language1] IS NULL THEN [CredentialRequests].[Language1] ELSE [Source].[Language1] END AS [Language1]
		,CASE WHEN [Source].[Language2] IS NULL THEN [CredentialRequests].[Language2] ELSE [Source].[Language2] END AS [Language2]
		,CASE WHEN [Source].[DirectionDisplayName] IS NULL THEN [CredentialRequests].[DirectionDisplayName] ELSE [Source].[DirectionDisplayName] END AS [DirectionDisplayName]
		
		,@Date AS [ModifiedDate]
		
		,CASE WHEN [Source].[StatusDateModified] IS NULL THEN [CredentialRequests].[StatusDateModified] ELSE [Source].[StatusDateModified] END AS [StatusDateModified]
		,CASE WHEN [Source].[StatusModifiedUser] IS NULL THEN [CredentialRequests].[StatusModifiedUser] ELSE [Source].[StatusModifiedUser] END AS [StatusModifiedUser]
		,CASE WHEN [Source].[Status] IS NULL THEN [CredentialRequests].[Status] ELSE [Source].[Status] END AS [Status]
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [CredentialRequests].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [CredentialRequests].[PractitionerNumber] ELSE [Source].[PractitionerNumber] END AS [PractitionerNumber]
		,CASE WHEN [Source].[Language1Code] IS NULL THEN [CredentialRequests].[Language1Code] ELSE [Source].[Language1Code] END AS [Language1Code]
		,CASE WHEN [Source].[Language1Group] IS NULL THEN [CredentialRequests].[Language1Group] ELSE [Source].[Language1Group] END AS [Language1Group]
		,CASE WHEN [Source].[Language2Code] IS NULL THEN [CredentialRequests].[Language2Code] ELSE [Source].[Language2Code] END AS [Language2Code]
		,CASE WHEN [Source].[Language2Group] IS NULL THEN [CredentialRequests].[Language2Group] ELSE [Source].[Language2Group] END AS [Language2Group]
		,CASE WHEN [Source].[State] IS NULL THEN [CredentialRequests].[State] ELSE [Source].[State] END AS [State]
		,CASE WHEN [Source].[Postcode] IS NULL THEN [CredentialRequests].[Postcode] ELSE [Source].[Postcode] END AS [Postcode]
		,CASE WHEN [Source].[CredentialId] IS NULL THEN [CredentialRequests].[CredentialId] ELSE [Source].[CredentialId] END AS [CredentialId]
		,CASE WHEN [Source].[LinkedCredentialRequestId] IS NULL THEN [CredentialRequests].[LinkedCredentialRequestId] ELSE [Source].[LinkedCredentialRequestId] END AS [LinkedCredentialRequestId]
		,CASE WHEN [Source].[LinkedCredentialRequestReason] IS NULL THEN [CredentialRequests].[LinkedCredentialRequestReason] ELSE [Source].[LinkedCredentialRequestReason] END AS [LinkedCredentialRequestReason]
				
		,CASE WHEN [Source].[CredentialRequestId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM
	(
		SELECT
			cr.[CredentialRequestId]
			,cap.[CredentialApplicationId] AS 'ApplicationId'
			,p.[PersonId]
			,e.[NAATINumber]
			,p.[PractitionerNumber]
			,pn.[Title]
			,pn.[GivenName]
			,pn.[OtherNames]
			,pn.[SurName] AS 'FamilyName'
			,a.[StreetDetails] AS 'PrimaryAddress'
			,sta.State AS 'State'
			,pc.Postcode AS 'Postcode'
			,(
				SELECT TOP 1
					[Name]
				FROM [naati_db]..[tblCountry] c
				WHERE c.[CountryId] = a.[CountryId]
			) COLLATE Latin1_General_CI_AS AS 'Country'
			,PrimaryPhone.[Number] COLLATE Latin1_General_CI_AS AS 'PrimaryPhone'
			,(SELECT TOP 1 
				[Email]
			  FROM [naati_db]..[tblEmail] email
			  WHERE email.[EntityId] = p.[EntityId] AND email.IsPreferredEmail = 1) COLLATE Latin1_General_CI_AS as 'PrimaryEmail'
			,capt.[DisplayName] AS 'ApplicationType'
			,CONCAT('APP', cap.[CredentialApplicationId]) COLLATE Latin1_General_CI_AS AS 'ApplicationReference'
			,CASE
				WHEN cap.[OwnedByApplicant] = 1 THEN 'Applicant'
				WHEN cap.[OwnedByApplicant] = 0 THEN (SELECT TOP 1 
				  [FullName]
				  FROM [naati_db]..[tblUser] ownedByUser
				  WHERE ownedByUser.[UserId] = cap.[OwnedByUserId])
			END COLLATE Latin1_General_CI_AS AS 'ApplicationOwner'
			,ct.[InternalName] AS 'CredentialTypeInternalName'
			,ct.[ExternalName] AS 'CredentialTypeExternalName'
			,ct.[Certification]
			,l1.[Name] AS 'Language1'
			,l1.[Code] AS 'Language1Code'
			,l1g.[Name] AS 'Language1Group'
			,l2.[Name] AS 'Language2'
			,l2.[Code] AS 'Language2Code'
			,l2g.[Name] AS 'Language2Group'
			,REPLACE(REPLACE(d.[DisplayName], '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) COLLATE Latin1_General_CI_AS AS 'DirectionDisplayName'
			,(SELECT TOP 1 
				  [FullName]
				  FROM [naati_db]..[tblUser] ModifiedUser
				  WHERE ModifiedUser.[UserId] = cr.[StatusChangeUserId]) AS 'StatusModifiedUser'
			,cr.[StatusChangeDate] AS 'StatusDateModified'
			,crst.[DisplayName] AS 'Status'
			,ccr.[CredentialId] AS 'CredentialId'
			,COALESCE(crcr1.[AssociatedCredentialRequestId], crcr2.[OriginalCredentialRequestId]) AS 'LinkedCredentialRequestId'
			,COALESCE(crat1.[DisplayName], CASE WHEN crat2.[CredentialRequestAssociationTypeId] IS NULL THEN NULL ELSE 'Original' END) AS 'LinkedCredentialRequestReason'
			
		FROM [naati_db]..[tblCredentialRequest] cr
		INNER JOIN [naati_db]..[tblCredentialApplication] cap ON cr.CredentialApplicationId = cap.CredentialApplicationId
		INNER JOIN [naati_db]..[tblPerson] p ON cap.PersonId = p.PersonId
		OUTER APPLY
		(
				SELECT TOP 1
					[Title]
					,[GivenName]
					,[OtherNames]
					,[SurName]
					,[PersonId]
				FROM [naati_db]..[tblPersonName] pn
				LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
				WHERE pn.[PersonId] = p.[PersonId]
				ORDER BY pn.[EffectiveDate] DESC
		) pn
		LEFT JOIN (SELECT [EntityId], MAX([AddressId]) AS 'LastAddressId' FROM [naati_db]..[tblAddress] WHERE [PrimaryContact] = 1 GROUP BY [EntityId]) ppAd ON ppAd.[EntityId] = p.[EntityId]
		LEFT JOIN [naati_db]..[tblEntity] e ON e.[EntityId] = p.[EntityId]
		LEFT JOIN [naati_db]..[tblAddress] a ON ppAd.[LastAddressId] = a.[AddressId]
		LEFT JOIN [naati_db]..[tblPostcode] pc ON pc.[PostcodeId] = a.[PostcodeId]
		LEFT JOIN [naati_db]..[tblSuburb] su ON su.[SuburbId] = pc.[SuburbId]
		LEFT JOIN [naati_db]..[tluState] sta ON sta.[StateId] = su.[StateId]
		OUTER APPLY [dbo].[GetPhoneNumber](p.[EntityId], 1) PrimaryPhone
		INNER JOIN [naati_db]..[tblCredentialApplicationType] capt ON cap.[CredentialApplicationTypeId] = capt.[CredentialApplicationTypeId]
		INNER JOIN [naati_db]..[tblCredentialType] ct ON ct.[CredentialTypeId] = cr.[CredentialTypeId]
		INNER JOIN [naati_db]..[tblSkill] s ON cr.[SkillId] = s.[SkillId]
		INNER JOIN [naati_db]..[tblLanguage] l1 ON s.[Language1Id] = l1.[LanguageId]
		LEFT JOIN [naati_db]..[tblLanguageGroup] l1g ON l1.[LanguageGroupId] = l1g.[LanguageGroupId]
		INNER JOIN [naati_db]..[tblLanguage] l2 ON s.[Language2Id] = l2.[LanguageId]
		LEFT JOIN [naati_db]..[tblLanguageGroup] l2g ON l2.[LanguageGroupId] = l2g.[LanguageGroupId]
		INNER JOIN [naati_db]..[tblDirectionType] d ON d.[DirectionTypeId] = s.[DirectionTypeId]
		INNER JOIN [naati_db]..[tblCredentialRequestStatusType] crst ON crst.[CredentialRequestStatusTypeId] = cr.[CredentialRequestStatusTypeId]
		LEFT JOIN [naati_db]..[tblCredentialCredentialRequest] ccr on ccr.[CredentialRequestId] = cr.[CredentialRequestId]
		LEFT JOIN [naati_db]..[tblCredentialRequestCredentialRequest] crcr1 on crcr1.[OriginalCredentialRequestId] = cr.[CredentialRequestId]
		LEFT JOIN [naati_db]..[tblCredentialRequestAssociationType] crat1 on crat1.[CredentialRequestAssociationTypeId] = crcr1.[AssociationTypeId]
		LEFT JOIN [naati_db]..[tblCredentialRequestCredentialRequest] crcr2 on crcr2.[AssociatedCredentialRequestId] = cr.[CredentialRequestId]
		LEFT JOIN [naati_db]..[tblCredentialRequestAssociationType] crat2 on crat2.[CredentialRequestAssociationTypeId] = crcr2.[AssociationTypeId]
	) [Source]
	FULL OUTER JOIN [CredentialRequests] ON [Source].[CredentialRequestId] = [CredentialRequests].[CredentialRequestId]
	WHERE (([Source].[PersonId] IS NOT NULL AND [CredentialRequests].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [CredentialRequests].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [CredentialRequests].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [CredentialRequests].[PersonId]))
	OR (([Source].[NAATINumber] IS NOT NULL AND [CredentialRequests].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [CredentialRequests].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [CredentialRequests].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [CredentialRequests].[NAATINumber]))
	OR (([Source].[PractitionerNumber] IS NOT NULL AND [CredentialRequests].[PractitionerNumber] IS NULL) OR ([Source].[PractitionerNumber] IS NULL AND [CredentialRequests].[PractitionerNumber] IS NOT NULL) OR (([Source].[PractitionerNumber] IS NOT NULL AND [CredentialRequests].[PractitionerNumber] IS NOT NULL) AND [Source].[PractitionerNumber] != [CredentialRequests].[PractitionerNumber]))
	OR (([Source].[Title] IS NOT NULL AND [CredentialRequests].[Title] IS NULL) OR ([Source].[Title] IS NULL AND [CredentialRequests].[Title] IS NOT NULL) OR (([Source].[Title] IS NOT NULL AND [CredentialRequests].[Title] IS NOT NULL) AND [Source].[Title] != [CredentialRequests].[Title]))
	OR (([Source].[OtherNames] IS NOT NULL AND [CredentialRequests].[OtherNames] IS NULL) OR ([Source].[OtherNames] IS NULL AND [CredentialRequests].[OtherNames] IS NOT NULL) OR (([Source].[OtherNames] IS NOT NULL AND [CredentialRequests].[OtherNames] IS NOT NULL) AND [Source].[OtherNames] != [CredentialRequests].[OtherNames]))
	OR (([Source].[FamilyName] IS NOT NULL AND [CredentialRequests].[FamilyName] IS NULL) OR ([Source].[FamilyName] IS NULL AND [CredentialRequests].[FamilyName] IS NOT NULL) OR (([Source].[FamilyName] IS NOT NULL AND [CredentialRequests].[FamilyName] IS NOT NULL) AND [Source].[FamilyName] != [CredentialRequests].[FamilyName]))
	OR (([Source].[PrimaryAddress] IS NOT NULL AND [CredentialRequests].[PrimaryAddress] IS NULL) OR ([Source].[PrimaryAddress] IS NULL AND [CredentialRequests].[PrimaryAddress] IS NOT NULL) OR (([Source].[PrimaryAddress] IS NOT NULL AND [CredentialRequests].[PrimaryAddress] IS NOT NULL) AND [Source].[PrimaryAddress] != [CredentialRequests].[PrimaryAddress]))
	OR (([Source].[State] IS NOT NULL AND [CredentialRequests].[State] IS NULL) OR ([Source].[State] IS NULL AND [CredentialRequests].[State] IS NOT NULL) OR (([Source].[State] IS NOT NULL AND [CredentialRequests].[State] IS NOT NULL) AND [Source].[State] != [CredentialRequests].[State]))
	OR (([Source].[Postcode] IS NOT NULL AND [CredentialRequests].[Postcode] IS NULL) OR ([Source].[Postcode] IS NULL AND [CredentialRequests].[Postcode] IS NOT NULL) OR (([Source].[Postcode] IS NOT NULL AND [CredentialRequests].[Postcode] IS NOT NULL) AND [Source].[Postcode] != [CredentialRequests].[Postcode]))
	OR (([Source].[Country] IS NOT NULL AND [CredentialRequests].[Country] IS NULL) OR ([Source].[Country] IS NULL AND [CredentialRequests].[Country] IS NOT NULL) OR (([Source].[Country] IS NOT NULL AND [CredentialRequests].[Country] IS NOT NULL) AND [Source].[Country] != [CredentialRequests].[Country]))
	OR (([Source].[PrimaryPhone] IS NOT NULL AND [CredentialRequests].[PrimaryPhone] IS NULL) OR ([Source].[PrimaryPhone] IS NULL AND [CredentialRequests].[PrimaryPhone] IS NOT NULL) OR (([Source].[PrimaryPhone] IS NOT NULL AND [CredentialRequests].[PrimaryPhone] IS NOT NULL) AND [Source].[PrimaryPhone] != [CredentialRequests].[PrimaryPhone]))
	OR (([Source].[PrimaryEmail] IS NOT NULL AND [CredentialRequests].[PrimaryEmail] IS NULL) OR ([Source].[PrimaryEmail] IS NULL AND [CredentialRequests].[PrimaryEmail] IS NOT NULL) OR (([Source].[PrimaryEmail] IS NOT NULL AND [CredentialRequests].[PrimaryEmail] IS NOT NULL) AND [Source].[PrimaryEmail] != [CredentialRequests].[PrimaryEmail]))
	OR (([Source].[ApplicationId] IS NOT NULL AND [CredentialRequests].[ApplicationId] IS NULL) OR ([Source].[ApplicationId] IS NULL AND [CredentialRequests].[ApplicationId] IS NOT NULL) OR (([Source].[ApplicationId] IS NOT NULL AND [CredentialRequests].[ApplicationId] IS NOT NULL) AND [Source].[ApplicationId] != [CredentialRequests].[ApplicationId]))
	OR (([Source].[ApplicationType] IS NOT NULL AND [CredentialRequests].[ApplicationType] IS NULL) OR ([Source].[ApplicationType] IS NULL AND [CredentialRequests].[ApplicationType] IS NOT NULL) OR (([Source].[ApplicationType] IS NOT NULL AND [CredentialRequests].[ApplicationType] IS NOT NULL) AND [Source].[ApplicationType] != [CredentialRequests].[ApplicationType]))
	OR (([Source].[ApplicationReference] IS NOT NULL AND [CredentialRequests].[ApplicationReference] IS NULL) OR ([Source].[ApplicationReference] IS NULL AND [CredentialRequests].[ApplicationReference] IS NOT NULL) OR (([Source].[ApplicationReference] IS NOT NULL AND [CredentialRequests].[ApplicationReference] IS NOT NULL) AND [Source].[ApplicationReference] != [CredentialRequests].[ApplicationReference]))
	OR (([Source].[ApplicationOwner] IS NOT NULL AND [CredentialRequests].[ApplicationOwner] IS NULL) OR ([Source].[ApplicationOwner] IS NULL AND [CredentialRequests].[ApplicationOwner] IS NOT NULL) OR (([Source].[ApplicationOwner] IS NOT NULL AND [CredentialRequests].[ApplicationOwner] IS NOT NULL) AND [Source].[ApplicationOwner] != [CredentialRequests].[ApplicationOwner]))
	OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [CredentialRequests].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [CredentialRequests].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [CredentialRequests].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [CredentialRequests].[CredentialTypeInternalName]))
	OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [CredentialRequests].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [CredentialRequests].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [CredentialRequests].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [CredentialRequests].[CredentialTypeExternalName]))
	OR (([Source].[Certification] IS NOT NULL AND [CredentialRequests].[Certification] IS NULL) OR ([Source].[Certification] IS NULL AND [CredentialRequests].[Certification] IS NOT NULL) OR (([Source].[Certification] IS NOT NULL AND [CredentialRequests].[Certification] IS NOT NULL) AND [Source].[Certification] != [CredentialRequests].[Certification]))
	OR (([Source].[Language1] IS NOT NULL AND [CredentialRequests].[Language1] IS NULL) OR ([Source].[Language1] IS NULL AND [CredentialRequests].[Language1] IS NOT NULL) OR (([Source].[Language1] IS NOT NULL AND [CredentialRequests].[Language1] IS NOT NULL) AND [Source].[Language1] != [CredentialRequests].[Language1]))
	OR (([Source].[Language1Code] IS NOT NULL AND [CredentialRequests].[Language1Code] IS NULL) OR ([Source].[Language1Code] IS NULL AND [CredentialRequests].[Language1Code] IS NOT NULL) OR (([Source].[Language1Code] IS NOT NULL AND [CredentialRequests].[Language1Code] IS NOT NULL) AND [Source].[Language1Code] != [CredentialRequests].[Language1Code]))
	OR (([Source].[Language1Group] IS NOT NULL AND [CredentialRequests].[Language1Group] IS NULL) OR ([Source].[Language1Group] IS NULL AND [CredentialRequests].[Language1Group] IS NOT NULL) OR (([Source].[Language1Group] IS NOT NULL AND [CredentialRequests].[Language1Group] IS NOT NULL) AND [Source].[Language1Group] != [CredentialRequests].[Language1Group]))
	OR (([Source].[Language2] IS NOT NULL AND [CredentialRequests].[Language2] IS NULL) OR ([Source].[Language2] IS NULL AND [CredentialRequests].[Language2] IS NOT NULL) OR (([Source].[Language2] IS NOT NULL AND [CredentialRequests].[Language2] IS NOT NULL) AND [Source].[Language2] != [CredentialRequests].[Language2]))
	OR (([Source].[Language2Code] IS NOT NULL AND [CredentialRequests].[Language2Code] IS NULL) OR ([Source].[Language2Code] IS NULL AND [CredentialRequests].[Language2Code] IS NOT NULL) OR (([Source].[Language2Code] IS NOT NULL AND [CredentialRequests].[Language2Code] IS NOT NULL) AND [Source].[Language2Code] != [CredentialRequests].[Language2Code]))
	OR (([Source].[Language2Group] IS NOT NULL AND [CredentialRequests].[Language2Group] IS NULL) OR ([Source].[Language2Group] IS NULL AND [CredentialRequests].[Language2Group] IS NOT NULL) OR (([Source].[Language2Group] IS NOT NULL AND [CredentialRequests].[Language2Group] IS NOT NULL) AND [Source].[Language2Group] != [CredentialRequests].[Language2Group]))
	OR (([Source].[DirectionDisplayName] IS NOT NULL AND [CredentialRequests].[DirectionDisplayName] IS NULL) OR ([Source].[DirectionDisplayName] IS NULL AND [CredentialRequests].[DirectionDisplayName] IS NOT NULL) OR (([Source].[DirectionDisplayName] IS NOT NULL AND [CredentialRequests].[DirectionDisplayName] IS NOT NULL) AND [Source].[DirectionDisplayName] != [CredentialRequests].[DirectionDisplayName]))
	OR (([Source].[StatusModifiedUser] IS NOT NULL AND [CredentialRequests].[StatusModifiedUser] IS NULL) OR ([Source].[StatusModifiedUser] IS NULL AND [CredentialRequests].[StatusModifiedUser] IS NOT NULL) OR (([Source].[StatusModifiedUser] IS NOT NULL AND [CredentialRequests].[StatusModifiedUser] IS NOT NULL) AND [Source].[StatusModifiedUser] != [CredentialRequests].[StatusModifiedUser]))
	OR (([Source].[StatusDateModified] IS NOT NULL AND [CredentialRequests].[StatusDateModified] IS NULL) OR ([Source].[StatusDateModified] IS NULL AND [CredentialRequests].[StatusDateModified] IS NOT NULL) OR (([Source].[StatusDateModified] IS NOT NULL AND [CredentialRequests].[StatusDateModified] IS NOT NULL) AND [Source].[StatusDateModified] != [CredentialRequests].[StatusDateModified]))
	OR (([Source].[Status] IS NOT NULL AND [CredentialRequests].[Status] IS NULL) OR ([Source].[Status] IS NULL AND [CredentialRequests].[Status] IS NOT NULL) OR (([Source].[Status] IS NOT NULL AND [CredentialRequests].[Status] IS NOT NULL) AND [Source].[Status] != [CredentialRequests].[Status]))
	OR (([Source].[CredentialId] IS NOT NULL AND [CredentialRequests].[CredentialId] IS NULL) OR ([Source].[CredentialId] IS NULL AND [CredentialRequests].[CredentialId] IS NOT NULL) OR (([Source].[CredentialId] IS NOT NULL AND [CredentialRequests].[CredentialId] IS NOT NULL) AND [Source].[CredentialId] != [CredentialRequests].[CredentialId]))
	OR (([Source].[LinkedCredentialRequestId] IS NOT NULL AND [CredentialRequests].[LinkedCredentialRequestId] IS NULL) OR ([Source].[LinkedCredentialRequestId] IS NULL AND [CredentialRequests].[LinkedCredentialRequestId] IS NOT NULL) OR (([Source].[LinkedCredentialRequestId] IS NOT NULL AND [CredentialRequests].[LinkedCredentialRequestId] IS NOT NULL) AND [Source].[LinkedCredentialRequestId] != [CredentialRequests].[LinkedCredentialRequestId]))
	OR (([Source].[LinkedCredentialRequestReason] IS NOT NULL AND [CredentialRequests].[LinkedCredentialRequestReason] IS NULL) OR ([Source].[LinkedCredentialRequestReason] IS NULL AND [CredentialRequests].[LinkedCredentialRequestReason] IS NOT NULL) OR (([Source].[LinkedCredentialRequestReason] IS NOT NULL AND [CredentialRequests].[LinkedCredentialRequestReason] IS NOT NULL) AND [Source].[LinkedCredentialRequestReason] != [CredentialRequests].[LinkedCredentialRequestReason]))
		
	--select * from @CredentialRequestsHistory
		
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE CredentialRequestsHistory AS Target USING(select * from @CredentialRequestsHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.CredentialRequestId = Source.CredentialRequestId AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE CredentialRequestsHistory AS Target USING(select * from @CredentialRequestsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.CredentialRequestId = Source.CredentialRequestId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE CredentialRequestsHistory AS Target USING(select * from @CredentialRequestsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.CredentialRequestId = Source.CredentialRequestId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		    Target.CredentialRequestId = Source.CredentialRequestId
		  ,Target.PersonId = Source.PersonId
		  ,Target.Title = Source.Title      
		  ,Target.GivenName = Source.GivenName
		  ,Target.OtherNames = Source.OtherNames      
		  ,Target.FamilyName = Source.FamilyName
		  ,Target.PrimaryAddress = Source.PrimaryAddress
		  ,Target.Country = Source.Country

		  ,Target.PrimaryEmail = Source.PrimaryEmail
		  ,Target.PrimaryPhone = Source.PrimaryPhone
		  ,Target.ApplicationId = Source.ApplicationId
		  ,Target.ApplicationType = Source.ApplicationType
		  ,Target.ApplicationReference = Source.ApplicationReference
		  ,Target.ApplicationOwner = Source.ApplicationOwner
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName
		  ,Target.Certification = Source.Certification
		  ,Target.Language1 = Source.Language1
		  ,Target.Language2 = Source.Language2
		  ,Target.DirectionDisplayName = Source.DirectionDisplayName
		  ,Target.ModifiedDate = @Date
		  ,Target.StatusDateModified = Source.StatusDateModified
		  ,Target.StatusModifiedUser = Source.StatusModifiedUser

		  ,Target.Status = Source.Status
		  ,Target.NAATINumber = Source.NAATINumber
		  ,Target.PractitionerNumber = Source.PractitionerNumber
		  ,Target.Language1Code = Source.Language1Code
		  ,Target.Language1Group = Source.Language1Group
		  ,Target.Language2Code = Source.Language2Code
		  ,Target.Language2Group = Source.Language2Group

		  ,Target.State = Source.State
		  ,Target.Postcode = Source.Postcode
		  ,Target.CredentialId = Source.CredentialId
		  ,Target.LinkedCredentialRequestId = Source.LinkedCredentialRequestId
		  ,Target.LinkedCredentialRequestReason = Source.LinkedCredentialRequestReason
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 

		 INSERT([CredentialRequestId],[PersonId],[Title],[GivenName],[OtherNames],[FamilyName],[PrimaryAddress],[Country],[PrimaryEmail],[PrimaryPhone],[ApplicationId],[ApplicationType],[ApplicationReference],[ApplicationOwner],[CredentialTypeInternalName],[CredentialTypeExternalName],
			 [Certification],[Language1],[Language2],[DirectionDisplayName],[ModifiedDate],[StatusDateModified],[StatusModifiedUser],[Status],[NAATINumber],[PractitionerNumber],[Language1Code],[Language1Group],[Language2Code],[Language2Group],[State],[Postcode],[CredentialId],
			 [LinkedCredentialRequestId],[LinkedCredentialRequestReason],[RowStatus])
	  
	     VALUES (Source.[CredentialRequestId],Source.[PersonId],Source.[Title],Source.[GivenName],Source.[OtherNames],Source.[FamilyName],Source.[PrimaryAddress],Source.[Country],Source.[PrimaryEmail],Source.[PrimaryPhone],Source.[ApplicationId],Source.[ApplicationType],Source.[ApplicationReference],
			  Source.[ApplicationOwner],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],Source.[Certification],Source.[Language1],Source.[Language2],Source.[DirectionDisplayName],@Date ,Source.[StatusDateModified],Source.[StatusModifiedUser],Source.[Status],
			  Source.[NAATINumber],Source.[PractitionerNumber],Source.[Language1Code],Source.[Language1Group],Source.[Language2Code],Source.[Language2Group],Source.[State],Source.[Postcode],Source.[CredentialId],
			  Source.[LinkedCredentialRequestId],Source.[LinkedCredentialRequestReason], 'Latest');	 
			  		  	
		COMMIT TRANSACTION;
		
END
