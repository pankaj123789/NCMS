using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using MyNaati.Contracts.BackOffice;

namespace MyNaati.Ui.Swashbuckle.Models
{
    public interface IParameterExampleProvider
    {
        object GetExample();
    }


    /// <summary>
    /// List of all request example
    /// </summary>
    public class HMacParameterExampleProvider : IParameterExampleProvider
    {
        public object GetExample()
        {
            return "Enter Valid HMAC Token";
        }
    }
    public class PractitionerCountRequestParameterExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new PractitionerCountRequest
            {
                Filters = new ApiPublicSearchFilter[]
                {
                    new ApiPublicSearchFilter
                    {
                        FilterId = 1,
                        Values = new[] {"13"}
                    }
                }
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            requestUri = json;

            return requestUri;
        }
    }

    public class ApiPublicPractitionerSearchRequestParameterExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new ApiPublicPractitionerSearchRequest
            {
                Filters = new ApiPublicSearchFilter[]
                {
                    new ApiPublicSearchFilter
                    {
                        FilterId = 1,
                        Values = new[] {"13"}
                    }
                },
                Skip = 0,
                SortingOptions = new List<ApiPublicSortingOption>
                {
                    new ApiPublicSortingOption
                    {
                        SortTypeId = ApiPublicSortType.None,
                        SortDirectionId = ApiPublicSortDirection.None
                    }
                },
                Take = 20,
                RandomSeed = 123456
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class LookupRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new LookupRequest
            {
                LookupId = 3
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class LanguagesRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new LanguagesRequest
            {
                CredentialTypeIds = new []{ 5, 19, 25, 6, 20, 26, 7, 21, 27 }
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class GetLegacyAccreditionsRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new GetLegacyAccreditionsRequest
            {
                PersonId = 180974
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class ApiTestSessionSearchRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new ApiTestSessionSearchRequest
            {
                Filters = new ApiTestSessionSearchFilter[]
                {
                    new ApiTestSessionSearchFilter
                    {
                        FilterId = 1,
                        Values = new[] { "1", "2", "3" }
                    }
                },
                Skip = 0,
                Take = 20
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class ApiEndorseQualificationSearchRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new ApiEndorseQualificationSearchRequest
            {
                Filters = new ApiPublicSearchFilter[]
                {
                    new ApiPublicSearchFilter
                    {
                        FilterId = 1,
                        Values = new[] { "22185" }
                    }
                },
                Skip = 0,
                Take = 20
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class ApiTestSessionAvailabilityRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new ApiTestSessionAvailabilityRequest
            {
                SkillId = 382,
                CredentialTypeId = 14,
                PreferredTestLocationId = 1,
                IncludeBacklog = true,
                // FromTestDate = DateTime.Now.ToString("dd-MM-yyyy"),
               // ToTestDate = DateTime.Now.AddDays(30).ToString("dd-MM-yyyy")
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class GetCertificationsRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new GetCertificationsRequest
            {
                PractitionerId = "CPN6PD01C"
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

    public class ApiPublicPersonPhotoRequestExample : IParameterExampleProvider
    {
        public object GetExample()
        {
            var request = new ApiPublicPersonPhotoRequest
            {
                PropertyType = 2,
                Value = "222"
            };
            var json = new JavaScriptSerializer().Serialize(request);
            var requestUri = HttpUtility.UrlEncode(json);
            return requestUri;
        }
    }

}