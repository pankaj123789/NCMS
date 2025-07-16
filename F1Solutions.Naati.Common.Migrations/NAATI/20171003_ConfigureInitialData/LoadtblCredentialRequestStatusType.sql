SET IDENTITY_INSERT tblcredentialrequeststatustype ON 

INSERT INTO tblCredentialRequestStatusType 
            (CredentialRequestStatusTypeId, [Name], DisplayName) 
VALUES      (1, 'Draft', 'Draft'),  
            (2, 'Rejected', 'Application Rejected'),  
            (3, 'RequestEntered', 'Request Entered'),  
            (4, 'ReadyForAssessment', 'Ready for Assessment'),  
            (5, 'BeingAssessed', 'Being Assessed'),  
            (6, 'Pending', 'Pending'),  
            (7, 'AssessmentFailed', 'Assessment Failed'),  
            (8, 'AssessmentPaidReview', 'Assessment Paid Review'),  
            (9, 'AssessmentComplete', 'Assessment Complete'),  
            (10, 'InProgress', 'In Progress'),  
            (11, 'TestFailed', 'Test Failed'),  
            (12, 'CertificationIssued', 'Certification Issued'),  
            (13, 'Cancelled', 'Cancelled'),  
            (14, 'Deleted', 'Deleted') 

SET IDENTITY_INSERT tblcredentialrequeststatustype OFF 