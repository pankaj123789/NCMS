using System.Web.Mvc;

namespace MyNaati.Ui.Controllers
{
    public class ErrorsController : BaseController
    {
        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult ReasonError(string error)
        {
            return View(error);
        }
    }
}