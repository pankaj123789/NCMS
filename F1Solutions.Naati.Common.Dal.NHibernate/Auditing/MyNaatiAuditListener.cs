using F1Solutions.Naati.Common.Contracts.Security;
using System.Configuration;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;

namespace F1Solutions.Naati.Common.Dal.NHibernate.Auditing
{
    public class MyNaatiAuditListener : AuditingListener
    {
        protected override string GetCurrentUserName()
        {
            var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();
            return secretsProvider.Get(SecuritySettings.MyNaatiDefaultIdentityKey);
        }
    }
}
