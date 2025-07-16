ALTER PROCEDURE [dbo].[ReportingSnapshot_WorkPractice]
		@Date DateTime
AS
BEGIN
DECLARE @WorkPracticeHistory as table([WorkPracticeId] [int] NOT NULL,[PersonID] [int] NOT NULL,[CustomerNumber] [int] NOT NULL,[PractitionerNumber] [nvarchar](50) NOT NULL,[ApplicationID] [int] NOT NULL,[ApplicationStatus] [nvarchar](100) NULL,[CredentialTypeInternalName] [nvarchar](50) NULL,
									  [CredentialTypeExternalName] [nvarchar](50) NULL,[Skill] [nvarchar](200) NULL,[CertificationPeriodID] [int] NULL,[CertificationPeriodStartDate] [datetime] NULL,[CertificationPeriodOriginalEndDate] [datetime] NULL,[CertificationPeriodEndDate] [datetime] NULL,
								      [DateCompleted] [datetime] NULL,[Description] [nvarchar](200) NULL,[Points] [decimal](9, 1) NULL,[WorkPracticeUnits] [nvarchar](20) NULL,[NumberOfAttachments] [int] NULL,
									  [ModifiedDate] [datetime] NOT NULL, [RowStatus] nvarchar(50))   

 	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @WorkPracticeHistory
	SELECT
	     CASE WHEN [Source].[WorkPracticeId] IS NULL THEN [WorkPractice].[WorkPracticeId] ELSE [Source].[WorkPracticeId] END AS [WorkPracticeId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [WorkPractice].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNumber] IS NULL THEN [WorkPractice].[CustomerNumber] ELSE [Source].[CustomerNumber] END AS [CustomerNumber]
		,CASE WHEN [Source].[PractitionerNumber] IS NULL THEN [WorkPractice].[PractitionerNumber] ELSE [Source].[PractitionerNumber] END AS [PractitionerNumber]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [WorkPractice].[ApplicationId] ELSE [Source].[ApplicationId] END AS [ApplicationId]
		,CASE WHEN [Source].[ApplicationStatus] IS NULL THEN [WorkPractice].[ApplicationStatus] ELSE [Source].[ApplicationStatus] END AS [ApplicationStatus]

		,CASE WHEN [Source].[CredentialTypeInternalName] IS NULL THEN [WorkPractice].[CredentialTypeInternalName] ELSE [Source].[CredentialTypeInternalName] END AS [CredentialTypeInternalName]
		,CASE WHEN [Source].[CredentialTypeExternalName] IS NULL THEN [WorkPractice].[CredentialTypeExternalName] ELSE [Source].[CredentialTypeExternalName] END AS [CredentialTypeExternalName]
		,CASE WHEN [Source].[Skill] IS NULL THEN [WorkPractice].[Skill] ELSE [Source].[Skill] END AS [Skill]

		,CASE WHEN [Source].[CertificationPeriodId] IS NULL THEN [WorkPractice].[CertificationPeriodId] ELSE [Source].[CertificationPeriodId] END AS [CertificationPeriodId]
		,CASE WHEN [Source].[CertificationPeriodStartDate] IS NULL THEN [WorkPractice].[CertificationPeriodStartDate] ELSE [Source].[CertificationPeriodStartDate] END AS [CertificationPeriodStartDate]
		,CASE WHEN [Source].[CertificationPeriodOriginalEndDate] IS NULL THEN [WorkPractice].[CertificationPeriodOriginalEndDate] ELSE [Source].[CertificationPeriodOriginalEndDate] END AS [CertificationPeriodOriginalEndDate]
		,CASE WHEN [Source].[CertificationPeriodEndDate] IS NULL THEN [WorkPractice].[CertificationPeriodEndDate] ELSE [Source].[CertificationPeriodEndDate] END AS [CertificationPeriodEndDate]
		,CASE WHEN [Source].[DateCompleted] IS NULL THEN [WorkPractice].[DateCompleted] ELSE [Source].[DateCompleted] END AS [DateCompleted]

		,CASE WHEN [Source].[Description] IS NULL THEN [WorkPractice].[Description] ELSE [Source].[Description] END AS [Description]
		,CASE WHEN [Source].[Points] IS NULL THEN [WorkPractice].[Points] ELSE [Source].[Points] END AS [Points]		
		
		,CASE WHEN [Source].[WorkPracticeUnits] IS NULL THEN [WorkPractice].[WorkPracticeUnits] ELSE [Source].[WorkPracticeUnits] END AS [WorkPracticeUnits]				
		,CASE WHEN [Source].[NumberOfAttachments] IS NULL THEN [WorkPractice].[NumberOfAttachments] ELSE [Source].[NumberOfAttachments] END AS [NumberOfAttachments]				

		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[WorkPracticeId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
		
	FROM
	(
		SELECT
			 wp.WorkPracticeId as 'WorkPracticeId'
			,cap.[PersonId] as 'PersonId'
			,e.[NAATINumber] as 'CustomerNumber'
			,p.[PractitionerNumber] as 'PractitionerNumber'
			,cap.[CredentialApplicationId] as 'ApplicationId'			
			,caps.[DisplayName] as 'ApplicationStatus'

			,ct.InternalName as 'CredentialTypeInternalName'
			,ct.ExternalName as 'CredentialTypeExternalName'			
			,REPLACE(REPLACE(d.[DisplayName], '[Language 1]', l1.[Name]), '[Language 2]', l2.[Name]) COLLATE Latin1_General_CI_AS as 'Skill'
			
			,cp.[CertificationPeriodId] as 'CertificationPeriodId'
			,cp.[StartDate] as 'CertificationPeriodStartDate'
			,cp.[EndDate] as 'CertificationPeriodEndDate'
			,cp.[OriginalEndDate] as 'CertificationPeriodOriginalEndDate'			

			,wp.[Description] as 'Description'
			,wp.[Points] as 'Points' 
			,wp.[Date] as 'DateCompleted'			
			
			,cc.[WorkPracticeUnits] as 'WorkPracticeUnits' 
			,(SELECT count(*)  FROM [naati_db]..[tblWorkPracticeAttachment] where [WorkPracticeId ] = wp.WorkPracticeId) as 'NumberOfAttachments'

			
		FROM [naati_db]..[tblWorkPractice] wp
		INNER JOIN [naati_db]..[tblWorkPracticeCredentialRequest] wpcr ON wp.WorkPracticeId = wpcr.WorkPracticeId
		INNER JOIN [naati_db]..[tblCredentialRequest] cr ON wpcr.CredentialRequestId = cr.CredentialRequestId

		INNER JOIN [naati_db]..[tblRecertification] rec on cr.CredentialApplicationId = rec.CredentialApplicationId
		INNER JOIN [naati_db]..[tblCertificationPeriod] cp ON rec.CertificationPeriodId = cp.CertificationPeriodId
		
		INNER JOIN [naati_db]..[tblCredentialType] ct ON cr.CredentialTypeId = ct.CredentialTypeId

		INNER JOIN [naati_db]..[tblCredentialApplication] cap ON cr.CredentialApplicationId = cap.CredentialApplicationId
		INNER JOIN [naati_db]..[tblPerson] p ON cap.PersonId = p.PersonId
		INNER JOIN [naati_db]..[tblEntity] e ON p.EntityId = e.EntityId		
		INNER JOIN [naati_db]..[tblCredentialApplicationStatusType] caps ON cap.CredentialApplicationStatusTypeId = caps.CredentialApplicationStatusTypeId

		INNER JOIN [naati_db]..[tblSkill] s ON cr.[SkillId] = s.[SkillId]
		INNER JOIN [naati_db]..[tblLanguage] l1 ON s.[Language1Id] = l1.[LanguageId]		
		INNER JOIN [naati_db]..[tblLanguage] l2 ON s.[Language2Id] = l2.[LanguageId]		
		INNER JOIN [naati_db]..[tblDirectionType] d ON d.[DirectionTypeId] = s.[DirectionTypeId]		
		INNER JOIN [naati_db]..[tblCredentialCategory] cc ON ct.CredentialCategoryId = cc.CredentialCategoryId	
		
	) [Source]

	FULL OUTER JOIN [WorkPractice] ON 
	([Source].[WorkPracticeId] = [WorkPractice].[WorkPracticeId] and [Source].[PersonId] = [WorkPractice].[PersonId] and [Source].[CustomerNumber] = [WorkPractice].[CustomerNumber] and 
	 [Source].[PractitionerNumber] = [WorkPractice].[PractitionerNumber] and [Source].[ApplicationId] = [WorkPractice].[ApplicationId] and [Source].[CertificationPeriodId] = [WorkPractice].[CertificationPeriodId])
	 
	WHERE 
	   (([Source].[PersonId] IS NOT NULL AND [WorkPractice].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [WorkPractice].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [WorkPractice].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [WorkPractice].[PersonId]))
	OR (([Source].[CustomerNumber] IS NOT NULL AND [WorkPractice].[CustomerNumber] IS NULL) OR ([Source].[CustomerNumber] IS NULL AND [WorkPractice].[CustomerNumber] IS NOT NULL) OR (([Source].[CustomerNumber] IS NOT NULL AND [WorkPractice].[CustomerNumber] IS NOT NULL) AND [Source].[CustomerNumber] != [WorkPractice].[CustomerNumber]))	
	OR (([Source].[PractitionerNumber] IS NOT NULL AND [WorkPractice].[PractitionerNumber] IS NULL) OR ([Source].[PractitionerNumber] IS NULL AND [WorkPractice].[PractitionerNumber] IS NOT NULL) OR (([Source].[PractitionerNumber] IS NOT NULL AND [WorkPractice].[PractitionerNumber] IS NOT NULL) AND [Source].[PractitionerNumber] != [WorkPractice].[PractitionerNumber]))	
	OR (([Source].[ApplicationId] IS NOT NULL AND [WorkPractice].[ApplicationId] IS NULL) OR ([Source].[ApplicationId] IS NULL AND [WorkPractice].[ApplicationId] IS NOT NULL) OR (([Source].[ApplicationId] IS NOT NULL AND [WorkPractice].[ApplicationId] IS NOT NULL) AND [Source].[ApplicationId] != [WorkPractice].[ApplicationId]))	
	OR (([Source].[ApplicationStatus] IS NOT NULL AND [WorkPractice].[ApplicationStatus] IS NULL) OR ([Source].[ApplicationStatus] IS NULL AND [WorkPractice].[ApplicationStatus] IS NOT NULL) OR (([Source].[ApplicationStatus] IS NOT NULL AND [WorkPractice].[ApplicationStatus] IS NOT NULL) AND [Source].[ApplicationStatus] != [WorkPractice].[ApplicationStatus]))	

	OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [WorkPractice].[CredentialTypeInternalName] IS NULL) OR ([Source].[CredentialTypeInternalName] IS NULL AND [WorkPractice].[CredentialTypeInternalName] IS NOT NULL) OR (([Source].[CredentialTypeInternalName] IS NOT NULL AND [WorkPractice].[CredentialTypeInternalName] IS NOT NULL) AND [Source].[CredentialTypeInternalName] != [WorkPractice].[CredentialTypeInternalName]))	
	OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [WorkPractice].[CredentialTypeExternalName] IS NULL) OR ([Source].[CredentialTypeExternalName] IS NULL AND [WorkPractice].[CredentialTypeExternalName] IS NOT NULL) OR (([Source].[CredentialTypeExternalName] IS NOT NULL AND [WorkPractice].[CredentialTypeExternalName] IS NOT NULL) AND [Source].[CredentialTypeExternalName] != [WorkPractice].[CredentialTypeExternalName]))	
	OR (([Source].[Skill] IS NOT NULL AND [WorkPractice].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [WorkPractice].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [WorkPractice].[Skill] IS NOT NULL) AND [Source].[Skill] != [WorkPractice].[Skill]))	
		
	OR (([Source].[CertificationPeriodId] IS NOT NULL AND [WorkPractice].[CertificationPeriodId] IS NULL) OR ([Source].[CertificationPeriodId] IS NULL AND [WorkPractice].[CertificationPeriodId] IS NOT NULL) OR (([Source].[CertificationPeriodId] IS NOT NULL AND [WorkPractice].[CertificationPeriodId] IS NOT NULL) AND [Source].[CertificationPeriodId] != [WorkPractice].[CertificationPeriodId]))
	OR (([Source].[CertificationPeriodStartDate] IS NOT NULL AND [WorkPractice].[CertificationPeriodStartDate] IS NULL) OR ([Source].[CertificationPeriodStartDate] IS NULL AND [WorkPractice].[CertificationPeriodStartDate] IS NOT NULL) OR (([Source].[CertificationPeriodStartDate] IS NOT NULL AND [WorkPractice].[CertificationPeriodStartDate] IS NOT NULL) AND [Source].[CertificationPeriodStartDate] != [WorkPractice].[CertificationPeriodStartDate]))
	OR (([Source].[CertificationPeriodEndDate] IS NOT NULL AND [WorkPractice].[CertificationPeriodEndDate] IS NULL) OR ([Source].[CertificationPeriodEndDate] IS NULL AND [WorkPractice].[CertificationPeriodEndDate] IS NOT NULL) OR (([Source].[CertificationPeriodEndDate] IS NOT NULL AND [WorkPractice].[CertificationPeriodEndDate] IS NOT NULL) AND [Source].[CertificationPeriodEndDate] != [WorkPractice].[CertificationPeriodEndDate]))
	OR (([Source].[CertificationPeriodOriginalEndDate] IS NOT NULL AND [WorkPractice].[CertificationPeriodOriginalEndDate] IS NULL) OR ([Source].[CertificationPeriodOriginalEndDate] IS NULL AND [WorkPractice].[CertificationPeriodOriginalEndDate] IS NOT NULL) OR (([Source].[CertificationPeriodOriginalEndDate] IS NOT NULL AND [WorkPractice].[CertificationPeriodOriginalEndDate] IS NOT NULL) AND [Source].[CertificationPeriodOriginalEndDate] != [WorkPractice].[CertificationPeriodOriginalEndDate]))
	OR (([Source].[Description] IS NOT NULL AND [WorkPractice].[Description] IS NULL) OR ([Source].[Description] IS NULL AND [WorkPractice].[Description] IS NOT NULL) OR (([Source].[Description] IS NOT NULL AND [WorkPractice].[Description] IS NOT NULL) AND [Source].[Description] != [WorkPractice].[Description]))
	OR (([Source].[Points] IS NOT NULL AND [WorkPractice].[Points] IS NULL) OR ([Source].[Points] IS NULL AND [WorkPractice].[Points] IS NOT NULL) OR (([Source].[Points] IS NOT NULL AND [WorkPractice].[Points] IS NOT NULL) AND [Source].[Points] != [WorkPractice].[Points]))
	OR (([Source].[DateCompleted] IS NOT NULL AND [WorkPractice].[DateCompleted] IS NULL) OR ([Source].[DateCompleted] IS NULL AND [WorkPractice].[DateCompleted] IS NOT NULL) OR (([Source].[DateCompleted] IS NOT NULL AND [WorkPractice].[DateCompleted] IS NOT NULL) AND [Source].[DateCompleted] != [WorkPractice].[DateCompleted]))

	OR (([Source].[WorkPracticeUnits] IS NOT NULL AND [WorkPractice].[WorkPracticeUnits] IS NULL) OR ([Source].[WorkPracticeUnits] IS NULL AND [WorkPractice].[WorkPracticeUnits] IS NOT NULL) OR (([Source].[WorkPracticeUnits] IS NOT NULL AND [WorkPractice].[WorkPracticeUnits] IS NOT NULL) AND [Source].[WorkPracticeUnits] != [WorkPractice].[WorkPracticeUnits]))	
	OR (([Source].[NumberOfAttachments] IS NOT NULL AND [WorkPractice].[NumberOfAttachments] IS NULL) OR ([Source].[NumberOfAttachments] IS NULL AND [WorkPractice].[NumberOfAttachments] IS NOT NULL) OR (([Source].[NumberOfAttachments] IS NOT NULL AND [WorkPractice].[NumberOfAttachments] IS NOT NULL) AND [Source].[NumberOfAttachments] != [WorkPractice].[NumberOfAttachments]))

	--select * from @WorkPracticeHistory
		
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE WorkPracticeHistory AS Target USING(select * from @WorkPracticeHistory where [RowStatus] = 'Deleted' ) AS Source ON 
			(Target.WorkPracticeId = Source.WorkPracticeId And Target.PersonId = Source.PersonId And Target.CustomerNumber = Source.CustomerNumber And Target.PractitionerNumber = Source.PractitionerNumber And Target.ApplicationId = Source.ApplicationId And 
			 Target.CertificationPeriodId = Source.CertificationPeriodId AND Target.RowStatus = 'Latest')
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE WorkPracticeHistory AS Target USING(	select * from @WorkPracticeHistory where [RowStatus] = 'NewOrModified' ) AS Source ON 
			(Target.WorkPracticeId = Source.WorkPracticeId And Target.PersonId = Source.PersonId And Target.CustomerNumber = Source.CustomerNumber And Target.PractitionerNumber = Source.PractitionerNumber And Target.ApplicationId = Source.ApplicationId And 
			 Target.CertificationPeriodId = Source.CertificationPeriodId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE WorkPracticeHistory AS Target USING(	select * from @WorkPracticeHistory where [RowStatus] = 'NewOrModified' ) AS Source ON 		
			(Target.WorkPracticeId = Source.WorkPracticeId And Target.PersonId = Source.PersonId And Target.CustomerNumber = Source.CustomerNumber And Target.PractitionerNumber = Source.PractitionerNumber And Target.ApplicationId = Source.ApplicationId And 
			 Target.CertificationPeriodId = Source.CertificationPeriodId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.WorkPracticeId = Source.WorkPracticeId
		  ,Target.PersonID = Source.PersonID
		  ,Target.CustomerNumber = Source.CustomerNumber
		  ,Target.PractitionerNumber = Source.PractitionerNumber
		  ,Target.ApplicationID = Source.ApplicationID
		  ,Target.ApplicationStatus = Source.ApplicationStatus      
		  ,Target.CredentialTypeInternalName = Source.CredentialTypeInternalName
		  ,Target.CredentialTypeExternalName = Source.CredentialTypeExternalName
		  ,Target.Skill = Source.Skill
		  ,Target.CertificationPeriodID = Source.CertificationPeriodID

		  ,Target.CertificationPeriodStartDate = Source.CertificationPeriodStartDate
		  ,Target.CertificationPeriodOriginalEndDate = Source.CertificationPeriodOriginalEndDate
		  ,Target.CertificationPeriodEndDate = Source.CertificationPeriodEndDate
		  ,Target.DateCompleted = Source.DateCompleted
		  ,Target.Description = Source.Description
		  ,Target.Points = Source.Points
		  ,Target.WorkPracticeUnits = Source.WorkPracticeUnits
		  ,Target.NumberOfAttachments = Source.NumberOfAttachments		  
		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT([WorkPracticeId],[PersonID],[CustomerNumber],[PractitionerNumber],[ApplicationID],[ApplicationStatus],[CredentialTypeInternalName],[CredentialTypeExternalName],[Skill],[CertificationPeriodID],
		     [CertificationPeriodStartDate],[CertificationPeriodOriginalEndDate],[CertificationPeriodEndDate],[DateCompleted],[Description],[Points],[WorkPracticeUnits],[NumberOfAttachments],
			 [ModifiedDate],[RowStatus])
			 	  
		 VALUES (Source.[WorkPracticeId],Source.[PersonID],Source.[CustomerNumber],Source.[PractitionerNumber],Source.[ApplicationID],Source.[ApplicationStatus],Source.[CredentialTypeInternalName],Source.[CredentialTypeExternalName],Source.[Skill],
			 Source.[CertificationPeriodID],Source.[CertificationPeriodStartDate],Source.[CertificationPeriodOriginalEndDate],Source.[CertificationPeriodEndDate],Source.[DateCompleted],Source.[Description],Source.[Points],Source.[WorkPracticeUnits],
			 Source.[NumberOfAttachments],@Date, 'Latest');

	COMMIT TRANSACTION;	

END


