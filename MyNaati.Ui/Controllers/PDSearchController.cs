//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Web;
//using System.Web.Helpers;
//using System.Web.Mvc;
//using System.Web.WebPages;
//using AutoMapper;
//using DataDynamics.ActiveReports.Export.Pdf;
//using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
//using F1Solutions.Naati.Common.Contracts.Security;
//using F1Solutions.NAATI.ePortal.Web.Reports.PractitionersDirectory;
//using MyNaati.Bl.Portal;
//using MyNaati.Contracts.BackOffice.PractitionerDirectory;
//using MyNaati.Contracts.Portal;
//using MyNaati.Ui.Attributes;
//using MyNaati.Ui.Common;
//using MyNaati.Ui.Helpers;
//using MyNaati.Ui.ViewModels;
//using MyNaati.Ui.ViewModels.PDSearch;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using Recaptcha;
//using SortDirection = F1Solutions.Naati.Common.Contracts.Dal.Portal.Common.SortDirection;

//namespace MyNaati.Ui.Controllers
//{
//    public class PDSearchController : BaseController
//    {
//        private readonly IPractitionerDirectoryService mPractitionerDirectoryService;
//        private readonly ILookupProvider mLookupProvider;
//        private readonly PractitionerDataService mPractitionerData;
//        private readonly OnlineDirectorySearch mOnlineDirectorySearch;
//        private readonly ISecretsCacheQueryService mSecretsProvider;

//        private const string IMAGE_TYPE = "image/jpg";
//        private const string NAATI_NUMBER = "NaatiNumber";
//        private const string THE_FIELD_IS_REQUIRED = "The {0} field is required";

//        private const int MAX_IMAGE_HEIGHT = 240;
//        private const int MAX_IMAGE_WIDTH = 160;

//        private const string PD_SEARCH_CAPTCHA_SESSION_KEY = "PDSearchCaptchaVerified";


//        public PDSearchController(IPractitionerDirectoryService practitionerDirectoryService, 
//            ILookupProvider lookupService, PractitionerDataService practitionerData, OnlineDirectorySearch onlineDirectorySearch, ISecretsCacheQueryService secretsProvider)
//        {
//            mPractitionerData = practitionerData;
//            mPractitionerDirectoryService = practitionerDirectoryService;
//            mLookupProvider = lookupService;
//            mOnlineDirectorySearch = onlineDirectorySearch;
//            mSecretsProvider = secretsProvider;
//        }

//        protected override void OnException(ExceptionContext filterContext)
//        {
//            if (filterContext != null &&
//                filterContext.Exception != null &&
//                filterContext.Exception is SessionExpiredException)
//            {
//                filterContext.ExceptionHandled = true;
//                Response.Redirect(Url.Action("SessionExpired"));
//            }

//            base.OnException(filterContext);
//        }

//        [HttpGet]
//        [MvcRecaptchaValidation(modelErrorKeys: nameof(PDSearchModel.ReCaptchaErrorMessage))]
//        public ActionResult NewSearch(PDSearchModel searchModel)
//        {
//            if (ModelState.IsValid)
//            {
//                var model = mOnlineDirectorySearch.GetNewSearchModel(true);
//                if (searchModel != null)
//                {
//                    if (searchModel.FirstLanguageId != 0 || searchModel.SecondLanguageId != 0)
//                    {
//                        model.FirstLanguageId = searchModel.FirstLanguageId;
//                        model.SecondLanguageId = searchModel.SecondLanguageId;
//                    }
//                }

//                return View(model);
//            }
//            return RedirectToAction("Index", "OnlineDirectory");
//        }

//        [HttpGet]
//        public ActionResult StartNewSearch()
//        {
//            return RedirectToAction("Index", "OnlineDirectory");
//        }

//        public static bool SessionCaptchaVerified
//        {
//            get
//            {
//                var sessionValue = System.Web.HttpContext.Current.Session[PD_SEARCH_CAPTCHA_SESSION_KEY];
//                if (sessionValue == null)
//                {
//                    return false;
//                }
//                return (bool)sessionValue;
//            }
//            set { System.Web.HttpContext.Current.Session[PD_SEARCH_CAPTCHA_SESSION_KEY] = value; }
//        }

//        [HttpPost]
//        [MvcRecaptchaValidation(modelErrorKeys: nameof(PDSearchModel.ReCaptchaErrorMessage))]
//        public ActionResult StartNewSearch(PDSearchModel searchModel)
//        {
//            var isModelValid = ModelState.IsValid;

//            //when response is false check for the error message
//            if (isModelValid)
//            {
//                SessionCaptchaVerified = true;
//                return RedirectToAction("NewSearch", new { FirstLanguageId = searchModel.FirstLanguageId, SecondLanguageId = searchModel.SecondLanguageId });
//            }

//            var model = mOnlineDirectorySearch.GetNewSearchModel(true);
//            model.FirstLanguageId = searchModel.FirstLanguageId;
//            model.SecondLanguageId = searchModel.SecondLanguageId;

//            return View(model);
//        }

      

//        [HttpPost]
//        public ActionResult Search(PDSearchModel searchModel, PagingData pagingData)
//        {
//            var message = new StringBuilder();

//            if (pagingData.Rows == 0)
//            {
//                pagingData.Rows = 10;
//            }
//            if (pagingData.Page == 0)
//            {
//                pagingData.Page = 1;
//            }

//            if (!SessionCaptchaVerified)
//            {
//                return Json(new JsonResultsWrapper<Practitioner> { Data = null, IsError = true, Message = "Recaptcha verification not passed for this session", RecaptchaRequired = false });
//            }

//            if (ModelState.IsValid)
//            {
//                RecaptchaControlMvc.SkipRecaptcha = false;
//                var criteria = new PractitionerSearchCriteria
//                {
//                    SecondLanguageId = searchModel.SecondLanguageId,
//                    AccreditationLevelId = searchModel.AccreditationLevelId,
//                    CountryId = searchModel.CountryId,
//                    FirstLanguageId = searchModel.FirstLanguageId,
//                    Skills = searchModel.Skills,
//                    Postcode = searchModel.Postcode,
//                    RandomSearchSeed = searchModel.RandomSearchSeed,
//                    StateId = searchModel.StateId,
//                    Surname = searchModel.Surname,
//                    ToEnglish = searchModel.ToEnglish,
//                    PageSize = pagingData.Rows,
//                    PageNumber = pagingData.Page,
//                };

//                PDSortMember sortMember;
//                Enum.TryParse(searchModel.SortMember, out sortMember);
//                criteria.SortMember = sortMember;

//                criteria.SortOrder = criteria.SortMember == PDSortMember.Level ? SortDirection.Descending : SortDirection.Ascending;
//                PDSortMember enumResult;
//                if (PDSortMember.TryParse(searchModel.SortMember, out enumResult))
//                {
//                    criteria.SortMember = enumResult;
//                }
//                else
//                {
//                    criteria.SortMember = PDSortMember.Level;
//                }
                
//                var results = mPractitionerDirectoryService.SearchPractitioners(criteria);

//                if (results.TotalResultsCount <= 0)
//                {
//                    criteria.StateId = null;
//                    criteria.Postcode = null;

//                    results = mPractitionerDirectoryService.SearchPractitioners(criteria);

//                    message.AppendLine("No results found using the selected filters. ");
//                    message.AppendLine("Showing results without State or Postcode instead.");

//                    return Json(new JsonResultsWrapper<Practitioner> { Data = results, IsError = false, Message = message.ToString(), Criteria = BuildCriteriaString(searchModel), Code = "1" }); // Code 1 means broadened search
//                }

//                if (results.PageNumber > results.TotalPageCount)
//                {
//                    criteria.PageNumber = 1;
//                    results = mPractitionerDirectoryService.SearchPractitioners(criteria);
//                }

//                return Json(new JsonResultsWrapper<Practitioner> { Data = results, IsError = false, Message = string.Empty, Criteria = BuildCriteriaString(searchModel) });
//            }

//            foreach (var error in ModelState.Values.Where(item => item.Errors.Any()).SelectMany(item => item.Errors))
//            {
//                message.AppendLine(error.ErrorMessage);
//            }

//            return Json(new JsonResultsWrapper<Practitioner> { Data = null, IsError = true, Message = message.ToString() });
//        }

//        [HttpPost]
//        public ActionResult Count(PDSearchModel searchModel)
//        {
//            if (searchModel.SortMember == null)
//            {
//                searchModel.SortMember = PDSortMember.Level.ToString();
//            }
//            var criteria = Mapper.Map<PDSearchModel, PractitionerSearchCriteria>(searchModel);

//            var results = mPractitionerDirectoryService.CountPractitioners(criteria);

//            return Json(new JsonCountWrapper<ValCount> { Data = results, IsError = false, Message = string.Empty, Criteria = BuildCriteriaString(searchModel) });
//        }

//        [HttpGet]
//        public ActionResult ExportSearchToPDF(int? skillsFirst, int? skillsSecond, int firstLanguageId,
//            int secondLanguageId,
//            int accreditationLevelId,
//            int countryId,
//            int? stateId,
//            string postcode,
//            string surname,
//            int randomSearchSeed,
//            bool? toEnglish)
//        {
//            var pagingData = new PagingData();
//            var searchModel = new PDSearchModel
//            {
//                FirstLanguageId = firstLanguageId,
//                SecondLanguageId = secondLanguageId,
//                AccreditationLevelId = accreditationLevelId,
//                CountryId = countryId,
//                StateId = stateId.GetValueOrDefault(),
//                Postcode = postcode,
//                RandomSearchSeed = randomSearchSeed,
//                ToEnglish = toEnglish,
//                Skills = new[] { skillsFirst.GetValueOrDefault(), skillsSecond.GetValueOrDefault() }
//            };

//            if (ModelState.IsValid)
//            {
//                if (searchModel.SortMember == null)
//                {
//                    searchModel.SortMember = PDSortMember.Level.ToString();
//                }

//                var criteria = Mapper.Map<PDSearchModel, PractitionerSearchCriteria>(searchModel);

//                criteria.PageSize = 1000000;
//                criteria.PageNumber = pagingData.Page;
//                criteria.SortOrder = pagingData.SortDirection;

//                var results = mPractitionerDirectoryService.ExportPractitioners(criteria);

//                foreach (var item in results.Results)
//                {
//                    item.ContactDetails = BuildContactDetails(item.ContactDetails);
//                }

//                var rpt = new PractitionerListing();
//                rpt.SearchCriteria.Text = BuildCriteriaString(searchModel);

//                rpt.DataSource = results.Results;

//                rpt.Run();

//                PdfExport pdf = new PdfExport();
//                using (var memStream = new MemoryStream())
//                {
//                    pdf.Export(rpt.Document, memStream);
//                    return File(memStream.ToArray(), "application/pdf", string.Format("ExportedPD-{0}-{1}.pdf", DateTime.Now.ToString("ddMMyyyy"),
//                        DateTime.Now.ToString("ddMMyyyy")));
//                }
//            }

//            var message = new StringBuilder();

//            foreach (var error in ModelState.Values.Where(item => item.Errors.Any()).SelectMany(item => item.Errors))
//            {
//                message.AppendLine(error.ErrorMessage);
//            }

//            searchModel.ReCaptchaErrorMessage = message.ToString();

//            return Json(new JsonResultsWrapper<Practitioner> { Data = null, IsError = true, Message = message.ToString() });
//        }

//        private List<ContactDetail> BuildContactDetails(List<ContactDetail> contactDetails)
//        {
//            var lsResult = new List<ContactDetail>();
//            foreach (var item in contactDetails)
//            {
//                item.SortOrder = item.Type == "Phone" ? 1 : 2;
//                lsResult.Add(new ContactDetail { Contact = item.Contact, SortOrder = item.SortOrder });
//            }

//            return lsResult.OrderBy(x => x.SortOrder).ThenBy(x => x.Type).ToList();
//        }

//        public string BuildCriteriaString(PDSearchModel model)
//        {
//            var message = new StringBuilder();


//            if (model.FirstLanguageId != 0 || model.SecondLanguageId != mLookupProvider.SystemValues.EnglishLanguageId)
//            {
//                string languageJoiningTerm = "and";
//                if (model.ToEnglish != null)
//                {
//                    languageJoiningTerm = model.ToEnglish.Value ? "to" : "from";
//                }

//                var firstLanguage = mLookupProvider.Languages.SingleOrDefault(x => x.SamId == model.FirstLanguageId);
//                var firstLanguageText = firstLanguage != null ? firstLanguage.DisplayText : string.Empty;

//                var secondLanguage = mLookupProvider.Languages.SingleOrDefault(x => x.SamId == model.SecondLanguageId);
//                var secondLanguageText = secondLanguage != null ? secondLanguage.DisplayText : string.Empty;

//                message.AppendLine(string.Format("Languages: {0} {2} {1}", firstLanguageText, secondLanguageText, languageJoiningTerm));
//            }

//            if (model.AccreditationLevelId != 0)
//            {
//                message.AppendLine(string.Format("Expertise: {0}", mLookupProvider.CertificatesCredentialTypes.FirstOrDefault(x=> x.SamId == model.AccreditationLevelId)?.DisplayText ?? string.Empty));
//            }

//            if (model.StateId != 0)
//            {
//                message.AppendLine(string.Format("State: {0}", mLookupProvider.States.Single(x => x.SamId == model.StateId).DisplayText));
//            }

//            var countryName = model.CountryId == 0 ? "All" : mLookupProvider.Countries.Single(x => x.SamId == model.CountryId).DisplayText;
//            message.AppendLine(string.Format("Country: {0}", countryName));

//            if (!string.IsNullOrEmpty(model.Postcode))
//            {
//                message.AppendLine(string.Format("Postcode: {0}", model.Postcode));
//            }

//            if (!string.IsNullOrEmpty(model.Surname))
//            {
//                message.AppendLine(string.Format("Surname: {0}", model.Surname));
//            }

//            return message.ToString();
//        }

//        [HttpGet]
//        public virtual ActionResult SessionExpired()
//        {
//            return View();
//        }
        
//        public ActionResult ContactDetails(int identifier, int hash, int seed)
//        {
//            var contactDetailsResponse =
//                mPractitionerDirectoryService.GetPractitionerContactDetails(
//                    new PractitionerDirectoryGetContactDetailsRequest { Identifier = identifier, Hash = hash, Seed = seed});


//            //Contact details is deliberately left out of the mapping so that we can filter it here.
//            //I didn't want to dilute the location of the rules by adding the filter to automappper, if that's possible.
//            var viewModel = Mapper.Map<PractitionerDirectoryGetContactDetailsResponse, ContactDetailsModel>(contactDetailsResponse);

//            viewModel.ContactDetails = BuildViewContactDetails(contactDetailsResponse.ContactDetails).ToList();
//            viewModel.Accreditations = contactDetailsResponse.LegacyAccreditations
//                .Select(x => new ViewAccreditation
//                {
//                    Accreditation = $"{x.Level} {x.Category} ({x.Direction})",
//                    Expiration =  $"Issue date: {x.StartDate}{(x.EndDate != null ? $" – Expiry date: {x.EndDate}" : "")}"
//                })
//                .ToList();
//            viewModel.DefaultContryId = mLookupProvider.Countries.First(x => x.IsHomeCountry).SamId;
//            viewModel.Credentails = mPractitionerDirectoryService.GetPractionerCredentials(identifier).Where(x => x.IncludeOD);
//            viewModel.Website = contactDetailsResponse.Website;
//            return View(viewModel);
//        }

//        private IEnumerable<ViewContactDetail> BuildViewContactDetails(IEnumerable<ContactDetail> contactDetails)
//        {
//            var viewContactDetails = new List<ViewContactDetail>();
//            foreach (var item in contactDetails)
//            {
//                item.SortOrder = item.Type == "Phone" ? 1 : 2;
//                viewContactDetails.Add(new ViewContactDetail { Contact = item.Contact, Type = item.Type, SortOrder = item.SortOrder });
//            }

//            return viewContactDetails.OrderBy(x => x.SortOrder).ThenBy(x => x.Type);
//        }

//        public bool LanguageSelectionsAreValid(PDSearchModel model)
//        {
//            return true;
//        }

//        [HttpPost]
//        [RecaptchaControlMvc.CaptchaValidator]
//        public ActionResult Credentials(string naatiNumber, bool captchaValid, string captchaErrorMessage)
//        {
//            var captchaMessage = RegisterHelper.GetCaptchaMessage(captchaValid, captchaErrorMessage);
//            if (!String.IsNullOrWhiteSpace(captchaMessage))
//                ModelState.AddModelError(string.Empty, captchaMessage);


//            naatiNumber = string.IsNullOrEmpty(naatiNumber) ? string.Empty : naatiNumber.Trim();
//            var model = new VerifyCredentialsModel
//            {
//                NaatiNumber = naatiNumber
//            };

//            if (string.IsNullOrEmpty(naatiNumber))
//            {
//                var displayName = DisplayHelpers.GetDisplayNameAttribute<VerifyCredentialsModel>(e => e.NaatiNumber);
//                ModelState.AddModelError(NAATI_NUMBER, string.Format(THE_FIELD_IS_REQUIRED, displayName));
//                return View("VerifyCredentials", model);
//            }

//            try
//            {
//                Practitioner practitioner = mPractitionerData.GetPractitioner(naatiNumber);
//                model.Practitioner = practitioner;
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError(string.Empty, ex.Message);
//            }

//            if (ModelState.IsValid)
//            {
//                return View(model);
//            }

//            return View("VerifyCredentials", model);
//        }

//        [HttpGet]
//        public ActionResult VerifyCredentials()
//        {
//            return View(new VerifyCredentialsModel());
//        }

//        [HttpPost]
//        public ActionResult VerifyCredentials(VerifyCredentialsModel model)
//        {
//            ModelState.Clear();
//            return View(model);
//        }

//        public ActionResult ProfilePicture(int naatiNumber)
//        {
//            byte[] image = mPractitionerDirectoryService.GetProfilePicture(naatiNumber);
//            if (image != null)
//            {
//                var webImage = new WebImage(image).Resize(MAX_IMAGE_WIDTH, MAX_IMAGE_HEIGHT);
//                return File(webImage.GetBytes(), IMAGE_TYPE);
//            }

//            var defaultImage = new WebImage(Server.MapPath(Url.Content("~/Content/images/PractitionerDefault.jpg")));
//            return File(defaultImage.GetBytes(), IMAGE_TYPE);
//        }
//    }

//    public class JsonResultsWrapper<T>
//    {
//        public SearchResults<T> Data { get; set; }
//        public bool IsError { get; set; }
//        public string Message { get; set; }
//        public string Criteria { get; set; }
//        public bool RecaptchaRequired { get; set; }
//        public string Code { get; set; }
//    }

//    public class JsonCountWrapper<T>
//    {
//        public CountResults<T> Data { get; set; }
//        public bool IsError { get; set; }
//        public string Message { get; set; }
//        public string Criteria { get; set; }
//    }

//    public static class CustomJavaScriptConverter
//    {
//        public static IHtmlString Convert(object value)
//        {
//            using (var stringWriter = new StringWriter())
//            using (var jsonWriter = new JsonTextWriter(stringWriter))
//            {
//                var serializer = new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() };

//                jsonWriter.QuoteName = false;
//                serializer.Serialize(jsonWriter, value);

//                return new HtmlString(stringWriter.ToString());
//            }
//        }
//    }
//}
