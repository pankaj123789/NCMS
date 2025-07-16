using System;
using System.Linq;
using System.Web.Security;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using MyNaati.Contracts.BackOffice.Panel;
using MyNaati.Ui.Security;
using GetPanelsRequest = MyNaati.Contracts.BackOffice.Panel.GetPanelsRequest;

namespace MyNaati.Ui.Helpers
{
    public class ExaminerHelper : IExaminerHelper
    {
        private readonly IPanelMembershipService mPanelService;

        public ExaminerHelper(IPanelMembershipService panelService)
        {
            mPanelService = panelService;
        }

        public bool IsValidated(string userName)
        {
            if (Roles.IsUserInRole(userName, SystemRoles.EXAMINER))
            {
                return Roles.IsUserInRole(userName, SystemRoles.AUTHENTICATED_EXAMINER);
            }

            return true;
        }

        public void LoadExaminerRoles(string userName, int naatiNumber)
        {
            if (Roles.IsUserInRole(userName, SystemRoles.EXAMINER))
            {
                Roles.RemoveUserFromRole(userName, SystemRoles.EXAMINER);
            }

            if (Roles.IsUserInRole(userName, SystemRoles.CHAIR))
            {
                Roles.RemoveUserFromRole(userName, SystemRoles.CHAIR);
            }

            if (Roles.IsUserInRole(userName, SystemRoles.UNAUTHENTICATED_EXAMINER))
            {
                Roles.RemoveUserFromRole(userName, SystemRoles.UNAUTHENTICATED_EXAMINER);
            }

            if (mPanelService.GetPanels(new GetPanelsRequest
            {
                NAATINumber = naatiNumber,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                RoleCategoryIds = new[] { (int)PanelRoleCategoryName.Examiner },
                Chair = true,
            }).Panels.Any())
            {
                Roles.AddUserToRole(userName, SystemRoles.CHAIR);
            }

            if (mPanelService.GetPanels(new GetPanelsRequest
            {
                NAATINumber = naatiNumber,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                RoleCategoryIds = new[] { (int)PanelRoleCategoryName.Examiner },
            }).Panels.Any())
            {
                Roles.AddUserToRole(userName, SystemRoles.EXAMINER);
            }
        }
    }
}