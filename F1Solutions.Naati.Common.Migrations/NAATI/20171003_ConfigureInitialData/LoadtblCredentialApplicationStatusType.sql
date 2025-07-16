SET IDENTITY_INSERT tblcredentialapplicationstatustype ON 

INSERT INTO tblCredentialApplicationStatusType 
			(CredentialApplicationStatusTypeId,  [Name],  DisplayName) 
VALUES      (1, 'Draft', 'Draft'),  
            (2, 'Entered', 'Entered'),  
            (3, 'BeingChecked', 'Being Checked'),  
            (4, 'Rejected', 'Rejected'),  
            (5, 'InProgress', 'In Progress'),  
            (6, 'Completed', 'Completed') 

SET IDENTITY_INSERT tblcredentialapplicationstatustype OFF 