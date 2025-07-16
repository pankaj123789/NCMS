ALTER PROCEDURE [dbo].[EmailSelect]
(
	@EntityId int
)
AS 

SELECT		e.EmailId, e.EntityId, e.Email, e.Note, e.IncludeInPD, e.Invalid, e.IsPreferredEmail, e.ExaminerCorrespondence
FROM		tblEmail e
WHERE		e.EntityId = @EntityId and (e.IsPreferredEmail = 1 or e.Invalid = 0)