IF OBJECT_ID('dbo.[TestResultExaminerRubricMarkingHistory]', 'C') IS NOT NULL 
BEGIN
ALTER TABLE [dbo].[TestResultExaminerRubricMarkingHistory]
DROP CONSTRAINT [PK_TestResultExaminerRubricMarkingHistory]
END