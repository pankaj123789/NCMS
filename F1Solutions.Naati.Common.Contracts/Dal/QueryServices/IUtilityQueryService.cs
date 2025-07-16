using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface IUtilityQueryService : IQueryService
    {
        EntityRecordsResponse GetEntityRecords(string tableName);
        LookupTypeResponse GetLookupType(LookupType lookupType);
        void LogSystemTerminationDate();
        string GetMachineName();

        void LogSystemStartDate();

        string GetSystemIp();

      //  bool IsLocalIp(string ipAddress);

        IEnumerable<PodHistoryDto> GetTerminatedPods(GetTerminatedPodsRequest request);
        void DeletePodHistory(IEnumerable<int> podHistoryIds);

        void LogPodDHistoryDeletionError(int podHistoryId, string errorMessage);

        bool RequestJobToken<TJobType>(TJobType jobTypeName);

        bool ReleaseJobToken<TJobType>(TJobType jobTypeName);
    }

    public enum ProductCategoryTypeName
    {
        AssessmentFee =1,
        TestingFee=2,
        AdminFee=3,
        Publications=4,
        Products =5,
        Marking = 6,
        MarkingReview =7,
        ApplicationFee=8,
        SupplementaryTestFee =9,
        ReviewFee=10,
        MaterialCreationFee = 11,
    }
}
