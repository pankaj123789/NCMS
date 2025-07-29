# Configure variables
$srcKV = "ntkeyvault3"
$srcSub = "71546dd0-1b4b-49a2-b6a3-7b599c5e1025"
$destKV = "naatibackupvaultuat"
$destSub = "e758ba72-18d4-4563-b424-9960b597ecee"

# Login to Azure if needed
if (-not (az account show 2>$null)) {
    az login
}

# Set context to source subscription
az account set --subscription $srcSub

# Get all secret names from source vault
$secretNames = az keyvault secret list --vault-name $srcKV --query "[].name" -o tsv
$secretNamesArray = $secretNames -split "`n"

# Switch to destination subscription
az account set --subscription $destSub

foreach ($secretName in $secretNamesArray) {
    # Skip empty lines
    if ([string]::IsNullOrWhiteSpace($secretName)) {
        continue
    }
    # Get secret value from source subscription vault (switch temporarily)
    az account set --subscription $srcSub
    $secretValue = az keyvault secret show --vault-name $srcKV --name $secretName --query value -o tsv

    # Switch back to destination subscription and set secret
    az account set --subscription $destSub
    az keyvault secret set --vault-name $destKV --name $secretName --value $secretValue

    Write-Host "Copied secret: $secretName"
}

Write-Host "Finished copying secrets."
