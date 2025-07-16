CREATE VIEW vwDistinctInstitutionName 
AS 
  SELECT InstitutionId, 
         MAX(InstitutionNameId) AS InstitutionNameId 
  FROM   dbo.tblInstitutionName 
  GROUP  BY InstitutionId 