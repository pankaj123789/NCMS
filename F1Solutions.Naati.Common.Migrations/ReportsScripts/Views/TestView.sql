ALTER VIEW [dbo].[Test] AS
SELECT
    [TestSittingId]
    ,DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS [DateRecorded]
    ,[CredentialRequestId]
    ,[TestResultId]
    ,[CredentialApplicationId]
    ,[PersonId]
    ,[CandidateName]
    ,[LanguageName1]
    ,[LanguageName2]
    ,[Rejected]
    ,[Sat]
    ,[ResultType]
    ,[ThirdExaminerRequired]
    ,[ProcessedDate]
    ,[SatDate]
    ,[ResultChecked]
    ,[Venue]
	,[CredentialTypeInternalName]
	,[CredentialTypeExternalName]
	,[Skill]
	,[ApplicationType]
	,[SupplementaryTest]	
    FROM [TestLatest] t
--FROM [TestHistory] t
--where [RowStatus] = 'Latest'