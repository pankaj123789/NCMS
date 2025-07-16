using System;

namespace Ncms.Contracts.Models.Application
{
    public class ApplicationSearchResultModel
    {
        public int Id { get; set; }
        public string ApplicationReference { get; set; }
        public string ApplicationType { get; set; }
        public int ApplicationTypeId { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string ApplicationOwner { get; set; }
        public int? SponsorNaatiNumber { get; set; }
        public string SponsorName { get; set; }
        public bool? AutoCreated { get; set; }
        public bool? CredentialRequestAutoCreated { get; set; }

    }
}
