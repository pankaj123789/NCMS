ALTER PROCEDURE [dbo].[ReportingSnapshot_Organisation]
	@Date DateTime
AS
BEGIN
	DECLARE @OrganisationHistory as table( OrganisationId int, NAATINumber int, Name nvarchar(100),PrimaryAddress nvarchar(500),Suburb nvarchar(50),Country nvarchar(50),PrimaryPhone nvarchar(60),PrimaryEmail nvarchar(200),TrustedPayer bit,ModifiedDate datetime not null DEFAULT GETDATE(), [RowStatus] nvarchar(50))   
	
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @OrganisationHistory
  SELECT
		CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[OrganisationId] ELSE [Source].[OrganisationId] END AS [OrganisationId]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[NAATINumber] ELSE [Source].[NAATINumber] END AS [NAATINumber]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[Name] ELSE [Source].[Name] END AS [Name]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[PrimaryAddress] ELSE [Source].[PrimaryAddress] END AS [PrimaryAddress]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[Suburb] ELSE [Source].[Suburb] END AS [Suburb]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[Country] ELSE [Source].[Country] END AS [Country]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[PrimaryPhone] ELSE [Source].[PrimaryPhone] END AS [PrimaryPhone]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[PrimaryEmail] ELSE [Source].[PrimaryEmail] END AS [PrimaryEmail]
		,CASE WHEN [Source].[OrganisationId] IS NULL THEN [Organisation].[TrustedPayer] ELSE [Source].[TrustedPayer] END AS [TrustedPayer]
		,@Date AS [ModifiedDate]

		,CASE WHEN [Source].[OrganisationId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]			
	FROM
	(
		SELECT
			i.[InstitutionId] AS 'OrganisationId'
			,e.[NAATINumber]
			,ina.[Name]
			,ad.[StreetDetails] COLLATE Latin1_General_CI_AS AS 'PrimaryAddress'
			,s.[Suburb] COLLATE Latin1_General_CI_AS AS 'Suburb'
			,c.[Name] COLLATE Latin1_General_CI_AS AS 'Country'
			,ph.[Number] COLLATE Latin1_General_CI_AS AS 'PrimaryPhone'
			,pe.[Email] COLLATE Latin1_General_CI_AS AS 'PrimaryEmail'
			,i.[TrustedPayer]
			
		FROM [naati_db]..[tblInstitution] i
		LEFT JOIN [naati_db]..[tblEntity] e ON i.[EntityId] = e.[EntityId]
		LEFT JOIN [naati_db]..[vwDistinctInstitutionName] distinctIna ON i.[InstitutionId] = distinctIna.[InstitutionId]
		LEFT JOIN [naati_db]..[tblInstitutionName] ina ON distinctIna.[InstitutionNameId] = ina.[InstitutionNameId]
		OUTER APPLY
		(
			SELECT TOP 1
			[StreetDetails]
			,[AddressId]
			,[PostcodeId]
			,[CountryId]
			FROM [naati_db]..[tblAddress] ad
			WHERE ad.[EntityId] = e.[EntityId] AND ad.[PrimaryContact] = 1
		) ad
		LEFT JOIN [naati_db]..[tblPostCode] p ON ad.[PostcodeId] = p.[PostcodeId]
		LEFT JOIN [naati_db]..[tblSuburb] s ON s.[SuburbId] = p.[SuburbId]
		LEFT JOIN [naati_db]..[tblCountry] c ON c.[CountryId] = ad.[CountryId]
		OUTER APPLY
		(
			SELECT TOP 1
			[Number]
			FROM [naati_db]..[tblPhone] ph
			WHERE ph.[EntityId] = e.[EntityId] AND ph.[PrimaryContact] = 1
		) ph
		OUTER APPLY
		(
			SELECT TOP 1
			[EmailId]
			,[Email]
			FROM [naati_db]..[tblEmail] pe
			WHERE pe.[EntityId] = e.[EntityId] AND pe.[IsPreferredEmail] = 1
		) pe
	) [Source]
	FULL OUTER JOIN [Organisation] ON [Source].[OrganisationId] = [Organisation].[OrganisationId]
	WHERE
	   (([Source].[NAATINumber] IS NOT NULL AND [Organisation].[NAATINumber] IS NULL) OR ([Source].[NAATINumber] IS NULL AND [Organisation].[NAATINumber] IS NOT NULL) OR (([Source].[NAATINumber] IS NOT NULL AND [Organisation].[NAATINumber] IS NOT NULL) AND [Source].[NAATINumber] != [Organisation].[NAATINumber]))
	  
	OR (([Source].[Name] IS NOT NULL AND [Organisation].[Name] IS NULL) OR ([Source].[Name] IS NULL AND [Organisation].[Name] IS NOT NULL) OR (([Source].[Name] IS NOT NULL AND [Organisation].[Name] IS NOT NULL) AND [Source].[Name] != [Organisation].[Name]))
	OR (([Source].[PrimaryAddress] IS NOT NULL AND [Organisation].[PrimaryAddress] IS NULL) OR ([Source].[PrimaryAddress] IS NULL AND [Organisation].[PrimaryAddress] IS NOT NULL) OR (([Source].[PrimaryAddress] IS NOT NULL AND [Organisation].[PrimaryAddress] IS NOT NULL) AND [Source].[PrimaryAddress] != [Organisation].[PrimaryAddress]))
	OR (([Source].[Suburb] IS NOT NULL AND [Organisation].[Suburb] IS NULL) OR ([Source].[Suburb] IS NULL AND [Organisation].[Suburb] IS NOT NULL) OR (([Source].[Suburb] IS NOT NULL AND [Organisation].[Suburb] IS NOT NULL) AND [Source].[Suburb] != [Organisation].[Suburb]))
	OR (([Source].[Country] IS NOT NULL AND [Organisation].[Country] IS NULL) OR ([Source].[Country] IS NULL AND [Organisation].[Country] IS NOT NULL) OR (([Source].[Country] IS NOT NULL AND [Organisation].[Country] IS NOT NULL) AND [Source].[Country] != [Organisation].[Country]))
	OR (([Source].[PrimaryPhone] IS NOT NULL AND [Organisation].[PrimaryPhone] IS NULL) OR ([Source].[PrimaryPhone] IS NULL AND [Organisation].[PrimaryPhone] IS NOT NULL) OR (([Source].[PrimaryPhone] IS NOT NULL AND [Organisation].[PrimaryPhone] IS NOT NULL) AND [Source].[PrimaryPhone] != [Organisation].[PrimaryPhone]))
	OR (([Source].[PrimaryEmail] IS NOT NULL AND [Organisation].[PrimaryEmail] IS NULL) OR ([Source].[PrimaryEmail] IS NULL AND [Organisation].[PrimaryEmail] IS NOT NULL) OR (([Source].[PrimaryEmail] IS NOT NULL AND [Organisation].[PrimaryEmail] IS NOT NULL) AND [Source].[PrimaryEmail] != [Organisation].[PrimaryEmail]))
	OR (([Source].[TrustedPayer] IS NOT NULL AND [Organisation].[TrustedPayer] IS NULL) OR ([Source].[TrustedPayer] IS NULL AND [Organisation].[TrustedPayer] IS NOT NULL) OR (([Source].[TrustedPayer] IS NOT NULL AND [Organisation].[TrustedPayer] IS NOT NULL) AND [Source].[TrustedPayer] != [Organisation].[TrustedPayer]))
	
	--select * from @OrganisationHistory

	BEGIN TRANSACTION
	
	   --Merge operation delete
		MERGE OrganisationHistory AS Target USING(select * from @OrganisationHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[OrganisationId] = Source.[OrganisationId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE OrganisationHistory AS Target USING(	select * from @OrganisationHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[OrganisationId] = Source.[OrganisationId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE OrganisationHistory AS Target USING(	select * from @OrganisationHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[OrganisationId] = Source.[OrganisationId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		  Target.[OrganisationId] = Source.[OrganisationId]
		  ,Target.[NAATINumber] = Source.[NAATINumber]
		  ,Target.[Name] = Source.[Name]
		  ,Target.[PrimaryAddress] = Source.[PrimaryAddress]
		  ,Target.[Suburb] = Source.[Suburb]
		  ,Target.[Country] = Source.[Country]      
		  ,Target.[PrimaryPhone] = Source.[PrimaryPhone]
		  ,Target.[PrimaryEmail] = Source.[PrimaryEmail]
		  ,Target.[TrustedPayer] = Source.[TrustedPayer]
		  ,Target.[ModifiedDate] = @Date
		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		 INSERT(OrganisationId, NAATINumber, Name,PrimaryAddress,Suburb,Country,PrimaryPhone,PrimaryEmail,TrustedPayer,ModifiedDate, [RowStatus]) 
		 VALUES (Source.OrganisationId,Source.NAATINumber,Source.[Name],Source.PrimaryAddress,Source.Suburb,Source.Country,Source.PrimaryPhone,Source.PrimaryEmail,Source.TrustedPayer,@Date, 'Latest');
	 	  	
	COMMIT TRANSACTION;	

END
