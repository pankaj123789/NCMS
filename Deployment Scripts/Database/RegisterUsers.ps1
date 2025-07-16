param([string]$environment, [string]$databaseServer)

. .\SqlFunctions.ps1

$variableDictionary = @{
    "ncms" = "NCMS_$environment";
    "mynaati" = "MyNAATI_$environment";   
	"temptbl" = "#ltdata"
}

RunSqlScript ".\RegisterMyNaatiUsers.sql" $databaseServer $variableDictionary
