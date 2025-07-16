ALTER VIEW[dbo].[WorkPractice] AS
SELECT
      [WorkPracticeId]
    , DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AS[DateRecorded]
    , [PersonId]
    , [CustomerNumber]
    , [PractitionerNumber]
    , [ApplicationID]
	, [ApplicationStatus]
	, [CredentialTypeInternalName]
	, [CredentialTypeExternalName]
	, [Skill]
    , [CertificationPeriodID]
    , [CertificationPeriodStartDate]
    , [CertificationPeriodOriginalEndDate]
    , [CertificationPeriodEndDate]
    , [DateCompleted]
    , [Description]
	, [Points]
	, [WorkPracticeUnits]
	, [NumberOfAttachments]
    , [ModifiedDate]    
FROM [WorkPracticeHistory] a
where [RowStatus] = 'Latest'