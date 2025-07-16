using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;

namespace MyNaati.Bl.BackOffice
{
   
    public class SystemValueService : ISystemValueService
    {
        private ISystemValueRepository mSystemValueRepository;

        public SystemValueService(ISystemValueRepository systemValueRepository) 
        {
            mSystemValueRepository = systemValueRepository;
        }

        public IList<SystemValue> GetAll()
        {
            return mSystemValueRepository.GetAll().ToList();
        }
    }
}
