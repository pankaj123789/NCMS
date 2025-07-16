using System;
using System.Web.Http;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Contracts.Bl;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [AllowAnonymous]
    [DisableRemoteCalls]
    public class LifecycleController : ApiController
    {
        private readonly ILifecycleService _lifecycleService;

        public LifecycleController(ILifecycleService lifecycleService)
        {
            _lifecycleService = lifecycleService;
        }
        [HttpGet]
        public void Stop()
        {
            _lifecycleService.Stop();
        }
    }
}
