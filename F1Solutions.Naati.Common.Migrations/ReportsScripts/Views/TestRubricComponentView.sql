ALTER VIEW TestRubricComponent AS
	SELECT
		j.[JobId]
		,tct.[TestComponentId]
		,tr.[TestResultId] 
		,ts.[TestSittingId] 
		,ts.[TestSessionId] 
		,p.[PersonId]
		,e.[NaatiNumber] CustomerNo
		,(
			SELECT TOP 1
				[Title] + ' ' + [GivenName] + ' ' + [SurName]
			FROM [naati_db]..[tblPersonName] pn
			LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.[TitleId]
			WHERE pn.[PersonId] = p.[PersonId]
			ORDER BY pn.[EffectiveDate] DESC
		) CandidateName
		,CAST(CASE WHEN j.[ReviewFromJobId] IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS  PaidReview
		,ts.Supplementary
		,tctt.Label TestTaskTypeLabel
		,tctt.Name TestTaskTypeName
		,tct.Name TestTaskLabel
		,tct.Name TestTaskName
		,tct.ComponentNumber TestTaskNumber
		,rt.Result
	FROM [naati_db]..[tblTestResult] tr 
		LEFT JOIN [naati_db]..[tblJob] j ON tr.[CurrentJobId] = j.[JobId]
		LEFT JOIN [naati_db]..[tblTestSitting] ts ON tr.[TestSittingId] = ts.[TestSittingId]
		LEFT JOIN [naati_db]..[tblTestSession] tss ON ts.[TestSessionId] = tss.[TestSessionId]
		LEFT JOIN [naati_db]..[tblTestComponent] tct ON ts.[TestSpecificationId] = tct.[TestSpecificationId]
			AND ts.[TestSpecificationId] = tct.[TestSpecificationId]
		LEFT JOIN [naati_db]..[tblTestComponentType] tctt ON tct.[TypeId] = tctt.[TestComponentTypeId] 
		LEFT JOIN [naati_db]..[tblCredentialRequest] cr ON ts.[CredentialRequestId] = cr.[CredentialRequestId]
		LEFT JOIN [naati_db]..[tblCredentialApplication] a ON cr.[CredentialApplicationId] = a.[CredentialApplicationId]
		LEFT JOIN [naati_db]..[tblPerson] p ON a.[PersonId] = p.[PersonId]
		LEFT JOIN [naati_db]..[tblEntity] e ON p.[EntityId] = e.[EntityId]
		LEFT JOIN [naati_db]..[tluResultType] rt ON tr.[ResultTypeId] = rt.[ResultTypeId]
	WHERE
		-- JUST RUBRIC MARKING
		(SELECT COUNT(1)
		FROM [naati_db]..[tblTestComponentType] tct
			JOIN [naati_db]..[tblRubricMarkingCompetency] rmc ON tct.[TestComponentTypeId] = rmc.[TestComponentTypeId]
		WHERE tct.[TestSpecificationId] = ts.[TestSpecificationId]) > 0