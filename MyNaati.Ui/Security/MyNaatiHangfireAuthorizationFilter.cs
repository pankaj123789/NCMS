using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Security;
using Hangfire.Dashboard;

namespace MyNaati.Ui.Security
{
    public class MyNaatiHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            IPrincipal user = HttpContext.Current?.User;
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                var userName = user.Identity.Name;
                var userPermissions = ServiceLocator.Resolve<INcmsUserPermissionQueryService>().GetUserPermissionsByUserName(userName) ?? new List<UserPermissionsDto>();

                var systemPermission = userPermissions.FirstOrDefault(x => x.Noun == SecurityNounName.Dashboard);
                return (systemPermission?.VerbMask & (long)SecurityVerbName.Manage) == (long)SecurityVerbName.Manage;
            }

            return false;
        }
    }
}