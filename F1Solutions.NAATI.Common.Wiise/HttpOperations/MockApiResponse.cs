using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace F1Solutions.Naati.Common.Wiise.HttpOperations
{
    public class MockApiResponse<T> : IRestResponse<T>
    {
        public MockApiResponse() { }

        public T Data { get; set; }
        public IRestRequest Request { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string ContentEncoding { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }


        public string StatusDescription { get; set; }
        public byte[] RawBytes { get; set; }
        public Uri ResponseUri { get; set; }
        public string Server { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
        public Version ProtocolVersion { get; set; }

        bool IRestResponse.IsSuccessful => true;

        IList<RestResponseCookie> IRestResponse.Cookies => new List<RestResponseCookie>();

        IList<Parameter> IRestResponse.Headers => new List<Parameter>();
    }
}
