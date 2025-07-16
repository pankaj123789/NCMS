ALTER PROCEDURE [dbo].[ReportingSnapshot_TestSessionRolePlayerDetails]
	@Date DateTime
AS
BEGIN	
DECLARE @TestSessionRolePlayerDetailsHistory as table([TestSessionRolePlayerDetailId] [int] NOT NULL,[TestSessionId] [int] NOT NULL,[TestSessionName] [nvarchar](200) NULL,[TestDate] [datetime] NULL,[PersonId] [int] NOT NULL,[CustomerNo] [int] NULL,[Status] [nvarchar](100) NULL,
													  [TaskName] [nvarchar](100) NULL,[Language] [nvarchar](100) NULL,[Position] [nvarchar](100) NULL,[ModifiedDate] [datetime] NOT NULL,[RowStatus] nvarchar(50))   
 	

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @TestSessionRolePlayerDetailsHistory

	SELECT 
		CASE WHEN [Source].[TestSessionRolePlayerDetailId] IS NULL THEN [TestSessionRolePlayerDetails].[TestSessionRolePlayerDetailId] ELSE [Source].[TestSessionRolePlayerDetailId] END AS [TestSessionRolePlayerDetailId]		
		,CASE WHEN [Source].[TestSessionId] IS NULL THEN [TestSessionRolePlayerDetails].[TestSessionId] ELSE [Source].[TestSessionId] END AS [TestSessionId]		
		,CASE WHEN [Source].[TestSessionName] IS NULL THEN [TestSessionRolePlayerDetails].[TestSessionName] ELSE [Source].[TestSessionName] END AS [TestSessionName]
		,CASE WHEN [Source].[TestDate] IS NULL THEN [TestSessionRolePlayerDetails].[TestDate] ELSE [Source].[TestDate] END AS [TestDate]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [TestSessionRolePlayerDetails].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNo] IS NULL THEN [TestSessionRolePlayerDetails].[CustomerNo] ELSE [Source].[CustomerNo] END AS [CustomerNo]
		,CASE WHEN [Source].[Status] IS NULL THEN [TestSessionRolePlayerDetails].[Status] ELSE [Source].[Status] END AS [Status]

		,CASE WHEN [Source].[TaskName] IS NULL THEN [TestSessionRolePlayerDetails].[TaskName] ELSE [Source].[TaskName] END AS [TaskName]
		,CASE WHEN [Source].[Language] IS NULL THEN [TestSessionRolePlayerDetails].[Language] ELSE [Source].[Language] END AS [Language]
		,CASE WHEN [Source].[Position] IS NULL THEN [TestSessionRolePlayerDetails].[Position] ELSE [Source].[Position] END AS [Position]
		
		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[TestSessionRolePlayerDetailId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
		
	FROM 
	(
		SELECT 
	        tsrpd.TestSessionRolePlayerDetailId
			,tsrpd.TestSessionRolePlayerId
			,ts.TestSessionId TestSessionId			
			,ts.Name TestSessionName						
			,ts.TestDateTime TestDate			
						
			,rp.PersonId PersonId
			,e.NAATINumber CustomerNo
			,rpst.DisplayName Status
			,tc.Name TaskName
			,l.Name [Language]
			,rprt.DisplayName Position
			
		FROM
            [naati_db]..[tblTestSessionRolePlayerDetail] tsrpd		
			INNER JOIN [naati_db]..[tblTestSessionRolePlayer] tsrp ON tsrpd.TestSessionRolePlayerId = tsrp.TestSessionRolePlayerId
			INNER JOIN [naati_db]..[tblTestComponent] tc on tsrpd.TestComponentId = tc.TestComponentId
			INNER JOIN [naati_db]..[tblLanguage] l on tsrpd.LanguageId = l.LanguageId
			INNER JOIN [naati_db]..[tblRolePlayerRoleType] rprt on tsrpd.RolePlayerRoleTypeId = rprt.RolePlayerRoleTypeId
		    INNER JOIN [naati_db]..[tblTestSession] ts ON tsrp.TestSessionId = ts.TestSessionId
			INNER JOIN [naati_db]..[tblRolePlayer] rp ON tsrp.RolePlayerId = rp.RolePlayerId
			INNER JOIN [naati_db]..[tblRolePlayerStatusType] rpst ON tsrp.RolePlayerStatusTypeId = rpst.RolePlayerStatusTypeId
			INNER JOIN [naati_db]..[tblPerson] p ON rp.PersonId = p.PersonId
			INNER JOIN [naati_db]..[tblEntity] e ON p.EntityId = e.EntityId
			
	) [Source]
	FULL OUTER JOIN [TestSessionRolePlayerDetails] ON ([Source].[TestSessionRolePlayerDetailId] = [TestSessionRolePlayerDetails].[TestSessionRolePlayerDetailId] AND [Source].PersonId = [TestSessionRolePlayerDetails].PersonId)
	WHERE 
		(([Source].[TestSessionRolePlayerDetailId] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestSessionRolePlayerDetailId] IS NULL) OR ([Source].[TestSessionRolePlayerDetailId] IS NULL AND [TestSessionRolePlayerDetails].[TestSessionRolePlayerDetailId] IS NOT NULL) OR (([Source].[TestSessionRolePlayerDetailId] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestSessionRolePlayerDetailId] IS NOT NULL) AND [Source].[TestSessionRolePlayerDetailId] != [TestSessionRolePlayerDetails].[TestSessionRolePlayerDetailId]))
		OR (([Source].[TestSessionId] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestSessionId] IS NULL) OR ([Source].[TestSessionId] IS NULL AND [TestSessionRolePlayerDetails].[TestSessionId] IS NOT NULL) OR (([Source].[TestSessionId] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestSessionId] IS NOT NULL) AND [Source].[TestSessionId] != [TestSessionRolePlayerDetails].[TestSessionId]))		
		OR (([Source].[TestSessionName] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestSessionName] IS NULL) OR ([Source].[TestSessionName] IS NULL AND [TestSessionRolePlayerDetails].[TestSessionName] IS NOT NULL) OR (([Source].[TestSessionName] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestSessionName] IS NOT NULL) AND [Source].[TestSessionName] != [TestSessionRolePlayerDetails].[TestSessionName]))
		OR (([Source].[TestDate] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestDate] IS NULL) OR ([Source].[TestDate] IS NULL AND [TestSessionRolePlayerDetails].[TestDate] IS NOT NULL) OR (([Source].[TestDate] IS NOT NULL AND [TestSessionRolePlayerDetails].[TestDate] IS NOT NULL) AND [Source].[TestDate] != [TestSessionRolePlayerDetails].[TestDate]))
		OR (([Source].[PersonId] IS NOT NULL AND [TestSessionRolePlayerDetails].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [TestSessionRolePlayerDetails].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [TestSessionRolePlayerDetails].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [TestSessionRolePlayerDetails].[PersonId]))
		OR (([Source].[CustomerNo] IS NOT NULL AND [TestSessionRolePlayerDetails].[CustomerNo] IS NULL) OR ([Source].[CustomerNo] IS NULL AND [TestSessionRolePlayerDetails].[CustomerNo] IS NOT NULL) OR (([Source].[CustomerNo] IS NOT NULL AND [TestSessionRolePlayerDetails].[CustomerNo] IS NOT NULL) AND [Source].[CustomerNo] != [TestSessionRolePlayerDetails].[CustomerNo]))		
		OR (([Source].[Status] IS NOT NULL AND [TestSessionRolePlayerDetails].[Status] IS NULL) OR ([Source].[Status] IS NULL AND [TestSessionRolePlayerDetails].[Status] IS NOT NULL) OR (([Source].[Status] IS NOT NULL AND [TestSessionRolePlayerDetails].[Status] IS NOT NULL) AND [Source].[Status] != [TestSessionRolePlayerDetails].[Status]))			

		OR (([Source].[TaskName] IS NOT NULL AND [TestSessionRolePlayerDetails].[TaskName] IS NULL) OR ([Source].[TaskName] IS NULL AND [TestSessionRolePlayerDetails].[TaskName] IS NOT NULL) OR (([Source].[TaskName] IS NOT NULL AND [TestSessionRolePlayerDetails].[TaskName] IS NOT NULL) AND [Source].[TaskName] != [TestSessionRolePlayerDetails].[TaskName]))			
		OR (([Source].[Language] IS NOT NULL AND [TestSessionRolePlayerDetails].[Language] IS NULL) OR ([Source].[Language] IS NULL AND [TestSessionRolePlayerDetails].[Language] IS NOT NULL) OR (([Source].[Language] IS NOT NULL AND [TestSessionRolePlayerDetails].[Language] IS NOT NULL) AND [Source].[Language] != [TestSessionRolePlayerDetails].[Language]))			
		OR (([Source].[Position] IS NOT NULL AND [TestSessionRolePlayerDetails].[Position] IS NULL) OR ([Source].[Position] IS NULL AND [TestSessionRolePlayerDetails].[Position] IS NOT NULL) OR (([Source].[Position] IS NOT NULL AND [TestSessionRolePlayerDetails].[Position] IS NOT NULL) AND [Source].[Position] != [TestSessionRolePlayerDetails].[Position]))			

	  --select * from @TestSessionRolePlayerDetailsHistory
	  	  
	  BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE TestSessionRolePlayerDetailsHistory AS Target USING(select * from @TestSessionRolePlayerDetailsHistory where [RowStatus] = 'Deleted' ) AS Source ON 
			(Target.[TestSessionRolePlayerDetailId] = Source.[TestSessionRolePlayerDetailId] AND Target.PersonId = Source.PersonId AND Target.RowStatus = 'Latest')
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestSessionRolePlayerDetailsHistory AS Target USING(	select * from @TestSessionRolePlayerDetailsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON 
			(Target.[TestSessionRolePlayerDetailId] = Source.[TestSessionRolePlayerDetailId] AND Target.PersonId = Source.PersonId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestSessionRolePlayerDetailsHistory AS Target USING(	select * from @TestSessionRolePlayerDetailsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON 
			(Target.[TestSessionRolePlayerDetailId] = Source.[TestSessionRolePlayerDetailId] AND Target.PersonId = Source.PersonId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.TestSessionRolePlayerDetailId = Source.TestSessionRolePlayerDetailId
		  ,Target.TestSessionId = Source.TestSessionId
		  ,Target.TestSessionName = Source.TestSessionName
		  ,Target.TestDate = Source.TestDate
		  ,Target.PersonId = Source.PersonId
		  ,Target.CustomerNo = Source.CustomerNo      
		  ,Target.Status = Source.Status
		  ,Target.TaskName = Source.TaskName
		  ,Target.Language = Source.Language
		  ,Target.Position = Source.Position

		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		  INSERT([TestSessionRolePlayerDetailId],[TestSessionId],[TestSessionName],[TestDate],[PersonId],[CustomerNo],[Status],[TaskName],[Language],[Position],[ModifiedDate],[RowStatus])	  
			VALUES (Source.[TestSessionRolePlayerDetailId],Source.[TestSessionId],Source.[TestSessionName],Source.[TestDate],Source.[PersonId],Source.[CustomerNo],Source.[Status],Source.[TaskName],Source.[Language],Source.[Position], @Date, 'Latest');
	    	
	COMMIT TRANSACTION;	

END