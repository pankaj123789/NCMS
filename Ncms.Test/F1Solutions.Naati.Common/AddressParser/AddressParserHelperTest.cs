using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using Ncms.Bl;
using Ncms.Contracts;
using Newtonsoft.Json;
using Xunit;

namespace Ncms.Test.F1Solutions.Naati.Common.AddressParser
{

    public class AddressParserHelperTest
    {

        [Theory(Skip = "Integration test. Calls google api to get the address")]
        [MemberData(nameof(CreateAddressTestData))]
        public void ParseAddress_WhenCalled_ThenResultIsProperlyParsed(int testId, string address, ParsedAddressDto expectedParsedAddress)
        {
            var googleAddress = GetGoogleAddress(address);

            var addressHelper = new AddressParserHelper();

            var mappedAddress = new GeoResultDto()
            {
                Address_Components = googleAddress.Address_Components.Select(
                        x => new GeoComponentDto()
                        {
                            Long_Name = x.Long_Name,
                            Short_Name = x.Short_Name,
                            Types = x.Types
                        }).ToList(),
                Formatted_Address = googleAddress.Formatted_Address,
                Geometry = new GeoGeometryDto()
                {
                    Location =
                        new GeoLocationDto()
                        {
                            Lat = googleAddress.Geometry.Location.Lat,
                            Lng = googleAddress.Geometry.Location.Lng
                        }
                }
            };

            var result = addressHelper.ParseAddress(mappedAddress);

            Assert.True(result != null, $"null object on TestId :{testId}");
            Assert.True(result.Country == expectedParsedAddress.Country, $"{nameof(expectedParsedAddress.Country)} on TestId :{testId}");
            Assert.True(result.StreetDetails == expectedParsedAddress.StreetDetails, $"{nameof(expectedParsedAddress.StreetDetails)} on TestId :{testId}");
            Assert.True(result.Postcode == expectedParsedAddress.Postcode, $"{nameof(expectedParsedAddress.Postcode)} on TestId :{testId}");
            Assert.True(result.State == expectedParsedAddress.State, $"{nameof(expectedParsedAddress.State)} on TestId :{testId}");
            Assert.True(result.StreetNumber == expectedParsedAddress.StreetNumber, $"{nameof(expectedParsedAddress.StreetNumber)} on TestId :{testId}");
            Assert.True(result.Suburb == expectedParsedAddress.Suburb, $"{nameof(expectedParsedAddress.Suburb)} on TestId :{testId}");

        }

        public GeoResultModel GetGoogleAddress(string address)
        {
            var googleApiKey = "AIzaSyAJc5073jvLjNocIp7vKspioObmLSLmdNs";
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&componentRestrictions:country=AU&key={googleApiKey}";

            var googleRequest = (HttpWebRequest)WebRequest.Create(url);
            LoggingHelper.LogInfo($"Google API URL: {url}");
            googleRequest.UseDefaultCredentials = true;
            googleRequest.Method = "GET";
            var googleResponse = googleRequest.GetResponse();

            string streamReaderReturn = null;

            using (var stream = googleResponse.GetResponseStream())
            {
                if (stream != null)
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        streamReaderReturn = streamReader.ReadToEnd();
                    }
                }
            }

            var geoResponse = JsonConvert.DeserializeObject<GeoResponse>(streamReaderReturn);

            if (geoResponse.Status == "OK")
            {
                return geoResponse.Results?.FirstOrDefault();

            }
            return null;
        }

        public static IEnumerable<object[]> CreateAddressTestData()
        {
            return new[]
            {
                new object[]
                {
                    1,
                    "103 Northbourne Avenue, Turner ACT, Australia",
                    new ParsedAddressDto()
                    {
                        Country = "Australia",
                        Postcode = "2612",
                        State = "ACT",
                        StreetDetails = "103 Northbourne Avenue",
                        Suburb = "Turner",
                        StreetNumber = "103"
                    }
                },
                new object[]
                {
                    2,
                    "36 Mayfield Avenue Lancaster UK",
                    new ParsedAddressDto()
                    {
                        Country = "United Kingdom",
                        Postcode = "LA1 2PF",
                        State = null,
                        StreetDetails = "36 Mayfield Avenue, Lancaster, England",
                        Suburb = "Lancaster",
                        StreetNumber = "36"
                    }
                },
                new object[]
                {
                    3,
                    "1 Miromiro Street, Greenhithe, Auckland, New Zealand",
                    new ParsedAddressDto()
                    {
                        Country = "New Zealand",
                        Postcode = "0632",
                        State = null,
                        StreetDetails = "1 Miromiro Street, Auckland, Auckland",
                        Suburb = "Auckland",
                        StreetNumber = "1"
                    }
                },
                new object[]
                {
                    4,
                    "5435 North Lincoln Avenue, Chicago, IL, USA",
                    new ParsedAddressDto()
                    {
                        Country = "United States of America",
                        Postcode = "60625",
                        State = null,
                        StreetDetails = "5435 North Lincoln Avenue, Chicago, Illinois",
                        Suburb = "Chicago",
                        StreetNumber = "5435"
                    }
                },
                new object[]
                {
                    5,
                    "1/19 Sixth Avenue, Saint Peters, South Australia, Australia",
                    new ParsedAddressDto
                    {
                        Country = "Australia",
                        Postcode = "5069",
                        State = "SA",
                        StreetDetails = "Unit 1/19 Sixth Avenue",
                        Suburb = "Saint Peters",
                        StreetNumber = "19"
                    }
                }
            };
        }
    }
}
