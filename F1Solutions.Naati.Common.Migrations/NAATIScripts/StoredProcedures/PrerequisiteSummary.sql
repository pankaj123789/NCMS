ALTER PROCEDURE [dbo].[PrerequisiteSummary]
	@CredentialRequestId int
AS
BEGIN
	SET NOCOUNT ON;
------------------------------------- CLEAN UP OF TEMP TABLES IN CASE THE PROCEDURE BOMBED BEFORE IT COULD DROP THE TABLE LAST RUN -------------------------------------
	DROP TABLE IF EXISTS #TempPrerequisiteHeirarchy;

------------------------------------- RECURSION TO EXTRACT THE PREREQUISITE HIERARCHY FOR THE GIVEN CREDENTIAL REQUEST -------------------------------------

	-- Using a technique called Common Table Expression (CTE) we are able to recursively extract a hierarchy of prerequisite data based on the given credential request
	WITH PrerequisiteHeirarchy 
	AS (
		SELECT 
			rootPrerequisite.CredentialTypeId,
			rootPrerequisite.CredentialTypePrerequisiteId,
			rootPrerequisite.ApplicationTypePrerequisiteId,
			rootPrerequisite.SkillMatch,
			rootSkill.SkillId,
			rootCredentialApplication.PersonId
		FROM
			tblCredentialRequest rootCredentialRequest
			join tblCredentialApplication rootCredentialApplication on rootCredentialRequest.CredentialApplicationId = rootCredentialApplication.CredentialApplicationId
			join tblCredentialPrerequisite rootPrerequisite on rootCredentialRequest.CredentialTypeId = rootPrerequisite.CredentialTypeId and rootCredentialApplication.CredentialApplicationTypeId = rootPrerequisite.CredentialApplicationTypeId
			join tblSkill rootSkill on rootCredentialRequest.SkillId = rootSkill.SkillId
		WHERE
			rootCredentialRequest.CredentialRequestId = @CredentialRequestId

		UNION ALL
	
		SELECT
			recursivePrerequisite.CredentialTypeId,
			recursivePrerequisite.CredentialTypePrerequisiteId,
			recursivePrerequisite.ApplicationTypePrerequisiteId,
			recursivePrerequisite.SkillMatch,
			prerequisiteHierarchy.SkillId,
			prerequisiteHierarchy.PersonId
		FROM
			tblCredentialPrerequisite recursivePrerequisite
			join PrerequisiteHeirarchy prerequisiteHierarchy on prerequisiteHierarchy.CredentialTypePrerequisiteId = recursivePrerequisite.CredentialTypeId and prerequisiteHierarchy.ApplicationTypePrerequisiteId = recursivePrerequisite.CredentialApplicationTypeId
	)
	SELECT
		CredentialTypeId,
		CredentialTypePrerequisiteId,
		SkillMatch,
		ApplicationTypePrerequisiteId,
		SkillId as RootSkillId,
		PersonId
	INTO #TempPrerequisiteHeirarchy
	FROM
		PrerequisiteHeirarchy

------------------------------------- FIND PERSONS CREDENTIALS THAT MATCH THE PREREQUISITE HIERARCHY DATA -------------------------------------

	-- SET UP TEMP RESULTS TABLE AS VARIABLE
	DECLARE @ResultsTable TABLE (PrerequisiteCredentialName nvarchar(50), PersonId int, ExistingApplicationId int, ExistingCredentialRequestId int, PrerequisiteCredentialId int,
								ExistingRequestCredentialTypeId int, MatchingCredentialName nvarchar(50), PrerequisiteCredentialLanguage1 nvarchar(50), PrerequisiteCredentialLanguage2 nvarchar(50), 
								PrerequistieDirection nvarchar(50), MatchingCredentialLanguage1 nvarchar(50),MatchingCredentialLanguage2 nvarchar(50), MatchingCredentialDirection nvarchar(50),
								Match bit, CredentialPrerequistieSkillMatch bit, MatchingCredentialStartDate DateTime, MatchingCredentialEndDate DateTime, 
								MatchingCredentialCertificationPeriodId int)

	-- SET UP CURSOR TO ITERATE TEMP TABLE TO FIND MATCHING CREDENTIALS FOR EACH PREREQUISITE LEVEL
	DECLARE 
		@CredentialTypeId int,
		@CredentialTypePrerequisiteId int,
		@SkillMatch bit,
		@ApplicationTypePrerequisiteId int,
		@RootSkillId int,
		@PersonId int

	DECLARE db_cursor CURSOR FOR
		SELECT
			*
		FROM
			#TempPrerequisiteHeirarchy

	OPEN db_cursor

	FETCH NEXT FROM db_cursor INTO
		@CredentialTypeId,
		@CredentialTypePrerequisiteId,
		@SkillMatch,
		@ApplicationTypePrerequisiteId,
		@RootSkillId,
		@PersonId

	-- GO THROUGH EACH LEVEL OF THE HIEARCHY TO FIND MATCHING CREDENTIALS, IF THERE AREN'T ANY THEN THE EXISTING RESULTS WILL BE NULL
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO @ResultsTable
			SELECT
				[Prerequisites].[PrerequisiteCredentialName],
				@PersonId,
				[Existing].[ExistingApplicationId],
				[Existing].[ExistingCredentialRequestId],
				[Prerequisites].[PrerequisiteCredentialTypeId],
				[Existing].[ExistingCredentialRequestCredentialTypeId],
				[Existing].[MatchingCredentialName],
				[Prerequisites].[PrerequisiteCredentialLanguage1],
				[Prerequisites].[PrerequisiteCredentialLanguage2],
				[Prerequisites].[PrerequistieDirection],
				[Existing].[MatchingCredentialLanguage1],
				[Existing].[MatchingCredentialLanguage2],
				[Existing].[MatchingCredentialDirection],
				CASE
					WHEN
						@SkillMatch = 1
					THEN
						CASE
							WHEN
								[Prerequisites].[PrerequisiteCredentialLanguage1] = [Existing].[MatchingCredentialLanguage1]
								AND [Prerequisites].[PrerequisiteCredentialLanguage2] = [Existing].[MatchingCredentialLanguage2]
							THEN 1
							ELSE 0
						END
					ELSE
						CASE
							WHEN
								[Prerequisites].[PrerequisiteCredentialLanguage1] = [Existing].[MatchingCredentialLanguage1]
							THEN 1
							ELSE 0
						END
				END as [Match],
				@SkillMatch as [CredentialPrerequistieSkillMatch],
				[Existing].[MatchingCredentialStartDate],
				[Existing].[MatchingCredentialEndDate],
				[Existing].[MatchingCredentialCertificationPeriodId]
			FROM
			(
				-- GET THE CREDENTIAL NAMES AND LANGUAGES FOR THE PREREQUISITE HIERARCHY
				SELECT
					prerequisiteCredentialType.InternalName AS [PrerequisiteCredentialName],
					CASE
						WHEN @CredentialTypePrerequisiteId = 16 THEN 'Ethics' -- if ethics then language 1 should be ethics, otherwise just use the root language 1
						ELSE 
							rootLanguage1.Name
					END AS [PrerequisiteCredentialLanguage1],
					CASE
						WHEN @CredentialTypePrerequisiteId = 16 THEN 'Ethics' -- if ethics then language 2 should be ethics, otherwise if skill match it should be root language 2, if not skill match use rootLanguage 1 again
						ELSE 
							CASE
								WHEN @SkillMatch = 1 THEN rootLanguage2.Name
								ELSE rootLanguage1.Name
							END
					END AS [PrerequisiteCredentialLanguage2],
					rootDirection.DisplayName as [PrerequistieDirection],
					prerequisiteCredentialType.CredentialTypeId as [PrerequisiteCredentialTypeId]
				FROM 
					tblCredentialType prerequisiteCredentialType,
					tblSkill rootSkill
					join tblLanguage rootLanguage1 on rootSkill.Language1Id = rootLanguage1.LanguageId
					join tblLanguage rootLanguage2 on rootSkill.Language2Id = rootLanguage2.LanguageId
					join tblDirectionType rootDirection on rootSkill.DirectionTypeId = rootDirection.DirectionTypeId
				WHERE
					prerequisiteCredentialType.CredentialTypeId = @CredentialTypePrerequisiteId
					and rootSkill.SkillId = @RootSkillId
			) [Prerequisites] LEFT OUTER JOIN
			(
				-- GET CREDENTIALS THAT MATCH THE PREREQUISITE HIERARCHY IF THEY EXIST
				SELECT
					existingRequestCredType.InternalName as [MatchingCredentialName],
					existingRequest.CredentialRequestId as [ExistingCredentialRequestId],
					existingRequest.CredentialTypeId as [ExistingCredentialRequestCredentialTypeId],
					existingApplication.CredentialApplicationId as [ExistingApplicationId],
					existingLanguage1.Name as [MatchingCredentialLanguage1],
					existingLanguage2.Name as [MatchingCredentialLanguage2],
					existingDirection.DisplayName as [MatchingCredentialDirection],
					CASE 
						WHEN 
							existingCertificationPeriod.CertificationPeriodId is null 
						THEN existingCredential.StartDate
						ELSE existingCertificationPeriod.StartDate
					END AS [MatchingCredentialStartDate],
					CASE 
						WHEN 
							existingCertificationPeriod.CertificationPeriodId is null 
						THEN existingCredential.ExpiryDate
						ELSE existingCertificationPeriod.EndDate
					END AS [MatchingCredentialEndDate],
					existingCertificationPeriod.CertificationPeriodId as [MatchingCredentialCertificationPeriodId]
				FROM
					tblCredentialApplication existingApplication
					inner join tblCredentialRequest existingRequest on existingApplication.CredentialApplicationId = existingRequest.CredentialApplicationId
					inner join tblCredentialType existingRequestCredType on existingRequest.CredentialTypeId = existingRequestCredType.CredentialTypeId
					inner join tblSkill existingSkill on existingRequest.SkillId = existingSkill.SkillId
					inner join tblLanguage existingLanguage1 on existingSkill.Language1Id = existingLanguage1.LanguageId
					inner join tblLanguage existingLanguage2 on existingSkill.Language2Id = existingLanguage2.LanguageId
					inner join tblDirectionType existingDirection on existingSkill.DirectionTypeId = existingDirection.DirectionTypeId
					inner join tblCredentialCredentialRequest existingCredentialCredentialRequest on existingRequest.CredentialRequestId = existingCredentialCredentialRequest.CredentialRequestId
					inner join tblCredential existingCredential on existingCredential.CredentialId = existingCredentialCredentialRequest.CredentialId
					left join tblCertificationPeriod existingCertificationPeriod on existingCertificationPeriod.CertificationPeriodId = existingCredential.CertificationPeriodId
				WHERE
					existingApplication.PersonId = @PersonId
					AND existingApplication.CredentialApplicationTypeId = @ApplicationTypePrerequisiteId
					AND existingApplication.CredentialApplicationStatusTypeId NOT IN (4, 7)
					AND existingRequest.CredentialRequestStatusTypeId = 12 -- Certification Issued
					AND existingRequest.CredentialRequestPathTypeId <> 4 -- recertify
					AND (
							existingCertificationPeriod.EndDate >= GetDate() OR 
							(existingCertificationPeriod.EndDate IS NULL AND existingCredential.ExpiryDate >= GetDate()) 
						)
			) [Existing]
			ON [Existing].[MatchingCredentialName] = [Prerequisites].[PrerequisiteCredentialName]
			AND
			[Existing].[MatchingCredentialLanguage1] = [Prerequisites].[PrerequisiteCredentialLanguage1]
			AND [Existing].[MatchingCredentialLanguage2] = [Prerequisites].PrerequisiteCredentialLanguage2
		-- GO TO THE NEXT LEVEL OF THE HIERARCHY AND REPEAT
		FETCH NEXT FROM db_cursor INTO
		@CredentialTypeId,
		@CredentialTypePrerequisiteId,
		@SkillMatch,
		@ApplicationTypePrerequisiteId,
		@RootSkillId,
		@PersonId
	END
	-- CLEAN UP THE CURSOR
	CLOSE db_cursor
	DEALLOCATE db_cursor

	-- DROP THE TEMP TABLE AT THE END OF THE PROCEDURE SO IT IS NO LONGER IN THE DB
	DROP TABLE IF EXISTS #TempPrerequisiteHeirarchy

	-- SELECT THE RESULTS SO THAT THE PROCEDURE RETURNS THE RESULTS
	SELECT 
		* 
	FROM
		@ResultsTable
END