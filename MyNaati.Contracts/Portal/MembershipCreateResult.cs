using System.Web.Security;

namespace MyNaati.Contracts.Portal
{
    
    public class MembershipCreateResult
    {
        
        public MembershipCreateStatus CreateStatus;

        
        public ePortalUser User;

         
        public bool EmailSuccess;

         
        public string NewPassword;

        public MembershipCreateResult()
        {
        }
        public MembershipCreateResult(ePortalUser user, MembershipCreateStatus createStatus, string newPassword, bool emailSuccess)
        {
            User = user;
            CreateStatus = createStatus;
            NewPassword = newPassword;
            EmailSuccess = emailSuccess;
        }
    }
}
