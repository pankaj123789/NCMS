using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Bl.AddressParser
{
    public interface IAddressParserHelper
    {
        ParsedAddressDto ParseAddress(GeoResultDto address);
    }

    public class ParsedAddressDto
    {
        public string StreetDetails { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string StreetNumber { get; set; }
    }
    public class GeoResultDto
    {
        public GeoGeometryDto Geometry { get; set; }
        public string Formatted_Address { get; set; }
        public List<string> Types { get; set; }
        public List<GeoComponentDto> Address_Components { get; set; }
    }

    public class GeoGeometryDto
    {
        public GeoLocationDto Location { get; set; }
    }

    public class GeoLocationDto
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    public class GeoComponentDto
    {
        public string Long_Name { get; set; }
        public string Short_Name { get; set; }
        public List<string> Types { get; set; }
    }
}
