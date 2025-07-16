Import-Module SqlServer

function RunSqlScript($scriptPath, $databaseServer, $sqlCmdVariables)
{
  $envDetailVariables = @()

  foreach($var in $sqlCmdVariables.GetEnumerator()) {
    $envDetailVariables += "$($var.Name)=$($var.Value)"
  }

  if ($serverVersion -ge 11) {
    Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $databaseServer -Variable $envDetailVariables -QueryTimeout 300
  } else {
    sqlps -Command {
      param ($path, $db, $variables)
      Invoke-Sqlcmd -InputFile "$path" -ServerInstance $db -Variable $variables -QueryTimeout 300
    } -args $scriptPath, $databaseServer, $envDetailVariables
  }
}

function EnsureServiceBroker($databaseServer, $databaseName)
{
    #$sqlcommand="ALTER DATABASE $databaseName SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;"
	$sqlcommand="ALTER DATABASE $databaseName SET NEW_BROKER WITH ROLLBACK IMMEDIATE;"
    Invoke-Sqlcmd -ServerInstance $databaseServer -Database $databaseName -Query $sqlcommand
}

function EnsureDatabaseEncryption($databaseServer, $databaseName, $certificateName)
{
    $sqlcommand="
    IF NOT EXISTS(select
        database_name = d.name,
        dek.encryptor_type,
        cert_name = c.name
        from sys.dm_database_encryption_keys dek
        left join sys.certificates c
        on dek.encryptor_thumbprint = c.thumbprint
        inner join sys.databases d
        on dek.database_id = d.database_id
        WHERE d.name = '$databaseName')
    BEGIN
        CREATE DATABASE ENCRYPTION KEY 
            WITH ALGORITHM = AES_128 
            ENCRYPTION BY SERVER CERTIFICATE ""$certificateName""

        ALTER DATABASE [$databaseName]
	    SET ENCRYPTION ON 
    END
"
    Invoke-Sqlcmd -ServerInstance $databaseServer -Database $databaseName -Query $sqlcommand
}

function FindPrimaryReplica($databases, $haName)
{
    #https://www.mssqltips.com/sqlservertip/3206/finding-primary-replicas-for-sql-server-2012-alwayson-availability-groups-with-powershell/

    ## Setup dataset to hold results
    $dataset = New-Object System.Data.DataSet
    ## populate variable with collection of SQL instances
    ## Setup connection to SQL server inside loop and run T-SQL against instance 
    foreach($Server in $databases) {
		$connectionString = "Data Source=$Server;Initial Catalog=Master;Integrated Security=SSPI;"
		Write-Host "Checking for HA status on: $connectionString"
		## place the T-SQL in variable to be executed by SqlCommand
		$sqlcommand="
			IF SERVERPROPERTY ('IsHadrEnabled') = 1
			BEGIN
			SELECT
			   AGC.name
			 , RCS.replica_server_name
			 , ARS.role_desc
			 , AGL.dns_name
			FROM
			 sys.availability_groups_cluster AS AGC
			  INNER JOIN sys.dm_hadr_availability_replica_cluster_states AS RCS
			   ON
				RCS.group_id = AGC.group_id
			  INNER JOIN sys.dm_hadr_availability_replica_states AS ARS
			   ON
				ARS.replica_id = RCS.replica_id
			  INNER JOIN sys.availability_group_listeners AS AGL
			   ON
				AGL.group_id = ARS.group_id
			WHERE
			 ARS.role_desc = 'PRIMARY'
                        AND
             AGC.name = '$haName'
			END
		"
		## Connect to the data source and open it
		$connection = New-Object System.Data.SqlClient.SqlConnection $connectionString
		$command = New-Object System.Data.SqlClient.SqlCommand $sqlCommand,$connection
		$connection.Open()
		## Execute T-SQL command in variable, fetch the results, and close the connection
		$adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
		#$dataset = New-Object System.Data.DataSet
		[void] $adapter.Fill($dataSet)
		$connection.Close()		
    }

    return $dataSet.Tables[0].Rows[0].Item("replica_server_name")
}