using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Security;
using MyNaati.Bl.Portal;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.BackOffice.PractitionerDirectory;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Attributes;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.PDSearch;
using Newtonsoft.Json;

namespace MyNaati.Ui.Controllers.API
{
    public class PDSearchApiController : BaseApiController
    {
        private readonly PractitionerDataService _practitionerData;
        private readonly LegacyPractitionerDataService _legacyPractitionerData;
        private readonly OnlineDirectorySearch _onlineDirectorySearch;
        private readonly ILookupProvider _lookupProvider;
       // private readonly IPractitionerDirectoryService _practitionerDirectoryService;
        private readonly IPersonalDetailsService _personalDetailsService;
        private readonly ISecretsCacheQueryService _secretsProvider;

        public PDSearchApiController(PractitionerDataService practitionerData, OnlineDirectorySearch onlineDirectorySearch,
            ILookupProvider lookupProvider, IPersonalDetailsService personalDetailsService, 
            LegacyPractitionerDataService legacyPractitionerData, ISecretsCacheQueryService secretsProvider)
        {
            _practitionerData = practitionerData;
            _onlineDirectorySearch = onlineDirectorySearch;
           
            _lookupProvider = lookupProvider;
            _personalDetailsService = personalDetailsService;
            _legacyPractitionerData = legacyPractitionerData;
            _secretsProvider = secretsProvider;
        }

        [HttpGet]
        [HttpRecaptchaValidation]
        public HttpResponseMessage Credentials(string naatiNumber)
        {
            var practitionerNo = naatiNumber;
            var person = _personalDetailsService.GetPersonByPractitionerNo(practitionerNo);
            var naatiNo = person.NaatiNumber.ToString();

            if (string.IsNullOrEmpty(naatiNo))
            {
                return ResponseMessage(() =>
                    Request.CreateResponse(HttpStatusCode.BadRequest,
                        $"Practitioner Number: {practitionerNo} is not found."));
            }

            var allowVerifyOnline = person.AllowVerifyOnline;
            if (!allowVerifyOnline)
            {
                return ResponseMessage(() =>
                    Request.CreateResponse(HttpStatusCode.BadRequest,
                        "NAATI is unable to display this practitioner’s credentials. Please contact NAATI if you require verification."));
            }

            return ResponseMessage(() =>
            {
                var pratictioner = _practitionerData.GetPractitioner(naatiNo);
                return Request.CreateResponse(HttpStatusCode.OK, pratictioner);
            });
        }

        [HttpGet]
        public HttpResponseMessage Accreditations(string naatiNumber)
        {

            return ResponseMessage(() =>
            {
                var pratictioner = _legacyPractitionerData.GetPractitioner(naatiNumber);
                return Request.CreateResponse(HttpStatusCode.OK, pratictioner);
            });
        }

        [HttpGet]
        public HttpResponseMessage WizardResults(string practitionerType, string isDocumentInEnglish, int language)
        {
            return ResponseMessage(() =>
            {
                var searchModel = _onlineDirectorySearch.GetNewSearchModel(false);
                searchModel.PageHeading = "Online Directory";
                searchModel.OnlyShowResults = true;
                searchModel.FirstLanguageId = language;
                searchModel.SecondLanguageId = _lookupProvider.SystemValues.EnglishLanguageId;

                return Request.CreateResponse(HttpStatusCode.OK, searchModel);
            });
        }

        //[HttpPost]
        //public HttpResponseMessage Search(PDSearchModel search)
        //{
        //    return ResponseMessage(() =>
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return GetErrors();
        //        }

        //        var criteria = Mapper.Map<PDSearchModel, PractitionerSearchCriteria>(search);

        //        criteria.PageSize = 10;
        //        criteria.PageNumber = 0;

        //        var results = _practitionerDirectoryService.SearchPractitioners(criteria);

        //        return Request.CreateResponse(HttpStatusCode.OK, results);
        //    });
        //}

        [HttpGet]
        public HttpResponseMessage GetLanguages()
        {
            return ResponseMessage(() =>
            {
                var list = _onlineDirectorySearch.BuildLanguageList(false, false);
                return Request.CreateResponse(HttpStatusCode.OK, list);
            });
        }
    }
}