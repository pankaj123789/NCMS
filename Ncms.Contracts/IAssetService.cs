using System.Collections.Generic;
using Ncms.Contracts.Models;

namespace Ncms.Contracts
{
    public interface IAssetService
    {
        IList<TestAttendanceAssetSearchResultModel> SearchAssets(TestAttendanceAssetSearchRequestModel search);
        AssetModel GetAsset(int assetId);
        void CreateMaterial(MaterialModel model);
        void UpdateMaterial(MaterialModel model);
        void DeleteMaterial(int materialId, bool permanentDelete);
        BulkFileDownloadModel GetAssetFilesAsZip(IEnumerable<int> storedFileIds);
    }
}
