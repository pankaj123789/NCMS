//using F1Solutions.NAATI.ePortal.ServiceContracts;
//using F1Solutions.NAATI.ePortal.Web.Security;
//using F1Solutions.NAATI.ePortal.Web.ViewModels.Configuration;
//using System.Web.Mvc;

//namespace F1Solutions.NAATI.ePortal.Web.Controllers
//{
//    public class ConfigurationController : Controller
//    {
//        private IConfigurationService mConfigurationService;
//        private const int DEFAULT_DAYS_DELAY_ACCREDITATION = 5;

//        public ConfigurationController(IConfigurationService configurationService)
//        {
//            mConfigurationService = configurationService;
//        }

//        [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
//        public ActionResult SystemSettings()
//        {
//            var model = new ConfigurationModel(mConfigurationService);
//            return View(model);
//        }

//        [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
//        public ActionResult ApplySettings(ConfigurationModel model, string submitButton)
//        {
//            switch (submitButton)
//            {
//                case "RestoreDefaults":
//                    ModelState.Clear();
//                    model.ShowPhoto = true;
//                    model.ShowVerifyCredentials = true;
//                    model.PaymentRequiredForPDListing = false;
//                    model.DaysToDelayAccreditation = DEFAULT_DAYS_DELAY_ACCREDITATION;
//                    mConfigurationService.UpdateShowVerifyCredentials(model.ShowPhoto);
//                    mConfigurationService.UpdateShowPhoto(model.ShowVerifyCredentials);
//                    mConfigurationService.UpdateDaysToDelayAccreditation(model.DaysToDelayAccreditation);
//                    mConfigurationService.UpdatePaymentRequiredForPDListing(model.PaymentRequiredForPDListing);
//                    break;

//                case "ApplySettings":
//                    if (ModelState.IsValid)
//                    {
//                        mConfigurationService.UpdateShowVerifyCredentials(model.ShowVerifyCredentials);
//                        mConfigurationService.UpdateShowPhoto(model.ShowPhoto);
//                        mConfigurationService.UpdateDaysToDelayAccreditation(model.DaysToDelayAccreditation);
//                        mConfigurationService.UpdatePaymentRequiredForPDListing(model.PaymentRequiredForPDListing);
//                        model.ChangesSaved = true;
//                    }
//                    break;
//            }
//            return View("SystemSettings", model);
//        }
//    }
//}
