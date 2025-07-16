ALTER PROCEDURE [dbo].[ReportingSnapshot_ProfessionalDevelopment]
	@Date DateTime
AS
BEGIN
DECLARE @ProfessionalDevelopmentHistory as table([ProfessionalDevelopmentActivityId] [int] NOT NULL,[PersonId] [int] NOT NULL,[CustomerNumber] [int] NOT NULL,[PractitionerNumber] [nvarchar](50) NOT NULL,	[ApplicationID] [int] NOT NULL,	[ApplicationStatus] [nvarchar](100) NULL,
										         [CertificationPeriodID] [int] NOT NULL,[CertificationPeriodStartDate] [datetime] NULL,[CertificationPeriodOriginalEndDate] [datetime] NULL,[CertificationPeriodEndDate] [datetime] NULL,[DateCompleted] [datetime] NULL,[Description] [nvarchar](500) NULL,
												 [SectionID] [int] NOT NULL,[SectionName] [nvarchar](50) NULL,[CategoryID] [int] NOT NULL,[CategoryName] [nvarchar](50) NULL,[CategoryGroup] [nvarchar](50) NULL,[RequirementID] [int] NOT NULL,[RequirementName] [nvarchar](255) NULL,	[Points] [int] NULL,
												 [NumberOfAttachments] [int] NULL,[ModifiedDate] [datetime] NOT NULL, [RowStatus] nvarchar(50))   

	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @ProfessionalDevelopmentHistory
	
	SELECT 
	
		 CASE WHEN [Source].[ProfessionalDevelopmentActivityId] IS NULL THEN [ProfessionalDevelopment].[ProfessionalDevelopmentActivityId] ELSE [Source].[ProfessionalDevelopmentActivityId] END AS [ProfessionalDevelopmentActivityId]
		,CASE WHEN [Source].[PersonId] IS NULL THEN [ProfessionalDevelopment].[PersonId] ELSE [Source].[PersonId] END AS [PersonId]
		,CASE WHEN [Source].[CustomerNumber] IS NULL THEN [ProfessionalDevelopment].[CustomerNumber] ELSE [Source].[CustomerNumber] END AS [CustomerNumber]
		,CASE WHEN [Source].[PractitionerNumber] IS NULL THEN [ProfessionalDevelopment].[PractitionerNumber] ELSE [Source].[PractitionerNumber] END AS [PractitionerNumber]
		,CASE WHEN [Source].[ApplicationId] IS NULL THEN [ProfessionalDevelopment].[ApplicationId] ELSE [Source].[ApplicationId] END AS [ApplicationId]
		,CASE WHEN [Source].[ApplicationStatus] IS NULL THEN [ProfessionalDevelopment].[ApplicationStatus] ELSE [Source].[ApplicationStatus] END AS [ApplicationStatus]
		,CASE WHEN [Source].[CertificationPeriodId] IS NULL THEN [ProfessionalDevelopment].[CertificationPeriodId] ELSE [Source].[CertificationPeriodId] END AS [CertificationPeriodId]
		,CASE WHEN [Source].[CertificationPeriodStartDate] IS NULL THEN [ProfessionalDevelopment].[CertificationPeriodStartDate] ELSE [Source].[CertificationPeriodStartDate] END AS [CertificationPeriodStartDate]
		,CASE WHEN [Source].[CertificationPeriodOriginalEndDate] IS NULL THEN [ProfessionalDevelopment].[CertificationPeriodOriginalEndDate] ELSE [Source].[CertificationPeriodOriginalEndDate] END AS [CertificationPeriodOriginalEndDate]
		,CASE WHEN [Source].[CertificationPeriodEndDate] IS NULL THEN [ProfessionalDevelopment].[CertificationPeriodEndDate] ELSE [Source].[CertificationPeriodEndDate] END AS [CertificationPeriodEndDate]
		,CASE WHEN [Source].[DateCompleted] IS NULL THEN [ProfessionalDevelopment].[DateCompleted] ELSE [Source].[DateCompleted] END AS [DateCompleted]		
		,CASE WHEN [Source].[Description] IS NULL THEN [ProfessionalDevelopment].[Description] ELSE [Source].[Description] END AS [Description]
		,CASE WHEN [Source].[SectionId] IS NULL THEN [ProfessionalDevelopment].[SectionId] ELSE [Source].[SectionId] END AS [SectionId]		
		,CASE WHEN [Source].[SectionName] IS NULL THEN [ProfessionalDevelopment].[SectionName] ELSE [Source].[SectionName] END AS [SectionName]
		,CASE WHEN [Source].[CategoryId] IS NULL THEN [ProfessionalDevelopment].[CategoryId] ELSE [Source].[CategoryId] END AS [CategoryId]		
		,CASE WHEN [Source].[CategoryName] IS NULL THEN [ProfessionalDevelopment].[CategoryName] ELSE [Source].[CategoryName] END AS [CategoryName]
		,CASE WHEN [Source].[CategoryGroup] IS NULL THEN [ProfessionalDevelopment].[CategoryGroup] ELSE [Source].[CategoryGroup] END AS [CategoryGroup]
		,CASE WHEN [Source].[RequirementId] IS NULL THEN [ProfessionalDevelopment].[RequirementId] ELSE [Source].[RequirementId] END AS [RequirementId]
		,CASE WHEN [Source].[RequirementName] IS NULL THEN [ProfessionalDevelopment].[RequirementName] ELSE [Source].[RequirementName] END AS [RequirementName]
		,CASE WHEN [Source].[Points] IS NULL THEN [ProfessionalDevelopment].[Points] ELSE [Source].[Points] END AS [Points]
		,CASE WHEN [Source].[NumberOfAttachments] IS NULL THEN [ProfessionalDevelopment].[NumberOfAttachments] ELSE [Source].[NumberOfAttachments] END AS [NumberOfAttachments]

		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[ProfessionalDevelopmentActivityId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	

	FROM
	(
		SELECT  
			pda.ProfessionalDevelopmentActivityId as 'ProfessionalDevelopmentActivityId'
			,p.[PersonId] as 'PersonId'
			,e.[NAATINumber] as 'CustomerNumber'
			,p.[PractitionerNumber] as 'PractitionerNumber'
			,cap.[CredentialApplicationId] as 'ApplicationId'			
			,caps.[DisplayName] as 'ApplicationStatus'

			,rec.[CertificationPeriodId] as 'CertificationPeriodId'
			,cp.[StartDate] as 'CertificationPeriodStartDate'
			,cp.[EndDate] as 'CertificationPeriodEndDate'
			,cp.[OriginalEndDate] as 'CertificationPeriodOriginalEndDate'			

			,pda.[Description] as 'Description'
			,pda.[DateCompleted] as 'DateCompleted'			

			,pds.[ProfessionalDevelopmentSectionId] as 'SectionId'			
			,pds.[Name] as 'SectionName'			

			,pdc.[ProfessionalDevelopmentCategoryId] as 'CategoryId'			
			,pdc.[Name] as 'CategoryName'			
			,pdcg.[Name] as 'CategoryGroup'			

			,pdr.[ProfessionalDevelopmentRequirementId] as 'RequirementId'			
			,pdr.[Name] as 'RequirementName'			
			,pdcr.[Points] as 'Points' 
			
			,(SELECT count(*)  FROM [naati_db]..[tblProfessionalDevelopmentActivityAttachment] where [ProfessionalDevelopmentActivityId] = pda.ProfessionalDevelopmentActivityId) as 'NumberOfAttachments'

		FROM [naati_db]..[tblProfessionalDevelopmentActivity] pda	
		INNER JOIN [naati_db]..[tblProfessionalDevelopmentCredentialApplication] pdcap on pda.ProfessionalDevelopmentActivityId = pdcap.ProfessionalDevelopmentActivityId
		INNER JOIN [naati_db]..[tblPerson] p ON pda.PersonId = p.PersonId
		INNER JOIN [naati_db]..[tblEntity] e ON p.EntityId = e.EntityId
				
		INNER JOIN [naati_db]..[tblCredentialApplication] cap ON pdcap.CredentialApplicationId = cap.CredentialApplicationId
		INNER JOIN [naati_db]..[tblCredentialApplicationStatusType] caps ON cap.CredentialApplicationStatusTypeId = caps.CredentialApplicationStatusTypeId
		INNER JOIN [naati_db]..[tblRecertification] rec on pdcap.CredentialApplicationId = rec.CredentialApplicationId

		INNER JOIN [naati_db]..[tblCertificationPeriod] cp ON rec.CertificationPeriodId = cp.CertificationPeriodId

		INNER JOIN [naati_db]..[tblProfessionalDevelopmentSectionCategory] pdsc ON pda.ProfessionalDevelopmentCategoryId = pdsc.ProfessionalDevelopmentCategoryId
		INNER JOIN [naati_db]..[tblProfessionalDevelopmentSection] pds ON pdsc.ProfessionalDevelopmentSectionId = pds.ProfessionalDevelopmentSectionId
		INNER JOIN [naati_db]..[tblProfessionalDevelopmentCategory] pdc ON  pda.ProfessionalDevelopmentCategoryId = pdc.ProfessionalDevelopmentCategoryId

	    INNER JOIN [naati_db]..[tblProfessionalDevelopmentCategoryRequirement] pdcr ON pda.ProfessionalDevelopmentCategoryId = pdcr.ProfessionalDevelopmentCategoryId AND pda.ProfessionalDevelopmentRequirementId = pdcr.ProfessionalDevelopmentRequirementId
		INNER JOIN [naati_db]..[tblProfessionalDevelopmentRequirement] pdr ON pda.ProfessionalDevelopmentRequirementId = pdr.ProfessionalDevelopmentRequirementId
		LEFT JOIN [naati_db]..[tblProfessionalDevelopmentCategoryGroup] pdcg ON pdc.ProfessionalDevelopmentCategoryGroupId = pdcg.ProfessionalDevelopmentCategoryGroupId
		
		) [Source]

		FULL OUTER JOIN [ProfessionalDevelopment] ON [Source].[ProfessionalDevelopmentActivityId] = [ProfessionalDevelopment].[ProfessionalDevelopmentActivityId]
		and [Source].[PersonId] = [ProfessionalDevelopment].[PersonId]
		and [Source].[CustomerNumber] = [ProfessionalDevelopment].[CustomerNumber]
		and [Source].[ApplicationId] = [ProfessionalDevelopment].[ApplicationId]

		and [Source].[CertificationPeriodId] = [ProfessionalDevelopment].[CertificationPeriodId]
		and [Source].[SectionId] = [ProfessionalDevelopment].[SectionId]
		and [Source].[CategoryId] = [ProfessionalDevelopment].[CategoryId]

		and [Source].[RequirementId] = [ProfessionalDevelopment].[RequirementId]		

		WHERE 
	   (([Source].[PersonId] IS NOT NULL AND [ProfessionalDevelopment].[PersonId] IS NULL) OR ([Source].[PersonId] IS NULL AND [ProfessionalDevelopment].[PersonId] IS NOT NULL) OR (([Source].[PersonId] IS NOT NULL AND [ProfessionalDevelopment].[PersonId] IS NOT NULL) AND [Source].[PersonId] != [ProfessionalDevelopment].[PersonId]))
    OR (([Source].[CustomerNumber] IS NOT NULL AND [ProfessionalDevelopment].[CustomerNumber] IS NULL) OR ([Source].[CustomerNumber] IS NULL AND [ProfessionalDevelopment].[CustomerNumber] IS NOT NULL) OR (([Source].[CustomerNumber] IS NOT NULL AND [ProfessionalDevelopment].[CustomerNumber] IS NOT NULL) AND [Source].[CustomerNumber] != [ProfessionalDevelopment].[CustomerNumber]))	
	OR (([Source].[PractitionerNumber] IS NOT NULL AND [ProfessionalDevelopment].[PractitionerNumber] IS NULL) OR ([Source].[PractitionerNumber] IS NULL AND [ProfessionalDevelopment].[PractitionerNumber] IS NOT NULL) OR (([Source].[PractitionerNumber] IS NOT NULL AND [ProfessionalDevelopment].[PractitionerNumber] IS NOT NULL) AND [Source].[PractitionerNumber] != [ProfessionalDevelopment].[PractitionerNumber]))	
	OR (([Source].[ApplicationId] IS NOT NULL AND [ProfessionalDevelopment].[ApplicationId] IS NULL) OR ([Source].[ApplicationId] IS NULL AND [ProfessionalDevelopment].[ApplicationId] IS NOT NULL) OR (([Source].[ApplicationId] IS NOT NULL AND [ProfessionalDevelopment].[ApplicationId] IS NOT NULL) AND [Source].[ApplicationId] != [ProfessionalDevelopment].[ApplicationId]))	
	OR (([Source].[ApplicationStatus] IS NOT NULL AND [ProfessionalDevelopment].[ApplicationStatus] IS NULL) OR ([Source].[ApplicationStatus] IS NULL AND [ProfessionalDevelopment].[ApplicationStatus] IS NOT NULL) OR (([Source].[ApplicationStatus] IS NOT NULL AND [ProfessionalDevelopment].[ApplicationStatus] IS NOT NULL) AND [Source].[ApplicationStatus] != [ProfessionalDevelopment].[ApplicationStatus]))	
	OR (([Source].[CertificationPeriodId] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodId] IS NULL) OR ([Source].[CertificationPeriodId] IS NULL AND [ProfessionalDevelopment].[CertificationPeriodId] IS NOT NULL) OR (([Source].[CertificationPeriodId] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodId] IS NOT NULL) AND [Source].[CertificationPeriodId] != [ProfessionalDevelopment].[CertificationPeriodId]))
	OR (([Source].[CertificationPeriodStartDate] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodStartDate] IS NULL) OR ([Source].[CertificationPeriodStartDate] IS NULL AND [ProfessionalDevelopment].[CertificationPeriodStartDate] IS NOT NULL) OR (([Source].[CertificationPeriodStartDate] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodStartDate] IS NOT NULL) AND [Source].[CertificationPeriodStartDate] != [ProfessionalDevelopment].[CertificationPeriodStartDate]))
	OR (([Source].[CertificationPeriodEndDate] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodEndDate] IS NULL) OR ([Source].[CertificationPeriodEndDate] IS NULL AND [ProfessionalDevelopment].[CertificationPeriodEndDate] IS NOT NULL) OR (([Source].[CertificationPeriodEndDate] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodEndDate] IS NOT NULL) AND [Source].[CertificationPeriodEndDate] != [ProfessionalDevelopment].[CertificationPeriodEndDate]))
	OR (([Source].[CertificationPeriodOriginalEndDate] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodOriginalEndDate] IS NULL) OR ([Source].[CertificationPeriodOriginalEndDate] IS NULL AND [ProfessionalDevelopment].[CertificationPeriodOriginalEndDate] IS NOT NULL) OR (([Source].[CertificationPeriodOriginalEndDate] IS NOT NULL AND [ProfessionalDevelopment].[CertificationPeriodOriginalEndDate] IS NOT NULL) AND [Source].[CertificationPeriodOriginalEndDate] != [ProfessionalDevelopment].[CertificationPeriodOriginalEndDate]))
	OR (([Source].[Description] IS NOT NULL AND [ProfessionalDevelopment].[Description] IS NULL) OR ([Source].[Description] IS NULL AND [ProfessionalDevelopment].[Description] IS NOT NULL) OR (([Source].[Description] IS NOT NULL AND [ProfessionalDevelopment].[Description] IS NOT NULL) AND [Source].[Description] != [ProfessionalDevelopment].[Description]))
	OR (([Source].[DateCompleted] IS NOT NULL AND [ProfessionalDevelopment].[DateCompleted] IS NULL) OR ([Source].[DateCompleted] IS NULL AND [ProfessionalDevelopment].[DateCompleted] IS NOT NULL) OR (([Source].[DateCompleted] IS NOT NULL AND [ProfessionalDevelopment].[DateCompleted] IS NOT NULL) AND [Source].[DateCompleted] != [ProfessionalDevelopment].[DateCompleted]))

	OR (([Source].[SectionId] IS NOT NULL AND [ProfessionalDevelopment].[SectionId] IS NULL) OR ([Source].[SectionId] IS NULL AND [ProfessionalDevelopment].[SectionId] IS NOT NULL) OR (([Source].[SectionId] IS NOT NULL AND [ProfessionalDevelopment].[SectionId] IS NOT NULL) AND [Source].[SectionId] != [ProfessionalDevelopment].[SectionId]))
	OR (([Source].[SectionName] IS NOT NULL AND [ProfessionalDevelopment].[SectionName] IS NULL) OR ([Source].[SectionName] IS NULL AND [ProfessionalDevelopment].[SectionName] IS NOT NULL) OR (([Source].[SectionName] IS NOT NULL AND [ProfessionalDevelopment].[SectionName] IS NOT NULL) AND [Source].[SectionName] != [ProfessionalDevelopment].[SectionName]))
	OR (([Source].[CategoryId] IS NOT NULL AND [ProfessionalDevelopment].[CategoryId] IS NULL) OR ([Source].[CategoryId] IS NULL AND [ProfessionalDevelopment].[CategoryId] IS NOT NULL) OR (([Source].[CategoryId] IS NOT NULL AND [ProfessionalDevelopment].[CategoryId] IS NOT NULL) AND [Source].[CategoryId] != [ProfessionalDevelopment].[CategoryId]))
	OR (([Source].[CategoryName] IS NOT NULL AND [ProfessionalDevelopment].[CategoryName] IS NULL) OR ([Source].[CategoryName] IS NULL AND [ProfessionalDevelopment].[CategoryName] IS NOT NULL) OR (([Source].[CategoryName] IS NOT NULL AND [ProfessionalDevelopment].[CategoryName] IS NOT NULL) AND [Source].[CategoryName] != [ProfessionalDevelopment].[CategoryName]))
	OR (([Source].[CategoryGroup] IS NOT NULL AND [ProfessionalDevelopment].[CategoryGroup] IS NULL) OR ([Source].[CategoryGroup] IS NULL AND [ProfessionalDevelopment].[CategoryGroup] IS NOT NULL) OR (([Source].[CategoryGroup] IS NOT NULL AND [ProfessionalDevelopment].[CategoryGroup] IS NOT NULL) AND [Source].[CategoryGroup] != [ProfessionalDevelopment].[CategoryGroup]))
	OR (([Source].[RequirementId] IS NOT NULL AND [ProfessionalDevelopment].[RequirementId] IS NULL) OR ([Source].[RequirementId] IS NULL AND [ProfessionalDevelopment].[RequirementId] IS NOT NULL) OR (([Source].[RequirementId] IS NOT NULL AND [ProfessionalDevelopment].[RequirementId] IS NOT NULL) AND [Source].[RequirementId] != [ProfessionalDevelopment].[RequirementId]))
	OR (([Source].[RequirementName] IS NOT NULL AND [ProfessionalDevelopment].[RequirementName] IS NULL) OR ([Source].[RequirementName] IS NULL AND [ProfessionalDevelopment].[RequirementName] IS NOT NULL) OR (([Source].[RequirementName] IS NOT NULL AND [ProfessionalDevelopment].[RequirementName] IS NOT NULL) AND [Source].[RequirementName] != [ProfessionalDevelopment].[RequirementName]))
	OR (([Source].[Points] IS NOT NULL AND [ProfessionalDevelopment].[Points] IS NULL) OR ([Source].[Points] IS NULL AND [ProfessionalDevelopment].[Points] IS NOT NULL) OR (([Source].[Points] IS NOT NULL AND [ProfessionalDevelopment].[Points] IS NOT NULL) AND [Source].[Points] != [ProfessionalDevelopment].[Points]))

	OR (([Source].[NumberOfAttachments] IS NOT NULL AND [ProfessionalDevelopment].[NumberOfAttachments] IS NULL) OR ([Source].[NumberOfAttachments] IS NULL AND [ProfessionalDevelopment].[NumberOfAttachments] IS NOT NULL) OR (([Source].[NumberOfAttachments] IS NOT NULL AND [ProfessionalDevelopment].[NumberOfAttachments] IS NOT NULL) AND [Source].[NumberOfAttachments] != [ProfessionalDevelopment].[NumberOfAttachments]))

	--select * from @ProfessionalDevelopmentHistory
		
	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE ProfessionalDevelopmentHistory AS Target USING(select * from @ProfessionalDevelopmentHistory where [RowStatus] = 'Deleted' ) AS Source ON 
		(Target.ProfessionalDevelopmentActivityId = Source.ProfessionalDevelopmentActivityId AND Target.PersonId = Source.PersonId AND Target.CustomerNumber = Source.CustomerNumber AND Target.PractitionerNumber = Source.PractitionerNumber AND Target.ApplicationId = Source.ApplicationId And 
	     Target.CertificationPeriodId = Source.CertificationPeriodId AND Target.SectionId = Source.SectionId AND Target.CategoryId = Source.CategoryId AND Target.RequirementId = Source.RequirementId AND Target.RowStatus = 'Latest')

		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE ProfessionalDevelopmentHistory AS Target USING(	select * from @ProfessionalDevelopmentHistory where [RowStatus] = 'NewOrModified' ) AS Source ON 
		(Target.ProfessionalDevelopmentActivityId = Source.ProfessionalDevelopmentActivityId AND Target.PersonId = Source.PersonId AND Target.CustomerNumber = Source.CustomerNumber AND Target.PractitionerNumber = Source.PractitionerNumber AND Target.ApplicationId = Source.ApplicationId And 
	     Target.CertificationPeriodId = Source.CertificationPeriodId AND Target.SectionId = Source.SectionId AND Target.CategoryId = Source.CategoryId AND Target.RequirementId = Source.RequirementId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE ProfessionalDevelopmentHistory AS Target USING(	select * from @ProfessionalDevelopmentHistory where [RowStatus] = 'NewOrModified' ) AS Source ON 
		(Target.ProfessionalDevelopmentActivityId = Source.ProfessionalDevelopmentActivityId AND Target.PersonId = Source.PersonId AND Target.CustomerNumber = Source.CustomerNumber AND Target.PractitionerNumber = Source.PractitionerNumber AND Target.ApplicationId = Source.ApplicationId And 
	     Target.CertificationPeriodId = Source.CertificationPeriodId AND Target.SectionId = Source.SectionId AND Target.CategoryId = Source.CategoryId AND Target.RequirementId = Source.RequirementId AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date)
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.ProfessionalDevelopmentActivityId = Source.ProfessionalDevelopmentActivityId
		  ,Target.PersonId = Source.PersonId
		  ,Target.CustomerNumber = Source.CustomerNumber
		  ,Target.PractitionerNumber = Source.PractitionerNumber
		  ,Target.ApplicationID = Source.ApplicationID
		  ,Target.ApplicationStatus = Source.ApplicationStatus      
		  ,Target.CertificationPeriodID = Source.CertificationPeriodID
		  ,Target.CertificationPeriodStartDate = Source.CertificationPeriodStartDate
		  ,Target.CertificationPeriodOriginalEndDate = Source.CertificationPeriodOriginalEndDate

		  ,Target.CertificationPeriodEndDate = Source.CertificationPeriodEndDate
		  ,Target.DateCompleted = Source.DateCompleted
		  ,Target.Description = Source.Description
		  ,Target.SectionID = Source.SectionID
		  ,Target.SectionName = Source.SectionName
		  ,Target.CategoryID = Source.CategoryID
		  ,Target.CategoryName = Source.CategoryName
		  ,Target.CategoryGroup = Source.CategoryGroup
		  ,Target.RequirementID = Source.RequirementID

		  ,Target.RequirementName = Source.RequirementName
		  ,Target.Points = Source.Points
		  ,Target.NumberOfAttachments = Source.NumberOfAttachments

		  ,Target.ModifiedDate = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		INSERT([ProfessionalDevelopmentActivityId],[PersonId],[CustomerNumber],[PractitionerNumber],[ApplicationID],[ApplicationStatus],[CertificationPeriodID],[CertificationPeriodStartDate],[CertificationPeriodOriginalEndDate],
		     [CertificationPeriodEndDate],[DateCompleted],[Description],[SectionID],[SectionName],[CategoryID],[CategoryName],[CategoryGroup],[RequirementID],[RequirementName],[Points],[NumberOfAttachments],
			 [ModifiedDate],[RowStatus])	  
	  
		VALUES (Source.[ProfessionalDevelopmentActivityId],Source.[PersonId],Source.[CustomerNumber],Source.[PractitionerNumber],Source.[ApplicationID],Source.[ApplicationStatus],Source.[CertificationPeriodID],Source.[CertificationPeriodStartDate],Source.[CertificationPeriodOriginalEndDate],
		     Source.[CertificationPeriodEndDate],Source.[DateCompleted],Source.[Description],Source.[SectionID],Source.[SectionName],Source.[CategoryID],Source.[CategoryName],Source.[CategoryGroup],Source.[RequirementID],Source.[RequirementName],Source.[Points],Source.[NumberOfAttachments],
			 @Date, 'Latest');
			 
	COMMIT TRANSACTION;	
	
END
