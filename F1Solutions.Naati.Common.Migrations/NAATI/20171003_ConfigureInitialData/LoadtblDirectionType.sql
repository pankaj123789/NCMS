SET IDENTITY_INSERT tbldirectiontype ON 

INSERT INTO tblDirectionType 
            (DirectionTypeId, NAME, DisplayName) 
VALUES      (1, 'L1toL2', '[Language 1] into [Language 2]'),  
            (2, 'L2toL1', '[Language 2] into [Language 1]'),  
            (3, 'L1andL2', '[Language 1] and [Language 2]'),  
            (4, 'L1', '[Language 1]') 

SET IDENTITY_INSERT tbldirectiontype OFF 