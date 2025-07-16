using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdatePersonSettingsRequest
    {
        public string Abn { get; set; }
        public bool AllowVerifyOnline { get; set; }
        public bool DoNotInviteToDirectory { get; set; }
        public int EntityId { get; set; }
        public bool GstApplies { get; set; }
        public int PersonId { get; set; }
        public bool ReleaseDetails { get; set; }
        public bool AllowAutoRecertification { get; set; }
        public bool RevalidationScheme { get; set; }
        public bool ShowPhotoOnline { get; set; }
        public bool InterculturalCompetency { get; set; }
        public bool EthicalCompetency { get; set; }
        public bool KnowledgeTest { get; set; }
        public bool UseEmail { get; set; }
        public string AccountNumber { get; set; }
        public string ExaminerTrackingCategory { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
        public string Gender { get; set; }
        public int? BirthCountryId { get; set; }
        public RolePlayerSettingsRequest RolePlayerSettingsRequest { get; set; }
        public bool Deceased { get; set; }

        public bool AccessDisabledByNcms { get; set; }
    }
}