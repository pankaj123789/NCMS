using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Saving;
using F1Solutions.Naati.Common;
using F1Solutions.Naati.Common.Bl;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Path = System.IO.Path;
    
namespace Ncms.Bl
{
    internal class PdfCreatorHelper : DocumentTokenReplacerHelper<PdfDataModel>
    {
        private IServiceLocator ServiceLocatorInstance { get; set; }
        private IPersonService PersonService => ServiceLocatorInstance.Resolve<IPersonService>();
        private IApplicationService ApplicationService => ServiceLocatorInstance.Resolve<IApplicationService>();

        public IList<DocumentData> CreateCredentialDocuments(CreateCredentialDocumentsRequestModel request, IEnumerable<CredentialTypeTemplateDto> templates, string tempFilePath)
        {
            var credentialDocuments = new List<DocumentData>();

            var pdfInfos = templates.Select(template =>
            {
                var documentNumber =
                    $"{request.CredentialId}-{request.ApplicationId}-{template.NextDocumentIdentifier}";
                var pdfDataModel = new PdfDataModel() { CredentialDataModel = request, DocumentNumber = documentNumber };
                var credentialPdf = ReplaceDocumentTokens(pdfDataModel, template.FilePath, new PdfSaveOptions());

                var documentName = GetDocumentNameFromTemplate(pdfDataModel, template.DocumentNameTemplate);

                var hasErrors = credentialPdf == null || string.IsNullOrEmpty(documentName);

                return new { hasErrors, template, documentNumber, credentialPdf, documentName };
            }).ToList();

            if (pdfInfos.Any(v => v.hasErrors))
            {
                throw new UserFriendlySamException($"Documents for credential id: {request.CredentialId} were not created because one or more tokens were not found.");
            }

            // The pdfs are created only if 'ALL' token replacement has been done for all documents.
            foreach (var pdfInfo in pdfInfos)
            {
                var pdfPath = Path.Combine(tempFilePath, $"{pdfInfo.documentName}.pdf");
                FileStream destination = new FileStream(pdfPath, FileMode.Create);
                pdfInfo.credentialPdf.CopyTo(destination);
                destination.Close();
                var fileInfo = new FileInfo(pdfPath);
                credentialDocuments.Add(new DocumentData
                {
                    FilePath = pdfPath,
                    FileSize = fileInfo.Length,
                    FileType = fileInfo.Extension,
                    DocumentNumber = pdfInfo.documentNumber,
                    StoredFileType = pdfInfo.template.OutputDocumentType,
                    DocumentTypeName = pdfInfo.template.OutputDocumentType.ToString()
                });
            }

            return credentialDocuments;
        }

        protected override IDictionary<string, string> GetTokens(PdfDataModel data)
        {
            var documentNumberFieldName = GetTokenNameFor(TokenReplacementField.DocumentNumber);
            var credentialTypeFieldName = GetTokenNameFor(TokenReplacementField.CredentialType);
            var credentialSkillFieldName = GetTokenNameFor(TokenReplacementField.Skill);
            var credentialCertificateLongDateName = GetTokenNameFor(TokenReplacementField.CertificateLongDate);
            var credentialLongExpiryDateName = GetTokenNameFor(TokenReplacementField.CertificateLongExpiryDate);
            var credentialCertificateShortDateName = GetTokenNameFor(TokenReplacementField.CertificateShortDate);
            var credentialShortExpiryDateName = GetTokenNameFor(TokenReplacementField.CertificateShortExpiryDate);

            var extraTokens =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { documentNumberFieldName, data.DocumentNumber },
                    { credentialTypeFieldName, data.CredentialDataModel.CredentialName },
                    { credentialSkillFieldName, data.CredentialDataModel.Skill },
                    { credentialCertificateLongDateName, data.CredentialDataModel.CredentialStartDate.ToLongDateWithoutWeekDayString() },
                    { credentialLongExpiryDateName, data.CredentialDataModel.CredentialExpiryDate?.ToLongDateWithoutWeekDayString() },
                    { credentialCertificateShortDateName, data.CredentialDataModel.CredentialStartDate.ToShortDateString() },
                    { credentialShortExpiryDateName, data.CredentialDataModel.CredentialExpiryDate?.ToShortDateString() },
                };

            if (!string.IsNullOrEmpty(data.CredentialDataModel.PractitionerNumber))
            {
                extraTokens[GetTokenNameFor(TokenReplacementField.PractitionerNumber)] = data.CredentialDataModel.PractitionerNumber;
            }

            return extraTokens;
        }

        protected override bool ReplaceTokens(PdfDataModel data, string text, Action<string, string> replacementAction, IDictionary<string, string> tokens)
        {
            IEnumerable<string> errors;
            var applicationDetailsResponse = ApplicationService.GetApplicationDetailsByApplicationId(new GetApplicationDetailsRequest { ApplicationId = data.CredentialDataModel.ApplicationId });
            var personInfoResponse = PersonService.GetPersonInfoResponse(applicationDetailsResponse.ApplicantDetails.NaatiNumber);

            TokenReplacementService.ReplaceTemplateFieldValues(text, applicationDetailsResponse, personInfoResponse.PersonDetails, personInfoResponse.ContactDetails, replacementAction, tokens, true, out errors);

            if (errors.Any())
            {
                return false;
            }

            return true;
        }

        private string GetDocumentNameFromTemplate(PdfDataModel request, string template)
        {
            string replacedValue = template;

            var tokens = GetTokens(request);
            var tokensReplaced = ReplaceTokens(request, template, (token, value) => replacedValue = replacedValue.Replace(token, value ?? string.Empty), tokens);

            if (!tokensReplaced)
            {
                return null;
            }

            return replacedValue;
        }

        public PdfCreatorHelper(ITokenReplacementService tokenReplacementService, IServiceLocator serviceLocator = null) : base(tokenReplacementService, "Aspose.Words.lic")
        {
            if (serviceLocator == null)
            {
                serviceLocator = ServiceLocator.GetInstance();
            }

            ServiceLocatorInstance = serviceLocator;
        }
    }

    public class PdfDataModel
    {
        public CreateCredentialDocumentsRequestModel CredentialDataModel { get; set; }
        public string DocumentNumber { get; set; }
    }
}
