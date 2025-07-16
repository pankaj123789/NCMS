
function RunPackageScripts($helmValuesFilePath, $keyVaultName ,$octopusPrefix)
{
	SetKeyVaultKeys $helmValuesFilePath $keyVaultName $octopusPrefix	
}

function SetKeyVaultKeys($helmValuesFilePath, $keyVaultName ,$octopusPrefix)
{
	$secretsKeys = "ConnectionString",
		"ReportingDbConnectionString",
		"NcmsMigrationConnectionString" ,
		"ReportingMigrationConnectionString" ,
		"AadAuthTenant" ,
		"AadAuthClientId" ,
		"AzureEmailAccount" ,
		"SharedAccounts" ,
		"GoogleApiKeyFrontend" ,
		"GoogleApiKeyBackend",
		"Office365AppId" ,
		"AzureAppClientId" ,
		"AzureAppTenant" ,
		"SASAccountName" ,
		"SASShareName" ,
		"SASAccountKey" ,	
		"DefaultUserDomain" ,
		"RecaptchaSecretKey" ,
		"SecurePayMerchantId" ,
		"SecurePayMerchantPassword" ,
		"NcmsDefaultIdentity" ,
		"MyNaatiDefaultIdentity",
		"SecurePayClientId",
		"SecurePayMerchantCode",
		"SecurePayClientSecret",
		"OutboundIP",
		"PayPalClientId",
		"PayPalClientSecret",
		"WiiseAuthClientId",
		"WiiseClientSecret",
		"WiiseAuthTenant"

	if([string]::IsNullOrEmpty($helmValuesFilePath) -eq $true)
	{
		Write-Warning  "helm values file is null or empty"
	}


	$parameters = @{ secretName =""
					helmValuesFilePath = $helmValuesFilePath
					octopusPrefix = $octopusPrefix
					keyVaultName = $keyVaultName
					}	

	ForEach ($secretKey in $secretsKeys)
	{
		$parameters.secretName = $secretKey
		SetKeyVaultSecret @parameters
	}

}


function SetKeyVaultSecret($secretName,  $helmValuesFilePath,  $keyVaultName, $octopusPrefix) 
{	
   $prefixedKey = GetPrefixedSecretKey $secretName  $helmValuesFilePath
   $secretValue = GetSecretValue $secretName $octopusPrefix
   Write-Host  "Setting KeyVaault: Key: '$prefixedKey'  Value:'$secretValue'"
   $result = az keyvault secret set --vault-name $keyVaultName --name $prefixedKey --value `"$secretValue`"
   Write-Host $result
}

function GetPrefixedSecretKey($secretKey, $valuesFilesPath)
{ 	 
	 $secretPrefix ='';
	 $matches = Get-Content $valuesFilesPath | select-string '"SecretPrefix" *: *"(.*)"'
	 if($matches.Matches.Length -eq 0)
	 {
		Write-Error "SecretPrefix was not found in file: $valuesFile"	
	 }

     $secretPrefix  = $matches.Matches[0].Groups[1].Value

	 if([string]::IsNullOrEmpty($secretPrefix))
	 {
		Write-Error "SecretPrefix is null or empty"	
		
	 }

	 return "$secretPrefix$secretKey"
}

function GetSecretValue($secretKey, $octopusPrefix)
{	
	$secretValue= $OctopusParameters["$octopusPrefix$secretKey"]

	if([string]::IsNullOrEmpty($secretValue) -eq $true)
	{
	   Write-Warning  "Value for key $secretKey is null or empty"
	}

	return $secretValue
}