using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Attributes;
using MyNaati.Ui.Helpers;
using MyNaati.Ui.Models;
using MyNaati.Ui.Security;
using MyNaati.Ui.ViewModels.Account;
using MyNaati.Ui.ViewModels.Home;
using TwoFactorAuthNet;

namespace MyNaati.Ui.Controllers
{
    public class AccountController : BaseController
    {

        private readonly IFormsAuthenticationService mFormsService;
        private readonly IMembershipProviderService mMembershipService;
        private readonly IEmailService mEmailService;
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly IUserService mUserService;
        private readonly IExaminerHelper mExaminerHelper;
        private readonly IRegisterHelper mRegisterHelper;
        private readonly ILookupProvider mLookupProvider;
        private readonly IConfigurationService mConfigurationService;
        private readonly IPasswordService mPasswordService;
        private readonly ICredentialApplicationService mApplicationService;
        private readonly IEmailCodeVerificationService mEmailCodeVerificationService;

        private const string GENERIC_ACCOUNT_ERROR = "There is a problem with your NAATI account.  One of the following may have occurred:|Your Customer number is not entered correctly; or|You are already registered to use myNAATI (you may try resetting your password using the link below); or|You have already submitted a registration request; or|Your information may not be available via myNAATI.|If your problem persists, contact NAATI for assistance.";
        private const string NOT_CANDIDATE_ERROR = "The Customer Number used to login is not of type Candidate. Please make sure that the person registering is a Candidate.";
        private const string APPROVE_ACCOUNT_ERROR_MSG = "Unable to create user.  The following problem occurred.\n \n{0}";
        private const string RESET_PASSWORD_EMAIL_FAILURE_MESSAGE = "There was a problem with the password reset.  Please try again later.  If the problem persists contact NAATI.";
        private const string GENERIC_USER_REGISTRATION_REQUEST_ERROR = "A problem occurred while registering you for myNAATI. Please contact NAATI.";
        private const string EMAIL_FAILURE_MESSAGE = "The confirmation email failed to send.";
        private const string USER_ALREADY_REGISTERED_ERROR = "User is already registered with myNAATI";
        private const string USER_ACCOUNT_CREATED_AND_EMAILED = "{0} has been registered for myNAATI.\n\nAn email has been sent to {1} with their new password.";
        private const string ACCESS_CODE_EMAIL_FAILURE_MESSAGE = "There was a problem sending the access code.  Please try again.  If the problem persists contact NAATI.";

        const int ADMINISTRATOR_ID = 1;

       

        public AccountController(IFormsAuthenticationService formsService,
                                 IMembershipProviderService membershipService,
                                 IPersonalDetailsService personalDetailsService,
                                 IEmailService emailService,
                                 IUserService userService,
                                 IExaminerHelper examinerHelper,
                                 IRegisterHelper registerHelper,
                                 ILookupProvider lookupService,
                                 IConfigurationService configurationService,
                                 IPasswordService passwordService,
                                 ICredentialApplicationService applicationService,
                                 IEmailCodeVerificationService emailCodeVerificationService)
        {
            mPasswordService = passwordService;
            mApplicationService = applicationService;
            mConfigurationService = configurationService;
            mFormsService = formsService;
            mMembershipService = membershipService;
            mPersonalDetailsService = personalDetailsService;
            mEmailService = emailService;
            mUserService = userService;
            mExaminerHelper = examinerHelper;
            mRegisterHelper = registerHelper;
            mLookupProvider = lookupService;
            mEmailCodeVerificationService = emailCodeVerificationService;
        }

        private ActionResult ValidateExaminer(string userName)
        {
            if (!mExaminerHelper.IsValidated(userName))
            {
                return RedirectToAction("ValidateExaminer", "Home");
            }

            return null;
        }

        [HttpGet]
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public static IEnumerable<MenuLinkModel> Menus()
        {
            var _urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

            var menuLinks = new List<MenuLinkModel>
            {
                {  new MenuLinkModel("Change My Password", _urlHelper.Action("ChangePassword", "Account"), MenuLinkModel.LinkIconType.Image, "change-my-password.png", index: 1) },
                { new MenuLinkModel("My Profile", _urlHelper.Action("Index", "PersonalDetails"), MenuLinkModel.LinkIconType.Image, "update-my-details.png", index: 2) },
                { new MenuLinkModel("Multi Factor Authentication", _urlHelper.Action("MultiFactorAuthentication", "Account"), MenuLinkModel.LinkIconType.Image, "mfa.png", index: 3) }
            };
            return menuLinks;
        }

        [HttpGet]
        public ActionResult MultiFactorAuthentication(int naatiNumber = 0)
        {
            if(naatiNumber == 0)
            {
                naatiNumber = CurrentUserNaatiNumber;
            }
            //at initialisation user is still not authenticated so cant get naatinumber
            if (naatiNumber == 0)
            {
                return View("Logon");
            }

            var result = mMembershipService.GetMfaDetails(naatiNumber);
            if(!result.Success)
            {
                //assume not configured
            }

            var model = new MfaModel()
            {
                mfaAlreadyConfigured = result.Data.MfaCode != null,
                naatiNumber = naatiNumber,
                email = result.Data.Email,
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult TurnOffMfa(int naatiNumber)
        {
            mMembershipService.DisableMfa(naatiNumber);

            //if currently logged on then show MFA screen. If not send back to logon
            if (CurrentUserNaatiNumber > 0)
            {
                var result = mMembershipService.GetMfaDetails(naatiNumber);

                LoggingHelper.LogInfo($"User {result.Data.Email} Successfully turned off MFA");
                return RedirectToAction("MultiFactorAuthentication", new { naatinumber = naatiNumber });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SetupMfa()
        {
            if(CurrentUserNaatiNumber == 0)
            {
                return View();
            }
            //need to get email address
            var personResponse = mPersonalDetailsService.GetPersonByNaatiNo(new NaatiNumberRequest() { NaatiNumber = CurrentUserNaatiNumber });
            var email = personResponse.Person.Email;
            var tfa = new TwoFactorAuth($"NAATI - {email}");
            var authCode = tfa.CreateSecret();
            var img = tfa.GetQrCodeImageAsDataUri($"NAATI", authCode);
            img = $"<img style='width: 200px; height: 200px' src='{img}'/>";
            var model = new MfaModel()
            {
                code = authCode,
                img = img,
                email = email
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult ReenterMfa(string returnUrl,int naatiNumber)
        {
            return View(new MfaModel() {returnUrl = returnUrl, naatiNumber = naatiNumber });
        }

        [HttpPost]
        [ActionName("ReenterMfa")]
        public ActionResult RenterMfaPost(string action, MfaAndAccessCodeModel model)
        {
            if(action == "CLOSE")
            {
                LoggingHelper.LogInfo("MfaModel Authentication abandoned", CurrentUserNaatiNumber, Request.UserHostAddress);

                return RedirectToAction("Index","Home");
            }
            var mfaDetailsResult = mMembershipService.GetMfaDetails(CurrentUserNaatiNumber);
            var personResponse = mPersonalDetailsService.GetPersonByNaatiNo(new NaatiNumberRequest() { NaatiNumber = CurrentUserNaatiNumber });
            if (!mfaDetailsResult.Success)
            {
                //error
                LoggingHelper.LogError("MfaModel Authentication errored", CurrentUserNaatiNumber, Request.UserHostAddress);
            }
            var person = personResponse.Person; //response doesnt have any success/fail?
            var mfaDetails = mfaDetailsResult.Data;

            var tfa = new TwoFactorAuth($"NAATI-{person.Email}");

            var timeSlices = mLookupProvider.SystemValues.MFACodeTimeoutSeconds / 60;

            //remove any whitespace
            model.MfaCode = Regex.Replace(model.MfaCode, @"\s+", "");

            var result = tfa.VerifyCode(mfaDetails.MfaCode, model.MfaCode, timeSlices);
            if (!result)
            {
                LoggingHelper.LogInfo("MfaModel Authentication invalid", CurrentUserNaatiNumber, Request.UserHostAddress);
                model.Error = "Authentication Error - The entered code is not valid.";
                return View("_MfaAndAccessCodePartial", model);
            }
            LoggingHelper.LogInfo("MfaModel Authentication succeeded", CurrentUserNaatiNumber, Request.UserHostAddress);
            //store 
            mMembershipService.SetMfaDetails(CurrentUserNaatiNumber, mfaDetails.MfaCode);

            return RedirectToAction(model.ReturnView, model.ReturnController);
        }

        [HttpPost]
        public ActionResult EnterMfaModal(string code, string key)
        {
            var personResponse = mPersonalDetailsService.GetPersonByNaatiNo(new NaatiNumberRequest() { NaatiNumber = CurrentUserNaatiNumber });
            var person = personResponse.Person; //response doesnt have any success/fail?

            //remove any whitespace
            code = Regex.Replace(code, @"\s+", "");

            var tfa = new TwoFactorAuth($"NAATI-{person.Email}");
            //allow discrepancy of 2 time slices
            var timeSlices = mLookupProvider.SystemValues.MFACodeTimeoutSeconds / 60;
            var result = tfa.VerifyCode(key, code, timeSlices);
            if(!result)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            //store 
            var response = mMembershipService.SetMfaDetails(CurrentUserNaatiNumber, key);

            LoggingHelper.LogInfo($"User { person.Email} Attempt to setup MFA.");
            if (!response.Success)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            LoggingHelper.LogInfo($"User { person.Email} Successfully turned on MFA.");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("EnterAccessCode")]
        public ActionResult EnterAccessCodeGet(AccessCodeModel model)
        {
            if (!model.codeSent)
            {
                SendEmailAccessCode(model);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult SendAccessCode(AccessCodeModel model)
        {
            SendEmailAccessCode(model);
            return RedirectToAction("EnterAccessCode", model);
        }

        private void SendEmailAccessCode(AccessCodeModel model)
        {
            var emailResponse = mEmailCodeVerificationService.SendEmailAccessCode(CurrentUserNaatiNumber);
            if (!emailResponse.Success)
            {
                model.error = ACCESS_CODE_EMAIL_FAILURE_MESSAGE;
                model.codeSent = false;
            }
            else
            {
                model.error = null;
                model.codeSent = true;
            }
        }

        [HttpPost]
        [ActionName("EnterAccessCode")]
        public ActionResult EnterAccessCodePost (AccessCodeModel model, string action)
        {
            if (action == "SEND NEW CODE")
            {
                SendEmailAccessCode(model);
                return View(model);
            }

            if(action == "CLOSE")
            {
                return RedirectToAction(model.returnView, model.returnController);
            }

            var code = 0;
            if(!int.TryParse(model.code, out code))
            {
                LoggingHelper.LogError($"Error verifying Access Code- {model.code} not an integer");
                return View(new AccessCodeModel()
                {
                    error = "Please check the code and try again."
                });
            }

            var verifyResponse = mEmailCodeVerificationService.VerifyEmailVerificationCode(CurrentUserNaatiNumber, code);
            if (!verifyResponse.Success)
            {
                LoggingHelper.LogError("Error verifying Access Code; NAATI number: {NaatiNumber}; {Error}", CurrentUserNaatiNumber, verifyResponse.Errors.FirstOrDefault());
                return View(new AccessCodeModel()
                {
                    error = "An error occurred. Please check the code and try again. If this problem persists, please contact NAATI."
                });
            }
            var success = verifyResponse.Data;
            if (!success)
            {
                return View(new AccessCodeModel()
                {
                    error = verifyResponse.Messages.FirstOrDefault()
                });
            }

            return RedirectToAction(model.returnView,model.returnController);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                //returning the model doesnt show the screen correctly.
                //instead set the models values
                //return View(model);
                model.CredentialsRejected = true;
            }

            if (mMembershipService.IsLockedOut(model.UserName))
            {
                return RedirectToAction("ResetPassword", "Account");
            }

            if (mMembershipService.ValidateUser(model.UserName, model.Password))
            {

                var user = mMembershipService.GetUser(model.UserName, true);
                var naatiNumber = mUserService.GetUser(user?.UserId ?? new Guid()).NaatiNumber;

                var accessDisabledByNaatiResponse = mMembershipService.GetAccessDisabledByNcms(naatiNumber);

                if(!accessDisabledByNaatiResponse.Success)
                {
                    //error
                }

                var accessDisabledByNaati = accessDisabledByNaatiResponse.Data;

                if(accessDisabledByNaati)
                {
                    LogLogin(model.UserName, false);

                    model.AccessDisabledByNaati = true;
                    model.MessageOfTheDay = mLookupProvider.SystemValues.MessageOfTheDay;
                    model.MyNaatiAvailable = mLookupProvider.SystemValues.MyNaatiAvailable == "true";
                    model.ShowMessageOfTheDay = mLookupProvider.SystemValues.ShowMessageOfTheDay == "true";

                    return View(model);
                }

                LogLogin(model.UserName, true);

                var numberofDays = (int)(DateTime.Now - user.LastPasswordChangedDate).TotalDays;
                var mfaDetailsResult = mMembershipService.GetMfaDetails(naatiNumber);
                if(!mfaDetailsResult.Success)
                {
                    //error
                }
 
                mFormsService.SignIn(model.UserName, model.RememberMe);

                var serviceRolesProvider = new ServiceRolesProvider();
                var userRole = serviceRolesProvider.GetRolesForUser(model.UserName);

                mExaminerHelper.LoadExaminerRoles(model.UserName, naatiNumber);
                mFormsService.LoadPractitioner(model.UserName, naatiNumber);
                mFormsService.LoadRecertification(model.UserName, naatiNumber);
              
                if (userRole.Any(role => role.Equals(SystemRoles.TEMPORARYPASSWORDUSERS)))
                {
                    return RedirectToAction("ChangePassword", "Account");
                }

                int? days = null;
                foreach (var role in userRole)
                {
                    var roleExpiryDate = GetExpiryDay(role);
                    days = Math.Min(days ?? int.MaxValue, roleExpiryDate);
                }

                days = days ?? mConfigurationService.OtherExpiryDay();
                if (numberofDays > days)
                {
                    Roles.AddUserToRole(user.Username, SystemRoles.TEMPORARYPASSWORDUSERS);
                    return RedirectToAction("ChangePassword", "Account", new { reason = $"Your password has expired as it has been more than {days} days since you last changed your password." });
                }


                var validateExaminerResult = ValidateExaminer(model.UserName);
                if (validateExaminerResult != null)
                {
                    return validateExaminerResult;
                }

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");

            }

            LogLogin(model.UserName, false);

            model.CredentialsRejected = true;
            model.MessageOfTheDay = mLookupProvider.SystemValues.MessageOfTheDay;
            model.MyNaatiAvailable = mLookupProvider.SystemValues.MyNaatiAvailable == "true";
            model.ShowMessageOfTheDay = mLookupProvider.SystemValues.ShowMessageOfTheDay == "true";

            if (mMembershipService.IsLockedOut(model.UserName))
                model.CredentialLockedOut = true;

            return View(model);
        }

        private void LogLogin(string username, bool successful)
        {
            var forwardedFor = GetForwardedFor();
            var template = successful
                ? "User {UserName} signing into myNAATI from {IpAddress}"
                : "Refused myNAATI login to {UserName} from {IpAddress}";

            if (forwardedFor == null)
            {
                LoggingHelper.LogInfo(template, username, Request.UserHostAddress);
            }
            else
            {
                template += " (forwarded for {ForwardedFor})";
                LoggingHelper.LogInfo(template, username, Request.UserHostAddress, forwardedFor);
            }
        }

        private string GetForwardedFor()
        {
            var forwardedFor = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !String.IsNullOrEmpty(forwardedFor) ? forwardedFor.Split(',')[0] : null;
        }

        private int GetExpiryDay(string role)
        {
            switch (role)
            {
                case SystemRoles.EXAMINER:
                case SystemRoles.CHAIR:
                case SystemRoles.AUTHENTICATED_EXAMINER:
                    return mConfigurationService.ExaminerExpiryDay();
                case SystemRoles.PRACTITIONER:
                case SystemRoles.FUTUREPRACTITIONER:
                case SystemRoles.FORMERPRACTITIONER:
                    return mConfigurationService.PractitionerExpiryDay();
                default:
                    return mConfigurationService.OtherExpiryDay(); 
            }
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            InvalidateCookie();
            mFormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        private void InvalidateCookie()
        {
            var naatiNumber = CurrentUserNaatiNumber;
            var cookie = Request.Cookies["MYNAATI"]?.Value;

            if (cookie != null)
            {
                var formsTicket = FormsAuthentication.Decrypt(cookie);
                if (formsTicket != null && !formsTicket.Expired)
                {
                    Startup.RefreshUserCache(naatiNumber, cookie, formsTicket.Expiration);
                }
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            var model = GetRegisterModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult RegisterError(string errorMessage, string redirectUrl, string pageMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            ViewBag.RedirectUrl = redirectUrl;
            ViewBag.PageMessage = pageMessage;
            if (Url.IsLocalUrl(ViewBag.RedirectUrl))
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "HomeController");
            }
        }

        [HttpGet]
        public ActionResult RegisterSuccess(string email)
        {
            ViewBag.RegisterEmail = email;
            return View();
        }
        
        [HttpPost]
        [MvcRecaptchaValidation(modelErrorKeys: (nameof(NonCandidateRegisterModel.Email)))]
        public ActionResult Register(NonCandidateRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(GetRegisterModel(model));
            }

            if (!model.DetailsRequired)
            {
                var naatiNumbers = mApplicationService.GetPersonNaatiNumber(model.Email).ToList();
                if (naatiNumbers.Count() > 1)
                {
                    return RedirectToErrorPage($"There is more than one user identified with your email. Please try again or contact NAATI",
                        nameof(HomeController.Index),
                        nameof(HomeController), "GO TO LOGON PAGE");

                }
                if (!naatiNumbers.Any())
                {
                    model.DetailsRequired = true;
                    return View(GetRegisterModel(model));
                }

                model.NaatiNumber = naatiNumbers.First().ToString();

            }
            else
            {
                var verifyResponse = mApplicationService.VerifyPerson(new VerifyPersonRequestContract()
                {
                    DateOfBirth = model.DateOfBirth,
                    FamilyName = model.FamilyName,
                    Email = model.Email,
                    FirstName = model.GivenName,
                    MiddleNames = model.MiddleNames,
                    Title = model.Title
                });
                if (!string.IsNullOrEmpty(verifyResponse.Message))
                {
                    return RedirectToErrorPage(verifyResponse.Message,
                        nameof(HomeController.Index),
                        nameof(HomeController), "GO TO LOGON PAGE");
                }

                model.NaatiNumber = verifyResponse.NaatiNumber.ToString();
            }

            if (model.NaatiNumber != null)
            {
                var existingUser = mApplicationService.GetPersonDetails(int.Parse(model.NaatiNumber));

                if (existingUser.Deceased)
                {
                    return RedirectToErrorPage("This email address is currently blocked for registration. Please contact NAATI for further info.",
                        nameof(HomeController.Index),
                        nameof(HomeController), "GO TO LOGON PAGE");
                }
            }

            var response = mRegisterHelper.CreateUser(int.Parse(model.NaatiNumber), model.Email);
            if (response.CreateStatus == MembershipCreateStatus.Success && response.EmailSuccess)
            {
                return RedirectToAction(nameof(RegisterSuccess), nameof(AccountController).Replace("Controller", string.Empty), new { email = model.ConfirmEmail });
            }

            if (response.CreateStatus == MembershipCreateStatus.DuplicateUserName)
            {
                return RedirectToErrorPage("You are already registered on myNAATI",
                    nameof(HomeController.Index),
                    nameof(HomeController), "GO TO LOGON PAGE");

            }

            return RedirectToErrorPage(GENERIC_USER_REGISTRATION_REQUEST_ERROR,
                nameof(AccountController.Register),
                nameof(AccountController), "TRY AGAIN");

        }

        private RedirectToRouteResult RedirectToErrorPage(string errorMessage, string nextRedirecAction, string nextRedirectController, string pageMessage)
        {
            var redirectUrl = Url.Action(nextRedirecAction, nextRedirectController.Replace("Controller", string.Empty), this.Request?.Url?.Scheme);
            return RedirectToAction(nameof(RegisterError), nameof(AccountController).Replace("Controller", string.Empty), new { errorMessage, redirectUrl, pageMessage });
        }


        [HttpGet]
        public ActionResult ManualRegister()
        {
            return View();
        }
        
        private NonCandidateRegisterModel GetRegisterModel(NonCandidateRegisterModel model = null)
        {
            model = model ?? new NonCandidateRegisterModel();
            var titles = mLookupProvider.PersonTitles.OrderBy(o => o.DisplayText).ToList();
            var countries = mLookupProvider.Countries.OrderBy(o => o.DisplayText).ToList();

            model.TitleList = new List<SelectListItem>
            {
                new SelectListItem { Text = string.Empty, Value = "-1"}
            };

            model.Countries = new List<SelectListItem>
            {
                new SelectListItem { Text = string.Empty, Value = "-1" }
            };

            titles.ForEach(i =>
            {
                model.TitleList.Add(new SelectListItem { Selected = false, Text = i.DisplayText, Value = i.SamId.ToString()});
            });

            countries.ForEach(i =>
            {
                model.Countries.Add(new SelectListItem { Selected = false, Text = i.DisplayText, Value = i.SamId.ToString()});
            });

            model.GenderList = new List<SelectListItem>
            {
                new SelectListItem { Selected = false, Text = "Male", Value = "M" },
                new SelectListItem { Selected = false, Text = "Female", Value = "F" },
                new SelectListItem { Selected = false, Text = "Unspecified", Value = "U" }
            };

            model.GmailWhiteListUrl = mLookupProvider.SystemValues.GmailWhitelistUrl;

            return model;
        }


        [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
        [HttpGet]
        public ActionResult Registrations()
        {
            return View();
        }

        public ActionResult DuplicateUserResolution(string givenNameDto, string familyNameDto, string birthDateDto)
        {
            var birthDate = DateTime.Parse(birthDateDto);
            var duplicatesResponse = mPersonalDetailsService.FindByNameAndBirthDate(givenNameDto, familyNameDto, birthDate);

            var model = new DuplicateUserResolutionModel
            {
                DuplicateUserList = duplicatesResponse.People.Select(
                    p => new DuplicateUserModel
                    {
                        NaatiNumber = p.NaatiNumber.ToString(),
                        GivenName = p.GivenName,
                        FamilyName = p.Surname,
                        IsEportalActive = p.IsEportalActive,
                        DateOfBirth = p.BirthDate,
                        Email = string.IsNullOrEmpty(p.Email) ? "none" : p.Email
                    }).ToList()
            };

            return View(model);
        }

        public string ApproveErrorMessage(MembershipCreateStatus createStatus)
        {
            var result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, createStatus);

            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "Username already exists.");
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "A username for that e-mail address already exists.");
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "The password provided is invalid.");
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "The e-mail address provided is invalid.");
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "The password retrieval answer provided is invalid.");
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "The password retrieval question provided is invalid.");
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "The user name provided is invalid.");
                    break;
                case MembershipCreateStatus.ProviderError:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "The authentication provider returned an error.");
                    break;
                case MembershipCreateStatus.UserRejected:
                    result = string.Format(APPROVE_ACCOUNT_ERROR_MSG, "The user creation request has been canceled.");
                    break;
            }

            return result;
        }



        [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
        [HttpGet]
        public ActionResult UserSearch()
        {
            return View();
        }

        //[AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
        //[HttpPost]
        //public ActionResult UserSearch(UserSearchModel model, Parameters dataTableParameters)
        //{
        //    var results = new List<object>();

        //    var naatiNumber = -1;
        //    if (!string.IsNullOrEmpty(model.NaatiNumber))
        //    {
        //        int.TryParse(model.NaatiNumber, out naatiNumber);
        //    }

        //    //  Get the reigistration requests from the ePortal database.
        //    var criteria = new EPortalSamUserSearchCriteria
        //    {
        //        NaatiNumber = naatiNumber != -1 ? (int?)naatiNumber : null,
        //        FamilyName = model.FamilyName,
        //        GivenName = model.GivenName,
        //        Email = model.Email,
        //        CreationDateFrom = model.CreatedDateFrom,
        //        CreationDateTo = model.CreatedDateTo,
        //        Start = dataTableParameters.Start,
        //        Length = dataTableParameters.Length,
        //        Sort = dataTableParameters.GetSortDirection()
        //    };

        //    if (!ValidateUserSearch(criteria))
        //    {
        //        return Json(new
        //        {
        //            error = string.Format("The date created from {0:dd/MM/yyyy} must be on or before the to date {1:dd/MM/yyyy}", criteria.CreationDateFrom, criteria.CreationDateTo)
        //        });
        //    }

        //    var searchResults = mEPortalUserService.Search(criteria);

        //    //  Get the data from the ePortal membership service that indicates if the user is active(approived)
        //    //  based on the Customer numbers from the previous search.
        //    var ePortalUsers = new ePortalUser[0];

        //    if (searchResults.Results.Any())
        //    {
        //        ePortalUsers = mMembershipService.GetUsers(searchResults.Results.Select(u => u.NaatiNumber.ToString()).ToArray());
        //    }

        //    //  start return the combined results to the page.
        //    results = searchResults.Results.Select(x => (object)new
        //    {
        //        x.NaatiNumber,
        //        x.FullName,
        //        x.Deceased,
        //        x.Email,
        //        WebAccountCreateDate = x.WebAccountCreateDate == null ? "" : Convert.ToDateTime(x.WebAccountCreateDate).ToString("dd MMM yyyy hh:mm tt"),
        //        Action = ePortalUsers.Any(u => u.IsActive && x.NaatiNumber.ToString().Equals(u.Username)) ? "Deactivate" : "Activate",
        //        ActionUrl = (ePortalUsers.Any(u => u.IsActive && x.NaatiNumber.ToString().Equals(u.Username)) ? Url.Action("UserDeactivate") : Url.Action("UserActivate")) + "/" + x.NaatiNumber
        //    }).ToList();

        //    var totalResultCount = searchResults.TotalResultsCount;

        //    return Json(new Result<object>
        //    {
        //        Draw = dataTableParameters.Draw,
        //        Total = totalResultCount,
        //        Filtered = totalResultCount,
        //        Data = results
        //    }.ToObject());
        //}

        private static bool ValidateUserSearch(EPortalSamUserSearchCriteria criteria)
        {
            var result = true;

            try
            {
                //if the creation date from is not null
                if (!(criteria.CreationDateFrom.Day == 1 && criteria.CreationDateFrom.Month == 1 && criteria.CreationDateFrom.Year == 1))
                {
                    if (!(criteria.CreationDateTo.Day == 1 && criteria.CreationDateTo.Month == 1 && criteria.CreationDateTo.Year == 1))
                    {
                        //creation date from and to
                        if (criteria.CreationDateFrom > criteria.CreationDateTo)
                        {
                            result = false;
                        }
                    }
                }
            }
            catch
            {
                // do nothing - error is suppressed
            }

            return result;
        }

        //[AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
        //[HttpPost]
        //public ActionResult ExportUser(UserSearchModel model, Parameters dataTableParameters)
        //{
        //    var naatiNumber = -1;
        //    if (!string.IsNullOrEmpty(model.NaatiNumber))
        //    {
        //        int.TryParse(model.NaatiNumber, out naatiNumber);
        //    }

        //    //  Get the reigistration requests from the ePortal database.
        //    var criteria = new EPortalSamUserSearchCriteria
        //    {
        //        NaatiNumber = naatiNumber != -1 ? (int?)naatiNumber : null,
        //        FamilyName = model.FamilyName,
        //        GivenName = model.GivenName,
        //        Email = model.Email,
        //        CreationDateFrom = model.CreatedDateFrom,
        //        CreationDateTo = model.CreatedDateTo,
        //        Start = 0,
        //        Length = -1,
        //        Sort = dataTableParameters.GetSortDirection()
        //    };

        //    var searchResults = mEPortalUserService.Search(criteria);

        //    var bytes = mUserCsvGenerator.GenerateUserCsv(searchResults.Results);

        //    var fileName = new StringBuilder();

        //    fileName.Append("ExportedUser");

        //    if (model.CreatedDateFrom != DateTime.MinValue)
        //    {
        //        fileName.AppendFormat("-{0:ddMMyyyy}", model.CreatedDateFrom);
        //    }

        //    if (model.CreatedDateTo != DateTime.MinValue)
        //    {
        //        fileName.AppendFormat("-{0:ddMMyyyy}", model.CreatedDateTo);
        //    }

        //    fileName.Append(".csv");

        //    return File(bytes, "text/csv", fileName.ToString());
        //}
        
        [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
        [HttpPost]
        public void UserActivate(string id)
        {
            mMembershipService.ActivateUser(id);
        }

        [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
        [HttpPost]
        public void UserDeactivate(string id)
        {
            mMembershipService.DeactivateUser(id);
        }


        [Authorize]
        [HttpGet]
        public ActionResult ChangePassword(string reason)
        {
            ViewBag.PasswordLength = mConfigurationService.MinimumPasswordLength();
            ViewBag.Reason = reason;
            return View();
        }

        public ActionResult ChangeLogOnEmailError(string errorMessage, string redirectUrl, string pageMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            ViewBag.RedirectUrl = redirectUrl;
            ViewBag.PageMessage = pageMessage;

            if (Url.IsLocalUrl(ViewBag.RedirectUrl))
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "HomeController");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangeLogOnEmail(int reference)
        {
            int emailChangeValidHours = Convert.ToInt32(ConfigurationManager.AppSettings["EmailChangeValidHours"]);
            var result = mRegisterHelper.GetRegisteredEmailChange(reference, emailChangeValidHours);

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                var errorMessage = result.ErrorMessage;
                var redirectUrl = ViewBag.RedirectUrl = Url.Action(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", string.Empty), this.Request?.Url?.Scheme);
                var pageMessage = "GO TO HOME PAGE";
                return RedirectToAction(nameof(ChangeLogOnEmailError), nameof(AccountController).Replace("Controller", string.Empty), new { errorMessage, redirectUrl, pageMessage });
            }

            var model = new ChangePrimaryEmailModel { Reference = reference };
            ViewBag.PrimaryEmail = result.PrimaryEmail;
            ViewBag.SecondaryEmail = result.SecondaryEmail;
            return View(model);
        }

        [HttpGet]
        public ActionResult EmailLogonChangeSuccess(string newEmail)
        {
            ViewBag.NewEmail = newEmail;
            ViewBag.RedirectUrl = Url.Action(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", string.Empty), this.Request?.Url?.Scheme);
            if (Url.IsLocalUrl(ViewBag.RedirectUrl))
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "HomeController");
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangeLogOnEmail(ChangePrimaryEmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            int emailChangeValidHours = Convert.ToInt32(ConfigurationManager.AppSettings["EmailChangeValidHours"]);
            var result = mRegisterHelper.ChangeLogOnEmail(model.Reference, model.Password, emailChangeValidHours);

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                ModelState.AddModelError("Error", result.ErrorMessage);
                return View(model);
            }

            return RedirectToAction(nameof(EmailLogonChangeSuccess), new { newEmail = result.NewLogonEmail });

        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = mMembershipService.GetUser(User.Identity.Name, true);
                if (
                    mPasswordService.ExistedPasswordHistory(new PasswordHistoryRequest()
                    {
                        DeleteCount = mConfigurationService.NumberPasswordsStore(),
                        Password = model.NewPassword,
                        UserId = user.UserId
                    }))
                {
                    ModelState.AddModelError("NewPassword", $@"Passwords cannot match the last {mConfigurationService.NumberPasswordsStore()} passwords used");
                    return View(model);
                }

                if (mMembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    var serviceRolesProvider = new ServiceRolesProvider();
                    var userRole = serviceRolesProvider.GetRolesForUser(User.Identity.Name);
                    mExaminerHelper.LoadExaminerRoles(User.Identity.Name, CurrentUserNaatiNumber);
                    mFormsService.LoadPractitioner(User.Identity.Name, CurrentUserNaatiNumber);
                    mPasswordService.SavePasswordHistory(new PasswordHistoryRequest()
                    {
                        DeleteCount = mConfigurationService.NumberPasswordsStore(),
                        UserId = user.UserId,
                        Password = mPasswordService.HashPassword(user.UserId.ToString(), model.NewPassword)
                    });

                    var validateExaminerResult = ValidateExaminer(User.Identity.Name);
                    if (validateExaminerResult != null)
                    {
                        TempData["ChangePasswordSuccess"] = true;
                        return validateExaminerResult;
                    }

                    return RedirectToAction("ChangePasswordSuccess");
                }
                ModelState.AddModelError("OldPassword", @"Current password might be incorrect.");
                ModelState.AddModelError("NewPassword", $@"Passwords must be at least {mConfigurationService.MinimumPasswordLength()} characters");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = mConfigurationService.MinimumPasswordLength();
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangePasswordSuccess()
        {
            @ViewBag.RedirectUrl = Url.Action(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", string.Empty), this.Request?.Url?.Scheme);

            if (Url.IsLocalUrl(ViewBag.RedirectUrl))
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "HomeController");
            }
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            var model = new ResetPasswordModel();
            model.GmailWhiteListUrl = mLookupProvider.SystemValues.GmailWhitelistUrl;
            return View(model);
        }

        [HttpPost]
        [MvcRecaptchaValidation(modelErrorKeys: (nameof(ResetPasswordModel.Email)))]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user= mMembershipService.GetUser(model.Email, false);
            if (user == null)
            {
                ModelState.AddModelError("ValidationError", "No account found with this Customer Number and Email Address.");
                return View(model);
            }

            var webUser = mUserService.GetUser(user.UserId);

            int naatiNumber;
            if (!int.TryParse(model.NaatiNumber, out naatiNumber) || webUser.NaatiNumber != naatiNumber || naatiNumber == 0)
            {
                ModelState.AddModelError("ValidationError", "No account found with this Customer Number and Email Address.");
                return View(model);
            }
            //  Perform processing.
            var newPassword = mMembershipService.ResetPassword(model.Email);

            if (!string.IsNullOrEmpty(newPassword))
            {
                var personRequest = new PersonNaatiNumberRequest { NaatiNumber = naatiNumber};
                var personResponse = mPersonalDetailsService.GetPerson(personRequest);

                //  Send an email to the user advising of their new password.
                var emailRequest = new SendEmailRequest(EmailTemplate.ResetPassword, model.Email);
                emailRequest.Tokens.Add(EmailTokens.Password, newPassword);
                emailRequest.Tokens.Add(EmailTokens.GivenName, personResponse.Person.GivenName);
                emailRequest.Tokens.Add(EmailTokens.NaatiNumber, personResponse.Person.NaatiNumber.ToString());
                var emailResponse = mEmailService.SendMail(emailRequest);

                if (emailResponse.Success)
                    return RedirectToAction("ResetPasswordSuccess", new { PasswordEmail = model.Email });

                ModelState.AddModelError("", RESET_PASSWORD_EMAIL_FAILURE_MESSAGE);
            }
            else
            {
                // If we got this far, then the reset failed so add the standard error messages regardless of the problem.
                ModelState.AddModelError("", "Your Customer Number is not registered for myNAATI; or");
                ModelState.AddModelError("", "Your Customer Number was incorrect; or");
                ModelState.AddModelError("", "Your email is not the one registered with us.  If your problem persists, contact NAATI for assistance.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpGet]
        public ActionResult ResetPasswordSuccess(string passwordEmail)
        {
            ViewBag.PasswordEmail = passwordEmail;
            return View();
        }

    }


    public class GetRegisteredEmailChangeResponse
    {
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class ChangeLogonEmailResponse
    {
        public string NewLogonEmail { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CreateUserResponse
    {
        public MembershipCreateStatus CreateStatus { get; set; }
        public bool EmailSuccess { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Credentialed { get; set; }
    }

    public class AcceptParameterAttribute : ActionMethodSelectorAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var req = controllerContext.RequestContext.HttpContext.Request;
            return req.Form[Name] == Value;
        }
    }
}