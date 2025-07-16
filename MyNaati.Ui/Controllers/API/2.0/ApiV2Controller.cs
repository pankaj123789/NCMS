using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.Common;
using MyNaati.Ui.Models;
using MyNaati.Ui.Swashbuckle.Helpers;
using MyNaati.Ui.Swashbuckle.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using EndpointPermission = MyNaati.Contracts.BackOffice.EndpointPermission;


namespace MyNaati.Ui.Controllers.API._2._0
{
    [ApiExplorerSettings(IgnoreApi = false)]
    [DisplayName("Public Api")]
    [RoutePrefix("api/2.0/public")]
    public class ApiV2Controller : BaseApiController
    {
        private readonly IApiPublicService _mApiPublicService;

        public ApiV2Controller(IApiPublicService apiPublicService)
        {
            _mApiPublicService = apiPublicService;
        }

        /// <summary>
        /// Endpoint Name: PractitionersCount, Permission Required: PractitionersCount
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.PractitionersCount)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api Endpoint PractitionersCount Call Successful", typeof(ApiPublicPractitionerCountResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPublicPractitionerCountResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPublicPractitionerCountResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(PractitionerCountRequestParameterExample), "request", "Encoded Request of Example Type")]
        [Route("PractitionersCount")]
        public JsonResult<ApiPublicPractitionerCountResponse> PractitionersCount(string request)
        {
            return ProcessRequest<PractitionerCountRequest, ApiPublicPractitionerCountResponse>(request, (param) => _mApiPublicService.PractitionersCount(param));
        }

        /// <summary>
        /// Endpoint Name: Practitioners, Permission Required: Practitioners
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.Practitioners)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api Endpoint Practitioners Call Successful", typeof(ApiPublicPractitionerSearchResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPublicPractitionerSearchResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPublicPractitionerSearchResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(ApiPublicPractitionerSearchRequestParameterExample), "request", "Encoded Request of Example Type")]
        [Route("Practitioners")]

        public JsonResult<ApiPublicPractitionerSearchResponse> Practitioners(string request)
        {
            return ProcessRequest<ApiPublicPractitionerSearchRequest, ApiPublicPractitionerSearchResponse>(request, (param) => _mApiPublicService.SearchPractitioner(param));
        }

        /// <summary>
        /// Endpoint Name: Lookups, Permission Required: Lookups
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.Lookups)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api Endpoint Lookups Call Successful", typeof(PublicLookupResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PublicLookupResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PublicLookupResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(LookupRequestExample), "request", "Encoded Request of Example Type")]
        [Route("Lookups")]
        public JsonResult<PublicLookupResponse> Lookups(string request)
        {
            return ProcessRequest<LookupRequest, PublicLookupResponse>(request, (param) => _mApiPublicService.GetLookups(param));
        }


        /// <summary>
        /// Endpoint Name: Languages, Permission Required: Languages
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.Languages)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api Languages Call Successful", typeof(LanguagesResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(LanguagesResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(LanguagesResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(LanguagesRequestExample), "request", "Encoded Request of Example Type")]
        [HMacParameterExample]
        [Route("Languages")]
        public JsonResult<LanguagesResponse> Languages(string request)
        {
            return ProcessRequest<LanguagesRequest, LanguagesResponse>(request, (param) => _mApiPublicService.GetLanguages(param));
        }


        /// <summary>
        /// Endpoint Name: LegacyAccreditations, Permission Required: LegacyAccreditations
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.LegacyAccreditations)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api LegacyAccreditations Call Successful", typeof(PublicLegacyAccreditionsResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PublicLegacyAccreditionsResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PublicLegacyAccreditionsResponseExample), typeof(DefaultContractResolver))]

        [RequestParameterExample(typeof(GetLegacyAccreditionsRequestExample), "request", "Encoded Request of Example Type")]
        
        [Route("LegacyAccreditations")]
        public JsonResult<PublicLegacyAccreditionsResponse> LegacyAccreditations(string request)
        {
            return ProcessRequest<GetLegacyAccreditionsRequest, PublicLegacyAccreditionsResponse>(request, (param) => _mApiPublicService.GetLegacyAccreditions(param));
        }


        /// <summary>
        /// Endpoint Name: TestSessions, Permission Required: TestSessions
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.TestSessions)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api TestSessions Call Successful", typeof(ApiTestSessionSearchResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiTestSessionSearchResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiTestSessionSearchResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(ApiTestSessionSearchRequestExample), "request", "Encoded Request of Example Type")]
        [Route("TestSessions")]
        public JsonResult<ApiTestSessionSearchResponse> TestSessions(string request)
        {
            return ProcessRequest<ApiTestSessionSearchRequest, ApiTestSessionSearchResponse>(request, (param) => _mApiPublicService.SearchTestSession(param));
        }


        /// <summary>
        /// Endpoint Name: EndorsedQualifications, Permission Required: EndorsedQualifications
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.EndorsedQualifications)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api EndorsedQualifications Call Successful", typeof(ApiEndorsedQualificationResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiEndorsedQualificationResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiEndorsedQualificationResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(ApiEndorseQualificationSearchRequestExample), "request", "Encoded Request of Example Type")]
        [Route("EndorsedQualifications")]
        public JsonResult<ApiEndorsedQualificationResponse> EndorsedQualifications(string request)
        {
            return ProcessRequest<ApiEndorseQualificationSearchRequest, ApiEndorsedQualificationResponse>(request, (param) => _mApiPublicService.SearchEndorsedQualification(param));
        }

        /// <summary>
        /// Endpoint Name: TestSessionAvailability, Permission Required: TestSessionAvailability
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.TestSessionAvailability)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api TestSessionAvailability Call Successful", typeof(ApiTestSessionAvailabilityResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiTestSessionAvailabilityResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiTestSessionAvailabilityResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(ApiTestSessionAvailabilityRequestExample), "request", "Encoded Request of Example Type")]
        
        [Route("TestSessionAvailability")]
        public JsonResult<ApiTestSessionAvailabilityResponse> TestSessionAvailability(string request)
        {
            return ProcessRequest<ApiTestSessionAvailabilityRequest, ApiTestSessionAvailabilityResponse>(request, (param) => _mApiPublicService.GetTestSessionAvailability(param));
        }

        [HttpGet]
        [Route("VerifyCertificationQrCode")]
        public JsonResult<ApiPublicVerifyCertificationQrCodeResponse> VerifyCertificationQrCode(string request)
        {
            var response = ProcessRequest<ApiCredentialQrCodeRequest, ApiPublicVerifyCertificationQrCodeResponse>(request, (param) => _mApiPublicService.VerifyCertificationQrCode(param));
            return response;
        }


        /// <summary>
        /// Endpoint Name: Certifications, Permission Required: Certifications
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.Certifications)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api Certifications Call Successful", typeof(ApiPublicCertificationsResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilter, "Invalid filter name which is not a part of the contract.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidFilterValue, "Invalid filter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidParameterValue, "Invalid parameter value which does not match the value’s data type.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPublicCertificationsResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPublicCertificationsResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(GetCertificationsRequestExample), "request", "Encoded Request of Example Type")]
        [Route("Certifications")]
        public JsonResult<ApiPublicCertificationsResponse> Certifications(string request)
        {
            var response = ProcessRequest<GetCertificationsRequest, ApiPublicCertificationsResponse>(request, (param) => _mApiPublicService.GetCertifications(param));
            return response;
        }


        /// <summary>
        /// Endpoint Name: PersonPhoto, Permission Required: PersonPhoto
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HmacAuthentication(EndpointPermission.PersonPhoto)]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Api PersonPhoto Call Successful", typeof(ApiPersonImageResponse))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.None, "No error occurred, Api Endpoint Call Successful.", typeof(ErrorResponse))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.GenericError, "Generic server error, can be happen due to connection or unexpected parameters.")]
        [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.ErrorParsingRequest, "Invalid request parsed through the endpoint.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidPractitionerParameterValue, "Invalid parameter value which can be null, white spaces or special character.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.PractitionerNotExist, "Parameter has been supplied but the practitioner does not exist in the system.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.PractitionerNotAllowVerifyOnline, "Cannot verify the details of this practitioner. Please contact NAATI if you require verification.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidPhotoPropertyType, "Invalid Photo Property Type which does not matches with the Table 3 -Available Photo Property Type Definition - specified.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.InvalidPhotoPropertyTypeValue, "Invalid Photo Property Type value which can be null, white spaces, special character or value that does not matches with the Property Type which specified in Table 3 -Available Photo Property Type Definition")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.PersonNotExist, "Referenced person does not exist in the system.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.PersonPhotoNotExist, "Referenced person photo does not exist in the system.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseNaatiApi(ApiPublicErrorCode.PersonPhotoNotAvailable, "Verification of the person's photo is not available in the system.")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ErrorMessageResponseExample))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPersonImageResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiPersonImageResponseExample), typeof(DefaultContractResolver))]
        [RequestParameterExample(typeof(ApiPublicPersonPhotoRequestExample), "request", "Encoded Request of Example Type")]
        [Route("PersonPhoto")]
        public JsonResult<ApiPersonImageResponse> PersonPhoto(string request)
        {
            var response = ProcessRequest<ApiPublicPersonPhotoRequest, ApiPersonImageResponse>(request, (param) => _mApiPublicService.GetPersonPhoto(param));
            return response;
        }

        [HmacAuthentication(EndpointPermission.EndorsedQualifications)]
        [HttpGet]
        [Route("VerifyDocument")]
        public JsonResult<ApiVerifyDocumentResponse> VerifyDocument(string request)
        {
            return ProcessRequest<ApiVerifyDocumentRequest, ApiVerifyDocumentResponse>(request, (param) => _mApiPublicService.VerifyDocument(param));
        }

        private JsonResult<R> ProcessRequest<T, R>(string request, Func<T, R> action, [CallerMemberName] string endpointName = null) where R : BaseResponse
        {
            T parsedRequest;
            try
            {
                var user = User?.Identity?.Name;
                LoggingHelper.LogInfo($" Processing API endPoint {{endpointName}} with request {{request}} for appId {{user}} ", endpointName, request, user);

                parsedRequest = JsonConvert.DeserializeObject<T>(request);
            }
            catch (Exception ex)
            {
                return GetApiErrorResponse<R>(request, endpointName, ex, "Request is invalid", ApiPublicErrorCode.ErrorParsingRequest);
            }
            try
            {
                var response = action(parsedRequest);

                if (response.ErrorCode != ApiPublicErrorCode.None)
                {
                    throw new ApiPublicException
                    {
                        ApiPublicErrorCode = response.ErrorCode,
                        ApiPublicErroMessage = response.Message
                    };
                }

                LoggingHelper.LogInfo($"API call with Request: {{request}}, EndpointName: {{endpointName}} processed.", request, endpointName);
                return Json(response);
            }
            catch (ApiPublicException ex)
            {
                return GetApiErrorResponse<R>(request, endpointName, ex, ex.ApiPublicErroMessage, ex.ApiPublicErrorCode);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<R>(request, endpointName, ex, "Generic error occurred.", ApiPublicErrorCode.GenericError);
            }
        }

        private JsonResult<R> GetErrorResponse<R>(string request, string endpointName, Exception exception, string responseMessage, ApiPublicErrorCode errorCode) where R : BaseResponse
        {
            var response = Activator.CreateInstance<R>();
            response.ErrorCode = errorCode;
            response.Message = responseMessage;
            LoggingHelper.LogException(exception, $"Error processing API request {{request}} {{endpointName}}. Response Message {{responseMessage}}. Error Code {{errorCode}} ", request, endpointName, responseMessage, errorCode);
            return Json(response);
        }

        private JsonResult<R> GetApiErrorResponse<R>(string request, string endpointName, Exception exception, string responseMessage, ApiPublicErrorCode errorCode) where R : BaseResponse
        {
            if (errorCode == ApiPublicErrorCode.GenericError)
            {
                return GetErrorResponse<R>(request, endpointName, exception, responseMessage, errorCode);
            }
            var response = Activator.CreateInstance<R>();
            response.ErrorCode = errorCode;
            response.Message = responseMessage;
            LoggingHelper.LogWarning(exception, $"Error processing API request {{request}} {{endpointName}}. Response Message {{responseMessage}}. Error Code {{errorCode}} ", request, endpointName, responseMessage, errorCode);
            return Json(response);
        }
    }
}