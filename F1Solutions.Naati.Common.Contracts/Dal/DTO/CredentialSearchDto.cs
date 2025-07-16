using System;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialSearchDto 
    {
        public int CredentialId { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public int NaatiNumber { get; set; }
        public string Category { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string Direction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime IssuedDate { get; set; }
        public int StatusId { get; set; }
        public bool ShowInOnlineDirectory { get; set; }
        public string PractitionerNumber { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string PrimaryEmail { get; set; }
        public string ApplicationType { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string Language1 { get; set; }
        public string Language1Code { get; set; }
        public string Language1Group { get; set; }
        public string Language2 { get; set; }
        public string Language2Code { get; set; }
        public string Language2Group { get; set; }
        public string Status { get; set; }
    }
}
