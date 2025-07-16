using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using SystemValue = F1Solutions.Naati.Common.Dal.Domain.SystemValue;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IConfigurationRepository : Contracts.Dal.Portal.IRepository<SystemValue>
    {
        SystemValue GetSystemValueBykey(string key);

        void UpdateSystemValue(string key, string value);
    }

    public class ConfigurationRepository : Repository<SystemValue>, IConfigurationRepository
    {
        public ConfigurationRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public SystemValue GetSystemValueBykey(string key)
        {
            return Session.Query<SystemValue>().FirstOrDefault(e => e.ValueKey == key);
        }

        public void UpdateSystemValue(string key, string value)
        {
            SystemValue myNaatiSystemValue = GetSystemValueBykey(key) ?? new SystemValue() { ValueKey = key, Value = value };
            myNaatiSystemValue.Value = value;

            Session.Save(myNaatiSystemValue);
            Session.Flush();
        }
    }
}
