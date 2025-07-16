ALTER PROCEDURE [dbo].[PrerequisiteApplications]

@ApplicationId int

AS

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
			rootCredentialApplication.PersonId,
			rootCredentialType.InternalName
		FROM
			tblCredentialRequest rootCredentialRequest
			join tblCredentialApplication rootCredentialApplication on rootCredentialRequest.CredentialApplicationId = rootCredentialApplication.CredentialApplicationId
			join tblCredentialType rootCredentialType on rootCredentialRequest.CredentialTypeId = rootCredentialType.CredentialTypeId
			join tblCredentialPrerequisite rootPrerequisite on rootCredentialRequest.CredentialTypeId = rootPrerequisite.CredentialTypeId and rootCredentialApplication.CredentialApplicationTypeId = rootPrerequisite.CredentialApplicationTypeId
			join tblSkill rootSkill on rootCredentialRequest.SkillId = rootSkill.SkillId
		WHERE
			rootCredentialApplication.CredentialApplicationId = @ApplicationId

		UNION ALL
	
		SELECT
			recursivePrerequisite.CredentialTypeId,
			recursivePrerequisite.CredentialTypePrerequisiteId,
			recursivePrerequisite.ApplicationTypePrerequisiteId,
			recursivePrerequisite.SkillMatch,
			prerequisiteHierarchy.SkillId,
			prerequisiteHierarchy.PersonId,
			prerequisiteHierarchy.InternalName
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
		PersonId,
		InternalName as RootCredentialType
	INTO #TempPrerequisiteHeirarchy
	FROM
		PrerequisiteHeirarchy

------------------------------------- FIND PERSONS CREDENTIALS THAT MATCH THE PREREQUISITE HIERARCHY DATA -------------------------------------
	-- SET UP TEMP RESULTS TABLE AS VARIABLE
	DECLARE @ResultsTable TABLE (CurrentCredentialRequestCredentialTypeName nvarchar(50), CurrentCredentialRequestLanguage1 nvarchar(50), CurrentCredentialRequestLanguage2 nvarchar(50),
								CurrentCredentialRequestSkillDirection nvarchar(50), RequiredPrerequisiteCredentialType nvarchar(50), RequiredPrerequisiteLanguage1 nvarchar(50), RequiredPrerequisiteLanguage2 nvarchar(50), 
								RequiredPrerequisiteSkillMatch bit, ExistingApplicationId int, ExistingApplicationStatusTypeId int, ExistingApplicationStatusName nvarchar(50), 
								ExistingApplicationTypeName nvarchar(50), ExistingApplicationAutoCreated bit, ExistingCredentialRequestCredentialTypeName nvarchar(50),
								ExistingCredentialRequestSkillId int, ExistingCredentialRequestSkillDirection nvarchar(50), ExistingCredentialRequestLanguage1 nvarchar(50), ExistingCredentialRequestLanguage2 nvarchar(50),
								ExistingCredentialRequestStatusTypeId int, ExsitingCredentialRequestStatusName nvarchar(50), ExistingCredentialRequestAutoCreated bit, ExistingApplicationCreatedDate datetime)


	-- SET UP CURSOR TO ITERATE TEMP TABLE TO FIND MATCHING CREDENTIALS FOR EACH PREREQUISITE LEVEL
	DECLARE 
		@CredentialTypeId int,
		@CredentialTypePrerequisiteId int,
		@SkillMatch bit,
		@ApplicationTypePrerequisiteId int,
		@RootSkillId int,
		@PersonId int,
		@RootCredentialTypeName nvarchar(50)

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
		@PersonId,
		@RootCredentialTypeName

	-- GO THROUGH EACH LEVEL OF THE HIEARCHY TO FIND MATCHING CREDENTIALS, IF THERE AREN'T ANY THEN THE EXISTING RESULTS WILL BE NULL
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO @ResultsTable
			SELECT
				@RootCredentialTypeName as [CurrentCredentialRequestCredentialTypeName],
				[Prerequisites].[CurrentCredentialRequestLanguage1],
				[Prerequisites].[CurrentCredentialRequestLanguage2],
				[Prerequisites].[CurrentCredentialRequestSkillDirection],
				[Prerequisites].[RequiredPrerequisiteCredentialType],
				[Prerequisites].[RequiredPrerequisiteLanguage1],
				[Prerequisites].[RequiredPrerequisiteLanguage2],
				@SkillMatch as [RequiredPrerequisiteSkillMatch],
				[Existing].[ExistingApplicationId],
				[Existing].[ExistingApplicationStatusTypeId],
				[Existing].[ExistingApplicationStatusName],
				[Existing].[ExistingApplicationTypeName],
				[Existing].[ExistingApplicationAutoCreated],
				[Existing].[ExistingCredentialRequestCredentialTypeName],
				[Existing].[ExistingCredentialRequestSkillId],
				[Existing].[ExistingCredentialRequestSkillDirection],
				[Existing].[ExistingCredentialRequestLanguage1],
				[Existing].[ExistingCredentialRequestLanguage2],
				[Existing].[ExistingCredentialRequestStatusTypeId],
				[Existing].[ExsitingCredentialRequestStatusName],
				[Existing].[ExistingCredentialRequestAutoCreated],
				[Existing].[ExistingApplicationCreatedDate]
			FROM
			(
				-- GET THE CREDENTIAL NAMES AND LANGUAGES FOR THE PREREQUISITE HIERARCHY
				SELECT
					prerequisiteCredentialType.InternalName AS [RequiredPrerequisiteCredentialType],
					CASE
						WHEN @CredentialTypePrerequisiteId = 16 THEN 'Ethics' -- if ethics then language 1 should be ethics, otherwise just use the root language 1
						ELSE 
							rootLanguage1.Name
					END AS [RequiredPrerequisiteLanguage1],
					CASE
						WHEN @CredentialTypePrerequisiteId = 16 THEN 'Ethics' -- if ethics then language 2 should be ethics, otherwise if skill match it should be root language 2, if not skill match use rootLanguage 1 again
						ELSE 
							CASE
								WHEN @SkillMatch = 1 THEN rootLanguage2.Name
								ELSE rootLanguage1.Name
							END
					END AS [RequiredPrerequisiteLanguage2],
					rootDirection.DisplayName as [CurrentCredentialRequestSkillDirection],
					rootLanguage1.Name as [CurrentCredentialRequestLanguage1],
					rootLanguage2.Name as [CurrentCredentialRequestLanguage2]
				FROM 
					tblCredentialType prerequisiteCredentialType,
					tblSkill rootSkill
					join tblLanguage rootLanguage1 on rootSkill.Language1Id = rootLanguage1.LanguageId
					join tblLanguage rootLanguage2 on rootSkill.Language2Id = rootLanguage2.LanguageId
					join tblDirectionType rootDirection on rootSkill.DirectionTypeId = rootDirection.DirectionTypeId
				WHERE
					prerequisiteCredentialType.CredentialTypeId = @CredentialTypePrerequisiteId
					and rootSkill.SkillId = @RootSkillId
			) [Prerequisites] INNER JOIN
			(
				-- GET CREDENTIALS THAT MATCH THE PREREQUISITE HIERARCHY IF THEY EXIST
				SELECT
					existingRequest.AutoCreated as [ExistingCredentialRequestAutoCreated],
					existingRequestCredType.InternalName as [ExistingCredentialRequestCredentialTypeName],
					existingRequestStatus.CredentialRequestStatusTypeId as [ExistingCredentialRequestStatusTypeId],
					existingRequestStatus.DisplayName as [ExsitingCredentialRequestStatusName],

					existingLanguage1.Name as [ExistingCredentialRequestLanguage1],
					existingLanguage2.Name as [ExistingCredentialRequestLanguage2],
					existingDirection.DisplayName as [ExistingCredentialRequestSkillDirection],
					existingApplication.AutoCreated as [ExistingApplicationAutoCreated],
					existingSkill.SkillId as [ExistingCredentialRequestSkillId],

					existingApplication.CredentialApplicationId as [ExistingApplicationId],
					existingApplication.EnteredDate as [ExistingApplicationCreatedDate],
					existingApplication.CredentialApplicationTypeId,
					existingApplicationStatus.CredentialApplicationStatusTypeId as [ExistingApplicationStatusTypeId],
					existingApplicationStatus.DisplayName as [ExistingApplicationStatusName],
					existingApplicationType.DisplayName as [ExistingApplicationTypeName]
				FROM
					tblCredentialApplication existingApplication
					inner join tblCredentialApplicationStatusType existingApplicationStatus on existingApplication.CredentialApplicationStatusTypeId = existingApplicationStatus.CredentialApplicationStatusTypeId
					inner join tblCredentialApplicationType existingApplicationType on existingApplication.CredentialApplicationTypeId = existingApplicationType.CredentialApplicationTypeId
					inner join tblCredentialRequest existingRequest on existingApplication.CredentialApplicationId = existingRequest.CredentialApplicationId
					inner join tblCredentialRequestStatusType existingRequestStatus on existingRequest.CredentialRequestStatusTypeId = existingRequestStatus.CredentialRequestStatusTypeId
					inner join tblCredentialType existingRequestCredType on existingRequest.CredentialTypeId = existingRequestCredType.CredentialTypeId
					inner join tblSkill existingSkill on existingRequest.SkillId = existingSkill.SkillId
					inner join tblLanguage existingLanguage1 on existingSkill.Language1Id = existingLanguage1.LanguageId
					inner join tblLanguage existingLanguage2 on existingSkill.Language2Id = existingLanguage2.LanguageId
					inner join tblDirectionType existingDirection on existingSkill.DirectionTypeId = existingDirection.DirectionTypeId
				WHERE
					existingApplication.PersonId = @PersonId
					AND existingApplication.AutoCreated = 1
					AND existingApplication.CredentialApplicationTypeId = @ApplicationTypePrerequisiteId
					AND existingRequestStatus.CredentialRequestStatusTypeId NOT IN (2, 13, 14)
					AND existingApplicationStatus.CredentialApplicationStatusTypeId NOT IN (4, 7)
			) [Existing]
			ON [Existing].[ExistingCredentialRequestCredentialTypeName] = [Prerequisites].[RequiredPrerequisiteCredentialType]
			and [Existing].[ExistingCredentialRequestLanguage1] = 
					CASE
						WHEN @CredentialTypePrerequisiteId = 16 THEN 'Ethics'
						ELSE [Prerequisites].[RequiredPrerequisiteLanguage1]
					END
			and [Existing].[ExistingCredentialRequestLanguage2] = 
					CASE
						WHEN @CredentialTypePrerequisiteId = 16 THEN 'Ethics'
						ELSE
							CASE
								WHEN @SkillMatch = 1 THEN [Prerequisites].[RequiredPrerequisiteLanguage2]
								ELSE [Prerequisites].[RequiredPrerequisiteLanguage1]
							END
					END
		-- GO TO THE NEXT LEVEL OF THE HIERARCHY AND REPEAT
		FETCH NEXT FROM db_cursor INTO
		@CredentialTypeId,
		@CredentialTypePrerequisiteId,
		@SkillMatch,
		@ApplicationTypePrerequisiteId,
		@RootSkillId,
		@PersonId,
		@RootCredentialTypeName
	END
	-- CLEAN UP THE CURSOR
	CLOSE db_cursor
	DEALLOCATE db_cursor

	-- DROP THE TEMP TABLE AT THE END OF THE PROCEDURE SO IT IS NO LONGER IN THE DB
	DROP TABLE IF EXISTS #TempPrerequisiteHeirarchy

	-- SELECT THE RESULTS SO THAT THE PROCEDURE RETURNS THE RESULTS
	SELECT DISTINCT
		* 
	FROM
		@ResultsTable