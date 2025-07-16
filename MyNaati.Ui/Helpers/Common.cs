using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;
using MyNaati.Ui.Controllers;
using Newtonsoft.Json;

namespace MyNaati.Ui.Helpers
{
    public static class Common
    {
        private static string GOOGLE_API_GEOCODE = string.Concat("https://maps.googleapis.com/maps/api/geocode/", "json?address=");
        /// <summary>
        ///  Call Google Map Service to get list of matching addresses
        /// </summary>
        /// <param name="address">The address to search for</param>
        /// <returns>A list of addresses that match the address provided</returns>        
        public static List<GoogleMapResult> GetGoogleMapResults(string address)
        {
            var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();
            var apiKey = secretsProvider.Get("GoogleApiKeyBackend");
            string url = string.Format("{0}{1}&key={2}", GOOGLE_API_GEOCODE, address, apiKey);

            LoggingHelper.LogInfo($"Google GeoCoding API URL: {url}");


            var googleMapResults = new List<GoogleMapResult>();

            WebResponse response = null;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.UseDefaultCredentials = true;
                request.Method = "GET";
                response = request.GetResponse();

                string streamReaderReturn = null;

                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            streamReaderReturn = streamReader.ReadToEnd();
                        }
                    }
                }

                var geoResponse = JsonConvert.DeserializeObject<PersonalDetailsController.GeoResponse>(streamReaderReturn);

                if (geoResponse.Status.ToUpperInvariant() != "OK")
                {
                    LoggingHelper.LogError("Google GeoCoding API response: {Status} - {Message}", geoResponse.Status,
                        geoResponse.ErrorMessage);
                }
                else
                {
                    var onlyStreetAddressResults = geoResponse.Results.Where(r => (r.Types.Contains(GeoResponseTypes.StreetAddress))
                    || r.Types.Contains(GeoResponseTypes.SubPremise) || r.Types.Contains("premise") || r.Types.Contains("point_of_interest"));

                    if (!onlyStreetAddressResults.Any())
                    {
                        return googleMapResults;
                    }

                    foreach (var result in onlyStreetAddressResults)
                    {
                        var googleMapResult = new GoogleMapResult
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
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in GetGoogleMapResults()");
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return googleMapResults;
        }

    }
}