namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class PersonalDetailsUpdateAddressRequest
    {
        
        public PersonalEditAddress Address { get; set; }

        
        public bool Delete { get; set; }

        
        public int NaatiNumber { get; set; }
    }

    
    public class PersonalEditAddress
    {
        
        public int AddressId { get; set; }

        
        public string StreetDetails { get; set; }

        
        public int PostcodeId { get; set; }

        
        public int CountryId { get; set; }
        
        
        public bool IsFromAustralia { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public int OdAddressVisibilityTypeId { get; set; }

        
        public string OdAddressVisibilityTypeName { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }

        
        public bool ValidateInExternalTool { get; set; }
    }
}
