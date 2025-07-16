/* 
usage:
 - must be run in SQLCMD mode
 - set the variables at the top as required
 - update the WHERE clause as required
 - ensure the folder matching outputFolder exists
 - ensure you have placed the ExportPersonImages.fmt file in the location matching formatFile
 - if you have lost the ExportPersonImages.fmt file, create a text file containing the following four lines:
11.0
1
1       SQLIMAGE            0       0       ""   1     Photo                                        ""

(the last line is blank)

notes:
 - user will probably need administrative level permissions to run this script
 - this script reconfigures the DB to allow master.dbo.xp_cmdshell execution. on completion, the DB will be reconfigured again to disallow master.dbo.xp_cmdshell execution.
 - customer photos are written to the file system. after the photos have been delivered, please delete them permanently from the file system.
*/

:SETVAR ncmsDB "NCMS"
:SETVAR ncmsReportingDB "NCMS_REPORTING"
:SETVAR outputFolder "E:\NcmsPersonImages"
:SETVAR formatFile "c:\scripts\ExportPersonImages.fmt"
--:SETVAR fromDate "2017-01-01"
--:SETVAR toDate "2017-12-31"
:SETVAR whereClause "(CredentialTypeInternalName LIKE '%interpreter%')"

EXEC sp_configure 'show advanced options', 1
RECONFIGURE
EXEC sp_configure 'xp_cmdshell', 1
RECONFIGURE

USE $(ncmsDB)

DECLARE person_cursor CURSOR FOR 
  (SELECT PractitionerNumber 
   FROM   tblPersonImage pi
          JOIN $(ncmsReportingDB).dbo.Credentials c 
            ON pi.PersonId = c.PersonId 
          JOIN tblCredentialCredentialRequest ccr 
            ON ccr.CredentialId = c.CredentialId 
          JOIN tblCredentialRequest cr 
            ON cr.CredentialRequestId = ccr.CredentialRequestId 
          JOIN tblCredentialApplicationFieldData cd 
            ON cd.CredentialApplicationId = cr.CredentialApplicationId 
   WHERE  cd.CredentialApplicationFieldId in (3,63) -- Products Claimed (Transition) or ID Card (Recertification)
      AND cd.Value = 'True' -- Filter for requests where the products claimed is True. 
      AND cr.StatusChangeDate > '$(fromDate)'
      AND cr.StatusChangeDate <= '$(toDate)' -- Date range based on Credential Issued date (to be used in the future to pick up new credentials).
      AND $(whereClause)
	  
UNION
SELECT PractitionerNumber 
   FROM   tblPersonImage pi
          JOIN $(ncmsReportingDB).dbo.Credentials c 
            ON pi.PersonId = c.PersonId 
          JOIN tblCredentialCredentialRequest ccr 
            ON ccr.CredentialId = c.CredentialId 
          JOIN tblCredentialRequest cr 
            ON cr.CredentialRequestId = ccr.CredentialRequestId 
          JOIN tblCredentialApplication ca 
            ON ca.CredentialApplicationId = cr.CredentialApplicationId 
   WHERE  ca.CredentialApplicationTypeId in (2,6) -- Certification, Certification Practioner
      AND cr.StatusChangeDate > '$(fromDate)'
      AND cr.StatusChangeDate <= '$(toDate)' -- Date range based on Credential Issued date (to be used in the future to pick up new credentials).
      AND $(whereClause))  
   
DECLARE @cn VARCHAR(9); 

OPEN person_cursor 

FETCH NEXT FROM person_cursor INTO @cn 

WHILE ( @@FETCH_STATUS <> -1 ) 
  BEGIN 
      DECLARE @outputFile VARCHAR(200) = '$(outputFolder)\' + @cn + '.jpg' 
      DECLARE @sql VARCHAR(500) = 'SELECT Photo FROM $(ncmsDB).dbo.tblPersonImage i JOIN $(ncmsDB).dbo.tblPerson p ON p.PersonId = i.PersonId WHERE PractitionerNumber = ''' + @cn + '''' 
      DECLARE @bcp VARCHAR(500) = 'BCP "' + @sql + '" QUERYOUT "' + @outputFile + '" -T -f $(formatFile) -S ' + @@SERVERNAME 

      EXEC master.dbo.xp_cmdshell @bcp 

      FETCH NEXT FROM person_cursor INTO @cn 
  END 

CLOSE person_cursor 

DEALLOCATE person_cursor 

EXEC sp_configure 'xp_cmdshell', 0
EXEC sp_configure 'show advanced options', 0
RECONFIGURE
