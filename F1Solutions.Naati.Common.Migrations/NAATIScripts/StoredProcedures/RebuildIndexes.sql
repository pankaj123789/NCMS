ALTER PROCEDURE [dbo].RebuildIndexes
AS

DECLARE @Table NVARCHAR(255);  
DECLARE @cmd NVARCHAR(1000);

   DECLARE TableCursor CURSOR READ_ONLY FOR SELECT TABLE_NAME as tableName 
   FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'   
   And TABLE_SCHEMA = 'dbo' --Ignore hangfire tables

   OPEN TableCursor   

   FETCH NEXT FROM TableCursor INTO @Table       

   WHILE @@FETCH_STATUS = 0   
   BEGIN
      BEGIN TRY   
         SET @cmd = 'ALTER INDEX ALL ON ' + @Table + ' REBUILD' 
         EXEC (@cmd) 
      END TRY
      BEGIN CATCH
         PRINT '---'
         PRINT @cmd
         PRINT ERROR_MESSAGE() 
         PRINT '---'
      END CATCH

      FETCH NEXT FROM TableCursor INTO @Table   

   END   

   CLOSE TableCursor   
   DEALLOCATE TableCursor  