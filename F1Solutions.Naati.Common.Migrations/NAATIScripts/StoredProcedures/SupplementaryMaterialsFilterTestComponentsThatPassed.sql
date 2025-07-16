CREATE PROCEDURE SupplementaryMaterialsFilterTestComponentsThatPassed 
-- this is used when sending out test materials for supplementary tests
-- it identifies which components passed so that the material is not sent out

	@CredentialRequestId int
AS
BEGIN
	SELECT 
	d.Successful,
	e.TestComponentId 
	FROM 
	tblTestSitting a inner join
	tbltestresult b ON b.TestSittingId = a.TestSittingId inner join
	tblTestResultRubricTestComponentResult c ON b.TestResultId = c.TestResultId inner join
	tblRubricTestComponentResult d ON d.RubricTestComponentResultId = c.RubricTestComponentResultId inner join
	tblTestComponent e ON d.TestComponentId = e.TestComponentId

	WHERE 
		CredentialRequestId = @CredentialRequestId AND 
		Supplementary = 0 AND 
		Sat = 1 AND 
		WasAttempted = 1 AND 
		RejectedDate IS NULL
END

