using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl.Credentials;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.VerifyPractitioner;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using MyNaati.Contracts.BackOffice;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MyNaati.Contracts.Portal;
using Resources = Naati.Resources;

namespace F1Solutions.Naati.Common.Bl.Credentials
{
    public class CredentialQrCodeService : ICredentialQrCodeService
    {
        private readonly ICredentialQrCodeDalService _credentialQrCodeDalService;
        private readonly IPersonQueryService _personQueryService;
        private readonly ICredentialApplicationService _credentialApplicationService;
        private readonly ILookupProvider _lookupProvider;

        public CredentialQrCodeService(ICredentialApplicationService credentialApplicationService, ICredentialQrCodeDalService credentialQrCodeDalService,IPersonQueryService personQueryService,
            ILookupProvider lookupProvider)
        {
            _credentialQrCodeDalService = credentialQrCodeDalService;
            _personQueryService = personQueryService;
            _credentialApplicationService = credentialApplicationService;
            _lookupProvider = lookupProvider;
        }

        public GenericResponse<string> GetCredentialQrStamp(int credentialId)
        {
            // get qr code from database or generate new one if it does not exist
            var credentialQrCodeResponse = GetCredentialQRCodeForStampResponse(credentialId);

            if (credentialQrCodeResponse.Success)
            {
                var credentialQrCode = credentialQrCodeResponse.Data;

                // generate qr code
                var qrCodeImage = GenerateCredentialQrCode(credentialQrCode.QrCodeGuid);

                //size the qr code to appropriate size
                Bitmap smallerQrImage = new Bitmap(qrCodeImage, 380, 380);

                // create image canvas to draw on
                Bitmap finalStamp = new Bitmap(920, 400);
                Graphics canvas = Graphics.FromImage(finalStamp);

                // Fill left side of screen with opaque white
                canvas.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), new Rectangle(0, 0, 400, 400));

                // Fill only QR area with solid white
                canvas.FillRectangle(new SolidBrush(Color.White), new Rectangle(37, 27, 326, 326));

                // draw qr code onto stamp
                canvas.DrawImage(smallerQrImage, 10, 0);
                // naati set text
                canvas.DrawString($"Scan QR code to verify credentials & stamp", new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel),
                    new SolidBrush(Color.Black), new Rectangle(20, 355, 360, 13), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                // credential qr url
                var qrCodeAccessUrl = ConfigurationManager.AppSettings["QrCodeAccessUrl"];

                //prepare other side of stamp
                canvas.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), new Rectangle(400, 0, 520, 400));
                canvas.DrawRectangle(new Pen(Color.Black, 3), new Rectangle(410, 5, 503, 345));
                canvas.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(420, 15, 484, 325));

                // draw naati logo onto right side of stamp
                Bitmap naatiLogo = Resources.CredentialQr.NaatiLogoBWForQR;
                // resize naati logo and draw
                Bitmap naatiLogoSmaller = new Bitmap(naatiLogo, 150, 150);
                canvas.DrawImage(naatiLogoSmaller, 430, 25);

                // buffer to deal with formatting practitioner name if it is really long
                var practitionerName = $"{credentialQrCode.Title}{(credentialQrCode.Title.IsNullOrEmpty()?string.Empty:" ")}{credentialQrCode.FirstName} {credentialQrCode.LastName} ";
                // buffer for if names are too long for 1 line
                var longNameBuffer = 0;
                var practitionerNameWidth = TextRenderer.MeasureText(practitionerName, new Font("Arial", 25, GraphicsUnit.Pixel)).Width;

                if (practitionerNameWidth > 295)
                {
                    var bufferAmount = practitionerNameWidth / 295;

                    for (int i = 0; i < bufferAmount; i++)
                    {
                        longNameBuffer += 26;
                    }

                }
                if (longNameBuffer > 52)
                {
                    // error out or generate qr code with awkward formatting?
                    LoggingHelper.LogWarning($"Practitioner Name '{practitionerName}' is too long. Generated stamp may have anomalies.");
                    longNameBuffer = 52;
                }

                // Practitioner Name
                canvas.DrawString(practitionerName, new Font("Arial", 25, GraphicsUnit.Pixel),
                    new SolidBrush(Color.Black), new Rectangle(590, 60 - (longNameBuffer / 2), 295, 35 + longNameBuffer), new StringFormat() { LineAlignment = StringAlignment.Far });
                // Practitioner Number
                canvas.DrawString(credentialQrCode.PractitionerNumber, new Font("Arial", 25, GraphicsUnit.Pixel), new SolidBrush(Color.Black), new Rectangle(590, 90 + (longNameBuffer / 2), 295, 30));
                // Skill
                var skill = credentialQrCode.SkillDto.DisplayName.Replace("into", ">");

                canvas.DrawString(skill, new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Black), new Rectangle(590, 120 + (longNameBuffer / 2), 295, 30));

                canvas.DrawLine(new Pen(Color.Black, 2), 420, 185, 885, 185);
                // NAATI set text
                canvas.DrawString("Digitally Authenticated by NAATI", new Font("Arial", 18, GraphicsUnit.Pixel), new SolidBrush(Color.Black), new Rectangle(430, 180, 454, 55), new StringFormat() { LineAlignment = StringAlignment.Center });
                canvas.DrawLine(new Pen(Color.Black, 2), 420, 225, 885, 225);
                // Generated Date
                canvas.DrawString($"Stamp generated on {credentialQrCode.IssueDate:dd/MM/yyyy}", new Font("Arial", 18, GraphicsUnit.Pixel), new SolidBrush(Color.Black), new Rectangle(430, 220, 454, 55), new StringFormat() { LineAlignment = StringAlignment.Center });
                canvas.DrawLine(new Pen(Color.Black, 2), 420, 265, 885, 265);
                // Credential
                canvas.DrawString(credentialQrCode.CredentialTypeName, new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Black),
                                  new Rectangle(430, 270, 454, 70), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                //now the URL
                canvas.DrawString(string.Format(qrCodeAccessUrl, credentialQrCode.QrCodeGuid), new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel),
                    new SolidBrush(Color.Black), new Rectangle(5, 360, 900, 40), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });


                var tempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"];

                var stampFilePath = $"{tempFileStorePath}\\{credentialQrCode.PractitionerNumber} {credentialQrCode.CredentialTypeName} {credentialQrCode.SkillDto.DisplayName} {credentialQrCode.IssueDate:yyyy-MM-dd}.png";
                if (File.Exists(stampFilePath))
                {
                    File.Delete(stampFilePath);
                }

                finalStamp.Save(stampFilePath, ImageFormat.Png);

                return stampFilePath;

            }

            return new GenericResponse<string>(null) { Success = false, Errors = credentialQrCodeResponse.Errors };
        }

        public GenericResponse<List<string>> GetCredentialIdCards(int naatiNumber)
        {
            // get all actice credentials that are certifications
            var allActiveCredentials = _credentialApplicationService.GetAllCredentialsByNaatiNumber(naatiNumber).Results.Where(x => x.Certification
            && x.Status != "Expired" && x.Status != "Terminated");

            var personImageResponse = _personQueryService.GetPersonImage(new GetPersonDetailsRequest() { NaatiNumber = naatiNumber });
            var personImageBytes = personImageResponse.PersonImageData;

            if(personImageBytes== null)
            {
                personImageBytes = GetDefaultImage();
            }

            // get image from personImageBytes
            Bitmap personImage;

            using (var ms = new MemoryStream(personImageBytes))
            {
                personImage = new Bitmap(ms);
            }

            var credentialIdCards = new List<string>();

            var response = new GenericResponse<List<Bitmap>>();

            var firstCredentialId = 0;

            var firstInterpreterCredential = allActiveCredentials.FirstOrDefault(x => x.CredentialTypeExternalName.Contains("Interpreter"));

            if(firstInterpreterCredential.IsNull())
            {
                firstCredentialId = allActiveCredentials.FirstOrDefault().Id;
            }

            if(firstInterpreterCredential.IsNotNull())
            {
                firstCredentialId = firstInterpreterCredential.Id;
            }

            var credentialQrCodeResponse = GetCredentialQRCodeForIdCardResponse(firstCredentialId);

            if (credentialQrCodeResponse.Success)
            {
                var credentialQrCode = credentialQrCodeResponse.Data;
                Bitmap idCardFront = new Bitmap(480, 640);

                using (Graphics canvas = Graphics.FromImage(idCardFront))
                {
                    canvas.FillRoundedRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 0, 480, 212), 40);
                    canvas.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 40, 480, 172));

                    canvas.FillRectangle(new SolidBrush(Color.FromArgb(255, 202, 233, 228)), new Rectangle(0, 212, 480, 348));

                    canvas.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 560, 480, 40));
                    canvas.FillRoundedRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 560, 480, 80), 40);

                    var naatiAlternateLogoWhite = Resources.CredentialQr.Alternate_logo_White_2022;
                    var naatiAlternateLogoWhiteScaled = ResizeImage(naatiAlternateLogoWhite, 375, 100);

                    canvas.DrawImage(naatiAlternateLogoWhiteScaled, 52, 10, 375, 100);

                    Bitmap personImageScaled = ResizeImage(personImage, 185, 185);
                    Image personImageScaledAndRounded = RoundCorners(personImageScaled, 20);

                    canvas.DrawImage(personImageScaledAndRounded, 240 - (personImageScaled.Width / 2), 212 - (personImageScaled.Height / 2));

                    var practitionerName = $"{credentialQrCode.Title} {credentialQrCode.FirstName} {credentialQrCode.LastName}";

                    var practitionerNameWidth = TextRenderer.MeasureText(practitionerName, new Font("Raleway", 18, FontStyle.Bold)).Width;
                    var maximumWidth = 478;

                    if (practitionerNameWidth > maximumWidth)
                    {
                        LoggingHelper.LogWarning($"Practitioner ID card may render with issues due to the practitioner name '{practitionerName}' with width {practitionerNameWidth} exceeds the maximum width of {maximumWidth}.");
                    }

                    canvas.DrawString(practitionerName, new Font("Raleway", 18, FontStyle.Bold),
                        new SolidBrush(Color.FromArgb(255, 0, 92, 93)), new Rectangle(1, 318, 478, 28), new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

                    canvas.DrawString(credentialQrCode.PractitionerNumber, new Font("Calibri", 18, GraphicsUnit.Pixel), new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(20, 346, 440, 28),
                        new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

                    // generate qr code
                    // error happening here somewhere. Logging is to help locate error
                    LoggingHelper.LogInfo("Creating QR Code Generator...");
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    LoggingHelper.LogInfo("QR Code Generator Created...");
                    var qrCodeAccessUrl = ConfigurationManager.AppSettings["QrCodeAccessUrl"];
                    LoggingHelper.LogInfo($"URL Template: {qrCodeAccessUrl}");
                    var qrCodeUrl = string.Format(qrCodeAccessUrl, credentialQrCode.QrCodeGuid);
                    LoggingHelper.LogInfo($"qrCodeURL after replaced qr code guid: {qrCodeUrl}");
                    LoggingHelper.LogInfo($"Creating QR Data...");
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
                    LoggingHelper.LogInfo("QR Data Created...");
                    LoggingHelper.LogInfo("Creating Qr Code...");
                    QRCode qrCode = new QRCode(qrCodeData);
                    LoggingHelper.LogInfo("QR Code Created...");
                    LoggingHelper.LogInfo("Generating Qr Image...");
                    Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.FromArgb(255, 0, 147, 131), Color.White, Resources.CredentialQr.NaatiLogoForQR, 20, 2, false, Color.White);
                    LoggingHelper.LogInfo("QR image generated...");
                    Bitmap scaledQrCodeImg = new Bitmap(qrCodeImage, 160, 160);
                    canvas.FillRectangle(new SolidBrush(Color.White), new Rectangle(155, 374, 170, 170));
                    canvas.DrawImage(scaledQrCodeImg, 160, 379);
                    LoggingHelper.LogInfo("QR Image Drawn to ID Card...");
                }

                var idCardFrontUrl = GetCredentialIdCardUrl(idCardFront);

                credentialIdCards.Add(idCardFrontUrl);

                Bitmap credentialIdCard = new Bitmap(480, 640);
                var credentialTextBuffer = 0;

                using (Graphics canvas = Graphics.FromImage(credentialIdCard))
                {
                    canvas.FillRoundedRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 0, 480, 80), 40);
                    canvas.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 40, 480, 40));

                    canvas.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255, 255)), new Rectangle(0, 80, 480, 480));

                    canvas.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 560, 480, 40));
                    canvas.FillRoundedRectangle(new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(0, 560, 480, 80), 40);

                    canvas.DrawLine(new Pen(Color.Black, 1), 0, 80, 0, 560);
                    canvas.DrawLine(new Pen(Color.Black, 1), 479, 80, 479, 560);

                    foreach (var credential in allActiveCredentials)
                    {
                        canvas.DrawString(credential.CredentialTypeExternalName, new Font("Raleway", 18, FontStyle.Bold),
                            new SolidBrush(Color.FromArgb(255, 0, 0, 0)), new Rectangle(50, 120 + credentialTextBuffer, 440, 28), new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near });

                        canvas.DrawString(credential.SkillDisplayName, new Font("Raleway", 16),
                            new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(50, 148 + credentialTextBuffer, 440, 28), new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near });

                        canvas.DrawString($"Expiry Date: {credential.ExpiryDate.Value:dd/MM/yyyy}", new Font("Raleway", 16),
                            new SolidBrush(Color.FromArgb(255, 0, 147, 131)), new Rectangle(50, 176 + credentialTextBuffer, 440, 28), new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near });

                        credentialTextBuffer += 94;
                    }
                }

                var credentialIdCardUrl = GetCredentialIdCardUrl(credentialIdCard);

                credentialIdCards.Add(credentialIdCardUrl);
            }

            return credentialIdCards;
        }

        public GenericResponse<Bitmap> GetCredentialQrCodeForStamp(int credentialId)
        {
            var credentialQrCodeResponse = GetCredentialQRCodeForStampResponse(credentialId);

            if (credentialQrCodeResponse.Success)
            {
                var qrCodeImage = GenerateCredentialQrCode(credentialQrCodeResponse.Data.QrCodeGuid);

                return qrCodeImage;
            }

            return new GenericResponse<Bitmap>(null) { Success = false, Errors = credentialQrCodeResponse.Errors };
        }

        public GenericResponse<Bitmap> GetCredentialQrCodeForIdCard(int credentialId)
        {
            var credentialQrCodeResponse = GetCredentialQRCodeForStampResponse(credentialId);

            if (credentialQrCodeResponse.Success)
            {
                var qrCodeImage = GenerateCredentialQrCode(credentialQrCodeResponse.Data.QrCodeGuid);

                return qrCodeImage;
            }

            return new GenericResponse<Bitmap>(null) { Success = false, Errors = credentialQrCodeResponse.Errors };
        }

        public GenericResponse<MfaDetailsModel> GetMFAConfiguredAndValid(int naatiNumber)
        {
            var personResponse = _personQueryService.GetPersonMfaDetails(naatiNumber);
            var person = personResponse.Data;
            var mfaActive = person.MfaExpireStartDate.IsNotNull() &&
                            person.MfaExpireStartDate.GetValueOrDefault().AddHours(_lookupProvider.SystemValues.MfaAndAccessCodeExpiryHours) > DateTime.Now;

            if(!mfaActive)
            {
                try
                {
                    LoggingHelper.LogInfo($"Mfa not configured StartDate:{person.MfaExpireStartDate},{_lookupProvider.SystemValues.MfaAndAccessCodeExpiryHours}");
                }
                catch(Exception ex)
                {
                    LoggingHelper.LogInfo(ex.ToString()); ;
                }
            }

            return new MfaDetailsModel()
            {
                MfaActive = mfaActive,
                MfaConfigured = person.MfaCode.IsNotNull(),
                Email = person.Email,
            };
        }

        public GenericResponse<bool> GetIsDeceasedFromPractitionerId(string practitionerId)
        {
            var practitionerDeceasedResponse = _credentialQrCodeDalService.GetIsDeceasedFromPractitionerId(practitionerId);

            if (!practitionerDeceasedResponse.Success)
            {
                return new GenericResponse<bool>(practitionerDeceasedResponse.Data)
                {
                    Success = false,
                    Errors = practitionerDeceasedResponse.Errors,
                };
            }

            return practitionerDeceasedResponse.Data;
        }

        public byte[] GetDefaultImage()
        {
            var defaultImageString = Resources.Test.DefaultAvatarBase64.Replace("data:image/png;base64,","");
            return Convert.FromBase64String(defaultImageString);
        }

        private Bitmap GenerateCredentialQrCode(Guid credentialQrCodeGuid)
        {
            // generate qr code  
            // error happening here somewhere. Logging is to help locate error
            LoggingHelper.LogInfo("Creating QR Code Generator...");
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            LoggingHelper.LogInfo("QR Code Generator Created...");
            var qrCodeAccessUrl = ConfigurationManager.AppSettings["QrCodeAccessUrl"];
            LoggingHelper.LogInfo($"URL Template: {qrCodeAccessUrl}");
            var qrCodeUrl = string.Format(qrCodeAccessUrl, credentialQrCodeGuid);
            LoggingHelper.LogInfo($"qrCodeURL after replaced qr code guid: {qrCodeUrl}");
            LoggingHelper.LogInfo($"Creating QR Data...");
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
            LoggingHelper.LogInfo("QR Data Created...");
            LoggingHelper.LogInfo("Creating Qr Code...");
            QRCode qrCode = new QRCode(qrCodeData);
            LoggingHelper.LogInfo("QR Code Created...");
            LoggingHelper.LogInfo("Generating Qr Image...");
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.FromArgb(0, Color.White), Resources.CredentialQr.NaatiLogoForQR, 25, 10, true, Color.White);
            LoggingHelper.LogInfo("QR Code Image Generated...");
            return qrCodeImage;
        }

        private GenericResponse<CredentialQrCodeDto> GetCredentialQRCodeForStampResponse(int credentialId)
        {
            var credentialQrCodeResponse = _credentialQrCodeDalService.GetCredentialQRCodeForStampAndId(credentialId);

            if (credentialQrCodeResponse.Success)
            {
                return credentialQrCodeResponse;
            }

            return new GenericResponse<CredentialQrCodeDto>(null) { Success = false, Errors = credentialQrCodeResponse.Errors };
        }

        private GenericResponse<CredentialQrCodeDto> GetCredentialQRCodeForIdCardResponse(int credentialId)
        {
            var credentialQrCodeResponse = _credentialQrCodeDalService.GetCredentialQRCodeForStampAndId(credentialId);

            if (credentialQrCodeResponse.Success)
            {
                return credentialQrCodeResponse;
            }

            return new GenericResponse<CredentialQrCodeDto>(null) { Success = false, Errors = credentialQrCodeResponse.Errors };
        }

        public Bitmap ResizeImage(Bitmap image, int maxWidth, int maxHeight)
        {
            if (maxHeight > 0 && maxWidth > 0)
            {
                int width = image.Width;
                int height = image.Height;
                float ratioBitmap = (float)width / (float)height;
                float ratioMax = (float)maxWidth / (float)maxHeight;

                int finalWidth = maxWidth;
                int finalHeight = maxHeight;
                if (ratioMax > ratioBitmap)
                {
                    finalWidth = (int)((float)maxHeight * ratioBitmap);
                }
                else
                {
                    finalHeight = (int)((float)maxWidth / ratioBitmap);
                }
                image = new Bitmap(image, finalWidth, finalHeight);
                return image;
            }
            else
            {
                return image;
            }
        }

        private Image RoundCorners(Image StartImage, int CornerRadius)
        {
            CornerRadius *= 2;
            Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
            using (Graphics g = Graphics.FromImage(RoundedImage))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush brush = new TextureBrush(StartImage);
                GraphicsPath gp = new GraphicsPath();
                gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                g.FillPath(brush, gp);
                return RoundedImage;
            }
        }

        private string GetCredentialIdCardUrl(Bitmap finalCard)
        {
            byte[] finalCardBytes;

            using (var stream = new MemoryStream())
            {
                finalCard.Save(stream, ImageFormat.Png);
                finalCardBytes = stream.ToArray();
            }

            var finalCardBase64 = Convert.ToBase64String(finalCardBytes);

            var finalCardUrl = $"data:image/png;base64, {finalCardBase64}";

            return finalCardUrl;
        }

        public GenericResponse<VerifyPractitionerServiceModel> GetVerifyPractitionerModelFromQrCode(Guid QrCode)
        {
            var result = _credentialQrCodeDalService.GetPractitionerVerificationModelFromQrCode(QrCode);
            if (!result.Success)
            {
                return new GenericResponse<VerifyPractitionerServiceModel>()
                {
                    Errors = result.Errors,
                    Success = result.Success
                };
            }
            var model = result.Data;
            var serviceModel = new VerifyPractitionerServiceModel()
            {
                PractitionerId = model.PractitionerId,
                GeneratedOn = model.GeneratedOn,
                IsDeceased = model.QrCodeModelDtos.Any()? model.QrCodeModelDtos.First().IsDeceased:false,
                AlowVerifyOnline = model.QrCodeModelDtos.Any() ? model.QrCodeModelDtos.First().AlowVerifyOnline : true,
                QrCodes = ConvertQrCodesToServiceModel(model.QrCodeModelDtos)
            };

            return serviceModel;
        }

        public GenericResponse<bool> DoesCredentialBelongToUser(int credentialId, int currentUserNaatiNumber)
        {
            var result = _credentialQrCodeDalService.DoesCredentialBelongToUser(credentialId, currentUserNaatiNumber);
            if (!result.Success)
            {
                return new GenericResponse<bool>()
                {
                    Errors = result.Errors,
                    Success = result.Success
                };
            }

            return result.Data;
        }

        private List<QrCodeModel> ConvertQrCodesToServiceModel(IList<QrCodeModelDto> qrCodeModelDtos)
        {
            var result = new List<QrCodeModel>();
            foreach(var dto in qrCodeModelDtos)
            {
                result.Add(new QrCodeModel()
                {
                    InactiveDate = dto.InactiveDate,
                    IssueDate = dto.IssueDate,
                    QrCode = dto.QrCode,
                });
            }

            return result;
        }
    }
}