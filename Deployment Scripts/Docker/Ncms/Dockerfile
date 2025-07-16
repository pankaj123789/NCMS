#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat 

FROM {{NaatiBase}}:{{NaatiBaseTag}}
ARG source
WORKDIR /inetpub/wwwroot
RUN Set-Timezone -Name 'AUS Eastern Standard Time'
RUN c:\windows\System32\inetsrv\appcmd.exe unlock config -section:system.webServer/security/authentication/anonymousAuthentication
RUN c:\windows\system32\inetsrv\appcmd.exe set apppool /apppool.name:DefaultAppPool /enable32BitAppOnWin64:false /autoStart:true /startMode:AlwaysRunning
COPY . .
RUN Remove-Item -path NCMSCertificate.ps1 -force
RUN Remove-Item -path ncmstest09.local.pfx -force


RUN icacls "c:\inetpub\wwwroot" /grant IIS_IUSRS:F /t
  