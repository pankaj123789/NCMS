ALTER PROCEDURE [dbo].[ReportingSnapshot_Application_Latest]
AS
BEGIN

	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'ApplicationLatest')
		BEGIN
			SELECT * INTO ApplicationLatest FROM ApplicationHistory WHERE RowStatus = 'Latest'
		END
	ELSE
		BEGIN
			BEGIN TRANSACTION 	

				--Merge operation Latest
				MERGE ApplicationLatest AS Target USING(select * from ApplicationHistory where [RowStatus] = 'Latest' ) AS Source ON Target.[ApplicationId] = Source.[ApplicationId] AND Target.RowStatus = 'Latest'
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
				  --,Target.ModifiedDate = GetDate() for updates leave as is
				  ,Target.[RowStatus] = 'Latest'
				WHEN NOT MATCHED THEN 
				 INSERT([ApplicationId],[ModifiedDate],[PersonId],[EnteredDate],[EnteredOffice],[ApplicationStatus],[StatusDateModified],[Sponsor],[Title],[GivenName],[OtherNames],[FamilyName],
					 [PrimaryAddress],[Country],[PrimaryPhone],[PrimaryEmail],[ApplicationType],[ApplicationReference],[ApplicationOwner],[StatusModifiedUser],[NAATINumber],[PractitionerNumber],[EnteredUser],
					 [SponsoredOrganisationId],[SponsoredOrganisationName],[SponsoredContactId],[SponsoredContactName],[PreferredTestLocationState],[PreferredTestLocationCity],[State],[Postcode],[RowStatus]) 

				 VALUES (Source.[ApplicationId],GetDate(),Source.[PersonId],Source.[EnteredDate],Source.[EnteredOffice],Source.[ApplicationStatus],Source.[StatusDateModified],Source.[Sponsor],Source.[Title],Source.[GivenName],Source.[OtherNames],Source.[FamilyName],
					  Source.[PrimaryAddress],Source.[Country],Source.[PrimaryPhone],Source.[PrimaryEmail],Source.[ApplicationType],Source.[ApplicationReference],Source.[ApplicationOwner],Source.[StatusModifiedUser],Source.[NAATINumber],Source.[PractitionerNumber],Source.[EnteredUser],
					  Source.[SponsoredOrganisationId],Source.[SponsoredOrganisationName],Source.[SponsoredContactId],Source.[SponsoredContactName],Source.[PreferredTestLocationState],Source.[PreferredTestLocationCity],Source.[State],Source.[Postcode], 'Latest');

				COMMIT TRANSACTION;

		END	
END

