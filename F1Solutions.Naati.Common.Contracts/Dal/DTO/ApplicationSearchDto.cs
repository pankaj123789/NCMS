using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ApplicationSearchDto
    {
        public int Id { get; set; }
        public string ApplicationReference { get; set; }
        public string ApplicationType { get; set; }
        public bool DisplayBills { get; set; }
        public int ApplicationTypeId { get; set; }
        public string ApplicationStatus { get; set; }
        public bool? AutoCreated { get; set; }
        public bool? CredentialRequestAutoCreated { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string ApplicationOwner { get; set; }
        public string SponsorName { get; set; }
        public int? SponsorNaatiNumber { get; set; }
        public DateTime EnteredDate { get; set; }
        public int? PreferredTestLocationId { get; set; }
    }
}
