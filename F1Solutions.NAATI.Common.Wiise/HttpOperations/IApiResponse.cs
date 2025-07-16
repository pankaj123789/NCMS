using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace F1Solutions.Naati.Common.Wiise.HttpOperations
{
    public interface IApiResponse
    {
        Type ResponseType { get; }
        Object Content { get; }

        HttpStatusCode StatusCode { get; }

        Multimap<string, string> Headers { get; }

        String ErrorText { get; set; }

        List<Cookie> Cookies { get; set; }

        ResponseStatus ResponseStatus { get; set; }
    }
}
