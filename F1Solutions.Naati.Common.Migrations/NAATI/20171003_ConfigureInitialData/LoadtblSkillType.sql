SET IDENTITY_INSERT tblskilltype ON 

INSERT INTO tblSkillType 
            (SkillTypeId, [Name], DisplayName) 
VALUES      (1, 'RPT', 'Recognised Practising Translator'),  
            (2, 'CT', 'Certified Translator'),  
            (3, 'CAT', 'Certified Advanced Translator'),  
            (4, 'CTLOTE', 'Certified Advanced Translator LOTE to LOTE'),  
            (5, 'RPI', 'Recognised Practising Interpreter'),  
            (6, 'CPI', 'Certified Provisional Interpreter'),  
            (7, 'CI', 'Certified Interpreter'),  
            (8, 'CCI', 'Certified Conference Interpreter'),  
            (9, 'CSIH', 'Certified Specialist Interpreter - Health'),  
            (10, 'CSIL', 'Certified Specialist Interpreter - Legal'),  
            (11, 'CCILOTE', 'Certified Conference Interpreter LOTE to LOTE'),  
            (12, 'CPDI', 'Certified Provisional Deaf Interpreter'),  
            (13, 'RPDI', 'Recognised Practising Deaf Interpreter'),  
            (14, 'CCL', 'Credentialed Community Language'),  
            (15, 'CLA', 'Community Language Aide'),  
            (16, 'Ethics', 'Ethics'),  
            (17, 'Intercultural', 'Intercultural'),  
            (18, 'Migration', 'Migration Assessment') 

SET IDENTITY_INSERT tblskilltype OFF 