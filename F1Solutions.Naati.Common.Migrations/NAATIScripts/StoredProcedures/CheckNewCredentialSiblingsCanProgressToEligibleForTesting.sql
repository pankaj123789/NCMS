ALTER PROCEDURE CheckNewCredentialSiblingsCanProgressToEligibleForTesting
@credentialApplicationId as int,
@credentialRequestId as int
AS
--DECLARE @credentialApplicationId as int
--DECLARE @credentialRequestId as int
--select 		@credentialApplicationId = 670346,
--		@credentialRequestId = 533495

IF OBJECT_ID(N'tempdb..#tmpCredentialTypes') IS NOT NULL
BEGIN
DROP TABLE #tmpCredentialTypes
END

IF OBJECT_ID(N'tempdb..#tmpFoundCredentialTypes') IS NOT NULL
BEGIN
DROP TABLE #tmpFoundCredentialTypes
END

--check credential application and make sure all requests can be moved forward

DECLARE @requestCount as int
DECLARE @resultCount as int
DECLARE @personId as int
DECLARE @result as bit

-- first test. are there any other cred requests in this application that can be moved forward?
SELECT @requestCount = count(*) 
from 
	tblCredentialApplication a INNER JOIN 
	tblCredentialRequest b on a.CredentialApplicationId = b.CredentialApplicationId INNER JOIN
	tblCredentialApplicationTypeCredentialType c on c.CredentialTypeId = b.CredentialTypeId
WHERE 
	a.credentialApplicationId = @credentialApplicationId
	AND b.CredentialRequestId <> @credentialRequestId
	AND a.CredentialApplicationStatusTypeId IN (2) -- Entered
	AND b.CredentialRequestStatusTypeId IN (3) -- Request Entered
	AND c.HasTest = 1

IF(@requestCount = 0)
--SELECT 'OK To Process'
RETURN 1 -- ok to use this one as only one cred request that can be moved forward


SELECT @personId = personId FROM tblCredentialApplication WHERE credentialApplicationId = @credentialApplicationId
SELECT CredentialTypeId INTO #tmpCredentialTypes FROM tblCredentialRequest 
WHERE 
	credentialApplicationId = @credentialApplicationId
	AND credentialRequestId <> @credentialRequestId

-- are there any credentialrequests that 
-- have had a previous certification issued
-- 
SELECT DISTINCT a.CredentialTypeId INTO #tmpFoundCredentialTypes
FROM 
	tblCredentialRequest a inner join
	tblCredentialApplication b on a.CredentialApplicationId = b.CredentialApplicationId inner join
	tblCredentialApplicationTypeCredentialType c on c.CredentialTypeId = a.CredentialTypeId
WHERE 
	b.personId = @personId
	AND a.CredentialTypeId IN (SELECT CredentialTypeId FROM #tmpCredentialTypes)
	AND a.CredentialRequestId <> @credentialRequestId
	AND c.HasTest = 1
	AND a.CredentialRequestStatusTypeId NOT IN 
		(1,     --'Draft', 
		2,      --'Rejected',
		3,      --'RequestEntered', 
		4,      --'ReadyForAssessment',
		5,      --'BeingAssessed'
		6,      --'Pending'
		7,      --'AssessmentFailed'
		8,      --'AssessmentPaidReview'
		13,     --'Cancelled'
		14,     --'Deleted'
		15,     --'AwaitingTestPayment'
		16)	    --'Withdrawn'
	AND b.CredentialApplicationTypeId IN 
		(3, -- CCL
		7, -- CCLV2
		13, -- CCLV3
		2, -- Certification
		6) -- CertificationPractitioner

--SELECT * FROM #tmpCredentialTypes
--SELECT * FROM #tmpFoundCredentialTypes

SELECT @requestCount = Count(*) FROM #tmpCredentialTypes
SELECT @resultCount = Count(*) FROM #tmpFoundCredentialTypes

IF @requestCount = @resultCount
--select 'Ok To Process'
RETURN 1
ELSE
RETURN 0
--select 'Cant proceed'









