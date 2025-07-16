USE $(ncms)

IF OBJECT_ID('tempdb..$(temptbl)') IS NOT NULL DROP TABLE $(temptbl)
GO

CREATE TABLE $(temptbl) 
  ( 
     Id          INT, 
     NaatiNumber INT, 
     Expiry      DATE 
  ) 

INSERT INTO $(temptbl) VALUES 
(2, 75201, '2019/03/27'), 
(3, 87451, '2019/03/12'), 
(4, 6525, '2019/01/08'), 
(5, 8467, '2019/03/27'), 
(6, 14274, '2019/01/23'), 
(7, 78746, '2019/01/23'), 
(8, 16250, '2019/01/08'), 
(9, 90068, '2019/03/23'), 
(10, 69508, '2019/02/16'), 
(11, 79188, '2019/02/28'), 
(12, 79278, '2019/03/15'), 
(13, 95382, '2019/02/17'), 
(14, 21199, '2019/01/09'), 
(15, 25708, '2019/03/31'), 
(16, 63930, '2019/01/31'), 
(17, 69647, '2019/01/14'), 
--(18, 29917, '2019/03/27'), 
(19, 75333, '2019/02/09'), 
(20, 88439, '2019/03/13'), 
(21, 69295, '2019/01/14'), 
(22, 65337, '2019/02/14'), 
(23, 59821, '2019/01/25'), 
(24, 75269, '2019/02/28'), 
(25, 96371, '2019/03/07'), 
(26, 79805, '2019/01/17'), 
(27, 96374, '2019/03/29'), 
(28, 74612, '2019/03/09'), 
(29, 76732, '2019/01/10'), 
(30, 66303, '2019/02/17'), 
(31, 55411, '2019/03/10'), 
(32, 80255, '2019/02/07'), 
(33, 34212, '2019/03/27'), 
(34, 95431, '2019/02/17'), 
(35, 69888, '2019/01/27'), 
(36, 64710, '2019/01/16'), 
(37, 83267, '2019/03/10'), 
(38, 67704, '2019/02/12'), 
(39, 96239, '2019/03/07'), 
(40, 67354, '2019/03/27'), 
(41, 66572, '2019/02/06'), 
(42, 74388, '2019/03/10'), 
(43, 68293, '2019/03/23'), 
(44, 82215, '2019/02/17'), 
(45, 2773, '2019/02/13'), 
(46, 30181, '2019/03/31'), 
(47, 34533, '2019/03/31'), 
(48, 35344, '2019/02/14'), 
(49, 69343, '2019/03/04'), 
(50, 71735, '2019/03/30')


DECLARE @deleteList TABLE 
  ( 
     NaatiNumber INT 
  ) 

INSERT INTO @deleteList 
SELECT e.NaatiNumber 
FROM   tblCertificationPeriod cp 
       JOIN tblCredential c 
         ON c.CertificationPeriodId = cp.CertificationPeriodId 
       JOIN tblCredentialCredentialRequest ccr 
         ON ccr.CredentialId = c.CredentialId 
       JOIN tblCredentialRequest cr 
         ON cr.CredentialRequestId = ccr.CredentialRequestId 
       JOIN tblCredentialApplication ca 
         ON ca.CredentialApplicationId = cr.CredentialApplicationId 
       JOIN tblPerson p 
         ON p.PersonId = ca.PersonId 
       JOIN tblEntity e 
         ON e.EntityId = p.EntityId 
	  LEFT JOIN tblPhone ph
		ON ph.EntityId = e.EntityId AND ph.Invalid = 0 
	 LEFT JOIN tblAddress addr
		ON addr.EntityId = e.EntityId AND addr.Invalid = 0 
	 LEFT JOIN tblEmail email
		ON email.EntityId = e.EntityId AND email.Invalid = 0 
WHERE  e.NAATINumber IN (SELECT NaatiNumber 
                         FROM   $(temptbl)) 
GROUP  BY e.NaatiNumber 
HAVING Count(DISTINCT( cp.CertificationPeriodId )) > 1 OR Count(DISTINCT( addr.AddressId )) <> 1 OR Count(DISTINCT( email.EmailId )) <> 1  OR Count(DISTINCT( ph.PhoneId )) <> 1 

-- delete people with more than one CP  OR PHONE, EMAIL OR ADDRESS <> 1
DELETE FROM $(temptbl) 
WHERE  NaatiNumber IN (SELECT * 
                       FROM   @deleteList) 

DECLARE @loop  INT = 31, 
        @maxId INT = 32, 
        @nn    INT, 
        @cp    INT, 
        @exp   DATE, 
        @email NVARCHAR(50),
		@userId UNIQUEIDENTIFIER 

SELECT @loop = Min(Id) 
FROM   $(temptbl) 

SELECT @maxId = Max(Id) 
FROM   $(temptbl) 

SET xact_abort ON 

BEGIN TRAN 

WHILE( @loop <= @maxId ) 
  BEGIN 
	  set @nn = null

      SELECT @nn = NaatiNumber, 
             @exp = Expiry 
      FROM   $(temptbl) 
      WHERE  Id = @loop 

      SET @loop = @loop + 1 

      IF @nn IS NULL 
        CONTINUE 

      PRINT 'NAATI #' + CONVERT(VARCHAR(6), @nn) 

      USE $(ncms) 

      SELECT @cp = cp.CertificationPeriodId 
      FROM   tblCertificationPeriod cp 
             JOIN tblCredential c 
               ON c.CertificationPeriodId = cp.CertificationPeriodId 
             JOIN tblCredentialCredentialRequest ccr 
               ON ccr.CredentialId = c.CredentialId 
             JOIN tblCredentialRequest cr 
               ON cr.CredentialRequestId = ccr.CredentialRequestId 
             JOIN tblCredentialApplication ca 
               ON ca.CredentialApplicationId = cr.CredentialApplicationId 
             JOIN tblPerson p 
               ON p.PersonId = ca.PersonId 
             JOIN tblEntity e 
               ON e.EntityId = p.EntityId 
      WHERE  e.NAATINumber = @nn 

      -- change the end dates of the CP  
      UPDATE tblCertificationPeriod 
      SET    EndDate = Dateadd(month, -3, @exp), 
             OriginalEndDate = Dateadd(month, -3, @exp) 
      WHERE  CertificationPeriodId = @cp 



      -- add PDs 
      DECLARE @personId INT 

      SELECT @personId = PersonId 
      FROM   tblPerson p 
             JOIN tblEntity e 
               ON e.EntityId = p.EntityId 
      WHERE  NaatiNumber = @nn 

      PRINT 'person: ' + CONVERT(VARCHAR(6), @personID) 


      -- delete all work practice for this person 
      DELETE FROM tblWorkPracticeAttachment 
      WHERE  WorkPracticeId IN (SELECT WorkPracticeId 
                                FROM   tblWorkPractice wp 
                                       JOIN tblCredential c 
                                         ON c.CredentialId = wp.CredentialId 
                                WHERE  c.CertificationPeriodId = @cp); 

      DELETE FROM tblWorkPracticeCredentialRequest
      WHERE  WorkPracticeId IN (SELECT WorkPracticeId 
                                FROM   tblWorkPractice wp 
                                       JOIN tblCredential c 
                                         ON c.CredentialId = wp.CredentialId 
                                WHERE  c.CertificationPeriodId = @cp); 

      DELETE FROM tblWorkPractice 
      WHERE  WorkPracticeId IN (SELECT WorkPracticeId 
                                FROM   tblWorkPractice wp 
                                       JOIN tblCredential c 
                                         ON c.CredentialId = wp.CredentialId 
                                WHERE  c.CertificationPeriodId = @cp); 

--      SET @email = 'LoadTest' + CONVERT(VARCHAR(6), @nn) + '@altf4solutions.com.au' 
      SET @email = Concat('Automation.Test.', @loop, '@altf4solutions.com.au') 
                   

      UPDATE tblEmail 
      SET    Email = @email 
      WHERE  IsPreferredEmail = 1 
             AND EntityId = (SELECT EntityId 
                             FROM   tblEntity 
                             WHERE  NAATINumber = @nn) 

	  -- Update the country and gender
	  update [tblPerson] set BirthCountryId = 13, Gender = 'M' where PersonId = @personId

	  -- update the title
	  update [tblPersonName] set TitleId = 2 where PersonId = @personId
	  

      USE $(mynaati) 
	  
	  SET @userId = null

      SELECT @userId = AspUserId 
      FROM   [User] 
      WHERE  NaatiNumber = @nn 

      IF @userId IS NULL 
        BEGIN 
			PRINT 'Adding MyNAATI user ' + CONVERT(varchar(6), @nn)
            SET @userId = Newid() 

            INSERT INTO [aspnet_Users] 
                        ([ApplicationId], 
                         [UserId], 
                         [UserName], 
                         [LoweredUserName], 
                         [MobileAlias], 
                         [IsAnonymous], 
                         [LastActivityDate]) 
            VALUES      ('a4b7c679-ed79-491e-841d-34a65938d621', 
                         @userId, 
                         @email, 
                         Lower(@email), 
                         NULL, 
                         0, 
                         Getdate()) 

            INSERT INTO [dbo].[aspnet_Membership] 
                        ([ApplicationId], 
                         [UserId], 
                         [Password], 
                         [PasswordFormat], 
                         [PasswordSalt], 
                         [Email], 
                         [LoweredEmail], 
                         [IsApproved], 
                         [IsLockedOut], 
                         [CreateDate], 
                         [LastLoginDate], 
                         [LastPasswordChangedDate], 
                         [LastLockoutDate], 
                         [FailedPasswordAttemptCount], 
                         [FailedPasswordAttemptWindowStart], 
                         [FailedPasswordAnswerAttemptCount], 
                         [FailedPasswordAnswerAttemptWindowStart], 
                         [Comment]) 
            VALUES      ('a4b7c679-ed79-491e-841d-34a65938d621', 
                         @userId, 
                         'pGVsAKEucmNL5Pq0A0YHGL2P++c=', 
                         1, 
                         'fqJfdD0PYadaodviKdmt+w==', 
                         @email, 
                         Lower(@email), 
                         1, 
                         0, 
                         Getdate(), 
                         Cast('' AS DATETIME), 
                         Getdate(), 
                         Cast('' AS DATETIME), 
                         0, 
                         Cast('' AS DATETIME), 
                         0, 
                         Cast('' AS DATETIME), 
                         'Created by deploy script') 

            INSERT INTO [dbo].[User] 
            VALUES      (@userId, 
                         @nn) 

			INSERT INTO [PasswordHistory] 
						([UserId], 
						 [Password], 
						 [CreatedDateTime]) 
			VALUES      (@userId, 
						 '7VfCzbor26KSVeFs9+t6XHprcaQY+CIrigyDpS1B9ZU=', 
						 Getdate()) 
        END 
      ELSE 
        BEGIN 
			PRINT 'MyNAATI user ' + CONVERT(varchar(6), @nn) + ' already exists'

            UPDATE aspnet_Users 
            SET    UserName = @email, 
                   LoweredUserName = Lower(@email) 
            WHERE  UserId = @userId 

            UPDATE aspnet_membership 
            SET    Email = @email, 
                   LoweredEmail = Lower(@email), 
                   Password = 'pGVsAKEucmNL5Pq0A0YHGL2P++c=', 
                   PasswordSalt = 'fqJfdD0PYadaodviKdmt+w==', 
                   IsLockedOut = 0 
            WHERE  UserId = @userId 
        END 
  END 

COMMIT TRAN 
