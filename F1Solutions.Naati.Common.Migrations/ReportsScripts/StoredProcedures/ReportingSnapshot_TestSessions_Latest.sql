ALTER PROCEDURE [dbo].[ReportingSnapshot_TestSessions_Latest]
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'TestSessionsLatest')
		BEGIN
			SELECT * INTO TestSessionsLatest FROM TestSessionsHistory WHERE RowStatus = 'Latest'
		END
	ELSE
		BEGIN		
		BEGIN TRANSACTION
		
		--Merge operation Latest
		MERGE TestSessionsLatest AS Target USING(select * from TestSessionsHistory where [RowStatus] = 'Latest' ) AS Source ON (Target.[TestSittingId] = Source.[TestSittingId] AND Target.[TestSessionId] = Source.[TestSessionId] AND Target.RowStatus = 'Latest')
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
		   --,Target.ModifiedDate = GetDate() for updates leave as is
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
			  Source.[StatusModifiedUser],GetDate(),Source.[Capacity],Source.[VenueCapacityOverridden],'Latest');

	COMMIT TRANSACTION;	
	END

END