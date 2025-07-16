
CREATE   VIEW [dbo].[vwRolePlayerLastTestSession]
--WITH SCHEMABINDING  
AS 
WITH RolePlayerSessions
as
(
SELECT testSessionRolePlayer.RolePlayerId, Max(testSession.TestDateTime) AS LastAttendedDate FROM tblTestSessionRolePlayer testSessionRolePlayer
INNER JOIN tblTestSession testSession on testSession.TestSessionId = testSessionRolePlayer.TestSessionId
where testSessionRolePlayer.Rejected = 0 And testSessionRolePlayer.Attended = 1
group by testSessionRolePlayer.RolePlayerId
)
select  RolePlayerSessions.RolePlayerId AS RolePlayerId, MAX(testSession.TestSessionId) AS LastAttendedTestSessionId   FROM	RolePlayerSessions
INNER join tblTestSessionRolePlayer testSessionRolePlayer ON RolePlayerSessions.RolePlayerId = testSessionRolePlayer.RolePlayerId
INNER JOIN tblTestSession testSession on testSession.TestSessionId = testSessionRolePlayer.TestSessionId
where testSessionRolePlayer.Rejected = 0 And testSessionRolePlayer.Attended = 1 AND testSession.TestDateTime =  RolePlayerSessions.LastAttendedDate
group by RolePlayerSessions.RolePlayerId

