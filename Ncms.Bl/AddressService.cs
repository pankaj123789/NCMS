using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Address;
using Newtonsoft.Json;

namespace Ncms.Bl
{
    public class AddressService : IAddressService
    {
        private readonly IAddressParserHelper _addressParser;
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public AddressService(IAddressParserHelper addressParser, ISecretsCacheQueryService secretsProvider, IAutoMapperHelper autoMapperHelper)
        {
            _addressParser = addressParser;
            _secretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
        }

        public AddressListResponseModel List(AddressListRequestModel request)
        {
            var googleApiKey = _secretsProvider.Get("GoogleApiKeyBackend");
            string url = string.Format("http://maps.googleapis.com/maps/api/geocode/json?address={0}&componentRestrictions:country=AU&key={1}", request.Address, googleApiKey);

            var googleMapResults = new List<AddressModel>();
            var response = new AddressListResponseModel { Addresses = googleMapResults };


            var googleRequest = (HttpWebRequest)WebRequest.Create(url);

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
                var onlyStreetAddressResults = geoResponse.Results.Where(r => (r.Types.Contains(GeoResponseTypes.StreetAddress)));

                if (!onlyStreetAddressResults.Any())
                {
                    return response;
                }

                foreach (var result in onlyStreetAddressResults)
                {
                    var googleMapResult = new AddressModel
                    {
                        Latitude = result.Geometry.Location.Lat,
                        Longitude = result.Geometry.Location.Lng
                    };

                    foreach (var component in result.Address_Components)
                    {
                        if (component.Types.Contains(GeoResponseTypes.Locality))
                        {
                            googleMapResult.Suburb = component.Long_Name;
                        }
                        else if (component.Types.Contains(GeoResponseTypes.SubPremise))
                        {
                            googleMapResult.SubPremise = component.Long_Name + "/";
                        }
                        else if (component.Types.Contains(GeoResponseTypes.StreetNumber))
                        {
                            googleMapResult.StreetNumber = component.Long_Name;
                        }
                        else if (component.Types.Contains(GeoResponseTypes.Route))
                        {
                            googleMapResult.StreetName = component.Long_Name;
                        }
                        else if (component.Types.Any(s => s.StartsWith(GeoResponseTypes.AdministrativeArea)))
                        {
                            googleMapResult.State = component.Short_Name;
                        }
                        else if (component.Types.Contains(GeoResponseTypes.PostalCode))
                        {
                            googleMapResult.PostCode = component.Long_Name;
                        }
                        else if (component.Types.Contains(GeoResponseTypes.Country))
                        {
                            googleMapResult.Country = component.Long_Name;
                        }
                    }

                    googleMapResults.Add(googleMapResult);
                }
            }

            return response;
        }

        public ParseAddressResponse ParseAddress(GeoResultModel request)
        {
            var address = _autoMapperHelper.Mapper.Map<GeoResultDto>(request);
            var result = _addressParser.ParseAddress(address);
            var response = _autoMapperHelper.Mapper.Map<ParseAddressResponse>(result);
            return response;
        }
    }

    public class GeoResponse
    {
        public string Status { get; set; }
        public GeoResultModel[] Results { get; set; }
    }
   
    public static class GeoResponseTypes
    {
        public static string Locality
        {
            get { return "locality"; }
        }

        public static string StreetAddress
        {
            get { return "street_address"; }
        }

        public static string SubPremise
        {
            get { return "subpremise"; }
        }

        public static string Route
        {
            get { return "route"; }
        }

        public static string AdministrativeArea
        {
            get { return "administrative_area"; }
        }

        public static string PostalCode
        {
            get { return "postal_code"; }
        }

        public static string Country
        {
            get { return "country"; }
        }

        public static string StreetNumber
        {
            get { return "street_number"; }
        }
    }
}
