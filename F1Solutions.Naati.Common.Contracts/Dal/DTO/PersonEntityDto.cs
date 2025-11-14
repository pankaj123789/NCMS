using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{  
    // DTO used to return data from the PersonSelect SP which queries tblPerson and tlbEntity
    public class PersonEntityDto
    {
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string AlternativeGivenName { get; set; }
        public string OtherNames { get; set; }
        public string Surname { get; set; }
        public string AlternativeSurname { get; set; }
        public int? SponsorInstitutionId { get; set; }
        public int? HighestEducationLevelId { get; set; }
        public int? TitleId { get; set; }
        public string ExpertiseFreeText { get; set; }
        public string NameOnAccreditationProduct { get; set; }
        public string ExaminerSecurityCode { get; set; }
        public string Abn { get; set; }
        public bool AllowVerifyOnline { get; set; }
        public bool Citizen { get; set; }
        public int? BirthCountryId { get; set; }
        public string BirthCountry { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? EnteredDate { get; set; }
        public DateTime? WebAccountCreateDate { get; set; }
        public bool Deceased { get; set; }
        public bool DoNotInviteToDirectory { get; set; }
        public bool ScanRequired { get; set; }
        public bool IsEportalActive { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
        public int EntityId { get; set; }
        public int? EntityTypeId { get; set; }
        public string Gender { get; set; }
        public bool GstApplies { get; set; }
        public bool HasPhoto { get; set; }
        public DateTime? PhotoDate { get; set; }
        public int? NaatiNumber { get; set; }
        public string NaatiNumberDisplay { get; set; }
        public string PractitionerNumber { get; set; }
        public string Name { get; set; }
        public int PersonId { get; set; }
        public bool ReleaseDetails { get; set; }
        public bool RevalidationScheme { get; set; }
        public bool ShowPhotoOnline { get; set; }
        public bool InterculturalCompetency { get; set; }
        public bool KnowledgeTest { get; set; }
        public bool EthicalCompetency { get; set; }
        public bool UseEmail { get; set; }
        public string AccountNumber { get; set; }
        public string ExaminerTrackingCategory { get; set; }
        public bool IsFormerPractitioner { get; set; }
        public bool IsPractitioner { get; set; }
        public bool IsApplicant { get; set; }
        public bool AllowAutoRecertification { get; set; }
        public bool IsExaminer { get; set; }
        public bool IsRolePlayer { get; set; }
        public bool IsRolePlayerAvailable { get; set; }
        public bool IsFuturePractitioner { get; set; }
        public DateTime? MaxCertificationPeriodEndDate { get; set; }
        public int? PostcodeId { get; set; }
        public int? StateId { get; set; }
        public string Email { get; set; }
        public int? EmailId { get; set; }
        public string Number { get; set; }
        public string PersonAddress { get; set; }
        public string StateName { get; set; }
        public string SecondaryEmail { get; set; }
        public string SecondaryAddress { get; set; }
        public string SecondaryContactNumber { get; set; }
        public string InOdEmails { get; set; }
        public string InOdPhones { get; set; }
        public string InOdAddresses { get; set; }
        public string PanelName { get; set; }
        public string RolePlayerPanelName { get; set; }
        public string MfaCode { get; set; }
        public DateTime? MfaExpireStartDate { get; set; }
        public DateTime? EmailCodeExpireStartDate { get; set; }
        public bool AccessDisabledByNcms { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTimeOffset? DeletedOn { get; set; }
        public virtual string DeletedBy { get; set; }
    }
}
