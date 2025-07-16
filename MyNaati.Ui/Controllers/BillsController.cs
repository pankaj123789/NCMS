using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.UI;
using MyNaati.Ui.ViewModels.Bills;
using PayPal.Api;
using static MyNaati.Contracts.Portal.PayPal.OrderSubmission;

namespace MyNaati.Ui.Controllers
{
    [AttributeUsage(AttributeTargets.All)]
    public class DisplayBills : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var naatiNumber = Thread.CurrentPrincipal.NaatiNumber();
            var service = ServiceLocator.Resolve<ICredentialApplicationService>();

            if (!service.IsDisplayBills(naatiNumber))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action","Unauthorized"},
                    {"controller","Errors"}
                });
            }
        }
    }



    [DisplayBills]
    [Authorize]
    public class BillsController : NewtonsoftController
    {
        private readonly IAccountingService mAccountingService;
        private readonly IPayPalService mPayPalService;
        private readonly ICredentialApplicationService mCredentialApplicationService;
        private readonly IOrderService mOrderService;
        private readonly ILookupProvider mLookupProvider;
        private readonly ISystemValueService mSystemValueService;
        private readonly ISecretsCacheQueryService mSecretsProvider;
        private readonly INcmsIntegrationService _ncmsIntegrationService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public BillsController(IAccountingService accountingService, ICredentialApplicationService credentialApplicationService, IOrderService orderService, ILookupProvider lookupProvider, ISystemValueService systemValueService, ISecretsCacheQueryService secretsProvider, INcmsIntegrationService ncmsIntegrationService, IAutoMapperHelper autoMapperHelper, IPayPalService payPalService)
        {
            mAccountingService = accountingService;
            mCredentialApplicationService = credentialApplicationService;
            mOrderService = orderService;
            mLookupProvider = lookupProvider;
            mSystemValueService = systemValueService;
            mSecretsProvider = secretsProvider;
            mPayPalService = payPalService;
            _ncmsIntegrationService = ncmsIntegrationService;
            _autoMapperHelper = autoMapperHelper;
        }


        [HttpGet]
        [Authorize]
        [Route("Bills")]
        public ActionResult Index()
        {
            ViewBag.Access = true;
            return View();
        }
        
        [Authorize]
        public ActionResult GetInvoices()
        {
            var invoices = GetAllInvoiceRecords();
            return Json(invoices, JsonRequestBehavior.AllowGet);
        }


        private GetAccountingInvoicesResponse GetAllInvoiceRecords()
        {
            var naatiNumberValue = new[] { CurrentUserNaatiNumber };
            var requestContract = new GetAccountingInvoiceRequestContract
            {
                IncludeFullPaymentInfo = true,
                ExcludePayables = true,
                NaatiNumber = naatiNumberValue,
                DateCreatedFrom = null,
                DateCreatedTo = null,
                Office = null,
                EftMachine = null,
                InvoiceNumber = null,
                PaidToAccount = null,
                PaymentType = null
            };
          
            var invoices = mAccountingService.GetInvoices(requestContract);

            return invoices;
        }

        [HttpGet]
        public FileStreamResult DownloadInvoicePdf(string number, Guid invoiceId, InvoiceType type, FinanceInfoLocation location)
        {

            var requestContract = new GetInvoicePdfRequestContract
            {
                InvoiceId = invoiceId,
                Type = type,
                Location = location
            };

            var response = mAccountingService.GetInvoicePdfByInvoiceId(requestContract);
            var stream = new MemoryStream(response.AccountingInvoicePdfFileContent);

            return File(stream, "application/pdf", $"{number}.pdf");
        }

        public ActionResult DownloadInvoicePayment(string invoiceId, InvoiceType type, FinanceInfoLocation location)
        {
            Stream stream;

            var requestContract = new GetInvoicePdfRequestContract
            {
                InvoiceId =  new Guid(invoiceId),
                Type = type,
                Location = location
            };

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=presentation.pdf");
            var response = mAccountingService.GetInvoicePdfByInvoiceId(requestContract);
            stream = new MemoryStream(response.AccountingInvoicePdfFileContent);

            return new FileStreamResult(stream, "application/pdf");
        }

        private AccountingInvoiceDto GetInvoice(string invoiceNumber)
        {
            var invoiceNumberArray = new[] { invoiceNumber };

            var requestContract = new GetAccountingInvoiceRequestContract
            {
                IncludeFullPaymentInfo = true,
                ExcludePayables = true,
                InvoiceNumber = invoiceNumberArray,
            };

            return mAccountingService.GetInvoices(requestContract).Data.FirstOrDefault();
        }


        [HttpGet]
        public ActionResult PayPalPayment(string invoiceNumber = null)
        {
            //getting the apiContext  
            var apiContext = PayPalConfigurationHelper.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    var invoice = GetInvoice(invoiceNumber);
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class   
                    string baseURI = $"{Request.Url.Scheme}://{Request.Url.IdnHost}{Request.Url.AbsolutePath}Succeeded?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Guid.NewGuid().ToString();
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = mPayPalService.CreatePayment(apiContext, baseURI + "guid=" + guid,invoice.InvoiceNumber, "Test Fee", invoice.Balance.ToString());
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = mPayPalService.ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                    if (executedPayment.state == PayPalProcessStatus.Failed)
                    {
                        return View("FailureView");
                    }

                    var invoice = GetInvoice(executedPayment.transactions.First().invoice_number);

                    //Paypal payment is successfully done
                    LoggingHelper.LogInfo("PayPal payment accepted for invoice {InvoiceNumber}. Amount: {PaymentAmount}", executedPayment.transactions.First().invoice_number, executedPayment.transactions.First().amount);

                    var paymentAccount = mLookupProvider.SystemValues.WiisePaymentAccount;
                    var wiiseReference = $"{executedPayment.id}";
                    var paymentCreateRequestModel = new PaymentCreateRequestModel
                    {
                        InvoiceNumber = invoice.InvoiceNumber,
                        PaymentType = PaymentTypeDto.PayPal,
                        Amount = Convert.ToDecimal(executedPayment.transactions.First().amount.total),
                        DatePaid = DateTime.Now,
                        Reference = wiiseReference,
                        AccountId = new Guid(paymentAccount),
                        UserName = GetUserName(),
                        NaatiNumber = invoice.NaatiNumber.ToString()
                    };

                    var createPaymentResponse = mAccountingService.CreatePayment(paymentCreateRequestModel);
                    if (createPaymentResponse.UnHandledException && !string.IsNullOrEmpty(createPaymentResponse.UnHandledExceptionMessage))
                    {
                        var errorText = createPaymentResponse.UnHandledExceptionMessage;
                        LoggingHelper.LogError("{InvoiceNumber}: Error adding payment to Wiise: {Message}", invoice.InvoiceNumber, errorText);
                        ModelState.AddModelError("", errorText);
                        //return View(paymentDetailsModel);
                    }

                    if (createPaymentResponse.Error)
                    {
                        LoggingHelper.LogError(
                            "Payment was taken successfully (PayPal ref: {SecurePayReferenceNumber}) but error occurred when updating Wiise invoice {InvoiceNumber}, NAATI #{NaatiNumber}: {Message}",
                            executedPayment.transactions.First().reference_id, invoice.InvoiceNumber, invoice.NaatiNumber.ToString(), createPaymentResponse.ErrorMessage);

                        var errorText = $"An error occurred when processing your transaction after your card was charged. Please contact NAATI to ensure your payment was successfully logged. Attempting to process your payment again may result in your card being billed multiple times. Please quote your invoice number when contacting NAATI. Invoice Number: {invoice.InvoiceNumber}, Payment Reference: {createPaymentResponse.PaymentId}";
                        ModelState.AddModelError("", errorText);
                        //paymentDetailsModel.IsPaidWithError = true;
                        //return View(paymentDetailsModel);
                    }

                    var updateNcmsOutstandingInvoicesError = _ncmsIntegrationService.UpdateNcmsOutstandingInvoices(invoice.InvoiceNumber, createPaymentResponse.Reference, wiiseReference, executedPayment.token);

                    if (!updateNcmsOutstandingInvoicesError.Success || updateNcmsOutstandingInvoicesError.Errors.Any())
                    {
                        LoggingHelper.LogError(
                            "Payment was taken successfully (SecurePay ref: {SecurePayReferenceNumber}) but error occurred when updating the CredentialRequest/Application for Invoice: {Message}",
                            createPaymentResponse.Reference, invoice.InvoiceNumber, invoice.NaatiNumber, updateNcmsOutstandingInvoicesError);

                        var errorText = $"An error occurred when processing your transaction after your card was charged. Please contact NAATI to ensure your payment was successfully logged. Attempting to process your payment again may result in your card being billed multiple times. Please quote your invoice number when contacting NAATI. Invoice Number: {invoice.InvoiceNumber}, Payment Reference: {executedPayment.id}";
                        ModelState.AddModelError("", errorText);
                        //paymentDetailsModel.IsPaidWithError = true;
                        //return View(paymentDetailsModel);
                    }

                    Session["IsPayOnlineComplete"] = true;
                    Session["InvoiceNumber"] = invoice.InvoiceNumber;
                    Session["ReferenceNumber"] = executedPayment.id;

                    return RedirectToAction("PaymentSuccess");

                }
            }
            catch (Exception)
            {
                return View("FailureView");
            }
        }

        [HttpGet]
        public ActionResult PayOnline(string invoiceNumber)
        {
            var invoiceNumberArray = new[] { invoiceNumber };

            var requestContract = new GetAccountingInvoiceRequestContract
            {
                IncludeFullPaymentInfo = true,
                ExcludePayables = true,
                NaatiNumber = null,
                DateCreatedFrom = null,
                DateCreatedTo = null,
                Office = null,
                EftMachine = null,
                InvoiceNumber = invoiceNumberArray,
                PaidToAccount = null,
                PaymentType = null,
            };

            var invoice = mAccountingService.GetInvoices(requestContract).Data.FirstOrDefault();

            var getOfficeAbbrAndEftMachineTermianlRequest = new GetOfficeAbbrAndEftMachineTermianlRequest
            {
                OnlineOfficeId = mLookupProvider.SystemValues.OnlineOfficeId,
                OnlineEftMachineId = mLookupProvider.SystemValues.OnlineEFTMachineId
            };
            var getOfficeAbbrAndEftMachineTermianlResponse = mAccountingService.GetOnlineofficeAbbrAndEftMachineTerminal(getOfficeAbbrAndEftMachineTermianlRequest);

            //var variables = mSystemValueService.GetAll();
            var paymentDetailsModel = new PaymentDetailsModel
            {
                InvoiceNumber = invoiceNumber,
                NaatiNumber = CurrentUserNaatiNumber.ToString(),
                AmountDue = invoice?.Balance ?? 0,
                AmountPay = invoice?.Balance ?? 0,
                BillType = invoice?.Type.ToString(),
                InvoiceAmount = invoice?.Total ?? 0,
                Payments = invoice?.Payment ?? 0,
                OnlineOfficeAbbr = getOfficeAbbrAndEftMachineTermianlResponse.OnlineOfficeAbbr,
                EFTMachineTerminal = getOfficeAbbrAndEftMachineTermianlResponse.OnlineEftMachineTerminalNo,
                InvoiceId = invoice?.InvoiceId ?? new Guid()
                //AllowPaymentByVisa = variables.FirstOrDefault(v => v.Key == "AllowPaymentByVisa")?.Value == "true",
                //AllowPaymentByMasterCard = variables.FirstOrDefault(v => v.Key == "AllowPaymentByMasterCard")?.Value == "true",
                //AllowPaymentByAmex = variables.FirstOrDefault(v => v.Key == "AllowPaymentByAmex")?.Value == "true",
                //AllowPaymentByDinersClub = variables.FirstOrDefault(v => v.Key == "AllowPaymentByDinersClub")?.Value == "true",
                //AllowPaymentByJcb = variables.FirstOrDefault(v => v.Key == "AllowPaymentByJcb")?.Value == "true",
            };

            return View(paymentDetailsModel);
        }


        [HttpPost]        
        public ActionResult PayOnline(PaymentDetailsModel paymentDetailsModel)
        {
            if (ModelState.IsValid)
            {
                // default error text
                string errorText =
                    "A system error has occurred. Please contact info@naati.com.au and quote your Customer Number and invoice number. Please do not attempt to pay again as this may result in your card being charged mutiple times.";
                try
                {
                    var securePayResponse = MakeSecurePaymentEmbedded(paymentDetailsModel);

                    if (securePayResponse.PaymentFailureDetails?.SystemError == true)
                    {
                        LoggingHelper.LogError("{InvoiceNumber}: Error during payment. Customer was advised to contact NAATI. {Message}", paymentDetailsModel.InvoiceNumber, securePayResponse.PaymentFailureDetails.SystemErrorMessage);
                        ModelState.AddModelError("", errorText);
                        return View(paymentDetailsModel);
                    }

                    if (securePayResponse.UnHandledException && !string.IsNullOrEmpty(securePayResponse.UnHandledExceptionMessage))
                    {
                        LoggingHelper.LogError("{InvoiceNumber}: Error during payment. Customer was advised to contact NAATI. {Message}", paymentDetailsModel.InvoiceNumber, securePayResponse.UnHandledExceptionMessage);
                        ModelState.AddModelError("", errorText);
                        return View(paymentDetailsModel);
                    }

                    if (securePayResponse.PaymentStatus == PaymentProcessStatus.Failed)
                    {
                        if (securePayResponse.PaymentFailureDetails != null && securePayResponse.PaymentFailureDetails.RejectedPayment)
                        {
                            errorText = $"Your payment was declined with {securePayResponse.PaymentFailureDetails.RejectionCode}: {securePayResponse.PaymentFailureDetails.RejectionDescription}. We are unable to proceed with your request.";
                            LoggingHelper.LogWarning("{InvoiceNumber}: Payment was declined with {RejectionCode}: {RejectionDescription}. CC last 4: {CreditCardLastFour}",
                                paymentDetailsModel.InvoiceNumber, securePayResponse.PaymentFailureDetails.RejectionCode, securePayResponse.PaymentFailureDetails.RejectionDescription,
                                paymentDetailsModel.LastDigits);

                            ModelState.AddModelError("", errorText);
                            return View(paymentDetailsModel);
                        }
                    }

                    if (securePayResponse.PaymentStatus == PaymentProcessStatus.Succeeded && !securePayResponse.Success)
                    {
                        errorText = $"An error occurred when processing your transaction after your card was charged. Please contact NAATI to ensure your payment was successfully lodged. Attempting to process your payment again may result in your card being billed multiple times. Please quote your invoice number when contacting NAATI. Invoice Number: {paymentDetailsModel.InvoiceNumber}";
                        LoggingHelper.LogError("{InvoiceNumber}: Unknown error during payment. Customer was advised to contact NAATI.", paymentDetailsModel.InvoiceNumber);
                        ModelState.AddModelError("", errorText);
                        paymentDetailsModel.IsPaidWithError = true;
                        return View(paymentDetailsModel);
                    }

                    //Secure payment is successfully done
                    LoggingHelper.LogInfo("SecurePay payment accepted for invoice {InvoiceNumber}. Amount: {PaymentAmount}", paymentDetailsModel.InvoiceNumber, paymentDetailsModel.AmountPay);
                    paymentDetailsModel.Type = securePayResponse.CardDetailsForReceipt.Type;
                    paymentDetailsModel.IsPaymentSuccessful = securePayResponse.Success;

                    //Wiise entry create
                    var createPaymentResponse = MakeWiiseAccountUpdate(paymentDetailsModel, securePayResponse.ReferenceNumber);

                    if (createPaymentResponse.UnHandledException && !string.IsNullOrEmpty(createPaymentResponse.UnHandledExceptionMessage))
                    {
                        errorText = createPaymentResponse.UnHandledExceptionMessage;
                        LoggingHelper.LogError("{InvoiceNumber}: Error adding payment to Wiise: {Message}", paymentDetailsModel.InvoiceNumber, errorText);
                        ModelState.AddModelError("", errorText);
                        return View(paymentDetailsModel);
                    }

                    if (createPaymentResponse.Error)
                    {
                        LoggingHelper.LogError(
                            "Payment was taken successfully (SecurePay ref: {SecurePayReferenceNumber}) but error occurred when updating Wiise invoice {InvoiceNumber}, NAATI #{NaatiNumber}: {Message}",
                            securePayResponse.ReferenceNumber, paymentDetailsModel.InvoiceNumber, paymentDetailsModel.NaatiNumber, createPaymentResponse.ErrorMessage);

                        errorText = $"An error occurred when processing your transaction after your card was charged. Please contact NAATI to ensure your payment was successfully logged. Attempting to process your payment again may result in your card being billed multiple times. Please quote your invoice number when contacting NAATI. Invoice Number: {paymentDetailsModel.InvoiceNumber}, Payment Reference: {securePayResponse.ReferenceNumber}";
                        ModelState.AddModelError("", errorText);
                        paymentDetailsModel.IsPaidWithError = true;
                        return View(paymentDetailsModel);
                    }

                    var updateNcmsOutstandingInvoicesError = _ncmsIntegrationService.UpdateNcmsOutstandingInvoices(paymentDetailsModel.InvoiceNumber, createPaymentResponse.Reference, securePayResponse.ReferenceNumber, securePayResponse.OrderNumber);

                    if (!updateNcmsOutstandingInvoicesError.Success || updateNcmsOutstandingInvoicesError.Errors.Any())
                    {
                        LoggingHelper.LogError(
                            "Payment was taken successfully (SecurePay ref: {SecurePayReferenceNumber}) but error occurred when updating the CredentialRequest/Application for Invoice: {Message}",
                            securePayResponse.ReferenceNumber, paymentDetailsModel.InvoiceNumber, paymentDetailsModel.NaatiNumber, updateNcmsOutstandingInvoicesError);

                        errorText = $"An error occurred when processing your transaction after your card was charged. Please contact NAATI to ensure your payment was successfully logged. Attempting to process your payment again may result in your card being billed multiple times. Please quote your invoice number when contacting NAATI. Invoice Number: {paymentDetailsModel.InvoiceNumber}, Payment Reference: {securePayResponse.ReferenceNumber}";
                        ModelState.AddModelError("", errorText);
                        paymentDetailsModel.IsPaidWithError = true;
                        return View(paymentDetailsModel);
                    }

                    paymentDetailsModel.IsWiiseSuccessful = true;
                    Session["IsPayOnlineComplete"] = paymentDetailsModel.IsPaymentSuccessful && paymentDetailsModel.IsWiiseSuccessful;
                    Session["InvoiceNumber"] = paymentDetailsModel.InvoiceNumber;
                    Session["ReferenceNumber"] = securePayResponse.ReferenceNumber;

                    return RedirectToAction("PaymentSuccess");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", errorText);
                    // UNHANDLED ERROR - LAST DITCH EFFORT TO LOG IT
                    try
                    {
                        LoggingHelper.LogException(ex, "Unhandled exception during or after bill payment (invoice {InvoiceNumber}): {Message}", paymentDetailsModel.InvoiceNumber, ex.Message);
                    }
                    catch { }
                    return View(paymentDetailsModel);
                }
            }

            return View(paymentDetailsModel);
        }

        private PaymentResponse MakeSecurePayment(PaymentDetailsModel paymentDetailsModel)
        {
            var request = new PaymentRequest
            {
                InvoiceNumber = paymentDetailsModel.InvoiceNumber,
                NAATINumber = paymentDetailsModel.NaatiNumber,
                Amount = paymentDetailsModel.AmountPay,
                CardDetails = _autoMapperHelper.Mapper.Map<PaymentDetailsModel, EnteredCardDetails>(paymentDetailsModel),
                PaymentDate = DateTime.Now
            };

            var securePayResponse = mOrderService.SubmitCreatePayment(request);
            return securePayResponse;
        }

        private PaymentResponse MakeSecurePaymentEmbedded(PaymentDetailsModel paymentDetailsModel)
        {
            var cardDetails = new EnteredCardDetails
            {
                CardToken = paymentDetailsModel.CreditCardToken
            };

            var request = new PaymentRequest
            {
                InvoiceNumber = paymentDetailsModel.InvoiceNumber,
                NAATINumber = paymentDetailsModel.NaatiNumber,
                Amount = paymentDetailsModel.AmountPay,
                CardDetails = cardDetails,
                PaymentDate = DateTime.Now
            };

            var securePayResponse = mOrderService.SubmitCreatePayment(request);
            return securePayResponse;
        }

        private PaymentCreateResponseModel MakeWiiseAccountUpdate(PaymentDetailsModel paymentDetailsModel, string referenceNumber)
        {
            var paymentAccount = mLookupProvider.SystemValues.WiisePaymentAccount;
            var wiiseReference = $"{paymentDetailsModel.OnlineOfficeAbbr}-EFTPOS-{paymentDetailsModel.EFTMachineTerminal} - {referenceNumber}";
            var paymentCreateRequestModel = new PaymentCreateRequestModel
            {
                InvoiceNumber = paymentDetailsModel.InvoiceNumber,
                PaymentType = PaymentTypeDto.Eft,
                Amount = paymentDetailsModel.AmountPay,
                DatePaid = DateTime.Now,
                Reference = wiiseReference,
                AccountId = new Guid(paymentAccount),
                UserName = GetUserName(),
                NaatiNumber = paymentDetailsModel.NaatiNumber
            };

            var createPaymentResponse = mAccountingService.CreatePayment(paymentCreateRequestModel);
            return createPaymentResponse;
        }

        public ActionResult PaymentSuccess()
        {

            var isPayOnlineComplete = (bool?) Session["IsPayOnlineComplete"] ?? false;

            if (isPayOnlineComplete)
            {
                Session["IsPayOnlineComplete"] = null;
                Session.Remove("IsPayOnlineComplete");

                var invoiceNumber = (string)Session["InvoiceNumber"] ?? "not found";
                var referenceNumber = (string)Session["ReferenceNumber"] ?? "not found";

                Session["InvoiceNumber"] = null;
                Session.Remove("InvoiceNumber");
                Session["ReferenceNumber"] = null;
                Session.Remove("ReferenceNumber");

                ViewBag.InvoiceNumber = invoiceNumber;
                ViewBag.ReferenceNumber = referenceNumber;

                return View();
            }
            return RedirectToAction("Index");
        }

        //private string UpdateNcmsOutstandingInvoices(string invoiceNumber)
        //{
        //    var request = new { InvoiceNumber = invoiceNumber };
        //    var data = JsonConvert.SerializeObject(request);
        //    var ctn = new StringContent(data, Encoding.UTF8, "application/json");

        //    var credentials = CredentialCache.DefaultNetworkCredentials;

        //    var ncmsUrl = ConfigurationManager.AppSettings["NcmsUrl"];
        //    var ncmsCookieName = ConfigurationManager.AppSettings["NcmsAuthCookieName"];
        //    var identity = mSecretsProvider.Get("MyNaatiDefaultIdentity");

        //    var httpCookie = FormsAuthentication.GetAuthCookie(identity, false);
        //    httpCookie.Expires = DateTime.Now.AddMinutes(1);
        //    var cookieContainer = new CookieContainer();
        //    cookieContainer.Add(new Uri(ncmsUrl), new Cookie(ncmsCookieName, httpCookie.Value));

        //    var httpClientHandler = new HttpClientHandler
        //    {
        //        Credentials = credentials,
        //        CookieContainer = cookieContainer
        //    };

        //    using (HttpClient client = new HttpClient(httpClientHandler))
        //    {
        //        client.BaseAddress = new Uri(ncmsUrl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = client.PostAsync("api/application/MyNaatiOutstandingInvoices", ctn).Result;

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var builder = new StringBuilder();
        //            builder.AppendLine("Error executing NCMS request.");
        //            builder.AppendLine($"Error occurred while executing UpdateOutstandingInvoices for invoice: '{invoiceNumber}'");
        //            builder.AppendLine($"URL: {response.RequestMessage?.RequestUri}");
        //            builder.AppendLine("Response:");
        //            builder.AppendLine(response.ToString());
        //            return builder.ToString();
        //        }

        //        return string.Empty;
        //    }
        //}

        private string GetUserName()
        {
            return mSecretsProvider.Get(SecuritySettings.MyNaatiDefaultIdentityKey);
        }

    }
}