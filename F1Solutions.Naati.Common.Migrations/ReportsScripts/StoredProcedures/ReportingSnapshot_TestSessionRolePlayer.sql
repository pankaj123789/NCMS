ALTER PROCEDURE [dbo].[ReportingSnapshot_TestSessionRolePlayer]
	@Date DateTime
AS
BEGIN	
	DECLARE @TestSessionRolePlayerHistory as table(	[TestSessionRolePlayerId] [int] NOT NULL,[TestSessionId] [int] NOT NULL,[TestSessionName] [nvarchar](200) NULL,	[TestLocationName] [nvarchar](1000) NULL,[TestLocationState] [nvarchar](50) NULL,[TestLocationCountry] [nvarchar](50) NULL,
	[TestDate] [datetime] NULL,[CredentialTypeInternalName] [nvarchar](100) NULL,[CredentialTypeExternalName] [nvarchar](100) NULL,[PersonId] [int] NOT NULL,[CustomerNo] [int] NULL,[Status] [nvarchar](100) NULL,[Rehearsed] [bit] NOT NULL,[Attended] [bit] NOT NULL,
	[Rejected] [bit] NOT NULL,ModifiedDate datetime not null DEFAULT GETDATE(), [RowStatus] nvarchar(50))   
	
	IF(@Date is NULL)
	set @Date = GETDATE()
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @TestSessionRolePlayerHistory
	SELECT 
		CASE WHEN [Source].[TestSessionRolePlayerId] IS NULL THEN [TestSessionRolePlayer].[TestSessionRolePlayerId] ELSE [Source].[TestSessionRolePlayerId] END AS [TestSessionRolePlayerId]
		,CASE WHEN [Source].[TestSessionId] IS NULL THEN [TestSessionRolePlayer].[TestSessionId] ELSE [Source].[TestSessionId] END AS [TestSessionId]		
		,CASE WHEN [Source].[TestSessionName] IS NULL THEN [TestSessionRolePlayer].[TestSessionName] ELSE [Source].[TestSessionName] END AS [TestSessionName]
		,CASE WHEN [Source].[TestLocationState] IS NULL THEN [TestSessionRolePlayer].[TestLocationState] ELSE [Source].[TestLocationState] END AS [TestLocationState]
		,CASE WHEN [Source].[TestLocationCountry] IS NULL THEN [TestSessionRolePlayer].[TestLocationCountry] ELSE [Source].[TestLocationCountry] END AS [TestLocationCountry]
		,CASE WHEN [Source].[TestLocationName] IS NULL THEN [TestSessionRolePlayer].[TestLocationName] ELSE [Source].[TestLocationName] END AS [TestLocationName]		
		,CASE WHEN [Source].[TestDate] IS NULL THEN [TestSessionRolePlayer].[TestDate] ELSE [Source].[TestDate] END AS [TestDate]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [TestSessionRolePlayer].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [TestSessionRolePlayer].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]		
		,CASE WHEN [Source].[PersonId] IS NULL THEN [TestSessionRolePlayer].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNo] IS NULL THEN [TestSessionRolePlayer].[CustomerNo] ELSE [Source].[CustomerNo] END AS [CustomerNo]
		,CASE WHEN [Source].[Status] IS NULL THEN [TestSessionRolePlayer].[Status] ELSE [Source].[Status] END AS [Status]

		,CASE WHEN [Source].[Rehearsed] IS NULL THEN [TestSessionRolePlayer].[Rehearsed] ELSE [Source].[Rehearsed] END AS [Rehearsed]
		,CASE WHEN [Source].[Attended] IS NULL THEN [TestSessionRolePlayer].[Attended] ELSE [Source].[Attended] END AS [Attended]
		,CASE WHEN [Source].[Rejected] IS NULL THEN [TestSessionRolePlayer].[Rejected] ELSE [Source].[Rejected] END AS [Rejected]
		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[TestSessionRolePlayerId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
	FROM 
	(
		SELECT 
			tsrp.TestSessionRolePlayerId
			,ts.TestSessionId TestSessionId			
			,ts.Name TestSessionName			
			
			,iname.Abbreviation TestLocationState
			,c.Name TestLocationCountry
			,tl.Name TestLocationName
			
			,ts.TestDateTime TestDate			
			,ct.InternalName CredentialTypeInternalName
			,ct.ExternalName CredentialTypeExternalName
						
			,rp.PersonId PersonId
			,e.NAATINumber CustomerNo
			,rpst.DisplayName Status
			
			,tsrp.Rehearsed
			,tsrp.Attended
			,tsrp.Rejected
			
		FROM
            [naati_db]..[tblTestSessionRolePlayer] tsrp		
		    INNER JOIN [naati_db]..[tblTestSession] ts ON tsrp.TestSessionId = ts.TestSessionId
			INNER JOIN [naati_db]..[tblRolePlayer] rp ON tsrp.RolePlayerId = rp.RolePlayerId
			INNER JOIN [naati_db]..[tblRolePlayerStatusType] rpst ON tsrp.RolePlayerStatusTypeId = rpst.RolePlayerStatusTypeId
			INNER JOIN [naati_db]..[tblPerson] p ON rp.PersonId = p.PersonId
			INNER JOIN [naati_db]..[tblEntity] e ON p.EntityId = e.EntityId
			
			INNER JOIN [naati_db]..[tblVenue] v ON ts.VenueId = v.VenueId
			INNER JOIN [naati_db]..[tblTestLocation] tl ON v.TestLocationId = tl.TestLocationId
						
			INNER JOIN [naati_db]..[tblOffice] o ON tl.OfficeId = o.OfficeId
			INNER JOIN [naati_db]..[tblInstitution] i ON o.InstitutionId = i.InstitutionId
			INNER JOIN [naati_db]..[vwDistinctInstitutionName] vwin ON i.InstitutionId = vwin.InstitutionId
			INNER JOIN [naati_db]..[tblInstitutionName] iname ON vwin.InstitutionNameId = iname.InstitutionNameId

			LEFT JOIN [naati_db]..[tblCountry] c ON tl.CountryId = c.CountryId			
			LEFT JOIN [naati_db]..[tblCredentialType] ct ON ts.CredentialTypeId = ct.CredentialTypeId
			
	) [Source]
	FULL OUTER JOIN [TestSessionRolePlayer] ON ([Source].[TestSessionRolePlayerId] = [TestSessionRolePlayer].[TestSessionRolePlayerId] AND [Source].[PersonId] = [TestSessionRolePlayer].[PersonId])
	WHERE 
		(([Source].[TestSessionRolePlayerId] IS NOT NULL AND [TestSessionRolePlayer].[TestSessionRolePlayerId] IS NULL) OR ([Source].[TestSessionRolePlayerId] IS NULL AND [TestSessionRolePlayer].[TestSessionRolePlayerId] IS NOT NULL) OR (([Source].[TestSessionRolePlayerId] IS NOT NULL AND [TestSessionRolePlayer].[TestSessionRolePlayerId] IS NOT NULL) AND [Source].[TestSessionRolePlayerId] != [TestSessionRolePlayer].[TestSessionRolePlayerId]))
		OR (([Source].[TestSessionId] IS NOT NULL AND [TestSessionRolePlayer].[TestSessionId] IS NULL) OR ([Source].[TestSessionId] IS NULL AND [TestSessionRolePlayer].[TestSessionId] IS NOT NULL) OR (([Source].[TestSessionId] IS NOT NULL AND [TestSessionRolePlayer].[TestSessionId] IS NOT NULL) AND [Source].[TestSessionId] != [TestSessionRolePlayer].[TestSessionId]))		
		OR (([Source].[TestSessionName] IS NOT NULL AND [TestSessionRolePlayer].[TestSessionName] IS NULL) OR ([Source].[TestSessionName] IS NULL AND [TestSessionRolePlayer].[TestSessionName] IS NOT NULL) OR (([Source].[TestSessionName] IS NOT NULL AND [TestSessionRolePlayer].[TestSessionName] IS NOT NULL) AND [Source].[TestSessionName] != [TestSessionRolePlayer].[TestSessionName]))
		OR (([Source].[TestLocationState] IS NOT NULL AND [TestSessionRolePlayer].[TestLocationState] IS NULL) OR ([Source].[TestLocationState] IS NULL AND [TestSessionRolePlayer].[TestLocationState] IS NOT NULL) OR (([Source].[TestLocationState] IS NOT NULL AND [TestSessionRolePlayer].[TestLocationState] IS NOT NULL) AND [Source].[TestLocationState] != [TestSessionRolePlayer].[TestLocationState]))
		OR (([Source].[TestLocationCountry] IS NOT NULL AND [TestSessionRolePlayer].[TestLocationCountry] IS NULL) OR ([Source].[TestLocationCountry] IS NULL AND [TestSessionRolePlayer].[TestLocationCountry] IS NOT NULL) OR (([Source].[TestLocationCountry] IS NOT NULL AND [TestSessionRolePlayer].[TestLocationCountry] IS NOT NULL) AND [Source].[TestLocationCountry] != [TestSessionRolePlayer].[TestLocationCountry]))
		OR (([Source].[TestLocationName] IS NOT NULL AND [TestSessionRolePlayer].[TestLocationName] IS NULL) OR ([Source].[TestLocationName] IS NULL AND [TestSessionRolePlayer].[TestLocationName] IS NOT NULL) OR (([Source].[TestLocationName] IS NOT NULL AND [TestSessionRolePlayer].[TestLocationName] IS NOT NULL) AND [Source].[TestLocationName] != [TestSessionRolePlayer].[TestLocationName]))
		OR (([Source].[TestDate] IS NOT NULL AND [TestSessionRolePlayer].[TestDate] IS NULL) OR ([Source].[TestDate] IS NULL AND [TestSessionRolePlayer].[TestDate] IS NOT NULL) OR (([Source].[TestDate] IS NOT NULL AND [TestSessionRolePlayer].[TestDate] IS NOT NULL) AND [Source].[TestDate] != [TestSessionRolePlayer].[TestDate]))
		OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestSessionRolePlayer].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [TestSessionRolePlayer].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [TestSessionRolePlayer].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [TestSessionRolePlayer].[CredentialTypeInternalName]))
		OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestSessionRolePlayer].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [TestSessionRolePlayer].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [TestSessionRolePlayer].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [TestSessionRolePlayer].[CredentialTypeExternalName]))
		OR (([Source].[PersonId] IS NOT NULL AND [TestSessionRolePlayer].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [TestSessionRolePlayer].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [TestSessionRolePlayer].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [TestSessionRolePlayer].[PersonId]))
		OR (([Source].[CustomerNo] IS NOT NULL AND [TestSessionRolePlayer].[CustomerNo] IS NULL) OR ([Source].[CustomerNo] IS NULL AND [TestSessionRolePlayer].[CustomerNo] IS NOT NULL) OR (([Source].[CustomerNo] IS NOT NULL AND [TestSessionRolePlayer].[CustomerNo] IS NOT NULL) AND [Source].[CustomerNo] != [TestSessionRolePlayer].[CustomerNo]))		
		OR (([Source].[Status] IS NOT NULL AND [TestSessionRolePlayer].[Status] IS NULL) OR ([Source].[Status] IS NULL AND [TestSessionRolePlayer].[Status] IS NOT NULL) OR (([Source].[Status] IS NOT NULL AND [TestSessionRolePlayer].[Status] IS NOT NULL) AND [Source].[Status] != [TestSessionRolePlayer].[Status]))			

		OR (([Source].[Rehearsed] IS NOT NULL AND [TestSessionRolePlayer].[Rehearsed] IS NULL) OR ([Source].[Rehearsed] IS NULL AND [TestSessionRolePlayer].[Rehearsed] IS NOT NULL) OR (([Source].[Rehearsed] IS NOT NULL AND [TestSessionRolePlayer].[Rehearsed] IS NOT NULL) AND [Source].[Rehearsed] != [TestSessionRolePlayer].[Rehearsed]))			
		OR (([Source].[Attended] IS NOT NULL AND [TestSessionRolePlayer].[Attended] IS NULL) OR ([Source].[Attended] IS NULL AND [TestSessionRolePlayer].[Attended] IS NOT NULL) OR (([Source].[Attended] IS NOT NULL AND [TestSessionRolePlayer].[Attended] IS NOT NULL) AND [Source].[Attended] != [TestSessionRolePlayer].[Attended]))			
		OR (([Source].[Rejected] IS NOT NULL AND [TestSessionRolePlayer].[Rejected] IS NULL) OR ([Source].[Rejected] IS NULL AND [TestSessionRolePlayer].[Rejected] IS NOT NULL) OR (([Source].[Rejected] IS NOT NULL AND [TestSessionRolePlayer].[Rejected] IS NOT NULL) AND [Source].[Rejected] != [TestSessionRolePlayer].[Rejected]))			

	    --select * from @TestSessionRolePlayerHistory
				
		BEGIN TRANSACTION
		
	   --Merge operation delete
		MERGE TestSessionRolePlayerHistory AS Target USING(select * from @TestSessionRolePlayerHistory where [RowStatus] = 'Deleted' ) AS Source ON (Target.TestSessionRolePlayerId = Source.TestSessionRolePlayerId  AND Target.PersonId = Source.PersonId AND Target.RowStatus = 'Latest')
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE TestSessionRolePlayerHistory AS Target USING(	select * from @TestSessionRolePlayerHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.TestSessionRolePlayerId = Source.TestSessionRolePlayerId AND Target.PersonId = Source.PersonId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE TestSessionRolePlayerHistory AS Target USING(	select * from @TestSessionRolePlayerHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.TestSessionRolePlayerId = Source.TestSessionRolePlayerId AND Target.PersonId = Source.PersonId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     
			Target.[TestSessionRolePlayerId] = Source.[TestSessionRolePlayerId]
			,Target.[TestSessionId] = Source.[TestSessionId]
			,Target.[TestSessionName] = Source.[TestSessionName]
			,Target.[TestLocationState] = Source.[TestLocationState]
			,Target.[TestLocationCountry] = Source.[TestLocationCountry]
			,Target.[TestLocationName] = Source.[TestLocationName]      
			,Target.[TestDate] = Source.[TestDate]
			,Target.[CredentialTypeInternalName] = Source.[CredentialTypeInternalName]
			,Target.[CredentialTypeExternalName] = Source.[CredentialTypeExternalName]

			,Target.[PersonId] = Source.[PersonId]
			,Target.[CustomerNo] = Source.[CustomerNo]
			,Target.[Status] = Source.[Status]
			,Target.[Rehearsed] = Source.[Rehearsed]
			,Target.[Attended] = Source.[Attended]
			,Target.[Rejected] = Source.[Rejected]

		  ,Target.[ModifiedDate] = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
			INSERT(TestSessionRolePlayerId, TestSessionId, TestSessionName,TestLocationName,TestLocationState,TestLocationCountry,TestDate,CredentialTypeInternalName,CredentialTypeExternalName,PersonId,CustomerNo,[Status],Rehearsed,Attended,Rejected,ModifiedDate, [RowStatus])
			VALUES (Source.TestSessionRolePlayerId,Source.TestSessionId,Source.TestSessionName,Source.TestLocationName,Source.TestLocationState,Source.TestLocationCountry,Source.TestDate,Source.CredentialTypeInternalName,Source.CredentialTypeExternalName,Source.PersonId,Source.CustomerNo,Source.[Status],Source.Rehearsed,Source.Attended,Source.Rejected,@Date, 'Latest');
	 	  	
	COMMIT TRANSACTION;	

	END