ALTER PROCEDURE [dbo].[ReportingSnapshop_MaterialRequestPayroll]
	@Date DateTime
AS
BEGIN
	DECLARE @MaterialRequestPayrollHistory as table([MaterialRequestPayrollId] [int] NOT NULL,[PanelMembershipId] [int] NOT NULL,[MaterialRequestId] [int] NOT NULL,[ApprovedDate] [datetime] NULL,[ApprovedByUserId] [int] NULL,[PaidByUserId] [int] NULL,[PaymentDate] [datetime] NULL,
													[Amount] [money] NULL,[PaymentReference] [nvarchar](255) NULL,[ModifiedDate] [datetime] NOT NULL,[RowStatus] nvarchar(50))   
														
	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @MaterialRequestPayrollHistory

	SELECT		 
		 CASE WHEN [Source].[MaterialRequestPayrollId] IS NULL THEN [MaterialRequestPayroll].[MaterialRequestPayrollId] ELSE [Source].[MaterialRequestPayrollId] END AS [MaterialRequestPayrollId]
		,CASE WHEN [Source].[PanelMembershipId] IS NULL THEN [MaterialRequestPayroll].[PanelMembershipId] ELSE [Source].[PanelMembershipId] END AS [PanelMembershipId]
		,CASE WHEN [Source].[MaterialRequestId] IS NULL THEN [MaterialRequestPayroll].[MaterialRequestId] ELSE [Source].[MaterialRequestId] END AS [MaterialRequestId]
		,CASE WHEN [Source].[ApprovedDate] IS NULL THEN [MaterialRequestPayroll].[ApprovedDate] ELSE [Source].[ApprovedDate] END AS [ApprovedDate]
		,CASE WHEN [Source].[ApprovedByUserId] IS NULL THEN [MaterialRequestPayroll].[ApprovedByUserId] ELSE [Source].[ApprovedByUserId] END AS [ApprovedByUserId]
		,CASE WHEN [Source].[PaidByUserId] IS NULL THEN [MaterialRequestPayroll].[PaidByUserId] ELSE [Source].[PaidByUserId] END AS [PaidByUserId]
		,CASE WHEN [Source].[PaymentDate] IS NULL THEN [MaterialRequestPayroll].[PaymentDate] ELSE [Source].[PaymentDate] END AS [PaymentDate]
		,CASE WHEN [Source].[Amount] IS NULL THEN [MaterialRequestPayroll].[Amount] ELSE [Source].[Amount] END AS [Amount]
		,CASE WHEN [Source].[PaymentReference] IS NULL THEN [MaterialRequestPayroll].[PaymentReference] ELSE [Source].[PaymentReference] END AS [PaymentReference]

		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[MaterialRequestPayrollId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
	FROM
	(
		SELECT	
		 mrpr.[MaterialRequestPayrollId] AS [MaterialRequestPayrollId]
		,mrpm.PanelMembershipId as [PanelMembershipId]
		,mrpm.MaterialRequestId as [MaterialRequestId]
		,mrpr.ApprovedDate as [ApprovedDate]
		,mrpr.ApprovedByUserId as [ApprovedByUserId]
		,mrpr.PaidByUserId as [PaidByUserId]
		,mrpr.PaymentDate as [PaymentDate]
		,mrpr.Amount as [Amount]
		,mrpr.PaymentReference as [PaymentReference]		

		FROM [naati_db]..[tblMaterialRequestPayroll] mrpr		
		    LEFT JOIN [naati_db]..[tblMaterialRequestPanelMembership] mrpm on mrpr.MaterialRequestPanelMembershipId = mrpm.MaterialRequestPanelMembershipId   
			LEFT JOIN [naati_db]..[tblMaterialRequest] mr ON mrpm.MaterialRequestId = mr.MaterialRequestId	
						
	) [Source]
	FULL OUTER JOIN [MaterialRequestPayroll] ON ([Source].[MaterialRequestPayrollId] = [MaterialRequestPayroll].[MaterialRequestPayrollId])

	WHERE 		
		(([Source].[MaterialRequestPayrollId] IS NOT NULL AND [MaterialRequestPayroll].[MaterialRequestPayrollId] IS NULL) OR ([Source].[MaterialRequestPayrollId] IS NULL AND [MaterialRequestPayroll].[MaterialRequestPayrollId] IS NOT NULL) OR (([Source].[MaterialRequestPayrollId] IS NOT NULL AND [MaterialRequestPayroll].[MaterialRequestPayrollId] IS NOT NULL) AND [Source].[MaterialRequestPayrollId] != [MaterialRequestPayroll].[MaterialRequestPayrollId]))
	    OR (([Source].[PanelMembershipId] IS NOT NULL AND [MaterialRequestPayroll].[PanelMembershipId] IS NULL) OR ([Source].[PanelMembershipId] IS NULL AND [MaterialRequestPayroll].[PanelMembershipId] IS NOT NULL) OR (([Source].[PanelMembershipId] IS NOT NULL AND [MaterialRequestPayroll].[PanelMembershipId] IS NOT NULL) AND [Source].[PanelMembershipId] != [MaterialRequestPayroll].[PanelMembershipId]))
		OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestPayroll].[MaterialRequestId] IS NULL) OR ([Source].[MaterialRequestId] IS NULL AND [MaterialRequestPayroll].[MaterialRequestId] IS NOT NULL) OR (([Source].[MaterialRequestId] IS NOT NULL AND [MaterialRequestPayroll].[MaterialRequestId] IS NOT NULL) AND [Source].[MaterialRequestId] != [MaterialRequestPayroll].[MaterialRequestId]))
		OR (([Source].[ApprovedDate] IS NOT NULL AND [MaterialRequestPayroll].[ApprovedDate] IS NULL) OR ([Source].[ApprovedDate] IS NULL AND [MaterialRequestPayroll].[ApprovedDate] IS NOT NULL) OR (([Source].[ApprovedDate] IS NOT NULL AND [MaterialRequestPayroll].[ApprovedDate] IS NOT NULL) AND [Source].[ApprovedDate] != [MaterialRequestPayroll].[ApprovedDate]))
		OR (([Source].[ApprovedByUserId] IS NOT NULL AND [MaterialRequestPayroll].[ApprovedByUserId] IS NULL) OR ([Source].[ApprovedByUserId] IS NULL AND [MaterialRequestPayroll].[ApprovedByUserId] IS NOT NULL) OR (([Source].[ApprovedByUserId] IS NOT NULL AND [MaterialRequestPayroll].[ApprovedByUserId] IS NOT NULL) AND [Source].[ApprovedByUserId] != [MaterialRequestPayroll].[ApprovedByUserId]))
		OR (([Source].[PaidByUserId] IS NOT NULL AND [MaterialRequestPayroll].[PaidByUserId] IS NULL) OR ([Source].[PaidByUserId] IS NULL AND [MaterialRequestPayroll].[PaidByUserId] IS NOT NULL) OR (([Source].[PaidByUserId] IS NOT NULL AND [MaterialRequestPayroll].[PaidByUserId] IS NOT NULL) AND [Source].[PaidByUserId] != [MaterialRequestPayroll].[PaidByUserId]))

		OR (([Source].[PaymentDate] IS NOT NULL AND [MaterialRequestPayroll].[PaymentDate] IS NULL) OR ([Source].[PaymentDate] IS NULL AND [MaterialRequestPayroll].[PaymentDate] IS NOT NULL) OR (([Source].[PaymentDate] IS NOT NULL AND [MaterialRequestPayroll].[PaymentDate] IS NOT NULL) AND [Source].[PaymentDate] != [MaterialRequestPayroll].[PaymentDate]))
		OR (([Source].[Amount] IS NOT NULL AND [MaterialRequestPayroll].[Amount] IS NULL) OR ([Source].[Amount] IS NULL AND [MaterialRequestPayroll].[Amount] IS NOT NULL) OR (([Source].[Amount] IS NOT NULL AND [MaterialRequestPayroll].[Amount] IS NOT NULL) AND [Source].[Amount] != [MaterialRequestPayroll].[Amount]))
		OR (([Source].[PaymentReference] IS NOT NULL AND [MaterialRequestPayroll].[PaymentReference] IS NULL) OR ([Source].[PaymentReference] IS NULL AND [MaterialRequestPayroll].[PaymentReference] IS NOT NULL) OR (([Source].[PaymentReference] IS NOT NULL AND [MaterialRequestPayroll].[PaymentReference] IS NOT NULL) AND [Source].[PaymentReference] != [MaterialRequestPayroll].[PaymentReference]))

	--select * from @MaterialRequestPayrollHistory
		
	BEGIN TRANSACTION
		--Merge operation delete
		MERGE MaterialRequestPayrollHistory AS Target USING(select * from @MaterialRequestPayrollHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[MaterialRequestPayrollId] = Source.[MaterialRequestPayrollId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE MaterialRequestPayrollHistory AS Target USING(select * from @MaterialRequestPayrollHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MaterialRequestPayrollId] = Source.[MaterialRequestPayrollId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE MaterialRequestPayrollHistory AS Target USING(select * from @MaterialRequestPayrollHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[MaterialRequestPayrollId] = Source.[MaterialRequestPayrollId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.MaterialRequestPayrollId = Source.MaterialRequestPayrollId
		  ,Target.PanelMembershipId = Source.PanelMembershipId
		  ,Target.MaterialRequestId = Source.MaterialRequestId
		  ,Target.ApprovedDate = Source.ApprovedDate
		  ,Target.ApprovedByUserId = Source.ApprovedByUserId
		  ,Target.PaidByUserId = Source.PaidByUserId      
		  ,Target.PaymentDate = Source.PaymentDate
		  ,Target.Amount = Source.Amount
		  ,Target.PaymentReference = Source.PaymentReference
		  ,Target.ModifiedDate = @Date

		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([MaterialRequestPayrollId],[PanelMembershipId],[MaterialRequestId],[ApprovedDate],[ApprovedByUserId],[PaidByUserId],[PaymentDate],
			 [Amount],[PaymentReference],[ModifiedDate],[RowStatus])
		 VALUES (Source.[MaterialRequestPayrollId],Source.[PanelMembershipId],Source.[MaterialRequestId],Source.[ApprovedDate],Source.[ApprovedByUserId],Source.[PaidByUserId],Source.[PaymentDate],
			 Source.[Amount],Source.[PaymentReference],@Date, 'Latest');

	
	  COMMIT TRANSACTION;	
	
END