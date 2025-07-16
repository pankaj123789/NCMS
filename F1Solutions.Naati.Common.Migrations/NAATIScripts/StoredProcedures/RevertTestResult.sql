CREATE PROCEDURE dbo.RevertTestResult @attendanceId INT 
AS 
    DECLARE @appType		  INT,
			@testStatus		  INT,
			@oldStatus        VARCHAR(50), 
            @newStatus        VARCHAR(50), 
            @newStatusDisplay VARCHAR(50), 
            @crId             INT, 
            @credId           INT, 
            @skillName        VARCHAR(100), 
            @appId            INT, 
            @note             VARCHAR(100) = '' 

    SET xact_abort ON 

	SELECT @appType = CredentialApplicationTypeId 
	FROM tblTestSitting s
		 JOIN tblCredentialRequest c 
		   ON c.CredentialRequestId = s.CredentialRequestId
		 JOIN tblCredentialApplication a 
		   ON a.CredentialApplicationId = c.CredentialApplicationId
	WHERE TestSittingId = @attendanceId

  SELECT @crId = CredentialRequestId 
  FROM   tblTestSitting 
  WHERE  TestSittingId = @attendanceId 

  SELECT @oldStatus = NAME     FROM   tblCredentialRequestStatusType 
  WHERE  CredentialRequestStatusTypeId = (SELECT CredentialRequestStatusTypeId 
                                          FROM   tblCredentialRequest 
                                          WHERE  CredentialRequestId = @crId) 

  IF @appType IN (2, 6, 10) AND @oldStatus <> 'TestFailed'
		  THROW 50000, 'This procedure can only revert Certification tests from Failed status', 0
  	
	SELECT @testStatus = TestStatusTypeId 
	FROM vwTestStatus
	WHERE TestSittingId = @attendanceId

	IF NOT @testStatus = 5
		THROW 50001, 'Test result cannot be reverted because Test is not finalised', 0
    
	BEGIN TRAN 
    -- get the status to revert the CR to 
    SELECT @newStatus = CASE HasPaidReviewExaminers 
                          WHEN 1 THEN 'UnderPaidReview' 
                          ELSE 'TestSat' 
                        END, 
           @newStatusDisplay = CASE HasPaidReviewExaminers 
                                 WHEN 1 THEN 'Under Paid Review' 
                                 ELSE 'Test Sat' 
                               END 
    FROM   vwTestStatus 
    WHERE  TestSittingId = @attendanceId 

    -- revert the CR status 
    UPDATE tblCredentialRequest 
    SET    CredentialRequestStatusTypeId = (SELECT CredentialRequestStatusTypeId 
                                            FROM tblCredentialRequestStatusType 
                                            WHERE [Name] = @newStatus) 
	OUTPUT inserted.CredentialRequestId 'CredentialRequest updated', @newStatus 'New status'
    WHERE  CredentialRequestId = @crId 

    -- revert the app status 
    UPDATE tblCredentialApplication 
    SET    CredentialApplicationStatusTypeId = 5 
	OUTPUT inserted.CredentialApplicationId 'CredentialApplication updated', 'InProgress' 'New status'
    WHERE  CredentialApplicationId = (SELECT CredentialApplicationId 
                                      FROM   tblCredentialRequest 
                                      WHERE  CredentialRequestId = @crId) 

    SELECT @credId = CredentialId 
    FROM   tblCredentialCredentialRequest 
    WHERE  CredentialRequestId = @crId 

	-- delete the credential if it was issued
    IF @credId IS NOT NULL 
      BEGIN 
		  -- delete attachments (note, the stored file itself remains and will still be referenced by emails)
          DELETE FROM tblCredentialAttachment 
          OUTPUT deleted.CredentialAttachmentId 'CredentialAttachment deleted' 
          WHERE  CredentialId = @credId 

          DELETE FROM tblCredentialCredentialRequest 
          OUTPUT deleted.CredentialCredentialRequestId 'CredentialCredentialRequest deleted' 
          WHERE  CredentialId = @credId 

		  -- delete credential
		  DELETE FROM tblCredential
          OUTPUT deleted.CredentialId 'Credential deleted' 
          WHERE  CredentialId = @credId 

          SET @note = ' The credential was deleted.' 
      END 

    -- create an application note 
    SELECT @skillName = ct.InternalName + ' ' + v.DisplayName 
    FROM   vwSkillDisplayName v 
           JOIN tblCredentialRequest cr 
             ON v.SkillId = cr.SkillId 
           JOIN tblCredentialType ct 
             ON ct.CredentialTypeId = cr.CredentialTypeId 
    WHERE  CredentialRequestId = @crId 

    SELECT @appId = CredentialApplicationId 
    FROM   tblCredentialRequest 
    WHERE  CredentialRequestId = @crId 

    INSERT INTO tblNote 
    OUTPUT      inserted.NoteId 'Note inserted' 
    VALUES      (40, concat('The status of credential request ', @skillName, ' was reverted to ', @newStatusDisplay, ' as requested by NAATI.', @note), 
                 getdate(), NULL, 0, 1) 

    INSERT INTO tblCredentialApplicationNote 
    OUTPUT      inserted.CredentialApplicationNoteId 'CredentialApplicationNote inserted' 
    VALUES      (@appId, scope_identity()) 

    COMMIT
