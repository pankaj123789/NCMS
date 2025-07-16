namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetAddressesResponse
    {
        
        public PersonalAddress[] Addresses { get; set; }
    }

    
    public class PersonalAddress
    {
        
        public int AddressId { get; set; }

        
        public string Suburb { get; set; }

        
        public string State { get; set; }

        
        public string Country { get; set; }

        
        public string StreetDetails { get; set; }

        
        public string AddressType { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public int OdAddressVisibilityTypeId { get; set; }

        
        public string OdAddressVisibilityTypeName { get; set; }

        
        public bool IsFromAustralia { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }
    }
}