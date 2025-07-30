$FONTS = 0x14
$Path=".\ContainerFonts"
$objShell = New-Object -ComObject Shell.Application
$objFolder = $objShell.Namespace($FONTS)

# Check if ContainerFonts directory exists and has files
if (Test-Path $Path) {
    $Fontdir = Get-ChildItem $Path -ErrorAction SilentlyContinue
    if ($Fontdir -and $Fontdir.Count -gt 0) {
        foreach($File in $Fontdir) {
            $objFolder.CopyHere($File.fullname)
        }
        Write-Host "Fonts installed successfully"
    } else {
        Write-Host "No fonts found in ContainerFonts directory - skipping font installation"
    }
} else {
    Write-Host "ContainerFonts directory not found - skipping font installation"
}