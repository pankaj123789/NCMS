ALTER PROCEDURE [dbo].[ReportingSnapshot_Application]
	@Date DateTime
AS
BEGIN
DECLARE @ApplicationHistory as table ([ApplicationId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL DEFAULT GETDATE(),[PersonId] [int] NOT NULL,[EnteredDate] [datetime] NOT NULL,[EnteredOffice] [varchar](100) NOT NULL,[ApplicationStatus] [nvarchar](100) NULL,[StatusDateModified] [datetime] NULL,[Sponsor] [nvarchar](100) NULL,
									  [Title] [nvarchar](50) NULL,[GivenName] [nvarchar](100) NULL,[OtherNames] [nvarchar](100) NULL,[FamilyName] [nvarchar](100) NULL,[PrimaryAddress] [nvarchar](500) NULL,[Country] [nvarchar](50) NULL,[PrimaryPhone] [nvarchar](60) NULL,[PrimaryEmail] [nvarchar](200) NULL,
									  [ApplicationType] [nvarchar](50) NULL,[ApplicationReference] [nvarchar](50) NULL,[ApplicationOwner] [nvarchar](100) NULL,[StatusModifiedUser] [nvarchar](100) NULL,[NAATINumber] [int] NULL,[PractitionerNumber] [varchar](50) NULL,[EnteredUser] [nvarchar](100) NULL,
									  [SponsoredOrganisationId] [int] NULL,[SponsoredOrganisationName] [nvarchar](100) NULL,[SponsoredContactId] [int] NULL,[SponsoredContactName] [nvarchar](201) NULL,[PreferredTestLocationState] [nvarchar](3) NULL,[PreferredTestLocationCity] [nvarchar](500) NULL,[State] [varchar](3) NULL,
									  [Postcode] [varchar](4) NULL,	[RowStatus] nvarchar(50))   
									  	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @ApplicationHistory
	SELECT
		 CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[ApplicationId] ELSE [Source].[ApplicationId] END AS [ApplicationId]

		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[EnteredDate] ELSE [Source].[EnteredDate] END AS [EnteredDate]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[EnteredOffice] ELSE [Source].[EnteredOffice] END AS [EnteredOffice]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[ApplicationStatus] ELSE [Source].[ApplicationStatus] END AS [ApplicationStatus]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[StatusDateModified] ELSE [Source].[StatusDateModified] END AS [StatusDateModified]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[Sponsor] ELSE [Source].[Sponsor] END AS [Sponsor]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[Title] ELSE [Source].[Title] END AS [Title]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[GivenName] ELSE [Source].[GivenName] END AS [GivenName]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[OtherNames] ELSE [Source].[OtherNames] END AS [OtherNames]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[FamilyName] ELSE [Source].[FamilyName] END AS [FamilyName]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[PrimaryAddress] ELSE [Source].[PrimaryAddress] END AS [PrimaryAddress]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[Country] ELSE [Source].[Country] END AS [Country]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[PrimaryPhone] ELSE [Source].[PrimaryPhone] END AS [PrimaryPhone]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[PrimaryEmail] ELSE [Source].[PrimaryEmail] END AS [PrimaryEmail]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[ApplicationType] ELSE [Source].[ApplicationType] END AS [ApplicationType]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[ApplicationReference] ELSE [Source].[ApplicationReference] END AS [ApplicationReference]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[ApplicationOwner] ELSE [Source].[ApplicationOwner] END AS [ApplicationOwner]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[StatusModifiedUser] ELSE [Source].[StatusModifiedUser] END AS [StatusModifiedUser]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[PractitionerNumber] ELSE [Source].[PractitionerNumber] END AS [PractitionerNumber]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[EnteredUser] ELSE [Source].[EnteredUser] END AS [EnteredUser]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[SponsoredOrganisationId] ELSE [Source].[SponsoredOrganisationId] END AS [SponsoredOrganisationId]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[SponsoredOrganisationName] ELSE [Source].[SponsoredOrganisationName] END AS [SponsoredOrganisationName]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[SponsoredContactId] ELSE [Source].[SponsoredContactId] END AS [SponsoredContactId]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[SponsoredContactName] ELSE [Source].[SponsoredContactName] END AS [SponsoredContactName]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[PreferredTestLocationState] ELSE [Source].[PreferredTestLocationState] END AS [PreferredTestLocationState]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[PreferredTestLocationCity] ELSE [Source].[PreferredTestLocationCity] END AS [PreferredTestLocationCity]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[State] ELSE [Source].[State] END AS [State]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [Application].[Postcode] ELSE [Source].[Postcode] END AS [Postcode]

		,CASE WHEN [Source].[ApplicationId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
	FROM
	(
		SELECT
			ap.[CredentialApplicationId] AS 'ApplicationId'
			,ap.[PersonId]
			,e.[NAATINumber]
			,p.[PractitionerNumber]
			,pn.[Title] COLLATE Latin1_General_CI_AS AS 'Title'
			,pn.[GivenName] COLLATE Latin1_General_CI_AS AS 'GivenName'
			,pn.[OtherNames] COLLATE Latin1_General_CI_AS AS 'OtherNames'
			,pn.[Surname] COLLATE Latin1_General_CI_AS AS 'FamilyName'
			,pad.[StreetDetails] COLLATE Latin1_General_CI_AS AS 'PrimaryAddress'
			,sta.State COLLATE Latin1_General_CI_AS AS 'State'
			,pc.Postcode COLLATE Latin1_General_CI_AS AS 'Postcode'
			,c.[Name] COLLATE Latin1_General_CI_AS AS 'Country'
			,ph.[Number] COLLATE Latin1_General_CI_AS AS 'PrimaryPhone'
			,em.[Email] COLLATE Latin1_General_CI_AS AS 'PrimaryEmail'
			,capt.[DisplayName] AS 'ApplicationType'
			,CONCAT('APP', ap.[CredentialApplicationId]) COLLATE Latin1_General_CI_AS AS 'ApplicationReference'
			,CASE
				WHEN ap.[OwnedByApplicant] = 1 THEN 'Applicant'
				WHEN ap.[OwnedByApplicant] = 0 THEN (SELECT TOP 1 
				  [FullName]
				  FROM [naati_db]..[tblUser] ownedByUser
				  WHERE ownedByUser.[UserId] = ap.[OwnedByUserId])
			END COLLATE Latin1_General_CI_AS AS 'ApplicationOwner'
			,capst.[Name] COLLATE Latin1_General_CI_AS AS 'ApplicationStatus'
			,ap.[EnteredDate]
			,inn.[Name] COLLATE Latin1_General_CI_AS AS 'EnteredOffice'
			,us.[FullName] AS 'EnteredUser'
			,ap.[StatusChangeDate] AS 'StatusDateModified'
			,scu.[FullName] AS 'StatusModifiedUser'
			,sinn.[Name] COLLATE Latin1_General_CI_AS AS 'Sponsor'
			,si.[InstitutionId] AS 'SponsoredOrganisationId'
			,sinn.[Name] COLLATE Latin1_General_CI_AS AS 'SponsoredOrganisationName'
			,incp.[ContactPersonId] AS 'SponsoredContactId'
			,incp.[Name] AS 'SponsoredContactName'
			,CASE WHEN tls.[State] = 'ONLINE' THEN 'ONL' ELSE tls.[State] END AS 'PreferredTestLocationState'
			,tl.[Name] AS 'PreferredTestLocationCity'
			
		FROM [naati_db]..[tblCredentialApplication] ap
		LEFT JOIN [naati_db]..[tblCredentialApplicationStatusType] capst ON ap.[CredentialApplicationStatusTypeId] = capst.[CredentialApplicationStatusTypeId]
		LEFT JOIN [naati_db]..[tblCredentialApplicationType] capt ON ap.[CredentialApplicationTypeId] = capt.[CredentialApplicationTypeId]
		LEFT JOIN [naati_db]..[tblUser] us ON ap.[EnteredUserId] = us.[UserId]
		LEFT JOIN [naati_db]..[tblUser] scu ON ap.[StatusChangeUserId] = scu.[UserId]
		LEFT JOIN [naati_db]..[tblOffice] o ON o.[OfficeId] = ap.[ReceivingOfficeId]
		LEFT JOIN [naati_db]..[tblInstitution] i ON o.[InstitutionId] = i.[InstitutionId]
		LEFT JOIN [naati_db]..[vwDistinctInstitutionName] din ON din.[InstitutionId] = i.[InstitutionId]
		LEFT JOIN [naati_db]..[tblInstitutionName] inn ON inn.[InstitutionNameId] = din.[InstitutionNameId]
		LEFT JOIN [naati_db]..[tblInstitution] si ON si.[InstitutionId] = ap.[SponsorInstitutionId]
		LEFT JOIN [naati_db]..[vwDistinctInstitutionName] spin ON spin.[InstitutionId] = si.[InstitutionId]
		LEFT JOIN [naati_db]..[tblInstitutionName] sinn ON sinn.[InstitutionNameId] = spin.[InstitutionNameId]
		LEFT JOIN [naati_db]..[tblContactPerson] incp ON incp.[ContactPersonId] = ap.[SponsorInstitutionContactPersonId]
		LEFT JOIN [naati_db]..[tblTestLocation] tl ON tl.[TestLocationId] = ap.[PreferredTestLocationId]
		LEFT JOIN [naati_db]..[tblOffice] tli ON tli.[OfficeId] = tl.[OfficeId]
		LEFT JOIN [naati_db]..[tluState] tls ON tls.[StateId] = tli.[StateId]
		OUTER APPLY
		(
			SELECT TOP 1
			[Title]
			,[GivenName]
			,[OtherNames]
			,[Surname]
			,[PersonId]
			FROM [naati_db]..[tblPersonName] pn
			LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
			WHERE pn.[PersonId] = ap.[PersonId]
			ORDER BY pn.[EffectiveDate] DESC
		) pn
		LEFT JOIN [naati_db]..[tblPerson] P ON ap.[PersonId] = p.[PersonId]
		LEFT JOIN [naati_db]..[tblEntity] e ON e.[EntityId] = p.[EntityId]
		OUTER APPLY
		(
			SELECT TOP 1
			p.[PersonId]
			,ad.[StreetDetails]
			,c.[Name]
			,c.[CountryId]
			,ad.[PostcodeId]
			FROM [naati_db]..[tblAddress] ad
			LEFT JOIN [naati_db]..[tblPerson] P ON ad.[EntityId] = p.[EntityId]
			LEFT JOIN [naati_db]..[tblCountry] c ON ad.[CountryId] = c.[CountryId]
			WHERE ad.Invalid = 0 AND ad.PrimaryContact = 1 AND p.[PersonId] = ap.[PersonId]
		) pad
		LEFT JOIN [naati_db]..[tblCountry] c ON pad.[CountryId] = c.[CountryId]
		LEFT JOIN [naati_db]..[tblPostcode] pc ON pc.[PostcodeId] = pad.[PostcodeId]
		LEFT JOIN [naati_db]..[tblSuburb] s ON s.[SuburbId] = pc.[SuburbId]
		LEFT JOIN [naati_db]..[tluState] sta ON sta.[StateId] = s.[StateId]
		OUTER APPLY
		(
			SELECT TOP 1
			[Number]
			,[EntityId]
			FROM [naati_db]..[tblPhone] ph
			WHERE PrimaryContact = 1 AND Invalid = 0 AND ph.[EntityId] = e.[EntityId]
		) ph
		OUTER APPLY
		(
			SELECT TOP 1
			[Email]
			,[EntityId]
			FROM [naati_db]..[tblEmail] em
			WHERE IsPreferredEmail = 1 AND Invalid = 0 AND em.[EntityId] = e.[EntityId]
		) em
	) [Source]
	FULL OUTER JOIN [Application] ON [Source].[ApplicationId] = [Application].[ApplicationId]
	WHERE (([Source].[PersonId] IS NOT NULL AND [Application].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [Application].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [Application].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [Application].[PersonId]))
	OR (([Source].[NAATINumber] IS NOT NULL AND [Application].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [Application].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [Application].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [Application].[NAATINumber]))
	OR (([Source].[PractitionerNumber] IS NOT NULL AND [Application].[PractitionerNumber] IS NULL) OR ([Source].[PractitionerNumber] IS NULL AND [Application].[PractitionerNumber] IS NOT NULL) OR (([Source].[PractitionerNumber] IS NOT NULL AND [Application].[PractitionerNumber] IS NOT NULL) AND [Source].[PractitionerNumber] != [Application].[PractitionerNumber]))
	OR (([Source].[Title] IS NOT NULL AND [Application].[Title] IS NULL) OR ([Source].[Title] IS NULL AND [Application].[Title] IS NOT NULL) OR (([Source].[Title] IS NOT NULL AND [Application].[Title] IS NOT NULL) AND [Source].[Title] != [Application].[Title]))
	OR (([Source].[GivenName] IS NOT NULL AND [Application].[GivenName] IS NULL) OR ([Source].[GivenName] IS NULL AND [Application].[GivenName] IS NOT NULL) OR (([Source].[GivenName] IS NOT NULL AND [Application].[GivenName] IS NOT NULL) AND [Source].[GivenName] != [Application].[GivenName]))
	OR (([Source].[OtherNames] IS NOT NULL AND [Application].[OtherNames] IS NULL) OR ([Source].[OtherNames] IS NULL AND [Application].[OtherNames] IS NOT NULL) OR (([Source].[OtherNames] IS NOT NULL AND [Application].[OtherNames] IS NOT NULL) AND [Source].[OtherNames] != [Application].[OtherNames]))
	OR (([Source].[FamilyName] IS NOT NULL AND [Application].[FamilyName] IS NULL) OR ([Source].[FamilyName] IS NULL AND [Application].[FamilyName] IS NOT NULL) OR (([Source].[FamilyName] IS NOT NULL AND [Application].[FamilyName] IS NOT NULL) AND [Source].[FamilyName] != [Application].[FamilyName]))
	OR (([Source].[PrimaryAddress] IS NOT NULL AND [Application].[PrimaryAddress] IS NULL) OR ([Source].[PrimaryAddress] IS NULL AND [Application].[PrimaryAddress] IS NOT NULL) OR (([Source].[PrimaryAddress] IS NOT NULL AND [Application].[PrimaryAddress] IS NOT NULL) AND [Source].[PrimaryAddress] != [Application].[PrimaryAddress]))
	OR (([Source].[State] IS NOT NULL AND [Application].[State] IS NULL) OR ([Source].[State] IS NULL AND [Application].[State] IS NOT NULL) OR (([Source].[State] IS NOT NULL AND [Application].[State] IS NOT NULL) AND [Source].[State] != [Application].[State]))
	OR (([Source].[Postcode] IS NOT NULL AND [Application].[Postcode] IS NULL) OR ([Source].[Postcode] IS NULL AND [Application].[Postcode] IS NOT NULL) OR (([Source].[Postcode] IS NOT NULL AND [Application].[Postcode] IS NOT NULL) AND [Source].[Postcode] != [Application].[Postcode]))
	OR (([Source].[Country] IS NOT NULL AND [Application].[Country] IS NULL) OR ([Source].[Country] IS NULL AND [Application].[Country] IS NOT NULL) OR (([Source].[Country] IS NOT NULL AND [Application].[Country] IS NOT NULL) AND [Source].[Country] != [Application].[Country]))
	OR (([Source].[PrimaryPhone] IS NOT NULL AND [Application].[PrimaryPhone] IS NULL) OR ([Source].[PrimaryPhone] IS NULL AND [Application].[PrimaryPhone] IS NOT NULL) OR (([Source].[PrimaryPhone] IS NOT NULL AND [Application].[PrimaryPhone] IS NOT NULL) AND [Source].[PrimaryPhone] != [Application].[PrimaryPhone]))
	OR (([Source].[PrimaryEmail] IS NOT NULL AND [Application].[PrimaryEmail] IS NULL) OR ([Source].[PrimaryEmail] IS NULL AND [Application].[PrimaryEmail] IS NOT NULL) OR (([Source].[PrimaryEmail] IS NOT NULL AND [Application].[PrimaryEmail] IS NOT NULL) AND [Source].[PrimaryEmail] != [Application].[PrimaryEmail]))
	OR (([Source].[ApplicationStatus] IS NOT NULL AND [Application].[ApplicationStatus] IS NULL) OR ([Source].[ApplicationStatus] IS NULL AND [Application].[ApplicationStatus] IS NOT NULL) OR (([Source].[ApplicationStatus] IS NOT NULL AND [Application].[ApplicationStatus] IS NOT NULL) AND [Source].[ApplicationStatus] != [Application].[ApplicationStatus]))
	OR (([Source].[EnteredDate] IS NOT NULL AND [Application].[EnteredDate] IS NULL) OR ([Source].[EnteredDate] IS NULL AND [Application].[EnteredDate] IS NOT NULL) OR (([Source].[EnteredDate] IS NOT NULL AND [Application].[EnteredDate] IS NOT NULL) AND [Source].[EnteredDate] != [Application].[EnteredDate]))
	OR (([Source].[EnteredOffice] IS NOT NULL AND [Application].[EnteredOffice] IS NULL) OR ([Source].[EnteredOffice] IS NULL AND [Application].[EnteredOffice] IS NOT NULL) OR (([Source].[EnteredOffice] IS NOT NULL AND [Application].[EnteredOffice] IS NOT NULL) AND [Source].[EnteredOffice] != [Application].[EnteredOffice]))
	OR (([Source].[StatusDateModified] IS NOT NULL AND [Application].[StatusDateModified] IS NULL) OR ([Source].[StatusDateModified] IS NULL AND [Application].[StatusDateModified] IS NOT NULL) OR (([Source].[StatusDateModified] IS NOT NULL AND [Application].[StatusDateModified] IS NOT NULL) AND [Source].[StatusDateModified] != [Application].[StatusDateModified]))
	OR (([Source].[EnteredUser] IS NOT NULL AND [Application].[EnteredUser] IS NULL) OR ([Source].[EnteredUser] IS NULL AND [Application].[EnteredUser] IS NOT NULL) OR (([Source].[EnteredUser] IS NOT NULL AND [Application].[EnteredUser] IS NOT NULL) AND [Source].[EnteredUser] != [Application].[EnteredUser]))
	OR (([Source].[ApplicationType] IS NOT NULL AND [Application].[ApplicationType] IS NULL) OR ([Source].[ApplicationType] IS NULL AND [Application].[ApplicationType] IS NOT NULL) OR (([Source].[ApplicationType] IS NOT NULL AND [Application].[ApplicationType] IS NOT NULL) AND [Source].[ApplicationType] != [Application].[ApplicationType]))
	OR (([Source].[ApplicationReference] IS NOT NULL AND [Application].[ApplicationReference] IS NULL) OR ([Source].[ApplicationReference] IS NULL AND [Application].[ApplicationReference] IS NOT NULL) OR (([Source].[ApplicationReference] IS NOT NULL AND [Application].[ApplicationReference] IS NOT NULL) AND [Source].[ApplicationReference] != [Application].[ApplicationReference]))
	OR (([Source].[ApplicationOwner] IS NOT NULL AND [Application].[ApplicationOwner] IS NULL) OR ([Source].[ApplicationOwner] IS NULL AND [Application].[ApplicationOwner] IS NOT NULL) OR (([Source].[ApplicationOwner] IS NOT NULL AND [Application].[ApplicationOwner] IS NOT NULL) AND [Source].[ApplicationOwner] != [Application].[ApplicationOwner]))
	OR (([Source].[StatusModifiedUser] IS NOT NULL AND [Application].[StatusModifiedUser] IS NULL) OR ([Source].[StatusModifiedUser] IS NULL AND [Application].[StatusModifiedUser] IS NOT NULL) OR (([Source].[StatusModifiedUser] IS NOT NULL AND [Application].[StatusModifiedUser] IS NOT NULL) AND [Source].[StatusModifiedUser] != [Application].[StatusModifiedUser]))
	OR (([Source].[Sponsor] IS NOT NULL AND [Application].[Sponsor] IS NULL) OR ([Source].[Sponsor] IS NULL AND [Application].[Sponsor] IS NOT NULL) OR (([Source].[Sponsor] IS NOT NULL AND [Application].[Sponsor] IS NOT NULL) AND [Source].[Sponsor] != [Application].[Sponsor]))
	OR (([Source].[SponsoredOrganisationId] IS NOT NULL AND [Application].[SponsoredOrganisationId] IS NULL) OR ([Source].[SponsoredOrganisationId] IS NULL AND [Application].[SponsoredOrganisationId] IS NOT NULL) OR (([Source].[SponsoredOrganisationId] IS NOT NULL AND [Application].[SponsoredOrganisationId] IS NOT NULL) AND [Source].[SponsoredOrganisationId] != [Application].[SponsoredOrganisationId]))
	OR (([Source].[SponsoredOrganisationName] IS NOT NULL AND [Application].[SponsoredOrganisationName] IS NULL) OR ([Source].[SponsoredOrganisationName] IS NULL AND [Application].[SponsoredOrganisationName] IS NOT NULL) OR (([Source].[SponsoredOrganisationName] IS NOT NULL AND [Application].[SponsoredOrganisationName] IS NOT NULL) AND [Source].[SponsoredOrganisationName] != [Application].[SponsoredOrganisationName]))
	OR (([Source].[SponsoredContactId] IS NOT NULL AND [Application].[SponsoredContactId] IS NULL) OR ([Source].[SponsoredContactId] IS NULL AND [Application].[SponsoredContactId] IS NOT NULL) OR (([Source].[SponsoredContactId] IS NOT NULL AND [Application].[SponsoredContactId] IS NOT NULL) AND [Source].[SponsoredContactId] != [Application].[SponsoredContactId]))
	OR (([Source].[SponsoredContactName] IS NOT NULL AND [Application].[SponsoredContactName] IS NULL) OR ([Source].[SponsoredContactName] IS NULL AND [Application].[SponsoredContactName] IS NOT NULL) OR (([Source].[SponsoredContactName] IS NOT NULL AND [Application].[SponsoredContactName] IS NOT NULL) AND [Source].[SponsoredContactName] != [Application].[SponsoredContactName]))
	OR (([Source].[PreferredTestLocationState] IS NOT NULL AND [Application].[PreferredTestLocationState] IS NULL) OR ([Source].[PreferredTestLocationState] IS NULL AND [Application].[PreferredTestLocationState] IS NOT NULL) OR (([Source].[PreferredTestLocationState] IS NOT NULL AND [Application].[PreferredTestLocationState] IS NOT NULL) AND [Source].[PreferredTestLocationState] != [Application].[PreferredTestLocationState]))
	OR (([Source].[PreferredTestLocationCity] IS NOT NULL AND [Application].[PreferredTestLocationCity] IS NULL) OR ([Source].[PreferredTestLocationCity] IS NULL AND [Application].[PreferredTestLocationCity] IS NOT NULL) OR (([Source].[PreferredTestLocationCity] IS NOT NULL AND [Application].[PreferredTestLocationCity] IS NOT NULL) AND [Source].[PreferredTestLocationCity] != [Application].[PreferredTestLocationCity]))

	--select * from @ApplicationHistory

	---------------------------------------------
	BEGIN TRANSACTION 	
	   --Merge operation delete
		MERGE ApplicationHistory AS Target USING(select * from @ApplicationHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[ApplicationId] = Source.[ApplicationId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE ApplicationHistory AS Target USING(select * from @ApplicationHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[ApplicationId] = Source.[ApplicationId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE ApplicationHistory AS Target USING(select * from @ApplicationHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[ApplicationId] = Source.[ApplicationId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.ApplicationId = Source.ApplicationId
		  ,Target.PersonId = Source.PersonId
		  ,Target.EnteredDate = Source.EnteredDate      
		  ,Target.EnteredOffice = Source.EnteredOffice
		  ,Target.ApplicationStatus = Source.ApplicationStatus      
		  ,Target.StatusDateModified = Source.StatusDateModified
		  ,Target.Sponsor = Source.Sponsor
		  ,Target.Title = Source.Title

		  ,Target.GivenName = Source.GivenName
		  ,Target.OtherNames = Source.OtherNames
		  ,Target.FamilyName = Source.FamilyName
		  ,Target.PrimaryAddress = Source.PrimaryAddress
		  ,Target.Country = Source.Country
		  ,Target.PrimaryPhone = Source.PrimaryPhone
		  ,Target.PrimaryEmail = Source.PrimaryEmail

		  ,Target.ApplicationType = Source.ApplicationType
		  ,Target.ApplicationReference = Source.ApplicationReference
		  ,Target.ApplicationOwner = Source.ApplicationOwner
		  ,Target.StatusModifiedUser = Source.StatusModifiedUser
		  ,Target.NAATINumber = Source.NAATINumber
		  ,Target.PractitionerNumber = Source.PractitionerNumber

		  ,Target.EnteredUser = Source.EnteredUser
		  ,Target.SponsoredOrganisationId = Source.SponsoredOrganisationId
		  ,Target.SponsoredOrganisationName = Source.SponsoredOrganisationName
		  ,Target.SponsoredContactId = Source.SponsoredContactId
		  ,Target.SponsoredContactName = Source.SponsoredContactName

		  ,Target.PreferredTestLocationState = Source.PreferredTestLocationState
		  ,Target.PreferredTestLocationCity = Source.PreferredTestLocationCity
		  ,Target.[State] = Source.[State]
		  ,Target.Postcode = Source.Postcode
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([ApplicationId],[ModifiedDate],[PersonId],[EnteredDate],[EnteredOffice],[ApplicationStatus],[StatusDateModified],[Sponsor],[Title],[GivenName],[OtherNames],[FamilyName],
			 [PrimaryAddress],[Country],[PrimaryPhone],[PrimaryEmail],[ApplicationType],[ApplicationReference],[ApplicationOwner],[StatusModifiedUser],[NAATINumber],[PractitionerNumber],[EnteredUser],
			 [SponsoredOrganisationId],[SponsoredOrganisationName],[SponsoredContactId],[SponsoredContactName],[PreferredTestLocationState],[PreferredTestLocationCity],[State],[Postcode],[RowStatus]) 

		 VALUES (Source.[ApplicationId],@Date,Source.[PersonId],Source.[EnteredDate],Source.[EnteredOffice],Source.[ApplicationStatus],Source.[StatusDateModified],Source.[Sponsor],Source.[Title],Source.[GivenName],Source.[OtherNames],Source.[FamilyName],
			  Source.[PrimaryAddress],Source.[Country],Source.[PrimaryPhone],Source.[PrimaryEmail],Source.[ApplicationType],Source.[ApplicationReference],Source.[ApplicationOwner],Source.[StatusModifiedUser],Source.[NAATINumber],Source.[PractitionerNumber],Source.[EnteredUser],
			  Source.[SponsoredOrganisationId],Source.[SponsoredOrganisationName],Source.[SponsoredContactId],Source.[SponsoredContactName],Source.[PreferredTestLocationState],Source.[PreferredTestLocationCity],Source.[State],Source.[Postcode], 'Latest');

		COMMIT TRANSACTION;
	
END
