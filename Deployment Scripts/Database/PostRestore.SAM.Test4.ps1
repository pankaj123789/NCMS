. .\SqlFunctions.ps1

$variableDictionary = @{
  "samDbName" = "SAM_TEST4";
  "reportDbName" = "SAM_Reporting_TEST4";
  "domain" = "f1test";
  "eportalServiceAccount" = "ePortalApplicationAp";
  "samServiceAccount" = "SAMApplicationAppPoo";
  "naatiTestServiceAccount" = "NAATITestService";
}

$databaseServer = "F1HADB004-TEST"

RunSqlScript ".\PreMigrations.sql" $databaseServer $variableDictionary
