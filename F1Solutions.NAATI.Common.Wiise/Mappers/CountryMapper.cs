using System;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    //use until the dust settles and we know what we are doing
    internal static class CountryMapper
    {
        internal static string MapToCountryCode(this string country)
        {
            switch(country)
            {
                case "Australia":
                    return "AU";
                default:
                    throw new Exception($"No country code exists for {country}");
            }
        }
    }
}
