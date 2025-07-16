-- THIS STORED PROCEDURE IS REDUNDANT AS WE ONLY WANT TO MOVE INDIVIDUAL CREDENTIAL REQUESTS AND NOT WHOLE APPLICATIONS
-- PLEASE USE THE NEW PROCEDURE 'GetNewCredentialsThatCanProgressToEligibleForTesting'
ALTER PROCEDURE [dbo].[GetNewApplicationsThatCanProgressToEligibleForTesting]
AS
SELECT
 DISTINCT newApplications.CredentialApplicationId
FROM
	tblCredentialApplication INNER JOIN
	tblCredentialRequest on tblCredentialApplication.CredentialApplicationId = tblCredentialRequest.CredentialApplicationId INNER JOIN
	tblCredentialRequestStatusType ON tblCredentialRequest.CredentialRequestStatusTypeId = tblCredentialRequestStatusType.CredentialRequestStatusTypeId INNER JOIN
	tblCredentialApplicationStatusType on tblCredentialApplication.CredentialApplicationStatusTypeId = tblCredentialApplicationStatusType.CredentialApplicationStatusTypeId INNER JOIN
	tblCredentialApplicationType on tblCredentialApplication.CredentialApplicationTypeId = tblCredentialApplicationType.CredentialApplicationTypeId INNER JOIN
	(SELECT 
		credentialApplication.CredentialApplicationId,PersonId,credentialApplication.CredentialApplicationTypeId
	FROM 
		tblCredentialApplication credentialApplication inner join
		tblCredentialApplicationStatusType on credentialApplication.CredentialApplicationStatusTypeId = tblCredentialApplicationStatusType.CredentialApplicationStatusTypeId inner join
		tblCredentialRequest on credentialApplication.CredentialApplicationId = tblCredentialRequest.CredentialApplicationId INNER JOIN
		tblCredentialApplicationType on credentialApplication.CredentialApplicationTypeId = tblCredentialApplicationType.CredentialApplicationTypeId
	WHERE 
		tblCredentialApplicationStatusType.CredentialApplicationStatusTypeId IN (2, 3) AND -- Entered, BeingChecked
		tblCredentialApplicationType.CredentialApplicationTypeId IN 
			(3, -- CCL
			7, -- CCLV2
			13, -- CCLV3
			2, -- Certification
			6) -- CertificationPractitioner
	) 
	AS newApplications
ON newApplications.PersonId = tblCredentialApplication.PersonId
WHERE 
tblCredentialApplicationStatusType.CredentialApplicationStatusTypeId NOT IN (2, 3) AND -- Entered, BeingChecked
tblCredentialRequestStatusType.CredentialRequestStatusTypeId NOT IN 
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
	17) AND --'Withdrawn'
tblCredentialApplication.CredentialApplicationTypeId = newApplications.CredentialApplicationTypeId