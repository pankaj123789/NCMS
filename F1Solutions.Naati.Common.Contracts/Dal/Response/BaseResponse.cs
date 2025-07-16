using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class BaseResponse{
     
        public string Message { get; set; }
        public ApiPublicErrorCode ErrorCode { get; set; }

    }


    public class ApiCallData <T, R> 
    {
        public string EndpointName { get; set; }
        public T  ParsedRequest { get; set; }
        public R Response { get; set; }
    }

    public enum ApiPublicErrorCode
    {
        None = 0,
        GenericError = 1,
        ErrorParsingRequest = 2,
        InvalidFilter = 3,
        InvalidFilterValue = 4,
        InvalidParameterValue = 5,

        InvalidPractitionerParameterValue = 6,
        PractitionerNotExist = 7,
        PractitionerNotAllowVerifyOnline = 8,

        InvalidPhotoPropertyType = 9,
        InvalidPhotoPropertyTypeValue = 10,
        PersonNotExist = 11,
        PersonPhotoNotExist = 12,
        PersonPhotoNotAvailable = 13
    }

    public class ApiPublicException : Exception
    {
       public  ApiPublicErrorCode ApiPublicErrorCode { get; set; }
       public string ApiPublicErroMessage { get; set; }
    }
}