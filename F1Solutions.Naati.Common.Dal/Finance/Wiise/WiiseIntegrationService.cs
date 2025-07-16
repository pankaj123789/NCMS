using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Wiise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiiseIntegrationService : IWiiseTokenProvider,IWiiseIntegrationService
    {
        private readonly IWiiseAuthorisationService _wiiseAuthorisationService;
        private string _wiiseTenantId;
        private IWiiseAccountingApi _apiInstance;
        public WiiseIntegrationService(IWiiseAuthorisationService wiiseAuthorisationService)
        {
            _wiiseAuthorisationService = wiiseAuthorisationService;
        }

        public WiiseToken GetToken()
        {
            var token =  new WiiseToken(_wiiseAuthorisationService.GetFreshAccessToken(), Tenant);
            return token;
        }

        public IWiiseAccountingApi Api => _apiInstance ?? (_apiInstance = ServiceLocator.Resolve<IWiiseAccountingApi>());


        protected string Tenant => _wiiseTenantId ?? (_wiiseTenantId = _wiiseAuthorisationService.GetTenant());
    }
}
