using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Bl.Services;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl
{
    public class NcmsIntegrationService : IntegrationService, INcmsIntegrationService
    {
        private readonly IConfigurationService _configurationService;

        public NcmsIntegrationService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            BaseAddress = ConfigurationManager.AppSettings["NcmsUrl"];
        }

        protected override string AuthenticationScheme => SecuritySettings.NcmsPrivateApiScheme;

        protected override string BaseAddress { get; }

        protected override string GetPrivateKey()
        {
            var myNaatiPrivateKey = _configurationService.GetSystemValue(SecuritySettings.MyNaatiPrivateKeyValue);
            myNaatiPrivateKey = HmacCalculatorHelper.UnProtectKey(myNaatiPrivateKey);
            return myNaatiPrivateKey;
        }

        protected override string GetPublicKey()
        {
            var myNaatiPublicKey = _configurationService.GetSystemValue(SecuritySettings.MyNaatiPublicKeyValue);
            myNaatiPublicKey = HmacCalculatorHelper.UnProtectKey(myNaatiPublicKey);
            return myNaatiPublicKey;
        }

        public CreateNcmsApplicationResponse CreateApplication(CreateNcmsApplicationRequest request)
        {
            var requestUrl = NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.CreateApplication;

            var response = SendPostRequest<CreateNcmsApplicationRequest, CreateNcmsApplicationResponse>(request, requestUrl);
            return response;
        }

        public BusinessServiceResponse CreateNcmsApplicationCredentialRequest(
            int applicationId,
            int skillId,
            int credentialTypeId,
            int categoryId)
        {
            var steps = GetCreateCredentialRequestSteps(applicationId, skillId, credentialTypeId, categoryId);
            return ExecuteNcmsApplicationAction(
                new NcmsApplicationActionRequest
                {
                    ApplicationId = applicationId,
                    ActionId = (int)SystemActionTypeName.NewCredentialRequest,
                    Steps = steps
                });
        }

        public BusinessServiceResponse ExecuteNcmsApplicationAction(NcmsApplicationActionRequest request)
        {
            var requestUrl = NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.ApplicationWizard;
            var response = SendPostRequest<NcmsApplicationActionRequest, BusinessServiceResponse>(request, requestUrl);
            return response;
        }

        public IList<string> ValidateCredentialApplication(NcmsApplicationActionRequest request)
        {
            var response = SendPostRequest<NcmsApplicationActionRequest, NcmstValidationResponse>(
                request,
                NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.ApplicationValidate);

            if (!response.Success)
            {
                return new[] { "General Error during validation occurred." };
            }

            return response.Data.Select(x => x.Message).ToList();
        }

        public BusinessServiceResponse ExecuteRolePlayerAction(NcmsRolePlayerActionRequest request)
        {
            var requestUrl = NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.RolePlayerWizard;
            var response = SendPostRequest<NcmsRolePlayerActionRequest, BusinessServiceResponse>(request, requestUrl);
            return response;
        }

        public BusinessServiceResponse UpdateNcmsOutstandingInvoices(string invoiceNumber, string paymentReference, string transactionId, string orderNumber)
        {
            var requestUrl = NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.ApplicationOutstandingInvoices;
            var request = new { InvoiceNumber = invoiceNumber, PaymentReference = paymentReference, TransactionId = transactionId, OrderNumber = orderNumber };
            var response = SendPostRequest<object, BusinessServiceResponse>(request, requestUrl);
            return response;
        }

        public IList<string> ValidateMaterialRequest(NcmsMaterialRequestActionRequest request)
        {
            var response = SendPostRequest<NcmsMaterialRequestActionRequest, NcmstValidationResponse>(request, 
                NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.MaterialRequestWizardValidate);

            if (!response.Success)
            {
                return new[] { "General Error during validation occurred." };
            }

            return response.Data.Select(x => x.Message).ToList();
        }

        public IList<string> ExecuteMaterialRequestAction(NcmsMaterialRequestActionRequest request)
        {
            var response = SendPostRequest<NcmsMaterialRequestActionRequest, BusinessServiceResponse>(request, 
                NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.MaterialRequestWizard);

            if (!response.Success)
            {
                return new[] { "General Error during validation occurred." };
            }

            return response.Errors;
        }

        public bool TestConnection()
        {
            var response = SendPostRequest<object, GenericResponse<bool>>(new object(),
                NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.TestConnection);
            return response.Success && response.Data;
        }

        public IEnumerable<string> ExecuteCredentialApplicationAction(NcmsMaterialRequestActionRequest request)
        {
            var response = SendPostRequest<NcmsMaterialRequestActionRequest, NcmstValidationResponse>(request, NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.ApplicationWizard);

            if (!response.Success)
            {
                return new[] { "General Error during validation occurred." };
            }

            return response.Data.Select(x => x.Message).ToList();
        }

        private IList<object> GetCreateCredentialRequestSteps(
            int applicationId,
            int skillId,
            int credentialTypeId,
            int categoryId)
        {
            var steps = new List<object>
            {
                new
                {
                    Id = 1,
                    Data = new
                    {
                        ApplicationId = applicationId,
                        CategoryId = categoryId,
                        CredentialTypeId = credentialTypeId,
                        SkillId = skillId
                    }
                }
            };
            return steps;
        }
    }
}