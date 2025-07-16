ALTER PROCEDURE [dbo].[ReportingSnapshot_TestResultExaminerRubricMarkingHistory]
	@Date DateTime -- this is here because of the snapshot runner, it is not used in this stored procedure 
-- rubric marking
AS

-- Extract

DECLARE @LastRun as DateTime;
DECLARE @NextRun as DateTime;

SELECT @NextRun = GetDate();

SELECT @LastRun = CONVERT(datetime, [Value], 126) --126 here is ISO 8601 format for date which is what is used in system value table
FROM  
	[naati_db]..tblSystemValue 
WHERE 
	ValueKey = 'TestResultExaminerRubricMarkingHistory_LastRun'


DROP TABLE IF EXISTS #tmpExtract PRINT 'Dropped #tmpExtract';

SELECT
	testResult.TestResultId,
	testSitting.TestSittingId,
	entity.NAATINumber,
	[ExaminerComments].ExaminerName,
	CASE WHEN
	testResult.SatDate IS NOT NULL
	THEN 
	testResult.SatDate
	ELSE
	testSession.TestDateTime
	END
	as [TestDateTime],
	credentialType.InternalName AS [CredentialType],
	CASE WHEN
	direction.DisplayName like '%[Language 2]%'
	THEN
	REPLACE(REPLACE(direction.DisplayName, '[Language 2]', language2.Name), '[Language 1]', language1.Name)
	ELSE
	REPLACE(direction.DisplayName, '[Language 1]', language1.Name)
	END AS [Skill],
	[ExaminerComments].ExaminerReceivedDate as [MarksReceivedDate],
	testComponentType.Name as [TestComponentType],
	competency.Name as [Competency],
	criterion.Name as [Criteria],
	band.Level,
	[ExaminerComments].Comments,
	[ExaminerComments].RubricAssessementCriterionResultId
INTO #tmpExtract
FROM
	(
		SELECT
			criterionResult.Comments,
			criterionResult.RubricAssessementCriterionResultId,
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
			criterionResult.RubricMarkingAssessmentCriterionId,
			criterionResult.RubricMarkingBandId,
			componentResult.RubricTestComponentResultId,
			jobExaminer.JobId,
			person.EntityId,
			jobExaminer.ExaminerReceivedDate,
			criterionResult.CommentDate
		FROM
			[naati_db]..tblRubricAssessementCriterionResult criterionResult
			INNER JOIN [naati_db]..tblRubricTestComponentResult componentResult on criterionResult.RubricTestComponentResultId = componentResult.RubricTestComponentResultId
			INNER JOIN [naati_db]..tblJobExaminerRubricTestComponentResult jobExaminerComponentResult on componentResult.RubricTestComponentResultId = jobExaminerComponentResult.RubricTestComponentResultId
			INNER JOIN [naati_db]..tblJobExaminer jobExaminer on jobExaminerComponentResult.JobExaminerId = jobExaminer.JobExaminerID
			INNER JOIN [naati_db]..tblPanelMembership panelMembership on jobExaminer.PanelMembershipId = panelMembership.PanelMembershipId
			INNER JOIN [naati_db]..tblPerson person on panelMembership.PersonId = person.PersonId
			INNER JOIN [naati_db]..vwDistinctPersonName latestPerson on person.PersonId = latestPerson.PersonId
			INNER JOIN [naati_db]..tblPersonName personName on latestPerson.PersonNameId = personName.PersonNameId
			LEFT JOIN [naati_db]..tluTitle title on personName.TitleId = title.TitleId
		WHERE
			componentResult.WasAttempted = 1
	) as [ExaminerComments]
	INNER JOIN [naati_db]..tblRubricMarkingBand band on [ExaminerComments].RubricMarkingBandId = band.RubricMarkingBandId
	INNER JOIN [naati_db]..tblRubricMarkingAssessmentCriterion criterion on band.RubricMarkingAssessmentCriterionId = criterion.RubricMarkingAssessmentCriterionId
	INNER JOIN [naati_db]..tblRubricMarkingCompetency competency on criterion.RubricMarkingCompetencyId = competency.RubricMarkingCompetencyId
	INNER JOIN [naati_db]..tblTestComponentType testComponentType on competency.TestComponentTypeId = testComponentType.TestComponentTypeId
	INNER JOIN [naati_db]..tblTestResult testResult on [ExaminerComments].JobId = testResult.CurrentJobId
	INNER JOIN [naati_db]..tblTestSitting testSitting on testResult.TestSittingId = testSitting.TestSittingId
	INNER JOIN [naati_db]..tblCredentialRequest credentialRequest on testSitting.CredentialRequestId = credentialRequest.CredentialRequestId
	INNER JOIN [naati_db]..tblCredentialType credentialType on credentialRequest.CredentialTypeId = credentialType.CredentialTypeId
	INNER JOIN [naati_db]..tblSkill skill on credentialRequest.SkillId = skill.SkillId
	INNER JOIN [naati_db]..tblLanguage language1 on skill.Language1Id = language1.LanguageId
	INNER JOIN [naati_db]..tblLanguage language2 on skill.Language2Id = language2.LanguageId
	INNER JOIN [naati_db]..tblDirectionType direction on skill.DirectionTypeId = direction.DirectionTypeId
	INNER JOIN [naati_db]..tblEntity entity on [ExaminerComments].EntityId = entity.EntityId
	INNER JOIN [naati_db]..tblTestSession testSession on testSitting.TestSessionId = testSession.TestSessionId
WHERE
	[ExaminerComments].CommentDate >= @LastRun
ORDER By [ExaminerComments].RubricAssessementCriterionResultId

select * from #tmpExtract

DECLARE @SelectedCount int;

SELECT @SelectedCount = COUNT(*) FROM #tmpExtract

PRINT 'SELECTED ' + CONVERT(nvarchar, @SelectedCount) + ' to move into reporting table';
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
DECLARE @CredentialType as varchar(50);
DECLARE @Skill as varchar(40);
DECLARE @MarksReceivedDate as DateTime;
DECLARE @TestComponentType as varchar(20);
DECLARE @Competency as varchar(20);
DECLARE @Criteria as varchar(100);
DECLARE @Band as varchar(20);
DECLARE @ExaminerComments as varchar(1000);
DECLARE @RubricAssessmentCriterionResultId as int;
DECLARE @DetectUpdate as int;

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO 
@TestResultId,
@TestSittingId,
@NaatiNumber,
@ExaminerName,
@TestSatDate,
@CredentialType,
@Skill,
@MarksReceivedDate,
@TestComponentType,
@Competency,
@Criteria,
@Band,
@ExaminerComments,
@RubricAssessmentCriterionResultId

PRINT CONCAT('FETCHED RubricAssessmentCriterionResultId: ', @RubricAssessmentCriterionResultId)

WHILE @@FETCH_STATUS = 0  
BEGIN  
		SELECT
			@DetectUpdate = COUNT(*)
		FROM
			[TestResultExaminerRubricMarkingHistory]
		WHERE 
			RubricAssessementCriterionResultId = @RubricAssessmentCriterionResultId

		IF @DetectUpdate = 0
			BEGIN
				PRINT CONCAT('INSERTING Rubric AssessmentCriterionResultId: ', @RubricAssessmentCriterionResultId)
				INSERT INTO [dbo].[TestResultExaminerRubricMarkingHistory](
	 				TestResultId,
					TestSittingId,
					NaatiNumber,
					ExaminerName,
					TestSatDate,
					CredentialType,
					Skill,
					MarksReceivedDate,
					TestComponentType,
					Competency,
					Criteria,
					Band,
					ExaminerComments,
					RubricAssessementCriterionResultId)
					VALUES(
					@TestResultId,
					@TestSittingId,
					@NaatiNumber,
					@ExaminerName,
					@TestSatDate,
					@CredentialType,
					@Skill,
					@MarksReceivedDate,
					@TestComponentType,
					@Competency,
					@Criteria,
					@Band,
					@ExaminerComments,
					@RubricAssessmentCriterionResultId)
			END
		ELSE
			BEGIN
				PRINT CONCAT('UPDATING RubricAssessmentCriterionResultId: ', @RubricAssessmentCriterionResultId)
				UPDATE 
					[dbo].[TestResultExaminerRubricMarkingHistory]
				SET	
					TestResultId = @TestResultId,
					TestSittingId = @TestSittingId,
					NaatiNumber = @NaatiNumber,
					ExaminerName = @ExaminerName,
					TestSatDate = @TestSatDate,
					CredentialType = @CredentialType,
					Skill = @Skill,
					MarksReceivedDate = @MarksReceivedDate,
					TestComponentType = @TestComponentType,
					Competency = @Competency,
					Criteria = @Criteria,
					Band = @Band,
					ExaminerComments = @ExaminerComments
				WHERE 
				RubricAssessementCriterionResultId = @RubricAssessmentCriterionResultId
			END
	 
	 FETCH NEXT FROM db_cursor INTO 
		@TestResultId,
		@TestSittingId,
		@NaatiNumber,
		@ExaminerName,
		@TestSatDate,
		@CredentialType,
		@Skill,
		@MarksReceivedDate,
		@TestComponentType,
		@Competency,
		@Criteria,
		@Band,
		@ExaminerComments,
		@RubricAssessmentCriterionResultId

	PRINT CONCAT('FETCHED RubricAssessmentCriterionResultId: ', @RubricAssessmentCriterionResultId)
END 

CLOSE db_cursor  
DEALLOCATE db_cursor 

-- if everything went OK then set the date for the next run.
UPDATE [naati_db]..tblSystemValue SET [Value] = CONVERT(datetime, @NextRun, 126) where ValueKey = 'TestResultExaminerRubricMarkingHistory_LastRun'
PRINT CONCAT('UPDATED SYSTEM VALUE TestResultExaminerRubricMarkingHistory_LastRun to ', CONVERT(datetime, @NextRun, 126))

PRINT 'FINISHED'

COMMIT TRANSACTION;