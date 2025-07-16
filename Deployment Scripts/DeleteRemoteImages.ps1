param ([string]$repository, [string]$name, [int]$retentionDays)

Set-StrictMode -Version Latest

$date = (Get-Date).AddDays(-1 * $retentionDays).ToString("yyyy-MM-dd")
Write-Host "Removing $repository images from $name before $date"

$images = az acr repository show-tags --repository $repository --name $name --detail --query "[?lastUpdateTime < '$date']" | ConvertFrom-Json
foreach ($image in $images) 
{
	Write-Host  "Removing image: $repository`:$($image.name)"
	$deleteResult = az acr repository delete -n $name --image "$repository`:$($image.name)" -y
	Write-Host $deleteResult
}

Write-Host "Removing naati helm packages from $name before $date"

$images = az acr helm list -n $name --query "naati[?name == 'naati' && created < '$date']"  -o json | ConvertFrom-Json
foreach ($image in $images) 
{
	Write-Host "Removing helm package: $($image.name)`:$($image.version)"
	try
	{
	  $deleteResult = az acr helm delete -n $name naati --version $image.version -y
	}
	catch 
	{
		##skip warning
	}
	
	Write-Host $deleteResult
}

return 0;