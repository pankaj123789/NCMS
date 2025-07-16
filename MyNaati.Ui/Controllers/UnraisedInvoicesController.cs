using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.UI;
using MyNaati.Ui.ViewModels.Bills;
using MyNaati.Ui.ViewModels.CredentialApplication;
using MyNaati.Ui.ViewModels.UnraisedInvoices;
using Newtonsoft.Json;
using PayPal.Api;
using static MyNaati.Contracts.Portal.PayPal.OrderSubmission;

namespace MyNaati.Ui.Controllers
{

    [AttributeUsage(AttributeTargets.All)]
    public class DisplayInvoices : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var naatiNumber = Thread.CurrentPrincipal.NaatiNumber();
            var service = ServiceLocator.Resolve<ICredentialApplicationService>();

            if (!service.IsDisplayInvoices(naatiNumber))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action","Unauthorized"},
                    {"controller","Errors"}
                });
            }
        }
    }

    [DisplayInvoices]
    [Authorize]
    public class UnraisedInvoicesController : NewtonsoftController
    {
        private readonly IAccountingService mAccountingService;
        private readonly IPayPalService mPayPalService;
        private readonly ICredentialApplicationService mCredentialApplicationService;
        private readonly IOrderService mOrderService;
        private readonly ILookupProvider mLookupProvider;
        private readonly ISystemValueService mSystemValueService;
        private readonly ISecretsCacheQueryService mSecretsProvider;
        private readonly IUnraisedInvoiceService mUnraisedInvoicesService;
        private readonly INcmsIntegrationService _ncmsIntegrationService;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IApplicationQueryService _applicationQueryService;


        private const string RequestCancelled = @"
<h2 class=""text-warning""><strong>Payment Cancelled</strong></h2>
<br/>
<p><a href = ""{0}"">Click here</a> to return to the invoices page</p>";

        public UnraisedInvoicesController(IAccountingService accountingService, ICredentialApplicationService credentialApplicationService, IUnraisedInvoiceService unraisedInvoiceService, IOrderService orderService, ILookupProvider lookupProvider, ISystemValueService systemValueService, ISecretsCacheQueryService secretsProvider, INcmsIntegrationService ncmsIntegrationService, IAutoMapperHelper autoMapperHelper, IPayPalService payPalService,
            IApplicationQueryService applicationQueryService)
        {
            mAccountingService = accountingService;
            mCredentialApplicationService = credentialApplicationService;
            mOrderService = orderService;
            mLookupProvider = lookupProvider;
            mSystemValueService = systemValueService;
            mSecretsProvider = secretsProvider;
            mPayPalService = payPalService;
            mUnraisedInvoicesService = unraisedInvoiceService;
            _ncmsIntegrationService = ncmsIntegrationService;
            _autoMapperHelper = autoMapperHelper;
            _applicationQueryService = applicationQueryService;
        }

        private const string ConfirmationContentCreditCard = @"
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
        <li>Your application will be checked. Once approved you will receive an email confirmation.</li>
    </ol>
</ul>";

        private const string ConfirmationContentPayPal = @"
<h2 style=""color:#008C7E;""><strong>Application Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>Also note your paypal transaction reference: <strong>[[PayPal Reference]]</strong> </p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ol>
        <li>Your payment will be processed by NAATI immediately and you will then receive a second email containing your invoice within 24 hours.</li>
        <li>Your application is then added to the processing queue for NAATI to check. This can take 1 week or more.</li>
        <li>Your application will be checked. Once approved you will receive an email confirmation.</li>
    </ol>
</ul>";

        [HttpGet]
        [Authorize]
        [Route("Invoices")]
        public ActionResult Index()
        {
            ViewBag.Access = true;
            return View();
        }

        [Authorize]
        public ActionResult GetUnraisedInvoices()
        {
            var invoices = GetAllInvoiceRecords();
            return Json(invoices, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RaiseAndPayInvoice(int applicationId)
        {
            var payPalClientId = mSecretsProvider.Get(SecuritySettings.PayPalClientId);
            ViewBag.PayPalClientId = $"https://www.paypal.com/sdk/js?client-id={payPalClientId}&currency=AUD";
            var request = new RaiseAndPayInvoiceRequest()
            {
                CredentialApplicationId = applicationId                
            };
            return View(request);
        }

        [HttpPost]
        [Route("unraisedinvoices/raiseandpayinvoice/nextquestion")]
        public ActionResult SelectRaiseAndPayInvoiceNextQuestion(NextSelectTestSessionQuestionRequest request)
        {
            var result = mUnraisedInvoicesService.GetUnraisedInvoiceSections(request.CredentialApplicationId);
            return GetNextQuestion(request, result);
        }

        [HttpGet]
        [Route("unraisedinvoices/raiseandpayinvoice/settings")]
        public ActionResult SelectRaiseAndPayInvoice(int credentialApplicationId)
        {
            var result = mUnraisedInvoicesService.GetUnraisedInvoiceSections(credentialApplicationId);

            var sections = SectionMapper.Map(result.Results.ToList());

            foreach (var s in sections)
            {
                s.Questions.Clear();
            }
            return Json(new
            {
                Sections = sections,
                DisablePayPalUi = mLookupProvider.SystemValues.DisablePayPalUi,
                //Skill = application.CredentialRequests.FirstOrDefault(x => x.Id == credentialRequestId).Skill.DisplayName,
            }, JsonRequestBehavior.AllowGet); ;
        }

        [HttpGet]
        [Route("unraisedinvoices/raiseandpayinvoice/paymentcontrol")]
        public ActionResult SelectInvoicePaymentControl(int credentialApplicationId)
        {
            var payPalSurcharge = double.Parse(mSystemValueService.GetAll().First(r => r.Key.Equals("PayPalSurcharge")).Value);

            var invoices = GetAllInvoiceRecords();

            var fees = new List<FeeContract>();

            foreach (var invoice in invoices.Data)
            {
                if (invoice.CredentialApplicationId == credentialApplicationId)
                {
                    var amount = Double.Parse(invoice.AmountDue.ToString());
                    var gst = invoice.GST.Value ? amount * .1 : 0;

                    fees.Add(new FeeContract()
                    {
                        Code = invoice.Code,
                        Description = invoice.CredentialApplicationTypeDisplayName,
                        Total = amount,
                        ExGST = amount - gst,
                        GST = gst,
                        PayPalSurcharge = payPalSurcharge
                    });
                }
            }
            //logic below for passing the sponsor object to the view model for rendering the additional line item in case the application is sponsored by NAATI.
            object sponsorJson;
            var application = _applicationQueryService.GetApplication(credentialApplicationId).Result;
            bool isNaatiFunded = application.SponsorInstitutionNaatiNumber == 950079;
            if (isNaatiFunded)
            {
                sponsorJson = new
                {
                    OrganisationName = application.SponsorInstitutionName,
                    Trusted = false
                };
            }
            else
            {
                sponsorJson = false;
            }
            return Json(new { Sponsor = sponsorJson, Fees = fees }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Route("unraisedinvoices/raiseandpayinvoice/applyembedded")]
        public ActionResult RaiseAndPayinvoiceSubmitEmbedded(NextSelectTestSessionQuestionRequest request)
        {
            var payOnlineResponse = new PayOnlineResponseModel();

            var questions = request.Form.Sections.SelectMany(x => x.Questions).ToList();
            var acceptedTermAndConditions = Convert.ToInt32(questions.Last().Response) == 1;
            if (!acceptedTermAndConditions)
            {

                var content = string.Format(RequestCancelled, Url.Action("MyInvoices"));
                var response = new SubmitApplicationResponse
                {
                    SaveApplicationFormResponse = new SaveApplicationFormResponse
                    {
                        ApplicationId = 0,
                        ApplicationReference = string.Empty,
                    },
                    PayOnlineResponse = payOnlineResponse,
                    ConfirmContent = content
                };

                return Json(response, JsonRequestBehavior.AllowGet);

            }

            //TODO needs to be a check on the CredentialApplication, not credentialrequest. Unless we get access to the Cred Req Id
            //if (!mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, request.CredentialRequestId))
            //{
            //    return Json(new { data = "Invalid User" }, JsonRequestBehavior.AllowGet);
            //}

            var unraisedInvoicesContract = GetUnraisedInvoicesContract(request.CredentialApplicationId);

            unraisedInvoicesContract.HasSponsor = false;
            unraisedInvoicesContract.ApplicationId = request.CredentialApplicationId;
            unraisedInvoicesContract.CredentialRequestId = request.CredentialRequestId;

            payOnlineResponse = SubmitPaymentEmbedded(request, unraisedInvoicesContract,payOnlineResponse, SystemActionTypeName.MakeApplicationPayment);

            string confirmationContent = GetConfirmationContent(unraisedInvoicesContract, payOnlineResponse, SystemActionTypeName.MakeApplicationPayment);

            var submitApplicationResponse = new SubmitApplicationResponse
            {
                SaveApplicationFormResponse = new SaveApplicationFormResponse
                {
                    ApplicationId = request.CredentialApplicationId,
                    ApplicationReference = request.CredentialApplicationId.ToString(),
                },
                PayOnlineResponse = payOnlineResponse,
                ConfirmContent = confirmationContent
            };

            return Json(submitApplicationResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("unraisedinvoices/raiseandpayinvoice/information")]
        public ActionResult Information(int credentialApplicationId)
        {
            return Json(GetUnraisedInvoicesContract(credentialApplicationId), JsonRequestBehavior.AllowGet);
        }

        private string GetConfirmationContent(UnraisedInvoicesContract unraisedInvoicesContract, PayOnlineResponseModel payOnlineResponse, SystemActionTypeName action)
        {
            ISponsoredPaymentConfirmationMessages confirmationMessages;

            confirmationMessages = new MakeApplicationConfirmationMessages();

            var confirmationContent = payOnlineResponse.IsException ? confirmationMessages.Error : payOnlineResponse.IsPayByCreditCard ? ConfirmationContentCreditCard :
                ConfirmationContentPayPal;

            return ReplaceTokens(confirmationContent, unraisedInvoicesContract, payOnlineResponse);
        }

        private string ReplaceTokens(string confirmationContent, UnraisedInvoicesContract unraisedInvoicesContract, PayOnlineResponseModel payOnlineResponse)
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
                        tokenValue = unraisedInvoicesContract.ApplicationReference;
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
                        tokenValue = unraisedInvoicesContract.NaatiNumber.ToString();
                        break;
                    case ConfimationContentTokens.PrimaryEmail:
                        tokenValue = unraisedInvoicesContract.ApplicantPrimaryEmail;
                        break;
                    case ConfimationContentTokens.GivenName:
                        tokenValue = unraisedInvoicesContract.ApplicantGivenName;
                        break;
                    case ConfimationContentTokens.FamilyName:
                        tokenValue = unraisedInvoicesContract.ApplicantFamilyName;
                        break;
                    case ConfimationContentTokens.ErrorMessage:
                        tokenValue = payOnlineResponse.ErrorMessage;
                        break;
                }

                confirmationContent = confirmationContent.Replace($"[[{token}]]", tokenValue ?? string.Empty);
            }

            return confirmationContent.Replace("[[Base Url]]", Url.Content("~/"));
        }

        private GetAccountingInvoicesResponse GetAllInvoiceRecords()
        {

            var invoices = mAccountingService.GetUnraisedInvoices(CurrentUserNaatiNumber);

            return invoices;
        }

        private PayOnlineResponseModel SubmitPaymentEmbedded(NextSelectTestSessionQuestionRequest request, UnraisedInvoicesContract unraisedInvoices, PayOnlineResponseModel payOnlineResponse, SystemActionTypeName action)
        {


            var feeQuestion = request.Form.Sections.SelectMany(x => x.Questions)
                            .FirstOrDefault(x => x.Type == (int)CredentialApplicationFormAnswerTypeName.PaymentControl);

            if (feeQuestion != null)
            {
                //which credentialrequest to use? For the moment just use the First()
                var credRequests = mCredentialApplicationService.GetCredentialRequests(request.CredentialApplicationId);
                var firstCredRequest = credRequests.Results.First();

                var answerResponse = feeQuestion.Response;
                var jsonSerializer = new JavaScriptSerializer();
                if (answerResponse != null)
                {
                    var feePaymentDetails = jsonSerializer.Deserialize<FeePaymentDetails>(answerResponse.ToString());
                    var errors = new List<string>();
                    switch (feePaymentDetails.PaymentMethodType)
                    {
                        case (int)PaymentMethodType.CreditCard:
                            payOnlineResponse = PayCreditCard(CurrentUserNaatiNumber, request.CredentialApplicationId, feePaymentDetails, feeQuestion.Id);
                            payOnlineResponse.IsPayByCreditCard = true;
                            if (payOnlineResponse.IsSecurePayStatusSuccess)
                            {
                                ExecuteAction(unraisedInvoices.ApplicationId, firstCredRequest.Id, payOnlineResponse.SecurePayReference, feePaymentDetails.Total, 0, null, null, payOnlineResponse.OrderNumber, payOnlineResponse.SecurePayReference, action, null, null);
                            }
                            break;
                        case (int)PaymentMethodType.PayPal:
                            payOnlineResponse = PayByPayPal(CurrentUserNaatiNumber, request.CredentialApplicationId, feePaymentDetails, feeQuestion.Id);
                            payOnlineResponse.IsPayByPayPal = true;
                            if (payOnlineResponse.IsPayPalStatusSuccess)
                            {
                                ExecuteAction(request.CredentialApplicationId, firstCredRequest.Id, payOnlineResponse.PayPalReference, feePaymentDetails.Total, 0, null, null, payOnlineResponse.OrderNumber, payOnlineResponse.SecurePayReference, action, null, null);
                            }
                            else
                            {
                                return payOnlineResponse;
                            }
                            break;
                        default:
                            errors = ValidateAction(unraisedInvoices.ApplicationId, unraisedInvoices.CredentialRequestId, string.Empty, string.Empty, feePaymentDetails.Total,  action).ToList();
                            if (errors.Any())
                            {
                                payOnlineResponse.ErrorMessage = string.Join(";", errors);
                                payOnlineResponse.IsException = true;
                                return payOnlineResponse;

                            }
                            ExecuteAction(unraisedInvoices.ApplicationId, unraisedInvoices.CredentialRequestId, null, 0, null, null, null, null, null, action, null, null);
                            break;
                    }

                }
            }
            else
            {
                var errors = ValidateAction(unraisedInvoices.ApplicationId, unraisedInvoices.CredentialRequestId, string.Empty, string.Empty, 0, action);
                if (errors.Any())
                {
                    payOnlineResponse.ErrorMessage = string.Join(";", errors);
                    payOnlineResponse.IsException = true;
                    return payOnlineResponse;

                }
                ExecuteAction(unraisedInvoices.ApplicationId, unraisedInvoices.CredentialRequestId, null, 0,  null, null, null, null, null, action, null, null);
            }

            return payOnlineResponse;
        }

        private PayOnlineResponseModel PayByPayPal(int naatiNumber, int applicationId, FeePaymentDetails feePaymentDetails, int questionId)
        {
            var payOnlineResponse = new PayOnlineResponseModel();

            try
            {
                var payPalResponse = MakePayPalPaymentEmbedded(naatiNumber, applicationId, feePaymentDetails);

                if (payPalResponse.PaymentFailureDetails?.SystemError == true)
                {
                    payOnlineResponse.FeesQuestionId = questionId;
                    payOnlineResponse.IsPayPalStatusSuccess = false;
                    payOnlineResponse.IsPayPalResponseSuccess = false;
                    payOnlineResponse.ErrorMessage = "Your payment processed, however an error occurred";
                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (PayerId: {feePaymentDetails.PayPalModel.PayerId})",
                        applicationId, payPalResponse.PaymentFailureDetails.SystemErrorMessage, feePaymentDetails.PayPalModel.PayerId);
                    return payOnlineResponse;
                }

                if (payPalResponse.PaymentStatus == PaymentProcessStatus.Failed)
                {
                    if (payPalResponse.PaymentFailureDetails != null && payPalResponse.PaymentFailureDetails.RejectedPayment)
                    {
                        payOnlineResponse.FeesQuestionId = questionId;
                        payOnlineResponse.IsPayPalStatusSuccess = false;
                        payOnlineResponse.ErrorMessage = $"Your payment was declined with: {payPalResponse.PaymentFailureDetails.RejectionDescription}.";
                        LoggingHelper.LogWarning("APP{ApplicationId}: Payment was declined with {RejectionCode}: {RejectionDescription}. PayerId: {feePaymentDetails.PayPalModel.PayerId}",
                            applicationId, payPalResponse.PaymentFailureDetails.RejectionCode, payPalResponse.PaymentFailureDetails.RejectionDescription,
                            feePaymentDetails);
                        return payOnlineResponse;
                    }
                }
                if (payPalResponse.PaymentStatus == PaymentProcessStatus.Succeeded)
                {
                    payOnlineResponse.OrderNumber = payPalResponse.OrderNumber;
                    payOnlineResponse.PayPalReference = payPalResponse.ReferenceNumber;
                }
                if (payPalResponse.PaymentStatus == PaymentProcessStatus.Succeeded && !payPalResponse.Success)
                {
                    var errorText = $"An error occurred when processing your transaction after your PayPal account was charged. Please contact NAATI to ensure your payment was successfully lodged. Attempting to process your payment again may result in your account being billed multiple times. Please quote your application Id when contacting NAATI. Application Id: {applicationId}";
                    payOnlineResponse.IsPayPalStatusSuccess = false;
                    payOnlineResponse.IsPayPalResponseSuccess = false;

                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (Payer ID: {feePaymentDetails.PayPalModel.PayerId})",
                        applicationId, errorText, feePaymentDetails);
                    return payOnlineResponse;
                }

                //PayPal payment is successfully done
                LoggingHelper.LogInfo("PayPal payment accepted for APP{ApplicationId}. Amount: {PaymentAmount}", applicationId, feePaymentDetails.Total);
                payOnlineResponse.IsPayPalStatusSuccess = true;
                payOnlineResponse.IsPayPalResponseSuccess = payPalResponse.Success;

            }
            catch (Exception ex)
            {
                try
                {
                    if (String.IsNullOrEmpty(payOnlineResponse.ErrorMessage))
                    {
                        payOnlineResponse.ErrorMessage = "An unknown error has occurred.";
                    }
                    LoggingHelper.LogException(ex, "APP{ApplicationId}: Unhandled exception during or after secure payment. Payment success: {SecurePaySuccess}. Message: {Message}",
                        applicationId, payOnlineResponse.IsSecurePayStatusSuccess, payOnlineResponse.ErrorMessage);
                }
                catch { }
            }

            return payOnlineResponse;

        }

        private PaymentResponse MakePayPalPaymentEmbedded(int naatiNumber, int applicationId, FeePaymentDetails feeCreditCardDetails)
        {
            try
            {
                var apiContext = PayPalConfigurationHelper.GetAPIContext();
                var executedPayment = mPayPalService.ExecutePayment(apiContext, feeCreditCardDetails.PayPalModel.PayerId, feeCreditCardDetails.PayPalModel.PaymentId);

                if (mLookupProvider.SystemValues.ThrowPayPalSystemError)
                {
                    throw new Exception("PayPal exception tester.");
                }

                return new PaymentResponse()
                {
                    ApplicationId = applicationId.ToString(),
                    Success = executedPayment.state.ToLower() == PayPalProcessStatus.Approved,
                    ReferenceNumber = $"PAYPAL-{executedPayment.transactions[0].related_resources[0].sale.id}",
                    OrderNumber = executedPayment.id,
                    //OrderNumber = feeCreditCardDetails.PayPalModel.OrderId,
                    PaymentStatus = (executedPayment.state.ToLower() == PayPalProcessStatus.Approved) ? PaymentProcessStatus.Succeeded : PaymentProcessStatus.Failed,

                };
            }
            catch (PayPal.PaymentsException ex)
            {
                var response = JsonConvert.DeserializeObject<PayPalError>(ex.Response);

                LoggingHelper.LogInfo($"PayPal error occurrent. Message : {ex.Message}");

                //paypal thinks a reference is an invoice
                response.message = response.message.Replace("invoice", "reference");

                return new PaymentResponse()
                {
                    ApplicationId = applicationId.ToString(),
                    Success = false,
                    PaymentStatus = PaymentProcessStatus.Failed,
                    PaymentFailureDetails = new PaymentFailureDetails()
                    {
                        RejectionDescription = response.message,
                        RejectedPayment = true,
                        SystemError = false,
                        SystemErrorMessage = response.message
                    }

                };
            }
            catch (Exception ex)
            {
                LoggingHelper.LogInfo($"PayPal system error occurrent. Message : {ex.Message}");
                return new PaymentResponse()
                {
                    ApplicationId = applicationId.ToString(),
                    Success = false,
                    PaymentStatus = PaymentProcessStatus.Failed,
                    PaymentFailureDetails = new PaymentFailureDetails()
                    {
                        RejectionDescription = "System Error",
                        RejectedPayment = true,
                        SystemError = true,
                        SystemErrorMessage = ex.Message
                    }

                };
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


        private UnraisedInvoicesContract GetUnraisedInvoicesContract(int applicationId)
        {
            var unraisedInvoiceContract = new UnraisedInvoicesContract();
            var naatiNumber = mCredentialApplicationService.GetNaatiNumberByApplicationId(applicationId);
            var application = _applicationQueryService.GetApplication(applicationId).Result;
            //var credentialRequests = _applicationQueryService.GetCredentialRequests(applicationId,new List<CredentialRequestStatusTypeName>());
            var allCredentialTestsResult = mCredentialApplicationService.GetSittableRequestsByNaatiNumberAndApplicationId(CurrentUserNaatiNumber,applicationId).Results;

            var preferredLocation = _applicationQueryService.GetVenue(new List<int>() { application.PreferredTestLocationId }).Results.FirstOrDefault();

            unraisedInvoiceContract.CredentialRequestsDisplayNames = string.Join(Environment.NewLine, allCredentialTestsResult.Select(x => $"{x.CredentialTypeDisplayName} {(string.IsNullOrEmpty(x.SkillDisplayName)?"": "-")} {x.SkillDisplayName}").ToArray());
            unraisedInvoiceContract.NaatiNumber = naatiNumber.ToString();
            unraisedInvoiceContract.ApplicantPrimaryEmail = application.ApplicantPrimaryEmail;
            unraisedInvoiceContract.ApplicantFamilyName = application.ApplicantFamilyName;
            unraisedInvoiceContract.ApplicantGivenName = application.ApplicantGivenName;
            unraisedInvoiceContract.ApplicationReference = $"APP{applicationId}";
            unraisedInvoiceContract.PreferredTestLocation = preferredLocation?.Name ?? "-";
            return unraisedInvoiceContract;
        }

        private void ExecuteAction(int applicationId, int credentialRequestId, string paymentReference, decimal paymentAmount, double? refundPercentage, RefundMethodTypeName? refundMethodTypeName, int? credentialWorkflowFeeId, string orderNumber, string transactionId, SystemActionTypeName action, string refundComments, string refundBankAccountDetails)
        {
            if (applicationId == 0)
            {
                return;
            }

            var reference = GetInvoiceReference(applicationId);

            var obj = new NcmsApplicationActionRequest
            {
                ApplicationId = applicationId,
                CredentialRequestId = credentialRequestId,
                ActionId = (int)action,
                Steps = new List<object>(),
                DueDate = DateTime.Now.AddDays(3),
                InvoiceReference = reference,
                PaymentReference = paymentReference,
                PaymentAmount = paymentAmount,
                RefundPercentage = refundPercentage,
                RefundMethodTypeId = refundMethodTypeName == null ? (int?)null : (int)refundMethodTypeName,
                CredentialWorkflowFeeId = credentialWorkflowFeeId,
                OrderNumber = orderNumber,
                TransactionId = transactionId,
                RefundComments = refundComments,
                RefundBankDetails = refundBankAccountDetails
            };

            var result = _ncmsIntegrationService.ExecuteNcmsApplicationAction(obj);
            if (!result.Success)
            {
                throw new Exception(String.Concat(", ", result.Errors));
            }
            else if ((result.Errors?.Any()).GetValueOrDefault())
            {
                throw new UserFriendlySamException(String.Join(",", result.Errors));
            }
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

        private PayOnlineResponseModel PayCreditCard(int naatiNumber, int applicationId, FeePaymentDetails feeCreditCardDetails, int questionId)
        {
            var payOnlineResponse = new PayOnlineResponseModel();

            try
            {
                var securePayResponse = MakeSecurePaymentEmbedded(naatiNumber, applicationId, feeCreditCardDetails);

                if (securePayResponse.PaymentFailureDetails?.SystemError == true)
                {
                    payOnlineResponse.FeesQuestionId = questionId;
                    payOnlineResponse.IsSecurePayStatusSuccess = false;
                    payOnlineResponse.ErrorMessage = securePayResponse.PaymentFailureDetails.SystemErrorMessage;
                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (CC last 4: {CreditCardLastFour})",
                        applicationId, securePayResponse.PaymentFailureDetails.SystemErrorMessage, F1Solutions.Global.Common.Util.Tail(feeCreditCardDetails.CreditCardNumber, 4));
                    return payOnlineResponse;
                }

                if (securePayResponse.UnHandledException && !string.IsNullOrEmpty(securePayResponse.UnHandledExceptionMessage))
                {
                    payOnlineResponse.FeesQuestionId = questionId;
                    payOnlineResponse.IsSecurePayStatusSuccess = false;
                    payOnlineResponse.ErrorMessage = securePayResponse.UnHandledExceptionMessage;
                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (CC last 4: {CreditCardLastFour})",
                        applicationId, securePayResponse.UnHandledExceptionMessage, F1Solutions.Global.Common.Util.Tail(feeCreditCardDetails.CreditCardNumber, 4));
                    return payOnlineResponse;
                }

                if (securePayResponse.PaymentStatus == PaymentProcessStatus.Failed)
                {
                    if (securePayResponse.PaymentFailureDetails != null && securePayResponse.PaymentFailureDetails.RejectedPayment)
                    {
                        payOnlineResponse.FeesQuestionId = questionId;
                        payOnlineResponse.IsSecurePayStatusSuccess = false;
                        payOnlineResponse.ErrorMessage = $"Your payment was declined with: {securePayResponse.PaymentFailureDetails.RejectionDescription}.";
                        LoggingHelper.LogWarning("APP{ApplicationId}: Payment was declined with {RejectionCode}: {RejectionDescription}. CC last 4: {CreditCardLastFour}",
                            applicationId, securePayResponse.PaymentFailureDetails.RejectionCode, securePayResponse.PaymentFailureDetails.RejectionDescription,
                            F1Solutions.Global.Common.Util.Tail(feeCreditCardDetails.CreditCardNumber, 4));
                        return payOnlineResponse;
                    }
                }
                if (securePayResponse.PaymentStatus == PaymentProcessStatus.Succeeded)
                {
                    payOnlineResponse.OrderNumber = securePayResponse.OrderNumber;
                    payOnlineResponse.SecurePayReference = securePayResponse.ReferenceNumber;
                }
                if (securePayResponse.PaymentStatus == PaymentProcessStatus.Succeeded && !securePayResponse.Success)
                {
                    var errorText = $"An error occurred when processing your transaction after your card was charged. Please contact NAATI to ensure your payment was successfully lodged. Attempting to process your payment again may result in your card being billed multiple times. Please quote your application Id when contacting NAATI. Application Id: {applicationId}";
                    payOnlineResponse.IsSecurePayStatusSuccess = true;
                    payOnlineResponse.IsSecurePayResponseSuccess = false;

                    Session["IsSecurePayStatusSuccess"] = true;
                    Session["IsSecurePayResponseSuccess"] = false;
                    Session["ReferenceNumber"] = securePayResponse.ReferenceNumber;
                    LoggingHelper.LogError("APP{ApplicationId}: {Message} (CC last 4: {CreditCardLastFour})",
                        applicationId, errorText, F1Solutions.Global.Common.Util.Tail(feeCreditCardDetails.CreditCardNumber, 4));
                    return payOnlineResponse;
                }

                //Secure payment is successfully done
                LoggingHelper.LogInfo("SecurePay payment accepted for APP{ApplicationId}. Amount: {PaymentAmount}", applicationId, feeCreditCardDetails.Total);
                payOnlineResponse.IsSecurePayStatusSuccess = true;
                payOnlineResponse.IsSecurePayResponseSuccess = securePayResponse.Success;

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
                        applicationId, payOnlineResponse.IsSecurePayStatusSuccess, payOnlineResponse.ErrorMessage);
                }
                catch { }
            }

            return payOnlineResponse;

        }


        private IList<string> ValidateAction(int applicationId, int credentialRequestId, string paymentReference, string reference, decimal paymentAmount, SystemActionTypeName action)
        {
            var obj = new NcmsApplicationActionRequest { ApplicationId = applicationId, CredentialRequestId = credentialRequestId, ActionId = (int)action, Steps = new object[] { }, DueDate = DateTime.Now.AddDays(3), InvoiceReference = reference, PaymentReference = paymentReference, PaymentAmount = paymentAmount };
            return _ncmsIntegrationService.ValidateCredentialApplication(obj);
        }

        private ActionResult GetNextQuestion(NextQuestionRequest request, GetUnraisedInvoiceSectionsResponse result)
        {
            //var sections = result.Results.Select(_autoMapperHelper.Mapper.Map<UnraisedInvoicesSectionModel>).ToList();
            var sections = SectionMapper.Map(result.Results.ToList());
            var currentSection = request.Form?.Sections?.FirstOrDefault(s => s.Id == request.SectionId);
            var lastQuestion = currentSection?.Questions?.LastOrDefault();
            var questions = sections.FirstOrDefault(s => currentSection == null || s.Id == currentSection.Id)?.Questions;
            var questionFound = lastQuestion == null ? true : false;
            var nextQuestion = new UnraisedInvoicesQuestionModel();

            foreach (var q in questions)
            {
                if (questionFound)
                {
                    var show = true;

                    if (q.Logics?.Any() ?? false)
                    {
                        var responses = request.Form?.Sections?.Where(s => s.Questions?.Any() ?? false).SelectMany(s => s.Questions).Select(x => x.Response).ToList();
                        show = (responses?.Any() ?? false) && q.Logics.All(l => responses.Contains(l.AnswerId));
                    }

                    if (show)
                    {
                        nextQuestion = q;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                questionFound = q.Id == lastQuestion.Id;
            }

            return Json(nextQuestion);
        }

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
    }


    public class NextRaiseAndPayinvoiceRequest : NextQuestionRequest
    {
        public int CredentialApplicationId { get; set; }
    }

    public class RaiseAndPayInvoiceRequest
    {
        public int CredentialApplicationId { get; set; }
    }

    public class UnraisedInvoicesContract
    {
        public bool HasSponsor { get; set; }
        public int ApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public string ApplicationReference { get; set; }
        public string ApplicantPrimaryEmail { get; set; }
        public string ApplicantFamilyName { get; set; }
        public string ApplicantGivenName { get; set; }
        public string NaatiNumber { get; set; }
        public string PreferredTestLocation { get; set; }
        public string CredentialRequestsDisplayNames { get; set; }
    }

    public static class SectionMapper
    {
        public static List<UnraisedInvoicesSectionModel> Map(this List<UnraisedInvoicesSectionContract> sectionContract)
        {
            var sections = new List<UnraisedInvoicesSectionModel>();
            foreach (var returnedSection in sectionContract)
            {
                var newsection = new UnraisedInvoicesSectionModel()
                {
                    Id = returnedSection.Id,
                    DisplayOrder = returnedSection.DisplayOrder,
                    Questions = new List<UnraisedInvoicesQuestionModel>(),
                    Name = returnedSection.Name
                };
                foreach (var question in returnedSection.Questions)
                {
                    var logicList = new List<UnraisedInvoicesQuestionLogicModel>();
                    var questionModel = new UnraisedInvoicesQuestionModel()
                    {
                        Description = question.Description,
                        DisplayOrder = question.DisplayOrder,                        
                        Logics = logicList,
                        Question = question.Text,
                        Type = question.AnswerTypeId,
                        Id = question.Id,
                        Answers = question.AnswerOptions.ToList().ToAnswers(),
                    };
                    foreach (var logic in question.QuestionLogics)
                    {
                        var newLogic = new UnraisedInvoicesQuestionLogicModel()
                        {
                            AnswerId = logic.AnswerId,
                            Id = logic.Id,
                            Type = (int)logic.Type
                        };
                        logicList.Add(newLogic);
                    }
                    newsection.Questions.Add(questionModel);
                }
                sections.Add(newsection);
            };
            return sections;
        }

        public static List<UnraisedInvoicesAnswerModel> ToAnswers(this List<AnswerOptionContract> answerOptions)
        {
            var answers = new List<UnraisedInvoicesAnswerModel>();
            foreach(var answerOption in answerOptions)
            {
                answers.Add(new UnraisedInvoicesAnswerModel()
                {
                    Description = answerOption.Description,
                    DefaultAnswer = answerOption.DefaultAnswer,
                    CredentialApplicationFieldId = answerOption.CredentialApplicationFieldId,
                    FieldData = answerOption.FieldData,
                    Id = answerOption.Id,
                    Name = answerOption.Option
                });
            }

            return answers;
        }


    }




}