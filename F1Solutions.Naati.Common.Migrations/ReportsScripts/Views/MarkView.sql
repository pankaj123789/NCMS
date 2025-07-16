ALTER VIEW [dbo].[Mark] AS
SELECT
    [MarkId]
	,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]
	,[PersonId]
    ,[TestSittingId]
    ,[TestResultId]
    ,[ComponentName]
    ,[ExaminerNAATINumber]
    ,[ExaminerName]
    ,[ExaminerType]
    ,[Cost]
    ,[PaperLost]
    ,[PaperReceived]
    ,[SentToPayroll]
    ,[IncludeMarks]
    ,[Mark]
    ,[PassMark]
    ,[TotalMark]
    ,[OverallMark]
    ,[OverallPassMark]
    ,[OverallTotalMark]
    ,[PrimaryFailureReason]
    ,[PoorPerformanceReasons]
    ,[MarkerComments]
    ,[SubmittedDate]	
FROM [MarkLatest] m
--FROM [MarkHistory] m
--where [RowStatus] = 'Latest'