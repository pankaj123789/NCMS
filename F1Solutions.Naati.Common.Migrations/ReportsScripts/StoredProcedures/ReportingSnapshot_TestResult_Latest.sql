ALTER PROCEDURE [dbo].[ReportingSnapshot_TestResult_Latest]
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'TestResultLatest')
		BEGIN
			SELECT * INTO TestResultLatest FROM TestResultHistory WHERE RowStatus = 'Latest'
		END
	ELSE
		BEGIN	
	BEGIN TRANSACTION
	
		--Merge operation Latest
		MERGE TestResultLatest AS Target USING(select * from TestResultHistory where [RowStatus] = 'Latest' ) AS Source ON Target.[TestResultId] = Source.[TestResultId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestResultId = Source.TestResultId		  
		  ,Target.PersonId = Source.PersonId
		  ,Target.TestSittingId = Source.TestSittingId
		  ,Target.ResultDueDate = Source.ResultDueDate
		  ,Target.LanguageName1 = Source.LanguageName1      
		  ,Target.CandidateName = Source.CandidateName
		  ,Target.NAATINumber = Source.NAATINumber
		  ,Target.NAATINumberDisplay = Source.NAATINumberDisplay
		  ,Target.PaidReview = Source.PaidReview

		  ,Target.TotalMarks = Source.TotalMarks
		  ,Target.PassMark = Source.PassMark
		  ,Target.TotalCost = Source.TotalCost
		  ,Target.GeneralComments = Source.GeneralComments
		  ,Target.OverallResult = Source.OverallResult
		  ,Target.ResultDate = Source.ResultDate
		  ,Target.LanguageName2 = Source.LanguageName2
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName
		  ,Target.EligibleForSupplementary = Source.EligibleForSupplementary
		  ,Target.EligibleForConcededPass = Source.EligibleForConcededPass

		  ,Target.MarksOverridden = Source.MarksOverridden


		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN
		  INSERT([TestResultId],[ModifiedDate],[PersonId],[TestSittingId],[ResultDueDate],[LanguageName1],[CandidateName],[NAATINumber],[NAATINumberDisplay],[PaidReview],
			 [TotalMarks],[PassMark],[TotalCost],[GeneralComments],[OverallResult],[ResultDate],[LanguageName2],[CredentialTypeInternalName],[CredentialTypeExternalName],
			 [EligibleForSupplementary],[EligibleForConcededPass],[MarksOverridden],[RowStatus])
	  
		VALUES (Source.[TestResultId],GetDate(),[PersonId],Source.[TestSittingId],Source.[ResultDueDate],Source.[LanguageName1],Source.[CandidateName],Source.[NAATINumber],Source.[NAATINumberDisplay],
			  Source.[PaidReview],Source.[TotalMarks],Source.[PassMark],Source.[TotalCost],Source.[GeneralComments],Source.[OverallResult],Source.[ResultDate],Source.[LanguageName2],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],
			  Source.[EligibleForSupplementary],Source.[EligibleForConcededPass],Source.[MarksOverridden], 'Latest');
	
	COMMIT TRANSACTION;	
	END

END
