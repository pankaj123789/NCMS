ALTER TABLE [tblTestSitting] ADD AllocatedDate DATETIME NULL;
ALTER TABLE [tblTestSitting] ADD RejectedDate DATETIME NULL;

GO 

WITH AllocatedCte(TestSittingId, NoteDate)
AS(
SELECT ts.TestsittingId, MAX(n.CreatedDate)  
FROM tblTestSitting ts
INNER JOIN tblCredentialRequest cr ON cr.CredentialRequestId = ts.CredentialRequestId
INNER JOIN tblCredentialType ct ON ct.CredentialTypeId  = cr.CredentialTypeId
INNER JOIN tblSkill sk ON sk.SkillId = cr.SkillId 
INNER JOIN tblLanguage l1 ON sk.Language1Id = l1.LanguageId
INNER JOIN tblLanguage l2 ON sk.Language2Id = l2.LanguageId
INNER JOIN tblDirectionType dt ON dt.DirectionTypeId = sk.DirectionTypeId
INNER JOIN tblCredentialApplicationNote can ON can.CredentialApplicationId = cr.CredentialApplicationId
INNER JOIN tblNote n ON n.NoteId = can.NoteId AND n.Description LIKE CONCAT('The credential request ', ct.InternalName, ' ', REPLACE(REPLACE(dt.DisplayName, '[Language 1]', l1.Name),'[Language 2]', l2.Name), ' has been allocated to Test Session TS', ts.TestSessionId,'%')
GROUP BY ts.TestSittingId
)
UPDATE testSitting
SET testSitting.allocatedDate = acte.NoteDate
FROM  tblTestSitting testSitting 
INNER JOIN AllocatedCte acte ON acte.TestSittingId = testSitting.TestSittingId

GO

UPDATE testSitting
SET allocatedDate = cr.StatusChangeDate
FROM tblTestSitting  testSitting
INNER JOIN tblCredentialRequest cr ON cr.CredentialRequestId = testSitting.CredentialRequestId
WHERE allocatedDate IS NULL

GO

WITH RejectedCte(TestSittingId, NoteDate)
AS(
SELECT ts.TestsittingId,max(n.CreatedDate)  FROM tblTestSitting ts
INNER JOIN tblCredentialRequest cr ON cr.CredentialRequestId = ts.CredentialRequestId
INNER JOIN tblCredentialType ct ON ct.CredentialTypeId  = cr.CredentialTypeId
INNER JOIN tblSkill sk ON sk.SkillId = cr.SkillId
INNER JOIN tblLanguage l1 ON sk.Language1Id = l1.LanguageId
INNER JOIN tblLanguage l2 ON sk.Language2Id = l2.LanguageId
INNER JOIN tblDirectionType dt ON dt.DirectionTypeId = sk.DirectionTypeId
INNER JOIN tblCredentialApplicationNote can ON can.CredentialApplicationId = cr.CredentialApplicationId
INNER JOIN tblNote n ON n.NoteId = can.NoteId AND n.Description LIKE CONCAT('The credential request ', ct.InternalName, ' ', REPLACE(REPLACE(dt.DisplayName, '[Language 1]', l1.Name),'[Language 2]', l2.Name), ' has been removed from Test Session TS', ts.TestSessionId,'%')
WHERE ts.Rejected = 1
GROUP BY ts.TestSittingId
)
UPDATE testSitting
SET testSitting.RejectedDate = cte.NoteDate
FROM  tblTestSitting testSitting
INNER JOIN RejectedCte cte ON cte.TestSittingId = testSitting.TestSittingId

GO

UPDATE testSitting
SET rejectedDate = cr.StatusChangeDate
FROM tblTestSitting  testSitting
INNER JOIN tblCredentialRequest cr ON cr.CredentialRequestId = testSitting.CredentialRequestId
WHERE rejectedDate IS NULL AND Rejected = 1

ALTER TABLE [tblTestSitting] ALTER COLUMN AllocatedDate DATETIME NOT NULL;