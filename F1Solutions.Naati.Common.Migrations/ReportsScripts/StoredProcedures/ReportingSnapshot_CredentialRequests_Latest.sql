ALTER PROCEDURE [dbo].[ReportingSnapshot_CredentialRequests_Latest]

AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'CredentialRequestsLatest')
		BEGIN
			SELECT * INTO CredentialRequestsLatest FROM CredentialRequestsHistory WHERE RowStatus = 'Latest'
		END
	ELSE
		BEGIN		
			BEGIN TRANSACTION
			
			--Merge operation Latest
			MERGE CredentialRequestsLatest AS Target USING(select * from CredentialRequestsHistory where [RowStatus] = 'Latest' ) AS Source ON Target.CredentialRequestId = Source.CredentialRequestId AND Target.RowStatus = 'Latest'
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
				  Source.[ApplicationOwner],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],Source.[Certification],Source.[Language1],Source.[Language2],Source.[DirectionDisplayName],GetDate() ,Source.[StatusDateModified],Source.[StatusModifiedUser],Source.[Status],
				  Source.[NAATINumber],Source.[PractitionerNumber],Source.[Language1Code],Source.[Language1Group],Source.[Language2Code],Source.[Language2Group],Source.[State],Source.[Postcode],Source.[CredentialId],
				  Source.[LinkedCredentialRequestId],Source.[LinkedCredentialRequestReason], 'Latest');	 
			  		  	
			COMMIT TRANSACTION;
	END
END