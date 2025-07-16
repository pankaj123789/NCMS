$password = ConvertTo-SecureString "Friday01!" -AsPlainText -Force
Import-PfxCertificate -FilePath ncmstest09.local.pfx -CertStoreLocation Cert:\LocalMachine\My -Password $password
Import-Module WebAdministration
New-WebBinding -Name "Default Web Site" -IP "*" -Port 443 -Protocol https
$cert = (Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object {$_.Subject -match "CN=ncmstest09.local"}).Thumbprint
cd IIS:\SslBindings
Get-Item cert:\LocalMachine\MY\$cert | new-item 0.0.0.0!443