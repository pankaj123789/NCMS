function GetMenuOption($heading, $message, $options) 
{
	Write-Host
	Write-Host $heading
	Write-Host "*********"
	Write-Host
	
	if($message -ne $null) {
		Write-Host $message
		Write-Host
	}	
	$message = $null
		
	Write-Host

	$index = 1
	foreach ($option in $options) {
		$output = "  $index : $($option.text)"
		Write-Host $output	
		$index++
	}
	
	Write-Host
	$input = Read-Host "Please make a selection" 
	
	return $options[$input - 1]
}

function InitialiseDevFolder($folder)
{
	New-Item -ItemType directory -Path $folder
}

function GetUserScriptFolder()
{
	$myDocuments = [Environment]::GetFolderPath("MyDocuments")
	$devFolder = "$myDocuments\OmniDevScripts"
	if(!(Test-Path -Path $devFolder )){
		InitialiseDevFolder $devFolder		
	}
	return $devFolder
}

function ReadCsvContentsIntoDictionary($filePath, $dictionary, $prefix)
{
	$csvData = Import-CSV $filePath
	foreach ($row in $csvData)
	{	
		if($row -ne $null) 
		{
			$key = ""
			if([string]::IsNullOrEmpty($prefix) -eq $false)
			{
				$key = "$prefix."
			}
		
			$key += $row.Key
		
			$dictionary[$key] = $row.Value
		}
	}
}

function GetSystemList([string]$rootPath = ".")
{
	return Get-ChildItem "$rootPath\XML/*.xml" | Select-Object @{Name="Name"; Expression={([regex]::Match($_.Name, "(.*)\.Args.*\.xml").Groups[1].Value) }}
}

function GetEnvironmentList([string]$rootPath = ".")
{
	return Get-ChildItem "$rootPath/Args" | where {$_.PSIsContainer}
}

function GetClientList([string]$environment, [string]$rootPath = ".")
{
	return Get-ChildItem "$rootPath/Args/$environment/Args.*.csv" | Where { $_.Name -notmatch ".*Environment.*" } | Select-Object @{Name="Name"; Expression={([regex]::Match($_, "Args\.(.*)\.csv").Groups[1].Value) }} 
}

function EnsureDirectoryExists([string]$folderPath)
{
	if((Test-Path "$folderPath") -eq $false)
	{
		Write-Host "Creating $folderPath directory since it doesn't exist"
		New-Item -Type directory "$folderPath"		
	}
	else
	{
		Write-Host "$folderPath directory already exists. Continuing."
	}
}

function CreateBuildNumber($versionNumber)
{
	write-host CreateBuildNumber input: $versionNumber
	$revision = $env:BUILD_BUILDNUMBER.Substring($env:BUILD_BUILDNUMBER.LastIndexOf(".") + 1)
	write-host CreateBuildNumber revision: $revision

	# make sure this year agrees with the year in SystemController.SystemInfo
	$autobuildnum = ([DateTime]::Now.Date - (new-object System.DateTime(2022,1,1))).Days
	$revisionPart = "$autobuildnum" + "$revision"

	$versionNumber = $versionNumber.Replace("X", $revisionPart)
	$versionNumber = $versionNumber.Replace("Y", $autobuildnum)
	$versionNumber = $versionNumber.Replace("Z", $revision)

	return $versionNumber
}