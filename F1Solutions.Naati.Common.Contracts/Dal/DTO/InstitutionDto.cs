using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class InstitutionDto : LookupItemBase
    {
        public string TradingName { get; set; }
        public bool HasAustralianAddress { get; set; }
        public string AbbreviatedName { get; set; }
        public DateTime? LatestEndDateForApprovedCourse { get; set; }
        public bool HasIndigenousLanguagesOnly { get; set; }
        public int NaatiNumber { get; set; }
        public GetContactDetailsResponse ContactDetails { get; set; }
        public int? InstitutionId { get; set; }
        public int EntityId { get; set; }
        public int EntityTypeId { get; set; }
        public string NaatiNumberDisplay { get; set; }
        public string Name { get; set; }
        public bool? TrustedPayer { get; set; }
        public IEnumerable<InstitutionNameDto> NameHistory { get; set; }
    }
}