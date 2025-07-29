# -----------------------------------------------
# Script to Copy Secrets Between Two Azure Key Vaults
# -----------------------------------------------

# Set your Key Vault names and subscription IDs
$sourceKV  = 'ntkeyvault3'
$sourceSub = '71546dd0-1b4b-49a2-b6a3-7b599c5e1025'
$destKV    = 'naatibackupvaultuat'
$destSub   = 'e758ba72-18d4-4563-b424-9960b597ecee'

# Step 1: Set context to the source subscription
az account set --subscription $sourceSub

# Step 2: Retrieve all secret names from source Key Vault
$secretNames = az keyvault secret list `
    --vault-name $sourceKV `
    --query "[].name" `
    -o json | ConvertFrom-Json

# Step 3: Switch context to the destination subscription
az account set --subscription $destSub

# Step 4: Loop through each secret and copy it
foreach ($name in $secretNames) {
    $value = $(az keyvault secret show `
        --name $name `
        --vault-name $sourceKV `
        --query "value" `
        -o tsv)

    az keyvault secret set `
        --vault-name $destKV `
        --name $name `
        --value $value

    Write-Host " Copied secret '$name' to Key Vault '$destKV'"
    $value = $null
}

# Step 5: Clear secret names
$secretNames = $null
Write-Host "`n🎉 All secrets copied successfully from '$sourceKV' to '$destKV'."
