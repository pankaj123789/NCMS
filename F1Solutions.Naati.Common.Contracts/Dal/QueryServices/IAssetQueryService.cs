using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IAssetQueryService : IQueryService
    {
        
        SearchTestAttendanceAssetsResponse SearchTestAttendanceAssets(SearchTestAttendanceAssetsRequest request);

        
        GetTestAttendanceAssetResponse GetTestAttendanceAsset(GetTestAttendanceAssetRequest request);

        
        CreateOrUpdateMaterialResponse CreateMaterial(CreateOrUpdateMaterialRequest request);

        
        CreateOrUpdateMaterialResponse UpdateMaterial(CreateOrUpdateMaterialRequest request);

        
        DeleteSubmittedMaterialResponse DeleteMaterial(DeleteSubmittedMaterialRequest request);
    }
}
