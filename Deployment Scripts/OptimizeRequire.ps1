function BuildMainDist($projectName) 
{
	Write-Host "Building main.dist.js for '$projectName' project"

	$sourcesFolder = $env:BUILD_SOURCESDIRECTORY

	$projectFolder = "$sourcesFolder\Naati\$projectName\App"
	$mainDistJs = "$projectFolder\main.dist.js"
	Set-ItemProperty $mainDistJs IsReadOnly $false
	$backDir = $pwd
	cd $projectFolder
	$messages = (& "C:\Program Files\nodejs\node.exe" .\build.js)

	if($messages -ne $null) {
		#if node build failed, make the build not succeed by using Write-Error
	
		$Host.UI.WriteErrorLine("Failed to build main.dist.js")
		$message = [String]::Join([Environment]::NewLine, $messages)
		$Host.UI.WriteErrorLine($message)
	}
	else {
		Write-Host "Successfully built"
	}

	Set-ItemProperty $mainDistJs IsReadOnly $true
	cd $backDir
}

BuildMainDist('MyNaati.Ui')
BuildMainDist('Ncms.Ui')