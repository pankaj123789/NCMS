ALTER PROCEDURE [dbo].[ReportingSnapshot_OrganisationContacts]
	@Date DateTime
AS
BEGIN

DECLARE @OrganisationContactsHistory as table(ContactPersonId int,OrganisationId int,Name nvarchar(max),Email nvarchar(500),Phone nvarchar(500),Address nvarchar(max),Description nvarchar(max),ModifiedDate datetime not null DEFAULT GETDATE(),[Inactive] [bit] NULL, [RowStatus] nvarchar(50))   
 
IF(@Date is NULL)
	SET @Date = GETDATE()
SET @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

INSERT into @OrganisationContactsHistory
 SELECT
		 CASE WHEN [Source].[ContactPersonId] IS NULL THEN OrganisationContacts.[ContactPersonId] ELSE [Source].[ContactPersonId] END AS [ContactPersonId]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN OrganisationContacts.[OrganisationId] ELSE [Source].[OrganisationId] END AS [OrganisationId]
		,CASE WHEN [Source].[Name] IS NULL THEN OrganisationContacts.[Name] ELSE [Source].[Name] END AS [Name]
		,CASE WHEN [Source].[Email] IS NULL THEN OrganisationContacts.[Email] ELSE [Source].[Email] END AS [Email]
		,CASE WHEN [Source].[Phone] IS NULL THEN OrganisationContacts.[Phone] ELSE [Source].[Phone] END AS [Phone]
		,CASE WHEN [Source].[Address] IS NULL THEN OrganisationContacts.[Address] ELSE [Source].[Address] END AS [Address]
		,CASE WHEN [Source].[ContactPersonId] IS NULL THEN OrganisationContacts.[Description] ELSE [Source].[Description] END AS [Description]		
		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[Inactive] IS NULL THEN OrganisationContacts.[Inactive] ELSE [Source].[Inactive] END AS [Inactive]	
		,CASE WHEN [Source].[ContactPersonId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	

					
	FROM
	(
		SELECT
			 cp.[ContactPersonId]
			,cp.[InstitutionId] AS [OrganisationId]
			,cp.[Name]
			,cp.[Email] COLLATE Latin1_General_CI_AS AS [Email]
			,cp.[Phone] COLLATE Latin1_General_CI_AS AS [Phone]
			,cp.[PostalAddress] COLLATE Latin1_General_CI_AS AS [Address]
			,cp.[Description] COLLATE Latin1_General_CI_AS AS [Description]
			,cp.Inactive			

		FROM [naati_db]..[tblContactPerson] cp
		LEFT JOIN [naati_db]..[vwDistinctPersonName] dpn ON dpn.[PersonId] = cp.[ContactPersonId]
		LEFT JOIN [naati_db]..[tblPersonName] pn ON pn.[PersonNameId] = dpn.[PersonNameId]		
	) [Source]
	FULL OUTER JOIN [OrganisationContacts] ON [Source].[ContactPersonId] = [OrganisationContacts].[ContactPersonId]
	WHERE (([Source].[OrganisationId] IS NOT NULL AND [OrganisationContacts].[OrganisationId] IS NULL) OR ([Source].[OrganisationId] IS NULL AND [OrganisationContacts].[OrganisationId] IS NOT NULL) OR (([Source].[OrganisationId] IS NOT NULL AND [OrganisationContacts].[OrganisationId] IS NOT NULL) AND [Source].[OrganisationId] != [OrganisationContacts].[OrganisationId]))
	OR (([Source].[Name] IS NOT NULL AND [OrganisationContacts].[Name] IS NULL) OR ([Source].[Name] IS NULL AND [OrganisationContacts].[Name] IS NOT NULL) OR (([Source].[Name] IS NOT NULL AND [OrganisationContacts].[Name] IS NOT NULL) AND [Source].[Name] != [OrganisationContacts].[Name]))
	OR (([Source].[Email] IS NOT NULL AND [OrganisationContacts].[Email] IS NULL) OR ([Source].[Email] IS NULL AND [OrganisationContacts].[Email] IS NOT NULL) OR (([Source].[Email] IS NOT NULL AND [OrganisationContacts].[Email] IS NOT NULL) AND [Source].[Email] != [OrganisationContacts].[Email]))
	OR (([Source].[Phone] IS NOT NULL AND [OrganisationContacts].[Phone] IS NULL) OR ([Source].[Phone] IS NULL AND [OrganisationContacts].[Phone] IS NOT NULL) OR (([Source].[Phone] IS NOT NULL AND [OrganisationContacts].[Phone] IS NOT NULL) AND [Source].[Phone] != [OrganisationContacts].[Phone]))
	OR (([Source].[Address] IS NOT NULL AND [OrganisationContacts].[Address] IS NULL) OR ([Source].[Address] IS NULL AND [OrganisationContacts].[Address] IS NOT NULL) OR (([Source].[Address] IS NOT NULL AND [OrganisationContacts].[Address] IS NOT NULL) AND [Source].[Address] != [OrganisationContacts].[Address]))
	OR (([Source].[Description] IS NOT NULL AND [OrganisationContacts].[Description] IS NULL) OR ([Source].[Description] IS NULL AND [OrganisationContacts].[Description] IS NOT NULL) OR (([Source].[Description] IS NOT NULL AND [OrganisationContacts].[Description] IS NOT NULL) AND [Source].[Description] != [OrganisationContacts].[Description]))
	OR (([Source].[Inactive] IS NOT NULL AND [OrganisationContacts].[Inactive] IS NULL) OR ([Source].[Inactive] IS NULL AND [OrganisationContacts].[Inactive] IS NOT NULL) OR (([Source].[Inactive] IS NOT NULL AND [OrganisationContacts].[Inactive] IS NOT NULL) AND [Source].[Inactive] != [OrganisationContacts].[Inactive]))
				
	--select * from @OrganisationContactsHistory

	BEGIN TRANSACTION
	   --Merge operation delete
		MERGE OrganisationContactsHistory AS Target USING(select * from @OrganisationContactsHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[ContactPersonId] = Source.[ContactPersonId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE OrganisationContactsHistory AS Target USING(	select * from @OrganisationContactsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[ContactPersonId] = Source.[ContactPersonId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE OrganisationContactsHistory AS Target USING(	select * from @OrganisationContactsHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[ContactPersonId] = Source.[ContactPersonId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		   Target.[ContactPersonId] = Source.[ContactPersonId]
		  ,Target.[OrganisationId] = Source.[OrganisationId]
		  ,Target.[Name] = Source.[Name]
		  ,Target.[Email] = Source.[Email]
		  ,Target.[Phone] = Source.[Phone]
		  ,Target.[Address] = Source.[Address]      
		  ,Target.[Description] = Source.[Description]
		  ,Target.[Inactive] = Source.[Inactive]
		  ,Target.[ModifiedDate] = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		  INSERT([ContactPersonId],[OrganisationId],[Name],[Email],[Phone],[Address],[Description],[ModifiedDate],[Inactive],[RowStatus]) VALUES (Source.[ContactPersonId],Source.[OrganisationId],Source.[Name],Source.[Email],Source.[Phone],Source.[Address],Source.[Description],@Date, Source.[Inactive], 'Latest');
		  	
	COMMIT TRANSACTION;	


END
