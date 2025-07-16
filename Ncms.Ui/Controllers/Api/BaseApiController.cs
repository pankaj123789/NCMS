using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Ncms.Bl.Security;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [NcmsAuthenticationCheck]
    public class BaseApiController : ApiController
    {
        public BaseApiController()
        {
            SetCurrentPrincipal();
        }

        protected NcmsPrincipal CurrentPrincipal { get; private set; }

        protected UserModel CurrentUser => CurrentPrincipal?.User;

        private void SetCurrentPrincipal()
        {
            var user = System.Threading.Thread.CurrentPrincipal;
            if (user.Identity.IsAuthenticated && user.Identity is FormsIdentity)
            {
                var principle = new NcmsPrincipal(user.Identity);
                HttpContext.Current.User = principle;
                System.Threading.Thread.CurrentPrincipal = principle;
                CurrentPrincipal = principle;
            }
        }

        /// <summary>
        /// Helper function to create dummy list, used during frontend development
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected List<dynamic> CreateDummyList(Func<int, dynamic> detail, int count)
        {
            var list = new List<dynamic>();

            for (int i = 1; i <= count; i++)
            {
                list.Add(detail(i));
            }

            return list;
        }
    }
}
