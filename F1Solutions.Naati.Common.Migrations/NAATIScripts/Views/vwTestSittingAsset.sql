ALTER VIEW [dbo].[vwTestSittingAsset]
AS
SELECT tblTestSittingDocument.TestSittingDocumentId, 
    tblTestSitting.TestSittingId, 
    tblTestMaterialAttachment.TestMaterialId,
    tblTestMaterialAttachment.TestMaterialAttachmentId,
    tblEntity.NaatiNumber, 
    tblStoredFile.StoredFileId, 
    tblStoredFile.FileName, 
    ISNULL(tblTestSittingDocument.Title, tblTestMaterialAttachment.Title) Title, 
    u1.UserId                               AS UploadedByUserId, 
    u1.FullName                             AS UploadedByUserName, 
    p1.PersonId                             AS UploadedByPersonId, 
    CONCAT(pd1.GivenName, ' ', pd1.Surname) AS UploadedByPersonName, 
	e1.NAATINumber                          AS UploadedByPersonNaatiNo,
    tblStoredFile.UploadedDateTime, 
    tblStoredFile.FileSize, 
	ISNULL(job1.JobId, job2.JobId)			AS JobId,
    tblTestResult.SatDate                   AS TestSatDate, 
    tblEvent.OfficeId                       AS TestOfficeId, 
    ISNULL(tblTestSittingDocument.Deleted, tblTestMaterialAttachment.Deleted) Deleted, 
    tluDocumentType.DisplayName             AS DocumentType,
    tluDocumentType.Name                    AS [Type],
    tblTestSittingDocument.EportalDownload,
    CAST(
        CASE
            WHEN em.CountMarks = 0 THEN 1
            ELSE 0
        END
    AS BIT) ExaminerMarksRemoved,
	em.SubmittedDate
FROM   tblStoredFile
    LEFT JOIN tblTestSittingDocument  
        ON tblStoredFile.StoredFileId = tblTestSittingDocument.StoredFileId 
    LEFT JOIN tluDocumentType
        ON tblStoredFile.DocumentTypeId = tluDocumentType.DocumentTypeId
    LEFT JOIN tblTestMaterialAttachment
        ON tblStoredFile.StoredFileId = tblTestMaterialAttachment.StoredFileId 
    LEFT JOIN tblPerson p1 
        ON p1.PersonId = tblStoredFile.UploadedByPersonId 
	LEFT JOIN tblEntity e1
		ON e1.EntityId = p1.EntityId
    LEFT JOIN vwPersonDistinct pd1 
        ON pd1.EntityId = p1.EntityId 
    LEFT JOIN tblUser u1 
        ON u1.UserId = tblStoredFile.UploadedByUserId 
    LEFT JOIN 
        (tblTestSitting 
        JOIN tblTestEvent 
            ON tblTestEvent.TestEventId = tblTestSitting.TestEventId 
        JOIN tblEvent 
            ON tblEvent.EventId = tblTestEvent.EventId 
        JOIN tblTestInvitation 
            ON tblTestInvitation.TestInvitationId = tblTestSitting.TestInvitationId 
        JOIN tblApplication 
            ON tblApplication.ApplicationId = tblTestInvitation.ApplicationId 
        JOIN tblLanguageExperience 
            ON tblLanguageExperience.LanguageExperienceId = tblApplication.LanguageExperienceId 
        )
        ON tblTestSitting.TestSittingId = tblTestSittingDocument.TestSittingId 
        AND tblTestSitting.Sat = 1
    LEFT JOIN tblPerson 
        ON tblPerson.PersonId = tblLanguageExperience.PersonId 
    LEFT JOIN tblEntity 
        ON tblEntity.EntityId = tblPerson.EntityId 
    LEFT JOIN tblTestResult 
        ON tblTestResult.TestSittingId = tblTestSitting.TestSittingId 
	LEFT JOIN (
		tblExaminerMarking em 
        JOIN tblJobExaminer je on em.JobExaminerID = je.JobExaminerID
        JOIN tblPanelMembership pm on je.PanelMembershipId = pm.PanelMembershipId
	) ON je.JobId = tblTestResult.CurrentJobId
        AND em.TestResultID = tblTestResult.TestResultId
        AND pm.PersonId = p1.PersonId
    LEFT JOIN tbljob job1
        ON (NOT tblTestResult.CurrentJobId IS NULL AND tblTestResult.CurrentJobId = job1.JobId)
    LEFT JOIN tbljob job2
        ON (NOT tblTestMaterialAttachment.TestMaterialId IS NULL AND tblTestMaterialAttachment.TestMaterialId = job2.SettingMaterialId)
WHERE tblStoredFile.DocumentTypeId in (2,3,4,5,6,7)