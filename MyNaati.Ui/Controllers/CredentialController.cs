using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.Credentials;
using F1Solutions.Naati.Common.Contracts.Bl.Credentials;
using MyNaati.Bl.BackOffice;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.Account;
using MyNaati.Ui.ViewModels.Credential;

namespace MyNaati.Ui.Controllers
{
    [Authorize]
    public class CredentialController : BaseController
    {

        private readonly ICredentialApplicationService mCredentialApplicationService;
        private readonly ICredentialQrCodeService mCredentialQrCodeService;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IEmailCodeVerificationService mEmailCodeVerificationService;

        public CredentialController(ICredentialApplicationService credentialApplicationService, 
            ICredentialQrCodeService credentialQrCodeService, IAutoMapperHelper autoMapperHelper, IEmailCodeVerificationService emailCodeVerificationService)
        {
            mCredentialApplicationService = credentialApplicationService;
            mCredentialQrCodeService = credentialQrCodeService;
            mEmailCodeVerificationService = emailCodeVerificationService;
            _autoMapperHelper = autoMapperHelper;
        }

        // GET: Credentials
        [Route("Credentials")]
        public ActionResult Index()
        {   
            var allCredentials = mCredentialApplicationService.GetAllCredentialsByNaatiNumber(CurrentUserNaatiNumber);

            var allCredentialDetailList = allCredentials.Results.Select(_autoMapperHelper.Mapper.Map<CredentialModel>).ToList();

            var mfaAndAccessCodeModel = GetMfaConfigurationAndEmailCodeActiveStatus(CurrentUserNaatiNumber);

            var credentialDetailRequestModel = new CredentialDetailRequestModel
            {
                Credentials = allCredentialDetailList,
                MfaConfigured = mfaAndAccessCodeModel.MfaActive
            };

            return PartialView(credentialDetailRequestModel);
        }

        [HttpPost]
        public ActionResult UpdateCredential(int credentialId, bool showInOnlineDirectory = false)
        {
            if (!mCredentialApplicationService.IsValidCredentialByNaatiNumber(credentialId, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }


            var credential = new CredentialContract
            {
                Id = credentialId,
                ShowInOnlineDirectory = showInOnlineDirectory
            };

            bool success;

            try
            {
                mCredentialApplicationService.UpdateCredential(credential);
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            var responseModel = new
            {
                Results = success
            };

            return Json(responseModel);
        }


        [HttpGet]
        public ActionResult GetAttachmentByCredentialId(int credentialId)
        {
            var response = mCredentialApplicationService.GetCredentialAttachmentsById(credentialId, CurrentUserNaatiNumber);

            if (response.Results.Any())
            {
                return Json(response.Results, JsonRequestBehavior.AllowGet);
            }

            return Json("{[]}", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCredentialQrCodeStamp(int credentialId)
        {
            var mfaConfigurationAndEmailCodeActiveStatus = GetMfaConfigurationAndEmailCodeActiveStatus(CurrentUserNaatiNumber);

            if (!mfaConfigurationAndEmailCodeActiveStatus.MfaActive)
            {
                return View("_MfaAndAccessCodePartial", new MfaAndAccessCodeModel()
                {
                    MfaConfigured = mfaConfigurationAndEmailCodeActiveStatus.MfaConfigured,
                    MfaActive = mfaConfigurationAndEmailCodeActiveStatus.MfaActive,
                    ReturnController = "Credential",
                    ReturnView = "Index",
                    Email = mfaConfigurationAndEmailCodeActiveStatus.Email
                });
            }

            var isValidResult = mCredentialQrCodeService.DoesCredentialBelongToUser(credentialId, CurrentUserNaatiNumber);
            if (!isValidResult.Success)
            {
                return HttpNotFound($"Could not get Credential QR Stamp for Credential.");
            }
            var qrCodeStampResponse = mCredentialQrCodeService.GetCredentialQrStamp(credentialId);
            if (!qrCodeStampResponse.Success)
            {
                return HttpNotFound($"Could not get Credential QR Stamp for Credential.");

            }

                var filePathToCredentialQrStamp = qrCodeStampResponse.Data;
                return File(OpenToMemoryAndDispose(filePathToCredentialQrStamp), MimeMapping.GetMimeMapping(Path.GetFileName(filePathToCredentialQrStamp)), Path.GetFileName(filePathToCredentialQrStamp));
        }

        public ActionResult DownloadAttachmentByCredentialAttachmentId(int storedFileId)
        {
            var validateResult = mCredentialApplicationService.DoesCredentialBelongToUser(storedFileId, CurrentUserNaatiNumber);

            if(!validateResult.Success)
            {
                return HttpNotFound($"Could not get Attachment for Credential.");
            }

            var getRequest = new GetCredentialAttachmentFileRequest
            {
                StoredFileId = storedFileId,
                TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"],
                NaatiNumber = CurrentUserNaatiNumber
            };

            var file = mCredentialApplicationService.GetCredentialAttachmentFileByCredentialAttachmentId(getRequest);
            Response.BufferOutput = false;

            return File(OpenToMemoryAndDispose(file.FilePaths[0]), MimeMapping.GetMimeMapping(file.FileName), file.FileName);
        }

        public ActionResult DigitalIDCard()
        {
            var credentialIdCardsResponse = mCredentialQrCodeService.GetCredentialIdCards(CurrentUserNaatiNumber);

            var mfaAndAccessCodeModel = GetMfaConfigurationAndEmailCodeActiveStatus(CurrentUserNaatiNumber);

            var credentialIdCardModel = new CredentialIdCardModel()
            {
                CredentialIdCards = credentialIdCardsResponse.Data,
                MfaAndAccessCodeModel = new ViewModels.Account.MfaAndAccessCodeModel()
                {
                    MfaActive = mfaAndAccessCodeModel.MfaActive,
                    MfaConfigured = mfaAndAccessCodeModel.MfaConfigured,
                    ReturnView = "DigitalIdCard",
                    ReturnController = "Credential",
                    Email = mfaAndAccessCodeModel.Email
                }
            };

            return View(credentialIdCardModel);
        }

        private static MemoryStream OpenToMemoryAndDispose(string path)
        {
            var fileData = new MemoryStream(System.IO.File.ReadAllBytes(path), false)
            {
                Position = 0
            };

            System.IO.File.Delete(path);

            return fileData;
        }

        //private MfaAndAccessCodeModel GetMfaConfigurationAndEmailCodeActiveStatus(int naatiNumber)
        //{
        //    var response = new MfaAndAccessCodeModel();

        //    //deal with MFA first. We also need to know if it has been set up anbd just expired for the UI

        //    var mfaConfiguredAndActiveResponse = mCredentialQrCodeService.GetMFAConfiguredAndValid(naatiNumber);

        //    if (!mfaConfiguredAndActiveResponse.Success)
        //    {
        //        var errors = new StringBuilder();

        //        foreach (var error in mfaConfiguredAndActiveResponse.Errors)
        //        {
        //            errors.Append(error);
        //        }

        //        LoggingHelper.LogError($"Error in GetMfaConfiguredAndActiveOrEmailCodeActive: {errors}");
        //        return null;
        //    }

        //    var mfaConfiguredAndActive = mfaConfiguredAndActiveResponse.Data;

        //    response.MfaConfigured = mfaConfiguredAndActiveResponse.Data.MfaConfigured;
        //    response.MfaConfiguredAndActiveOrEmailCodeActive = mfaConfiguredAndActiveResponse.Data.MfaActive;

        //    if (response.MfaConfiguredAndActiveOrEmailCodeActive)
        //    {
        //        //that's all we need. Can return
        //        return response; 
        //    }

        //    //otherwise look at the access code

        //    var accessCodeResponse = mEmailCodeVerificationService.IsEmailVerificationCodeCurrent(CurrentUserNaatiNumber);

        //    if(!accessCodeResponse.Success)
        //    {
        //        LoggingHelper.LogError($"Error in GetMfaConfiguredAndActiveOrEmailCodeActive: {accessCodeResponse.Messages.First()}");
        //        response.MfaConfigured = true;
        //    }

        //    var accessCodeCurrent = accessCodeResponse.Data;

        //    response.MfaConfiguredAndActiveOrEmailCodeActive = accessCodeCurrent;

        //    return response;
        //}
    }
}
