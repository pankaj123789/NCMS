ALTER VIEW [dbo].[MaterialRequestPayroll] AS
SELECT	
	 [MaterialRequestPayrollId]
	,[PanelMemberShipId] 
	,[MaterialRequestId]
	,[ApprovedDate]
	,[ApprovedByUserId]
	,[PaidByUserId]
	,[PaymentDate]
	,[Amount]
	,[PaymentReference]		
FROM [MaterialRequestPayrollHistory] tr
where [RowStatus] = 'Latest'