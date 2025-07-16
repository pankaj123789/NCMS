using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;


namespace Ncms.Contracts
{
    public interface IBackgroundOperationScheduler
    {
        BusinessServiceResponse Enqueue(NcmsBackgoundOperationTypeName jobName, IDictionary<string,string> parameters);
    }

    public enum NcmsBackgoundOperationTypeName
    {
        DownloadAllTestMaterials,
    }

    public static class BackgroundOperationParameters
    {
        public const string TestSessionId = "TestSessionId";
        public const string IncludeExaminers = "IncludeExaminers";
        public const string CreatedUserName = "CreatedUserName";
    }



}
