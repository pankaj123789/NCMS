param([string]$environment, [string]$databaseServer, [string]$scriptName)

. .\SqlFunctions.ps1

$variableDictionary = @{
    "ncmsDbName" = "NCMS_$environment"; 
	"mynaati" = "MyNAATI_$environment";   
	"ncmsDbReporting" = "NCMS_Reporting_$environment";
}

$scriptName = ".\" + $scriptName

RunSqlScript $scriptName $databaseServer $variableDictionary 
