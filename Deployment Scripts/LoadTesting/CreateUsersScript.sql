
CREATE PROCEDURE CreateTestUser
@FirstName AS VARCHAR(100),
@LastName AS VARCHAR(100),
@EnteredUser AS int
AS

DECLARE @Email AS VARCHAR(100) = ''
DECLARE @UserId AS UNIQUEIDENTIFIER 
DECLARE @EntityId AS INT
DECLARE @PersonId AS INT
DECLARE @NaatiNumber AS VARCHAR(100)
DECLARE @CredentialApplicationId AS INT

SELECT @Email = @FirstName + '.' + @LastName + '@altf4solutions.com.au'
SELECT @UserId = NEWID()
SELECT @NaatiNumber = (max(naatinumber))+1 from tblentity

PRINT 'EMAIL:' + @Email
PRINT 'USERID:'
PRINT @UserId
PRINT 'NAATINUMBER:' + @NaatiNumber

INSERT INTO [dbo].[aspnet_Users]([ApplicationId],[UserId],[UserName],[LoweredUserName],[MobileAlias],[IsAnonymous],[LastActivityDate])
     VALUES('A4B7C679-ED79-491E-841D-34A65938D621',@USERID,@Email,@Email
           ,null,0,GetDate())

INSERT INTO [dbo].[aspnet_Membership]([ApplicationId],[UserId],[Password],[PasswordFormat],[PasswordSalt]
           ,[MobilePIN],[Email],[LoweredEmail],[PasswordQuestion],[PasswordAnswer],[IsApproved],[IsLockedOut],[CreateDate]
           ,[LastLoginDate],[LastPasswordChangedDate],[LastLockoutDate],[FailedPasswordAttemptCount],[FailedPasswordAttemptWindowStart]
           ,[FailedPasswordAnswerAttemptCount],[FailedPasswordAnswerAttemptWindowStart],[Comment])
     VALUES
           ('A4B7C679-ED79-491E-841D-34A65938D621',@UserId,'K+WpEfxsPWYvOvwSYTNv5PBbKGk=',1,'JWlGohQQQWW7P8HYjI+09w=='
           ,null,@Email,@Email,'KrcC0@BkXA',null
           ,1,0,GetDate(),GetDate(),GetDate(),GetDate()
           ,0,GetDate(),0
           ,GetDate(),null)

INSERT INTO [dbo].[aspnet_UsersInRoles]([UserId],[RoleId])
VALUES( @UserId,'1CB569E8-173A-4429-9037-F80E08D81804')

INSERT INTO [dbo].[tblEntity]([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId],[AccountNumber])
VALUES ('http://me.com','','',0,0,0,@NaatiNumber,1,null)

SELECT @EntityId = EntityId FROM tblEntity WHERE [NAATINumber] = @NaatiNumber
PRINT 'EntityId'
PRINT @EntityId

INSERT INTO [dbo].[tblMyNaatiUser]([AspUserId],[NaatiNumber])
VALUES( @UserId,@NaatiNumber)


INSERT INTO [dbo].[tblPerson]([EntityId],[Gender],[BirthDate],[BirthCountryId],[SponsorInstitutionId],[Deceased],[HighestEducationLevelId]
           ,[ReleaseDetails],[DoNotInviteToDirectory],[EnteredDate],[ExpertiseFreeText],[NameOnAccreditationProduct],[DoNotSendCorrespondence]
           ,[ScanRequired],[IsEportalActive],[PersonalDetailsLastUpdatedOnEportal],[WebAccountCreateDate],[AllowVerifyOnline],[ShowPhotoOnline]
           ,[RevalidationScheme],[ExaminerSecurityCode],[ExaminerTrackingCategory],[PractitionerNumber],[EthicalCompetency],[InterculturalCompetency]
           ,[AllowAutoRecertification],[KnowledgeTest])
     VALUES
           (@EntityId,'M','1968-09-28 00:00:00.000',105,null,0,null,1,0,GETDATE(),'','',0,0,1,'2020-11-09 14:15:23.130','2020-11-09 14:15:23.130',1,1,null,null
           ,null,'CPN3XF40U',0,0,1,0)

SELECT @PersonId = PersonId FROM [tblPerson] WHERE [EntityId] = @EntityId
PRINT 'PersonId'
PRINT @PersonId

INSERT INTO [dbo].[tblPersonName]([PersonId],[GivenName],[OtherNames],[Surname],[TitleId],[EffectiveDate],[AlternativeGivenName],[AlternativeSurname])
     VALUES(@PersonId,@FirstName,'',@LastName,7,'1990-01-01 00:00:00.000','','')

INSERT INTO [dbo].[tblEmail]([EntityId],[Email],[Note],[IncludeInPD],[IsPreferredEmail],[Invalid],[ExaminerCorrespondence])
     VALUES
           (@EntityId,@Email,'',0,1,0,0)

INSERT INTO [dbo].[tblAddress]([EntityId],[StreetDetails],[PostcodeId],[CountryId],[Note],[StartDate],[EndDate],[PrimaryContact],[Invalid],[SubscriptionExpiryDate],[SubscriptionRenewSentDate],[ContactPerson],[ValidateInExternalTool],[ExaminerCorrespondence],[ODAddressVisibilityTypeId])
     VALUES(@EntityId,'45 Bennett Road',4601,13,'','2017-10-09 09:48:09.000',null,1,0,null,null,null,0,0,1)

	 INSERT INTO [dbo].[tblPhone]([EntityId],[CountryCode],[AreaCode],[LocalNumber],[Note],[IncludeInPD],[AllowSmsNotification],[Invalid],[PrimaryContact],[ExaminerCorrespondence])
     VALUES(@EntityId,'','',62457999,'',0,0,0,1,0)

	 --266313
	 INSERT INTO [dbo].[tblCredentialApplication]([CredentialApplicationTypeId],[CredentialApplicationStatusTypeId],[EnteredDate],[PersonId],[SponsorInstitutionId],[EnteredUserId],[ReceivingOfficeId]
           ,[StatusChangeDate],[StatusChangeUserId],[OwnedByUserId],[OwnedByApplicant],[PreferredTestLocationId],[SponsorInstitutionContactPersonId])
     VALUES
           (13,2,GetDate(),@PersonId,null,@EnteredUser,1,GetDate()
           ,@EnteredUser,null,1,13,null)

SELECT @CredentialApplicationId = CredentialApplicationId from [tblCredentialApplication] WHERE PersonId = @PersonId

INSERT INTO [dbo].[tblCredentialRequest]([CredentialApplicationId],[CredentialTypeId],[SkillId],[CredentialRequestStatusTypeId],[StatusChangeDate],[StatusChangeUserId],[CredentialRequestPathTypeId],[Supplementary])
VALUES (@CredentialApplicationId,14,369,10,GetDate(),@EnteredUser,2,0) -- status is moved to 10 straight away as Eligible For Testing

--this creates the test, which is unrelated other than we need one for every user

INSERT INTO [dbo].[tblTestSession]([VenueId],[Name],[TestDateTime],[ArrivalTime],[Duration],[CredentialTypeId],[Completed],[PublicNote],[AllowSelfAssign],[OverrideVenueCapacity],[Capacity],[RehearsalDateTime],[RehearsalNotes],[DefaultTestSpecificationId],[AllowAvailabilityNotice])
VALUES (1035,(@FirstName + @LastName),(Getdate()+14),null,60,14,0,null,1,0,null,null,null,1,0)

GO

