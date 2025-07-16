ALTER PROCEDURE [dbo].[ReportingSnapshot_MarkRubric_Latest]
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'MarkRubricLatest')
		BEGIN
			SELECT * INTO MarkRubricLatest FROM MarkRubricHistory WHERE RowStatus = 'Latest'
		END
	ELSE
		BEGIN
		BEGIN TRANSACTION
			MERGE MarkRubricLatest AS Target USING(select * from MarkRubricHistory where [RowStatus] = 'Latest') AS Source ON (Target.[TestResultId] = Source.[TestResultId] AND Target.[TestComponentId] = Source.[TestComponentId] AND Target.[JobExaminerId] = Source.[JobExaminerId] AND Target.[RubricAssessementCriterionResultId] = Source.[RubricAssessementCriterionResultId] AND Target.RowStatus = 'Latest')
			WHEN MATCHED THEN 
			  UPDATE Set     
		       Target.TestResultId = Source.TestResultId
			  ,Target.TestComponentId = Source.TestComponentId
			  ,Target.JobExaminerId = Source.JobExaminerId
			  ,Target.RubricAssessementCriterionResultId = Source.RubricAssessementCriterionResultId			  
			  ,Target.TestSittingId = Source.TestSittingId      
			  ,Target.TestSessionId = Source.TestSessionId
			  ,Target.PersonId = Source.PersonId
			  ,Target.CustomerNo = Source.CustomerNo
			  ,Target.CandidateName = Source.CandidateName
	  
			  ,Target.PaidReview = Source.PaidReview
			  ,Target.Supplementary = Source.Supplementary
			  ,Target.TestTaskTypeLabel = Source.TestTaskTypeLabel
			  ,Target.TestTaskTypeName = Source.TestTaskTypeName
			  ,Target.TestTaskLabel = Source.TestTaskLabel
			  ,Target.TestTaskName = Source.TestTaskName
			  ,Target.TestTaskNumber = Source.TestTaskNumber
			  ,Target.MarkType = Source.MarkType
			  ,Target.ResultType = Source.ResultType
			  ,Target.ExaminerCustomerNo = Source.ExaminerCustomerNo
			  ,Target.ExaminerName = Source.ExaminerName
			  ,Target.ExaminerType = Source.ExaminerType
	  
			  ,Target.Cost = Source.Cost
			  ,Target.ExaminerSubmittedDate = Source.ExaminerSubmittedDate
			  ,Target.IncludeMarks = Source.IncludeMarks
			  ,Target.WasAttempted = Source.WasAttempted
			  ,Target.Successful = Source.Successful
			  ,Target.RubricCompetencyLabel = Source.RubricCompetencyLabel
			  ,Target.RubricCompetencyName = Source.RubricCompetencyName
			  ,Target.RubricCriterionName = Source.RubricCriterionName
			  ,Target.RubricCriterionLabel = Source.RubricCriterionLabel
	  
			  ,Target.RubricSelectedBandLabel = Source.RubricSelectedBandLabel
			  ,Target.RubricSelectedBandLevel = Source.RubricSelectedBandLevel

			  ,Target.[RowStatus] = 'Latest'
			WHEN NOT MATCHED THEN 
			 INSERT([TestResultId],[TestComponentId],[JobExaminerId],[RubricAssessementCriterionResultId],[ModifiedDate],[TestSittingId],[TestSessionId],[PersonId],[CustomerNo],[CandidateName],[PaidReview],
			 [Supplementary],[TestTaskTypeLabel],[TestTaskTypeName],[TestTaskLabel],[TestTaskName],[TestTaskNumber],[MarkType],[ResultType],[ExaminerCustomerNo],[ExaminerName],[ExaminerType],[Cost],[ExaminerSubmittedDate],
			 [IncludeMarks],[WasAttempted],[Successful],[RubricCompetencyLabel],[RubricCompetencyName],[RubricCriterionName],[RubricCriterionLabel],[RubricSelectedBandLabel],[RubricSelectedBandLevel],[RowStatus])
	  	  
			 VALUES (Source.[TestResultId],Source.[TestComponentId],Source.[JobExaminerId],Source.[RubricAssessementCriterionResultId], GetDate(), Source.[TestSittingId],Source.[TestSessionId],Source.[PersonId],Source.[CustomerNo],
			  Source.[CandidateName],[PaidReview],Source.[Supplementary],Source.[TestTaskTypeLabel],Source.[TestTaskTypeName],Source.[TestTaskLabel],Source.[TestTaskName],Source.[TestTaskNumber],Source.[MarkType],Source.[ResultType],
			  Source.[ExaminerCustomerNo],Source.[ExaminerName],Source.[ExaminerType],Source.[Cost],Source.[ExaminerSubmittedDate],Source.[IncludeMarks],Source.[WasAttempted],Source.[Successful],Source.[RubricCompetencyLabel],
			  Source.[RubricCompetencyName],Source.[RubricCriterionName],Source.[RubricCriterionLabel],Source.[RubricSelectedBandLabel],Source.[RubricSelectedBandLevel], 'Latest');

			COMMIT TRANSACTION;	
		END	
		
END