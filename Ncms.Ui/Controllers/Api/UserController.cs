using System;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/user")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;



        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //user-search
        [HttpGet]
        [Route("userSearch")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.User)]
        public HttpResponseMessage UserSearch()
        {
            return this.CreateResponse(() => _userService.UserSearch());
        }

        [HttpGet]
        [Route("userRoles")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.User)]
        public HttpResponseMessage GetUserRoles()
        {
            return this.CreateResponse(() => _userService.GetUserRoles());
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.User)]
        [Route("createOrUpdateUser")]
        public HttpResponseMessage CreateOrUpdateUser(UserDetailsModel model)
        {
           
            return this.CreateResponse(() => {
                var response = _userService.CreateOrUpdateUser(model);
                Startup.RefreshUserCache(model.UserName, null, default(DateTime));
                return response;
            });
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.User)]
        [Route("getUserDetailsById/{id}")]
        public HttpResponseMessage GetUserDetailsById(int id)
        {
            return this.CreateResponse(() => _userService.GetUserDetailsById(id));
        }


        [HttpGet]
        [Route("getExistingRoleByUserId/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.User)]
        public HttpResponseMessage GetExistingRoleByUserId(int id)
        {
            var response = _userService.GetExistingRoleByUserId(id);
            return this.CreateResponse(() => response);
        }
    }
}