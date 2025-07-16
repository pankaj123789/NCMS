using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;

namespace F1Solutions.Naati.Common.Bl.AddressParser
{
    public class AddressParserHelper : IAddressParserHelper
    {
    
        public ParsedAddressDto ParseAddress(GeoResultDto address)
        {
            var parsedAddress = new ParsedAddressDto();
            var components = GetAddressComponents(address);
            var place = components.logNameComponents;
            var streetDetails = new List<string>();

            string streetNumber;
            if (place.TryGetValue("street_number", out streetNumber))
            {
                string subpremise;
                if (place.TryGetValue("subpremise", out subpremise))
                {
                    streetDetails.Add(subpremise + '/' + streetNumber);
                }
                else
                {
                    streetDetails.Add(streetNumber);
                }
            }

            string route;
            if (place.TryGetValue("route", out route))
            {
                streetDetails.Add(route);
            }

            string sublocality;
            place.TryGetValue("sublocality", out sublocality);
            string placeLocality;
            place.TryGetValue("locality", out placeLocality);
            string postalTown;
            place.TryGetValue("postal_town", out postalTown);
            string colloquialArea;
            place.TryGetValue("colloquial_area", out colloquialArea);
            var locality = sublocality ?? placeLocality ?? postalTown ?? colloquialArea ?? string.Empty;

            string country;
            if (!place.TryGetValue("country", out country))
            {
                throw new Exception("Country not found");
            }

            if (country != "Australia")
            {
                string partialStreetDetails;
                string administrativeAreaLevel1;
                place.TryGetValue("administrative_area_level_1", out administrativeAreaLevel1);

                if (!string.IsNullOrWhiteSpace(administrativeAreaLevel1))
                {
                    partialStreetDetails = ", " + locality + ", " + administrativeAreaLevel1;
                }
                else
                {
                    partialStreetDetails = ", " + locality;
                }

                parsedAddress.StreetDetails = string.Join(" ", streetDetails) + partialStreetDetails;

            }
            else
            {
                string state;
                components.shortNameComponents.TryGetValue("administrative_area_level_1", out state);
                parsedAddress.State = state;
                parsedAddress.StreetDetails = string.Join(" ", streetDetails);
            }

            parsedAddress.Suburb = locality;
            string postcode;
            place.TryGetValue("postal_code", out postcode);
            parsedAddress.Postcode = postcode;
            parsedAddress.Country = country;
            parsedAddress.StreetNumber = streetNumber;

            var countryNameMapper = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"United States", "United States of America"},
                {"USA", "United States of America"}
            };

            if (!string.IsNullOrEmpty(parsedAddress.Country) && countryNameMapper.ContainsKey(parsedAddress.Country))
            {
                parsedAddress.Country = countryNameMapper[parsedAddress.Country];
            }
            return parsedAddress;
        }

        private (IDictionary<string, string>  logNameComponents, IDictionary<string, string>  shortNameComponents) GetAddressComponents(GeoResultDto place)
        {
            var logNameDictionary = place.Address_Components.ToDictionary(y => y.Types.First(), x => x.Long_Name);
            var shortNameDictionary = place.Address_Components.ToDictionary(y => y.Types.First(), x => x.Short_Name);
            return (logNameDictionary , shortNameDictionary);
        }
    }
}
