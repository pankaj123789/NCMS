#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat 

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
ARG source
WORKDIR /inetpub/wwwroot
RUN c:\windows\System32\inetsrv\appcmd.exe unlock config -section:system.webServer/security/authentication/anonymousAuthentication
RUN c:\windows\System32\inetsrv\appcmd.exe unlock config -section:system.webServer/security/authentication/windowsAuthentication
RUN c:\windows\system32\inetsrv\appcmd.exe set apppool /apppool.name:DefaultAppPool /enable32BitAppOnWin64:true /autoStart:true /startMode:AlwaysRunning
COPY . .
RUN icacls "c:\inetpub\wwwroot" /grant IIS_IUSRS:F /t
