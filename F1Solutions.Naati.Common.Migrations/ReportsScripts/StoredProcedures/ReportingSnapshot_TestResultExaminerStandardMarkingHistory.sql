ALTER PROCEDURE ReportingSnapshot_TestResultExaminerStandardMarkingHistory
	@Date DateTime -- this is here because of the snapshot runner, it is not used in this stored procedure
-- standard marking
AS

-- Extract

DECLARE @LastRun as DateTime;
DECLARE @NextRun as DateTime;

SELECT @NextRun = GetDate();

SELECT  @LastRun = CONVERT(datetime,[Value], 126) FROM  [naati_db]..tblSystemValue WHERE ValueKey = 'TestResultExaminerStandardMarkingHistory_LastRun'

DROP TABLE IF EXISTS #tmpExtract;

SELECT
testResult.TestResultId,
testSitting.TestSittingId,
entity.NAATINumber,
CONCAT
(
CASE WHEN
title.Title is null
THEN
''
ELSE
CONCAT(title.Title, ' ')
END ,
personName.GivenName, ' ', personName.OtherNames, ' ', personName.Surname
) AS [ExaminerName],
testResult.SatDate AS [TestSatDate],
credentialType.InternalName AS [CredentialType],
CASE WHEN
direction.DisplayName like '%[Language 2]%'
THEN
REPLACE(REPLACE(direction.DisplayName, '[Language 2]', language2.Name), '[Language 1]', language1.Name)
ELSE
REPLACE(direction.DisplayName, '[Language 1]', language1.Name)
END AS [Skill],
examinerMarking.SubmittedDate as [MarksSubmittedDate],
examinerMarking.Comments as [ExaminerComments]
INTO #tmpExtract
FROM
[naati_db]..tblTestResult testResult
inner join [naati_db]..tblTestSitting testSitting on testResult.TestSittingId = testSitting.TestSittingId
inner join [naati_db]..tblJob job on testResult.CurrentJobId = job.JobId
inner join [naati_db]..tblJobExaminer jobExaminer on job.JobId =jobExaminer.JobId
inner join [naati_db]..tblExaminerMarking examinerMarking on jobExaminer.JobExaminerID = examinerMarking.JobExaminerID
inner join [naati_db]..tblPanelMembership panelMembership on jobExaminer.PanelMembershipId = panelMembership.PanelMembershipId
inner join [naati_db]..tblPerson person on panelMembership.PersonId = person.PersonId
inner join [naati_db]..tblEntity entity on person.EntityId = entity.EntityId
inner join [naati_db]..tblPersonName personName on person.PersonId = personName.PersonId
left join [naati_db]..tluTitle title on personName.TitleId = title.TitleId
inner join [naati_db]..tblCredentialRequest credentialRequest on testSitting.CredentialRequestId = credentialRequest.CredentialRequestId
inner join [naati_db]..tblCredentialType credentialType on credentialRequest.CredentialTypeId = credentialType.CredentialTypeId
inner join [naati_db]..tblSkill skill on credentialRequest.SkillId = skill.SkillId
inner join [naati_db]..tblLanguage language1 on skill.Language1Id = language1.LanguageId
inner join [naati_db]..tblLanguage language2 on skill.Language2Id = language2.LanguageId
inner join [naati_db]..tblDirectionType direction on skill.DirectionTypeId = direction.DirectionTypeId
WHERE
credentialRequest.CredentialRequestStatusTypeId = 12 AND  -- Certification Issued
credentialRequest.StatusChangeDate >= @LastRun

-- LOAD

BEGIN TRANSACTION 

DECLARE db_cursor CURSOR FOR 
SELECT * 
FROM #tmpExtract

DECLARE @TestResultId as int;
DECLARE @TestSittingId as int;
DECLARE @NaatiNumber as varchar(10);
DECLARE @ExaminerName as varchar(100);
DECLARE @TestSatDate as DateTime;
DECLARE @CredentialType as varchar(20);
DECLARE @Skill as varchar(40);
DECLARE @MarksSubmittedDate as DateTime;

DECLARE @ExaminerComments as varchar(1000);

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO 
@TestResultId,
@TestSittingId,
@NaatiNumber,
@ExaminerName,
@TestSatDate,
@CredentialType,
@Skill,
@MarksSubmittedDate,
@ExaminerComments

WHILE @@FETCH_STATUS = 0  
BEGIN  
	 INSERT INTO [dbo].[TestResultExaminerStandardMarkingHistory](
	 	TestResultId,
		TestSittingId,
		NaatiNumber,
		ExaminerName,
		TestSatDate,
		CredentialType,
		Skill,
		MarksSubmittedDate,
		ExaminerComments)
		VALUES(
		@TestResultId,
		@TestSittingId,
		@NaatiNumber,
		@ExaminerName,
		@TestSatDate,
		@CredentialType,
		@Skill,
		@MarksSubmittedDate,
		@ExaminerComments)

      FETCH NEXT FROM db_cursor INTO
	    @TestResultId,
		@TestSittingId,
		@NaatiNumber,
		@ExaminerName,
		@TestSatDate,
		@CredentialType,
		@Skill,
		@MarksSubmittedDate,
		@ExaminerComments
END 

CLOSE db_cursor  
DEALLOCATE db_cursor 

-- if everything went OK then set the date for the next run.
UPDATE [naati_db]..tblSystemValue SET [Value] = CONVERT(datetime, @NextRun, 126) where ValueKey = 'TestResultExaminerStandardMarkingHistory_LastRun'

PRINT 'Finished'

COMMIT TRANSACTION;	
