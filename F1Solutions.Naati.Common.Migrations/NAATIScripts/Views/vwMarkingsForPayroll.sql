CREATE VIEW [dbo].[vwMarkingsForPayroll]
AS
SELECT
    u1.FullName									AS ProductSpecificationChangedUser
    ,ct.InternalName							AS TestType
    ,REPLACE(REPLACE(d.DisplayName, '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) AS [Language]
    ,Surname + ', ' + GivenName					AS Examiner
    ,pm.PersonId								AS ExaminerPersonId
    ,ent.EntityId								AS ExaminerEntityId
    ,ent.NaatiNumber							AS ExaminerNaatiNumber
    ,ent.AccountNumber							AS ExaminerAccountNumber
    ,je.ExaminerReceivedDate					AS ResultReceivedDate
    ,je.ExaminerPaperReceivedDate				AS PaperReceivedDate
    ,ts.TestSittingId							AS TestAttendanceId
    ,glc.Code									AS GlCode
    ,ps.ProductSpecificationId
    ,ps.Code									AS ProductSpecificationCode
    ,je.ExaminerCost
    ,je.JobExaminerID
    ,je.ValidatedDate
    ,u2.FullName								AS ValidatedUser
    ,pay.PayrollId
    ,jep.AccountingReference
    ,pay.ModifiedDate							AS PayrollModifiedDate
    ,u3.FullName								AS PayrollModifiedUser
    ,CASE
        WHEN oin.Abbreviation = '' THEN oin.Name
        ELSE oin.Abbreviation
    END Office
    ,je.PaidReviewer
    ,jeps.[Name]								AS PayrollStatus
    ,jeps.PayrollStatusId
	,ts.Supplementary								AS Supplementary
FROM 
    tblJobExaminer je
    JOIN tblPanelMembership pm ON pm.PanelMembershipId = je.PanelMembershipId
    JOIN tblPerson p ON p.PersonId = pm.PersonId
    JOIN vwDistinctPersonName dpn ON dpn.PersonId = p.PersonId
    JOIN tblPersonName pn ON pn.PersonNameId = dpn.PersonNameId
    JOIN tblEntity ent ON ent.EntityId = p.EntityId
    LEFT JOIN tblProductSpecification ps ON ps.ProductSpecificationId = je.ProductSpecificationId
    LEFT JOIN tblGLCode glc ON glc.GLCodeId = ps.GLCodeId
    JOIN tblTestResult tr ON tr.CurrentJobId = je.JobId
    JOIN tblExaminerMarking em ON em.JobExaminerID = je.JobExaminerID AND em.TestResultID = tr.TestResultId
    LEFT JOIN tblUser u1 ON u1.UserId = je.ProductSpecificationChangedUserId
    LEFT JOIN tblUser u2 ON u2.UserId = je.ValidatedUserId
    LEFT JOIN tblJobExaminerPayroll jep ON jep.JobExaminerID = je.JobExaminerId
    LEFT JOIN tblPayroll pay ON pay.PayrollId = jep.PayrollId
    LEFT JOIN tblUser u3 ON u3.UserId = pay.ModifiedUserId
    JOIN vwJobExaminerPayrollStatus jeps ON je.JobExaminerID = jeps.JobExaminerId
	JOIN tblTestSitting ts ON tr.TestSittingId = ts.TestSittingId
	JOIN tblTestSession tss ON ts.TestSessionId = tss.TestSessionId
	JOIN tblVenue v ON tss.VenueId = v.VenueId
	JOIN tblTestLocation tl ON v.TestLocationId = tl.TestLocationId
	JOIN tblCredentialRequest cr ON ts.CredentialRequestId = cr.CredentialRequestId
	JOIN tblCredentialType ct ON cr.CredentialTypeId = ct.CredentialTypeId
	JOIN tblSkill s ON cr.SkillId = s.SkillId
	JOIN tblDirectionType d ON s.DirectionTypeId = d.DirectionTypeId
    LEFT JOIN tblLanguage l1 ON l1.LanguageId = s.Language1Id
    LEFT JOIN tblLanguage l2 ON l2.LanguageId = s.Language2Id
    LEFT JOIN tblOffice o ON o.OfficeId = tl.OfficeId
    LEFT JOIN tblInstitution i on o.InstitutionId = i.InstitutionId
    LEFT JOIN vwDistinctInstitutionName din on i.InstitutionId = din.InstitutionId
    LEFT JOIN tblInstitutionName oin on din.InstitutionNameId = oin.InstitutionNameId
WHERE je.ExaminerReceivedDate IS NOT NULL AND ExaminerToPayrollDate IS NULL