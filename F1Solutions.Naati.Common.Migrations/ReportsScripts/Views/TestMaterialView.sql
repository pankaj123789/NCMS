ALTER VIEW [dbo].[TestMaterial] AS
SELECT
	[TestMaterialId]
	,[Title]
	,[LanguageId]
	,[Language]
	,[SkillId]
	,[Skill]
	,[Available]
	,[MaterialRequestId]
	,[TestMaterialTypeId]
	,[TestMaterialType]
	,[TestMaterialDomainId]
	,[TestMaterialDomain]	
FROM [TestMaterialHistory] a
where [RowStatus] = 'Latest'