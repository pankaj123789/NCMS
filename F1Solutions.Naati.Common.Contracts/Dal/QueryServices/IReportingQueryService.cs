using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface IReportingQueryService : IQueryService
    {
        void ProcessExecuteNcmsReports();

        void ProcessSyncReportLogs();

        void ClearNcmsReportingCache();

    }

}
