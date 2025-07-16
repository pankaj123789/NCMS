param([string]$environment, [string]$databaseServer)

. .\SqlFunctions.ps1

$variableDictionary = @{
    "ncms" = "NCMS_$environment";
    "mynaati" = "MyNAATI_$environment";   
}

RunSqlScript ".\SyncNAATIAndNCMSAccounts.sql" $databaseServer $variableDictionary
