ALTER FUNCTION [dbo].[GetTestResultOverallMark]
(
	@testResultId int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		SUM([Mark]) AS 'TestResultOverallMark'
	FROM [naati_db]..[tblTestComponentResult]
	WHERE [TestResultId] = @testResultId
	GROUP BY [TestResultId]
)
