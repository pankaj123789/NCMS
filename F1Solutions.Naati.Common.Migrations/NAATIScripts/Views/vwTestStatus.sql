
CREATE   VIEW [dbo].[vwTestStatus]
--WITH SCHEMABINDING  
AS 

WITH TestStatusInfo
as
(
select testSitting.TestSittingId,
MAX(Cast(testSitting.Supplementary as int)) AS IsSupplementaryTestSitting,
MAX(cast(credentialRequest.Supplementary as int)) AS IsSupplementaryCredentialRequest,
sum(CASE WHEN testResult.ResultChecked IS NOT NULL AND  testResult.ResultChecked = 1 THEN 0 ELSE 1 END) AS ResultsToBeChecked,
sum(CASE WHEN jobExaminer.PaidReviewer = 1 THEN 1 ELSE 0 END) AS SumPaidViewer, 
sum(CASE WHEN jobExaminer.ThirdExaminer = 1 THEN 1 ELSE 0 END) AS SumThirdExaminers,
sum(CASE WHEN jobExaminer.JobExaminerId >= 0 THEN 1 ELSE 0 END) AS SumExaminers,
sum(CASE WHEN jobExaminer.JobExaminerId >= 0 AND jobExaminer.ExaminerReceivedDate IS NOT NULL THEN 1 ELSE 0 END) As SumMarksReceived,
sum(CASE WHEN storedFile.StoredFileId IS NOT NULL THEN 1 ELSE 0 END) SumAssets,
sum(CASE WHEN job.DueDate IS NOT NULL AND DATEADD(day, DATEDIFF(day, 0, job.DueDate), 1) < GETDATE() AND  jobExaminer.JobExaminerId >= 0  AND jobExaminer.ExaminerReceivedDate IS NULL THEN 1 ELSE 0 END) AS SumOverdueExaminers,
Max(credentialRequest.CredentialRequestStatusTypeId) As CredentialRequestStatus,
MAX(CASE WHEN job.ReviewFromJobId IS NOT NULL AND testResult.EligibleForSupplementary = 1 THEN 1 ELSE 0 END) AS PaidReviewEligibleForSupplementary,
MAX(CASE WHEN job.ReviewFromJobId IS NULL AND testResult.EligibleForSupplementary = 1 THEN 1 ELSE 0 END) AS ResultEligibleForSupplementary,
MAX(CASE WHEN job.ReviewFromJobId IS NOT NULL AND testResult.EligibleForConcededPass = 1 THEN 1 ELSE 0 END) AS PaidReviewEligibleForConcededPass,
MAX(CASE WHEN job.ReviewFromJobId IS NULL AND testResult.EligibleForConcededPass = 1 THEN 1 ELSE 0 END) AS ResultEligibleForConcededPass,
sum(CASE WHEN testResult.ResultChecked IS NOT NULL AND  testResult.ResultChecked = 1 AND testResult.ResultTypeId = 5 THEN 1 ELSE 0 END) AS ResultsInvaliated,
MAX(CASE WHEN jobExaminer.JobExaminerId >= 0 AND jobExaminer.ExaminerReceivedDate IS NOT NULL THEN jobExaminer.ExaminerReceivedDate ELSE NULL END) As LastExaminerReceivedDate,
sum(CASE WHEN testResult.AllowIssue  = 0 THEN 1 ELSE 0 END) SumNotAllowIssue
from [dbo].tblTestSitting testSitting
inner join tblCredentialRequest credentialRequest on testSitting.CredentialRequestId = credentialRequest.credentialRequestId
LEFT join tblTestResult testResult on testResult.TestSittingId = testSitting.TestSittingId
LEFT join tblJob job on testResult.CurrentJobId = job.JobId
LEFT join tblJobExaminer jobExaminer on jobExaminer.JobId = job.JobId
left join tblTestSittingDocument testDocument on testSitting.TestSittingId = testDocument.TestSittingId
left join tblStoredFile storedFile on storedFile.StoredFileId = testDocument.StoredFileId and storedFile.DocumentTypeId in (2,3,4,5,6) -- UnmarkedTestAsset,MarkedTestAsset,EnglishMarking,ReviewReport,TestMaterial

where testSitting.Rejected = 0 and testSitting.Sat = 1
group by testSitting.TestSittingId
)
select 
TestStatusInfo.TestSittingId,
TestStatusInfo.LastExaminerReceivedDate,
CASE WHEN TestStatusInfo.CredentialRequestStatus = 30 OR  TestStatusInfo.ResultsInvaliated > 0 THEN 7-- Test Invalidated
ELSE
  CASE WHEN TestStatusInfo.IsSupplementaryTestSitting = 0 and  TestStatusInfo.IsSupplementaryCredentialRequest = 1 and TestStatusInfo.CredentialRequestStatus <> 11 AND  TestStatusInfo.CredentialRequestStatus <> 12  AND  TestStatusInfo.CredentialRequestStatus <> 16 THEN 6-- Pending Supplementary
	ELSE
	CASE WHEN TestStatusInfo.CredentialRequestStatus = 11 OR  TestStatusInfo.CredentialRequestStatus = 12 OR TestStatusInfo.CredentialRequestStatus = 16   THEN 5 --finalised  // Credential request in status failed or issued credential or withdrawn
	 ELSE 
		CASE WHEN TestStatusInfo.CredentialRequestStatus=23 OR TestStatusInfo.CredentialRequestStatus=27 OR TestStatusInfo.CredentialRequestStatus=28 OR TestStatusInfo.SumPaidViewer > 0 THEN 4 --Under paid review
		ELSE 
			CASE WHEN TestStatusInfo.SumThirdExaminers > 0 THEN 3 -- Under review
			ELSE
				CASE WHEN TestStatusInfo.SumExaminers >0 THEN 2 -- In progresss
				ELSE 1 -- Sat
				END
			END
		END
	 END
 END
 END AS TestStatusTypeId,
 CASE WHEN TestStatusInfo.SumMarksReceived = TestStatusInfo.SumExaminers  and TestStatusInfo.SumExaminers > 0 THEN 1 ELSE 0 END AS AllMarksReceived,
 CASE WHEN TestStatusInfo.SumAssets > 0 THEN 1 ELSE 0 END AS HasAssets,
 CASE WHEN TestStatusInfo.SumExaminers > 0 THEN 1 ELSE 0 END AS HasExaminers,
 CASE WHEN TestStatusInfo.SumOverdueExaminers > 0 THEN 1 ELSE 0 END AS HasOverdueExaminers,
 CASE WHEN TestStatusInfo.SumMarksReceived > 0 THEN 1 ELSE 0 END AS HasSubmittedExaminers,
 CASE WHEN TestStatusInfo.SumPaidViewer > 0 THEN 1 ELSE 0 END AS HasPaidReviewExaminers,
 CASE WHEN TestStatusInfo.SumExaminers - TestStatusInfo.SumMarksReceived-TestStatusInfo.SumOverdueExaminers > 0  THEN 1 ELSE 0 END AS HasInProgressExaminers,
 CASE WHEN (TestStatusInfo.SumPaidViewer > 0 AND PaidReviewEligibleForSupplementary = 1) OR (TestStatusInfo.SumPaidViewer = 0 AND ResultEligibleForSupplementary = 1)  THEN 1 ELSE 0 END AS EligibleForSupplementary,
 CASE WHEN (TestStatusInfo.SumPaidViewer > 0 AND PaidReviewEligibleForConcededPass = 1) OR (TestStatusInfo.SumPaidViewer = 0 AND ResultEligibleForConcededPass = 1) THEN 1 ELSE 0  END AS EligibleForConcededPass,
 CASE WHEN TestStatusInfo.SumNotAllowIssue > 0 THEN 0 ELSE 1 END AS AllowIssue
 FROM TestStatusInfo



 ---

