using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.Common.PDListing
{
    
    public class PractitionerDirectoryUpdateListingRequest
    {
        [MessageHeader(Name=WcfHeaderKeys.HEADER_NAME, Namespace=WcfHeaderKeys.NAMESPACE)]
        public NaatiWebUser NaatiUser { get; set; }

        
        public AddressListingUpdate[] AddressListings { get; set; }

        
        public ContactDetailsUpdate[] PhoneDetails { get; set; }

        
        public ContactDetailsUpdate[] EmailDetails { get; set; }

        
        public ContactDetailsUpdate WebsiteDetails { get; set; }

        
        public ExpertiseItem[] WorkAreas { get; set; }

        
        public CredentialUpdate[] Credentials { get; set; }

        
        public bool IsSelectionChangeOnly { get; set; }

        
        public int NaatiNumber { get; set; }
    }

    
    public class AddressListingUpdate
    {
        
        public int AddressId { get; set; }
        
        public bool InDirectory { get; set; }
        
        public int OdAddressVisibilityTypeId { get; set; }

        
        public string OdAddressVisibilityTypeName { get; set; }
    }

    
    public class ContactDetailsUpdate
    {
        
        public int Id { get; set; }

        
        public bool InDirectory { get; set; }
    }

    
    public class CredentialUpdate
    {
        
        public int Id { get; set; }

        
        public bool InDirectory { get; set; }
    }

    
    public class ExpertiseItem
    {
        
        public int Id { get; set; }

        
        public string Expertise { get; set; }

        
        public bool IncludeInPractitionerDirectory { get; set; }
    }
}