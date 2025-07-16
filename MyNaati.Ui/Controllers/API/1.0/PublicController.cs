using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Global.Common.Logging;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Ui.Common;
using MyNaati.Ui.Models;

namespace MyNaati.Ui.Controllers.API._1._0
{
    [RoutePrefix("api/1.0/public")]
    public class PublicController : BaseApiController
    {
        private readonly IAccreditationResultService mAccreditationResultService;
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly IApiAccessService mApiAccessService;

        public PublicController(IPersonalDetailsService personalDetailsService,
            IAccreditationResultService accreditationResultService,
            IApiAccessService apiAccessService)
        {
            mPersonalDetailsService = personalDetailsService;
            mAccreditationResultService = accreditationResultService;
            mApiAccessService = apiAccessService;
        }

        [HmacAuthentication(MyNaati.Contracts.BackOffice.EndpointPermission.None)]
        [HttpGet]
        [Route("Certifications")]
        public HttpResponseMessage Certifications(string practionerId, string practitionerId = null)
        {
            return Certifications(practionerId);
        }

        [HmacAuthentication(MyNaati.Contracts.BackOffice.EndpointPermission.None)]
        [HttpGet]
        [Route("Certifications")]
        public HttpResponseMessage Certifications(string practitionerId)
        {
            ApiAccessResponse apiAccess = null;
            try
            {
                var appId = System.Web.HttpContext.Current.Request.Headers["Authorization"]?.Split(':', ' ')[1];
                if (appId != null)
                {
                    apiAccess = mApiAccessService.GetApiAccess(appId);
                }

                if (string.IsNullOrWhiteSpace(practitionerId))
                {
                    LoggingHelper.LogWarning("A Public API call from {InstitutionName} was received with no Pracitioner ID", apiAccess?.InstitutionName);
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                // too many calls per second is handled by WebApiThrottle in Global.asax.cs
                var person = mPersonalDetailsService.GetPersonByPractitionerNo(practitionerId);
                if (person?.NaatiNumber == null || person.NaatiNumber <= 0)
                {
                    LoggingHelper.LogWarning("A Public API call from {InstitutionName} was received with and invalid Practitioner ID ({PractitionerId})",
                        apiAccess?.InstitutionName, practitionerId);
                    var errorModel = new PublicError()
                    {
                        ErrorCode = 1,
                        ErrorDescription = "Could not find a practitioner with the given Practitioner ID."
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, errorModel);
                }

                if (!person.AllowVerifyOnline)
                {
                    var errorModel = new PublicError()
                    {
                        ErrorCode = 2,
                        ErrorDescription =
                            "Cannot verify the details of this practitioner. Please contact NAATI if you require verification."
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, errorModel);
                }

                var request = new NaatiNumberRequest
                {
                    NaatiNumber = person.NaatiNumber.Value
                };

                var personDetails = mPersonalDetailsService.GetPersonByNaatiNo(request).Person ?? new PersonalEditPerson();
                var currentCredentials = mAccreditationResultService.GetCurrentCredentialsForPerson(request);
                var previousCredentials = mAccreditationResultService.GetPreviousCredentialsForPerson(request);

                if (!currentCredentials.Credentials.Any() && !previousCredentials.Credentials.Any())
                {
                    var errorModel = new PublicError()
                    {
                        ErrorCode = 1,
                        ErrorDescription = "Could not find a practitioner with the given Practitioner ID."
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, errorModel);
                }

                var certification = new PublicCertifications()
                {
                    ErrorCode = 0,
                    Practitioner = new PublicPractitioner()
                    {
                        PractitionerId = personDetails.PractitionerNumber,
                        GivenName = personDetails.GivenName,
                        FamilyName = personDetails.Surname,
                        Country = personDetails.Country
                    },
                    CurrentCertifications = currentCredentials.Credentials?.Select(ToPublicCredentialModel),
                    PreviousCertifications = previousCredentials.Credentials?.Select(ToPublicCredentialModel),
                };
                LoggingHelper.LogInfo("Valid API call (Institution Name: {InstitutionName}, Practitioner ID: {PractitionerId}: IPAddress: {IpAddress}).",
                    apiAccess?.InstitutionName, practitionerId, GetRemoteIpAddress());
                return Request.CreateResponse(HttpStatusCode.OK, certification);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex,
                    "Public API (Institution Name: {InstitutionName}, Practitioner ID: {PractitionerId}), Exception: {Message}, IPAddress: {IpAddress}",
                    apiAccess?.InstitutionName, practitionerId, ex.Message, GetRemoteIpAddress());
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private static PublicCredentials ToPublicCredentialModel(Credential credential)
        {
            return new PublicCredentials
            {
                Direction = credential.Direction.Replace(credential.Language, "[Language 1]").Replace(credential.ToLanguage, "[Language 2]"),
                Skill = credential.Direction,
                StartDate = credential.StartDate?.ToString("dd/MM/yyyy"),
                CertificationType = credential.Skill,
                EndDate = credential.ExpiryDate?.ToString("dd/MM/yyyy"),
            Language1 = credential.Language,
                Language2 = credential.ToLanguage
            };
        }
    }

}