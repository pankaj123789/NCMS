using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationDto
    {
        public int ApplicationId { get; set; }
        public int ApplicationTypeId { get; set; }
        public int ApplicationStatusTypeId { get; set; }
        public int EnteredUserId { get; set; }
        public string EnteredUserName { get; set; }
        public int ReceivingOfficeId { get; set; }
        public int ReceivingInstitutionId { get; set; }
        public string ReceivingInstitutionName { get; set; }
        public string ReceivingInstitutionAbbreviation { get; set; }
        public int StatusChangeUserId { get; set; }
        public int? OwnedByUserId { get; set; }
        public int? SponsorInstitutionId { get; set; }
        public int? SponsorEntityId { get; set; }
        public DateTime EnteredDate { get; set; }
        public int NaatiNumber { get; set; }
        public string PractitionerNumber { get; set; }
        public string ApplicationTypeName { get; set; }
        public string ApplicationOwner { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public string StatusModifiedBy { get; set; }
        public string ApplicantGivenName { get; set; }
        public string ApplicantFamilyName { get; set; }
        public string ApplicantPrimaryEmail { get; set; }
        public bool OwnedByApplicant { get; set; }
        public string ApplicationReference { get; set; }
        public int PreferredTestLocationId { get; set; }
        public int SponsorInstitutionContactPersonId { get; set; }
        public string SponsorInstitutionContactPersonName { get; set; }
        public int SponsorInstitutionNaatiNumber { get; set; }
        public string SponsorInstitutionName { get; set; }
        public bool? TrustedInstitutionPayer { get; set; }
        public string SponsorEmail { get; set; }
        public bool ShowNonPreferredTestLocationInfo { get; set; }
        public bool ErrorNonPreferredTestLocationInfo { get; set; }
        public int CertificationPeriodId { get; set; }

        public bool HasAddressInAustralia { get; set; }
        public bool? AutoCreated { get; set; }

        public CredentialApplicationTypeCategoryName ApplicationTypeCategory { get; set; }
	}
}
