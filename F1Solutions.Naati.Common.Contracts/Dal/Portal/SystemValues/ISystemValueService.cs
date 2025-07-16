using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues
{
    
    public interface ISystemValueService : IInterceptableservice
    {
        
        IList<SystemValue> GetAll();
    }
}
