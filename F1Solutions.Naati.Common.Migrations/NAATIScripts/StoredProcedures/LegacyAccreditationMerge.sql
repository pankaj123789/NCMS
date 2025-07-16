
ALTER PROCEDURE [dbo].[LegacyAccreditationMerge]
AS
insert into [naati_db].[dbo].[tblLegacyAccreditation]
select 
a.ApplicationId,
al.WebDisplay as 'Level', ac.Description as'Category',
(CASE WHEN am.AccreditationMethodId != (select AccreditationMethodId from [sam4_db].[dbo].[tluAccreditationMethod] where Code = 'E') 
THEN (CASE WHEN ToEnglish is null THEN 'B' ELSE (CASE WHEN ToEnglish = 1 THEN 'E' ELSE 'O' END) END) 
ELSE (CASE WHEN LanguageToBoth = 1 THEN 'B' ELSE 'O' END) END) 
as 'Direction',l.Name as 'Language 1', ISNULL(l2.Name,'English') as 'Language 2',
ar.ResultDate as 'StartDate',
 ar.ExpiryDate,
 ar.IncludeInPD as 'IncludeInOD',
 e.NAATINumber
from [sam4_db].[dbo].[tblApplication] a
left join [sam4_db].[dbo].[tblLanguageExperience] le on a.LanguageExperienceId = le.LanguageExperienceId
left join [sam4_db].[dbo].[tblLanguageExperience] le2 on a.ToLanguageExperienceId = le2.LanguageExperienceId
left join [sam4_db].[dbo].[tblLanguage] l on l.LanguageId = le.LanguageId
left join [sam4_db].[dbo].[tblLanguage] l2 on l2.LanguageId = le2.LanguageId
left join [sam4_db].[dbo].[tblPerson] p on le.PersonId = p.PersonId
left join [sam4_db].[dbo].[tluAccreditationLevel] al on al.AccreditationLevelId = a.AccreditationLevelId
left join [sam4_db].[dbo].[tluAccreditationCategory] ac on ac.AccreditationCategoryId = a.AccreditationCategoryId
left join [sam4_db].[dbo].[tluAccreditationMethod] am on am.AccreditationMethodId = a.AccreditationMethodId
left join [sam4_db].[dbo].[tblAccreditationResult] ar on ar.ApplicationId = a.ApplicationId
left join [sam4_db].[dbo].[tblEntity] e on e.EntityId = p.EntityId

left join [naati_db].[dbo].[tblLegacyAccreditation] la on la.accreditationId = a.ApplicationId
inner join [naati_db].[dbo].[tblEntity] e2 on e2.NaatiNumber = e.NaatiNumber

where (((ar.ResultTypeId = 2 or ar.ResultTypeId = 3) and am.AccreditationMethodId = 1)
 or (am.AccreditationMethodId != 6 and am.AccreditationMethodId != 1 and ar.FailureReasonId is null  and ar.AccreditationResultId is not null))  and la.accreditationId is null
order by p.personId, a.ApplicationId