param ( 
        [string]$ConfigurationFileName = $(Throw "Please provide the name of the file that contains the details of the databases to be restored. For example DatabaseSettings.csv"),
		[bool]$hasAvailabilityGroups = $false
    )
	
	#load assemblies
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null

#Need SmoExtended for backup
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoEnum") | Out-Null

Push-Location
. .\SqlFunctions.ps1
Import-Module SqlServer
Pop-Location
	
function New-Database()
{
	param ([string]$DatabaseSourcePath,
		[string]$DatabaseSourceFileName,
		[string]$DatabaseName,
		[string]$DatabaseDestinationServer,
		[string]$TemporaryFolder)

	$application = new-object PSObject

	$application | add-member -type NoteProperty -Name DatabaseSourcePath -Value $DatabaseSourcePath
	$application | add-member -type NoteProperty -Name DatabaseSourceFileName -Value $DatabaseSourceFileName
	$application | add-member -type NoteProperty -Name DatabaseName -Value $DatabaseName
	$application | add-member -type NoteProperty -Name DatabaseDestinationServer -Value $DatabaseDestinationServer
	$application | add-member -type NoteProperty -Name TemporaryFolder -Value $TemporaryFolder
	$application | add-member -type NoteProperty -Name AGName -Value ""
	$application | add-member -type NoteProperty -Name AGSharedPath -Value ""
	$application | add-member -type NoteProperty -Name AGSecondServer -Value ""
    $application | add-member -type NoteProperty -Name TDECertificate -Value ""

	
	return $application
}

function RestoreDatabase {
    param([string]$serverInstance, [string]$destinationDbName, [string]$backupDataFile, [string]$actionType)

    Write-Host "Restoring database..."

    $server = GetServer($serverInstance)
    $server.ConnectionContext.StatementTimeout = 0
    $db = $server.Databases["$destinationDbName"]

    Write-Host "Database: $destinationDbName"
 
    #Create the restore object and set properties
    $restore = new-object ('Microsoft.SqlServer.Management.Smo.Restore')
    $restore.Database = $destinationDbName
    $restore.NoRecovery = $false
    $restore.PercentCompleteNotification = 10
    $restore.Devices.AddDevice($backupDataFile, [Microsoft.SqlServer.Management.Smo.DeviceType]::File)

   # if(-not $db)
  #  {
        Write-Debug "$destinationDbName does not exist..."
  
        #Grab the default MDF & LDF file locations.
        $defaultMdf = $server.Settings.DefaultFile
        $defaultLdf = $server.Settings.DefaultLog

        #If the default locations are the same as the master database, 
        #then those values do not get populated and must be pulled from the MasterPath.
        if($defaultMdf.Length -eq 0)
        {
            $defaultMdf = $server.Information.MasterDBPath
        }
        if($defaultLdf.Length -eq 0)
        {
            $defaultLdf = $server.Information.MasterDBLogPath
        }
        
        if(-not $defaultMdf.EndsWith("\"))
        {
            $defaultMdf = $defaultMdf + "\"
        }
        if(-not $defaultLdf.EndsWith("\"))
        {
            $defaultLdf = $defaultLdf + "\"
        }
          
        $restore.ReplaceDatabase = $True

        #Get the database logical file names            
        try
        {
            $logicalNameDT = $restore.ReadFileList($server)
        }
        catch
        {
            CheckForErrors
        }

        $FileType = ""

        Write-Debug "Restoring $destinationDbName to the following physical locations:"

        foreach($Row in $logicalNameDT)
        {
            # Put the file type into a local variable.
            # This will be the variable that we use to find out which file
            # we are working with.
            $FileType = $Row["Type"].ToUpper()
        
            # If Type = "D", then we are handling the Database File name.
            If($FileType.Equals("D"))
            {
                $dbLogicalName = $Row["LogicalName"]
    
                $targetDbFilePath = $Row["PhysicalName"]
                $position = $targetDbFilePath.LastIndexOf("\") + 1
                $targetDbFilePath = $targetDbFilePath.Substring($position,$targetDbFilePath.Length - $position)
                $targetDbFilePath = $defaultMdf + $targetDbFilePath
    
                if((Test-Path -Path $targetDbFilePath) -eq $true)
                {
                    $targetDbFilePath = $targetDbFilePath -replace $dbLogicalName, $destinationDbName
                }

                $targetDbFilePath = $defaultMdf + $destinationDbName + ".mdf"

                #Specify new data files (mdf and ndf)
                $relocateDataFile = new-object ('Microsoft.SqlServer.Management.Smo.RelocateFile')
                $relocateDataFile.LogicalFileName = $dbLogicalName            
                $relocateDataFile.PhysicalFileName = $targetDbFilePath
                $restore.RelocateFiles.Add($relocateDataFile) | out-null
  
                Write-Host $relocateDataFile.PhysicalFileName
            }
            # If Type = "L", then we are handling the Log File name.
            elseif($FileType.Equals("L"))
            {
                $logLogicalName = $Row["LogicalName"]
    
                $targetLogFilePath = $Row["PhysicalName"]
                $position = $targetLogFilePath.LastIndexOf("\") + 1
                $targetLogFilePath = $targetLogFilePath.Substring($position,$targetLogFilePath.Length - $position)
                $targetLogFilePath = $defaultLdf + $targetLogFilePath

                if((Test-Path -Path $targetLogFilePath) -eq $true)
                {
                    $tempName = $destinationDbName + "_Log"
                    $targetLogFilePath = $targetLogFilePath -replace $logLogicalName, $tempName
                }

                $targetLogFilePath = $defaultLdf + $destinationDbName + "_log.ldf"

                #Specify new log files (ldf)
                $relocateLogFile  = new-object ('Microsoft.SqlServer.Management.Smo.RelocateFile')
                $relocateLogFile.LogicalFileName = $logLogicalName            
                $relocateLogFile.PhysicalFileName = $targetLogFilePath          
                $restore.RelocateFiles.Add($relocateLogFile) | out-null
  
                Write-Host $relocateLogFile.PhysicalFileName
            }          
        }
    #}
   <# else
    {
        Write-Debug "Overwritting existing database..."
  
        #Set recovery model to simple on destination database before restore
        if($db.RecoveryModel -ne [Microsoft.SqlServer.Management.Smo.RecoveryModel]::Simple)
        {
            Write-Debug "Changing recovery model to SIMPLE"
            $db.RecoveryModel = [Microsoft.SqlServer.Management.Smo.RecoveryModel]::Simple
            try
            {
                $db.Alter()
            }
            catch
            {
                CheckForErrors
            }
        }

        #Set destination database to single user mode to kill any active connections
        $db.UserAccess = "Single"
        try
        {
            $db.Alter([Microsoft.SqlServer.Management.Smo.TerminationClause]"RollbackTransactionsImmediately")
        }
        catch
        {
            CheckForErrors
        }
    }
 #>
    if($db -ne $null)
    {
        #Set destination database to single user mode to kill any active connections
        $db.UserAccess = "Single"
        try
        {
            $db.Alter([Microsoft.SqlServer.Management.Smo.TerminationClause]"RollbackTransactionsImmediately")
        }
        catch
        {
            CheckForErrors
        }
    }

    #Do the restore
    try
    {
        $restore.SqlRestore($server)
    }
    catch
    {
        CheckForErrors
    }
 
    #Reload the restored database object
    #$db = $server.Databases["$destinationDbName"]
    
    #Set recovery model to simple on destination database after restore
    #if($db.RecoveryModel -ne [Microsoft.SqlServer.Management.Smo.RecoveryModel]::Simple)
    #{
        #Write-Debug "Changing recovery model to SIMPLE"
        #$db.RecoveryModel = [Microsoft.SqlServer.Management.Smo.RecoveryModel]::Simple
        #try
        #{
            #$db.Alter()
        #}
        #catch
        #{
           # CheckForErrors
        #}
    #}
 
    Write-Host $actionType.ToString() "Restore: OK"
}


function RemoveFromAG($DatabaseServer, $DatabaseName, $AGName, $SecondaryDatabase)
{
    Write-Host "Attempting to remove $DatabaseName from High Availability Group $AGName on $DatabaseServer..."

    Remove-SqlAvailabilityDatabase -Path "SQLSERVER:\SQL\$DatabaseServer\DEFAULT\AvailabilityGroups\$AGName\AvailabilityDatabases\$DatabaseName" -ErrorAction:SilentlyContinue  
}

function AddToAG($SqlServerPrimName, $SqlServerSecName, $SqlAgName, $SqlAgDatabase, $BackupDirectory)
{
    #modified from https://gallery.technet.microsoft.com/scriptcenter/Create-an-AlwaysOn-4f450340
    Write-Host "Attempting to remove existing database from secondary server..."
	# drop the second server DB here. if we do it before the restore it sometimes hasn't transferred out of the group in time
	$query = @"        
            USE [master] 
            GO 
            IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE (name = '$SqlAgDatabase'))) 
                ALTER DATABASE [$SqlAgDatabase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE 
            GO 
            USE [master] 
            IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE (name = '$SqlAgDatabase'))) 
                DROP DATABASE [$SqlAgDatabase] 
            GO            
"@

    $query = @"
            USE [master] 
            IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE (name = '$SqlAgDatabase'))) 
                DROP DATABASE [$SqlAgDatabase]
"@

    Invoke-SqlCmd -Query $query -ServerInstance "$SqlServerSecName"
    Write-Host "Done."

    $SqlServerPrim = New-Object Microsoft.SqlServer.Management.Smo.Server($SqlServerPrimName) 
    $SqlServerSec = New-Object Microsoft.SqlServer.Management.Smo.Server($SqlServerSecName) 
	try { 
 
    Write-Host "Backing up primary database..."
    # backup the database (full database backup) 
    $DbBackup = New-Object Microsoft.SqlServer.Management.Smo.Backup 
    $DbBackup.Database = $SqlAgDatabase 
    $DbBackup.Action = [Microsoft.SqlServer.Management.Smo.BackupActionType]::Database 
    $DbBackup.Initialize = $true 
    $DbBackup.Devices.AddDevice("$BackupDirectory\$($SqlAgDatabase)_AgSetup_full.bak",  
        [Microsoft.SqlServer.Management.Smo.DeviceType]::File) 
    $DbBackup.SqlBackup($SqlServerPrim) 
    Write-Host "Backing up primary database transaction logs..."
    # backup the database (transaction log backup) 
    $DbBackup = New-Object Microsoft.SqlServer.Management.Smo.Backup 
    $DbBackup.Database = $SqlAgDatabase 
    $DbBackup.Action = [Microsoft.SqlServer.Management.Smo.BackupActionType]::Log 
    $DbBackup.Initialize = $true 
    $DbBackup.Devices.AddDevice("$BackupDirectory\$($SqlAgDatabase)_AgSetup_log.trn",  
        [Microsoft.SqlServer.Management.Smo.DeviceType]::File) 
    $DbBackup.SqlBackup($SqlServerPrim) 
 
    Write-Host "Restoring database to secondary..."
    # restore the database (full database restore) 
    $DbRestore = New-Object Microsoft.SqlServer.Management.Smo.Restore 
    $DbRestore.Database = $SqlAgDatabase 
    $DbRestore.Action = [Microsoft.SqlServer.Management.Smo.RestoreActionType]::Database 
    $DbRestore.Devices.AddDevice("$BackupDirectory\$($SqlAgDatabase)_AgSetup_full.bak",  
        [Microsoft.SqlServer.Management.Smo.DeviceType]::File) 
    $DbRestore.NoRecovery = $true 
    $DbRestore.SqlRestore($SqlServerSec) 
    Write-Host "Restoring transaction logs to secondary..."
    # restore the database (transaction log restore) 
    $DbRestore = New-Object Microsoft.SqlServer.Management.Smo.Restore 
    $DbRestore.Database = $SqlAgDatabase 
    $DbRestore.Action = [Microsoft.SqlServer.Management.Smo.RestoreActionType]::Log 
    $DbRestore.Devices.AddDevice("$BackupDirectory\$($SqlAgDatabase)_AgSetup_log.trn",  
        [Microsoft.SqlServer.Management.Smo.DeviceType]::File) 
    $DbRestore.NoRecovery = $true 
    $DbRestore.SqlRestore($SqlServerSec) 
          
    Write-Host "Adding to availability group on primary..."
    Add-SqlAvailabilityDatabase -Path "SQLSERVER:\SQL\$SqlServerPrimName\DEFAULT\AvailabilityGroups\$SqlAgName" -Database "$SqlAgDatabase"

    Write-Host "High availability group configuration complete."
} 
catch { 
    Write-Error $_.Exception 
}
}

function CheckForErrors {
    $errorsReported = $False
    if($Error.Count -ne 0)
    {
  Write-Host
  Write-Host "******************************"
        Write-Host "Errors:" $Error.Count
        Write-Host "******************************"
        foreach($err in $Error)
        {
            $errorsReported  = $True
            if( $err.Exception.InnerException -ne $null)
            {
                Write-Host $err.Exception.InnerException.ToString()
            }
            else
            {
                Write-Host $err.Exception.ToString()
            }
            Write-Host
        }
        throw;
    }
}
 
 function GetServer {
    Param([string]$serverInstance)

    $server = New-Object ("Microsoft.SqlServer.Management.Smo.Server")($serverInstance)
    $server.ConnectionContext.ApplicationName = "AutoDatabaseRefresh"
 $server.ConnectionContext.ConnectTimeout = 5
    $server;
}
 
$CurrentPath = $MyInvocation.MyCommand.Path | Split-Path -Parent
 
$Databases = @()
$DatabaseDetails = Import-CSV (Join-Path $CurrentPath $ConfigurationFileName)

foreach ($DatabaseDetail in $DatabaseDetails)
{
    $Database = New-Database
            
	$Database.DatabaseSourcePath = $DatabaseDetail.DatabaseSourcePath

    if($Database.DatabaseSourcePath.EndsWith("\") -eq $false)
    {
        $Database.DatabaseSourcePath += "\"
    }

	$Database.DatabaseSourceFileName = $DatabaseDetail.DatabaseSourceFileName
	$Database.DatabaseName = $DatabaseDetail.DatabaseName
	$Database.DatabaseDestinationServer = $DatabaseDetail.DatabaseDestinationServer
	$Database.TemporaryFolder = $DatabaseDetail.TemporaryFolder
	$Database.AGName = $DatabaseDetail.AGName
	$Database.AGSharedPath = $DatabaseDetail.AGSharedPath
	$Database.AGSecondServer = $DatabaseDetail.AGSecondServer
    $Database.TDECertificate = $DatabaseDetail.TDECertificate

    $Databases += $Database
    Pop-Location
}

foreach ($Database in $Databases)
{
	Write-Host ''
	Write-Host 'Commencing Database Task...'
	    
    $primaryReplica = $Database.DatabaseDestinationServer 
    $secondaryReplica = ""
	    
	if($hasAvailabilityGroups) 
	{
		$primaryReplica = FindPrimaryReplica $Database.DatabaseDestinationServer,$Database.AGSecondServer $Database.AGName
		
		if($primaryReplica -eq $Database.DatabaseDestinationServer)
		{
			$secondaryReplica = $Database.AGSecondServer
		}
		else
		{
			$secondaryReplica = $Database.DatabaseDestinationServer
		}

		RemoveFromAG $primaryReplica $Database.DatabaseName $Database.AGName $secondaryReplica
	}
		
	$file = $Database.DatabaseSourcePath + $Database.DatabaseSourceFileName
	RestoreDatabase $primaryReplica $Database.DatabaseName $file "Database"

	EnsureServiceBroker $primaryReplica $Database.DatabaseName

    if($Database.TDECertificate -ne "" -and $Database.TDECertificate -ne $null)
    {
        EnsureDatabaseEncryption $primaryReplica $Database.DatabaseName $Database.TDECertificate
    }	
	
	if($hasAvailabilityGroups) 
	{        
		AddToAG $primaryReplica $secondaryReplica $Database.AGName $Database.DatabaseName $Database.AGSharedPath
	}

	Write-Host 'Database Task Complete...'
	Write-Host ''
}