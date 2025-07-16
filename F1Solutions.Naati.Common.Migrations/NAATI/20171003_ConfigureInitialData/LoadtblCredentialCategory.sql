SET IDENTITY_INSERT tblcredentialcategory ON 

INSERT INTO tblCredentialCategory 
            (CredentialCategoryId, [Name], DisplayName) 
VALUES      (1, 'Translator', 'Translator'),  
            (2, 'Interpreter', 'Interpreter'),  
            (3, 'DeafInterpreter', 'Deaf Interpreter'),  
            (4, 'CCL', 'Credentialed Community Language'),  
            (5, 'CLA', 'Community Language Aide'),  
            (6, 'Ethics', 'Ethics'),  
            (7, 'Intercultural', 'Intercultural'),  
            (8, 'Migration', 'Migration Assessment') 

SET IDENTITY_INSERT tblcredentialcategory OFF 