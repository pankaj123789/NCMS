param ([string]$name, [int]$retentionDays)

Set-StrictMode -Version Latest

$date = (Get-Date).AddDays(-1 * $retentionDays)
$filter = "*$($name):*"
Write-Host "Removing $name images before $date filter $filter"

$images = (docker images --filter=reference="*/$filter" --filter=reference=$filter --format "{{json .}}" | ConvertFrom-Json)

foreach ($image in $images) 
{
	
	$imageId = $image.ID;

	[datetime]$createdAt = $image.CreatedAt -replace "[a-zA-Z]"
	Write-Host "Image $imageId  created on : $createdAt"
	if ($createdAt -lt $date) 
	{
		Write-Host "Removing image: $imageId"
		docker rmi -f $imageId
	}
}

return 0;