using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PersonDetailsBasicDto
    {
        public int EntityId { get; set; }
        public int PersonId { get; set; }
        public string PractitionerNumber { get; set; }
        public int NaatiNumber { get; set; }
        public string Title { get; set; }
        public int? TitleId { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCode { get; set; }
        public string Suburb { get; set; }
        public bool? IsOverseas { get; set; }
        public string CountryOfBirth { get; set; }
        public int? CountryOfBirthId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public bool HasPhoto { get; set; }
        public bool InterculturalCompetency { get; set; }
        public bool KnowledgeTest { get; set; }
        public bool EthicalCompetency { get; set; }
        public bool IsMyNaatiRegistered { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
        public bool Deceased { get; set; }
        public int EntityTypeId { get; set; }
        public bool AllowAutoRecertification { get; set; }

        public bool HasAddressInAustralia { get; set; }


    }
}