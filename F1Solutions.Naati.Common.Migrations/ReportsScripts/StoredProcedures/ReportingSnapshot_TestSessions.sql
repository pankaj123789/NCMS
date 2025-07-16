ALTER PROCEDURE [dbo].[ReportingSnapshot_TestSessions]
		@Date DateTime
AS
BEGIN
	DECLARE @TestSessionsHistory as table([TestSittingId] [int] NOT NULL,[TestSessionId] [int] NOT NULL,[CredentialRequestId] [int] NOT NULL,[TestSessionName] [nvarchar](200) NULL,[TestLocationState] [nvarchar](50) NULL,[TestLocationCountry] [nvarchar](50) NULL,
										  [TestLocationName] [nvarchar](1000) NULL,[VenueName] [nvarchar](200) NULL,[VenueAddress] [nvarchar](510) NULL,[TestDate] [datetime] NULL,[TestStartTime] [datetime] NULL,[TestArrivalTime] [datetime] NULL,
										  [TestEndTime] [datetime] NULL,[ApplicationType] [nvarchar](100) NULL,[CredentialTypeInternalName] [nvarchar](100) NULL,[CredentialTypeExternalName] [nvarchar](100) NULL,[TestSessionCompleted] [bit] NULL,
										  [PersonId] [int] NOT NULL,[CustomerNo] [int] NULL,[Title] [nvarchar](50) NULL,[GivenName] [nvarchar](100) NULL,[OtherNames] [nvarchar](100) NULL,[FamilyName] [nvarchar](100) NULL,[PrimaryAddress] [nvarchar](500) NULL,
										  [State] [char](50) NULL,[Postcode] [char](4) NULL,[Country] [nvarchar](50) NULL,[PrimaryPhone] [nvarchar](60) NULL,[PrimaryEmail] [nvarchar](200) NULL,[ApplicationId] [int] NULL,[ApplicationReference] [nvarchar](10) NULL,
										  [Certification] [nvarchar](100) NULL,[Language1] [nvarchar](50) NULL,[Language1Code] [nvarchar](10) NULL,[Language1Group] [nvarchar](100) NULL,[Language2] [nvarchar](50) NULL,[Language2Code] [nvarchar](50) NULL,
										  [Language2Group] [nvarchar](100) NULL,[Skill] [nvarchar](100) NULL,[Status] [nvarchar](100) NULL,[StatusDateModified] [datetime] NOT NULL,[StatusModifiedUser] [nvarchar](100) NULL,[ModifiedDate] [datetime] NOT NULL,
										  [Capacity] [int] NULL,[VenueCapacityOverridden] [bit] NULL, [RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	DECLARE 
		@testSessionAllocated INT = 18,
		@testSessionAccepted INT = 19

	INSERT into @TestSessionsHistory

	SELECT 
		CASE WHEN [Source].[TestSittingId] IS NULL THEN [TestSessions].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		,CASE WHEN [Source].[TestSessionId] IS NULL THEN [TestSessions].[TestSessionId] ELSE [Source].[TestSessionId] END AS [TestSessionId]
		,CASE WHEN [Source].[CredentialRequestId] IS NULL THEN [TestSessions].[CredentialRequestId] ELSE [Source].[CredentialRequestId] END AS [CredentialRequestId]
		,CASE WHEN [Source].[TestSessionName] IS NULL THEN [TestSessions].[TestSessionName] ELSE [Source].[TestSessionName] END AS [TestSessionName]
		,CASE WHEN [Source].[TestLocationState] IS NULL THEN [TestSessions].[TestLocationState] ELSE [Source].[TestLocationState] END AS [TestLocationState]
		,CASE WHEN [Source].[TestLocationCountry] IS NULL THEN [TestSessions].[TestLocationCountry] ELSE [Source].[TestLocationCountry] END AS [TestLocationCountry]
		,CASE WHEN [Source].[TestLocationName] IS NULL THEN [TestSessions].[TestLocationName] ELSE [Source].[TestLocationName] END AS [TestLocationName]
		,CASE WHEN [Source].[VenueName] IS NULL THEN [TestSessions].[VenueName] ELSE [Source].[VenueName] END AS [VenueName]
		,CASE WHEN [Source].[VenueAddress] IS NULL THEN [TestSessions].[VenueAddress] ELSE [Source].[VenueAddress] END AS [VenueAddress]
		,CASE WHEN [Source].[TestDate] IS NULL THEN [TestSessions].[TestDate] ELSE [Source].[TestDate] END AS [TestDate]
		,CASE WHEN [Source].[TestStartTime] IS NULL THEN [TestSessions].[TestStartTime] ELSE [Source].[TestStartTime] END AS [TestStartTime]
		,CASE WHEN [Source].[TestArrivalTime] IS NULL THEN [TestSessions].[TestArrivalTime] ELSE [Source].[TestArrivalTime] END AS [TestArrivalTime]
		,CASE WHEN [Source].[TestEndTime] IS NULL THEN [TestSessions].[TestEndTime] ELSE [Source].[TestEndTime] END AS [TestEndTime]
		,CASE WHEN [Source].[ApplicationType] IS NULL THEN [TestSessions].[ApplicationType] ELSE [Source].[ApplicationType] END AS [ApplicationType]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [TestSessions].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [TestSessions].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[TestSessionCompleted] IS NULL THEN [TestSessions].[TestSessionCompleted] ELSE [Source].[TestSessionCompleted] END AS [TestSessionCompleted]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [TestSessions].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNo] IS NULL THEN [TestSessions].[CustomerNo] ELSE [Source].[CustomerNo] END AS [CustomerNo]
		,CASE WHEN [Source].[Title] IS NULL THEN [TestSessions].[Title] ELSE [Source].[Title] END AS [Title]
		,CASE WHEN [Source].[GivenName] IS NULL THEN [TestSessions].[GivenName] ELSE [Source].[GivenName] END AS [GivenName]
		,CASE WHEN [Source].[OtherNames] IS NULL THEN [TestSessions].[OtherNames] ELSE [Source].[OtherNames] END AS [OtherNames]
		,CASE WHEN [Source].[FamilyName] IS NULL THEN [TestSessions].[FamilyName] ELSE [Source].[FamilyName] END AS [FamilyName]
		,CASE WHEN [Source].[PrimaryAddress] IS NULL THEN [TestSessions].[PrimaryAddress] ELSE [Source].[PrimaryAddress] END AS [PrimaryAddress]
		,CASE WHEN [Source].[State] IS NULL THEN [TestSessions].[State] ELSE [Source].[State] END AS [State]
		,CASE WHEN [Source].[Postcode] IS NULL THEN [TestSessions].[Postcode] ELSE [Source].[Postcode] END AS [Postcode]
		,CASE WHEN [Source].[Country] IS NULL THEN [TestSessions].[Country] ELSE [Source].[Country] END AS [Country]
		,CASE WHEN [Source].[PrimaryPhone] IS NULL THEN [TestSessions].[PrimaryPhone] ELSE [Source].[PrimaryPhone] END AS [PrimaryPhone]
		,CASE WHEN [Source].[PrimaryEmail] IS NULL THEN [TestSessions].[PrimaryEmail] ELSE [Source].[PrimaryEmail] END AS [PrimaryEmail]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [TestSessions].[ApplicationId] ELSE [Source].[ApplicationId] END AS [ApplicationId]
		,CASE WHEN [Source].[ApplicationReference] IS NULL THEN [TestSessions].[ApplicationReference] ELSE [Source].[ApplicationReference] END AS [ApplicationReference]
		,CASE WHEN [Source].[Certification] IS NULL THEN [TestSessions].[Certification] ELSE [Source].[Certification] END AS [Certification]
		,CASE WHEN [Source].[Language1] IS NULL THEN [TestSessions].[Language1] ELSE [Source].[Language1] END AS [Language1]
		,CASE WHEN [Source].[Language1Code] IS NULL THEN [TestSessions].[Language1Code] ELSE [Source].[Language1Code] END AS [Language1Code]
		,CASE WHEN [Source].[Language1Group] IS NULL THEN [TestSessions].[Language1Group] ELSE [Source].[Language1Group] END AS [Language1Group]
		,CASE WHEN [Source].[Language2] IS NULL THEN [TestSessions].[Language2] ELSE [Source].[Language2] END AS [Language2]
		,CASE WHEN [Source].[Language2Code] IS NULL THEN [TestSessions].[Language2Code] ELSE [Source].[Language2Code] END AS [Language2Code]
		,CASE WHEN [Source].[Language2Group] IS NULL THEN [TestSessions].[Language2Group] ELSE [Source].[Language2Group] END AS [Language2Group]
		,CASE WHEN [Source].[Skill] IS NULL THEN [TestSessions].[Skill] ELSE [Source].[Skill] END AS [Skill]
		,CASE WHEN [Source].[Status] IS NULL THEN [TestSessions].[Status] ELSE [Source].[Status] END AS [Status]
		,CASE WHEN [Source].[StatusDateModified] IS NULL THEN [TestSessions].[StatusDateModified] ELSE [Source].[StatusDateModified] END AS [StatusDateModified]
		,CASE WHEN [Source].[StatusModifiedUser] IS NULL THEN [TestSessions].[StatusModifiedUser] ELSE [Source].[StatusModifiedUser] END AS [StatusModifiedUser]
		
		,@Date AS [ModifiedDate]
		
		,CASE WHEN [Source].[Capacity] IS NULL THEN [TestSessions].[Capacity] ELSE [Source].[Capacity] END AS [Capacity]
		,CASE WHEN [Source].[VenueCapacityOverridden] IS NULL THEN [TestSessions].[VenueCapacityOverridden] ELSE [Source].[VenueCapacityOverridden] END AS [VenueCapacityOverridden]
		
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM 
	(
		SELECT 
			tscr.TestSittingId TestSittingId
			,ts.TestSessionId TestSessionId
			,cr.CredentialRequestId CredentialRequestId
			,ts.Name TestSessionName
			,s.State TestLocationState
			,c.Name TestLocationCountry
			,tl.Name TestLocationName
			,v.Name VenueName
			,v.Address VenueAddress
			,ts.TestDateTime TestDate
			,ts.TestDateTime TestStartTime
			,DATEADD(minute, -1 * ts.ArrivalTime, ts.TestDateTime) TestArrivalTime --(Note that this needs to be calculated)
			,DATEADD(minute, ts.Duration, ts.TestDateTime) TestEndTime --(Note that this needs to be calculated)
			,cat.DisplayName ApplicationType
			,ct.InternalName CredentialTypeInternalName
			,ct.ExternalName CredentialTypeExternalName
			,ts.Completed TestSessionCompleted
			,p.PersonId PersonId
			,e.NAATINumber CustomerNo
			,t.Title Title
			,pn.GivenName GivenName
			,pn.OtherNames OtherNames
			,pn.Surname FamilyName
			,ad.StreetDetails PrimaryAddress
			,adst.State State
			,pc.Postcode Postcode
			,adc.Name Country
			,ph.Number PrimaryPhone
			,em.Email PrimaryEmail
			,ca.CredentialApplicationId ApplicationId
			,'APP' + CAST(ca.CredentialApplicationId AS VARCHAR) ApplicationReference
			,ct.Certification Certification
			,l1.Name Language1
			,l1.Code Language1Code
			,lg1.Name Language1Group
			,l2.Name Language2
			,l2.Code Language2Code
			,lg2.Name Language2Group
			,REPLACE(REPLACE(d.[DisplayName], '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) COLLATE Latin1_General_CI_AS Skill
			,CASE 
				WHEN tscr.Rejected = 1 then 'Session Rejected'
				WHEN cr.CredentialRequestStatusTypeId = @testSessionAccepted THEN 'Confirmed on ' + CONVERT(VARCHAR, cr.StatusChangeDate, 103)
				WHEN cr.CredentialRequestStatusTypeId = @testSessionAllocated THEN 'Awaiting Confirmation'
			END 'Status'
			,ca.StatusChangeDate StatusDateModified
			,uca.FullName StatusModifiedUser
			,COALESCE(ts.Capacity, v.Capacity) Capacity
			,OverrideVenueCapacity VenueCapacityOverridden

		FROM [naati_db]..[tblTestSession] ts
			LEFT JOIN [naati_db]..[tblVenue] v ON ts.VenueId = v.VenueId
			LEFT JOIN [naati_db]..[tblTestLocation] tl ON v.TestLocationId = tl.TestLocationId
			LEFT JOIN [naati_db]..[tblOffice] o ON tl.OfficeId = o.OfficeId
			LEFT JOIN [naati_db]..[tluState] s ON o.StateId = s.StateId
			LEFT JOIN [naati_db]..[tblCountry] c ON tl.CountryId = c.CountryId
			INNER JOIN [naati_db]..[tblTestSitting] tscr ON ts.TestSessionId = tscr.TestSessionId
			LEFT JOIN [naati_db]..[tblCredentialRequest] cr ON tscr.CredentialRequestId = cr.CredentialRequestId
			LEFT JOIN [naati_db]..[tblCredentialApplication] ca ON cr.CredentialApplicationId = ca.CredentialApplicationId
			LEFT JOIN [naati_db]..[tblCredentialApplicationType] cat ON ca.CredentialApplicationTypeId = cat.CredentialApplicationTypeId
			LEFT JOIN [naati_db]..[tblCredentialType] ct ON cr.CredentialTypeId = ct.CredentialTypeId
			LEFT JOIN [naati_db]..[tblPerson] p ON ca.PersonId = p.PersonId
			LEFT JOIN [naati_db]..[tblEntity] e ON p.EntityId = e.EntityId
			LEFT JOIN [naati_db]..[vwDistinctPersonName] dpn ON p.PersonId = dpn.PersonId
			LEFT JOIN [naati_db]..[tblPersonName] pn ON dpn.PersonNameId = pn.PersonNameId
			LEFT JOIN [naati_db]..[tluTitle] t ON pn.TitleId = t.TitleId
			LEFT JOIN [naati_db]..[tblAddress] ad ON e.EntityId = ad.EntityId
				AND ad.PrimaryContact = 1
			LEFT JOIN [naati_db]..[tblPostcode] pc ON ad.PostcodeId = pc.PostcodeId
			LEFT JOIN [naati_db]..[tblCountry] adc ON ad.CountryId = adc.CountryId
			LEFT JOIN [naati_db]..[tblSuburb] ads ON pc.SuburbId = ads.SuburbId
			LEFT JOIN [naati_db]..[tluState] adst ON ads.StateId = adst.StateId
			LEFT JOIN [naati_db]..[tblPhone] ph ON e.EntityId = ph.EntityId
				AND ph.PrimaryContact = 1
			LEFT JOIN [naati_db]..[tblEmail] em ON e.EntityId = em.EntityId
				AND em.IsPreferredEmail = 1
			LEFT JOIN [naati_db]..[tblSkill] sk ON cr.SkillId = sk.SkillId
			LEFT JOIN [naati_db]..[tblLanguage] l1 ON sk.Language1Id = l1.LanguageId
			LEFT JOIN [naati_db]..[tblLanguage] l2 ON sk.Language2Id = l2.LanguageId
			LEFT JOIN [naati_db]..[tblLanguageGroup] lg1 ON l1.LanguageGroupId = lg1.LanguageGroupId
			LEFT JOIN [naati_db]..[tblLanguageGroup] lg2 ON l2.LanguageGroupId = lg2.LanguageGroupId
			LEFT JOIN [naati_db]..[tblDirectionType] d ON d.[DirectionTypeId] = sk.[DirectionTypeId]	
			LEFT JOIN [naati_db]..[tblSkillType] st ON sk.SkillTypeId = st.SkillTypeId
			LEFT JOIN [naati_db]..[tblUser] uca ON ca.StatusChangeUserId = uca.UserId
	) [Source]
	FULL OUTER JOIN [TestSessions] ON ([Source].[TestSittingId] = [TestSessions].[TestSittingId] AND [Source].[TestSessionId] = [TestSessions].[TestSessionId]) 
	WHERE 
		(([Source].[TestSittingId] IS NOT NULL AND [TestSessions].[TestSittingId] IS NULL) OR ([Source].[TestSittingId] IS NULL AND [TestSessions].[TestSittingId] IS NOT NULL) OR (([Source].[TestSittingId] IS NOT NULL AND [TestSessions].[TestSittingId] IS NOT NULL) AND [Source].[TestSittingId] != [TestSessions].[TestSittingId]))
		OR (([Source].[TestSessionId] IS NOT NULL AND [TestSessions].[TestSessionId] IS NULL) OR ([Source].[TestSessionId] IS NULL AND [TestSessions].[TestSessionId] IS NOT NULL) OR (([Source].[TestSessionId] IS NOT NULL AND [TestSessions].[TestSessionId] IS NOT NULL) AND [Source].[TestSessionId] != [TestSessions].[TestSessionId]))
		OR (([Source].[CredentialRequestId] IS NOT NULL AND [TestSessions].[CredentialRequestId] IS NULL) OR ([Source].[CredentialRequestId] IS NULL AND [TestSessions].[CredentialRequestId] IS NOT NULL) OR (([Source].[CredentialRequestId] IS NOT NULL AND [TestSessions].[CredentialRequestId] IS NOT NULL) AND [Source].[CredentialRequestId] != [TestSessions].[CredentialRequestId]))
		OR (([Source].[TestSessionName] IS NOT NULL AND [TestSessions].[TestSessionName] IS NULL) OR ([Source].[TestSessionName] IS NULL AND [TestSessions].[TestSessionName] IS NOT NULL) OR (([Source].[TestSessionName] IS NOT NULL AND [TestSessions].[TestSessionName] IS NOT NULL) AND [Source].[TestSessionName] != [TestSessions].[TestSessionName]))
		OR (([Source].[TestLocationState] IS NOT NULL AND [TestSessions].[TestLocationState] IS NULL) OR ([Source].[TestLocationState] IS NULL AND [TestSessions].[TestLocationState] IS NOT NULL) OR (([Source].[TestLocationState] IS NOT NULL AND [TestSessions].[TestLocationState] IS NOT NULL) AND [Source].[TestLocationState] != [TestSessions].[TestLocationState]))
		OR (([Source].[TestLocationCountry] IS NOT NULL AND [TestSessions].[TestLocationCountry] IS NULL) OR ([Source].[TestLocationCountry] IS NULL AND [TestSessions].[TestLocationCountry] IS NOT NULL) OR (([Source].[TestLocationCountry] IS NOT NULL AND [TestSessions].[TestLocationCountry] IS NOT NULL) AND [Source].[TestLocationCountry] != [TestSessions].[TestLocationCountry]))
		OR (([Source].[TestLocationName] IS NOT NULL AND [TestSessions].[TestLocationName] IS NULL) OR ([Source].[TestLocationName] IS NULL AND [TestSessions].[TestLocationName] IS NOT NULL) OR (([Source].[TestLocationName] IS NOT NULL AND [TestSessions].[TestLocationName] IS NOT NULL) AND [Source].[TestLocationName] != [TestSessions].[TestLocationName]))
		OR (([Source].[VenueName] IS NOT NULL AND [TestSessions].[VenueName] IS NULL) OR ([Source].[VenueName] IS NULL AND [TestSessions].[VenueName] IS NOT NULL) OR (([Source].[VenueName] IS NOT NULL AND [TestSessions].[VenueName] IS NOT NULL) AND [Source].[VenueName] != [TestSessions].[VenueName]))
		OR (([Source].[VenueAddress] IS NOT NULL AND [TestSessions].[VenueAddress] IS NULL) OR ([Source].[VenueAddress] IS NULL AND [TestSessions].[VenueAddress] IS NOT NULL) OR (([Source].[VenueAddress] IS NOT NULL AND [TestSessions].[VenueAddress] IS NOT NULL) AND [Source].[VenueAddress] != [TestSessions].[VenueAddress]))
		OR (([Source].[TestDate] IS NOT NULL AND [TestSessions].[TestDate] IS NULL) OR ([Source].[TestDate] IS NULL AND [TestSessions].[TestDate] IS NOT NULL) OR (([Source].[TestDate] IS NOT NULL AND [TestSessions].[TestDate] IS NOT NULL) AND [Source].[TestDate] != [TestSessions].[TestDate]))
		OR (([Source].[TestStartTime] IS NOT NULL AND [TestSessions].[TestStartTime] IS NULL) OR ([Source].[TestStartTime] IS NULL AND [TestSessions].[TestStartTime] IS NOT NULL) OR (([Source].[TestStartTime] IS NOT NULL AND [TestSessions].[TestStartTime] IS NOT NULL) AND [Source].[TestStartTime] != [TestSessions].[TestStartTime]))
		OR (([Source].[TestArrivalTime] IS NOT NULL AND [TestSessions].[TestArrivalTime] IS NULL) OR ([Source].[TestArrivalTime] IS NULL AND [TestSessions].[TestArrivalTime] IS NOT NULL) OR (([Source].[TestArrivalTime] IS NOT NULL AND [TestSessions].[TestArrivalTime] IS NOT NULL) AND [Source].[TestArrivalTime] != [TestSessions].[TestArrivalTime]))
		OR (([Source].[TestEndTime] IS NOT NULL AND [TestSessions].[TestEndTime] IS NULL) OR ([Source].[TestEndTime] IS NULL AND [TestSessions].[TestEndTime] IS NOT NULL) OR (([Source].[TestEndTime] IS NOT NULL AND [TestSessions].[TestEndTime] IS NOT NULL) AND [Source].[TestEndTime] != [TestSessions].[TestEndTime]))
		OR (([Source].[ApplicationType] IS NOT NULL AND [TestSessions].[ApplicationType] IS NULL) OR ([Source].[ApplicationType] IS NULL AND [TestSessions].[ApplicationType] IS NOT NULL) OR (([Source].[ApplicationType] IS NOT NULL AND [TestSessions].[ApplicationType] IS NOT NULL) AND [Source].[ApplicationType] != [TestSessions].[ApplicationType]))
		OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestSessions].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [TestSessions].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestSessions].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [TestSessions].[CredentialTypeInternalName]))
		OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestSessions].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [TestSessions].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestSessions].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [TestSessions].[CredentialTypeExternalName]))
		OR (([Source].[TestSessionCompleted] IS NOT NULL AND [TestSessions].[TestSessionCompleted] IS NULL) OR ([Source].[TestSessionCompleted] IS NULL AND [TestSessions].[TestSessionCompleted] IS NOT NULL) OR (([Source].[TestSessionCompleted] IS NOT NULL AND [TestSessions].[TestSessionCompleted] IS NOT NULL) AND [Source].[TestSessionCompleted] != [TestSessions].[TestSessionCompleted]))
		OR (([Source].[PersonId] IS NOT NULL AND [TestSessions].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [TestSessions].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [TestSessions].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [TestSessions].[PersonId]))
		OR (([Source].[CustomerNo] IS NOT NULL AND [TestSessions].[CustomerNo] IS NULL) OR ([Source].[CustomerNo] IS NULL AND [TestSessions].[CustomerNo] IS NOT NULL) OR (([Source].[CustomerNo] IS NOT NULL AND [TestSessions].[CustomerNo] IS NOT NULL) AND [Source].[CustomerNo] != [TestSessions].[CustomerNo]))
		OR (([Source].[Title] IS NOT NULL AND [TestSessions].[Title] IS NULL) OR ([Source].[Title] IS NULL AND [TestSessions].[Title] IS NOT NULL) OR (([Source].[Title] IS NOT NULL AND [TestSessions].[Title] IS NOT NULL) AND [Source].[Title] != [TestSessions].[Title]))
		OR (([Source].[GivenName] IS NOT NULL AND [TestSessions].[GivenName] IS NULL) OR ([Source].[GivenName] IS NULL AND [TestSessions].[GivenName] IS NOT NULL) OR (([Source].[GivenName] IS NOT NULL AND [TestSessions].[GivenName] IS NOT NULL) AND [Source].[GivenName] != [TestSessions].[GivenName]))
		OR (([Source].[OtherNames] IS NOT NULL AND [TestSessions].[OtherNames] IS NULL) OR ([Source].[OtherNames] IS NULL AND [TestSessions].[OtherNames] IS NOT NULL) OR (([Source].[OtherNames] IS NOT NULL AND [TestSessions].[OtherNames] IS NOT NULL) AND [Source].[OtherNames] != [TestSessions].[OtherNames]))
		OR (([Source].[FamilyName] IS NOT NULL AND [TestSessions].[FamilyName] IS NULL) OR ([Source].[FamilyName] IS NULL AND [TestSessions].[FamilyName] IS NOT NULL) OR (([Source].[FamilyName] IS NOT NULL AND [TestSessions].[FamilyName] IS NOT NULL) AND [Source].[FamilyName] != [TestSessions].[FamilyName]))
		OR (([Source].[PrimaryAddress] IS NOT NULL AND [TestSessions].[PrimaryAddress] IS NULL) OR ([Source].[PrimaryAddress] IS NULL AND [TestSessions].[PrimaryAddress] IS NOT NULL) OR (([Source].[PrimaryAddress] IS NOT NULL AND [TestSessions].[PrimaryAddress] IS NOT NULL) AND [Source].[PrimaryAddress] != [TestSessions].[PrimaryAddress]))
		OR (([Source].[State] IS NOT NULL AND [TestSessions].[State] IS NULL) OR ([Source].[State] IS NULL AND [TestSessions].[State] IS NOT NULL) OR (([Source].[State] IS NOT NULL AND [TestSessions].[State] IS NOT NULL) AND [Source].[State] != [TestSessions].[State]))
		OR (([Source].[Postcode] IS NOT NULL AND [TestSessions].[Postcode] IS NULL) OR ([Source].[Postcode] IS NULL AND [TestSessions].[Postcode] IS NOT NULL) OR (([Source].[Postcode] IS NOT NULL AND [TestSessions].[Postcode] IS NOT NULL) AND [Source].[Postcode] != [TestSessions].[Postcode]))
		OR (([Source].[Country] IS NOT NULL AND [TestSessions].[Country] IS NULL) OR ([Source].[Country] IS NULL AND [TestSessions].[Country] IS NOT NULL) OR (([Source].[Country] IS NOT NULL AND [TestSessions].[Country] IS NOT NULL) AND [Source].[Country] != [TestSessions].[Country]))
		OR (([Source].[PrimaryPhone] IS NOT NULL AND [TestSessions].[PrimaryPhone] IS NULL) OR ([Source].[PrimaryPhone] IS NULL AND [TestSessions].[PrimaryPhone] IS NOT NULL) OR (([Source].[PrimaryPhone] IS NOT NULL AND [TestSessions].[PrimaryPhone] IS NOT NULL) AND [Source].[PrimaryPhone] != [TestSessions].[PrimaryPhone]))
		OR (([Source].[PrimaryEmail] IS NOT NULL AND [TestSessions].[PrimaryEmail] IS NULL) OR ([Source].[PrimaryEmail] IS NULL AND [TestSessions].[PrimaryEmail] IS NOT NULL) OR (([Source].[PrimaryEmail] IS NOT NULL AND [TestSessions].[PrimaryEmail] IS NOT NULL) AND [Source].[PrimaryEmail] != [TestSessions].[PrimaryEmail]))
		OR (([Source].[ApplicationId] IS NOT NULL AND [TestSessions].[ApplicationId] IS NULL) OR ([Source].[ApplicationId] IS NULL AND [TestSessions].[ApplicationId] IS NOT NULL) OR (([Source].[ApplicationId] IS NOT NULL AND [TestSessions].[ApplicationId] IS NOT NULL) AND [Source].[ApplicationId] != [TestSessions].[ApplicationId]))
		OR (([Source].[ApplicationReference] IS NOT NULL AND [TestSessions].[ApplicationReference] IS NULL) OR ([Source].[ApplicationReference] IS NULL AND [TestSessions].[ApplicationReference] IS NOT NULL) OR (([Source].[ApplicationReference] IS NOT NULL AND [TestSessions].[ApplicationReference] IS NOT NULL) AND [Source].[ApplicationReference] != [TestSessions].[ApplicationReference]))
		OR (([Source].[Certification] IS NOT NULL AND [TestSessions].[Certification] IS NULL) OR ([Source].[Certification] IS NULL AND [TestSessions].[Certification] IS NOT NULL) OR (([Source].[Certification] IS NOT NULL AND [TestSessions].[Certification] IS NOT NULL) AND [Source].[Certification] != [TestSessions].[Certification]))
		OR (([Source].[Language1] IS NOT NULL AND [TestSessions].[Language1] IS NULL) OR ([Source].[Language1] IS NULL AND [TestSessions].[Language1] IS NOT NULL) OR (([Source].[Language1] IS NOT NULL AND [TestSessions].[Language1] IS NOT NULL) AND [Source].[Language1] != [TestSessions].[Language1]))
		OR (([Source].[Language1Code] IS NOT NULL AND [TestSessions].[Language1Code] IS NULL) OR ([Source].[Language1Code] IS NULL AND [TestSessions].[Language1Code] IS NOT NULL) OR (([Source].[Language1Code] IS NOT NULL AND [TestSessions].[Language1Code] IS NOT NULL) AND [Source].[Language1Code] != [TestSessions].[Language1Code]))
		OR (([Source].[Language1Group] IS NOT NULL AND [TestSessions].[Language1Group] IS NULL) OR ([Source].[Language1Group] IS NULL AND [TestSessions].[Language1Group] IS NOT NULL) OR (([Source].[Language1Group] IS NOT NULL AND [TestSessions].[Language1Group] IS NOT NULL) AND [Source].[Language1Group] != [TestSessions].[Language1Group]))
		OR (([Source].[Language2] IS NOT NULL AND [TestSessions].[Language2] IS NULL) OR ([Source].[Language2] IS NULL AND [TestSessions].[Language2] IS NOT NULL) OR (([Source].[Language2] IS NOT NULL AND [TestSessions].[Language2] IS NOT NULL) AND [Source].[Language2] != [TestSessions].[Language2]))
		OR (([Source].[Language2Code] IS NOT NULL AND [TestSessions].[Language2Code] IS NULL) OR ([Source].[Language2Code] IS NULL AND [TestSessions].[Language2Code] IS NOT NULL) OR (([Source].[Language2Code] IS NOT NULL AND [TestSessions].[Language2Code] IS NOT NULL) AND [Source].[Language2Code] != [TestSessions].[Language2Code]))
		OR (([Source].[Language2Group] IS NOT NULL AND [TestSessions].[Language2Group] IS NULL) OR ([Source].[Language2Group] IS NULL AND [TestSessions].[Language2Group] IS NOT NULL) OR (([Source].[Language2Group] IS NOT NULL AND [TestSessions].[Language2Group] IS NOT NULL) AND [Source].[Language2Group] != [TestSessions].[Language2Group]))
		OR (([Source].[Skill] IS NOT NULL AND [TestSessions].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [TestSessions].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [TestSessions].[Skill] IS NOT NULL) AND [Source].[Skill] != [TestSessions].[Skill]))
		-- TFS 176518
		--OR (([Source].[Status] IS NOT NULL AND [TestSessions].[Status] IS NULL) OR ([Source].[Status] IS NULL AND [TestSessions].[Status] IS NOT NULL) OR (([Source].[Status] IS NOT NULL AND [TestSessions].[Status] IS NOT NULL) AND [Source].[Status] != [TestSessions].[Status]))
		OR (([Source].[StatusDateModified] IS NOT NULL AND [TestSessions].[StatusDateModified] IS NULL) OR ([Source].[StatusDateModified] IS NULL AND [TestSessions].[StatusDateModified] IS NOT NULL) OR (([Source].[StatusDateModified] IS NOT NULL AND [TestSessions].[StatusDateModified] IS NOT NULL) AND [Source].[StatusDateModified] != [TestSessions].[StatusDateModified]))
		OR (([Source].[StatusModifiedUser] IS NOT NULL AND [TestSessions].[StatusModifiedUser] IS NULL) OR ([Source].[StatusModifiedUser] IS NULL AND [TestSessions].[StatusModifiedUser] IS NOT NULL) OR (([Source].[StatusModifiedUser] IS NOT NULL AND [TestSessions].[StatusModifiedUser] IS NOT NULL) AND [Source].[StatusModifiedUser] != [TestSessions].[StatusModifiedUser]))
		OR (([Source].[Capacity] IS NOT NULL AND [TestSessions].[Capacity] IS NULL) OR ([Source].[Capacity] IS NULL AND [TestSessions].[Capacity] IS NOT NULL) OR (([Source].[Capacity] IS NOT NULL AND [TestSessions].[Capacity] IS NOT NULL) AND [Source].[Capacity] != [TestSessions].[Capacity]))
		OR (([Source].[VenueCapacityOverridden] IS NOT NULL AND [TestSessions].[VenueCapacityOverridden] IS NULL) OR ([Source].[VenueCapacityOverridden] IS NULL AND [TestSessions].[VenueCapacityOverridden] IS NOT NULL) OR (([Source].[VenueCapacityOverridden] IS NOT NULL AND [TestSessions].[VenueCapacityOverridden] IS NOT NULL) AND [Source].[VenueCapacityOverridden] != [TestSessions].[VenueCapacityOverridden]))

	    --select * from @TestSessionsHistory
				
		BEGIN TRANSACTION
		
	   --Merge operation delete
		MERGE TestSessionsHistory AS Target USING(select * from @TestSessionsHistory where [RowStatus] = 'Deleted' ) AS Source ON (Target.[TestSittingId] = Source.[TestSittingId] AND Target.[TestSessionId] = Source.[TestSessionId] AND Target.RowStatus = 'Latest')
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestSessionsHistory AS Target USING(	select * from @TestSessionsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.[TestSittingId] = Source.[TestSittingId] AND Target.[TestSessionId] = Source.[TestSessionId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestSessionsHistory AS Target USING(	select * from @TestSessionsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.[TestSittingId] = Source.[TestSittingId] AND Target.[TestSessionId] = Source.[TestSessionId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestSittingId = Source.TestSittingId
		  ,Target.TestSessionId = Source.TestSessionId
		  ,Target.CredentialRequestId = Source.CredentialRequestId
		  ,Target.TestSessionName = Source.TestSessionName
		  ,Target.TestLocationState = Source.TestLocationState
		  ,Target.TestLocationCountry = Source.TestLocationCountry      
		  ,Target.TestLocationName = Source.TestLocationName
		  ,Target.VenueName = Source.VenueName
		  ,Target.VenueAddress = Source.VenueAddress
		  ,Target.TestDate = Source.TestDate

		  ,Target.TestStartTime = Source.TestStartTime
		  ,Target.TestArrivalTime = Source.TestArrivalTime
		  ,Target.TestEndTime = Source.TestEndTime
		  ,Target.ApplicationType = Source.ApplicationType
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName
		  ,Target.TestSessionCompleted = Source.TestSessionCompleted
		  ,Target.CustomerNo = Source.CustomerNo
		  ,Target.Title = Source.Title
		  ,Target.GivenName = Source.GivenName
		  ,Target.OtherNames = Source.OtherNames
		  ,Target.FamilyName = Source.FamilyName
		  ,Target.PrimaryAddress = Source.PrimaryAddress
		  ,Target.State = Source.State
		  ,Target.Postcode = Source.Postcode
		  ,Target.Country = Source.Country

		  ,Target.PrimaryPhone = Source.PrimaryPhone
		  ,Target.PrimaryEmail = Source.PrimaryEmail
		  ,Target.ApplicationId = Source.ApplicationId
		  ,Target.ApplicationReference = Source.ApplicationReference
		  ,Target.Certification = Source.Certification
		  ,Target.Language1 = Source.Language1
		  ,Target.Language1Code = Source.Language1Code
		  ,Target.Language1Group = Source.Language1Group
		  ,Target.Language2 = Source.Language2
		  ,Target.Language2Code = Source.Language2Code

		  ,Target.Language2Group = Source.Language2Group
		  ,Target.Skill = Source.Skill
		  ,Target.Status = Source.Status
		  ,Target.StatusDateModified = Source.StatusDateModified
		  ,Target.StatusModifiedUser = Source.StatusModifiedUser		  
		  ,Target.Capacity = Source.Capacity

		  ,Target.VenueCapacityOverridden = Source.VenueCapacityOverridden
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 

		 INSERT([TestSittingId],[TestSessionId],[CredentialRequestId],[TestSessionName],[TestLocationState],[TestLocationCountry],[TestLocationName],[VenueName],[VenueAddress],[TestDate],[TestStartTime],[TestArrivalTime],[TestEndTime],
			 [ApplicationType],[CredentialTypeInternalName],[CredentialTypeExternalName],[TestSessionCompleted],[PersonId],[CustomerNo],[Title],[GivenName],[OtherNames],[FamilyName],[PrimaryAddress],[State],[Postcode],[Country],
			 [PrimaryPhone],[PrimaryEmail],[ApplicationId],[ApplicationReference],[Certification],[Language1],[Language1Code],[Language1Group],[Language2],[Language2Code],[Language2Group],[Skill],[Status],[StatusDateModified],
			 [StatusModifiedUser],[ModifiedDate],[Capacity],[VenueCapacityOverridden],[RowStatus])
	  	  
		VALUES (Source.[TestSittingId],Source.[TestSessionId],Source.[CredentialRequestId],Source.[TestSessionName],Source.[TestLocationState],Source.[TestLocationCountry],Source.[TestLocationName],Source.[VenueName],Source.[VenueAddress],Source.[TestDate],
			  Source.[TestStartTime],Source.[TestArrivalTime],Source.[TestEndTime],Source.[ApplicationType],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],Source.[TestSessionCompleted],Source.[PersonId],Source.[CustomerNo],Source.[Title],
			  Source.[GivenName],Source.[OtherNames],Source.[FamilyName],Source.[PrimaryAddress],Source.[State],Source.[Postcode],Source.[Country],Source.[PrimaryPhone],Source.[PrimaryEmail],Source.[ApplicationId],Source.[ApplicationReference],
			  Source.[Certification],Source.[Language1],Source.[Language1Code],Source.[Language1Group],Source.[Language2],Source.[Language2Code],Source.[Language2Group],Source.[Skill],Source.[Status],Source.[StatusDateModified],
			  Source.[StatusModifiedUser],@Date,Source.[Capacity],Source.[VenueCapacityOverridden],'Latest');

	COMMIT TRANSACTION;	

END