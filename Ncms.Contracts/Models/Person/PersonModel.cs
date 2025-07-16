using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Application;
using Newtonsoft.Json;

namespace Ncms.Contracts.Models.Person
{
    public class PersonModel
    {
        public int PersonId { get; set; }
        public int EntityId { get; set; }
        public int? NaatiNumber { get; set; }
        public string NaatiNumberDisplay { get; set; }
        public string PractitionerNumber { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public int? PostcodeId { get; set; }
        public int? StateId { get; set; }
        public string Email { get; set; }
        public int? EmailId { get; set; }
        public string Number { get; set; }
        public bool GstApplies { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
        public string PersonAddress { get; set; }
        public int? EntityTypeId { get; set; }
        public string StateName { get; set; }
        public bool? Deceased { get; set; }
        public bool HasPhoto { get; set; }
        public DateTime? PhotoDate { get; set; }
        public string Gender { get; set; }
        public int? BirthCountryId { get; set; }
        public string BirthCountry { get; set; }
        public bool ReleaseDetails { get; set; }
        public bool RevalidationScheme { get; set; }
        public bool ShowPhotoOnline { get; set; }
        public bool EthicalCompetency { get; set; }
        public bool InterculturalCompetency { get; set; }
        public bool KnowledgeTest { get; set; }
        public bool UseEmail { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Abn { get; set; }
        public bool AllowVerifyOnline { get; set; }
        public bool DoNotInviteToDirectory { get; set; }
        public bool IsEportalActive { get; set; }
        public DateTime? WebAccountCreateDate { get; set; }
        public ContactDetailsModel ContactDetails { get; set; }
        public IEnumerable<DateTime> ArchiveHistory { get; set; }
        public IEnumerable<PersonNameModel> NameHistory { get; set; }
        public string AccountNumber { get; set; }
        public string ExaminerTrackingCategory { get; set; }
        public string SecondaryEmail { get; set; }
        public string SecondaryAddress { get; set; }
        public string SecondaryContactNumber { get; set; }
        public string InOdEmails { get; set; }
        public string InOdPhones { get; set; }
        public string InOdAddresses { get; set; }
        public string OtherNames { get; set; }
        public bool IsFormerPractitioner { get; set; }
        public bool IsPractitioner { get; set; }
        public bool IsExaminer { get; set; }
        public bool IsRolePlayer { get; set; }
        public bool IsRolePlayerAvailable { get; set; }
        public bool IsFuturePractitioner { get; set; }
        public bool IsApplicant { get; set; }  
        public DateTime? MaxCertificationPeriodEndDate { get; set; }

        public IEnumerable<CredentialDetailsModel> Credentials { get; set; }
        public string PanelName { get; set; }
        public string RolePlayerPanelName { get; set; }

        public int [] RolePlayLocations { get; set; }
        public int MaximumRolePlaySessions { get; set; }
        public decimal Rating { get; set; }
        public bool Senior { get; set; }
        public bool AllowAutoRecertification { get; set; }
        public bool ShowAllowAutoRecertification { get; set; }
        public bool MfaModeIsSet { get; set; }
        public DateTime? MfaExpireStartDate { get; set; }
        public DateTime? EmailCodeExpireStartDate { get; set; }
        public bool AccessDisabledByNcms { get; set; }


    }

    public class RolePlayerSettingsModel
    {
        public int[] RolePlayLocations { get; set; }
        public int MaximumRolePlaySessions { get; set; }
        public decimal Rating { get; set; }
        public bool Senior { get; set; }
        public int NaatiNumber { get; set; }
    }

    public enum PersonTypeModel : int
    {
        Undefined = 0,
        Applicant = 1,
        Practitioner = 2,
        FormerPractitioner = 3,
    }

    public class PersonBasicModel
    {
        public int EntityId { get; set; }
        public int PersonId { get; set; }
        public int NaatiNumber { get; set; }
        public string PractitionerNumber { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string CountryOfBirth { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public bool EthicalCompetency { get; set; }
        public bool InterculturalCompetency { get; set; }
        public bool KnowledgeTest { get; set; }
        public bool AllowAutoRecertification { get; set; }

        public bool HasAddressInAustralia { get; set; }

    }

    public class PersonResultModel
    {
        public int Id { get; set; }
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string PersonTypes { get; set; }
        public string PractitionerNumber { get; set; }
    }

    public class CreateCertificationPeriodModel
    {
        public int PersonId { get; set; }
        public int CredentialApplicationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime OriginalEndDate { get; set; }
    }

    public class CertificationPeriodModel
    {
        public int Id { get; set; }
		public int NaatiNumber { get; set; }
		public string Notes { get; set; }
		public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime OriginalEndDate { get; set; }
		public CertificationPeriodStatus CertificationPeriodStatus { get; set; }
		public CertificationPeriodRecertificationStatus RecertificationStatus { get; set; }
        public List<CredentialModel> Credentials { get; set; }

        public int CredentialApplicationId { get; set; }
    }

	public class CheckCertificationPeriodResult
	{
		public IEnumerable<string> Errors { get; set; }
		public IEnumerable<string> Warnings { get; set; }
	}

    public class PersonCredentialRequestModel
    {
        public int PersonId { get; set; }
        public string CredentialType { get; set; }
        public string Language { get; set; }
        public string Direction { get; set; }
        public string CredentialStatus { get; set; }
    }

    public class PersonCheckModel
    {
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string FamilyName { get; set; }
        public string PrimaryEmail { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int CountryOfBirthId { get; set; }
        public string CountryOfBirth { get; set; }
        public string Gender { get; set; }
    }

    public class GetCertificationPeriodsRequestModel
    {
        public int? PersonId { get; set; }
        public int? NaatiNumber { get; set; }
        public IEnumerable<CertificationPeriodStatusModel> CertificationPeriodStatus { get; set; }
    }

    public enum CertificationPeriodStatusModel
    {
        Expired = 1,
        Current = 2,
        Future = 3
    }

    public class EntitySearchResultModel
    {
        public int? InstitutionId { get; set; }
        public int? PersonId { get; set; }
        public int EntityId { get; set; }
        public int? NaatiNumber { get; set; }
        public string NaatiNumberDisplay { get; set; }
        public string Name { get; set; }
        public int? EntityTypeId { get; set; }
        public string PrimaryEmail { get; set; }
        public bool MfaSecretSet { get; set; }
        public DateTime? MfaExpiryStartDate { get; set; }
    }

    public class MyNaatiUserDetailsModel
    {
        public string Username { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
    }

    public class AddressModel
    {
        public int? AddressId { get; set; }
        public int EntityId { get; set; }
        public string StreetDetails { get; set; }
        public string Suburb { get; set; }
        public string SuburbName { get; set; }
        public string StateAbbreviation { get; set; }
        public int? SuburbId { get; set; }
        public string Postcode { get; set; }
        public int? PostcodeId { get; set; }
        public int? CountryId { get; set; }
        public string CountryName { get; set; }
        public string Note { get; set; }
        public bool PrimaryContact { get; set; }
        public bool ValidateInExternalTool { get; set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool IsExaminer { get; set; }
        public int OdAddressVisibilityTypeId { get; set; }
        public string OdAddressVisibilityTypeName { get; set; }
        public bool IsOrganisation { get; set; }
    }

    public class PersonNameModel
    {
        public int PersonNameId { get; set; }
        public int? TitleId { get; set; }
        public string GivenName { get; set; }
        public string AlternativeGivenName { get; set; }
        public string OtherNames { get; set; }
        public string Surname { get; set; }
        public string AlternativeSurname { get; set; }
        public string Title { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    public class UpdatePhotoRequestModel
    {
        public int NaatiNumber { get; set; }
        public string FilePath { get; set; }
    }

    public class GetImageRequestModel
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
    }

    public class SuburbModel
    {
        [JsonIgnore]
        public string Suburb { get; set; }
        [JsonIgnore]
        public string State { get; set; }
        [JsonProperty(PropertyName = "P")]
        public int PostcodeId { get; set; }
        [JsonIgnore]
        public string Postcode { get; set; }
        [JsonProperty(PropertyName = "N")]
        public string Name => $"{Suburb}, {State}, {Postcode}";
    }

    public class PhoneModel
    {
        public int EntityId { get; set; }
        public int? PhoneId { get; set; }
        public string LocalNumber { get; set; }
        public string Note { get; set; }
        public bool PrimaryContact { get; set; }
        public bool IncludeInPd { get; set; }
        public bool AllowSmsNotification { get; set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool IsExaminer { get; set; }
    }

    public class EmailModel
    {
        public int EntityId { get; set; }
        public int? EmailId { get; set; }
        public bool IsPreferredEmail { get; set; }
        public bool IncludeInPd { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool ShowExaminer { get; set; }
        public bool IsExaminer { get; set; }
    }

    public class WebsiteModel
    {
        public int EntityId { get; set; }
        public string Url { get; set; }
        public bool IncludeInPd { get; set; }
    }

    public class ContactDetailsModel
    {
        public IEnumerable<AddressModel> Addresses { get; set; }
        public IEnumerable<PhoneModel> Phones { get; set; }
        public IEnumerable<EmailModel> Emails { get; set; }
        public IEnumerable<WebsiteModel> Websites { get; set; }
        public bool ShowWebsite { get; set; }
        public bool IsMyNaatiRegistered { get; set; }
    }

    public class SetCertificationEndDateRequestModel
    {
        public int Id { get; set; }
        public DateTime NewEndDate { get; set; }
        public string Notes { get; set; }
    }

    public class SetCredentialTerminateDateModel
    {
        public int Id { get; set; }
        public DateTime? NewTerminationDate { get; set; }
        public string Notes { get; set; }
      
        public DateTime? CertificationPeriodEndDate { get; set; }
    }
}
