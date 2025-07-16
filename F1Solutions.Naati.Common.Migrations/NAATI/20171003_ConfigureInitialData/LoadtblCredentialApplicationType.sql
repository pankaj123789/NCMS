SET IDENTITY_INSERT tblcredentialapplicationtype ON 

INSERT INTO tblCredentialApplicationType 
            (CredentialApplicationTypeId, [Name], DisplayName, [Online], BackOffice, RequiresChecking,  
             RequiresAssessment, PendingAllowed, AssessmentReviewAllowed) 
VALUES      (1, 'Transition', 'Transition', 0, 1, 1, 1, 1, 0) 

SET IDENTITY_INSERT tblcredentialapplicationtype OFF 