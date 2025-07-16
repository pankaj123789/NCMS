using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ApiSearchRequest
    {
        public int? Skip { get; set; }

        public int? Take { get; set; }

        public string Filter { get; set; }
    }

    public class ApiCredentialQrCodeRequest
    {
        public string CredentialQrCode { get; set; }
    }

    public class ApiOdSearchRequest : ApiSearchRequest
    {
        public IEnumerable<SortingOption> SortingOptions { get; set; }
        public int RandomSeed { get; set; }
    }

    public class PractitionerCountRequest
    {
        public IEnumerable<ApiPublicSearchFilter> Filters { get; set; }
    }

    public class ApiPublicPractitionerSearchRequest
    {
        public IEnumerable<ApiPublicSearchFilter> Filters { get; set; }
        public IEnumerable<ApiPublicSortingOption> SortingOptions { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public int RandomSeed { get; set; }
    }

    public class ApiPublicSearchFilter
    {
        public int FilterId { get; set; }
        public IEnumerable<string> Values { get; set; }
    }

    public class ApiRequest<T>
    {
        public T request { get; set; }
    }

    public class ApiTestSessionSearchRequest
    {
        public IEnumerable<ApiTestSessionSearchFilter> Filters { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
    public class ApiTestSessionSearchFilter
    {
        public int FilterId { get; set; }
        public IEnumerable<string> Values { get; set; }
    }

    public class ApiSessionAvailabilityRequest
    {
        public int SkillId { get; set; }
        public int CredentialTypeId { get; set; }
        public int PreferredTestLocationId { get; set; }
        public bool IncludeBacklog { get; set; }
        public DateTime? FromTestDate { get; set; }
        public DateTime? ToTestDate { get; set; }
    }

    public class ApiPublicPersonPhotoRequest
    {
        public int PropertyType { get; set; }
        public string Value { get; set; }
    }

    public class GetPublicPersonPhotoRequest
    {
        public ApiPublicPhotoRequestPropertyType PropertyType { get; set; }
        public string Value { get; set; }
    }

}