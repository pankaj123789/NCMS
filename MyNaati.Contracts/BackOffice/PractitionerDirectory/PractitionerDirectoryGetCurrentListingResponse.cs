using System;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common.PDListing;

namespace MyNaati.Contracts.BackOffice.PractitionerDirectory
{
    
    public class PractitionerDirectoryGetCurrentListingResponse
    {
        
        public AddressListing[] AddressListings { get; set; }

        
        public ContactDetailListing[] PhoneListings { get; set; }

        
        public ContactDetailListing[] EmailListings { get; set; }
        
        
        public ContactDetailListing WebsiteListing { get; set; }

        
        public ExpertiseItem[] WorkAreas { get; set; }

        
        public CredentialListing[] Credentials { get; set; }

        
        public DateTime? ExpiryDate { get; set; }

        
        public bool ListedForCurrentFinancialYear { get; set; }

        
        public bool ListedForNextFinancialYear { get; set; }
    }

    
    public class AddressListing
    {
        
        public int AddressId { get; set; }

        
        public string FullAddress { get; set;}

        
        public string Type { get; set; }

        
        public int OdAddressVisibilityTypeId { get; set; }

        
        public string OdAddressVisibilityTypeName { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public string StreetAddress { get; set; }

        
        public string Suburb { get; set; }

        
        public string Country { get; set; }

        
        public int CountryId { get; set; }

        
        public bool InDirectory { get; set; }

        public string Location { get; set; }
    }

    
    public class ContactDetailListing
    {
        
        public int ContactDetailsId { get; set; }

        
        public string Contact { get; set; }

        
        public string Type { get; set; }

        
        public bool IncludeInPractitionerDirectory { get; set; }
    }

    
    public class CredentialListing
    {
        
        public int Id { get; set; }

        
        public string Skill { get; set; }

        
        public string Level { get; set; }

        
        public string Direction { get; set; }

        
        public DateTime? Expiry { get; set; }

        
        public bool IncludeInPractitionerDirectory { get; set; }

        
        public bool IsIndigenous { get; set; }
    }    
}