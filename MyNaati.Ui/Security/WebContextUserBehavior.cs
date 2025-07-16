using System;
using System.Web;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
using MyNaati.Bl.Portal.Security;

namespace MyNaati.Ui.Security
{
    /// <summary>
    /// Returns an instance of CurrentUserInspector that can
    /// retrieve the current WebUser from HttpContext.
    /// </summary>
    //public class WebContextUserBehavior : ContextUserBehavior
    //{
    //    protected override ContextUserMessageInspector NewInspector
    //    {
    //        get
    //        {
    //            return new ContextUserMessageInspector(() =>
    //            {
    //                var currentUser = HttpContext.Current.User;
    //                if (currentUser == null)
    //                    return null;

    //                if (string.IsNullOrEmpty(currentUser.Identity.Name))
    //                    return null;

    //                //  Allow for admin user, not just Customer number user names.


    //                return new NaatiWebUser() { Email = currentUser.Identity.Name };
    //            });
    //        }
    //    }

    //    public override Type BehaviorType
    //    {
    //        get
    //        {
    //            return typeof(WebContextUserBehavior);
    //        }
    //    }

    //    protected override object CreateBehavior()
    //    {
    //        return new WebContextUserBehavior();
    //    }
    //}
}
