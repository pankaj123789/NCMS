CREATE PROCEDURE GetStoredFilesForFileDeletion
	@DocumentType int,
	@Entity nvarchar(500),
	@IncludeQueued bit,
	@SelectLimit int
AS
BEGIN
	SET NOCOUNT ON;

	-- SPECIAL CASES WITH NO Entity and Document Type
	-- Policy 8: "MarkedTestAsset", TestSittingDocumentEntity
	IF @Entity like '%tblTestSittingDocument%' and @DocumentType = 0
		SELECT TOP(@SelectLimit)
		storedFile.StoredFileId
	FROM
		tblStoredFile storedFile JOIN
		tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
		tblTestSittingDocument testSittingDocument ON storedFile.StoredFileId = testSittingDocument.StoredFileId JOIN
		tblTestSitting testSitting ON testSittingDocument.TestSittingId = testSitting.TestSittingId JOIN
		tblTestResult testResult ON testSitting.TestSittingId = testResult.TestSittingId JOIN
		tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
			ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
		tblStoredFileDeletePolicy storedFileDeletePolicy
			ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
	WHERE
		storedFile.StoredFileStatusTypeId IN 
			(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
		documentType.Name like CONCAT('%', 'MarkedTestAsset', '%') AND
		storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
		testResult.ProcessedDate < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
	ORDER BY storedFile.StoredFileId

	IF @Entity like '%tblCredentialApplicationAttachment%'
		SELECT TOP(@SelectLimit)
			storedFile.StoredFileId
		FROM
			tblStoredFile storedFile JOIN
			tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblCredentialApplicationAttachment credentialApplicationAttachment ON storedFile.StoredFileId = credentialApplicationAttachment.StoredFileId JOIN
			tblCredentialApplication credentialApplication ON credentialApplicationAttachment.CredentialApplicationId = credentialApplication.CredentialApplicationId JOIN
			tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
				ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblStoredFileDeletePolicy storedFileDeletePolicy
				ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
		WHERE
			storedFile.StoredFileStatusTypeId IN 
				(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
			credentialApplication.CredentialApplicationStatusTypeId = 6 AND -- completed
			documentType.DocumentTypeId = @DocumentType AND
			storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
			credentialApplication.StatusChangeDate < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
		ORDER BY storedFile.StoredFileId

	IF @Entity like '%tblTestSittingDocument%'
		SELECT TOP(@SelectLimit)
		storedFile.StoredFileId
	FROM
		tblStoredFile storedFile JOIN
		tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
		tblTestSittingDocument testSittingDocument ON storedFile.StoredFileId = testSittingDocument.StoredFileId JOIN
		tblTestSitting testSitting ON testSittingDocument.TestSittingId = testSitting.TestSittingId JOIN
		tblTestResult testResult ON testSitting.TestSittingId = testResult.TestSittingId JOIN
		tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
			ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
		tblStoredFileDeletePolicy storedFileDeletePolicy
			ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
	WHERE
		storedFile.StoredFileStatusTypeId IN 
			(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
		documentType.DocumentTypeId = @DocumentType AND
		storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
		testResult.ProcessedDate < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
	ORDER BY storedFile.StoredFileId

	IF @Entity like '%tblNoteAttachment%'
		SELECT DISTINCT TOP(@SelectLimit)
			storedFile.StoredFileId
		FROM
			tblStoredFile storedFile JOIN
			tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblNoteAttachment noteAttachment ON storedFile.StoredFileId = noteAttachment.StoredFileId JOIN
			tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
				ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblStoredFileDeletePolicy storedFileDeletePolicy
				ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
		WHERE
			storedFile.StoredFileStatusTypeId IN 
				(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
			documentType.DocumentTypeId = @DocumentType AND
			storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
			storedFile.UploadedDateTime < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
		ORDER BY storedFile.StoredFileId

	IF @Entity like '%tblPersonAttachment%'
		SELECT TOP(@SelectLimit)
			storedFile.StoredFileId
		FROM
			tblStoredFile storedFile JOIN
			tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblPersonAttachment personAttachment ON storedFile.StoredFileId = personAttachment.StoredFileId JOIN
			tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
				ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblStoredFileDeletePolicy storedFileDeletePolicy
				ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
		WHERE
			storedFile.StoredFileStatusTypeId IN 
				(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
			documentType.DocumentTypeId = @DocumentType AND
			storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
			storedFile.UploadedDateTime < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
		ORDER BY storedFile.StoredFileId

	IF @Entity like '%tblWorkPracticeAttachment%'
		SELECT DISTINCT TOP(@SelectLimit)
			storedFile.StoredFileId
		FROM
			tblStoredFile storedFile JOIN
			tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblWorkPracticeAttachment  workPracticeAttachment ON storedFile.StoredFileId = workPracticeAttachment.StoredFileId JOIN
			tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
				ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblStoredFileDeletePolicy storedFileDeletePolicy
				ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
		WHERE
			storedFile.StoredFileStatusTypeId IN 
				(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
			documentType.DocumentTypeId = @DocumentType AND
			storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
			storedFile.UploadedDateTime < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
		ORDER BY storedFile.StoredFileId

	IF @Entity like '%tblMaterialRequestRoundAttachment%'
		SELECT TOP(@SelectLimit)
			storedFile.StoredFileId
		FROM
			tblStoredFile storedFile JOIN
			tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblMaterialRequestRoundAttachment materialRequestRoundAttachment ON storedFile.StoredFileId = materialRequestRoundAttachment.StoredFileId JOIN
			tblMaterialRequestRound materialRequestRound on materialRequestRoundAttachment.MaterialRequestRoundId = materialRequestRound.MaterialRequestRoundId JOIN
			tblMaterialRequest materialRequest on materialRequestRound.MaterialRequestId = materialRequest.MaterialRequestId JOIN
			tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
				ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblStoredFileDeletePolicy storedFileDeletePolicy
				ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
		WHERE
			storedFile.StoredFileStatusTypeId IN 
				(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
			materialRequest.MaterialRequestStatusTypeId = 3 AND -- Finalised
			documentType.DocumentTypeId = @DocumentType AND
			storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
			materialRequest.StatusChangeDate < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
		ORDER BY storedFile.StoredFileId

	IF @Entity like '%tblProfessionalDevelopmentActivityAttachment%'
		SELECT TOP(@SelectLimit)
			storedFile.StoredFileId
		FROM
			tblStoredFile storedFile JOIN
			tluDocumentType documentType ON storedFile.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblProfessionalDevelopmentActivityAttachment professionalDevelopmentActivityAttachment ON storedFile.StoredFileId = professionalDevelopmentActivityAttachment.StoredFileId JOIN
			tblProfessionalDevelopmentActivity professionalDevelopmentActivity ON 
			professionalDevelopmentActivityAttachment.ProfessionalDevelopmentActivityId = professionalDevelopmentActivity.ProfessionalDevelopmentActivityId JOIN
			tblProfessionalDevelopmentCredentialApplication professinalDevelopmentActivityCredentialApplication ON
			professionalDevelopmentActivity.ProfessionalDevelopmentActivityId = professinalDevelopmentActivityCredentialApplication.ProfessionalDevelopmentActivityId JOIN
			tblCredentialApplication credentialApplication ON professinalDevelopmentActivityCredentialApplication.CredentialApplicationId = credentialApplication.CredentialApplicationId JOIN
			tblStoredFileDeletePolicyDocumentType storedFileDeletePolicyDocumentType
				ON storedFileDeletePolicyDocumentType.DocumentTypeId = documentType.DocumentTypeId JOIN
			tblStoredFileDeletePolicy storedFileDeletePolicy
				ON storedFileDeletePolicy.StoredFileDeletePolicyId = storedFileDeletePolicyDocumentType.StoredFileDeletePolicyId
		WHERE
			storedFile.StoredFileStatusTypeId IN 
				(1, (CASE WHEN @IncludeQueued = 1 THEN 2 ELSE 1 END)) AND
			credentialApplication.CredentialApplicationStatusTypeId = 6 AND -- completed
			documentType.DocumentTypeId = @DocumentType AND
			storedFileDeletePolicyDocumentType.EntityType like CONCAT('%', @Entity, '%') AND
			credentialApplication.StatusChangeDate < DATEADD(day, -storedFileDeletePolicy.DaysToKeep, getdate())
		ORDER BY storedFile.StoredFileId
END