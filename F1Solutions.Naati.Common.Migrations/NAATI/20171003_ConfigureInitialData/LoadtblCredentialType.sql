SET IDENTITY_INSERT tblcredentialtype ON 

INSERT INTO tblCredentialType 
            (CredentialTypeId, CredentialCategoryId, InternalName, ExternalName, UpgradePath, Simultaneous, SkillTypeId, Certification)
VALUES      (1, 1, 'Recognised Practising Translator', 'Recognised Practising Translator', 100, 0, 1, 1), 
            (2, 1, 'Certified Translator', 'Certified Translator', 200, 0, 2, 1),  
            (3, 1, 'Certified Advanced Translator', 'Certified Advanced Translator', 300, 1, 3, 1),  
            (4, 1, 'Certified Advanced Translator LOTE to LOTE', 'Certified Advanced Translator', 300, 1, 4, 1), 
            (5, 2, 'Recognised Practising Interpreter', 'Recognised Practising Interpreter', 100, 0, 5, 1), 
            (6, 2, 'Certified Provisional Interpreter', 'Certified Provisional Interpreter', 200, 0, 6, 1), 
            (7, 2, 'Certified Interpreter', 'Certified Interpreter', 300, 1, 7, 1),  
            (8, 2, 'Certified Specialist Interpreter - Health', 'Certified Specialist Interpreter - Health', 400, 1, 9, 1), 
            (9, 2, 'Certified Specialist Interpreter - Legal', 'Certified Specialist Interpreter - Legal', 400, 1, 10, 1), 
            (10, 2, 'Certified Conference Interpreter', 'Certified Conference Interpreter', 400, 1, 8, 1), 
            (11, 2, 'Certified Conference Interpreter LOTE to LOTE', 'Certified Conference Interpreter', 400, 1, 11, 1), 
            (12, 3, 'Recognised Practising Deaf Interpreter', 'Recognised Practising Deaf Interpreter', 100, 0, 13, 1), 
            (13, 3, 'Certified Provisional Deaf Interpreter', 'Certified Provisional Deaf Interpreter', 100, 0, 12, 1), 
            (14, 4, 'CCL', 'Credentialed Community Language', 100, 0, 14, 0),  
            (15, 5, 'CLA', 'Community Language Aide', 100, 0, 15, 0),  
            (16, 6, 'Ethics', 'Ethics', 100, 0, 16, 0),  
            (17, 7, 'Intercultural', 'Intercultural', 100, 0, 17, 0),  
            (18, 8, 'Migration', 'Migration Assessment', 100, 0, 18, 0) 

SET IDENTITY_INSERT tblcredentialtype OFF 