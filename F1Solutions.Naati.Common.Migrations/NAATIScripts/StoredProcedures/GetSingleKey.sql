CREATE PROCEDURE [dbo].[GetSingleKey] (@TableName VARCHAR(100), 
                                       @NextKey   INT OUTPUT) 
AS 
    DECLARE @table TABLE 
      ( 
         id INT 
      ) 

    UPDATE tblTableData 
    SET    NextKey = NextKey + 1 
    OUTPUT inserted.NextKey 
    INTO @table 
    WHERE  TableName = @TableName 

    SELECT @NextKey = Id 
    FROM   @table 
