$FONTS = 0x14
$Path=".\ContainerFonts"
$objShell = New-Object -ComObject Shell.Application
$objFolder = $objShell.Namespace($FONTS)
$Fontdir = dir $Path

foreach($File in $Fontdir) 
{
	$objFolder.CopyHere($File.fullname)
}