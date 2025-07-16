ALTER PROCEDURE [dbo].[ReportingSnapshot_Mark_Latest]
AS
BEGIN
	
	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'MarkLatest')
		BEGIN
			SELECT * INTO MarkLatest FROM MarkHistory WHERE RowStatus = 'Latest'
		END
	ELSE
		BEGIN	
	
	BEGIN TRANSACTION 
		MERGE MarkLatest AS Target USING(	select * from MarkHistory where [RowStatus] = 'Latest' ) AS Source ON Target.[MarkId] = Source.[MarkId] AND Target.RowStatus = 'Latest' 
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.PersonId = Source.PersonId
		  ,Target.TestSittingId = Source.TestSittingId		  
		  ,Target.ExaminerNAATINumber = Source.ExaminerNAATINumber
		  ,Target.ExaminerName = Source.ExaminerName
		  ,Target.IncludeMarks = Source.IncludeMarks      
		  ,Target.MarkId = Source.MarkId
		  ,Target.Mark = Source.Mark
		  ,Target.TotalMark = Source.TotalMark
		  ,Target.OverallMark = Source.OverallMark

		  ,Target.TestResultId = Source.TestResultId
		  ,Target.ComponentName = Source.ComponentName
		  ,Target.ExaminerType = Source.ExaminerType
		  ,Target.Cost = Source.Cost
		  ,Target.PaperLost = Source.PaperLost
		  ,Target.PaperReceived = Source.PaperReceived
		  ,Target.SentToPayroll = Source.SentToPayroll
		  ,Target.PassMark = Source.PassMark
		  ,Target.OverallPassMark = Source.OverallPassMark
		  ,Target.OverallTotalMark = Source.OverallTotalMark

		  ,Target.PrimaryFailureReason = Source.PrimaryFailureReason
		  ,Target.PoorPerformanceReasons = Source.PoorPerformanceReasons
		  ,Target.MarkerComments = Source.MarkerComments
		  ,Target.SubmittedDate = Source.SubmittedDate


		  ,Target.[RowStatus] = 'Latest'

		WHEN NOT MATCHED THEN 
		 INSERT([PersonId],[TestSittingId],[ModifiedDate],[ExaminerNAATINumber],[ExaminerName],[IncludeMarks],[MarkId],[Mark],[TotalMark],[OverallMark],[TestResultId],[ComponentName],[ExaminerType],
			 [Cost],[PaperLost],[PaperReceived],[SentToPayroll],[PassMark],[OverallPassMark],[OverallTotalMark],[PrimaryFailureReason],[PoorPerformanceReasons],[MarkerComments],[SubmittedDate],[RowStatus])	  
		 VALUES (Source.[PersonId],Source.[TestSittingId],GetDate(),Source.[ExaminerNAATINumber],Source.[ExaminerName],Source.[IncludeMarks],Source.[MarkId],Source.[Mark],Source.[TotalMark],Source.[OverallMark],Source.[TestResultId],Source.[ComponentName],Source.[ExaminerType],
			  Source.[Cost],Source.[PaperLost],Source.[PaperReceived],Source.[SentToPayroll],Source.[PassMark],Source.[OverallPassMark],Source.[OverallTotalMark],Source.[PrimaryFailureReason],Source.[PoorPerformanceReasons],Source.[MarkerComments],Source.[SubmittedDate], 'Latest');
	    	
	COMMIT TRANSACTION;
	END

END