param([string]$environment, [string]$databaseServer)

. .\SqlFunctions.ps1

$variableDictionary = @{
    "ncmsDbName" = "NCMS_$environment";
    "samDbName" = "SAM_$environment";
    "reportDbName" = "NCMS_Reporting_$environment";
    "domain" = "f1test";
    "eportalServiceAccount" = "ePortalApplicationAp";
    "samServiceAccount" = "SAMApplicationAppPoo";
    "naatiTestServiceAccount" = "NAATITestService";
}

RunSqlScript ".\PreMigrations.sql" $databaseServer $variableDictionary
