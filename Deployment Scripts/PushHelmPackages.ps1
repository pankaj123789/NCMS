param([string]$NaatiRepositoryName, [string]$HelmFolder)

Write-Output "Entered PushHelmPackages.ps1(NaatiRepositoryName = ${NaatiRepositoryName}, HelmFolder = ${HelmFolder}"
$ErrorActionPreference = "Stop"

Write-Output "Change current directory to ${HelmFolder}."
Push-Location $HelmFolder

Write-Output "Login to Azure Container Registry '${NaatiRepositoryName}'."
az acr login --name $NaatiRepositoryName

Write-Output "Use Helm to create the 'naati' package."
helm package naati

Write-Output "Use Helm to create the 'naati' package index"
helm repo index .
$PackageName =(get-childItem naati*.tgz).Name
Write-Host "##vso[task.setvariable variable=HelmVersion;]$($packageName.Replace('naati-', '').Replace('.tgz', ''))"

Write-Output "Pushing the package '${PackageName}' to Azure Container Registry '${NaatiRepositoryName}'"
helm push $PackageName oci://${NaatiRepositoryName}.azurecr.io/helm

Write Output "Setting pipeline variable HelmPackage = ${PackageName}"
Write-Host ("##vso[task.setvariable variable=HelmPackage;]$PackageName")

Pop-Location

return 0;