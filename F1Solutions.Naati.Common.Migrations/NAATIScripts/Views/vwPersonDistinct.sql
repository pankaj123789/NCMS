CREATE VIEW vwPersonDistinct 
AS 
  SELECT tblPerson.EntityId, 
         tblPersonName.GivenName, 
         tblPersonName.Surname 
  FROM   tblPerson 
         INNER JOIN tblPersonName 
                 ON tblPerson.PersonId = tblPersonName.PersonId 
         INNER JOIN vwDistinctPersonName 
                 ON tblPersonName.PersonNameId = vwDistinctPersonName.PersonNameId 