ALTER FUNCTION [dbo].[GetExaminerComponentMarks]
(
	@testResultId int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		etcr.[Mark]
		,etcr.[TotalMarks]
		,em.[JobExaminerId]
		,etcr.[ComponentNumber]
	FROM [naati_db]..[tblExaminerTestComponentResult] etcr
	INNER JOIN [naati_db]..[tblExaminerMarking] em ON em.[ExaminerMarkingID] = etcr.[ExaminerMarkingID]
	INNER JOIN [naati_db]..[tblTestResult] tr ON tr.[TestResultID] = em.[TestResultID]
	WHERE tr.[TestResultId] = @testResultId
)
