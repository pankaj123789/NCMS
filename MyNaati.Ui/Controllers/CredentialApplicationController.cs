using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using MyNaati.Bl.BackOffice.Helpers;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.UI;
using MyNaati.Ui.ViewModels.CredentialApplication;
using Newtonsoft.Json;
using IOrderService = MyNaati.Contracts.Portal.IOrderService;

namespace MyNaati.Ui.Controllers
{
    public class CredentialApplicationController : NewtonsoftController
    {
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly ICredentialApplicationService mCredentialApplicationService;
        private readonly ILookupProvider mLookupProvider;
        private readonly IEmailService mEmailService;
        private readonly IOrderService mOrderService;
        private readonly IAccountingService mAccountingService;
        private readonly ISystemValueService mSystemValueService;
        private readonly INcmsIntegrationService _ncmsIntegrationService;
        private readonly ISecretsCacheQueryService mSecretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IPayPalService mPayPalService;

        public static class ConfimationContentTokens
        {
            public const string ApplicationReference = "Application Reference";
            public const string SecurepayReference = "Securepay Reference";
            public const string PayPalReference = "PayPal Reference";
            public const string InvoiceNumber = "Invoice Number";
            public const string CustomerNumber = "Customer Number";
            public const string PrimaryEmail = "Primary Email";
            public const string GivenName = "Given Name";
            public const string FamilyName = "Family Name";
            public const string ErrorMessage = "Error Message";
        }

        private const string ConfirmationContentCCLCreditCard = @"
<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>Also note your credit card transaction reference: <strong>[[Securepay Reference]]</strong> </p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ol>
        <li>Your payment will be processed by NAATI immediately and you will then receive a second email containing your invoice within 24 hours.</li>
        <li>Your application is then added to the processing queue for NAATI to check. This can take 1 week or more.</li>
        <li>Your application will be checked. Once approved you will receive an email confirmation. You will then be able to log onto myNAATI and select your test session.</li>
    </ol>
</ul>

<p><strong>NOTE:</strong> CCL test places are in high demand. You should plan ahead as there may be a gap of several months before you can sit a test. </p>
<p><a href = ""https://www.naati.com.au/services/ccl/"">Click here</a> to return to the NAATI CCL testing information page.</p>";

        private const string ConfirmationContentCCLPayPal = @"
<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>Also note your PayPal transaction reference: <strong>[[Securepay Reference]]</strong> </p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ol>
        <li>Your payment will be processed by NAATI immediately and you will then receive a second email containing your invoice within 24 hours.</li>
        <li>Your application is then added to the processing queue for NAATI to check. This can take 1 week or more.</li>
        <li>Your application will be checked. Once approved you will receive an email confirmation. You will then be able to log onto myNAATI and select your test session.</li>
    </ol>
</ul>

<p><strong>NOTE:</strong> CCL test places are in high demand. You should plan ahead as there may be a gap of several months before you can sit a test. </p>
<p><a href = ""https://www.naati.com.au/services/ccl/"">Click here</a> to return to the NAATI CCL testing information page.</p>";


        private const string ConfirmationContentCCLCashChequeAndDirectDeposit = @"<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong>

<ul>
    <ol>
        <li>NAATI will email your invoice within 24 hours. All invoices need to be paid within 3 days otherwise NAATI reserves the right to cancel your application.</li>
        <li>NAATI will then reconcile your payment. This can take up to 72 hours.</li>
        <li>Once reconciled, your application is then added to the processing queue for NAATI to check. This can take 1 week or more.</li>
        <li>Your application will be checked. Once approved you will receive email confirmation. You will then be able to log onto myNAATI and select your test session.</li>
    </ol>
</ul>


<p><strong>NOTE:</strong> CCL test places are in high demand. You should plan ahead as there may be a gap of several months before you can sit a test. </p>
<p><a href=""https://www.naati.com.au/services/ccl/"">Click here</a> to return to the NAATI CCL testing information page.</p>";

        private const string ConfirmationContentCCLV3 = @"<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong>

<ul>
    <ol>

        <li>Your application will be added to the processing queue for NAATI to check. This can take 1 week or more.</li>
        <li>Your application will be checked. Once approved you will receive email confirmation. You will then be able to log in to myNAATI, select a test date and make payment.</li>        
    </ol>
</ul>


<p><strong>NOTE:</strong> CCL test places are in high demand. You should plan ahead as there may be a gap of several months before you can sit a test. </p>
<p><a href=""https://www.naati.com.au/services/ccl/"">Click here</a> to return to the NAATI CCL testing information page.</p>";

        private const string ConfirmationContentCertification = @"<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>
<ul>
    <li> NAATI will now begin processing your application. Should we need to follow up any missing or additional information we will contact you.</li>
    <li> If you need to sit a prerequisite test, we will contact you to make arrangements.</li>
    <li>  Once your application has been assessed and you are eligible to sit a Certification test, you will be sent an email. You can then log in to myNAATI, select a test date and make payment.</li>
    <li> Please be aware that it may take several months for test places to become available. <br /></li>
</ul>
<p><a href=""https://www.naati.com.au"">Click here</a> to return to the NAATI home page.</p>";

        private const string ConfirmationContentCertificationPractitioner = @"<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>
<ul>
    <li> NAATI will now begin processing your application. Should we need to follow up any missing or additional information we will contact you.</li>
    <li> Once your application has been assessed and you are eligible to sit a Certification test, you will be sent an email. You can then log in to myNAATI, select a test date and make payment.</li>
    <li> Please be aware that it may take several months for test places to become available. <br /></li>
</ul>
<p><a href=""https://www.naati.com.au"">Click here</a> to return to the NAATI home page.</p>";

        private const string ConfirmationContentRecertification = @"<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>
<p>1. NAATI will perform an initial check of your application. This should be completed within 2 weeks.</p>
<p>2. We will then email you an invoice for payment. All invoices need to be paid within 3 days, otherwise NAATI reserves the right to cancel your application.</p>
<p>3. Once your payment is received, NAATI will begin processing your application. If we require any missing or additional information, we will contact you. Your application assessment should be completed within 4 weeks of payment.</p>
<br/>
<p>If you have requested any products with your application, these products will be mailed to your primary address after your application assessment has been completed. </p>
<p><a href=""https://www.naati.com.au"">Click here</a> to return to the NAATI home page.</p>";

        private const string ConfirmationContentCla = @"<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>
<p>1. NAATI will perform an initial check of your application. This should be completed within 1 week.</p>
<p>2. If your application is not sponsored, NAATI will then email you an invoice for payment. All invoices need to be paid within 3 days, otherwise NAATI reserves the right to cancel your application.</p>
<p>3. Once your payment is received, NAATI will begin processing your application. If we require any missing or additional information, we will contact you. Your application assessment should be completed within 2 weeks of payment.</p>
<p>4. Should NAATI deem you eligible to sit a CLA test and once test dates are available, NAATI will email you with instructions on how to select your preferred test date.</p>

<p><a href=""https://www.naati.com.au"">Click here</a> to return to the NAATI home page.</p>";

        private const string ConfirmationContentPracticeTest = @"<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>
<p>1. Your application will be added to the queue for NAATI to process.</p>
<p>2. Once approved you will receive email confirmation. You will then be able to log in to myNAATI, select a date for your practice test and make payment.</p>

<p><a href=""https://www.naati.com.au"">Click here</a> to return to the NAATI home page.</p>";

        public CredentialApplicationController(IPersonalDetailsService personalDetailsService, ICredentialApplicationService credentialApplicationService, ILookupProvider lookupProvider, 
            IEmailService emailService, IOrderService orderService, IAccountingService accountingService, ISystemValueService systemValueService, INcmsIntegrationService ncmsIntegrationService,
            ISecretsCacheQueryService secretsProvider, IAutoMapperHelper autoMapperHelper, IPayPalService payPalService)
        {
            mPersonalDetailsService = personalDetailsService;
            mLookupProvider = lookupProvider;
            mCredentialApplicationService = credentialApplicationService;
            mEmailService = emailService;
            mOrderService = orderService;
            mAccountingService = accountingService;
            mSystemValueService = systemValueService;
            _ncmsIntegrationService = ncmsIntegrationService;
            mSecretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
            mPayPalService = payPalService;
        }


        [HttpGet]
        [Route("apply/{id?}")]
        public ActionResult Index(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(id);
            }

            return RedirectToAction("LogOn", "Account");
        }

        [HttpGet]
        [Route("apply/submission-result/{id}")]
        public ActionResult SubmissionResult(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ApplicationReference = id;
                ViewBag.NAATIHomePage = ConfigurationManager.AppSettings["ExternalUrl_NAATI_URL"];

                string strApplicationId = id.Substring(3, id.Length - 3);
                int applicationId = Convert.ToInt32(strApplicationId);

                ViewBag.NAATINumber = mCredentialApplicationService.GetNaatiNumberByApplicationId(applicationId);

                //Secure pay error handling 
                var isSecurePayStatusSuccess = (bool?)Session["IsSecurePayStatusSuccess"] ?? false;
                var isSecurePayResponseSuccess = (bool?)Session["IsSecurePayResponseSuccess"] ?? false;
                ViewBag.IsSecurePayStatusSuccess = isSecurePayStatusSuccess;
                ViewBag.IsSecurePayResponseSuccess = isSecurePayResponseSuccess;

                //secure pay
                ViewBag.ReferenceNumber = (string)Session["ReferenceNumber"] ?? "secure pay reference not found";

                //remove Secure pay session variables
                Session["IsSecurePayStatusSuccess"] = null;
                Session.Remove("IsSecurePayStatusSuccess");
                Session["IsSecurePayResponseSuccess"] = null;
                Session.Remove("IsSecurePayResponseSuccess");

                Session["ReferenceNumber"] = null;
                Session.Remove("ReferenceNumber");

                return View();
            }

            return RedirectToAction("LogOn", "Account");
        }

        private string ReplaceTokens(string confirmationContent, GetApplicationDetailsResponse applicationDetails, PayOnlineResponseModel payOnlineResponse)
        {
            var constants = typeof(ConfimationContentTokens).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

            foreach (var constant in constants)
            {
                var token = Convert.ToString(constant.GetValue(null));
                var tokenValue = String.Empty;

                switch (token)
                {
                    case ConfimationContentTokens.ApplicationReference:
                        tokenValue = applicationDetails.ApplicationInfo.ApplicationReference;
                        break;
                    case ConfimationContentTokens.SecurepayReference:
                        tokenValue = payOnlineResponse.SecurePayReference;
                        break;
                    case ConfimationContentTokens.PayPalReference:
                        tokenValue = payOnlineResponse.PayPalReference;
                        break;
                    case ConfimationContentTokens.InvoiceNumber:
                        tokenValue = payOnlineResponse.WiiseInvoice;
                        break;
                    case ConfimationContentTokens.CustomerNumber:
                        tokenValue = applicationDetails.ApplicationInfo.NaatiNumber.ToString();
                        break;
                    case ConfimationContentTokens.PrimaryEmail:
                        tokenValue = applicationDetails.ApplicationInfo.ApplicantPrimaryEmail;
                        break;
                    case ConfimationContentTokens.GivenName:
                        tokenValue = applicationDetails.ApplicationInfo.ApplicantGivenName;
                        break;
                    case ConfimationContentTokens.FamilyName:
                        tokenValue = applicationDetails.ApplicationInfo.ApplicantFamilyName;
                        break;
                }

                confirmationContent = confirmationContent.Replace($"[[{token}]]", tokenValue);
            }

            return confirmationContent;
        }

        private string GetConfirmationContent(GetApplicationDetailsResponse applicationDetails, PayOnlineResponseModel payOnlineResponse)
        {
            var credentialApplicationType = (CredentialApplicationTypeName)applicationDetails.ApplicationType.Id;
            var confirmationContent = String.Empty;

            switch (credentialApplicationType)
            {
                case CredentialApplicationTypeName.Certification:
                    confirmationContent = ConfirmationContentCertification;
                    break;
                case CredentialApplicationTypeName.CertificationPractitioner:
                    confirmationContent = ConfirmationContentCertificationPractitioner;
                    break;
                case CredentialApplicationTypeName.CCL:
                    confirmationContent = ConfirmationContentCCLCashChequeAndDirectDeposit;
                    break;
                case CredentialApplicationTypeName.CCLV2:
                    if (payOnlineResponse.IsPayByCreditCard)
                    {
                        confirmationContent = ConfirmationContentCCLCreditCard;
                    }else
                    {
                        if(payOnlineResponse.IsPayByPayPal)
                        {
                            confirmationContent = ConfirmationContentCCLPayPal;
                        }
                        else
                        {
                            confirmationContent = ConfirmationContentCCLCashChequeAndDirectDeposit;
                        }
                    }
                    break;
                case CredentialApplicationTypeName.CCLV3:
                    confirmationContent = ConfirmationContentCCLV3;
                    break;
                case CredentialApplicationTypeName.Recertification:
                    confirmationContent = ConfirmationContentRecertification;
                    break;
                case CredentialApplicationTypeName.Cla:
                    confirmationContent = ConfirmationContentCla;
                    break;
                case CredentialApplicationTypeName.PracticeTest:
                    confirmationContent = ConfirmationContentPracticeTest;
                    break;
                default:
                    confirmationContent = String.Empty;
                    break;
            }

            return ReplaceTokens(confirmationContent, applicationDetails, payOnlineResponse);
        }

        [HttpGet]
        public ActionResult Forms()
        {
            if (User.Identity.IsAuthenticated)
            {
                var isPractitioner = User.IsPractitioner();
                var isRecertification = User.IsRecertification();
                var model = mCredentialApplicationService.GetPublicApplicationForms(isPractitioner, isRecertification, User?.Identity?.IsAuthenticated ?? false);
                model.IsAuthenticated = User.Identity.IsAuthenticated;
                model.BaseUrl = ConfigurationManager.AppSettings["ExternalUrl_NAATI_URL"];

                return Json(model, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("LogOn", "Account");
        }


        [HttpGet]
        public ActionResult PersonDetails()
        {
            var naatiNumber = User.NaatiNumber();
            if (naatiNumber == default(int))
            {
                return Json(new { Anonymous = true }, JsonRequestBehavior.AllowGet);
            }

            var serviceResponse = mCredentialApplicationService.GetPersonDetailsBasic(naatiNumber).PersonDetails;

            var personDetails = new
            {
                serviceResponse.NaatiNumber,
                serviceResponse.FamilyName,
                serviceResponse.GivenName,
                serviceResponse.OtherNames,
                serviceResponse.PrimaryEmail,
                serviceResponse.CountryOfBirth,
                DateOfBirth = serviceResponse.DateOfBirth?.Date.ToString(CultureInfo.InvariantCulture),
                serviceResponse.Gender,
                serviceResponse.Title
            };

            return Json(new
            {
                PersonDetails = personDetails
            }, JsonRequestBehavior.AllowGet);
        }

        private void ValidateApplicationToken(int applicationId, int token)
        {
            if (!mCredentialApplicationService.ValidateApplicationToken(applicationId, token))
            {
                throw new Exception($"Error validating action. Application ID:{applicationId} Token: {token} ");
            }
        }

        private void ValidatePersonToken(int naatiNumber, int token)
        {
            if (!mCredentialApplicationService.ValidatePersonToken(naatiNumber, token))
            {
                throw new Exception($"Error validating action. Person NaatiNumber:{naatiNumber} Token: {token} ");
            }
        }
        /// <summary>
        /// Get available sections to online form
        /// </summary>
        /// <returns>Available sections with questions and answers</returns>
        [HttpGet]
        public ActionResult Sections(GetSectionsRequestModel request)
		{
			if (request.ApplicationId > 0)
			{
				ValidateApplicationToken(request.ApplicationId, request.Token);
			}

			var list = Sections(request.ApplicationFormId, request.ApplicationId);
			return Json(list, JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        public ActionResult CanEditPersonDetails()
        {
            if (CurrentUserNaatiNumber == 0)
            {
                return Json(new { CanEditPersonDetails = true, CanEditEmail = true }, JsonRequestBehavior.AllowGet);
            }

            var hasSubmittedApplications =
                mCredentialApplicationService.HasSubmittedApplications(CurrentUserNaatiNumber);
            return Json(new { CanEditPersonDetails = !hasSubmittedApplications, CanEditEmail = false }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ApplicationFormSectionModel> Sections(int applicationFormId, int applicationId = 0)
		{
			var applicationRequestContract = new GetFormSectionRequestContract
			{
				ApplicationId = applicationId,
				ApplicationFormId = applicationFormId,
				ExternalUrls = GetExternalUrls(),
				IsPractitioner = User.Identity.IsAuthenticated && User.IsPractitioner(),
			    IsRecertificationUser = User.Identity.IsAuthenticated && User.IsRecertification(),
                NaatiNumber = CurrentUserNaatiNumber
			};

			var sections = mCredentialApplicationService.GetCredentialApplicationFormSections(applicationRequestContract);
            
			var list = sections.Results.Select(_autoMapperHelper.Mapper.Map<ApplicationFormSectionModel>);
			return list;
		}

		private IEnumerable<KeyValuePair<string, string>> GetExternalUrls()
        {
            var keys = ConfigurationManager.AppSettings.AllKeys.Where(k => k.StartsWith("ExternalUrl"));
            var list = keys.Select(x => new KeyValuePair<string, string>(x, ConfigurationManager.AppSettings[x])).ToList();
            return list;
        }

        [HttpPost]
        public ActionResult ParseAddress(GeoResultModel request)
        {
            var response = mPersonalDetailsService.ParseAddress(request);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save form as draft
        /// </summary>
        /// <param name="request">Sections, questions and answers</param>
        [HttpPost]
        public ActionResult Save(SaveApplicationFormRequestModel request)
        {
            if (request.ApplicationId > 0)
            {
                ValidateApplicationToken(request.ApplicationId, request.Token);
            }
            else if (request.NaatiNumber > 0)
            {
                ValidatePersonToken(request.NaatiNumber, request.Token);
            }

            var requestContract = _autoMapperHelper.Mapper.Map<SaveApplicationFormRequestContract>(request);
 
            var result = mCredentialApplicationService.SaveApplicationForm(requestContract);

            return Json(new SaveApplicationFormResponse
            {
                ApplicationId = result.ApplicationInfo?.ApplicationId ?? 0,
                ApplicationReference = result.ApplicationInfo?.ApplicationReference,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Submit an Application
        /// </summary>
        /// <param name="request">Sections, questions and answers</param>
        [HttpPost]
        // public ActionResult Submit(SaveApplicationFormRequestModel request)
        public ActionResult Submit(SaveApplicationFormRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var requestContract = _autoMapperHelper.Mapper.Map<SaveApplicationFormRequestContract>(request);
            //validate PostCode and Suburb are present (WIISE isuue/bug/requirement)
            var UiCheckResponse = VerifyUiModelIsCorrect(requestContract);
            if (UiCheckResponse != null)
            {
                return Json(new SubmitApplicationResponse()
                {
                    SaveApplicationFormResponse = new SaveApplicationFormResponse()
                    {
                        ErrorMessage = @"<h2 style=""color:#008C7E;""><strong>Application Not Submitted</strong></h2>
                        <br/>

                        <p><strong> Address is incomplete. Please Contact NAATI. </p>
                        <p><a href = ""https://www.naati.com.au/services/ccl/"">Click here</a> to return to the NAATI CCL testing information page.</p>"
                    }
                });
            }
            var result = mCredentialApplicationService.SaveApplicationForm(requestContract);

            var payOnlineResponse = new PayOnlineResponseModel();

                var feeSection = request.Sections.Where(x => x.Name == "Fees").ToList();
                if (feeSection.Any())
                {
                    var questions = feeSection.Select(y => y.Questions).FirstOrDefault();
                    if (questions != null)
                    {
                        var answerResponse = questions.Where(x => x.AnswerTypeName == "Fees").Select(x => x.Response).FirstOrDefault();
                        var jsonSerializer = new JavaScriptSerializer();
                        if (answerResponse != null)
                        {
                            var feeCreditCardDetails = jsonSerializer.Deserialize<FeePaymentDetails>(answerResponse.ToString());
                            if (feeCreditCardDetails.PaymentMethodType == (int)PaymentMethodType.CreditCard)
                            {
                                var errors = ValidateSubmitApplicationWithCreditCard(request.ApplicationId, string.Empty, string.Empty,
                                    feeCreditCardDetails.Total);
                                if (errors.Any())
                                {
                                    throw new Exception(string.Concat(errors));
                                }
                                payOnlineResponse = PayOnline(request, feeCreditCardDetails);
                                payOnlineResponse.IsPayByCreditCard = true;
                                if (payOnlineResponse.IsSecurePayStatusSuccess)
                                {
                                    SubmitSamApplicationWithCreditCard(request.ApplicationId, payOnlineResponse.SecurePayReference, feeCreditCardDetails.Total, payOnlineResponse.OrderNumber);
                                }
                            }
                            else
                            {
                                SubmitSamApplication(request.ApplicationId);
                            }
                        }
                    }
                }
                else
                {
                    SubmitSamApplication(request.ApplicationId);
                }

                var confirmationContent = GetConfirmationContent(result, payOnlineResponse);

                var submitApplicationResponse = new SubmitApplicationResponse
                {
                    SaveApplicationFormResponse = new SaveApplicationFormResponse
                    {
                        ApplicationId = result.ApplicationInfo.ApplicationId,
                        ApplicationReference = result.ApplicationInfo.ApplicationReference,
                    },
                    PayOnlineResponse = payOnlineResponse,
                    ConfirmContent = confirmationContent
                };

                return Json(submitApplicationResponse, JsonRequestBehavior.AllowGet);
            
        }

        private PayOnlineResponseModel PayOnline(SaveApplicationFormRequestModel request, FeePaymentDetails feeCreditCardDetails)
        {
            var payOnlineResponse = new PayOnlineResponseModel();

            try
            {
                var securePayResponse = MakeSecurePaymentEmbedded(request.NaatiNumber, request.ApplicationId, feeCreditCardDetails);

                if (securePayResponse.PaymentFailureDetails?.SystemError == true)
                {
                    payOnlineResponse.FeesQuestionId = mCredentialApplicationService.GetFeeQuestionId();
                    payOnlineResponse.IsSecurePayStatusSuccess = false;
                    payOnlineResponse.ErrorMessage = securePayResponse.PaymentFailureDetails.SystemErrorMessage;
                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (CC last 4: {CreditCardLastFour})", 
                        request.ApplicationId, securePayResponse.PaymentFailureDetails.SystemErrorMessage, feeCreditCardDetails.CreditCardNumber.Tail(4));
                    return payOnlineResponse;
                }

                if (securePayResponse.UnHandledException && !string.IsNullOrEmpty(securePayResponse.UnHandledExceptionMessage))
                {
                    payOnlineResponse.FeesQuestionId = mCredentialApplicationService.GetFeeQuestionId();
                    payOnlineResponse.IsSecurePayStatusSuccess = false;
                    payOnlineResponse.ErrorMessage = securePayResponse.UnHandledExceptionMessage;
                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (CC last 4: {CreditCardLastFour})", 
                        request.ApplicationId, securePayResponse.UnHandledExceptionMessage, feeCreditCardDetails.CreditCardNumber.Tail(4));
                    return payOnlineResponse;
                }

                if (securePayResponse.PaymentStatus == PaymentProcessStatus.Failed)
                {
                    if (securePayResponse.PaymentFailureDetails != null && securePayResponse.PaymentFailureDetails.RejectedPayment)
                    {
                        payOnlineResponse.FeesQuestionId = mCredentialApplicationService.GetFeeQuestionId();
                        payOnlineResponse.IsSecurePayStatusSuccess = false;
                        payOnlineResponse.ErrorMessage = $"Your payment was declined with: {securePayResponse.PaymentFailureDetails.RejectionDescription}.";
                        LoggingHelper.LogWarning("APP{ApplicationId}: Payment was declined with {RejectionCode}: {RejectionDescription}. CC last 4: {CreditCardLastFour}",
                            request.ApplicationId, securePayResponse.PaymentFailureDetails.RejectionCode, securePayResponse.PaymentFailureDetails.RejectionDescription,
                            feeCreditCardDetails.CreditCardNumber.Tail(4));
                        return payOnlineResponse;
                    }
                }

                if (securePayResponse.PaymentStatus == PaymentProcessStatus.Succeeded && !securePayResponse.Success)
                {
                    var errorText = $"An error occurred when processing your transaction after your card was charged. Please contact NAATI to ensure your payment was successfully lodged. Attempting to process your payment again may result in your card being billed multiple times. Please quote your application Id when contacting NAATI. Application Id: {request.ApplicationId}";
                    payOnlineResponse.IsSecurePayStatusSuccess = true;
                    payOnlineResponse.IsSecurePayResponseSuccess = false;
                    payOnlineResponse.OrderNumber = securePayResponse.OrderNumber;

                    Session["IsSecurePayStatusSuccess"] = true;
                    Session["IsSecurePayResponseSuccess"] = false;
                    Session["ReferenceNumber"] = securePayResponse.ReferenceNumber;
                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (CC last 4: {CreditCardLastFour})", 
                        request.ApplicationId, errorText, feeCreditCardDetails.CreditCardNumber.Tail(4));
                    return payOnlineResponse;
                }

                //Secure payment is successfully done
                LoggingHelper.LogInfo("SecurePay payment accepted for APP{ApplicationId}. Amount: {PaymentAmount}", request.ApplicationId, feeCreditCardDetails.Total);
                payOnlineResponse.IsSecurePayStatusSuccess = true;
                payOnlineResponse.IsSecurePayResponseSuccess = securePayResponse.Success;
                payOnlineResponse.SecurePayReference = securePayResponse.ReferenceNumber;
                Session["IsSecurePayStatusSuccess"] = true;
                Session["IsSecurePayResponseSuccess"] = true;
                Session["ReferenceNumber"] = securePayResponse.ReferenceNumber;

            }
            catch (Exception ex)
            {
                try
                {
                    if (String.IsNullOrEmpty(payOnlineResponse.ErrorMessage))
                    {
                        payOnlineResponse.ErrorMessage = "An unknown error has occurred.";
                    }
                    LoggingHelper.LogException(ex, "APP{ApplicationId}: Unhandled exception during or after secure payment. Payment succes: {SecurePaySuccess}. Message: {Message}",
                        request.ApplicationId, payOnlineResponse.IsSecurePayStatusSuccess, payOnlineResponse.ErrorMessage);
                }
                catch { }
            }

            return payOnlineResponse;

        }

        private PaymentResponse MakeSecurePayment(int naatiNumber, int applicationId, FeePaymentDetails feeCreditCardDetails)
        {
            var cardDetails = new EnteredCardDetails
            {
                NameOnCard = feeCreditCardDetails.CreditCardName,
                Type = feeCreditCardDetails.CreditCardType == 1 ? CardType.MasterCard : CardType.Visa,
                CardNumber = feeCreditCardDetails.CreditCardNumber,
                CardVerificationValue = feeCreditCardDetails.CreditCardCCV,
                ExpiryMonth = new DateTime(feeCreditCardDetails.CreditCardExpiryYear, feeCreditCardDetails.CreditCardExpiryMonth, 1)
            };

            var request = new PaymentRequest
            {
                ApplicationId = applicationId.ToString(),
                NAATINumber = naatiNumber.ToString(),
                Amount = feeCreditCardDetails.Total,
                CardDetails = cardDetails,
                PaymentDate = DateTime.Now
            };

            var securePayResponse = mOrderService.SubmitCreatePayment(request);
            return securePayResponse;
        }

        private PaymentResponse MakeSecurePaymentEmbedded(int naatiNumber, int applicationId, FeePaymentDetails feeCreditCardDetails)
        {
            var cardDetails = new EnteredCardDetails
            {
                CardToken = feeCreditCardDetails.CreditCardToken,
                Type = feeCreditCardDetails.CreditCardType == 1 ? CardType.MasterCard : CardType.Visa,
            };

            var request = new PaymentRequest
            {
                ApplicationId = applicationId.ToString(),
                NAATINumber = naatiNumber.ToString(),
                Amount = feeCreditCardDetails.Total,
                CardDetails = cardDetails,
                PaymentDate = DateTime.Now,
            };

            var securePayResponse = mOrderService.SubmitCreatePayment(request);
            return securePayResponse;
        }

        private void SubmitSamApplication(int applicationId)
        {
            var reference = GetInvoiceReference(applicationId);
            var obj = new NcmsApplicationActionRequest { ApplicationId = applicationId, ActionId = (int)SystemActionTypeName.SubmitApplication, Steps = new List<object>(), DueDate = DateTime.Now.AddDays(3), InvoiceReference = reference };

            var response = _ncmsIntegrationService.ExecuteNcmsApplicationAction(obj);
            if (!response.Success)
            {
                throw new Exception(string.Concat(",", response.Errors));
            }
        }

        private string GetInvoiceReference(int applicationId)
        {
            var request = new GetOfficeAbbrAndEftMachineTermianlRequest
            {
                OnlineOfficeId = mLookupProvider.SystemValues.OnlineOfficeId,
                OnlineEftMachineId = mLookupProvider.SystemValues.OnlineEFTMachineId
            };

            var response = mAccountingService.GetOnlineofficeAbbrAndEftMachineTerminal(request);
            var reference = response.OnlineOfficeAbbr + " - APP" + applicationId;
            return reference;
        }

        private void SubmitSamApplicationWithCreditCard(int applicationId, string paymentReference, decimal paymentAmount, string orderNumber)
        {
            var reference = GetInvoiceReference(applicationId);
            var obj = new NcmsApplicationActionRequest { ApplicationId = applicationId, ActionId = (int)SystemActionTypeName.SubmitApplication, Steps = new List<object>(), DueDate = DateTime.Now.AddDays(3), InvoiceReference = reference, PaymentReference = paymentReference, PaymentAmount = paymentAmount, OrderNumber = orderNumber };

            var response = _ncmsIntegrationService.ExecuteNcmsApplicationAction(obj);
            if (!response.Success)
            {
                throw new Exception(string.Concat(",", response.Errors));
            }
        }

        private IList<string> ValidateSubmitApplicationWithCreditCard(int applicationId, string paymentReference, string reference, decimal paymentAmount)
        {
            var obj = new NcmsApplicationActionRequest { ApplicationId = applicationId, ActionId = (int)SystemActionTypeName.SubmitApplication, Steps = new List<object>(), DueDate = DateTime.Now.AddDays(3), InvoiceReference = reference, PaymentReference = paymentReference, PaymentAmount = paymentAmount };
            return _ncmsIntegrationService.ValidateCredentialApplication(obj);
        }

        private void DeleteSamApplication(int applicationId)
        {
            // LoggingHelper.LogWarning($"Application {applicationId} is going to be deleted", this.GetType());
            var obj = new NcmsApplicationActionRequest { ApplicationId = applicationId, ActionId = (int)SystemActionTypeName.DeleteApplication, Steps = new List<object>() };
            
            var response = _ncmsIntegrationService.ExecuteNcmsApplicationAction(obj);
            if (!response.Success)
            {
                throw new Exception(string.Concat(",", response.Errors));
            }
        }


        /// <summary>
        /// Delete an Application
        /// </summary>
        /// <param name="request">Sections, questions and answers</param>
        [HttpPost]
        // public ActionResult Save(SaveApplicationFormRequestModel request)
        public void Delete(SaveApplicationFormRequestModel request)
        {
            // LoggingHelper.LogWarning($"Application DELETE was called. Application Id: {request.ApplicationId} is going to be deleted", this.GetType());
            ValidateApplicationToken(request.ApplicationId, request.Token);
            DeleteSamApplication(request.ApplicationId);
        }

        /// <summary>
        /// Validations to Person Verification special control
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        public ActionResult PersonVerification(PersonVerificationModel request)
        {
            var verifyPersonRequest = _autoMapperHelper.Mapper.Map<VerifyPersonRequestContract>(request);

            var naatiNumber = CurrentUserNaatiNumber;
            if (naatiNumber > 0)
            {
                verifyPersonRequest.NaatiNumber = naatiNumber;
            }

            var personVerificationResponse = mCredentialApplicationService.UpdateOrVerifyPersonDetails(verifyPersonRequest, naatiNumber > 0);

            if (personVerificationResponse.IsNewPerson)
            {
                SendNewUserEmailNotification(request.FirstName, personVerificationResponse.NaatiNumber,
                    request.Email);
            }
            return
                Json(
                    new
                    {
                        personVerificationResponse.Message,
                        NAATINumber = personVerificationResponse.NaatiNumber,
                        personVerificationResponse.Token
                    });
        }

        private void SendNewUserEmailNotification(string givenName, int naatiNumber, string email)
        {
            //  Send an email to the new user
            var request = new SendEmailRequest(EmailTemplate.NewPersonCreated, email);

            request.Tokens.Add(EmailTokens.GivenName, givenName);
            request.Tokens.Add(EmailTokens.NaatiNumber, naatiNumber.ToString());
            mEmailService.SendMail(request);
        }

        [HttpPost]
        public ActionResult CreateCredentialApplication(SaveApplicationFormRequestModel request)
        {
            ValidatePersonToken(request.NaatiNumber, request.Token);

            var createAppRequest = new CreateApplicationRequest
            {
                NaatiNumber = request.NaatiNumber,
                ApplicationFormId = request.ApplicationFormId,
                UserName = GetUserName()
            };

            var response = mCredentialApplicationService.CreateCredentialApplication(createAppRequest);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get in progress application 
        /// </summary>
        /// <param name="request">Customer Number and applicationform id</param>
        [HttpGet]
        public ActionResult CustomerApplication(GetCustomerApplicationModel request)
        {
            //LoggingHelper.LogInfo($"CustomerApplication details called for application Form Id:{request.ApplicationFormId} naatiNumber :{request.NAATINumber} and Token: {request.Token}", this.GetType());
            ValidatePersonToken(request.NAATINumber, request.Token);
            var requestContract = new ExitingApplicationRequestContract
            {
                ApplicationFormId = request.ApplicationFormId,
                NaatiNumber = request.NAATINumber,
                ExternalUrls = GetExternalUrls(),
            };

            var response = mCredentialApplicationService.GetExistingDraftApplication(requestContract);
            if (response.CredentialApplication != null)
            {
                DeleteSamApplication(response.CredentialApplication.Id);
            }

            var result = new { response.Message, Application = new CredentialApplicationModel() };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get in customer details
        /// </summary>
        /// <param name="naatiNumber">Customer No</param>
        /// <param name="token">person token</param>
        [HttpGet]
        public ActionResult CustomerDetails(int? naatiNumber, int token)
        {
            ValidatePersonToken(naatiNumber.GetValueOrDefault(), token);

            var response = mCredentialApplicationService.GetPersonDetails(naatiNumber.GetValueOrDefault());

            var details = new CustomerDetailsModel
            {

                NAATINumber = response.NaatiNumber,
                PhoneNumber = response.PhoneNumber
            };

            if (response.Address != null)
            {
                details.Address = new AddressModel
                {
                    StreetDetails = response.Address.StreetDetails,
                    IsFromAustralia = response.Address.IsFromAustralia,
                    CountryName = response.Address.CountryName,
                    SuburbName = response.Address.SuburbName,
                    Postcode = response.Address.Postcode,
                    State = response.Address.State,
                    Latitude = response.Address.Latitude,
                    Longitude = response.Address.Longitude,
                    Errors = new List<string>(),
                    ValidateInExternalTool = response.Address.ValidateInExternalTool,
                    PostCodeId = response.Address.PostCodeId?.ToString()
                    
                };
            }

            return Json(details, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Countries()
        {
            return Json(mLookupProvider.Countries, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult PersonTitles()
        {
            return Json(mLookupProvider.PersonTitles, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Postcodes(string term)
        {
            var postcodes = mLookupProvider.Postcodes.Where(p => p.DisplayText.ToLower().Contains(term?.ToLower()));
            return Json(postcodes.GroupBy(x => new { x.Code, x.DisplayText }).Select(x => new { x.Key.Code, x.Key.DisplayText, x.First().SamId, x.First().State, x.First().Suburb, x.First().SuburbId }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Languages(LookupsRequestModel request)
        {
            var languages = mCredentialApplicationService.GetLanguagesForApplicationForm(request.ApplicationFormId, request.ApplicationId, request.NAATINumber);
            var languagesModels = languages.Results.Select(x => new { x.Id, Name = x.DisplayName }).ToList();
            return Json(languagesModels, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EndorsedQualification(EndorsedQualificationLookupRequestModel request)
        {
            var qualifications = mCredentialApplicationService.GetEndorsedQualificationForApplicationForm(request.ApplicationFormId, request.ApplicationId, request.NAATINumber);
            var qualificationModels = qualifications.Results
                .Where(x => x.Location == request.Location && x.InstitutionId == request.InstitutionId)
                .Select(x => new { Id = x.EndorsedQualificationId, Name = $"{x.Qualification} - {x.CredentialTypeExternalName}"  })
                .ToList();

            return Json(qualificationModels, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EndorsedQualificationInstitution(LookupsRequestModel request)
        {
            var qualifications = mCredentialApplicationService.GetEndorsedQualificationForApplicationForm(request.ApplicationFormId, request.ApplicationId, request.NAATINumber);
            var qualificationModels = qualifications.Results.Select(x => new { Id = x.InstitutionId, Name = x.Institution }).OrderBy(x => x.Name).GroupBy(x=> x.Id).Select(y=> y.First()).ToList();
            return Json(qualificationModels, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EndorsedQualificationLocation(EndorsedQualificationLookupRequestModel request)
        {
            var qualifications = mCredentialApplicationService.GetEndorsedQualificationForApplicationForm(request.ApplicationFormId, request.ApplicationId, request.NAATINumber);
            var qualificationModels = qualifications.Results
                .Where(x => x.InstitutionId == request.InstitutionId)
                .Select(x => new { Id = x.Location, Name = x.Location })
                .Distinct()
                .ToList();
            return Json(qualificationModels, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Locations(LookupsRequestModel request)
        {
            var response = mCredentialApplicationService.GetLocations(request.ApplicationFormId);
            var locations = response.Results.Select(x => new { x.Id, Name = x.DisplayName });

            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Documents(DocumentsRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var response = mCredentialApplicationService.GetAttachments(request.ApplicationId);

            foreach (var document in response.Results)
            {
                mCredentialApplicationService.DeleteAttachment(document.Id);
            }

            //  var list = response.Results.Select(Mapper.DynamicMap<DocumentModel>);

            return Json(Enumerable.Empty<DocumentModel>(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete a document from an application form
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void DeleteDocument(DeleteRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            mCredentialApplicationService.DeleteAttachment(request.DocumentId);
        }

        [HttpGet]
        public ActionResult DocumentTypes(DocumentsRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var documentTypesRequest = new GetDocumentTypesRequestContract { ApplicationId = request.ApplicationId, ApplicationFormId = request.ApplicationFormId, ExternalUrls = GetExternalUrls(), NaatiNumber = CurrentUserNaatiNumber };
            var response = mCredentialApplicationService.GetDocumentTypes(documentTypesRequest);
            return Json(response.Results, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get credentials added on this application form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Credentials(CredentialRequestRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var response = mCredentialApplicationService.GetCredentialRequests(request.ApplicationId);
            var requests = response.Results.Select(_autoMapperHelper.Mapper.Map<CredentialRequestModel>);
            return Json(requests, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get categories available to this appliacation form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Categories(CredentialRequestRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var response = mCredentialApplicationService.GetCredentialCategoriesForApplication(request.ApplicationId, request.ApplicationFormId);
            return Json(response.Results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AvailableCredentials(CredentialRequestRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var response = mCredentialApplicationService.GetAvailableCredentials(request.ApplicationId, request.NAATINumber, request.ApplicationFormId);
            return Json(response.Results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AdditionalSkills(CredentialRequestRequestModel request)
        {
            var credentialTypes = request.CredentialTypes.Split(',').Select(c => int.Parse(c));
            var response = mCredentialApplicationService.GetAdditionalSkills(new GetSkillsForApplicationForCredentialTypeRequest
            {
                CredentialTypeIds = credentialTypes,
                NAATINumber = request.NAATINumber,
                CredentialRequestPathTypeId = request.CredentialRequestPathTypeId,
                ApplicationId = request.ApplicationId
            });
            return Json(response.Results, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get levels available to these appliacation form / ncategory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Levels(CredentialRequestRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var response = mCredentialApplicationService.GetCredentialTypesForApplicationForm(request.ApplicationFormId, request.ApplicationId, request.CategoryId);
            return Json(response.Results, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get skills available to these appliacation form / category / level
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Skills(CredentialRequestRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var credentialTypes = request.CredentialTypes.Split(',').Select(c => int.Parse(c));
            var response = mCredentialApplicationService.GetSkillsForApplicationForCredentialType(new GetSkillsForApplicationForCredentialTypeRequest {
                CredentialTypeIds = credentialTypes,
                NAATINumber = request.NAATINumber,
                ApplicationId = request.ApplicationId
            });
            return Json(response.Results, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Push a credential to an application form
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Credential(CredentialRequestRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            var requestContract = _autoMapperHelper.Mapper.Map<CredentialRequestRequestContract>(request);
            requestContract.UserName = GetUserName();
            var response = mCredentialApplicationService.SaveCredentialRequest(requestContract);
            return Json(new { Id = response.CredentialRequestId, PathTypeId = response.CredentialRequestPathTypeId });
        }

        /// <summary>
        /// Delete a credential from an application form
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void DeleteCredential(DeleteRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);
            mCredentialApplicationService.DeleteCredentialRequest(new DeleteCredentialRequestContract
            {
                ApplicationId = request.ApplicationId,
                CredentialRequestId = request.Id,
                UserName = GetUserName()
            });
        }

        [HttpGet]
        public ActionResult TestSessions(TestSessionRequestRequestModel request)
        {
            ValidateApplicationToken(request.ApplicationId, request.Token);

            var response = mCredentialApplicationService.GetAvailableTestSessions(new GetAvailableTestSessionRequest
            {
                ApplicationId = request.ApplicationId,
            });


            var list = response.Results;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Fees(int applicationFormId)
        {
            var list = mCredentialApplicationService.GetApplicationTypeFees(applicationFormId);

            return Json(list.FeeProducts.Select(f =>
            {
                var total = 0d;
                var exGst = 0d;
                var gst = 0d;

                if (!f.ProductSpecification.GstApplies)
                {
                    exGst = Convert.ToDouble(f.ProductSpecification.CostPerUnit);
                    gst = exGst * .1;
                    total = exGst + gst;
                }
                else
                {
                    total = Convert.ToDouble(f.ProductSpecification.CostPerUnit);
                    exGst = total / 1.1;
                    gst = total - exGst;
                }

                return new
                {
                    Code = f.ProductSpecification.Code,
                    Description = f.ProductSpecification.Description,
                    ExGST = exGst,
                    GST = gst,
                    Total = total
                };
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            var ids = new List<int>();

            var random = new Random();
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
                {
                    continue;
                }

                var randomNumber = random.Next(10000, 99999);
                var fileName = Path.GetFileNameWithoutExtension(file.FileName) + randomNumber +
                               Path.GetExtension(file.FileName);

                var fileExtension = fileName.Split('.').Last();

                if (!ApplicationSettingsHelper.IncludedFileExtensionsList.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    throw new Exception($"Unsupported File Type {fileName}");
                }

                var filePath = ConfigurationManager.AppSettings["tempFilePath"] + '\\' + fileName;
                var title = Path.GetFileNameWithoutExtension(file.FileName);

                var typeId = Convert.ToInt32(Request["TypeId"]);
                var applicationId = Convert.ToInt32(Request["ApplicationId"]);
                var token = Convert.ToInt32(Request["Token"]);
                ValidateApplicationToken(applicationId, token);

                string path;

                using (var fileStream = System.IO.File.Create(filePath))
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    file.InputStream.CopyTo(fileStream);
                    path = fileStream.Name;
                }

                var request = new CreateOrReplaceAttachmentContract
                {
                    CredentialApplicationId = applicationId,
                    CredentialApplicationAttachmentId = 0,
                    DocumentType = string.Empty,
                    FileName = fileName,
                    FilePath = path,
                    FileSize = 0,
                    Title = title,
                    Type = typeId,
                    UserName = GetUserName(),
                    TokenToRemoveFromFilename = randomNumber.ToString()
                };

                ids.Add(mCredentialApplicationService.CreateOrReplaceAttachment(request).StoredFileId);
            }

            return Json(new { success = true, ids });
        }

        public class TestModel
        {
            public int NaatiNumber { get; set; }
            public int Token { get; set; }
        }

        [HttpGet]
        public ActionResult PersonPhoto(int naatiNumber, int token)
        {
            ValidatePersonToken(naatiNumber, token);
            var photoUrl = string.Empty;

            if (naatiNumber != 0 && mCredentialApplicationService.PersonHasPhoto(naatiNumber))
            {
                photoUrl = Url.Action(nameof(GetImageFile), nameof(CredentialApplicationController).Replace("Controller", string.Empty), new { NaatiNumber = naatiNumber, Token = token }, this.Request?.Url?.Scheme);
            }
            return Json(new { PhotoUrl = photoUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetImageFile(TestModel reqModel)
        {
            ValidatePersonToken(reqModel.NaatiNumber, reqModel.Token);
            var response = mCredentialApplicationService.GetPersonImage(new GetImageRequestContract { NaatiNumber = reqModel.NaatiNumber, TempFolderPath = ConfigurationManager.AppSettings["tempFilePath"] });

            byte[] photo = null;

            if (System.IO.File.Exists(response.FilePath))
            {
                photo = System.IO.File.ReadAllBytes(response.FilePath);
                System.IO.File.Delete(response.FilePath);
            }

            return File(photo, "image/jpg");
        }

        [HttpPost]
        public ActionResult PersonPhoto()
        {
            var random = new Random();
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
                {
                    continue;
                }
                var randomNumber = random.Next(10000, 99999);
                var fileName = Path.GetFileNameWithoutExtension(file.FileName) + randomNumber + Path.GetExtension(file.FileName);

                var naatiNumber = Convert.ToInt32(Request["naatiNumber"]);
                var token = Convert.ToInt32(Request["Token"]);
                ValidatePersonToken(naatiNumber, token);

                string path;
                using (var fileStream = System.IO.File.Create(ConfigurationManager.AppSettings["tempFilePath"] + '\\' + fileName))
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    file.InputStream.CopyTo(fileStream);
                    path = fileStream.Name;
                }

                mCredentialApplicationService.UpdatePersonPhoto(new UpdatePhotoRequestContract
                {
                    FilePath = path,
                    NaatiNumber = naatiNumber,
                    TokenToRemoveFromFilename = randomNumber.ToString()
                });
            }

            return Json(new { success = true });
		}

		[HttpGet]
		public ActionResult PdPointsMet(int applicationId)
		{
		    var response = mCredentialApplicationService.GetRecertficationPdPointsStatus(new PdPointsStatusRequest
		    {
		        ApplicationId = applicationId,
		        NaatiNumber = CurrentUserNaatiNumber
		    });
		    return Json(response, JsonRequestBehavior.AllowGet);
        }

		[HttpGet]
		public ActionResult GetRecertificaitonOptions()
		{
		    var response = mCredentialApplicationService.GetRecertificationOptions(CurrentUserNaatiNumber);
			var list = response.Options.Select(_autoMapperHelper.Mapper.Map<ApplicationFormCredentialAnswerModel>);
			return Json(list, JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        public ActionResult WorkPracticeMet(int applicationId)
        {
            var response = mCredentialApplicationService.GetRecertficationtWorkPracticeStatus(new WorkPracticeStatusRequest
            {
                ApplicationId = applicationId,
                NaatiNumber = CurrentUserNaatiNumber
            });
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
		public ActionResult ReplaceTokens(int questionId, int applicationFormId)
		{
		
		    var question = mCredentialApplicationService.ReplaceQuestionFormTokens(
		        new ReplaceFormTokenRequest {ApplicationFormId = applicationFormId, QuestionId = questionId, NaatiNumber = CurrentUserNaatiNumber, ExternalUrls = GetExternalUrls(), });
		    var result = _autoMapperHelper.Mapper.Map<ApplicationFormQuestionModel>(question);

            return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult CredentialRequestLimit()
		{
			return Json(mLookupProvider.SystemValues.CredentialRequestLimit, JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        public ActionResult GetActiveAndFutureCredentials(int naatiNumber, int token)
        {
            ValidatePersonToken(naatiNumber, token);
            var result = mCredentialApplicationService.GetActiveAndFutureCredentials(naatiNumber).Credentials;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetInProgressCredentialsRequests(int naatiNumber, int token)
        {
            ValidatePersonToken(naatiNumber, token);
            var result = mCredentialApplicationService.GetInProgressCredentialRequests(naatiNumber).Results;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AvailableCreditCards()
        {
            const string creditCardOptionPrefix = "AllowPaymentBy";
            var variables = mSystemValueService.GetAll();
            var creditCardOptions = variables.Where(r => r.Key.StartsWith(creditCardOptionPrefix) && r.Value == "true").Select(r => r.Key.Replace(creditCardOptionPrefix, String.Empty).ToLowerInvariant());
            return Json(creditCardOptions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPaymentConfiguration()
        {
            var paymentGatewayClientId = mSecretsProvider.Get(SecuritySettings.SecurePayClientId);
            var paymentGatewayMerchantCode = mSecretsProvider.Get(SecuritySettings.SecurePayMerchantCode);

            return Json(new
            {
                PaymentGatewayClientId = paymentGatewayClientId,
                PaymentGatewayMerchantCode = paymentGatewayMerchantCode
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetUserName()
        {
            return mSecretsProvider.Get(SecuritySettings.MyNaatiDefaultIdentityKey);
        }

        private string VerifyUiModelIsCorrect(SaveApplicationFormRequestContract request)
        {
            try
            {
                //start with verifying that Austrlain Addresses have a postcode
                var personalDetails = request.Sections.FirstOrDefault(x => x.Name == "Your Details");
                if (personalDetails == null)
                {
                    return null;
                }

                var addressDetails = personalDetails.Questions.FirstOrDefault(x => x.AnswerTypeName == "PersonDetails");
                if (addressDetails == null || addressDetails.Response == null)
                {
                    return null;
                }

                VerifyUIModel address;
                try
                {
                    address = JsonConvert.DeserializeObject<VerifyUIModel>(addressDetails.Response.ToString());
                }
                catch (Exception)
                {
                    var error = $"Could not read Personal Details: {addressDetails.Response}";
                    LoggingHelper.LogError(error);
                    return null;
                }

                if (address.Address == null)
                {
                    return null;
                }

                //If an Australian Address then must have Postcode. Never replicated but it does happen
                if ((address.Address.CountryName == "Australia" || address.Address.CountryId == 13) && (!address.Address.Postcode.HasValue))
                {
                    var error = $"Australian Addresses require a Suburb and Postcode: {addressDetails.Response}";
                    LoggingHelper.LogError(error);

                    return "Australian Addresses require a Suburb and Postcode";
                }
                return null;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }

    public class VerifyUIModel
    {
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
    }
    public class Address
    {
        public string StreetDetails { get; set; }
        public string CountryName { get; set; }
        public int? CountryId { get; set; }
        public bool IsFromAustralia { get; set; }
        public string SuburbName { get; set; }
        public int? PostCodeId { get; set; }
        public int? Postcode { get; set; }
        public string State { get; set; }
        public bool ValidateInExternalTool { get; set; }
        public bool Validated { get; set; }
        public string FormattedAddress { get; set; }
        public bool InvalidNumber { get; set; }
        public bool InvalidSuburb { get; set; }
        public bool ShowGoogleSuburbDropDown { get; set; }
    }


}
