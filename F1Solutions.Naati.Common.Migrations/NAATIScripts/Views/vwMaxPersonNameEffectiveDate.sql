CREATE VIEW vwMaxPersonNameEffectiveDate 
AS 
  SELECT PersonId, 
         MAX(EffectiveDate) AS EffectiveDate 
  FROM   tblPersonName 
  GROUP  BY PersonId 