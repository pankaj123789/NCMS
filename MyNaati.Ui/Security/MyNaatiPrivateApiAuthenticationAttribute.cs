using System.Security.Principal;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using MyNaati.Contracts.Portal;

namespace MyNaati.Ui.Security
{
    public class MyNaatiPrivateApiAuthenticationAttribute : PrivateApiAuthenticationAttribute
    {
        private readonly EndpointCaller _caller;
        protected override string AuthenticationScheme => SecuritySettings.MyNaatiPrivateApiScheme;
        
        private static string _defaultUserName;
        protected override string DefaultUserName => _defaultUserName ?? (_defaultUserName =
                                                         ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(_caller == EndpointCaller.MyNaati ? SecuritySettings.MyNaatiDefaultIdentityKey:
                                                             SecuritySettings.NcmsDefaultIdentityKey));

        public MyNaatiPrivateApiAuthenticationAttribute(EndpointCaller caller)
        {
            _caller = caller;
        }

        protected override string GetPrivateKey()
        {
            var service = ServiceLocator.Resolve<IConfigurationService>();
            var ncmsPrivateKey = service.GetSystemValue(_caller == EndpointCaller.MyNaati ? SecuritySettings.MyNaatiPrivateKeyValue : SecuritySettings.NcmsPrivateKeyValue);
            return ncmsPrivateKey;
        }

        protected override IPrincipal GetPrincipal(string userName)
        {
            var currentPrincipal = new GenericPrincipal(new GenericIdentity(userName), null);
            return currentPrincipal;
        }
    }
}

   