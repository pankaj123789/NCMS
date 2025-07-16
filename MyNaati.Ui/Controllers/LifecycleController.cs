using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Ui.Attributes;

namespace MyNaati.Ui.Controllers
{
    [DisableRemoteCalls]
    public class LifecycleController : BaseController
    {
        private readonly ILifecycleService _lifecycleService;

        public LifecycleController(ILifecycleService lifecycleService)
        {
            _lifecycleService = lifecycleService;
        }

        [HttpGet]
        public ActionResult Stop()
        {
            _lifecycleService.Stop();
            return new EmptyResult();
        }
    }
}