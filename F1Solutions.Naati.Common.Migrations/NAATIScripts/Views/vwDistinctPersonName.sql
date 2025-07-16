CREATE VIEW vwDistinctPersonName 
AS 
  SELECT pn.PersonId, 
         MAX(PersonNameId) AS PersonNameId 
  FROM   tblPersonName pn 
         INNER JOIN vwMaxPersonNameEffectiveDate vef 
                 ON vef.PersonId = pn.PersonId 
                    AND vef.EffectiveDate = pn.EffectiveDate 
  GROUP  BY pn.PersonId 