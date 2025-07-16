ALTER PROCEDURE [dbo].[ReportingSnapshot_PanelMembers]
	@Date DateTime
AS
BEGIN
	DECLARE @PanelMembersHistory as table([PanelMembersId] [int] NOT NULL,[PanelId] [int] NOT NULL,[PersonName] [nvarchar](252) NULL,[ModifiedDate] [datetime] NOT NULL,[NAATINumber] [nvarchar](50) NOT NULL,[Role] [nvarchar](50) NULL,[StartDate] [datetime] NOT NULL,[EndDate] [datetime] NULL,
									      [PersonId] [int] NOT NULL, [RowStatus] nvarchar(50))   
	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @PanelMembersHistory
	SELECT
		 CASE WHEN [Source].[PanelMembersId] IS NULL THEN [PanelMembers].[PanelMembersId] ELSE [Source].[PanelMembersId] END AS [PanelMembersId]		
		,CASE WHEN [Source].[PanelId] IS NULL THEN [PanelMembers].[PanelId] ELSE [Source].[PanelId] END AS [PanelId]
		,CASE WHEN [Source].[PersonName] IS NULL THEN [PanelMembers].[PersonName] ELSE [Source].[PersonName] END AS [PersonName]
		
		,@Date AS [ModifiedDate]
		
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [PanelMembers].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[Role] IS NULL THEN [PanelMembers].[Role] ELSE [Source].[Role] END AS [Role]
		,CASE WHEN [Source].[StartDate] IS NULL THEN [PanelMembers].[StartDate] ELSE [Source].[StartDate] END AS [StartDate]
		,CASE WHEN [Source].[EndDate] IS NULL THEN [PanelMembers].[EndDate] ELSE [Source].[EndDate] END AS [EndDate]		
		,CASE WHEN [Source].[PersonId] IS NULL THEN [PanelMembers].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		
		,CASE WHEN [Source].[PanelMembersId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]		
	FROM
	(
		SELECT
			p.[PanelId]
			,pm.[PanelMembershipId] AS 'PanelMembersId'
			,e.[NAATINumber]
			,(SELECT TOP 1 CASE WHEN pn.TitleId IS NULL THEN [GivenName] + ' ' + [SurName] ELSE [Title] + ' ' + [GivenName] + ' ' + [SurName] END AS [PersonName]
			  FROM [naati_db]..[tblPersonName] pn
					INNER JOIN [naati_db]..vwDistinctPersonName dpn on dpn.PersonId = pn.PersonId
					LEFT JOIN [naati_db]..[tluTitle] t ON pn.[TitleId] = t.TitleId
					WHERE pn.PersonId = psn.PersonId
					ORDER BY pn.[EffectiveDate] desc) COLLATE Latin1_General_CI_AS AS 'PersonName'
			,psn.[PersonId]
			,rt.[Name] COLLATE Latin1_General_CI_AS AS 'Role'
			,pm.[StartDate]
			,pm.[EndDate]
						
		FROM [naati_db]..[tblPanelMembership] pm
		LEFT JOIN [naati_db]..[tblPanel] p ON pm.[PanelId] = p.[PanelId]
		LEFT JOIN [naati_db]..[tblPerson] psn ON pm.[PersonId] = psn.[PersonId]
		LEFT JOIN [naati_db]..[tblEntity] e ON psn.EntityId = e.EntityId
		LEFT JOIN [naati_db]..[tblPanelRole] rt ON pm.[PanelRoleId] = rt.[PanelRoleId]
	) [Source]
	FULL OUTER JOIN [PanelMembers] ON [Source].[PanelMembersId] = [PanelMembers].[PanelMembersId]
	WHERE (([Source].[PersonName] IS NOT NULL AND [PanelMembers].[PersonName] IS NULL) OR ([Source].[PersonName] IS NULL AND [PanelMembers].[PersonName] IS NOT NULL) OR (([Source].[PersonName] IS NOT NULL AND [PanelMembers].[PersonName] IS NOT NULL) AND [Source].[PersonName] != [PanelMembers].[PersonName]))
	OR (([Source].[NAATINumber] IS NOT NULL AND [PanelMembers].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [PanelMembers].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [PanelMembers].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [PanelMembers].[NAATINumber]))
	OR (([Source].[PersonId] IS NOT NULL AND [PanelMembers].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [PanelMembers].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [PanelMembers].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [PanelMembers].[PersonId]))
	OR (([Source].[PanelId] IS NOT NULL AND [PanelMembers].[PanelId] IS NULL) OR ([Source].[PanelId] IS NULL AND [PanelMembers].[PanelId] IS NOT NULL) OR (([Source].[PanelId] IS NOT NULL AND [PanelMembers].[PanelId] IS NOT NULL) AND [Source].[PanelId] != [PanelMembers].[PanelId]))
	OR (([Source].[Role] IS NOT NULL AND [PanelMembers].[Role] IS NULL) OR ([Source].[Role] IS NULL AND [PanelMembers].[Role] IS NOT NULL) OR (([Source].[Role] IS NOT NULL AND [PanelMembers].[Role] IS NOT NULL) AND [Source].[Role] != [PanelMembers].[Role]))
	OR (([Source].[StartDate] IS NOT NULL AND [PanelMembers].[StartDate] IS NULL) OR ([Source].[StartDate] IS NULL AND [PanelMembers].[StartDate] IS NOT NULL) OR (([Source].[StartDate] IS NOT NULL AND [PanelMembers].[StartDate] IS NOT NULL) AND [Source].[StartDate] != [PanelMembers].[StartDate]))
	OR (([Source].[EndDate] IS NOT NULL AND [PanelMembers].[EndDate] IS NULL) OR ([Source].[EndDate] IS NULL AND [PanelMembers].[EndDate] IS NOT NULL) OR (([Source].[EndDate] IS NOT NULL AND [PanelMembers].[EndDate] IS NOT NULL) AND [Source].[EndDate] != [PanelMembers].[EndDate]))

	--select * from @PanelMembersHistory
		
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE PanelMembersHistory AS Target USING(select * from @PanelMembersHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[PanelMembersId] = Source.[PanelMembersId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
	      Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE PanelMembersHistory AS Target USING(	select * from @PanelMembersHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[PanelMembersId] = Source.[PanelMembersId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE PanelMembersHistory AS Target USING(	select * from @PanelMembersHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[PanelMembersId] = Source.[PanelMembersId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.PanelMembersId = Source.PanelMembersId
		  ,Target.PanelId = Source.PanelId
		  ,Target.PersonName = Source.PersonName		  
		  ,Target.NAATINumber = Source.NAATINumber
		  ,Target.Role = Source.Role      
		  ,Target.StartDate = Source.StartDate
		  ,Target.EndDate = Source.EndDate
		  ,Target.PersonId = Source.PersonId
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
			INSERT([PanelMembersId],[PanelId],[PersonName],[ModifiedDate],[NAATINumber],[Role],[StartDate],[EndDate],[PersonId],[RowStatus])	  
			VALUES (Source.[PanelMembersId],Source.[PanelId],Source.[PersonName],@Date,Source.[NAATINumber],Source.[Role],Source.[StartDate],Source.[EndDate],Source.[PersonId],'Latest');

	COMMIT TRANSACTION;	

END
