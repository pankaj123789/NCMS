
--:setvar ncms "ncms_cloud"
--:setvar mynaati "ncms_cloud"

-- this script synchronises accounts between myNAATI and NCMS. 
-- it is useful when the NCMS DB has been reset
-- among other things, it will update email addresses in NCMS to match myNAATI. 

-- make sure the myNAATI User table is up to date
INSERT INTO [$(mynaati)]..[tblMyNaatiUser] 
            (NaatiNumber,AspUserId) 
OUTPUT INSERTED.NaatiNumber 'User added'
SELECT NaatiNumber = Min(e.NaatiNumber),u.UserId 
FROM   [$(ncms)]..tblEmail m 
       INNER JOIN [$(ncms)]..tblEntity e 
               ON e.EntityId = m.EntityId 
       INNER JOIN [$(mynaati)]..aspnet_users u 
               ON u.Username COLLATE database_default = m.email COLLATE database_default 
       LEFT JOIN [$(mynaati)]..[tblMyNaatiUser] wu 
              ON u.UserId = wu.AspUserId 
WHERE  wu.UserId IS NULL 
       AND m.IsPreferredEmail = 1 
       AND m.Invalid = 0 
GROUP  BY u.UserId 
ORDER  BY UserId 


-- update NCMS emails to match myNAATI accounts
UPDATE [$(ncms)]..tblEmail 
SET    Email = uu.UserName 
OUTPUT DELETED.Email + '  -->  ' + INSERTED.Email 'NCMS Email update' 
FROM   [$(mynaati)]..aspnet_users uu 
       JOIN [$(mynaati)]..[tblMyNaatiUser] u 
         ON u.AspUserId = uu.UserId 
       JOIN [$(ncms)]..tblEntity e 
         ON e.NaatiNumber = u.NaatiNumber 
WHERE  e.EntityId = [$(ncms)]..tblEmail.EntityId 
       AND [$(ncms)]..tblEmail.IsPreferredEmail = 1 
       AND [$(ncms)]..tblEmail.Email COLLATE database_default <> uu.UserName COLLATE database_default 


-- delete orphan myNAATI accounts
DELETE FROM [$(mynaati)]..aspnet_membership 
OUTPUT DELETED.Email 'Membership deleted' 
WHERE  UserId NOT IN (SELECT UserId 
                      FROM   [$(mynaati)]..aspnet_Users m 
                             LEFT JOIN [$(ncms)]..tblEmail e 
                                    ON e.Email COLLATE database_default = m.UserName COLLATE database_default
                      WHERE  e.IsPreferredEmail = 1) 

DELETE FROM [$(mynaati)]..aspnet_UsersInRoles 
WHERE  UserId NOT IN (SELECT UserId 
                      FROM   [$(mynaati)]..aspnet_Users m 
                             LEFT JOIN [$(ncms)]..tblEmail e 
                                    ON e.Email COLLATE database_default = m.UserName COLLATE database_default
                      WHERE  e.IsPreferredEmail = 1)

					  
DELETE FROM [$(mynaati)]..[tblEmailChange] 
OUTPUT DELETED.PrimaryEmailAddress 'EmailChange deleted' 
WHERE  UserId NOT IN (SELECT UserId 
                         FROM   [$(mynaati)]..aspnet_Users m 
                                LEFT JOIN [$(ncms)]..tblEmail e 
                                       ON e.Email COLLATE database_default = m.UserName COLLATE database_default
                         WHERE  e.IsPreferredEmail = 1)

DELETE FROM [$(mynaati)]..[tblMyNaatiUser] 
OUTPUT DELETED.NaatiNumber 'User deleted' 
WHERE  AspUserId NOT IN (SELECT UserId 
                         FROM   [$(mynaati)]..aspnet_Users m 
                                LEFT JOIN [$(ncms)]..tblEmail e 
                                       ON e.Email COLLATE database_default = m.UserName COLLATE database_default
                         WHERE  e.IsPreferredEmail = 1)

DELETE FROM [$(mynaati)]..aspnet_Users 
OUTPUT DELETED.UserName 'User deleted' 
WHERE  UserId NOT IN (SELECT UserId 
                      FROM   [$(mynaati)]..aspnet_Users m 
                             LEFT JOIN [$(ncms)]..tblEmail e 
                                    ON e.Email COLLATE database_default = m.UserName COLLATE database_default
                      WHERE  e.IsPreferredEmail = 1)


-- mark Persons as IsEportalActive = 1 if they have a myNaatiAccount
UPDATE $(ncms)..tblPerson 
SET    IsEportalActive = 1, WebAccountCreateDate = getdate()
FROM   $(ncms)..tblPerson p 
       JOIN $(ncms)..tblEntity e 
         ON p.EntityId = e.EntityId 
WHERE  naatinumber IN (SELECT NaatiNumber 
                       FROM   $(mynaati)..aspnet_Membership m 
                              JOIN $(mynaati)..[tblMyNaatiUser] u 
                                ON u.AspUserId = m.UserId) 
       AND p.IsEportalActive = 0 


-- mark Persons as IsEportalActive = 0 if they don't have a myNaatiAccount
UPDATE $(ncms)..tblPerson 
SET    IsEportalActive = 0, WebAccountCreateDate = null 
FROM   $(ncms)..tblPerson p 
       JOIN $(ncms)..tblEntity e 
         ON p.EntityId = e.EntityId 
WHERE  naatinumber NOT IN (SELECT NaatiNumber 
                           FROM   $(mynaati)..aspnet_Membership m 
                                  JOIN $(mynaati)..[tblMyNaatiUser] u 
                                    ON u.AspUserId = m.UserId) 
       AND p.IsEportalActive = 1
