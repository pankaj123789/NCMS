ALTER VIEW [dbo].[TestResult] AS
SELECT
    [TestResultId]
    ,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]
    ,[PersonId]
    ,[TestSittingId]
    ,[ResultDueDate]
	,[CredentialTypeInternalName]
	,[CredentialTypeExternalName]
    ,[LanguageName1]
    ,[LanguageName2]
    ,[CandidateName]
    ,[NAATINumber]
    ,[NAATINumberDisplay]
    ,[PaidReview]
    ,[TotalMarks]
    ,[PassMark]
    ,[TotalCost]
    ,[GeneralComments]
    ,[OverallResult]
    ,[ResultDate]
	,[EligibleForSupplementary]
	,[EligibleForConcededPass]
	,[MarksOverridden]	
FROM [TestResultLatest] tr
--FROM [TestResultHistory] tr
--where [RowStatus] = 'Latest'
