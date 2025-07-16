using System;
using System.Collections.Generic;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetAddressResponse
    {
        
        public PersonalViewAddress Address { get; set; }

        
        public string ErrorMessage { get; set; }

        public bool WasSuccessful { get { return String.IsNullOrEmpty(ErrorMessage); } }
    }

    
    public class PersonalViewAddress
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

    public class ParseAddressResponse
    {
        public string StreetDetails { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string StreetNumber { get; set; }
    }

    public class GeoResultModel
    {
        public GeoGeometryModel Geometry { get; set; }
        public string Formatted_Address { get; set; }
        public List<string> Types { get; set; }
        public List<GeoComponentModel> Address_Components { get; set; }
    }

    public class GeoGeometryModel
    {
        public GeoLocationModel Location { get; set; }
    }

    public class GeoLocationModel
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    public class GeoComponentModel
    {
        public string Long_Name { get; set; }
        public string Short_Name { get; set; }
        public List<string> Types { get; set; }
    }
}
