ALTER PROCEDURE [dbo].[ReportingSnapshot_Credentials]
	@Date DateTime
AS
BEGIN
DECLARE @CredentialsHistory as table([CredentialId] [int] NOT NULL,[PersonId] [int] NOT NULL,[Title] [nvarchar](50) NULL,[GivenName] [nvarchar](100) NULL,[OtherNames] [nvarchar](100) NULL,[FamilyName] [nvarchar](100) NULL,[PrimaryAddress] [nvarchar](500) NULL,[Country] [nvarchar](50) NULL,
									 [PrimaryPhone] [nvarchar](60) NULL,[PrimaryEmail] [nvarchar](200) NULL,[CredentialTypeInternalName] [nvarchar](50) NOT NULL,[DirectionDisplayName] [nvarchar](105) NOT NULL,[StartDate] [datetime] NOT NULL,[ExpiryDate] [datetime] NOT NULL,
									 [ModifiedDate] [datetime] NOT NULL,[TerminationDate] [datetime] NULL,[Status] [nvarchar](50) NOT NULL,[ShowInOnlineDirectory] [bit] NOT NULL,[NAATINumber] [int] NULL,[PractitionerNumber] [varchar](50) NULL,[CredentialTypeExternalName] [nvarchar](50) NULL,
									 [Certification] [bit] NOT NULL,[Language1] [nvarchar](50) NULL,[Language1Code] [nvarchar](10) NULL,[Language1Group] [nvarchar](50) NULL,[Language2] [nvarchar](50) NULL,[Language2Code] [nvarchar](10) NULL,[Language2Group] [nvarchar](50) NULL,[State] [varchar](3) NULL,
									 [Postcode] [varchar](4) NULL,[RowStatus] nvarchar(50))   
	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	
	Insert into @CredentialsHistory
	SELECT
		 CASE WHEN [Source].[CredentialId] IS NULL THEN [Credentials].[CredentialId] ELSE [Source].[CredentialId] END AS [CredentialId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [Credentials].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[Title] IS NULL THEN [Credentials].[Title] ELSE [Source].[Title] END AS [Title]
		,CASE WHEN [Source].[GivenName] IS NULL THEN [Credentials].[GivenName] ELSE [Source].[GivenName] END AS [GivenName]
		,CASE WHEN [Source].[OtherNames] IS NULL THEN [Credentials].[OtherNames] ELSE [Source].[OtherNames] END AS [OtherNames]
		,CASE WHEN [Source].[FamilyName] IS NULL THEN [Credentials].[FamilyName] ELSE [Source].[FamilyName] END AS [FamilyName]
		,CASE WHEN [Source].[PrimaryAddress] IS NULL THEN [Credentials].[PrimaryAddress] ELSE [Source].[PrimaryAddress] END AS [PrimaryAddress]
		,CASE WHEN [Source].[Country] IS NULL THEN [Credentials].[Country] ELSE [Source].[Country] END AS [Country]
		,CASE WHEN [Source].[PrimaryPhone] IS NULL THEN [Credentials].[PrimaryPhone] ELSE [Source].[PrimaryPhone] END AS [PrimaryPhone]
		,CASE WHEN [Source].[PrimaryEmail] IS NULL THEN [Credentials].[PrimaryEmail] ELSE [Source].[PrimaryEmail] END AS [PrimaryEmail]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [Credentials].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[DirectionDisplayName] IS NULL THEN [Credentials].[DirectionDisplayName] ELSE [Source].[DirectionDisplayName] END AS [DirectionDisplayName]
		,CASE WHEN [Source].[StartDate] IS NULL THEN [Credentials].[StartDate] ELSE [Source].[StartDate] END AS [StartDate]
		,CASE WHEN [Source].[ExpiryDate] IS NULL THEN [Credentials].[ExpiryDate] ELSE [Source].[ExpiryDate] END AS [ExpiryDate]
		
		,@Date AS [ModifiedDate]
		
		,CASE WHEN [Source].[TerminationDate] IS NULL THEN [Credentials].[TerminationDate] ELSE [Source].[TerminationDate] END AS [TerminationDate]
		,CASE WHEN [Source].[Status] IS NULL THEN [Credentials].[Status] ELSE [Source].[Status] END AS [Status]
		,CASE WHEN [Source].[ShowInOnlineDirectory] IS NULL THEN [Credentials].[ShowInOnlineDirectory] ELSE [Source].[ShowInOnlineDirectory] END AS [ShowInOnlineDirectory]
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [Credentials].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[PractitionerNumber] IS NULL THEN [Credentials].[PractitionerNumber] ELSE [Source].[PractitionerNumber] END AS [PractitionerNumber]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [Credentials].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[Certification] IS NULL THEN [Credentials].[Certification] ELSE [Source].[Certification] END AS [Certification]
		,CASE WHEN [Source].[Language1] IS NULL THEN [Credentials].[Language1] ELSE [Source].[Language1] END AS [Language1]
		,CASE WHEN [Source].[Language1Code] IS NULL THEN [Credentials].[Language1Code] ELSE [Source].[Language1Code] END AS [Language1Code]
		,CASE WHEN [Source].[Language1Group] IS NULL THEN [Credentials].[Language1Group] ELSE [Source].[Language1Group] END AS [Language1Group]
		,CASE WHEN [Source].[Language2] IS NULL THEN [Credentials].[Language2] ELSE [Source].[Language2] END AS [Language2]
		,CASE WHEN [Source].[Language2Code] IS NULL THEN [Credentials].[Language2Code] ELSE [Source].[Language2Code] END AS [Language2Code]
		,CASE WHEN [Source].[Language2Group] IS NULL THEN [Credentials].[Language2Group] ELSE [Source].[Language2Group] END AS [Language2Group]
		,CASE WHEN [Source].[State] IS NULL THEN [Credentials].[State] ELSE [Source].[State] END AS [State]
		,CASE WHEN [Source].[Postcode] IS NULL THEN [Credentials].[Postcode] ELSE [Source].[Postcode] END AS [Postcode]

		,CASE WHEN [Source].[CredentialId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
		
	FROM
	(
		SELECT
			t1.[CredentialId]
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
			,ct.[InternalName] AS 'CredentialTypeInternalName'
			,ct.[ExternalName] AS 'CredentialTypeExternalName'
			,l1.[Name] AS 'Language1'
			,l1.[Code] AS 'Language1Code'
			,l1g.[Name] AS 'Language1Group'
			,l2.[Name] AS 'Language2'
			,l2.[Code] AS 'Language2Code'
			,l2g.[Name] AS 'Language2Group'
			,REPLACE(REPLACE(dt.[DisplayName], '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) COLLATE Latin1_General_CI_AS AS 'DirectionDisplayName'
			,t1.[StartDate]
			,CASE WHEN ct.[Certification] = 1 THEN cp.[EndDate] ELSE t1.[ExpiryDate] END 'ExpiryDate'
			,t1.[TerminationDate]
			,crst.[DisplayName] AS 'Status'
			,t1.[ShowInOnlineDirectory]
			,ct.[Certification]
						
		FROM 
		
		(SELECT 
			ccr.CredentialCredentialRequestId, ccr.CredentialId, ccr.CredentialRequestId, c.StartDate, c.ExpiryDate, c.TerminationDate, c.ShowInOnlineDirectory, c.CertificationPeriodId,  
			cr.CredentialApplicationId, cr.CredentialTypeId, cr.SkillId, cr.CredentialRequestStatusTypeId, cr.StatusChangeDate, cr.StatusChangeUserId, cr.CredentialRequestPathTypeId, cr.Supplementary		
		FROM  [naati_db]..[tblCredentialCredentialRequest] ccr
		INNER JOIN [naati_db]..[tblCredential] c ON c.CredentialId = ccr.CredentialId
		INNER JOIN [naati_db]..[tblCredentialRequest] cr ON cr.CredentialRequestId = ccr.CredentialRequestId) AS t1

		INNER JOIN (SELECT ccr.CredentialId AS CredentialId, MAX(ccr.CredentialRequestId) AS CredentialRequestId FROM [naati_db]..[tblCredentialCredentialRequest] AS ccr GROUP BY ccr.CredentialId) AS t2 ON t2.CredentialRequestId = t1.CredentialRequestId
		
		LEFT JOIN [naati_db]..[tblCredentialApplication] ca ON ca.CredentialApplicationId = t1.CredentialApplicationId
		INNER JOIN [naati_db]..[tblPerson] p ON ca.PersonId = p.PersonId
		INNER JOIN [naati_db]..[tblEntity] e ON e.EntityId = p.EntityId
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
		LEFT JOIN [naati_db]..[tblAddress] a ON ppAd.[LastAddressId] = a.[AddressId]
		LEFT JOIN [naati_db]..[tblPostcode] pc ON pc.[PostcodeId] = a.[PostcodeId]
		LEFT JOIN [naati_db]..[tblSuburb] su ON su.[SuburbId] = pc.[SuburbId]
		LEFT JOIN [naati_db]..[tluState] sta ON sta.[StateId] = su.[StateId]
		OUTER APPLY [dbo].[GetPhoneNumber](p.[EntityId], 1) PrimaryPhone
		LEFT JOIN [naati_db]..[tblCredentialType] ct ON t1.CredentialTypeId = ct.CredentialTypeId
		LEFT JOIN [naati_db]..[tblCertificationPeriod] cp ON cp.CertificationPeriodId = t1.CertificationPeriodId
		LEFT JOIN [naati_db]..[tblSkill] s ON t1.SkillId = s.SkillId
		INNER JOIN [naati_db]..[tblLanguage] l1 ON s.[Language1Id] = l1.[LanguageId]
		LEFT JOIN [naati_db]..[tblLanguageGroup] l1g ON l1.[LanguageGroupId] = l1g.[LanguageGroupId]
		INNER JOIN [naati_db]..[tblLanguage] l2 ON s.[Language2Id] = l2.[LanguageId]
		LEFT JOIN [naati_db]..[tblLanguageGroup] l2g ON l2.[LanguageGroupId] = l2g.[LanguageGroupId]
		LEFT JOIN [naati_db]..[tblDirectionType] dt ON s.DirectionTypeId = dt.DirectionTypeId
		LEFT JOIN [naati_db]..[tblCredentialRequestStatusType] crst ON crst.CredentialRequestStatusTypeId = t1.CredentialRequestStatusTypeId

	) [Source]
	FULL OUTER JOIN [Credentials] ON [Source].[CredentialId] = [Credentials].[CredentialId]
	WHERE (([Source].[PersonId] IS NOT NULL AND [Credentials].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [Credentials].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [Credentials].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [Credentials].[PersonId]))
	OR (([Source].[NAATINumber] IS NOT NULL AND [Credentials].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [Credentials].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [Credentials].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [Credentials].[NAATINumber]))
	OR (([Source].[PractitionerNumber] IS NOT NULL AND [Credentials].[PractitionerNumber] IS NULL) OR ([Source].[PractitionerNumber] IS NULL AND [Credentials].[PractitionerNumber] IS NOT NULL) OR (([Source].[PractitionerNumber] IS NOT NULL AND [Credentials].[PractitionerNumber] IS NOT NULL) AND [Source].[PractitionerNumber] != [Credentials].[PractitionerNumber]))
	OR (([Source].[Title] IS NOT NULL AND [Credentials].[Title] IS NULL) OR ([Source].[Title] IS NULL AND [Credentials].[Title] IS NOT NULL) OR (([Source].[Title] IS NOT NULL AND [Credentials].[Title] IS NOT NULL) AND [Source].[Title] != [Credentials].[Title]))
	OR (([Source].[GivenName] IS NOT NULL AND [Credentials].[GivenName] IS NULL) OR ([Source].[GivenName] IS NULL AND [Credentials].[GivenName] IS NOT NULL) OR (([Source].[GivenName] IS NOT NULL AND [Credentials].[GivenName] IS NOT NULL) AND [Source].[GivenName] != [Credentials].[GivenName]))
	OR (([Source].[OtherNames] IS NOT NULL AND [Credentials].[OtherNames] IS NULL) OR ([Source].[OtherNames] IS NULL AND [Credentials].[OtherNames] IS NOT NULL) OR (([Source].[OtherNames] IS NOT NULL AND [Credentials].[OtherNames] IS NOT NULL) AND [Source].[OtherNames] != [Credentials].[OtherNames]))
	OR (([Source].[FamilyName] IS NOT NULL AND [Credentials].[FamilyName] IS NULL) OR ([Source].[FamilyName] IS NULL AND [Credentials].[FamilyName] IS NOT NULL) OR (([Source].[FamilyName] IS NOT NULL AND [Credentials].[FamilyName] IS NOT NULL) AND [Source].[FamilyName] != [Credentials].[FamilyName]))
	OR (([Source].[PrimaryAddress] IS NOT NULL AND [Credentials].[PrimaryAddress] IS NULL) OR ([Source].[PrimaryAddress] IS NULL AND [Credentials].[PrimaryAddress] IS NOT NULL) OR (([Source].[PrimaryAddress] IS NOT NULL AND [Credentials].[PrimaryAddress] IS NOT NULL) AND [Source].[PrimaryAddress] != [Credentials].[PrimaryAddress]))
	OR (([Source].[State] IS NOT NULL AND [Credentials].[State] IS NULL) OR ([Source].[State] IS NULL AND [Credentials].[State] IS NOT NULL) OR (([Source].[State] IS NOT NULL AND [Credentials].[State] IS NOT NULL) AND [Source].[State] != [Credentials].[State]))
	OR (([Source].[Postcode] IS NOT NULL AND [Credentials].[Postcode] IS NULL) OR ([Source].[Postcode] IS NULL AND [Credentials].[Postcode] IS NOT NULL) OR (([Source].[Postcode] IS NOT NULL AND [Credentials].[Postcode] IS NOT NULL) AND [Source].[Postcode] != [Credentials].[Postcode]))
	OR (([Source].[Country] IS NOT NULL AND [Credentials].[Country] IS NULL) OR ([Source].[Country] IS NULL AND [Credentials].[Country] IS NOT NULL) OR (([Source].[Country] IS NOT NULL AND [Credentials].[Country] IS NOT NULL) AND [Source].[Country] != [Credentials].[Country]))
	OR (([Source].[PrimaryPhone] IS NOT NULL AND [Credentials].[PrimaryPhone] IS NULL) OR ([Source].[PrimaryPhone] IS NULL AND [Credentials].[PrimaryPhone] IS NOT NULL) OR (([Source].[PrimaryPhone] IS NOT NULL AND [Credentials].[PrimaryPhone] IS NOT NULL) AND [Source].[PrimaryPhone] != [Credentials].[PrimaryPhone]))
	OR (([Source].[PrimaryEmail] IS NOT NULL AND [Credentials].[PrimaryEmail] IS NULL) OR ([Source].[PrimaryEmail] IS NULL AND [Credentials].[PrimaryEmail] IS NOT NULL) OR (([Source].[PrimaryEmail] IS NOT NULL AND [Credentials].[PrimaryEmail] IS NOT NULL) AND [Source].[PrimaryEmail] != [Credentials].[PrimaryEmail]))
	OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [Credentials].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [Credentials].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [Credentials].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [Credentials].[CredentialTypeInternalName]))
	OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [Credentials].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [Credentials].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [Credentials].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [Credentials].[CredentialTypeExternalName]))
	OR (([Source].[Language1] IS NOT NULL AND [Credentials].[Language1] IS NULL) OR ([Source].[Language1] IS NULL AND [Credentials].[Language1] IS NOT NULL) OR (([Source].[Language1] IS NOT NULL AND [Credentials].[Language1] IS NOT NULL) AND [Source].[Language1] != [Credentials].[Language1]))
	OR (([Source].[Language1Code] IS NOT NULL AND [Credentials].[Language1Code] IS NULL) OR ([Source].[Language1Code] IS NULL AND [Credentials].[Language1Code] IS NOT NULL) OR (([Source].[Language1Code] IS NOT NULL AND [Credentials].[Language1Code] IS NOT NULL) AND [Source].[Language1Code] != [Credentials].[Language1Code]))
	OR (([Source].[Language1Group] IS NOT NULL AND [Credentials].[Language1Group] IS NULL) OR ([Source].[Language1Group] IS NULL AND [Credentials].[Language1Group] IS NOT NULL) OR (([Source].[Language1Group] IS NOT NULL AND [Credentials].[Language1Group] IS NOT NULL) AND [Source].[Language1Group] != [Credentials].[Language1Group]))
	OR (([Source].[Language2] IS NOT NULL AND [Credentials].[Language2] IS NULL) OR ([Source].[Language2] IS NULL AND [Credentials].[Language2] IS NOT NULL) OR (([Source].[Language2] IS NOT NULL AND [Credentials].[Language2] IS NOT NULL) AND [Source].[Language2] != [Credentials].[Language2]))
	OR (([Source].[Language2Code] IS NOT NULL AND [Credentials].[Language2Code] IS NULL) OR ([Source].[Language2Code] IS NULL AND [Credentials].[Language2Code] IS NOT NULL) OR (([Source].[Language2Code] IS NOT NULL AND [Credentials].[Language2Code] IS NOT NULL) AND [Source].[Language2Code] != [Credentials].[Language2Code]))
	OR (([Source].[Language2Group] IS NOT NULL AND [Credentials].[Language2Group] IS NULL) OR ([Source].[Language2Group] IS NULL AND [Credentials].[Language2Group] IS NOT NULL) OR (([Source].[Language2Group] IS NOT NULL AND [Credentials].[Language2Group] IS NOT NULL) AND [Source].[Language2Group] != [Credentials].[Language2Group]))
	OR (([Source].[DirectionDisplayName] IS NOT NULL AND [Credentials].[DirectionDisplayName] IS NULL) OR ([Source].[DirectionDisplayName] IS NULL AND [Credentials].[DirectionDisplayName] IS NOT NULL) OR (([Source].[DirectionDisplayName] IS NOT NULL AND [Credentials].[DirectionDisplayName] IS NOT NULL) AND [Source].[DirectionDisplayName] != [Credentials].[DirectionDisplayName]))
	OR (([Source].[StartDate] IS NOT NULL AND [Credentials].[StartDate] IS NULL) OR ([Source].[StartDate] IS NULL AND [Credentials].[StartDate] IS NOT NULL) OR (([Source].[StartDate] IS NOT NULL AND [Credentials].[StartDate] IS NOT NULL) AND [Source].[StartDate] != [Credentials].[StartDate]))
	OR (([Source].[ExpiryDate] IS NOT NULL AND [Credentials].[ExpiryDate] IS NULL) OR ([Source].[ExpiryDate] IS NULL AND [Credentials].[ExpiryDate] IS NOT NULL) OR (([Source].[ExpiryDate] IS NOT NULL AND [Credentials].[ExpiryDate] IS NOT NULL) AND [Source].[ExpiryDate] != [Credentials].[ExpiryDate]))
	OR (([Source].[TerminationDate] IS NOT NULL AND [Credentials].[TerminationDate] IS NULL) OR ([Source].[TerminationDate] IS NULL AND [Credentials].[TerminationDate] IS NOT NULL) OR (([Source].[TerminationDate] IS NOT NULL AND [Credentials].[TerminationDate] IS NOT NULL) AND [Source].[TerminationDate] != [Credentials].[TerminationDate]))
	OR (([Source].[Status] IS NOT NULL AND [Credentials].[Status] IS NULL) OR ([Source].[Status] IS NULL AND [Credentials].[Status] IS NOT NULL) OR (([Source].[Status] IS NOT NULL AND [Credentials].[Status] IS NOT NULL) AND [Source].[Status] != [Credentials].[Status]))
	OR (([Source].[ShowInOnlineDirectory] IS NOT NULL AND [Credentials].[ShowInOnlineDirectory] IS NULL) OR ([Source].[ShowInOnlineDirectory] IS NULL AND [Credentials].[ShowInOnlineDirectory] IS NOT NULL) OR (([Source].[ShowInOnlineDirectory] IS NOT NULL AND [Credentials].[ShowInOnlineDirectory] IS NOT NULL) AND [Source].[ShowInOnlineDirectory] != [Credentials].[ShowInOnlineDirectory]))
	OR (([Source].[Certification] IS NOT NULL AND [Credentials].[Certification] IS NULL) OR ([Source].[Certification] IS NULL AND [Credentials].[Certification] IS NOT NULL) OR (([Source].[Certification] IS NOT NULL AND [Credentials].[Certification] IS NOT NULL) AND [Source].[Certification] != [Credentials].[Certification]))
		
	--select * from @CredentialsHistory

	BEGIN TRANSACTION
	   --Merge operation delete
		MERGE CredentialsHistory AS Target USING(select * from @CredentialsHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[CredentialId] = Source.[CredentialId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE CredentialsHistory AS Target USING(select * from @CredentialsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[CredentialId] = Source.[CredentialId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE CredentialsHistory AS Target USING(select * from @CredentialsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[CredentialId] = Source.[CredentialId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.CredentialId = Source.CredentialId
		  ,Target.PersonId = Source.PersonId
		  ,Target.Title = Source.Title
		  ,Target.GivenName = Source.GivenName
		  ,Target.OtherNames = Source.OtherNames
		  ,Target.FamilyName = Source.FamilyName      
		  ,Target.PrimaryAddress = Source.PrimaryAddress
		  ,Target.Country = Source.Country
		  ,Target.PrimaryPhone = Source.PrimaryPhone
		  ,Target.PrimaryEmail = Source.PrimaryEmail

		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.DirectionDisplayName = Source.DirectionDisplayName
		  ,Target.StartDate = Source.StartDate
		  ,Target.ExpiryDate = Source.ExpiryDate		  
		  ,Target.TerminationDate = Source.TerminationDate
		  ,Target.Status = Source.Status
		  ,Target.ShowInOnlineDirectory = Source.ShowInOnlineDirectory
		  ,Target.NAATINumber = Source.NAATINumber
		  ,Target.PractitionerNumber = Source.PractitionerNumber
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName

		  ,Target.Certification = Source.Certification
		  ,Target.Language1 = Source.Language1
		  ,Target.Language1Code = Source.Language1Code
		  ,Target.Language1Group = Source.Language1Group
		  ,Target.Language2 = Source.Language2
		  ,Target.Language2Code = Source.Language2Code
		  ,Target.Language2Group = Source.Language2Group
		  ,Target.State = Source.State
		  ,Target.Postcode = Source.Postcode

		  ,Target.[ModifiedDate] = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([CredentialId],[PersonId],[Title],[GivenName],[OtherNames],[FamilyName],[PrimaryAddress],[Country],[PrimaryPhone],[PrimaryEmail],[CredentialTypeInternalName],[DirectionDisplayName],[StartDate],[ExpiryDate],
		     [ModifiedDate],[TerminationDate],[Status],[ShowInOnlineDirectory],[NAATINumber],[PractitionerNumber],[CredentialTypeExternalName],[Certification],[Language1],[Language1Code],[Language1Group],
			 [Language2],[Language2Code],[Language2Group],[State],[Postcode],[RowStatus])	  
	  
	      VALUES (Source.[CredentialId],Source.[PersonId],Source.[Title],Source.[GivenName],Source.[OtherNames],Source.[FamilyName],Source.[PrimaryAddress],Source.[Country],Source.[PrimaryPhone],Source.[PrimaryEmail],Source.[CredentialTypeInternalName],
			  Source.[DirectionDisplayName],Source.[StartDate],Source.[ExpiryDate],@Date,Source.[TerminationDate],Source.[Status],Source.[ShowInOnlineDirectory],Source.[NAATINumber],Source.[PractitionerNumber],Source.[CredentialTypeExternalName],
			  Source.[Certification],Source.[Language1],Source.[Language1Code],Source.[Language1Group],Source.[Language2],Source.[Language2Code],Source.[Language2Group],Source.[State],Source.[Postcode], 'Latest');
			  		  	
		COMMIT TRANSACTION;	

END
