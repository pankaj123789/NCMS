ALTER VIEW RubricTestComponentResult AS
	SELECT
		rtcr.[RubricTestComponentResultId]
		,rtcr.[TestComponentId]
		,racr.[RubricAssessementCriterionResultId]
		,rtcr.WasAttempted WasAttempted
		,rtcr.Successful Successful
		,rmc.Label RubricCompetencyLabel
		,rmc.Name RubricCompetencyName
		,rmac.Name RubricCriterionName
		,rmac.Label RubricCriterionLabel
		,rmb.Label RubricSelectedBandLabel
		,rmb.Level RubricSelectedBandLevel
		,rtcr.MarkingResultTypeId
	FROM [naati_db]..[tblRubricTestComponentResult] rtcr 
		LEFT JOIN [naati_db]..[tblRubricAssessementCriterionResult] racr ON rtcr.[RubricTestComponentResultId] = racr.[RubricTestComponentResultId]
		LEFT JOIN [naati_db]..[tblRubricMarkingAssessmentCriterion] rmac ON racr.[RubricMarkingAssessmentCriterionId] = rmac.[RubricMarkingAssessmentCriterionId]
		LEFT JOIN [naati_db]..[tblRubricMarkingCompetency] rmc ON rmac.[RubricMarkingCompetencyId] = rmc.[RubricMarkingCompetencyId]
		LEFT JOIN [naati_db]..[tblRubricMarkingBand] rmb ON racr.[RubricMarkingBandId] = rmb.[RubricMarkingBandId]