SELECT c.PersonId, 
       c.NAATINumber, 
       c.PractitionerNumber, 
       c.Title, 
       c.GivenName, 
       c.FamilyName, 
       Replace(Replace(c.PrimaryAddress, Char(10), ' '), Char(13), '') AS 'Address', 
       p.Suburb, 
       c.State, 
       c.Postcode, 
       c.Country, 
       c.PrimaryEmail, 
       c.CredentialId, 
       cat.DisplayName                                                 AS 'Application Type', 
       c.CredentialTypeInternalName, 
       c.CredentialTypeExternalName, 
       c.Language1, 
       c.Language1Code, 
       c.Language1Group, 
       c.Language2, 
       c.Language2Code, 
       c.Language2Group, 
       c.DirectionDisplayName, 
       c.StartDate, 
       c.ExpiryDate, 
       c.TerminationDate, 
       c.Status, 
       request.StatusChangeDate 
FROM   [NCMS_Reporting].[dbo].[Credentials] c 
       JOIN [NCMS].[dbo].tblCredentialCredentialRequest ccr 
         ON ccr.CredentialId = c.CredentialId 
       JOIN [NCMS].[dbo].tblCredentialRequest request 
         ON request.CredentialRequestId = ccr.CredentialRequestId 
       JOIN tblCredentialApplication ca 
         ON ca.CredentialApplicationId = request.CredentialApplicationId 
       JOIN tblCredentialApplicationType cat 
         ON cat.CredentialApplicationTypeId = ca.CredentialApplicationTypeId 
       JOIN [NCMS].[dbo].tblCredentialApplicationFieldData cd 
         ON cd.CredentialApplicationId = request.CredentialApplicationId 
       JOIN [NCMS_Reporting].[dbo].[Person] p 
         ON c.PersonId = p.PersonId 
WHERE  cd.CredentialApplicationFieldId IN ( 3, 63 ) -- Products Claimed (transition) or ID Card (recertification) 
       AND cd.Value = 'True' -- Filter for requests where the products claimed is True. 
       AND c.CredentialTypeExternalName LIKE '%Interpreter%' -- Filter for Translator category 
       AND request.StatusChangeDate BETWEEN '$(fromDate)' AND '$(toDate)' -- Date range based on Credential Issued date (to be used in the future to pick up new credentials).
UNION 
SELECT c.PersonId, 
       c.NAATINumber, 
       c.PractitionerNumber, 
       c.Title, 
       c.GivenName, 
       c.FamilyName, 
       Replace(Replace(c.PrimaryAddress, Char(10), ' '), Char(13), '') AS 'Address', 
       p.Suburb, 
       c.State, 
       c.Postcode, 
       c.Country, 
       c.PrimaryEmail, 
       c.CredentialId, 
       cat.DisplayName                                                 AS 'Application Type', 
       c.CredentialTypeInternalName, 
       c.CredentialTypeExternalName, 
       c.Language1, 
       c.Language1Code, 
       c.Language1Group, 
       c.Language2, 
       c.Language2Code, 
       c.Language2Group, 
       c.DirectionDisplayName, 
       c.StartDate, 
       c.ExpiryDate, 
       c.TerminationDate, 
       c.Status, 
       request.StatusChangeDate 
FROM   [NCMS_Reporting].[dbo].[Credentials] c 
       JOIN [NCMS].[dbo].tblCredentialCredentialRequest ccr 
         ON ccr.CredentialId = c.CredentialId 
       JOIN [NCMS].[dbo].tblCredentialRequest request 
         ON request.CredentialRequestId = ccr.CredentialRequestId 
       JOIN tblCredentialApplication ca 
         ON ca.CredentialApplicationId = request.CredentialApplicationId 
       JOIN tblCredentialApplicationType cat 
         ON cat.CredentialApplicationTypeId = ca.CredentialApplicationTypeId 
       JOIN [NCMS_Reporting].[dbo].[Person] p 
         ON c.PersonId = p.PersonId 
WHERE  ca.CredentialApplicationTypeId IN ( 2, 6 ) -- Certification, Certification Practioner 
       AND c.CredentialTypeExternalName LIKE '%Interpreter%' -- Filter for Translator category 
       AND request.StatusChangeDate BETWEEN '$(fromDate)' AND '$(toDate)'-- Date range based on Credential Issued date (to be used in the future to pick up new credentials).
ORDER  BY c.PractitionerNumber 