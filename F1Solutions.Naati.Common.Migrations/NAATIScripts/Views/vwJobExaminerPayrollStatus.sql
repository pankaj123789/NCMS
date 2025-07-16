CREATE VIEW [dbo].[vwJobExaminerPayrollStatus] 
AS
WITH js
AS
(
	SELECT
		tblJobExaminer.JobExaminerID
		,CASE
			WHEN ExaminerToPayrollDate IS NOT NULL
					-- this cutoff prevents old markings without a SentToPayroll date being shown in the Validate Payroll screen. We'll probably fine tune this, or run a DB update to make it unnecessary
					OR ExaminerReceivedDate < '2017/02/01' THEN 'ClaimForm'
			WHEN ExaminerReceivedDate IS NULL THEN 'NotReceived'
			WHEN ValidatedDate IS NULL THEN 'Received'
			WHEN JobExaminerPayrollId IS NULL THEN 'Ready'
			ELSE tluPayrollStatus.[Name]
		END Status
	FROM tblJobExaminer
	LEFT JOIN tblJobExaminerPayroll ON tblJobExaminerPayroll.JobExaminerId = tblJobExaminer.JobExaminerID
	LEFT JOIN tblPayroll ON tblJobExaminerPayroll.PayrollId = tblPayroll.PayrollId
	LEFT JOIN tluPayrollStatus ON tluPayrollStatus.PayrollStatusId = tblPayroll.PayrollStatusId)
SELECT
	JobExaminerId
	,ps.*
FROM js
LEFT JOIN tluPayrollStatus ps ON ps.[Name] = js.Status
