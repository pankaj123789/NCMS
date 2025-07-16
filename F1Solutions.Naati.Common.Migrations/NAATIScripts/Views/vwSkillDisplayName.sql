CREATE VIEW dbo.vwSkillDisplayName 
AS 
  SELECT SkillId, 
         Replace(Replace(d.DisplayName, '[Language 1]', 
            l1.[Name]), '[Language 2]', 
         l2.[Name]) AS DisplayName 
  FROM   tblSkill 
         JOIN tblLanguage l1 
           ON l1.LanguageId = tblSkill.Language1Id 
         JOIN tblLanguage l2 
           ON l2.LanguageId = tblSkill.Language2Id 
         JOIN tblDirectionType d 
           ON d.DirectionTypeId = tblSkill.DirectionTypeId 