param([string]$dropFolder, [string]$helmFolder)

Set-StrictMode -Version Latest

$buildName = "Ncms and MyNaati" + $env:BUILD_BUILDNUMBER.Substring($env:BUILD_BUILDNUMBER.LastIndexOf("-"))
$dropFolder =  "$dropFolder\$buildName"
$dropFolder = Join-Path $dropFolder "Helm"

# Create folder structure
Write-Host "Creating dropFolder: $dropFolder"
mkdir $dropFolder

$outputHemlZip = "$dropFolder\Helm.zip"

Write-Host "Zipping Helm to $outputHemlZip"
Compress-Archive -Path "$helmFolder\*" -DestinationPath $outputHemlZip -CompressionLevel Fastest -Verbose 