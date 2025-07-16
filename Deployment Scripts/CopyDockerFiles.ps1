param( [string]$outputFolder, [string]$baseImageName, [string]$baseImageNameTag)
$sourceFolder = $env:BUILD_SOURCESDIRECTORY

$ncmsOutputFolder = "$outputFolder\Ncms.Ui\_PublishedWebsites\Ncms.Ui"
$myNaatiOutputFolder = "$outputFolder\MyNaati.Ui\_PublishedWebsites\MyNaati.Ui"


xcopy "$sourceFolder\Naati\Deployment Scripts\Docker\Ncms\DockerFile" "$ncmsOutputFolder\" /s /y
xcopy "$sourceFolder\Naati\Deployment Scripts\Docker\Ncms\.dockerignore" "$ncmsOutputFolder\" /s /y



xcopy "$sourceFolder\Naati\Deployment Scripts\Docker\MyNaati\DockerFile" "$myNaatiOutputFolder\" /s /y
xcopy "$sourceFolder\Naati\Deployment Scripts\Docker\MyNaati\.dockerignore" "$myNaatiOutputFolder\" /s /y

$dockerFiles = @("$myNaatiOutputFolder\DockerFile","$ncmsOutputFolder\DockerFile")

write-host "Replacing base image for docker files"
Foreach ($dockerFile in $dockerFiles )
{   
	$file =$dockerFile;
	$filecontent = Get-Content($file)
	attrib $file -r
	write-host  "Setting version to image $baseImageName with tag $baseImageNameTag to $dockerFile"
	$filecontent -replace "{{NaatiBase}}", $baseImageName | Set-Content -Path  $file

	$filecontent = Get-Content($file)
	attrib $file -r
	$filecontent -replace "{{NaatiBaseTag}}", $baseImageNameTag | Set-Content -Path $file
}
