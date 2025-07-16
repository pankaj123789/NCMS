using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Ui.Extensions;
using Ncms.Ui.Models;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/security")]
    public class SecurityController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var user = CurrentPrincipal?.User;
            return user == null
                ? this.FailureResponse("Sorry, you are not authorised to access NCMS.")
                : this.CreateResponse(() => new { user.Id, user.Name, user.DomainName, user.OfficeId, CurrentPrincipal.Permissions });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("verbs")]
        public HttpResponseMessage GetVerbs()
        {
            return this.CreateResponse(() => GetVerbModels());
        }

        [AllowAnonymous]
        private IList<SecurityVerbModel> GetVerbModels()
        {
            var names = Enum.GetNames(typeof(SecurityVerbName));
            var values = Enum.GetValues(typeof(SecurityVerbName));
            return values.Cast<object>()
                .Select((t, i) => new SecurityVerbModel
                                  {
                                      Id = i + 1,
                                      Name = names[i],
                                      DisplayName = ((SecurityVerbName)values.GetValue(i)).GetDescription(),
                                      Value = (long)values.GetValue(i)
                                  })
                .ToList();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ShowAllMenuItems")]
        public HttpResponseMessage ShowAllMenuItems()
        {
            return this.CreateResponse(() => Convert.ToBoolean(ConfigurationManager.AppSettings["ShowAllMenuItems"]));
        }
    }
}