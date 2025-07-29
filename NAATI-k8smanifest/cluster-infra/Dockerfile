# Use a Windows base image with PowerShell and OpenSSH
# FROM mcr.microsoft.com/windows/servercore:ltsc2022
FROM mcr.microsoft.com/windows/servercore:ltsc2019

# Install OpenSSH Client
RUN powershell -Command \
    Add-WindowsCapability -Online -Name OpenSSH.Client~~~~0.0.1.0

# Create directories for SSH and scripts
RUN mkdir C:\.ssh; mkdir C:\scripts

# Set working directory (must be absolute path with no space or newline)
WORKDIR C:\
//
ENTRYPOINT ["powershell"]

# Default command (can be overridden)
CMD ["-NoLogo", "-ExecutionPolicy", "Bypass"]
