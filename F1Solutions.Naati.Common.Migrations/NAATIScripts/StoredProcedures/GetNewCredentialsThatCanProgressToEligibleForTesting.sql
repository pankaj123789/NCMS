ALTER PROCEDURE [dbo].[GetNewCredentialsThatCanProgressToEligibleForTesting]
AS
SELECT DISTINCT
	newCredentialRequest.CredentialApplicationId,
	newCredentialRequest.CredentialRequestId
FROM
	tblCredentialRequest existingCredentialRequest
	join tblCredentialApplication existingCredentialApplication on existingCredentialRequest.CredentialApplicationId = existingCredentialApplication.CredentialApplicationId
	join tblCredentialApplication newCredentialApplication on existingCredentialApplication.PersonId = newCredentialApplication.PersonId AND newCredentialApplication.CredentialApplicationTypeId = existingCredentialApplication.CredentialApplicationTypeId
	join tblCredentialRequest newCredentialRequest on newCredentialApplication.CredentialApplicationId = newCredentialRequest.CredentialApplicationId AND existingCredentialRequest.CredentialTypeId = newCredentialRequest.CredentialTypeId
	join tblCredentialApplicationTypeCredentialType catct on newCredentialRequest.CredentialTypeId = catct.CredentialTypeId and newCredentialApplication.CredentialApplicationTypeId = catct.CredentialApplicationTypeId
	left join tblCredentialApplicationFieldData newCredentialApplicationFieldData on newCredentialApplication.CredentialApplicationId = newCredentialApplicationFieldData.CredentialApplicationId
		AND newCredentialApplicationFieldData.CredentialApplicationFieldId in (33,37,55,65,69,93) -- these values are field ID's for 'Sponsored Applicaton' for different application types
WHERE
	existingCredentialApplication.CredentialApplicationStatusTypeId NOT IN (2, 3) -- Entered, Being Checked
	AND existingCredentialRequest.CredentialRequestStatusTypeId NOT IN 
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
	AND newCredentialApplication.CredentialApplicationTypeId IN 
		(3, -- CCL
		7, -- CCLV2
		13, -- CCLV3
		2, -- Certification
		6) -- CertificationPractitioner
	AND newCredentialApplication.CredentialApplicationStatusTypeId IN (2) -- Entered
	AND newCredentialRequest.CredentialRequestStatusTypeId IN (3) -- Request Entered
	AND catct.HasTest = 1
	AND (
			SELECT
				COUNT(a.CredentialApplicationId)
			FROM
				tblCredentialApplication a
				JOIN tblCredentialRequest b on a.CredentialApplicationId = b.CredentialApplicationId
				JOIN tblCredentialApplicationTypeCredentialType c on b.CredentialTypeId = c.CredentialTypeId and a.CredentialApplicationTypeId = c.CredentialApplicationTypeId
			WHERE
				a.CredentialApplicationId = newCredentialApplication.CredentialApplicationId
				AND c.HasTest = 0
		) = 0
	AND (newCredentialApplicationFieldData.Value like '%false%' OR newCredentialApplicationFieldData.Value IS NULL) -- only get applications that are not marked as sponsored