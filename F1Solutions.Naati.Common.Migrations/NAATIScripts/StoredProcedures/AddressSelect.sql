ALTER PROCEDURE [dbo].[AddressSelect]
(
	@EntityId int
)
AS 
declare @true bit = 1
declare @false bit = 0

SELECT	a.AddressId,
		a.EntityId,
		a.StreetDetails,
		s.Suburb + ', ' + pc.Postcode + ', ' + st.State AS Suburb,
		s.Suburb as SuburbName,
		st.State as StateAbbreviation,
		pc.Postcode,
		c.Name AS CountryName,
		a.PostcodeId,
		a.CountryId,
		a.Note,
		a.StartDate,
		a.EndDate, 
		a.PrimaryContact,
		a.ODAddressVisibilityTypeId,
		odadtype.DisplayName,
		a.SubscriptionExpiryDate,
		a.Invalid,
		a.SubscriptionRenewSentDate,		
		a.ContactPerson,
		(
			SELECT		top 1 u.FullName
			FROM		tblAuditLog al
			INNER JOIN	tblUser u on al.UserId = u.UserId
			WHERE		al.RecordId = a.AddressId
			ORDER BY	al.DateTime desc
		) AS ModifiedBy,
		a.ValidateInExternalTool,
		a.ExaminerCorrespondence,
		CASE WHEN i.InstitutionId IS NULL THEN @false ELSE  @true END AS IsOrganisation

FROM		tblAddress a
LEFT JOIN	tblPostcode pc ON a.PostcodeId = pc.PostcodeId
LEFT JOIN	tblSuburb s ON pc.SuburbId = s.SuburbId
LEFT JOIN	tluState st ON s.StateId = st.StateId
LEFT JOIN	tblODAddressVisibilityType odadtype on a.ODAddressVisibilityTypeId = odadtype.ODAddressVisibilityTypeId
LEFT JOIN tblInstitution i on a.EntityId = i.EntityId
INNER JOIN	tblCountry c ON a.CountryId = c.CountryId
WHERE		a.EntityId = @EntityId and (a.PrimaryContact = 1 or a.Invalid = 0)