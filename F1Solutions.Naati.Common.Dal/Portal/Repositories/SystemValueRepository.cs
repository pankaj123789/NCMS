using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;
using NHibernate;
using SystemValue = F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValue;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
     public interface ISystemValueRepository : IRepository<Contracts.Dal.Portal.SystemValue>
    {
        SystemValue GetByKey(string key);

        IList<SystemValue> GetByKeys(IList<string> keys);

        IEnumerable<SystemValue> GetAll();
        IEnumerable<SystemValue> GetAll(ISession sessionToUse);
    }

    public class SystemValueRepository : Repository<SystemValue>, ISystemValueRepository
    {
        public SystemValueRepository(ICustomSessionManager sessionManager) : base(sessionManager)
        {

        }

        public SystemValue GetByKey(string key)
        {
            var result = new SystemValue{Key = key};

            var systemVar = Session
                .Query<Naati.Common.Dal.Domain.SystemValue>()
                .SingleOrDefault(value => value.ValueKey.Equals(key));

            if (systemVar == null)
            {
                var property = typeof(SystemParameter).GetProperties().First(x => x.Name == key);
                result.Value = property.GetValue(Session.Query<SystemParameter>().First(), null).ToString();
            }
            else
            {
                result.Value = systemVar.Value;
            }

            return result;
        }

        public IList<SystemValue> GetByKeys(IList<string> keys)
        {
            return keys.Select(GetByKey).ToList();
        }

        public IEnumerable<SystemValue> GetAll()
        {
            return GetAll(Session);
        }

        public IEnumerable<SystemValue> GetAll(ISession sessionToUse)
        {
            var systemValues = new List<SystemValue>();

            systemValues.AddRange(
                sessionToUse.Query<Naati.Common.Dal.Domain.SystemValue>().Select(x => new SystemValue { Key = x.ValueKey, Value = x.Value }));

            var systemParams = sessionToUse.Query<SystemParameter>().First();

            foreach (var property in typeof(SystemParameter).GetProperties().Where(x => x.DeclaringType == typeof(SystemParameter)))
            {
                if (systemValues.All(x => x.Key != property.Name))
                {
                    systemValues.Add(new SystemValue { Key = property.Name, Value = property.GetValue(systemParams, null).ToString() });
                }
            }

            return systemValues;
        }
    }
}

