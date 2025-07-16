using System.Web.Mvc;

namespace Ncms.Ui.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult InternalServerError()
        {
            return View();
        }
    }
}