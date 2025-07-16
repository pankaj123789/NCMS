ALTER PROCEDURE [dbo].[ReportingSnapshot_PanelMembershipCredentialTypes]
	@Date DateTime
AS
BEGIN	
DECLARE @PanelMembershipCredentialTypesHistory as table([PanelMemberShipCredentialTypeId] [int] NOT NULL,[PanelMembershipId] [int] NOT NULL,[PanelId] [int] NOT NULL,[PanelName] [nvarchar](200) NULL,[Role] [nvarchar](200) NULL,[PersonId] [int] NOT NULL,[PersonName] [nvarchar](400) NOT NULL,[NaatiNumber] [int] NOT NULL,
														[CredentialTypeInternalName] [nvarchar](200) NOT NULL,[CredentialTypeExternalName] [nvarchar](200) NULL,[ModifiedDate] [datetime] NOT NULL,	[RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @PanelMembershipCredentialTypesHistory

	SELECT 
		 CASE WHEN [Source].[PanelMemberShipCredentialTypeId] IS NULL THEN [PanelMembershipCredentialTypes].[PanelMemberShipCredentialTypeId] ELSE [Source].[PanelMemberShipCredentialTypeId] END AS [PanelMemberShipCredentialTypeId]
		,CASE WHEN [Source].[PanelMembershipId] IS NULL THEN [PanelMembershipCredentialTypes].[PanelMembershipId] ELSE [Source].[PanelMembershipId] END AS [PanelMembershipId]
		,CASE WHEN [Source].[PanelId] IS NULL THEN [PanelMembershipCredentialTypes].[PanelId] ELSE [Source].[PanelId] END AS [PanelId]		
		,CASE WHEN [Source].[PanelName] IS NULL THEN [PanelMembershipCredentialTypes].[PanelName] ELSE [Source].[PanelName] END AS [PanelName]
		,CASE WHEN [Source].[Role] IS NULL THEN [PanelMembershipCredentialTypes].[Role] ELSE [Source].[Role] END AS [Role]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [PanelMembershipCredentialTypes].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[PersonName] IS NULL THEN [PanelMembershipCredentialTypes].[PersonName] ELSE [Source].[PersonName] END AS [PersonName]		
		,CASE WHEN [Source].[NAATINumber] IS NULL THEN [PanelMembershipCredentialTypes].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [PanelMembershipCredentialTypes].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [PanelMembershipCredentialTypes].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]	
		
		,@Date AS [ModifiedDate]
		,CASE WHEN [Source].[PanelMembershipId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
		
	FROM 
	(
		SELECT 
			 plmct.PanelMemberShipCredentialTypeId
			,plm.PanelMembershipId
			,pl.PanelId
			,pl.Name [PanelName]
			,plr.Name [Role]
			,p.PersonId
			,pn.GivenName + ' ' + pn.Surname [PersonName]
			,e.NAATINumber
					
			,ct.InternalName CredentialTypeInternalName
			,ct.ExternalName CredentialTypeExternalName
			
		FROM
          [naati_db]..[tblPanelMemberShipCredentialType] plmct		
		  INNER JOIN [naati_db]..[tblPanelMembership] plm on plmct.PanelMemberShipId = plm.PanelMembershipId		
		  INNER JOIN [naati_db]..[tblPanel] pl ON plm.PanelId = pl.PanelId
		  INNER JOIN [naati_db]..[tblPanelRole] plr ON plm.PanelRoleId = plr.PanelRoleId
		  INNER JOIN [naati_db]..[tblPerson] p ON plm.PersonId = p.PersonId
		  INNER JOIN [naati_db]..[vwDistinctPersonName] vpn ON plm.PersonId = vpn.PersonId
		  INNER JOIN [naati_db]..[tblPersonName] pn ON vpn.PersonNameId = pn.PersonNameId
		  INNER JOIN [naati_db]..[tblEntity] e ON p.EntityId = e.EntityId
		  INNER JOIN [naati_db]..[tblCredentialType] ct ON plmct.CredentialTypeId = ct.CredentialTypeId
			
	) [Source]
	FULL OUTER JOIN [PanelMembershipCredentialTypes] ON [Source].[PanelMemberShipCredentialTypeId] = [PanelMembershipCredentialTypes].[PanelMemberShipCredentialTypeId]
	WHERE 
		(([Source].[PanelMemberShipCredentialTypeId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelMemberShipCredentialTypeId] IS NULL) OR ([Source].[PanelMemberShipCredentialTypeId] IS NULL AND [PanelMembershipCredentialTypes].[PanelMemberShipCredentialTypeId] IS NOT NULL) OR (([Source].[PanelMemberShipCredentialTypeId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelMemberShipCredentialTypeId] IS NOT NULL) AND [Source].[PanelMemberShipCredentialTypeId] != [PanelMembershipCredentialTypes].[PanelMemberShipCredentialTypeId]))		

		OR (([Source].[PanelMembershipId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelMembershipId] IS NULL) OR ([Source].[PanelMembershipId] IS NULL AND [PanelMembershipCredentialTypes].[PanelMembershipId] IS NOT NULL) OR (([Source].[PanelMembershipId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelMembershipId] IS NOT NULL) AND [Source].[PanelMembershipId] != [PanelMembershipCredentialTypes].[PanelMembershipId]))		
		OR (([Source].[PanelId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelId] IS NULL) OR ([Source].[PanelId] IS NULL AND [PanelMembershipCredentialTypes].[PanelId] IS NOT NULL) OR (([Source].[PanelId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelId] IS NOT NULL) AND [Source].[PanelId] != [PanelMembershipCredentialTypes].[PanelId]))		
		OR (([Source].[PanelName] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelName] IS NULL) OR ([Source].[PanelName] IS NULL AND [PanelMembershipCredentialTypes].[PanelName] IS NOT NULL) OR (([Source].[PanelName] IS NOT NULL AND [PanelMembershipCredentialTypes].[PanelName] IS NOT NULL) AND [Source].[PanelName] != [PanelMembershipCredentialTypes].[PanelName]))
		OR (([Source].[Role] IS NOT NULL AND [PanelMembershipCredentialTypes].[Role] IS NULL) OR ([Source].[Role] IS NULL AND [PanelMembershipCredentialTypes].[Role] IS NOT NULL) OR (([Source].[Role] IS NOT NULL AND [PanelMembershipCredentialTypes].[Role] IS NOT NULL) AND [Source].[Role] != [PanelMembershipCredentialTypes].[Role]))
		OR (([Source].[PersonId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [PanelMembershipCredentialTypes].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [PanelMembershipCredentialTypes].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [PanelMembershipCredentialTypes].[PersonId]))
		OR (([Source].[NAATINumber] IS NOT NULL AND [PanelMembershipCredentialTypes].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [PanelMembershipCredentialTypes].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [PanelMembershipCredentialTypes].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [PanelMembershipCredentialTypes].[NAATINumber]))

	--select * from @PanelMembershipCredentialTypesHistory
		
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE PanelMembershipCredentialTypesHistory AS Target USING(select * from @PanelMembershipCredentialTypesHistory where [RowStatus] = 'Deleted' ) AS Source ON (Target.[PanelMemberShipCredentialTypeId] = Source.[PanelMemberShipCredentialTypeId] AND Target.RowStatus = 'Latest')
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE PanelMembershipCredentialTypesHistory AS Target USING(	select * from @PanelMembershipCredentialTypesHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.[PanelMemberShipCredentialTypeId] = Source.[PanelMemberShipCredentialTypeId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE PanelMembershipCredentialTypesHistory AS Target USING(	select * from @PanelMembershipCredentialTypesHistory where [RowStatus] = 'NewOrModified' ) AS Source ON (Target.[PanelMemberShipCredentialTypeId] = Source.[PanelMemberShipCredentialTypeId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.PanelMemberShipCredentialTypeId = Source.PanelMemberShipCredentialTypeId
	      ,Target.PanelMembershipId = Source.PanelMembershipId
		  ,Target.PanelId = Source.PanelId
		  ,Target.PanelName = Source.PanelName
		  ,Target.Role = Source.Role
		  ,Target.PersonId = Source.PersonId
		  ,Target.PersonName = Source.PersonName      
		  ,Target.NaatiNumber = Source.NaatiNumber
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName
		  
		  ,Target.[ModifiedDate] = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		INSERT([PanelMemberShipCredentialTypeId],[PanelMembershipId],[PanelId],[PanelName],[Role],[PersonId],[PersonName],[NaatiNumber],[CredentialTypeInternalName],[CredentialTypeExternalName], [ModifiedDate],[RowStatus])	  
		VALUES ([Source].[PanelMemberShipCredentialTypeId], Source.[PanelMembershipId],Source.[PanelId],Source.[PanelName],Source.[Role],Source.[PersonId],Source.[PersonName],Source.[NaatiNumber],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName], @Date, 'Latest');	  
	
	COMMIT TRANSACTION;	
	
END