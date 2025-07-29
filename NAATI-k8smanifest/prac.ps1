# Configure variables
$srcKV = "naati-dev-kv-jyrb"
$destKV = "naati-dev-sql2-kv-jyrb"
$subscriptionId = "e758ba72-18d4-4563-b424-9960b597ecee"

# Log in to Azure if not already logged in
if (-not (az account show 2>$null)) {
    az login
}

# Set the subscription context
az account set --subscription $subscriptionId

# Get all secret names from the source Key Vault
$secretNames = az keyvault secret list --vault-name $srcKV --query "[].name" -o tsv
$secretNamesArray = $secretNames -split "`n"

foreach ($secretName in $secretNamesArray) {
    # Skip if the line is empty
    if ([string]::IsNullOrWhiteSpace($secretName)) {
        continue
    }

    # Get the secret value
    $secretValue = az keyvault secret show --vault-name $srcKV --name $secretName --query value -o tsv

    # Set the secret in the destination Key Vault
    az keyvault secret set --vault-name $destKV --name $secretName --value $secretValue

    Write-Host "Copied secret: $secretName"
}

Write-Host "`n✅ All secrets copied from $srcKV to $destKV in subscription $subscriptionId"
