using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Hangfire;

namespace F1Solutions.Naati.Common.Bl.Services
{
    public class LifecycleService : ILifecycleService
    {
        private readonly IUtilityQueryService _utilityQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public LifecycleService(IUtilityQueryService utilityQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _utilityQueryService = utilityQueryService;
            _autoMapperHelper = autoMapperHelper;
        }
        public void Stop()
        {
            SystemLifecycleHelper.UpdateLifecycleStatus(SystemLifecycleStatus.Stopping);
            _utilityQueryService.LogSystemTerminationDate();
        }

        //public bool IsLocalIp(string ipAddress)
        //{
        //    return _utilityQueryService.IsLocalIp(ipAddress);
        //}

        public string GetMachineName()
        {
            return _utilityQueryService.GetMachineName();
        }

        public IEnumerable<PodHistoryModel> GetTerminatedPods(GetTerminatedPodsRequest request)
        {
            var terminatedPods = _utilityQueryService.GetTerminatedPods(request)
                .Select(_autoMapperHelper.Mapper.Map<PodHistoryModel>)
                .ToList();

            return terminatedPods;
        }

        public void DeletePodHistory(IEnumerable<int> podHistoryIds)
        {
            _utilityQueryService.DeletePodHistory(podHistoryIds);
        }

        public void LogPodDHistoryDeletionError(int podHistoryId, string errorMessage)
        {
            _utilityQueryService.LogPodDHistoryDeletionError(podHistoryId, errorMessage);
        }

        public string GetSystemIp()
        {
            return _utilityQueryService.GetSystemIp();
        }
    }
}
