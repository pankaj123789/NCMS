ALTER FUNCTION [dbo].[GetExaminerOverallMark]
(
	@testResultId int,
	@jobExaminerId int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		[OverallMark] AS 'ExaminerOverallMark'
		,[TotalMarks] AS 'ExaminerTotalMarks'
	FROM
	(
		SELECT
			tr2.[TestResultId]
			,componentMarks.[JobExaminerId]
			,SUM(componentMarks.[Mark]) AS 'OverallMark'
			,SUM(componentMarks.[TotalMarks]) AS 'TotalMarks'
		FROM [naati_db]..[tblTestResult] tr2
		OUTER APPLY [dbo].[GetExaminerComponentMarks](tr2.[TestResultId]) componentMarks
		GROUP BY [JobExaminerId], [TestResultId]
	) tm
	WHERE [TestResultId] = @testResultId
	AND [JobExaminerId] = @jobExaminerId
)
