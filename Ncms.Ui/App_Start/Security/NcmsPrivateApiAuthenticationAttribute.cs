using System.Security.Principal;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl;
using Ncms.Bl.Security;
using Ncms.Contracts;

namespace Ncms.Ui.Security
{
    public class NcmsPrivateApiAuthenticationAttribute : PrivateApiAuthenticationAttribute
    {
        private readonly EndpointCaller _caller;

        public NcmsPrivateApiAuthenticationAttribute(EndpointCaller caller)
        {
            _caller = caller;
        }
        protected override string AuthenticationScheme => SecuritySettings.NcmsPrivateApiScheme;

        private static string _defaultUserName;

        protected override string DefaultUserName => _defaultUserName ?? (_defaultUserName =
                                                         ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(_caller == EndpointCaller.MyNaati ? SecuritySettings.MyNaatiDefaultIdentityKey: SecuritySettings.NcmsDefaultIdentityKey));
       
        protected override string GetPrivateKey()
        {
            var service = ServiceLocator.Resolve<ISystemService>();
            var myNaatiPrivateKey = service.GetSystemValue(_caller == EndpointCaller.MyNaati ? SecuritySettings.MyNaatiPrivateKeyValue : SecuritySettings.NcmsPrivateKeyValue);
            return myNaatiPrivateKey;
        }

        protected override IPrincipal GetPrincipal(string userName)
        {
            var currentPrincipal = new NcmsPrincipal(userName);
            return currentPrincipal;
        }
    }
}