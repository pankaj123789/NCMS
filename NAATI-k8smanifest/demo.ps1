# -----------------------------------------------
# Script to Copy Secrets Between Two Azure Key Vaults (Same Subscription)
# -----------------------------------------------

# Set your source and destination Key Vault names
$sourceKV = 'naati-dev-kv-jyrb'
$destKV   = 'naati-dev-sql1-kv-jyrb'

# Optional: Set the subscription ID explicitly (if needed)
$subscriptionId = 'e758ba72-18d4-4563-b424-9960b597ecee'
az account set --subscription $subscriptionId

# Step 1: Get all secret names from the source Key Vault
$secretNames = az keyvault secret list `
    --vault-name $sourceKV `
    --query "[].name" `
    -o json | ConvertFrom-Json

# Step 2: Loop through each secret and copy it to the destination Key Vault
foreach ($name in $secretNames) {
    # Get the secret value from the source Key Vault
    $value = $(az keyvault secret show `
        --vault-name $sourceKV `
        --name $name `
        --query "value" `
        -o tsv)

    # Set the secret in the destination Key Vault
    az keyvault secret set `
        --vault-name $destKV `
        --name $name `
        --value $value

    Write-Host "✅ Copied secret '$name' to Key Vault '$destKV'"

    # Clear secret value from memory
    $value = $null
}

# Clear variable from memory
$secretNames = $null

Write-Host "`n🎉 All secrets copied successfully from '$sourceKV' to '$destKV'."
