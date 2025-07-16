. .\SqlFunctions.ps1

$variableDictionary = @{
  "samDbName" = "NAATI_SAMLegacy_Test";
  "reportDbName" = "NAATI_ReportingLegacy_Test";
  "domain" = "f1test";
  "eportalServiceAccount" = "ePortalApplicationAp";
  "samServiceAccount" = "SAMApplicationAppPoo";
  "naatiTestServiceAccount" = "NAATITestService";
}

$databaseServer = "F1HADB001-TEST"

RunSqlScript ".\PreMigrations.sql" $databaseServer $variableDictionary
