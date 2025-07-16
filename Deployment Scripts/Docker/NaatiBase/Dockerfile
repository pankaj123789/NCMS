#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat 

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
ARG source
WORKDIR /windows/fonts
COPY containerfonts .
RUN import-module webadministration; Set-ItemProperty -Path IIS:\AppPools\DefaultAppPool -Name recycling.periodicRestart.time -Value 0.00:00:00 