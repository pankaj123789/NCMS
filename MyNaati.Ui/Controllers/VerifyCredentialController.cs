using F1Solutions.Naati.Common.Contracts.Bl.Credentials;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.VerifyPractitioner;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.ViewModels.VerifyCredential;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MyNaati.Ui.Controllers
{
    public class VerifyCredentialController : Controller
    {
        private readonly ICredentialQrCodeService mCredentialQrCodeService;
        private readonly ICredentialApplicationService mCredentialApplicationService;
        private readonly IApiPublicService mApiPublicService;

        public VerifyCredentialController(ICredentialQrCodeService credentialQrCodeService, ICredentialApplicationService credentialApplicationService, IApiPublicService apiPublicService)
        {
            mCredentialQrCodeService = credentialQrCodeService;
            mCredentialApplicationService = credentialApplicationService;
            mApiPublicService = apiPublicService;
        }

        [HttpGet]
        public ActionResult Index(Guid? QrCode)
        {
            if(!QrCode.HasValue)
            {
                LoggingHelper.LogError($"Unable to find the matching practitioner. QrCode is null");
                return View(new VerifyCredentialModel() { Message = "Unable to find the matching practitioner. Please contact the practitioner or info@naati.com.au for more information" });
            }

            //need practitionerid and also QrCodes as they are not in the External Api call
            var serviceModelResult = mCredentialQrCodeService.GetVerifyPractitionerModelFromQrCode(QrCode.Value);

            if (!serviceModelResult.Success)
            {
                LoggingHelper.LogError($"Unable to find the matching practitioner. {QrCode.Value}");
                return View(new VerifyCredentialModel() { Message = "Unable to find the matching practitioner. Please contact the practitioner or info@naati.com.au for more information" });
            }

            var serviceModel = serviceModelResult.Data;

            if (!serviceModel.AlowVerifyOnline)
            {
                LoggingHelper.LogError($"Unable to return the matching practitioner (AlowVerifyOnline). {QrCode.Value}");
                return View(new VerifyCredentialModel() { Message = "NAATI is unable to display this practitioner’s credentials.Please contact NAATI if you require verification." });
            }

            var apiResult = mApiPublicService.GetCertifications(new GetCertificationsRequest()
            {
                PractitionerId = serviceModel.PractitionerId
            });

            var passedInQrCode = serviceModel.QrCodes.FirstOrDefault(x => x.QrCode == QrCode);

            var message = string.Empty;

            var getPhotoResult = GetPractitionerPhoto(apiResult.Practitioner.PractitionerId);
            if (!getPhotoResult.Success)
            {
                LoggingHelper.LogError($"Unable to return the practitioner photo {getPhotoResult.Messages.First()}. {QrCode.Value}");

                return View(new VerifyCredentialModel() { Message = getPhotoResult.Messages.First() });
            }

            var model = new VerifyCredentialModel()
            {
                Name = $"{apiResult.Practitioner.GivenName} {apiResult.Practitioner.FamilyName}",
                CertificationDetails = GetCertificationDetails(apiResult.CurrentCertifications),
                Location = apiResult.Practitioner.Country,
                PractitionerId = apiResult.Practitioner.PractitionerId,
                GeneratedOn = serviceModel.GeneratedOn,
                IsDeceased = serviceModel.IsDeceased,
                Message = VerifyQrCode(QrCode.Value, apiResult.CurrentCertifications, serviceModel),
                PractitionerImage = getPhotoResult.Data
            };

            //VerfiyQRCode ensures the passed in QrCode is valid. If it returns null, then it is. 
            model.DigitalStampValid = model.Message == null?"VALID":"INVALID";

            LoggingHelper.LogInfo($"Verify Practitioner details shown successfully {QrCode.Value}");

            return View(model);
        }

        private GenericResponse<string> GetPractitionerPhoto(string practitionerId)
        {
            var getPhotoResponse = mApiPublicService.GetPersonPhoto(new ApiPublicPersonPhotoRequest
            {
                PropertyType = 3, //ApiPublicPhotoRequestPropertyType.PractitionerId
                Value = practitionerId
            });
            switch (getPhotoResponse.Message)
            {
                case null:
                    break;
                case "Referenced person does not exist in the system.":
                case "Referenced person photo does not exist in the system.":
                    return GetDefaultPhoto();
                default:
                    break;
            }
            // if the practitioner is deceased, get default person image
            if (getPhotoResponse.IsDeceased)
            {
                return GetDefaultPhoto();
            }
            if (!getPhotoResponse.ShowPhotoOnline)
            {
                return GetDefaultPhoto();
            }

            var response = mCredentialApplicationService.GetPersonImage(new GetImageRequestContract { PractitionerId = practitionerId, TempFolderPath = ConfigurationManager.AppSettings["tempFilePath"] });

            if (System.IO.File.Exists(response.FilePath))
            {
                var photoBytes = System.IO.File.ReadAllBytes(response.FilePath);

                Bitmap photoBitmap;

                using (var ms = new MemoryStream(photoBytes))
                {
                    photoBitmap = new Bitmap(ms);
                }

                var photoResized = mCredentialQrCodeService.ResizeImage(photoBitmap, 200, 200);

                var photoUrlTemplate = "data:image/png;base64, {0}";

                string photoUrl;

                using (var ms = new MemoryStream())
                {
                    photoResized.Save(ms, ImageFormat.Png);
                    var photoResizedBytes = ms.ToArray();

                    var photoBase64 = Convert.ToBase64String(photoResizedBytes);

                    photoUrl = string.Format(photoUrlTemplate, photoBase64);
                }

                return photoUrl;
            }

            return null;
        }

        private string GetDefaultPhoto()
        {
            var defaultPhotoBytes = mCredentialQrCodeService.GetDefaultImage();

            Bitmap defaultPhotoBitmap;

            using (var ms = new MemoryStream(defaultPhotoBytes))
            {
                defaultPhotoBitmap = new Bitmap(ms);
            }

            var defaultPhotoResized = mCredentialQrCodeService.ResizeImage(defaultPhotoBitmap, 200, 300);

            var defaultPhotoUrlTemplate = "data:image/png;base64, {0}";

            string defaultPhotoUrl;

            using (var ms = new MemoryStream())
            {
                defaultPhotoResized.Save(ms, ImageFormat.Png);
                var defaultPhotoResizedBytes = ms.ToArray();

                var defaultPhotoBase64 = Convert.ToBase64String(defaultPhotoResizedBytes);

                defaultPhotoUrl = string.Format(defaultPhotoUrlTemplate, defaultPhotoBase64);
            }

            return defaultPhotoUrl;
        }

        private string VerifyQrCode(Guid qrCode, IEnumerable<ApiPublicCredentials> currentCertifications, F1Solutions.Naati.Common.Contracts.Bl.VerifyPractitioner.VerifyPractitionerServiceModel serviceModel)
        {
            var qrCodeRow = serviceModel.QrCodes.FirstOrDefault(x => x.QrCode == qrCode);

            //if the code was not found
            if (qrCodeRow == null)
            {
                return "Unable to find the matching practitioner. Please contact the practitioner or info@naati.com.au for more information";
            }

            //if passed in code is inactive and there are active credentials
            if (qrCodeRow.InactiveDate.HasValue)
            {
                if (currentCertifications.Any())
                {
                    return $"The practitioner has active credential(s), but this QR code was deactivated on {qrCodeRow.InactiveDate.Value.ToShortDateString()} and is not valid. Please contact the practitioner or info@naati.com.au for more information";
                }
                return $"The person does not have an active credential and this QR code was deactivated on {qrCodeRow.InactiveDate.Value.ToShortDateString()}. The QR Code maybe valid if the practitioner was certified when the translation was done.";
            }
            return null;
        }

        private IList<CertificationDetail> GetCertificationDetails(IEnumerable<ApiPublicCredentials> credentialTypes)
        {
            var result = new List<CertificationDetail>();
            foreach (var detail in credentialTypes)
            {
                result.Add(new CertificationDetail()
                {
                    Certification = detail.CertificationType,
                    Skill = detail.Skill,
                    ValidFrom = DateTime.Parse(detail.StartDate),
                    ValidTo = DateTime.Parse(detail.EndDate)
                });
            }
            return result;
        }
    }
}