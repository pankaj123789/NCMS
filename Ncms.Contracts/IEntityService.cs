using System.Collections.Generic;

namespace Ncms.Contracts
{
    public interface IEntityService
    {
        IEnumerable<IDictionary<string, object>> List(string entityName);
    }
}
