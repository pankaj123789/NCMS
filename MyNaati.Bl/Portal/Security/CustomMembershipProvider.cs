using System.Collections.Specialized;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal.Security
{
    public class CustomMembershipProvider : System.Web.Security.SqlMembershipProvider
    {
        private IConfigurationService _ConfigurationService;
        private PasswordGenerator _PasswordGenerator;

        public IConfigurationService ConfigurationService => _ConfigurationService ?? (_ConfigurationService = ServiceLocator.Resolve<IConfigurationService>());
        public PasswordGenerator PasswordGenerator => _PasswordGenerator ?? (_PasswordGenerator = ServiceLocator.Resolve<PasswordGenerator>());

        public override void Initialize(string name, NameValueCollection config)
        {
            var secretsProvider = ServiceLocator.Resolve<ISecretsCacheQueryService>();
            config["connectionString"] = secretsProvider.Get("ConnectionString");
            base.Initialize(name, config);
        }

        public override string GeneratePassword()
        {
            return PasswordGenerator.GetNewEPortalPassword();
        }

        public override int MaxInvalidPasswordAttempts => ConfigurationService.PasswordLockoutCount();
        public override int MinRequiredPasswordLength => ConfigurationService.MinimumPasswordLength();
    }
}
