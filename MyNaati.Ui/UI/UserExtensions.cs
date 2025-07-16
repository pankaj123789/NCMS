using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using F1Solutions.Naati.Common;
using F1Solutions.Naati.Common.Bl;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.Panel;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.Security;
using GetPanelsRequest = MyNaati.Contracts.BackOffice.Panel.GetPanelsRequest;
using GetRolePlaySessionRequest = MyNaati.Contracts.BackOffice.GetRolePlaySessionRequest;
using IExaminerToolsService = MyNaati.Contracts.BackOffice.IExaminerToolsService;

namespace MyNaati.Ui.UI
{
    public static class UserExtensions
    {
        private static readonly SessionStorage<string, bool> NonCandidateLookup = new SessionStorage<string, bool>();
        public static bool IsNonCandidate(this IPrincipal principal)
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Current.User?.Identity?.Name))
            {
                return false;
            }

            var userName = HttpContext.Current.User.Identity.Name;

            if (userName == "administrator")
            {
                return false;
            }

            if (NonCandidateLookup.ContainsKey(userName))
            {
                return NonCandidateLookup.Get(userName);
            }

            var personalDetailsService = ServiceLocator.Resolve<IPersonalDetailsService>();
           
            var request = new PersonNaatiNumberRequest()
            {
                NaatiNumber = principal.NaatiNumber()
            };

            var result = personalDetailsService.GetPerson(request);
            var isNonCandidate = result?.Person?.EntityTypeId == Constants.ENTITY_TYPE_PERSON_NON_CANDIDATE;

            NonCandidateLookup.Put(isNonCandidate, userName);

            return isNonCandidate;
        }

        public static bool IsAdministrator(this IPrincipal principal)
        {
            return principal.IsInRole(SystemRoles.ADMINISTRATORS);
        }

        public static bool IsPractitioner(this IPrincipal principal)
        {
            return principal.IsInRole(SystemRoles.PRACTITIONER);
        }

        public static bool IsLoggedIn(this IPrincipal principal)
        {
            return principal.Identity?.IsAuthenticated == true;
        }

        public static bool IsRecertification(this IPrincipal principal)
        {
            if (principal.Identity?.IsAuthenticated == true)
            {
                var mLogbookService = ServiceLocator.Resolve<ILogbookService>();
                var credentials = mLogbookService.GetCredentials(principal.NaatiNumber()).List;
                foreach (var credentialDto in credentials)
                {
                    var response = mLogbookService.GetCredentialRecertificationStatus(credentialDto.Id);
                    if (response.StatusId == (int) RecertificationStatus.EligibleForNew)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsRolePlayer(this IPrincipal principal)
        {
            if (!principal.Identity?.IsAuthenticated == true)
            {
                return false;
            }

            var displayRolePlayerQueryService = ServiceLocator.Resolve<IDisplayRolePlayerCacheQueryService>();
            return displayRolePlayerQueryService.IsRolePlayer(principal.NaatiNumber());
        }

        public static bool HasRolePlaySession(this IPrincipal principal)
		{
			if (!principal.Identity?.IsAuthenticated == true)
			{
				return false;
			}

			var service = ServiceLocator.Resolve<IExaminerToolsService>();
			var response = service.GetRolePlaySession(new GetRolePlaySessionRequest { NaatiNumber = NaatiNumber(principal) });
			return response.Sessions.Any();
		}

		public static bool IsFormerPractitioner(this IPrincipal principal)
        {
            return principal.IsInRole(SystemRoles.FORMERPRACTITIONER);
        }

		public static bool IsFuturePractitioner(this IPrincipal principal)
        {
            return principal.IsInRole(SystemRoles.FUTUREPRACTITIONER);
        }

        public static int NaatiNumber(this IPrincipal principal)
        {
            if (String.IsNullOrWhiteSpace(principal.Identity.Name))
            {
                return default(int);
            }

            var userService = ServiceLocator.Resolve<IUserService>();
            var membershipService = ServiceLocator.Resolve<IMembershipProviderService>();
            var user = userService.GetUser(membershipService.GetUser(principal.Identity.Name, true)?.UserId ?? new Guid());
            return user.NaatiNumber;
        }

        public static string GetEnvironment(this IPrincipal principal)
        {
            return new EnvironmentConfigurationHelper().GetEnvironmentDetails().EnvironmentDisplayName;
        }
        public static bool IsCandidate(this IPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}