using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Ui.Security;
using MyNaati.Ui.UI;
using MyNaati.Ui.ViewModels.Home;

namespace MyNaati.Ui.Helpers
{
    public class MenuHelper : IMenuHelper
    {
        private readonly UrlHelper _urlHelper;
        private readonly IPersonalDetailsService _personalDetailsService;
        private readonly ICredentialApplicationService _credentialApplicationService;
        private readonly IAccountingService _accountingService;
        private readonly ISystemQueryService _systemQueryService;
        private readonly IDictionary<string, MenuLinkModel> _menuLinks;
        private bool IsRolePlayerEnabled;

        public MenuHelper(IPersonalDetailsService personalDetailsService, ICredentialApplicationService credentialApplicationService, IAccountingService accountingService, ISystemQueryService systemQueryService)
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            _personalDetailsService = personalDetailsService;
            _credentialApplicationService = credentialApplicationService;
            _accountingService = accountingService;
            _systemQueryService = systemQueryService;
            IsRolePlayerEnabled = _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "RolePlayerAvailable" }).Value == "true"? true:false;

            var index = 0;

			_menuLinks = new Dictionary<string, MenuLinkModel>
            {
                { "My Digital ID Mobile Card", new MenuLinkModel("My Digital ID Card", _urlHelper.Action("DigitalIDCard", "Credential"), MenuLinkModel.LinkIconType.Image, "mfa.png", index: index++, id: "MobileCredentialCards") },
                { "My Account", new MenuLinkModel("My Account", _urlHelper.Action("Index", "Account"), MenuLinkModel.LinkIconType.Image, "my-account.png", index: index++) },
                { "Manage Users", new MenuLinkModel("Manage Users", _urlHelper.Action("UserSearch", "Account"), MenuLinkModel.LinkIconType.Image, "ManageUsers.png", index: index++) },
                { "Manage Requests", new MenuLinkModel("Manage Requests", _urlHelper.Action("Registrations", "Account"), MenuLinkModel.LinkIconType.Image, "ManageRequests.png", index: index++) },
				{ "Portal Statistics", new MenuLinkModel("Portal Statistics", _urlHelper.Action("PortalStatistics", "Orders"), MenuLinkModel.LinkIconType.Image, "PortalStatistics.png", index: index++) },
				{ "Export Details", new MenuLinkModel("Export Details", _urlHelper.Action("ExportOnlineOrderDetails", "Orders"), MenuLinkModel.LinkIconType.Image, "ExportDetails.png", index: index++) },
				{ "Manage Form Templates", new MenuLinkModel("Manage Form Templates", _urlHelper.Action("UploadFile", "File"), MenuLinkModel.LinkIconType.Image, "ManageFormTemplates.png", index: index++) },
				{ "System Settings", new MenuLinkModel("System Settings", _urlHelper.Action("SystemSettings", "Configuration"), MenuLinkModel.LinkIconType.Image, "SystemSettings.png", index: index++) },
				//{ "Update My Details", new MenuLinkModel("Update My Details", _urlHelper.Action("Index", "PersonalDetails"), MenuLinkModel.LinkIconType.Image, "update-my-datails.png", index: index++) },
                //{ "Change My Password", new MenuLinkModel("Change My Password", _urlHelper.Action("ChangePassword", "Account"), MenuLinkModel.LinkIconType.Image, "change-my-password.png", index: index++) },
                { "My Logbook", new MenuLinkModel("My Logbook", _urlHelper.Action("Index", "Logbook"), MenuLinkModel.LinkIconType.Image, "my-logbook.png", index: index++) },
				{ "Manage My Tests", new MenuLinkModel("Manage My Tests", _urlHelper.Action("MyTests", "Applications"), MenuLinkModel.LinkIconType.Image, "manage-my-tests.png", index: index++) },
				{ "My Test Results", new MenuLinkModel("My Test Results", _urlHelper.Action("MyTestResults", "Applications"), MenuLinkModel.LinkIconType.Image, "my-test-results.png", index: index++) },
				{ "My Credentials", new MenuLinkModel("My Credentials", _urlHelper.Action("Index", "Credential"), MenuLinkModel.LinkIconType.Image, "my-credentials.png", index: index++) },
                { "Examiner Tools", new MenuLinkModel("Examiner and Role-player Tools", _urlHelper.Action("Index", "ExaminerTools"), MenuLinkModel.LinkIconType.Image, "ExaminerToolsMenu.png", index: index++) },
                { "My Bills", new MenuLinkModel("My Transactions", _urlHelper.Action("Index", "Bills"), MenuLinkModel.LinkIconType.Image, "my-test-material-dev.svg", index: index++) },
				{ "Apply for Certification", new MenuLinkModel("Submit an Application", _urlHelper.Action("Index", "CredentialApplication"), MenuLinkModel.LinkIconType.Image, "ApplyForCertification.png", index: index++) },
                { "My Invoices", new MenuLinkModel("My Invoices", _urlHelper.Action("Index", "UnraisedInvoices"), MenuLinkModel.LinkIconType.Image, "my-bills.png", index: index++) },
                { "My Digital ID Card", new MenuLinkModel("My Digital ID Card", _urlHelper.Action("DigitalIDCard", "Credential"), MenuLinkModel.LinkIconType.Image, "my-credential-id.png", index: index++, id: "CredentialCards") },
                //{ "Find a Translator Or Interpreter", new MenuLinkModel("Find a Translator Or Interpreter", _urlHelper.Action("OnlineDirectory", "Home"), MenuLinkModel.LinkIconType.Image, "find-a-translator-or-interpreter.png", index: index++) }
			};

            if(!IsRolePlayerEnabled)
            {
                (_menuLinks["Examiner Tools"]).Text = "Examiner Tools";
            }
		}


        public IEnumerable<MenuLinkModel> Menus(IPrincipal user)
        {
            var links = new List<MenuLinkModel>();

            var hasMyTests = user.Identity.IsAuthenticated;// && GetHasMyTests(user.NaatiNumber());
            var hasMyTestResults = user.Identity.IsAuthenticated;// && GetHasMyTestResults(user.NaatiNumber());
            var hasCredentials = user.Identity.IsAuthenticated;// && GetHasCredentials(user.NaatiNumber());
            var hasApplyRole = user.Identity.IsAuthenticated; // && GetHasApplyRole(user);
            var isDisplayBills = user.Identity.IsAuthenticated && _credentialApplicationService.IsDisplayBills(user.NaatiNumber());
            var isDisplayInvoices = user.Identity.IsAuthenticated && _accountingService.GetUnraisedInvoices(user.NaatiNumber()).Data.Any();
            var isPractitioner = user.Identity.IsAuthenticated && GetIsPractitioner(user.NaatiNumber());

            if (user.IsAdministrator())
            {
                links.AddRange(new[]
                {
                    _menuLinks["Manage Users"],
                    _menuLinks["Manage Requests"],
                    _menuLinks["Portal Statistics"],
                    _menuLinks["Export Details"],
                    _menuLinks["Manage Form Templates"],
                    _menuLinks["System Settings"],
                });
            }
            else
            {
                if ((user.IsInRole(SystemRoles.EXAMINER) && user.IsInRole(SystemRoles.AUTHENTICATED_EXAMINER)) || (user.IsRolePlayer() && IsRolePlayerEnabled))
                {
                    links.Add(_menuLinks["Examiner Tools"]);
                }

                if (user.IsPractitioner() || user.IsFormerPractitioner() || user.IsFuturePractitioner())
                {
                    links.AddRange(new[]
                    {
                        _menuLinks["My Logbook"],
                        //_menuLinks["Find a Translator Or Interpreter"]
                    });
				}

				if (hasMyTests)
				{
					links.Add(_menuLinks["Manage My Tests"]);
				}

				if (hasMyTestResults)
				{
					links.Add(_menuLinks["My Test Results"]);
				}

				if (hasCredentials)
                {
                   links.Add(_menuLinks["My Credentials"]);    
                }

                if (isPractitioner)
                {
                    links.Add(_menuLinks["My Digital ID Mobile Card"]);
                    links.Add(_menuLinks["My Digital ID Card"]);
                }

                if (isDisplayBills)
                {
                    links.Add(_menuLinks["My Bills"]);
                }

                if (isDisplayInvoices)
                {
                    links.Add(_menuLinks["My Invoices"]);
                }

                links.Add(_menuLinks["My Account"]);

                if (hasApplyRole)
                {
                    links.Add(_menuLinks["Apply for Certification"]);
                }
            }

            return links.OrderBy(l => l.Index);
        }

        private bool GetHasApplyRole(IPrincipal user)
        {
            var lookupList = new List<LookupContract>();

            var response = _credentialApplicationService.GetPrivateApplicationForms();

            if (user.IsLoggedIn() && response.Results.Any())
            {
                return true;
            }
          
            lookupList.AddRange(response.Results);
            response = _credentialApplicationService.GetPractionerApplicationForms();
            lookupList.AddRange(response.Results);

            if (user.IsPractitioner() && lookupList.Any())
            {
                return true;
            }
         
            response = _credentialApplicationService.GetRecertificationApplicationForms();
            lookupList.AddRange(response.Results);

            if (user.IsRecertification() && lookupList.Any())
            {
                return true;
            }

            return false;
        }

        private bool GetHasCredentials(int naatiNumber)
        {
            var hasCredentials = _credentialApplicationService.HasCredentialsByNaatiNumber(naatiNumber);
            return hasCredentials;
		}

        private bool GetIsPractitioner(int naatiNumber)
        {
            var isPractitionerResponse = _credentialApplicationService.GetIsPractitionerFromNaatiNumber(naatiNumber);

            if (!isPractitionerResponse.Success)
            {
                throw new Exception(string.Join(". ", isPractitionerResponse.Errors.ToArray()));
            }

            return isPractitionerResponse.Data;
        }


		private bool GetHasMyTests(int naatiNumber)
		{
			var hasMyTests = _credentialApplicationService.HasAvailableTests(naatiNumber);
			return hasMyTests;
		}

		private bool GetHasMyTestResults(int naatiNumber)
		{
			var testsCount = _credentialApplicationService.GetTestResults(naatiNumber);
			return testsCount.Results.Any();
		}
	}
}