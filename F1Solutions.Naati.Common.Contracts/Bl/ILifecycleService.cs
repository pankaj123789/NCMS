using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Bl
{
    public interface ILifecycleService
    {
        void Stop();

        //bool IsLocalIp(string ipAddress);

        string GetMachineName();

        string GetSystemIp();

        IEnumerable<PodHistoryModel> GetTerminatedPods(GetTerminatedPodsRequest request);

        void DeletePodHistory(IEnumerable<int> podHistoryIds);

        void LogPodDHistoryDeletionError(int podHistoryId, string errorMessage);
    }

    public class GetTerminatedPodsRequest
    {
        public DateTime MaxTerminationDate { get; set; }
        public int Take { get; set; }
    }
}
