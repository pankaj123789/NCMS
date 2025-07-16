ALTER PROCEDURE [dbo].[ReportingSnapshot_Main]	
AS
BEGIN

	Declare @DisableReportingExecution varchar(max)
	set @DisableReportingExecution = dbo.GetSystemValue('DisableReportingExecution')

	IF @DisableReportingExecution <> '1' 
	BEGIN
		DECLARE @Date datetime = DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)	
	
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Exection Started...'

		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshop_ExaminerJob'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshop_MaterialRequestPayroll'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshop_TestMaterial'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_Application'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_ApplicationCustomFIelds'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_CredentialRequests'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_Credentials'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_Mark'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_MarkRubric'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_MaterialRequest'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_MaterialRequestPanelMember'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_MaterialRequestRound'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_MaterialRequestTaskHours'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_Organisation'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_OrganisationContacts'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_Panel'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_PanelMembers'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_PanelMembershipCredentialTypes'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_Person'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_ProfessionalDevelopment'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_Test'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestResult'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestResultRubric'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestSessionRolePlayer'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestSessionRolePlayerDetails'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestSessions'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestSittingTestMaterial'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_WorkPractice'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshop_EndorsedQualification'	
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestResultExaminerRubricMarkingHistory'
		EXEC ReportingSnapshot_Runner @Date, 'ReportingSnapshot_TestResultExaminerStandardMarkingHistory'

		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Extraction Finished.'
		--Speed improvements
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_Application_Latest.'
		EXEC ReportingSnapshot_Application_Latest
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_ApplicationCustomFields_Latest.'
		EXEC ReportingSnapshot_ApplicationCustomFields_Latest
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_CredentialRequests_Latest.'
		EXEC ReportingSnapshot_CredentialRequests_Latest
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_Mark_Latest.'
		EXEC ReportingSnapshot_Mark_Latest
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_MarkRubric_Latest.'
		EXEC ReportingSnapshot_MarkRubric_Latest
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_Test_Latest.'
		EXEC ReportingSnapshot_Test_Latest
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_TestResult_Latest.'
		EXEC ReportingSnapshot_TestResult_Latest
		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Begin ReportingSnapshot_TestSessions_Latest.'
		EXEC ReportingSnapshot_TestSessions_Latest

		EXEC LogExecutionInfo 'ReportingSnapshot_Main','Exection Finished.'
	END
	ElSE
	BEGIN
		EXEC LogExecutionWarning 'ReportingSnapshot_Main','Report Execution is disabled.'
	END	

END