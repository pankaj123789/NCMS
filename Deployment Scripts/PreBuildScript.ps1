param([string]$build)

write-host Sources folder is $env:BUILD_SOURCESDIRECTORY
$sourcesFolder = "$env:BUILD_SOURCESDIRECTORY\Naati"

Set-Location $PSScriptRoot
. "$sourcesFolder\Deployment Scripts\FunctionLibrary.ps1"

$versionNumber = CreateBuildNumber($build)

Write-Host "Searching files in  verion in $sourcesFolder folder "
$assemblyInfoFiles = (Get-ChildItem -Include AssemblyInfo.cs -Recurse -Path $sourcesFolder).FullName

# Update AssemblyInfo.cs
write-host Updating AssemblyInfo files 
Foreach ($assemblyInfoFile in $assemblyInfoFiles )
{   
	$file =$assemblyInfoFile;
	$filecontent = Get-Content($file)
	attrib $file -r
	write-host Setting version to $versionNumber to $assemblyInfoFile
	$filecontent -replace "1.0.0.0", $versionNumber | Out-File $file
}

$helmChartFiles= (Get-ChildItem -Include Chart.yaml -Recurse -Path $sourcesFolder).FullName
# Update Chart.yaml
write-host Updating Helm chart files 
$branchNamePrefix = "$env:BranchPrefix"
$versiontag ="$versionNumber-$branchNamePrefix"
$helmPackageVersion = $build.ToUpper().replace(".X", "").replace(".Y","").replace(".Z","")
$buildSequence = CreateBuildNumber("X")
$helmPackageVersion = "$helmPackageVersion-$branchNamePrefix.$buildSequence"
Foreach ($helmChartFile in $helmChartFiles )
{   
	$file =$helmChartFile;
	$filecontent = Get-Content($file)
	attrib $file -r
	write-host Setting version to $versionNumber to $helmChartFile
	$filecontent = $filecontent.replace("appVersion: 1.0.0.0", "appVersion: $versiontag")
	$filecontent -replace "version: 1.0.0", "version: $helmPackageVersion" | Out-File $file
}
Write-Host Set container tag
Write Host Set Container Tag $versiontag
Write-Host ("##vso[task.setvariable variable=ContainerTag;]$versiontag")


# Run require optimizer build script
.\OptimizeRequire.ps1

$env:ContainerTag =$versiontag

