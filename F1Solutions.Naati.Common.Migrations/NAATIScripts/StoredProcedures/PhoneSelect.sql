ALTER PROCEDURE [PhoneSelect]
@EntityId int
WITH EXEC AS CALLER
AS
SELECT		p.PhoneId, p.EntityId, p.Number, p.CountryCode, p.AreaCode, p.LocalNumber, p.Note, p.IncludeInPD, p.PrimaryContact, p.Invalid, p.AllowSmsNotification,  p.ExaminerCorrespondence
FROM		tblPhone p
WHERE		EntityId = @EntityId and (p.PrimaryContact = 1 or p.Invalid = 0)