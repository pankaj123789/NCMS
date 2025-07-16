using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace Ncms.Contracts.Models.Institution
{
    public class InstitutionModel
    {
        public int InstitutionId { get; set; }
        public int NaatiNumber { get; set; }
        public int EntityId { get; set; }
        public string TradingName { get; set; }
        public string AbbreviatedName { get; set; }
        public GetContactDetailsResponse ContactDetails { get; set; }
        public bool? TrustedPayer { get; set; }
        public string Name { get; set; }
        public IEnumerable<InstitutionNameDto> NameHistory { get; set; }
    }
}
