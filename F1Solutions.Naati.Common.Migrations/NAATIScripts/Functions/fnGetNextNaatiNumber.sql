CREATE FUNCTION [dbo].fnGetNextNAATINumber (@NAATINumberType INT) 
RETURNS INT 
AS 
  BEGIN 
      DECLARE @NewNAATINumber INT 

      IF ( @NAATINumberType = 3 ) 
        SET @NewNAATINumber = (SELECT Max(NAATINumber) + 1 
                               FROM   tblEntity 
                               WHERE  NAATINumber >= 910000) 
      ELSE 
        SET @NewNAATINumber = (SELECT Max(NAATINumber) + 1 
                               FROM   tblEntity 
                               WHERE  NAATINumber < 910000) 

      IF @NewNAATINumber >= 910000 
        SET @NewNAATINumber = (SELECT Max(NAATINumber) + 1 
                               FROM   tblEntity 
                               WHERE  NAATINumber > 910000) 

      RETURN @NewNAATINumber 
  END 