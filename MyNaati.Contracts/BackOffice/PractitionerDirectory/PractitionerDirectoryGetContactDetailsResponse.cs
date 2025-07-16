using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;

namespace MyNaati.Contracts.BackOffice.PractitionerDirectory
{
    public class PractitionerDirectoryGetContactDetailsResponse
    {
        
        public string Surname { get; set; }

        
        public string GivenName { get; set; }

        
        public string OtherNames { get; set; }

        
        public string Title { get; set; }

        
        public string StreetDetails { get; set; }

        
        public string Country { get; set; }

        
        public string Suburb { get; set; }
        
        public string State { get; set; }
        
        public string Postcode { get; set; }

        
        public int CountryId { get; set; }

        
        public int OdAddressVisibilityTypeId { get; set; }

        
        public string OdAddressVisibilityTypeName { get; set; }

        
        public IEnumerable<ContactDetail> ContactDetails { get; set; }

        
        public IEnumerable<AccreditationLegacy> LegacyAccreditations { get; set; }

        
        public string Website { get; set; }
    }
}
