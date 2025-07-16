Finding duplicates in the CredentialApplicationActionBuilderHelper:

Steps Taken:
1. Opened CredentialApplicationActionBuilderHelper.cs

2. Created SQL script to reflect the created EmailTemplateBuilderExtensions i.e Select action, event, application type, email template detail type and count occurences in database, then filter where the occurence is more than 1

3. Manually compared the sql script results with the EmailTemplateBuilderExtensions by ctrl + f in the cs file for each SystemActionTypeName in the sql table, and comparing sql results table columns with relevant fields for any found SystemActionTypeName in cs file

4. Only found 1 SystemActionTypeName in cs file 'SubmitApplication', and found a duplicate in submitApplicationNoneEvent50 and submitApplicationNoneEvent51

5. Added SQL script to SQL file FindDuplicateCredentialApplicationAction.sql for future reference

6. Created and added this README.txt for future reference
