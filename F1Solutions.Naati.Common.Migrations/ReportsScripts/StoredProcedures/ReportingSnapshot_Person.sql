ALTER PROCEDURE [dbo].[ReportingSnapshot_Person]
	@Date DateTime
AS
BEGIN
DECLARE @PersonHistory as table([PersonId] [int] NOT NULL,[ModifiedDate] [datetime] NOT NULL,[GivenName] [varchar](100) NULL,[OtherNames] [varchar](100) NULL,[FamilyName] [varchar](100) NULL,[Title] [varchar](50) NULL,[Gender] [char](1) NOT NULL,[DateOfBirth] [datetime] NULL,
									[CountryOfBirth] [varchar](50) NULL,[PrimaryAddress] [nvarchar](500) NULL,[Country] [varchar](50) NULL,[PrimaryEmail] [nvarchar](200) NULL,[PrimaryPhone] [nvarchar](60) NULL,[Suburb] [varchar](50) NULL,[SecondaryAddress] [nvarchar](500) NULL,
									[SecondaryPhone] [nvarchar](60) NULL,[SecondaryEmail] [nvarchar](200) NULL,[NAATINumber] [int] NULL,[PractitionerNumber] [varchar](50) NULL,[DoNotSendCorrespondence] [int] NULL,[EthicalCompetency] [bit] NOT NULL,[InterculturalCompetency] [bit] NOT NULL,
									[State] [varchar](3) NULL,[Postcode] [varchar](4) NULL,[RolePlayerRating] [decimal](3, 1) NULL,[RolePlayerSenior] [bit] NULL,[KnowledgeTest] [bit] NOT NULL,[Deceased] [bit] NOT NULL, [RowStatus] nvarchar(50))   
										
	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @PersonHistory
	SELECT
		 CASE WHEN [Source].[PersonId] IS NULL THEN [Person].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		 
		 ,@Date AS [ModifiedDate]
		
		,CASE WHEN [Source].[GivenName] IS NULL THEN [Person].[GivenName] ELSE [Source].[GivenName] END AS [GivenName]		
		,CASE WHEN [Source].[OtherNames] IS NULL THEN [Person].[OtherNames] ELSE [Source].[OtherNames] END AS [OtherNames]
		,CASE WHEN [Source].[FamilyName] IS NULL THEN [Person].[FamilyName] ELSE [Source].[FamilyName] END AS [FamilyName]
		,CASE WHEN [Source].[Title] IS NULL THEN [Person].[Title] ELSE [Source].[Title] END AS [Title]
		,CASE WHEN [Source].[Gender] IS NULL THEN [Person].[Gender] ELSE [Source].[Gender] END AS [Gender]
		,CASE WHEN [Source].[DateOfBirth] IS NULL THEN [Person].[DateOfBirth] ELSE [Source].[DateOfBirth] END AS [DateOfBirth]
		,CASE WHEN [Source].[CountryOfBirth] IS NULL THEN [Person].[CountryOfBirth] ELSE [Source].[CountryOfBirth] END AS [CountryOfBirth]
		,CASE WHEN [Source].[PrimaryAddress] IS NULL THEN [Person].[PrimaryAddress] ELSE [Source].[PrimaryAddress] END AS [PrimaryAddress]
		,CASE WHEN [Source].[Country] IS NULL THEN [Person].[Country] ELSE [Source].[Country] END AS [Country]
		,CASE WHEN [Source].[PrimaryEmail] IS NULL THEN [Person].[PrimaryEmail] ELSE [Source].[PrimaryEmail] END AS [PrimaryEmail]
		,CASE WHEN [Source].[PrimaryPhone] IS NULL THEN [Person].[PrimaryPhone] ELSE [Source].[PrimaryPhone] END AS [PrimaryPhone]
		,CASE WHEN [Source].[Suburb] IS NULL THEN [Person].[Suburb] ELSE [Source].[Suburb] END AS [Suburb]
		,CASE WHEN [Source].[SecondaryAddress] IS NULL THEN [Person].[SecondaryAddress] ELSE [Source].[SecondaryAddress] END AS [SecondaryAddress]
		,CASE WHEN [Source].[SecondaryPhone] IS NULL THEN [Person].[SecondaryPhone] ELSE [Source].[SecondaryPhone] END AS [SecondaryPhone]
		,CASE WHEN [Source].[SecondaryEmail] IS NULL THEN [Person].[SecondaryEmail] ELSE [Source].[SecondaryEmail] END AS [SecondaryEmail]
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [Person].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[PractitionerNumber] IS NULL THEN [Person].[PractitionerNumber] ELSE [Source].[PractitionerNumber] END AS [PractitionerNumber]
		,CASE WHEN [Source].[DoNotSendCorrespondence] IS NULL THEN [Person].[DoNotSendCorrespondence] ELSE [Source].[DoNotSendCorrespondence] END AS [DoNotSendCorrespondence]
		,CASE WHEN [Source].[EthicalCompetency] IS NULL THEN [Person].[EthicalCompetency] ELSE [Source].[EthicalCompetency] END AS [EthicalCompetency]
		,CASE WHEN [Source].[InterculturalCompetency] IS NULL THEN [Person].[InterculturalCompetency] ELSE [Source].[InterculturalCompetency] END AS [InterculturalCompetency]
		,CASE WHEN [Source].[State] IS NULL THEN [Person].[State] ELSE [Source].[State] END AS [State]
		,CASE WHEN [Source].[Postcode] IS NULL THEN [Person].[Postcode] ELSE [Source].[Postcode] END AS [Postcode]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [Person].[RolePlayerRating] ELSE [Source].[Rating] END AS [RolePlayerRating]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [Person].[RolePlayerSenior] ELSE [Source].[Senior] END AS [RolePlayerSenior]
		,CASE WHEN [Source].[KnowledgeTest] IS NULL THEN [Person].[KnowledgeTest] ELSE [Source].[KnowledgeTest] END AS [KnowledgeTest]
		,CASE WHEN [Source].[Deceased] IS NULL THEN [Person].[Deceased] ELSE [Source].[Deceased] END AS [Deceased]
		
		,CASE WHEN [Source].[PersonId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			

	FROM
	(
		SELECT
			p.[PersonId]
			,e.[NAATINumber]
			,p.[PractitionerNumber]
			,p.[DoNotSendCorrespondence]
			,pn.[GivenName] COLLATE Latin1_General_CI_AS AS 'GivenName'
			,pn.[OtherNames] COLLATE Latin1_General_CI_AS AS 'OtherNames'
			,pn.[Surname] COLLATE Latin1_General_CI_AS AS 'FamilyName'
			,t.[Title] COLLATE Latin1_General_CI_AS AS 'Title'
			,COALESCE(CAST(p.[Gender] AS CHAR(1)), '') COLLATE Latin1_General_CI_AS AS 'Gender'
			,p.[BirthDate] AS 'DateOfBirth'
			,bc.[Name] COLLATE Latin1_General_CI_AS AS 'CountryOfBirth'
			,a.[StreetDetails] COLLATE Latin1_General_CI_AS AS 'PrimaryAddress'
			,s.[Suburb] COLLATE Latin1_General_CI_AS AS 'Suburb'
			,sta.State COLLATE Latin1_General_CI_AS AS 'State'
			,pc.Postcode COLLATE Latin1_General_CI_AS AS 'Postcode'
			,c.[Name] COLLATE Latin1_General_CI_AS AS 'Country'
			,(SELECT TOP 1 
				[Email]
			  FROM [naati_db]..[tblEmail] email
			  WHERE email.[EntityId] = e.[EntityId] AND email.IsPreferredEmail = 1) COLLATE Latin1_General_CI_AS as 'PrimaryEmail'
			,(SELECT TOP 1 
				[Email]
			  FROM [naati_db]..[tblEmail] email
			  WHERE email.[EntityId] = e.[EntityId] AND email.IsPreferredEmail = 0) COLLATE Latin1_General_CI_AS as 'SecondaryEmail'
			,homePhone.[Number] COLLATE Latin1_General_CI_AS as 'PrimaryPhone'
			,otherPhone.[Number] COLLATE Latin1_General_CI_AS as 'SecondaryPhone'
			,sa.[StreetDetails] AS 'SecondaryAddress'
			,p.EthicalCompetency
			,p.InterculturalCompetency
			,p.KnowledgeTest
			,rp.Rating
			,rp.Senior
			,p.Deceased

		FROM [naati_db]..[tblPerson] p
		JOIN [naati_db]..[tblEntity] e ON e.[EntityId] = p.[EntityId]
		LEFT JOIN (SELECT [EntityId], MAX([AddressId]) AS 'LastAddressId' FROM [naati_db]..[tblAddress] WHERE [PrimaryContact] = 1 GROUP BY [EntityId]) ppAd ON ppAd.[EntityId] = e.[EntityId]
		LEFT JOIN [naati_db]..[tblAddress] a ON ppAd.[LastAddressId] = a.[AddressId]
		LEFT JOIN [naati_db]..[vwDistinctPersonName] dpn ON dpn.[PersonId] = p.[PersonId]
		LEFT JOIN [naati_db]..[tblPersonName] pn ON pn.[PersonNameId] = dpn.[PersonNameId]
		LEFT JOIN [naati_db]..[tluTitle] t ON t.[TitleId] = pn.[TitleId]
		LEFT JOIN [naati_db]..[tblCountry] bc ON bc.[CountryId] = p.[BirthCountryId]
		LEFT JOIN [naati_db]..[tblCountry] c ON c.[CountryId] = a.[CountryId]
		LEFT JOIN [naati_db]..[tblPostcode] pc ON pc.[PostcodeId] = a.[PostcodeId]
		LEFT JOIN [naati_db]..[tblSuburb] s ON s.[SuburbId] = pc.[SuburbId]
		LEFT JOIN [naati_db]..[tluState] sta ON sta.[StateId] = s.[StateId]
		LEFT JOIN [naati_db]..[tblRolePlayer] rp ON p.PersonId = rp.PersonId

		LEFT JOIN
		(
			SELECT TOP 1
			[StreetDetails]
			,[EntityId]
			FROM [naati_db]..[tblAddress] sa
			WHERE Invalid = 0 AND PrimaryContact = 0
		) sa ON sa.EntityId = e.EntityId
		OUTER APPLY [dbo].[GetPhoneNumber](e.[EntityId], 1) homePhone
		OUTER APPLY [dbo].[GetPhoneNumber](e.[EntityId], 0) otherPhone
	) [Source]
	FULL OUTER JOIN [Person] ON [Source].[PersonId] = [Person].[PersonId]
	WHERE (([Source].[GivenName] IS NOT NULL AND [Person].[GivenName] IS NULL) OR ([Source].[GivenName] IS NULL AND [Person].[GivenName] IS NOT NULL) OR (([Source].[GivenName] IS NOT NULL AND [Person].[GivenName] IS NOT NULL) AND [Source].[GivenName] != [Person].[GivenName]))
	OR (([Source].[OtherNames] IS NOT NULL AND [Person].[OtherNames] IS NULL) OR ([Source].[OtherNames] IS NULL AND [Person].[OtherNames] IS NOT NULL) OR (([Source].[OtherNames] IS NOT NULL AND [Person].[OtherNames] IS NOT NULL) AND [Source].[OtherNames] != [Person].[OtherNames]))
	OR (([Source].[FamilyName] IS NOT NULL AND [Person].[FamilyName] IS NULL) OR ([Source].[FamilyName] IS NULL AND [Person].[FamilyName] IS NOT NULL) OR (([Source].[FamilyName] IS NOT NULL AND [Person].[FamilyName] IS NOT NULL) AND [Source].[FamilyName] != [Person].[FamilyName]))
	OR (([Source].[NAATINumber] IS NOT NULL AND [Person].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [Person].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [Person].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [Person].[NAATINumber]))
	OR (([Source].[PractitionerNumber] IS NOT NULL AND [Person].[PractitionerNumber] IS NULL) OR ([Source].[PractitionerNumber] IS NULL AND [Person].[PractitionerNumber] IS NOT NULL) OR (([Source].[PractitionerNumber] IS NOT NULL AND [Person].[PractitionerNumber] IS NOT NULL) AND [Source].[PractitionerNumber] != [Person].[PractitionerNumber]))
	OR (([Source].[DoNotSendCorrespondence] IS NOT NULL AND [Person].[DoNotSendCorrespondence] IS NULL) OR ([Source].[DoNotSendCorrespondence] IS NULL AND [Person].[DoNotSendCorrespondence] IS NOT NULL) OR (([Source].[DoNotSendCorrespondence] IS NOT NULL AND [Person].[DoNotSendCorrespondence] IS NOT NULL) AND [Source].[DoNotSendCorrespondence] != [Person].[DoNotSendCorrespondence]))
	OR (([Source].[Title] IS NOT NULL AND [Person].[Title] IS NULL) OR ([Source].[Title] IS NULL AND [Person].[Title] IS NOT NULL) OR (([Source].[Title] IS NOT NULL AND [Person].[Title] IS NOT NULL) AND [Source].[Title] != [Person].[Title])) 
	OR (([Source].[Gender] IS NOT NULL AND [Person].[Gender] IS NULL) OR ([Source].[Gender] IS NULL AND [Person].[Gender] IS NOT NULL) OR (([Source].[Gender] IS NOT NULL AND [Person].[Gender] IS NOT NULL) AND [Source].[Gender] != [Person].[Gender]))
	OR (([Source].[DateOfBirth] IS NOT NULL AND [Person].[DateOfBirth] IS NULL) OR ([Source].[DateOfBirth] IS NULL AND [Person].[DateOfBirth] IS NOT NULL) OR (([Source].[DateOfBirth] IS NOT NULL AND [Person].[DateOfBirth] IS NOT NULL) AND [Source].[DateOfBirth] != [Person].[DateOfBirth]))
	OR (([Source].[CountryOfBirth] IS NOT NULL AND [Person].[CountryOfBirth] IS NULL) OR ([Source].[CountryOfBirth] IS NULL AND [Person].[CountryOfBirth] IS NOT NULL) OR (([Source].[CountryOfBirth] IS NOT NULL AND [Person].[CountryOfBirth] IS NOT NULL) AND [Source].[CountryOfBirth] != [Person].[CountryOfBirth]))
	OR (([Source].[PrimaryAddress] IS NOT NULL AND [Person].[PrimaryAddress] IS NULL) OR ([Source].[PrimaryAddress] IS NULL AND [Person].[PrimaryAddress] IS NOT NULL) OR (([Source].[PrimaryAddress] IS NOT NULL AND [Person].[PrimaryAddress] IS NOT NULL) AND [Source].[PrimaryAddress] != [Person].[PrimaryAddress]))
	OR (([Source].[SecondaryAddress] IS NOT NULL AND [Person].[SecondaryAddress] IS NULL) OR ([Source].[SecondaryAddress] IS NULL AND [Person].[SecondaryAddress] IS NOT NULL) OR (([Source].[SecondaryAddress] IS NOT NULL AND [Person].[SecondaryAddress] IS NOT NULL) AND [Source].[SecondaryAddress] != [Person].[SecondaryAddress]))
	OR (([Source].[Suburb] IS NOT NULL AND [Person].[Suburb] IS NULL) OR ([Source].[Suburb] IS NULL AND [Person].[Suburb] IS NOT NULL) OR (([Source].[Suburb] IS NOT NULL AND [Person].[Suburb] IS NOT NULL) AND [Source].[Suburb] != [Person].[Suburb])) 
	OR (([Source].[State] IS NOT NULL AND [Person].[State] IS NULL) OR ([Source].[State] IS NULL AND [Person].[State] IS NOT NULL) OR (([Source].[State] IS NOT NULL AND [Person].[State] IS NOT NULL) AND [Source].[State] != [Person].[State]))
	OR (([Source].[Postcode] IS NOT NULL AND [Person].[Postcode] IS NULL) OR ([Source].[Postcode] IS NULL AND [Person].[Postcode] IS NOT NULL) OR (([Source].[Postcode] IS NOT NULL AND [Person].[Postcode] IS NOT NULL) AND [Source].[Postcode] != [Person].[Postcode]))
	OR (([Source].[Country] IS NOT NULL AND [Person].[Country] IS NULL) OR ([Source].[Country] IS NULL AND [Person].[Country] IS NOT NULL) OR (([Source].[Country] IS NOT NULL AND [Person].[Country] IS NOT NULL) AND [Source].[Country] != [Person].[Country]))
	OR (([Source].[PrimaryEmail] IS NOT NULL AND [Person].[PrimaryEmail] IS NULL) OR ([Source].[PrimaryEmail] IS NULL AND [Person].[PrimaryEmail] IS NOT NULL) OR (([Source].[PrimaryEmail] IS NOT NULL AND [Person].[PrimaryEmail] IS NOT NULL) AND [Source].[PrimaryEmail] != [Person].[PrimaryEmail]))
	OR (([Source].[SecondaryEmail] IS NOT NULL AND [Person].[SecondaryEmail] IS NULL) OR ([Source].[SecondaryEmail] IS NULL AND [Person].[SecondaryEmail] IS NOT NULL) OR (([Source].[SecondaryEmail] IS NOT NULL AND [Person].[SecondaryEmail] IS NOT NULL) AND [Source].[SecondaryEmail] != [Person].[SecondaryEmail]))
	OR (([Source].[PrimaryPhone] IS NOT NULL AND [Person].[PrimaryPhone] IS NULL) OR ([Source].[PrimaryPhone] IS NULL AND [Person].[PrimaryPhone] IS NOT NULL) OR (([Source].[PrimaryPhone] IS NOT NULL AND [Person].[PrimaryPhone] IS NOT NULL) AND [Source].[PrimaryPhone] != [Person].[PrimaryPhone]))
	OR (([Source].[SecondaryPhone] IS NOT NULL AND [Person].[SecondaryPhone] IS NULL) OR ([Source].[SecondaryPhone] IS NULL AND [Person].[SecondaryPhone] IS NOT NULL) OR (([Source].[SecondaryPhone] IS NOT NULL AND [Person].[SecondaryPhone] IS NOT NULL) AND [Source].[SecondaryPhone] != [Person].[SecondaryPhone]))
	OR (([Source].[EthicalCompetency] IS NOT NULL AND [Person].[EthicalCompetency] IS NULL) OR ([Source].[EthicalCompetency] IS NULL AND [Person].[EthicalCompetency] IS NOT NULL) OR (([Source].[EthicalCompetency] IS NOT NULL AND [Person].[EthicalCompetency] IS NOT NULL) AND [Source].[EthicalCompetency] != [Person].[EthicalCompetency]))
	OR (([Source].[InterculturalCompetency] IS NOT NULL AND [Person].[InterculturalCompetency] IS NULL) OR ([Source].[InterculturalCompetency] IS NULL AND [Person].[InterculturalCompetency] IS NOT NULL) OR (([Source].[InterculturalCompetency] IS NOT NULL AND [Person].[InterculturalCompetency] IS NOT NULL) AND [Source].[InterculturalCompetency] != [Person].[InterculturalCompetency]))
	OR (([Source].[KnowledgeTest] IS NOT NULL AND [Person].[KnowledgeTest] IS NULL) OR ([Source].[KnowledgeTest] IS NULL AND [Person].[KnowledgeTest] IS NOT NULL) OR (([Source].[KnowledgeTest] IS NOT NULL AND [Person].[KnowledgeTest] IS NOT NULL) AND [Source].[KnowledgeTest] != [Person].[KnowledgeTest]))
	OR (([Source].[Rating] IS NOT NULL AND [Person].[RolePlayerRating] IS NULL) OR ([Source].[Rating] IS NULL AND [Person].[RolePlayerRating] IS NOT NULL) OR (([Source].[Rating] IS NOT NULL AND [Person].[RolePlayerRating] IS NOT NULL) AND [Source].[Rating] != [Person].[RolePlayerRating]))
	OR (([Source].[Senior] IS NOT NULL AND [Person].[RolePlayerSenior] IS NULL) OR ([Source].[Senior] IS NULL AND [Person].[RolePlayerSenior] IS NOT NULL) OR (([Source].[Senior] IS NOT NULL AND [Person].[RolePlayerSenior] IS NOT NULL) AND [Source].[Senior] != [Person].[RolePlayerSenior]))
	OR (([Source].[Deceased] IS NOT NULL AND [Person].[Deceased] IS NULL) OR ([Source].[Deceased] IS NULL AND [Person].[Deceased] IS NOT NULL) OR (([Source].[Deceased] IS NOT NULL AND [Person].[Deceased] IS NOT NULL) AND [Source].[Deceased] != [Person].[Deceased]))

	--select * from @PersonHistory
		
	BEGIN TRANSACTION
	   --Merge operation delete
		MERGE PersonHistory AS Target USING(select * from @PersonHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[PersonId] = Source.[PersonId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE PersonHistory AS Target USING(	select * from @PersonHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[PersonId] = Source.[PersonId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE PersonHistory AS Target USING(	select * from @PersonHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[PersonId] = Source.[PersonId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.PersonId = Source.PersonId		  
		  ,Target.GivenName = Source.GivenName
		  ,Target.OtherNames = Source.OtherNames
		  ,Target.FamilyName = Source.FamilyName
		  ,Target.Title = Source.Title      
		  ,Target.Gender = Source.Gender
		  ,Target.DateOfBirth = Source.DateOfBirth
		  ,Target.CountryOfBirth = Source.CountryOfBirth
		  ,Target.PrimaryAddress = Source.PrimaryAddress

		  ,Target.Country = Source.Country      
		  ,Target.PrimaryEmail = Source.PrimaryEmail      
		  ,Target.PrimaryPhone = Source.PrimaryPhone      
		  ,Target.Suburb = Source.Suburb      
		  ,Target.SecondaryAddress = Source.SecondaryAddress      
		  ,Target.SecondaryPhone = Source.SecondaryPhone      
		  ,Target.SecondaryEmail = Source.SecondaryEmail      
		  ,Target.NAATINumber = Source.NAATINumber      
		  ,Target.PractitionerNumber = Source.PractitionerNumber      
		  ,Target.DoNotSendCorrespondence = Source.DoNotSendCorrespondence      
		  ,Target.EthicalCompetency = Source.EthicalCompetency      
		  ,Target.InterculturalCompetency = Source.InterculturalCompetency      

		  ,Target.State = Source.State      
		  ,Target.Postcode = Source.Postcode      
		  ,Target.RolePlayerRating = Source.RolePlayerRating      
		  ,Target.RolePlayerSenior = Source.RolePlayerSenior      
		  ,Target.KnowledgeTest = Source.KnowledgeTest   
		  ,Target.ModifiedDate = @Date
		  ,Target.Deceased = Source.Deceased
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([PersonId],[ModifiedDate],[GivenName],[OtherNames],[FamilyName],[Title],[Gender],[DateOfBirth],[CountryOfBirth],[PrimaryAddress],[Country],[PrimaryEmail],[PrimaryPhone],[Suburb],
			 [SecondaryAddress],[SecondaryPhone],[SecondaryEmail],[NAATINumber],[PractitionerNumber],[DoNotSendCorrespondence],[EthicalCompetency],[InterculturalCompetency],
			 [State],[Postcode],[RolePlayerRating],[RolePlayerSenior],[KnowledgeTest],[Deceased],[RowStatus])
	  
		 VALUES (Source.[PersonId],@Date,Source.[GivenName],Source.[OtherNames],Source.[FamilyName],Source.[Title],Source.[Gender],Source.[DateOfBirth],Source.[CountryOfBirth],
		      Source.[PrimaryAddress],Source.[Country],Source.[PrimaryEmail],Source.[PrimaryPhone],Source.[Suburb],Source.[SecondaryAddress],Source.[SecondaryPhone],Source.[SecondaryEmail],
			  Source.[NAATINumber],Source.[PractitionerNumber],Source.[DoNotSendCorrespondence],Source.[EthicalCompetency],Source.[InterculturalCompetency],Source.[State],Source.[Postcode],
			  Source.[RolePlayerRating],Source.[RolePlayerSenior],Source.[KnowledgeTest],Source.[Deceased], 'Latest');
		  	
	COMMIT TRANSACTION;	

END

