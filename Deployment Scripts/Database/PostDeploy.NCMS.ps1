param([string]$environment, [string]$databaseServer)

. .\SqlFunctions.ps1

$variableDictionary = @{
    "ncmsDbName" = "NCMS_$environment";	
	"mynaati" = "MyNAATI_$environment";
    "domain" = "f1test"; 
    "eportalServiceAccount" = "ePortalApplicationAp";
    "samServiceAccount" = "SAMApplicationAppPoo";
    "naatiTestServiceAccount" = "NAATITestService";
	"ncmsServiceAccount" = "SAMTest_svc";
}

RunSqlScript ".\PostMigrations.sql" $databaseServer $variableDictionary
