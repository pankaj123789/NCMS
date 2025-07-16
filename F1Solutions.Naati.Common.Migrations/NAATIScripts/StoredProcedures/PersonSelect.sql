ALTER PROCEDURE [dbo].[PersonSelect]
@EntityId int
AS

DECLARE @IsRolePlayerAvailable bit

DECLARE @PersonId int

SET @IsRolePlayerAvailable = (SELECT CASE Value WHEN 'true' THEN 1 ELSE 0 END FROM tblSystemValue WHERE ValueKey = 'RolePlayerAvailable')

SELECT @PersonId = PersonId from tblPerson WHERE EntityId = @EntityId

SELECT *
	,CAST(CASE WHEN PersonCredentials > 0 AND PersonCredentials = ExpiredCredentials THEN 1 ELSE 0 END AS BIT) IsFormerPractitioner
	,CAST(CASE WHEN PersonCredentials > 0 AND ActiveCredentials > 0 THEN 1 ELSE 0 END AS BIT) IsPractitioner
	,CAST(CASE WHEN ActiveApplications > 0 THEN 1 ELSE 0 END AS BIT) IsApplicant
	,CAST(CASE WHEN PersonCredentials > 0 AND PersonCredentials = FutureCredentials THEN 1 ELSE 0 END AS BIT) IsFuturePractitioner
	,CAST(CASE WHEN ActiveExaminerMemberships > 0 THEN 1 ELSE 0 END AS BIT) IsExaminer
	,CAST(CASE WHEN ActiveRolePlayerMemberships > 0 THEN 1 ELSE 0 END AS BIT) IsRolePlayer
	,@IsRolePlayerAvailable  IsRolePlayerAvailable
FROM (
	SELECT TOP 1
		p.PersonId
		,p.EntityId
		,pn.GivenName
		,pn.AlternativeGivenName
		,pn.OtherNames
		,pn.Surname
		,pn.AlternativeSurname
		,p.Gender
		,p.BirthDate
		,p.BirthCountryId
		,c.Name as BirthCountry
		,p.SponsorInstitutionId
		,p.HighestEducationLevelId
		,p.Deceased
		,p.DoNotInviteToDirectory
		,p.ExpertiseFreeText
		,p.ReleaseDetails
		,p.EnteredDate
		,pn.TitleId
		,tt.Title		
		,p.NameOnAccreditationProduct
		,p.DoNotSendCorrespondence
		,p.ScanRequired
		,p.IsEportalActive
		,p.WebAccountCreateDate
		,p.AllowVerifyOnline
		,p.ShowPhotoOnline
		,p.RevalidationScheme
		,p.ExaminerSecurityCode
		,p.PractitionerNumber
		,e.NaatiNumber
		,e.GstApplies
		,e.UseEmail
		,e.Abn
		,e.EntityTypeId
		,p.AllowAutoRecertification
		,CASE
			WHEN pi.Photo IS NOT NULL THEN Cast(1 AS BIT)
			ELSE Cast(0 AS BIT)
		END AS HasPhoto
		,pi.PhotoDate
		,e.AccountNumber
		,p.ExaminerTrackingCategory
		,p.EthicalCompetency
		,p.InterculturalCompetency
		,p.KnowledgeTest
		,p.MfaCode
		,p.MfaExpireStartDate
		,p.EmailCodeExpireStartDate
		,p.AccessDisabledByNcms
		,lcp.EndDate as MaxCertificationPeriodEndDate
		,(Select count(1) from tblCredential c inner join tblCertificationPeriod cp on cp.CertificationPeriodId = c.CertificationPeriodId where cp.PersonId = p.PersonId) PersonCredentials
		,(Select count(1) from tblCredential c inner join tblCertificationPeriod cp on cp.CertificationPeriodId = c.CertificationPeriodId where ( C.TerminationDate <=GETDATE() OR cp.EndDate <= GETDATE()) AND cp.PersonId = p.PersonId) ExpiredCredentials
		,(Select count(1) from tblCredential c inner join tblCertificationPeriod cp on cp.CertificationPeriodId = c.CertificationPeriodId where c.StartDate <= GETDATE() AND (c.TerminationDate IS NULL OR C.TerminationDate > GETDATE()) AND cp.EndDate>GETDATE() AND cp.PersonId = p.PersonId) ActiveCredentials
		,(SELECT COUNT(1) FROM tblCredentialApplication ca where ca.PersonId = p.PersonId AND ca.CredentialApplicationStatusTypeId NOT IN (1, 4, 6, 7)) ActiveApplications
		,(Select count(1) from tblCredential c inner join tblCertificationPeriod cp on cp.CertificationPeriodId = c.CertificationPeriodId where  c.StartDate > GETDATE() AND (c.TerminationDate IS NULL OR C.TerminationDate > GETDATE()) AND cp.EndDate>GETDATE() AND cp.PersonId = p.PersonId) FutureCredentials
		,(select count(1) from tblPanelMembership panelms inner join tblPanel panel on panel.PanelId = panelms.PanelId INNER JOIN tblPanelRole msrole ON  panelms.panelRoleId = msrole.PanelRoleId where  panelms.PersonId = p.PersonId AND GETDATE() >= panelms.StartDate and panelms.EndDate >= GETDATE() and  msrole.PanelRoleCategoryId = 1) ActiveExaminerMemberships
		,(select count(1) from tblPanelMembership panelms inner join tblPanel panel on panel.PanelId = panelms.PanelId INNER JOIN tblPanelRole msrole ON  panelms.panelRoleId = msrole.PanelRoleId where  panelms.PersonId = p.PersonId AND GETDATE() >= panelms.StartDate and panelms.EndDate >= GETDATE() and  msrole.PanelRoleCategoryId = 2) ActiveRolePlayerMemberships
		,(select STUFF((select ', ' + panel.name from tblPanelMembership panelms inner join tblPanel panel on panel.PanelId = panelms.PanelId INNER JOIN tblPanelRole msRole ON msRole.PanelRoleId = panelms.panelRoleId
		where  panelms.PersonId = p.PersonId AND GETDATE() >= panelms.StartDate and panelms.EndDate >= GETDATE() and msrole.PanelRoleCategoryId = 1 FOR XML PATH('')),1,1,'')) PanelName
		,(select STUFF((select ', ' + panel.name from tblPanelMembership panelms inner join tblPanel panel on panel.PanelId = panelms.PanelId INNER JOIN tblPanelRole msRole ON msRole.PanelRoleId = panelms.panelRoleId
		where  panelms.PersonId = p.PersonId AND GETDATE() >= panelms.StartDate and panelms.EndDate >= GETDATE() and msrole.PanelRoleCategoryId = 2 FOR XML PATH('')),1,1,'')) RolePlayerPanelName
	FROM tblPerson p
	JOIN tblEntity e ON e.EntityId = p.EntityId
	LEFT JOIN tblPersonImage pi ON p.PersonId = pi.PersonId
	LEFT JOIN tblPersonName pn ON p.PersonId = pn.PersonId
	LEFT JOIN tluTitle tt on pn.TitleId = tt.TitleId
	LEFT JOIN tblCountry c on p.BirthCountryId = c.CountryId
	LEFT JOIN
	(
		SELECT
			Max(cp.EndDate) AS EndDate
			,cp.PersonId
		FROM tblCertificationPeriod cp
		WHERE cp.PersonId = @personId
		GROUP BY cp.PersonId
	) lcp ON lcp.PersonId = p.PersonId
	WHERE p.EntityId = @EntityId
	ORDER BY pn.EffectiveDate DESC, pn.PersonNameId DESC
) A