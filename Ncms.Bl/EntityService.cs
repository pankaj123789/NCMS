using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;

namespace Ncms.Bl
{
    public class EntityService : IEntityService
    {
        private readonly IUtilityQueryService _utilityQueryService;
        public EntityService(IUtilityQueryService utilityQueryService)
        {
            _utilityQueryService = utilityQueryService;
        }


        public IEnumerable<IDictionary<string, object>> List(string entityName)
        {
            var result = _utilityQueryService.GetEntityRecords(entityName);
            var properties = result.PropertyNames.ToList();
            var dictionaries = new ConcurrentBag<IDictionary<string, object>>();
            Parallel.ForEach(result.PropertyValues, source => dictionaries.Add(GetDictionary(properties, source)));
           
            return dictionaries;
        }

        private IDictionary<string, object> GetDictionary(IList<string> propertyNames, IList<string> propertyValues)
        {
            var dictionary = new Dictionary<string, object>();
            for (var i = 0; i < propertyNames.Count; i++)
            {
                var propertyName = propertyNames[i];
                var propertyValue = propertyValues[i];
                double doubleValue;
                bool booleanValue;
                if (double.TryParse(propertyValue, out doubleValue))
                {
                    dictionary[propertyName] = doubleValue;
                }
                else if (bool.TryParse(propertyValue, out booleanValue)) 
                {
                    dictionary[propertyName] = booleanValue;
                }
                else
                {
                    dictionary[propertyName] = propertyValue;
                }
               
            }

            return dictionary;
        }
    }
}
