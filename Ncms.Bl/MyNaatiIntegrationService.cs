using System.Configuration;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Bl.Services;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl
{
    public class MyNaatiIntegrationService : IntegrationService, IMyNaatiIntegrationService
    {
        private readonly ISystemService _systemService;

        protected override string BaseAddress { get; }

        protected override string AuthenticationScheme => SecuritySettings.MyNaatiPrivateApiScheme;

        protected override string GetPrivateKey()
        {
            var ncmsPrivateKey = _systemService.GetSystemValue(SecuritySettings.NcmsPrivateKeyValue);
            ncmsPrivateKey = HmacCalculatorHelper.UnProtectKey(ncmsPrivateKey);
            return ncmsPrivateKey;
        }

        protected override string GetPublicKey()
        {
            var ncmsPublicKey = _systemService.GetSystemValue(SecuritySettings.NcmsPublicKeyValue);
            ncmsPublicKey = HmacCalculatorHelper.UnProtectKey(ncmsPublicKey);
            return ncmsPublicKey;
        }

        public MyNaatiIntegrationService(ISystemService systemService)
        {
            _systemService = systemService;
            BaseAddress = ConfigurationManager.AppSettings["MyNaatiUrl"];
        }

        public GenericResponse<bool> DeleteUser(string userName)
        {
            var request = MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.DeleteUser;
            var response = SendPostRequest<object, GenericResponse<bool>>(new { userName }, request);
            return response;
        }

        public BusinessServiceResponse RenameUser(string oldUserName, string newUserName)
        {
            var request = MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.RenameUser;
            var response = SendPostRequest<object, BusinessServiceResponse>(new { oldUserName, newUserName }, request);
            return response;
        }

        public GenericResponse<bool> UnlockUser(string userName)
        {
            var request = MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.UnlockUser;
            var response = SendPostRequest<object, GenericResponse<bool>>(new { userName }, request);
            return response;
        }

        public GenericResponse<MyNaatiUserDetailsModel> GetUser(string userName)
        {
            var request = MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.GetUser;
            var response = SendPostRequest<object, GenericResponse<MyNaatiUserDetailsModel>>(new { userName }, request);
            return response;
        }
    }
}
