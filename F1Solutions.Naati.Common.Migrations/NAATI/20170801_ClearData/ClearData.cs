using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170801_ClearData
{
    [NaatiMigration(201708011800)]
    public class ClearData : NaatiMigration
    {
        public override void Up()
        {
            DropMailCategoryColumnFromStandardLetter();
            ClearTables();
            ClearInstitutionName();
            ClearInstitutions();
            ClearEntities();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }


        void DropMailCategoryColumnFromStandardLetter()
        {

            var deleteForeingKeysQuery = @"DECLARE @SCHEMA_NAME NVARCHAR(256)
                            DECLARE @TABLE_NAME NVARCHAR(256)
                            DECLARE @COL_NAME NVARCHAR(256)
                            DECLARE @COMMAND  NVARCHAR(1000)

                            SET @SCHEMA_NAME = N'DBO'
                            SET @TABLE_NAME = N'TBLSTANDARDLETTER'
                            SET @COL_NAME = N'MAILCATEGORYID'
                            SELECT @COMMAND = 'ALTER TABLE ' + @SCHEMA_NAME + '.' + @TABLE_NAME + ' DROP CONSTRAINT ' + FK.NAME FROM SYS.FOREIGN_KEYS FK
                               INNER JOIN SYS.FOREIGN_KEY_COLUMNS FKC
                               ON FK.OBJECT_ID = FKC.CONSTRAINT_OBJECT_ID
                               INNER JOIN SYS.TABLES PT
                               ON PT.OBJECT_ID = FKC.PARENT_OBJECT_ID
                               INNER JOIN SYS.COLUMNS PC
                               ON PT.OBJECT_ID = PC.OBJECT_ID AND FKC.PARENT_COLUMN_ID =  PC.COLUMN_ID 
                               WHERE PT.SCHEMA_ID = SCHEMA_ID(@SCHEMA_NAME) AND PT.NAME =@TABLE_NAME AND PC.NAME =@COL_NAME
                            EXECUTE (@COMMAND)";

            Execute.Sql(deleteForeingKeysQuery);

            var deleteDefaultConstraintsQuery = @"DECLARE @SCHEMA_NAME NVARCHAR(256)
                                                DECLARE @TABLE_NAME NVARCHAR(256)
                                                DECLARE @COL_NAME NVARCHAR(256)
                                                DECLARE @COMMAND  NVARCHAR(1000)

                                                SET @SCHEMA_NAME = N'DBO'
                                                SET @TABLE_NAME = N'TBLSTANDARDLETTER'
                                                SET @COL_NAME = N'MAILCATEGORYID'
                                                SELECT @COMMAND = 'ALTER TABLE ' + @SCHEMA_NAME + '.' + @TABLE_NAME + ' DROP CONSTRAINT ' + DC.NAME FROM SYS.DEFAULT_CONSTRAINTS DC  
                                                   INNER JOIN SYS.TABLES PT
                                                   ON PT.OBJECT_ID = DC.PARENT_OBJECT_ID
                                                   INNER JOIN SYS.COLUMNS PC
                                                   ON PT.OBJECT_ID = PC.OBJECT_ID AND DC.PARENT_COLUMN_ID =  PC.COLUMN_ID 
                                                   WHERE PT.SCHEMA_ID = SCHEMA_ID(@SCHEMA_NAME) AND PT.NAME =@TABLE_NAME AND PC.NAME =@COL_NAME
                                                PRINT @COMMAND
                                                EXECUTE(@COMMAND)";

            Execute.Sql(deleteDefaultConstraintsQuery);

            var deleteStatistics = @"DECLARE @SCHEMA_NAME NVARCHAR(256)
                                    DECLARE @TABLE_NAME NVARCHAR(256)
                                    DECLARE @COL_NAME NVARCHAR(256)
                                    DECLARE @COMMAND  NVARCHAR(1000)

                                    SET @SCHEMA_NAME = N'DBO'
                                    SET @TABLE_NAME = N'TBLSTANDARDLETTER'
                                    SET @COL_NAME = N'MAILCATEGORYID'
                                    SELECT @COMMAND = 'DROP STATISTICS ' + @SCHEMA_NAME + '.' + @TABLE_NAME + '.' + ST.NAME FROM SYS.STATS ST
                                       INNER JOIN SYS.STATS_COLUMNS SC
                                       ON ST.OBJECT_ID = SC.OBJECT_ID AND ST.STATS_ID = SC.STATS_ID 
                                       INNER JOIN SYS.TABLES PT
                                       ON PT.OBJECT_ID = SC.OBJECT_ID
                                       INNER JOIN SYS.COLUMNS C
                                       ON C.OBJECT_ID = SC.OBJECT_ID AND SC.COLUMN_ID =  C.COLUMN_ID 
                                       WHERE PT.SCHEMA_ID = SCHEMA_ID(@SCHEMA_NAME) AND PT.NAME =@TABLE_NAME AND C.NAME =@COL_NAME
                                    EXECUTE (@COMMAND)";

            Execute.Sql(deleteStatistics);

        }

        void ClearInstitutions()
        {
            var query = "DELETE FROM TBLINSTITUTION WHERE INSTITUTIONID NOT IN(SELECT DISTINCT INSTITUTIONID FROM TBLOFFICE)";
            Execute.Sql(query);
        }

        void ClearEntities()
        {
            var query = "DELETE FROM TBLENTITY WHERE ENTITYID NOT IN (SELECT DISTINCT ENTITYID FROM TBLINSTITUTION)";
            Execute.Sql(query);
        }

        void ClearTables()
        {
            Execute.Sql(Sql.clearTables);
        }

        void ClearInstitutionName()
        {
            var query =
                "DELETE FROM TBLINSTITUTIONNAME WHERE INSTITUTIONID NOT IN (SELECT DISTINCT INSTITUTIONID FROM TBLOFFICE)";
            Execute.Sql(query);
        }
       
    }
}
