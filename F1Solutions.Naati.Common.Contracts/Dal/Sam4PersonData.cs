using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class Sam4PersonData
    {
        public int? SponsorInstitutionId { get; set; }
        public int? HighestEducationLevelId { get; set; }
        public string ExpertiseFreeText { get; set; }
        public string NameOnAccreditationProduct { get; set; }
        public string ExaminerSecurityCode { get; set; }
        public bool AllowVerifyOnline { get; set; }
        public bool Citizen { get; set; }
        public int? BirthCountryId { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? EnteredDate { get; set; }
        public DateTime? WebAccountCreateDate { get; set; }
        public bool Deceased { get; set; }
        public bool ScanRequired { get; set; }
        public bool IsEportalActive { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
        public string Gender { get; set; }
        public bool ReleaseDetails { get; set; }
        public bool ShowPhotoOnline { get; set; }
        public bool EthicalCompetency { get; set; }
        public bool InterculturalCompetency { get; set; }
        public string ExaminerTrackingCategory { get; set; }
    }
}