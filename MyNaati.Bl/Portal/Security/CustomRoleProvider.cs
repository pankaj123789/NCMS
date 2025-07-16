using System.Collections.Specialized;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;

namespace MyNaati.Bl.Portal.Security
{
    public class CustomRoleProvider : System.Web.Security.SqlRoleProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            var secretsProvider = ServiceLocator.Resolve<ISecretsCacheQueryService>();
            config["connectionString"] = secretsProvider.Get("ConnectionString");
            base.Initialize(name, config);
        }
    }
}
