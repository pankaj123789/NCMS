using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Attributes;
using MyNaati.Ui.ViewModels.CredentialApplication;
using Newtonsoft.Json;
using PayPal.Api;
using static MyNaati.Contracts.Portal.PayPal.OrderSubmission;
using IOrderService = MyNaati.Contracts.Portal.IOrderService;
using ISystemValueService = F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues.ISystemValueService;

namespace MyNaati.Ui.Controllers
{
    public class ApplicationsController : NewtonsoftController
    {
        private readonly ISystemValueService mSystemValueService;
        private readonly IAccreditationResultService mAccreditationResultService;
        private readonly ICredentialApplicationService mCredentialApplicationService;
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly IPayPalService mPayPalService;
        public const int Translator = 1;
        public const int Interpreter = 2;
        public const int DeafInterpreter = 3;
        public const int ConferenceInterpreter = 4;
        public const int HealthSpecialistInterpreter = 5;
        public const int LegalSpecialistInterpreter = 6;
        public const int GeneralInterpreter = 7;
        private readonly ILookupProvider mLookupProvider;
        private readonly IOrderService mOrderService;
        private readonly IAccountingService mAccountingService;
        private readonly INcmsIntegrationService _ncmsIntegrationService;
        private readonly IRefundCalculator mRefundCalculator;
        private readonly ISecretsCacheQueryService mSecretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IApplicationQueryService _applicationQueryService;
        private readonly ICredentialApplicationRefundService _credentialApplicationRefundService;


        private const string RequestCancelled = @"
<h2 class=""text-warning""><strong>Application Cancelled</strong></h2>
<br/>
<p><a href = ""{0}"">Click here</a> to return to the Test Result page</p>";
        public ApplicationsController(ISystemValueService systemValueService,
            IAccreditationResultService accreditationResultService,
            ICredentialApplicationService credentialApplicationService,
            IPersonalDetailsService personalDetailsService,
            IPayPalService payPalService,
            ILookupProvider lookupProvider,
            IOrderService orderService,
            IAccountingService accountingService,
            INcmsIntegrationService ncmsIntegrationService,
            IRefundCalculator refundCalculator,
            ISecretsCacheQueryService secretsProvider,
            IAutoMapperHelper autoMapperHelper,
            IApplicationQueryService applicationQueryService,
            ICredentialApplicationRefundService credentialApplicationRefundService)
        {

            mSystemValueService = systemValueService;
            mAccreditationResultService = accreditationResultService;
            mCredentialApplicationService = credentialApplicationService;
            mPersonalDetailsService = personalDetailsService;
            mPayPalService = payPalService;
            mLookupProvider = lookupProvider;
            mOrderService = orderService;
            mAccountingService = accountingService;
            _ncmsIntegrationService = ncmsIntegrationService;
            mRefundCalculator = refundCalculator;
            mSecretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
            _applicationQueryService = applicationQueryService;
            _credentialApplicationRefundService = credentialApplicationRefundService;
        }

        [HttpGet]
        public ActionResult ExpressionOfInterest()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MyTests(string errorMessage = null)
        {
            var allCredentialTestsResult = mCredentialApplicationService.GetSittableRequestsByNaatiNumber(CurrentUserNaatiNumber).Results;

            var myTestViewModelList = new MyTestViewModelList()
            {
                ErrorMessage = errorMessage
            };

            allCredentialTestsResult.ForEach(p =>
                myTestViewModelList.MyTestList.Add(new MyTestViewModel
                {
                    CredentialRequestId = p.CredentialRequestId,
                    CredentialApplicationId = p.CredentialApplicationId,
                    TestDate = p.TestDate,
                    ApplicationTypeDisplayName = p.ApplicationTypeDisplayName,
                    CredentialTypeDisplayName = p.CredentialTypeDisplayName,
                    VenueName = p.VenueName,
                    SkillDisplayName = p.SkillDisplayName,
                    Status = p.Status,
                    CanOpenDetails = p.CanOpenDetails,
                    CanSelectTestSession = p.CanSelectTestSession,
                    CanRequestRefund = p.CanRequestRefund
                })
            );

            return View(myTestViewModelList);
        }

        [HttpGet]
        [Authorize]
        public ActionResult MyTestResults()
        {
            var allTestResults = mCredentialApplicationService.GetTestResults(CurrentUserNaatiNumber).Results;

            var myTestViewModelList = new MyTestResultViewModelList();

            allTestResults.ForEach(p =>
                myTestViewModelList.MyTestResultList.Add(new MyTestResultViewModel
                {
                    TestDate = p.TestDate,
                    CredentialTypeDisplayName = p.CredentialTypeDisplayName,
                    VenueName = p.VenueName,
                    SkillDisplayName = p.SkillDisplayName,
                    EligibleForAPaidTestReview = p.EligibleForAPaidTestReview,
                    EligibleForASupplementaryTest = p.EligibleForASupplementaryTest,
                    OverallResult = p.OverallResult,
                    TestSittingId = p.TestSittingId,
                    State = p.State,
                    TestLocationName = p.TestLocationName,
                    Supplementary = p.Supplementary
                })
            );

            return View(myTestViewModelList);
        }

        #region Apply for a Paid Review
        [HttpGet]
        [Authorize]
        public ActionResult ApplyForAPaidReview(int testSittingId)
        {
            var credentialType = mCredentialApplicationService.GetCredentialTypeByTestSittingId(testSittingId);
            var inProgressCredentialRequests = mCredentialApplicationService.GetInProgressCredentialRequests(CurrentUserNaatiNumber);

            if (credentialType != null)
            {
                var applicationInprogessWithSameCredential = inProgressCredentialRequests.Results != null && inProgressCredentialRequests.Results.Any(x => x.LevelId == credentialType.CredentialTypeId && x.SkillId == credentialType.SkillId);

                if (applicationInprogessWithSameCredential)
                    return RedirectToAction("MyTestResults");
            }

            var payPalClientId = mSecretsProvider.Get(SecuritySettings.PayPalClientId);
            ViewBag.PayPalClientId = $"https://www.paypal.com/sdk/js?client-id={payPalClientId}&currency=AUD";

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ApplyForASupplementaryTest(int testSittingId)
        {
            var payPalClientId = mSecretsProvider.Get(SecuritySettings.PayPalClientId);
            ViewBag.PayPalClientId = $"https://www.paypal.com/sdk/js?client-id={payPalClientId}&currency=AUD";
            return View("SupplementaryTest");
        }

        [HttpGet]
        [Route("applications/applyforpaidreview/settings")]
        public ActionResult Settings(int testAttendanceId)
        {
            var result = mCredentialApplicationService.GetPaidReviewSections(CurrentUserNaatiNumber, testAttendanceId);
            var sections = result.Results.Select(_autoMapperHelper.Mapper.Map<ApplicationFormSectionModel>).ToList();
            foreach (var s in sections)
            {
                s.Questions.Clear();
            }
            return Json(new
            {
                Sections = sections,
                DisablePayPalUi = mLookupProvider.SystemValues.DisablePayPalUi
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("applications/applyforpaidreview/nextquestion")]
        public ActionResult NextQuestion(NextPaidReviewQuestionRequest request)
        {
            var result = mCredentialApplicationService.GetPaidReviewSections(CurrentUserNaatiNumber, request.TestAttendanceId);
            return GetNextQuestion(request, result);
        }

        [HttpGet]
        [Route("applications/applyforpaidreview/testdetail")]
        public ActionResult TestDetail(int testAttendanceId)
        {
            var response = mCredentialApplicationService.GetPaidReviewTestDetails(CurrentUserNaatiNumber, testAttendanceId);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("applications/applyforpaidreview/paymentcontrol")]
        public ActionResult PaymentControl(int testAttendanceId)
        {
            var response = mCredentialApplicationService.GetPaidReviewTestDetails(CurrentUserNaatiNumber, testAttendanceId);
            return Json(new { Sponsor = response.Sponsor, Fees = new[] { response.Fee } }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        [Route("applications/supplementarytest/settings")]
        public ActionResult SupplementaryTestSettings(int testAttendanceId)
        {

            var result = mCredentialApplicationService.GetSupplementaryTestSections(CurrentUserNaatiNumber, testAttendanceId);
            var sections = result.Results.Select(_autoMapperHelper.Mapper.Map<ApplicationFormSectionModel>).ToList();
            foreach (var s in sections)
            {
                s.Questions.Clear();
            }
            return Json(new
            {
                Sections = sections,
                DisablePayPalUi = mLookupProvider.SystemValues.DisablePayPalUi
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("applications/supplementarytest/nextquestion")]
        public ActionResult SupplementaryTestNextQuestion(NextPaidReviewQuestionRequest request)
        {
            var result = mCredentialApplicationService.GetSupplementaryTestSections(CurrentUserNaatiNumber, request.TestAttendanceId);
            return GetNextQuestion(request, result);
        }

        [HttpGet]
        [Route("applications/supplementarytest/testdetail")]
        public ActionResult SupplementaryTestTestDetail(int testAttendanceId)
        {
            var response = mCredentialApplicationService.GetSupplementaryTestDetails(CurrentUserNaatiNumber, testAttendanceId);

            return Json(response, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        [Route("applications/supplementarytest/paymentcontrol")]
        public ActionResult SupplementaryTestPaymentControl(int testAttendanceId)
        {

            var response = mCredentialApplicationService.GetSupplementaryTestDetails(CurrentUserNaatiNumber, testAttendanceId);
            return Json(new { Sponsor = response.Sponsor, Fees = new[] { response.Fee } }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("applications/supplementarytest/tasks")]
        public ActionResult SupplementaryTestTasks(int testAttendanceId)
        {
            var response = mCredentialApplicationService.GetSupplementaryTestTasks(CurrentUserNaatiNumber, testAttendanceId);
            return Json(response.Tasks, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [MvcRecaptchaValidation(modelErrorKeys: (nameof(MyTestsModel.ErrorMessage)))]
        public ActionResult AvailableTestSessions(MyTestsModel myTestsModel)
        {
            var canProceed = ModelState.IsValid; // forms is coming from recaptcha and is valid
            if (!canProceed)
            {
                //'Reject Test Session' has reposted the form and is OK to proceed
                canProceed = Request.UrlReferrer.AbsolutePath.Contains("AvailableTestSessions");
            }

            if (!canProceed || !mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, myTestsModel.CredentialRequestId))
            {
                var errorMessage = ModelState["ErrorMessage"]?.Errors[0]?.ErrorMessage;
                return RedirectToAction("MyTests", new { errorMessage });
            }

            var availableTestSessions = new AvailableTestSessionViewModelList
            {
                CredentialRequestId = myTestsModel.CredentialRequestId,
                CredentialApplicationId = myTestsModel.CredentialApplicationId
            };

            var allAvailableTestSessionsAndRejectableTestSessionsResponse = mCredentialApplicationService.GetAllAvailableTestSessionsAndRejectableTestSessions(myTestsModel.CredentialRequestId);

            var allAvailableTestSessions = allAvailableTestSessionsAndRejectableTestSessionsResponse.AvailableTestSessions;
            var allocatedTestSession = allAvailableTestSessionsAndRejectableTestSessionsResponse.AllocatedTestSession;

            allAvailableTestSessions.ForEach(p =>
                availableTestSessions.AvailableTestSessionList.Add(new AvailableTestSessionViewModel
                {
                    Id = p.TestSessionId,
                    TestSessionId = "TS" + p.TestSessionId.ToString(),
                    TestDate = p.TestDate.ToLongDateString().ToString(),
                    TestStart = p.TestDate.ToShortTimeString(),
                    TestLocation = p.TestLocation,
                    ExpectedCompletion = GetExpectedCompletion(p.TestDate, p.TestSessionDuration),
                    VenueAddress = p.VenueAddress,
                    VenueName = p.VenueName,
                    Availability = GetAvailability(p.AvailableSeats)[0],
                    AvailabilityClass = GetAvailability(p.AvailableSeats)[1],
                    HasAllocatedTestSession = p.Selected,
                    CredentialRequestId = myTestsModel.CredentialRequestId,
                    CredentialApplicationId = myTestsModel.CredentialApplicationId,
                    IsPreferedLocation = p.IsPreferedLocation,
                    TestFeePaid = p.TestFeePaid,
                })
            );

            if(allocatedTestSession != null)
            {
                availableTestSessions.AllocatedTestSession = new AvailableTestSessionViewModel
                {
                    Id = allocatedTestSession.TestSessionId,
                    TestSessionId = "TS" + allocatedTestSession.TestSessionId.ToString(),
                    TestDate = allocatedTestSession.TestDate.ToLongDateString().ToString(),
                    TestStart = allocatedTestSession.TestDate.ToShortTimeString(),
                    TestLocation = allocatedTestSession.TestLocation,
                    ExpectedCompletion = GetExpectedCompletion(allocatedTestSession.TestDate, allocatedTestSession.TestSessionDuration),
                    VenueAddress = allocatedTestSession.VenueAddress,
                    VenueName = allocatedTestSession.VenueName,
                    Availability = GetAvailability(allocatedTestSession.AvailableSeats)[0],
                    AvailabilityClass = GetAvailability(allocatedTestSession.AvailableSeats)[1],
                    HasAllocatedTestSession = allocatedTestSession.Selected,
                    CredentialRequestId = myTestsModel.CredentialRequestId,
                    CredentialApplicationId = myTestsModel.CredentialApplicationId,
                    IsPreferedLocation = allocatedTestSession.IsPreferedLocation,
                    TestFeePaid = allocatedTestSession.TestFeePaid,
                };
            }

            return View(availableTestSessions);
        }

        private string GetExpectedCompletion(DateTime? testDate, int duration)
        {
            double dDuration = duration;
            if (testDate.HasValue)
            {
                var expectedCompletion = testDate?.AddMinutes(dDuration);
                return expectedCompletion.Value.ToShortTimeString();
            }
            return string.Empty;
        }

        private string[] GetAvailability(int availableSeat)
        {
            var availability = new string[2];

            if (availableSeat < 5)
            {
                availability[0] = "Nearly Full";
                availability[1] = "warning";
            }
            else if (availableSeat >= 5 && availableSeat <= 25)
            {
                availability[0] = "Filling Up";
                availability[1] = "primary";
            }
            else if (availableSeat > 25)
            {
                availability[0] = "Seats Available";
                availability[1] = "success";
            }
            return availability;
        }

        [HttpGet]
        [Authorize]
        public ActionResult ManageTest(int credentialRequestId)
        {
            var testSessionResult = mCredentialApplicationService.GetManageTestSession(CurrentUserNaatiNumber, credentialRequestId).Result;

            var manageTestModel = new ManageTestModel
            {
                TestSessionId = testSessionResult.TestSessionId,
                TestSessionCredentialRequestId = testSessionResult.TestSessionCredentialRequestId,
                CustomerNo = testSessionResult.CustomerNo,
                Status = testSessionResult.Status,
                TestDateString = testSessionResult.TestDate?.ToLongDateString(),
                TestStart = GetTestStartTime(testSessionResult.TestDate, testSessionResult.ArrivalTime),
                ExpectedCompletion = GetExpectedCompletionTime(testSessionResult.TestDate, testSessionResult.Duration),
                Application = testSessionResult.Application,
                CredentialType = testSessionResult.CredentialType,
                Skill = testSessionResult.Skill,
                VenueName = testSessionResult.VenueName,
                VenueAddress = testSessionResult.VenueAddress,
                Notes = testSessionResult.Notes,
                CredentialRequestId = testSessionResult.CredentialRequestId,
                CredentialApplicationId = testSessionResult.CredentialApplicationId,
                CanChangeRejectTestDate = testSessionResult.CanChangeRejectTestDate,
                VenueCoordinates = testSessionResult.VenueCoordinates

            };

            return View(manageTestModel);
        }

        [HttpGet]
        [Authorize]
        public ActionResult RefundDetails(int credentialRequestId, int credentialApplicationId)
        {
            var refundStatusResponse = mRefundCalculator.CalculateRefund(credentialRequestId);

            var refundModel = new RefundViewModel
            {
                IsEligibleForRefund = refundStatusResponse.RefundCalculationResultType == RefundCalculationResultTypeName.RefundAvailable,
                RefundNotCalculated = refundStatusResponse.RefundCalculationResultType == RefundCalculationResultTypeName.NotCalculated,
                RefundPercentage = refundStatusResponse.RefundPercentage.GetValueOrDefault() * 100 + "%",
                CredentialRequestId = credentialRequestId,
                CredentialApplicationId = credentialApplicationId,
                Policy = refundStatusResponse.Policy,
                IsManualRefund = refundStatusResponse.AvailableRefundMethodTypes.Count == 1 && refundStatusResponse.AvailableRefundMethodTypes.Contains(RefundMethodTypeName.DirectDeposit)
            };

            var isValid = mRefundCalculator.ValidateRefundRequest(credentialRequestId);

            if (!isValid)
            {
                var refundForm = mSystemValueService.GetAll().First(r => r.Key.Equals("RefundFormURL")).Value;

                refundModel.IsPreMigrationRefundRequest = true;
                refundModel.PreMigrationRequestMessage = string.Format("Please fill in the <a href=\"{0}\" target=\"_blank\">refund form</a> and send it to finance@naati.com.au.", refundForm);
            }

            return View(refundModel);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ValidatePayPalRefund(int credentialRequestId)
        {
            var refundStatusResponse = mRefundCalculator.CalculateRefund(credentialRequestId);

            // is the refund method paypal?
            if (refundStatusResponse.AvailableRefundMethodTypes.Contains(RefundMethodTypeName.PayPal))
            {
                // get if the paypal refund is valid
                var payPalRefundIsValid = _credentialApplicationRefundService.ValidatePayPalDateForRefund(credentialRequestId);

                // if not return false, otherwise continue the refund as normal
                if (!payPalRefundIsValid)
                {
                    return Json(false);
                }
            }

            return Json(true);
        }

        [HttpPost]
        [Authorize]
        public ActionResult RequestRefund(RefundRequestModel refundRequest)
        {
            if (!mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, refundRequest.CredentialRequestId))
            {
                return Json(new { data = "Invalid User" }, JsonRequestBehavior.AllowGet);
            }
            if (refundRequest.IsManualRefund && string.IsNullOrWhiteSpace(refundRequest.BankAccountDetails))
            {
                return Json(new { data = "Please enter the bank details" }, JsonRequestBehavior.AllowGet);
            }

            var refundStatusResponse = mRefundCalculator.CalculateRefund(refundRequest.CredentialRequestId);

            if (refundStatusResponse.RefundCalculationResultType != RefundCalculationResultTypeName.RefundAvailable)
            {
                return Json(new { data = "Failed to accept the refund request. Please try again or contact NAATI. <a href=\"mailto:info@naati.com.au ? subject = Fail to accept test date\">info@naati.com.au</a> so that they can resolve this issue." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                ExecuteAction(refundRequest.CredentialApplicationId, refundRequest.CredentialRequestId, null, 0, 0, refundStatusResponse.RefundPercentage, refundStatusResponse.AvailableRefundMethodTypes.First(), refundStatusResponse.CredentialWorkflowFeeId, null, null, SystemActionTypeName.RequestRefund, refundRequest.Comments, refundRequest.BankAccountDetails);
            }
            catch (UserFriendlySamException ex)
            {
                return Json(new { data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { data = "Failed to accept the refund request. Please try again or contact NAATI. <a href=\"mailto:info@naati.com.au ? subject = Fail to accept test date\">info@naati.com.au</a> so that they can resolve this issue." }, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        private string GetTestStartTime(DateTime? testDateTime, int arrivalTime)
        {
            if (testDateTime == null)
            {
                return string.Empty;
            }
            var updateTestTime = testDateTime.GetValueOrDefault().AddMinutes(-arrivalTime);
            var res = updateTestTime.ToString("hh:mm tt");

            var strTestStart = testDateTime.GetValueOrDefault().ToString("hh:mm tt") + " ( Arrive at " + res + " )";
            return strTestStart;
        }

        private string GetExpectedCompletionTime(DateTime? testDateTime, int duration)
        {
            if (testDateTime == null)
            {
                return string.Empty;
            }
            var updateTestTime = testDateTime.GetValueOrDefault().AddMinutes(duration);
            var res = updateTestTime.ToString("hh:mm tt");
            return res;
        }

        [HttpPost]
        [Authorize]
        public ActionResult AcceptTestSession(SelectTestSessionRequest request)
        {
            if (!mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, request.CredentialRequestId))
            {
                return Json(new { data = "Invalid User" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                ExecuteAction(request.CredentialApplicationId, request.CredentialRequestId, null, 0, request.TestSessionId, null, null, null, null, null, SystemActionTypeName.AllocateTestSessionFromMyNaati, null, null);
            }
            catch (UserFriendlySamException ex)
            {
                return Json(new { data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { data = "Failed to accept the test date. Please try again or conctact NAATI. <a href=\"mailto:info@naati.com.au ? subject = Fail to accept test date\">info@naati.com.au</a> so that they can resolve this issue." }, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [Authorize]
        public ActionResult RejectTestSession(int credentialRequestId, int credentialApplicationId)
        {
            if (!mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, credentialRequestId))
            {
                return Json(new { data = "Invalid User" }, JsonRequestBehavior.AllowGet);
            }

            var message = ExecuteNcmsWizardAction(credentialRequestId, credentialApplicationId, 1020);

            if (!string.IsNullOrEmpty(message))
            {
                return Json(new { data = "Failed to reject the test date. Please contact NAATI immediately on <a href=\"mailto:info@naati.com.au ? subject = Fail to accept test date\">info@naati.com.au</a> so that they can resolve this issue." }, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);

        }
        private string ExecuteNcmsWizardAction(int credentialRequestId, int credentialApplicationId, int actionId)
        {
            var request = new NcmsApplicationActionRequest { ApplicationId = credentialApplicationId, CredentialRequestId = credentialRequestId, ActionId = actionId, Steps = new List<object>() };
            var response = _ncmsIntegrationService.ExecuteNcmsApplicationAction(request);
            if (!response.Success)
            {
                return "General Error during validation occurred.";
            }

            return string.Empty;
        }

        private IEnumerable<string> GetErrors(HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            var errors = JsonConvert.DeserializeObject<IEnumerable<string>>(data);
            return errors;
        }

        [HttpPost]
        [Route("applications/applyforpaidreview/apply")]
        public ActionResult Submit(NextPaidReviewQuestionRequest request)
        {
            //TODO : REFACTOR ALL THIS AND CONSOLIDATE WITH SUBMIT APPLICATION OF CREDENTIAL APPLICATION CONTROLLER

            var test = mCredentialApplicationService.GetPaidReviewTestDetails(CurrentUserNaatiNumber, request.TestAttendanceId);
            return SubmitRequest(request, test, SystemActionTypeName.CreatePaidTestReview);
        }

        [HttpPost]
        [Route("applications/supplementarytest/apply")]
        public ActionResult SubmitSupplementaryTest(NextPaidReviewQuestionRequest request)
        {
            //TODO : REFACTOR ALL THIS AND CONSOLIDATE WITH SUBMIT APPLICATION OF CREDENTIAL APPLICATION CONTROLLER

            var test = mCredentialApplicationService.GetSupplementaryTestDetails(CurrentUserNaatiNumber, request.TestAttendanceId);
            return SubmitRequest(request, test, SystemActionTypeName.CreateSupplementaryTest);

        }

        #region Select Test Session
        [Route("applications/selecttestsession")]
        [HttpPost]
        public ActionResult SelectTestSession(SelectTestSessionRequest request)
        {
            if (!mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, request.CredentialRequestId))
            {
                return RedirectToAction("MyTests");
            }

            var allAvailableTestSessionsAndRejectableTestSessionsResponse = mCredentialApplicationService.GetAllAvailableTestSessionsAndRejectableTestSessions(request.CredentialRequestId);

            var allAvailableTestSessions = allAvailableTestSessionsAndRejectableTestSessionsResponse.AvailableTestSessions;

            if (allAvailableTestSessions.All(x => x.TestSessionId != request.TestSessionId))
            {
                ViewBag.ErrorMessage = "Test Session is Full";
                return View("ReasonError");
            }

            var payPalClientId = mSecretsProvider.Get(SecuritySettings.PayPalClientId);
            ViewBag.PayPalClientId = $"https://www.paypal.com/sdk/js?client-id={payPalClientId}&currency=AUD";

            return View(request);
        }

        [Route("applications/selecttestsession")]
        public ActionResult GetSelectTestSession(SelectTestSessionRequest request)
        {
            return Redirect("MyTests");
        }

        [HttpGet]
        [Route("applications/selecttestsession/settings")]
        public ActionResult SelectTestSessionSettings(int testSessionId, int credentialRequestId, int credentialApplicationId)
        {
            var result = mCredentialApplicationService.GetSelectTestSessionSections(CurrentUserNaatiNumber, testSessionId, credentialRequestId, credentialApplicationId);
            var sections = result.Results.Select(_autoMapperHelper.Mapper.Map<ApplicationFormSectionModel>).ToList();
            var application = _applicationQueryService.GetApplicationDetails(new F1Solutions.Naati.Common.Contracts.Dal.Request.GetApplicationDetailsRequest()
            {
                ApplicationId = credentialApplicationId,
                CredentialRequestId = credentialRequestId                
            });
            foreach (var s in sections)
            {
                s.Questions.Clear();
            }
            return Json(new
            {
                Sections = sections,
                DisablePayPalUi = mLookupProvider.SystemValues.DisablePayPalUi,
                Skill = application.CredentialRequests.FirstOrDefault(x=>x.Id == credentialRequestId).Skill.DisplayName,
            }, JsonRequestBehavior.AllowGet); ;
        }

        [HttpPost]
        [Route("applications/selecttestsession/nextquestion")]
        public ActionResult SelectTestSessionNextQuestion(NextSelectTestSessionQuestionRequest request)
        {
            var result = mCredentialApplicationService.GetSelectTestSessionSections(CurrentUserNaatiNumber, request.TestSessionId, request.CredentialRequestId, request.CredentialApplicationId);
            return GetNextQuestion(request, result);
        }

        [HttpGet]
        [Route("applications/selecttestsession/testdetail")]
        public ActionResult SelectTestSessionTestDetail(int testSessionId, int credentialRequestId, int credentialApplicationId)
        {
            var response = mCredentialApplicationService.GetSelectTestSessionTestDetails(testSessionId, credentialApplicationId, credentialRequestId);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("applications/selecttestsession/paymentcontrol")]
        public ActionResult SelectTestSessionPaymentControl(int testSessionId, int credentialRequestId, int credentialApplicationId)
        {
            var response = mCredentialApplicationService.GetSelectTestSessionTestDetails(testSessionId, credentialApplicationId, credentialRequestId);
            return Json(new { Sponsor = response.Sponsor, Fees = new[] { response.Fee } }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("applications/selecttestsession/authorisePayPal")]
        public ActionResult AuthorisePayPal(string amount,string reference,string unitType)
        {
            var apiContext = PayPalConfigurationHelper.GetAPIContext();
            string baseURI = $"{Request.Url.Scheme}://{Request.Url.IdnHost}{Request.Url.AbsolutePath}Succeeded?";
            //here we are generating guid for storing the paymentID received in session  
            //which will be used in the payment execution  
            var guid = Guid.NewGuid().ToString();
            //CreatePayment function gives us the payment approval url  
            //on which payer is redirected for paypal account payment  
            var createdPayment = mPayPalService.CreatePayment(apiContext, baseURI + "guid=" + guid, reference,unitType,amount);
            //get links returned from paypal in response to Create function call  
            var links = createdPayment.links.GetEnumerator();
            string paypalRedirectUrl = null;
            string token = null;
            string pattern = @"EC-\w+";
            while (links.MoveNext())
            {
                Links lnk = links.Current;
                if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                {
                    //saving the payapalredirect URL to which user will be redirected for payment  
                    paypalRedirectUrl = lnk.href;
                    Match m = Regex.Match(lnk.href, pattern, RegexOptions.IgnoreCase);
                    if (m.Success) token = m.Value;
                }
            }

            return Json(new { token = token });
        }


        [HttpPost]
        [Route("applications/selecttestsession/apply")]
        public ActionResult SelectTestSessionSubmit(NextSelectTestSessionQuestionRequest request)
        {
            var test = mCredentialApplicationService.GetSelectTestSessionTestDetails(request.TestSessionId, request.CredentialApplicationId, request.CredentialRequestId);
            var payOnlineResponse = new PayOnlineResponseModel();

            var questions = request.Form.Sections.SelectMany(x => x.Questions).ToList();
            var acceptedTermAndConditions = Convert.ToInt32(questions.Last().Response) == 1;
            if (!acceptedTermAndConditions || test == null)
            {

                var content = string.Format(RequestCancelled, Url.Action("MyTests"));
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

            if (!mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, request.CredentialRequestId))
            {
                return Json(new { data = "Invalid User" }, JsonRequestBehavior.AllowGet);
            }

            payOnlineResponse = SubmitPayment(request, test, payOnlineResponse, SystemActionTypeName.AllocateTestSessionFromMyNaati);

            string confirmationContent = GetConfirmationContent(test, payOnlineResponse, SystemActionTypeName.AllocateTestSessionFromMyNaati);

            var submitApplicationResponse = new SubmitApplicationResponse
            {
                SaveApplicationFormResponse = new SaveApplicationFormResponse
                {
                    ApplicationId = test.ApplicationId,
                    ApplicationReference = test.ApplicationReference,
                },
                PayOnlineResponse = payOnlineResponse,
                ConfirmContent = confirmationContent
            };

            return Json(submitApplicationResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("applications/selecttestsession/applyembedded")]
        public ActionResult SelectTestSessionSubmitEmbedded(NextSelectTestSessionQuestionRequest request)
        {
            var test = mCredentialApplicationService.GetSelectTestSessionTestDetails(request.TestSessionId, request.CredentialApplicationId, request.CredentialRequestId);
            var payOnlineResponse = new PayOnlineResponseModel();

            var questions = request.Form.Sections.SelectMany(x => x.Questions).ToList();
            var acceptedTermAndConditions = Convert.ToInt32(questions.Last().Response) == 1;
            if (!acceptedTermAndConditions || test == null)
            {

                var content = string.Format(RequestCancelled, Url.Action("MyTests"));
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

            if (!mCredentialApplicationService.CheckCredentialRequestBelongsToCurrentUser(CurrentUserNaatiNumber, request.CredentialRequestId))
            {
                return Json(new { data = "Invalid User" }, JsonRequestBehavior.AllowGet);
            }

            payOnlineResponse = PayIfApplicableAndAllocateTestSessions(request, test, payOnlineResponse, SystemActionTypeName.AllocateTestSessionFromMyNaati);

            string confirmationContent = GetConfirmationContent(test, payOnlineResponse, SystemActionTypeName.AllocateTestSessionFromMyNaati);

            var submitApplicationResponse = new SubmitApplicationResponse
            {
                SaveApplicationFormResponse = new SaveApplicationFormResponse
                {
                    ApplicationId = test.ApplicationId,
                    ApplicationReference = test.ApplicationReference,
                },
                PayOnlineResponse = payOnlineResponse,
                ConfirmContent = confirmationContent
            };

            return Json(submitApplicationResponse, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private ActionResult GetNextQuestion(NextQuestionRequest request, GetApplicationFormSectionsResponse result)
        {
            var sections = result.Results.Select(_autoMapperHelper.Mapper.Map<ApplicationFormSectionModel>).ToList();
            var currentSection = request.Form?.Sections?.FirstOrDefault(s => s.Id == request.SectionId);
            var lastQuestion = currentSection?.Questions?.LastOrDefault();
            var questions = sections.FirstOrDefault(s => currentSection == null || s.Id == currentSection.Id)?.Questions;
            var questionFound = lastQuestion == null ? true : false;
            var nextQuestion = new ApplicationFormQuestionModel();

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

        private ActionResult SubmitRequest(NextPaidReviewQuestionRequest request, TestFeeContract test, SystemActionTypeName action)
        {
            // TODO> REFACTOR THIS!!
            var payOnlineResponse = new PayOnlineResponseModel();

            var questions = request.Form.Sections.SelectMany(x => x.Questions).ToList();
            var acceptedTermAndConditions = Convert.ToInt32(questions.Last().Response) == 1;
            if (!acceptedTermAndConditions || test == null)
            {

                var content = string.Format(RequestCancelled, Url.Action("MyTestResults"));
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

            payOnlineResponse = SubmitPayment(request, test, payOnlineResponse, action);

            string confirmationContent = GetConfirmationContent(test, payOnlineResponse, action);

            var submitApplicationResponse = new SubmitApplicationResponse
            {
                SaveApplicationFormResponse = new SaveApplicationFormResponse
                {
                    ApplicationId = test.ApplicationId,
                    ApplicationReference = test.ApplicationReference,
                },
                PayOnlineResponse = payOnlineResponse,
                ConfirmContent = confirmationContent
            };

            return Json(submitApplicationResponse, JsonRequestBehavior.AllowGet);
        }

        private string GetConfirmationContent(TestFeeContract test, PayOnlineResponseModel payOnlineResponse, SystemActionTypeName action)
        {
            ISponsoredPaymentConfirmationMessages confirmationMessages;

            switch (action)
            {
                case SystemActionTypeName.CreateSupplementaryTest:
                    confirmationMessages = new SupplementaryTestConfirmationMessages();
                    break;

                case SystemActionTypeName.AllocateTestSessionFromMyNaati:
                    confirmationMessages = new MakeApplicationConfirmationMessages();
                    break;
                case SystemActionTypeName.CreatePaidTestReview:
                    confirmationMessages = new PaidTestReviewConfirmationMessages();
                    break;
                default:
                    throw new Exception("No email confirmation messages configured for action {action} ");

            }

            var confirmationContent = payOnlineResponse.IsException ? confirmationMessages.Error : !String.IsNullOrEmpty(test.Sponsor?.Contact) ?
                (test.Sponsor.Trusted ? confirmationMessages.TrustedPayer : confirmationMessages.NonTrustedPayer) :
                (payOnlineResponse.IsPayByCreditCard ? confirmationMessages.CreditCard : 
                (payOnlineResponse.IsPayByPayPal ? confirmationMessages.PayPal :confirmationMessages.CashDirectDeposit));

            return ReplaceTokens(confirmationContent, test, payOnlineResponse);
        }

        /// <summary>
        /// If the payment method is via credit card or pay pal then process the payment and then execute the AllocateAndAcceptTestSessionAction.
        /// If the application is sponsored then just execute the AllocateAndAcceptTestSessionAction.
        /// If none of the above apply, then error out.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="test"></param>
        /// <param name="payOnlineResponse"></param>
        /// <param name="action"></param>
        /// <returns>Model for the UI</returns>
        private PayOnlineResponseModel PayIfApplicableAndAllocateTestSessions(NextQuestionRequest request, TestFeeContract test, PayOnlineResponseModel payOnlineResponse, SystemActionTypeName action)
        {

            var feeQuestion = request.Form.Sections.SelectMany(x => x.Questions)
                            .FirstOrDefault(x => x.Type == (int)CredentialApplicationFormAnswerTypeName.PaymentControl);

            if (feeQuestion != null)
            {
                var answerResponse = feeQuestion.Response;
                var jsonSerializer = new JavaScriptSerializer();
                if (answerResponse != null)
                {
                    var feePaymentDetails = jsonSerializer.Deserialize<FeePaymentDetails>(answerResponse.ToString());
                    var errors = new List<string>();
                    switch (feePaymentDetails.PaymentMethodType)
                    {
                        case (int)PaymentMethodType.CreditCard:
                            errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty,
                                feePaymentDetails.Total, test.TestSessionId, action).ToList();
                            if (errors.Any())
                            {
                                payOnlineResponse.ErrorMessage = string.Join(";", errors);
                                payOnlineResponse.IsException = true;
                                return payOnlineResponse;

                            }
                            payOnlineResponse = PayCreditCard(CurrentUserNaatiNumber, test.ApplicationId, feePaymentDetails, feeQuestion.Id);
                            payOnlineResponse.IsPayByCreditCard = true;
                            if (payOnlineResponse.IsSecurePayStatusSuccess)
                            {
                                ExecuteAction(test.ApplicationId, test.CredentialRequestId, payOnlineResponse.SecurePayReference, feePaymentDetails.Total, test.TestSessionId, null, null, null, payOnlineResponse.OrderNumber, payOnlineResponse.SecurePayReference, action, null, null);
                            }
                            break;
                        case (int)PaymentMethodType.PayPal:
                            errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty,
                                feePaymentDetails.Total, test.TestSessionId, action).ToList();
                            if (errors.Any())
                            {
                                payOnlineResponse.ErrorMessage = string.Join(";", errors);
                                payOnlineResponse.IsException = true;
                                return payOnlineResponse;

                            }
                            payOnlineResponse = PayByPayPal(CurrentUserNaatiNumber, test.ApplicationId, feePaymentDetails, feeQuestion.Id);
                            payOnlineResponse.IsPayByPayPal = true;
                            if (payOnlineResponse.IsPayPalStatusSuccess)
                            {
                                ExecuteAction(test.ApplicationId, test.CredentialRequestId, payOnlineResponse.PayPalReference, feePaymentDetails.Total, test.TestSessionId, null, null, null, payOnlineResponse.OrderNumber, payOnlineResponse.SecurePayReference, action, null, null);
                            }
                            else
                            {
                                return payOnlineResponse;
                            }
                            break;
                        default:
                            if (!test.Sponsor.OrganisationName.IsNullOrEmpty())
                            {
                                errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty, feePaymentDetails.Total, test.TestSessionId, action).ToList();
                                if (errors.Any())
                                {
                                    payOnlineResponse.ErrorMessage = string.Join(";", errors);
                                    payOnlineResponse.IsException = true;
                                    return payOnlineResponse;
                                }
                                ExecuteAction(test.ApplicationId, test.CredentialRequestId, null, 0, test.TestSessionId, null, null, null, null, null, action, null, null);
                                break;
                            }
                            //only paypal, securepay, and sponsored are valid payment methods. If it gets to here then return an error
                            payOnlineResponse.ErrorMessage = "An error occurred during payment. Please try again";
                            payOnlineResponse.IsException = true;
                            return payOnlineResponse;
                    }
                }
            }
            else
            {
                var errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty, 0, test.TestSessionId, action);
                if (errors.Any())
                {
                    payOnlineResponse.ErrorMessage = string.Join(";", errors);
                    payOnlineResponse.IsException = true;
                    return payOnlineResponse;

                }
                ExecuteAction(test.ApplicationId, test.CredentialRequestId, null, 0, test.TestSessionId, null, null, null, null, null, action, null,null);
            }

            return payOnlineResponse;
        }

        private PayOnlineResponseModel SubmitPayment(NextQuestionRequest request, TestFeeContract test, PayOnlineResponseModel payOnlineResponse, SystemActionTypeName action)
        {

            var feeQuestion = request.Form.Sections.SelectMany(x => x.Questions)
                            .FirstOrDefault(x => x.Type == (int)CredentialApplicationFormAnswerTypeName.PaymentControl);

            if (feeQuestion != null)
            {
                var answerResponse = feeQuestion.Response;
                var jsonSerializer = new JavaScriptSerializer();
                if (answerResponse != null)
                {
                    var feePaymentDetails = jsonSerializer.Deserialize<FeePaymentDetails>(answerResponse.ToString());
                    switch (feePaymentDetails.PaymentMethodType)
                    {
                        case (int)PaymentMethodType.CreditCard:
                            var errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty,
                            feePaymentDetails.Total, test.TestSessionId, action);
                            if (errors.Any())
                            {
                                payOnlineResponse.ErrorMessage = string.Join(";", errors);
                                payOnlineResponse.IsException = true;
                                return payOnlineResponse;

                            }
                            payOnlineResponse = PayCreditCard(CurrentUserNaatiNumber, test.ApplicationId, feePaymentDetails, feeQuestion.Id);
                            payOnlineResponse.IsPayByCreditCard = true;
                            if (payOnlineResponse.IsSecurePayStatusSuccess)
                            {
                                ExecuteAction(test.ApplicationId, test.CredentialRequestId, payOnlineResponse.SecurePayReference, feePaymentDetails.Total, test.TestSessionId, null, null, null, payOnlineResponse.OrderNumber, payOnlineResponse.SecurePayReference, action, null,null);
                            }
                            break;
                        case (int)PaymentMethodType.PayPal:
                            errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty,
                                feePaymentDetails.Total, test.TestSessionId, action).ToList();
                            if (errors.Any())
                            {
                                payOnlineResponse.ErrorMessage = string.Join(";", errors);
                                payOnlineResponse.IsException = true;
                                return payOnlineResponse;

                            }
                            payOnlineResponse = PayByPayPal(CurrentUserNaatiNumber, test.ApplicationId, feePaymentDetails, feeQuestion.Id);
                            payOnlineResponse.IsPayByPayPal = true;
                            if (payOnlineResponse.IsPayPalStatusSuccess)
                            {
                                ExecuteAction(test.ApplicationId, test.CredentialRequestId, payOnlineResponse.PayPalReference, feePaymentDetails.Total, test.TestSessionId, null, null, null, payOnlineResponse.OrderNumber, payOnlineResponse.SecurePayReference, action, null, null);
                            }
                            break;
                        default:
                            errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty, feePaymentDetails.Total, test.TestSessionId, action);
                            if (errors.Any())
                            {
                                payOnlineResponse.ErrorMessage = string.Join(";", errors);
                                payOnlineResponse.IsException = true;
                                return payOnlineResponse;

                            }
                            ExecuteAction(test.ApplicationId, test.CredentialRequestId, null, 0, test.TestSessionId, null, null, null, null, null, action, null, null);
                            break;
                    }
                }
            }
            else
            {
                var errors = ValidateAction(test.ApplicationId, test.CredentialRequestId, string.Empty, string.Empty, 0, test.TestSessionId, action);
                if (errors.Any())
                {
                    payOnlineResponse.ErrorMessage = string.Join(";", errors);
                    payOnlineResponse.IsException = true;
                    return payOnlineResponse;

                }
                ExecuteAction(test.ApplicationId, test.CredentialRequestId, null, 0, test.TestSessionId, null, null, null, null, null, action, null,null);
            }

            return payOnlineResponse;
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

        private void ExecuteAction(int applicationId, int credentialRequestId, string paymentReference, decimal paymentAmount, int testSessionId, double? refundPercentage, RefundMethodTypeName? refundMethodTypeName, int? credentialWorkflowFeeId, string orderNumber, string transactionId, SystemActionTypeName action, string refundComments, string refundBankAccountDetails)
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
                TestSessionId = testSessionId,
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

        private string ReplaceTokens(string confirmationContent, TestFeeContract testDetails, PayOnlineResponseModel payOnlineResponse)
        {
            //TODO: CONSOLIDATE WITH CREDENTIAL APPLICATION CONTROLLER
            var constants = typeof(CredentialApplicationController.ConfimationContentTokens).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

            foreach (var constant in constants)
            {
                var token = Convert.ToString(constant.GetValue(null));
                var tokenValue = String.Empty;

                switch (token)
                {

                    case CredentialApplicationController.ConfimationContentTokens.ApplicationReference:
                        tokenValue = testDetails.ApplicationReference;
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.SecurepayReference:
                        tokenValue = payOnlineResponse.SecurePayReference;
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.PayPalReference:
                        tokenValue = payOnlineResponse.PayPalReference;
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.InvoiceNumber:
                        tokenValue = payOnlineResponse.WiiseInvoice;
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.CustomerNumber:
                        tokenValue = testDetails.NaatiNumber.ToString();
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.PrimaryEmail:
                        tokenValue = testDetails.ApplicantPrimaryEmail;
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.GivenName:
                        tokenValue = testDetails.ApplicantGivenName;
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.FamilyName:
                        tokenValue = testDetails.ApplicantFamilyName;
                        break;
                    case CredentialApplicationController.ConfimationContentTokens.ErrorMessage:
                        tokenValue = payOnlineResponse.ErrorMessage;
                        break;
                }

                confirmationContent = confirmationContent.Replace($"[[{token}]]", tokenValue ?? string.Empty);
            }

            return confirmationContent.Replace("[[Base Url]]", Url.Content("~/"));
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

        private IList<string> ValidateAction(int applicationId, int credentialRequestId, string paymentReference, string reference, decimal paymentAmount, int testSessionId, SystemActionTypeName action)
        {
            var obj = new NcmsApplicationActionRequest { ApplicationId = applicationId, CredentialRequestId = credentialRequestId, ActionId = (int)action, Steps = new object[] { }, DueDate = DateTime.Now.AddDays(3), InvoiceReference = reference, PaymentReference = paymentReference, PaymentAmount = paymentAmount, TestSessionId = testSessionId };
            return _ncmsIntegrationService.ValidateCredentialApplication(obj);
        }

    }

    public class NextSelectTestSessionQuestionRequest : NextQuestionRequest
    {
        public int TestSessionId { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }

    }

    public class NextQuestionRequest
    {
        public int SectionId { get; set; }
        public SaveApplicationFormRequestModel Form { get; set; }
    }

    public class NextPaidReviewQuestionRequest : NextQuestionRequest
    {
        public int TestAttendanceId { get; set; }

    }

    public class SelectTestSessionRequest
    {
        public int TestSessionId { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
    }
}
