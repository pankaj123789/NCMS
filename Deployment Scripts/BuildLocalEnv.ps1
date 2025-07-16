
$env:BUILD_SOURCESDIRECTORY = "D:\Temp\Build\s"
$buldConsecutiveFile = "D:\Temp\BuildHelmPackages\buildConsecutive"
$buildReleaseFile = "D:\Temp\BuildHelmPackages\buildReleaseNotes.txt"
$buildPackages =  "D:\Temp\BuildHelmPackages"

$sourceDirectory = "D:\TFS\NAATI SAM and ePortal\Dev\Dev\Src\Naati\*"


$RepositoryFullName ="acrtest0002.azurecr.io"
$NaatiBaseImageName = "naatibase"
$NcmsImageName = "ncmsui"
$MyNaatiImageName = "mynaatiui"
$RepositoryName ="acrtest0002"
$Version = "2.0.0"
$env:BranchPrefix = "lcl" #Put your custom prefix here.
$deployEnvironment ="" ##LEAVE EMTPY IF NOT DEPLOYMENT
$aksContext = "aksclustertest" 
$runTests = $true

$ErrorActionPreference = "Stop"
$Build =@{ }
$Build.StagingDirectory = "D:\Temp\Build\a"
$Build.Repository = @{}
$Build.Repository = @{}

$stagingDirectory =$Build.StagingDirectory

$Build.Repository.LocalPath = $env:BUILD_SOURCESDIRECTORY

Write-Host Set location 
Set-Location $env:BUILD_SOURCESDIRECTORY

Write-Host Drop build folder
Remove-Item -path $env:BUILD_SOURCESDIRECTORY\*  -force -Recurse
$dirtoremove = $Build.StagingDirectory
Remove-Item -path $dirtoremove\*  -force -Recurse


Write-Host $Build.Repository.LocalPath

Write-Host Copy files to build folder
xcopy $sourceDirectory  $env:BUILD_SOURCESDIRECTORY\Naati\ /s /y

Write-Host Source directory is $env:BUILD_SOURCESDIRECTORY 

if(Test-Path $buldConsecutiveFile)
{
 $foundContent =Get-Content($buldConsecutiveFile)
}
else
{
 $foundContent = "0"
}
$consecutive = ([int]($foundContent) +1)

$consecutive | Out-File $buldConsecutiveFile
Write-Host Build consecutive $consecutive


$number =GET-DATE -FORMAT "yyyyMMdd"
$env:BUILD_BUILDNUMBER = "NAATI ALL - Local_$number.$consecutive"



$buildPath = $Build.Repository.LocalPath
$prebuildFile ="$buildPath/Naati/Deployment Scripts/PreBuildScript.ps1"
&$prebuildFile "$Version.X"
$ContainerTag =$env:ContainerTag

Write-Host Build Number is  $env:BUILD_BUILDNUMBER

$msBuild ="C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\Msbuild.exe"



Write-Host build  ncms and mynaati
&$msBuild  "$buildPath\Naati\Ncms.Ui\Ncms.Ui.csproj" /nr:false /nologo /p:DeployDefaultTarget=WebPublish  /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=True /p:publishUrl=$stagingDirectory\Ncms.Ui\_PublishedWebsites\Ncms.Ui /t:WebPublish /p:Platform=AnyCPU /p:Configuration=Release /m

&$msBuild  "$buildPath\Naati\MyNaati.Ui\MyNaati.Ui.csproj" /nologo /nr:false  /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=True /p:publishUrl=$stagingDirectory\MyNaati.Ui\_PublishedWebsites\MyNaati.Ui /t:WebPublish /p:Platform=AnyCPU /p:Configuration=Release /m



if($runTests = $true)
{
$testConsole = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\Extensions\TestPlatform\vstest.console.exe"
&$msBuild  "$buildPath\Naati\MyNaati.Test\MyNaati.Test.csproj" /nologo /nr:false  /p:OutDir=$stagingDirectory\MyNaati.Test   /p:Platform=AnyCPU /p:Configuration=Release /m
&$msBuild  "$buildPath\Naati\Ncms.Test\Ncms.Test.csproj" /nologo /nr:false  /p:OutDir=$stagingDirectory\Ncms.Test   /p:Platform=AnyCPU /p:Configuration=Release /m
&$testConsole $stagingDirectory\MyNaati.Test\MyNaati.Test.dll $stagingDirectory\Ncms.Test\Ncms.Test.dll /EnableCodeCoverage /InIsolation /logger:trx 
}


if(-not $deployEnvironment)
{
  return
}

Write-Host build  Copy docker files

$copyDockerFiles = "$buildPath/Naati/Deployment Scripts/CopyDockerFiles.ps1"
&$copyDockerFiles $stagingDirectory "$RepositoryFullName/naatibase" $ContainerTag


Write-Host build  images
$dockerExe = "C:\Program Files\Docker\Docker\Resources\bin\docker.exe"
&$dockerExe build -f "$buildPath/Naati/Deployment Scripts/Docker/NaatiBase/DockerFile" -t "$RepositoryFullName/$NaatiBaseImageName`:$ContainerTag" "$buildPath\Naati\Deployment Scripts\Docker\NaatiBase"
&$dockerExe build -f "$buildPath\Ncms.Ui\_PublishedWebsites\Ncms.Ui\DockerFile " -t "$RepositoryFullName/$NcmsImageName`:$ContainerTag" "$buildPath\Ncms.Ui\_PublishedWebsites\Ncms.Ui"
&$dockerExe build -f "$buildPath\MyNaati.Ui\_PublishedWebsites\MyNaati.Ui\DockerFile" -t "$RepositoryFullName/$MyNaatiImageName`:$ContainerTag" "$buildPath\MyNaati.Ui\_PublishedWebsites\MyNaati.Ui"

Write-Host Push images
az acr login --name $RepositoryName
&$dockerExe push  "$RepositoryFullName/$NcmsImageName`:$ContainerTag"
&$dockerExe push "$RepositoryFullName/$MyNaatiImageName`:$ContainerTag"

Write-Host Push helm package

$pushHelmPackage = "$buildPath\Naati\Deployment Scripts\PushHelmPackages.ps1"
&$pushHelmPackage  -naatiRepositoryName $RepositoryName  -helmFolder "$buildPath\Naati\Deployment Scripts\Helm"

$HelmPackage = $env:HelmPackage

Write-Host Backup helm Package

$copyHelmToFolder = "$buildPath/Naati/Deployment Scripts/CopyHelmToDropFolder.ps1"
$helPackageFolder ="$buildPath\Naati\Deployment Scripts\Helm"
&$copyHelmToFolder -dropFolder $buildPackages -helmFolder $helPackageFolder

Write-Host "$ContainerTag,$HelmPackage,$NcmsImageName,$MyNaatiImageName"


if($deployEnvironment)
{
   Write-Host Deploy to $deployEnvironment
   cd $helPackageFolder
  helm upgrade $deployEnvironment naati  -f "./naati/_values.$deployEnvironment.yaml" --namespace $deployEnvironment --install --kube-context $aksContext
}


Write-Host "$ContainerTag,$HelmPackage,$NcmsImageName,$MyNaatiImageName" 
$buildDate =Get-Date
 "$buildDate ->    $ContainerTag,$HelmPackage,$NcmsImageName,$MyNaatiImageName" >> $buildReleaseFile 

Write-host Delete local images
$deleteLocalImageFile ="$buildPath\Naati\Deployment Scripts\DeleteLocalImages.ps1"
&$deleteLocalImageFile  -name $NcmsImageName -retentionDays 0
&$deleteLocalImageFile  -name $MyNaatiImageName -retentionDays 0


Write-Host Delete old remote images
$deleteRemoteImageFile = "$buildPath\Naati\Deployment Scripts\DeleteRemoteImages.ps1"
try
{
&$deleteRemoteImageFile  -repository $NcmsImageName -name $RepositoryName -retentionDays 20
}
catch
{
}
try
{
&$deleteRemoteImageFile   -repository $MyNaatiImageName -name $RepositoryName -retentionDays 20
}
catch
{
}
try
{
&$deleteRemoteImageFile   -repository $NaatiBaseImageName -name $RepositoryName -retentionDays 20
}
catch
{
}






