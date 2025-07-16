using System.Collections.Generic;
using Ncms.Contracts.Models.Address;

namespace Ncms.Contracts
{
    public interface IAddressService
    {
        AddressListResponseModel List(AddressListRequestModel request);
        ParseAddressResponse ParseAddress(GeoResultModel request);
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
