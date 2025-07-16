using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;

namespace Ncms.Bl
{
    public class AssetService : IAssetService
    {
        private readonly IAssetQueryService _assetQueryService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public AssetService(IAssetQueryService assetQueryService, IFileStorageService fileStorageService, IAutoMapperHelper autoMapperHelper)
        {
            _assetQueryService = assetQueryService;
            _fileStorageService = fileStorageService;
            _autoMapperHelper = autoMapperHelper;
        }
        public IList<TestAttendanceAssetSearchResultModel> SearchAssets(TestAttendanceAssetSearchRequestModel search)
        {
            // get a list of assets without populating the FileData property
            var request = _autoMapperHelper.Mapper.Map<SearchTestAttendanceAssetsRequest>(search);

            SearchTestAttendanceAssetsResponse response = null;
            response = _assetQueryService.SearchTestAttendanceAssets(request);

            var mapper = new AssetSearchMapper();

            if (response?.TestAttendanceAssets == null)
            {
                return new List<TestAttendanceAssetSearchResultModel>();
            }

            return response.TestAttendanceAssets.Select(asset =>
            {
                var model = mapper.Map(asset);
                model.UploadedByName = String.IsNullOrWhiteSpace(asset.UploadedByPersonName) ? asset.UploadedByUserName : asset.UploadedByPersonName;
                if (asset.UploadedByPersonId.HasValue)
                {
                    // UC-SAM-1002 BR7.	The uploaded by column will append the word ‘Examiner’ to files uploaded through the Examiner ePortal.
                    model.UploadedByName += " (Examiner)";
                }
                return model;
            }).ToList();
        }

        public AssetModel GetAsset(int assetId)
        {
            // get asset metadata
            var assetRequest = new GetTestAttendanceAssetRequest { TestAttendanceAssetId = assetId };
            GetTestAttendanceAssetResponse assetResponse = null;
             assetResponse = _assetQueryService.GetTestAttendanceAsset(assetRequest);

            if (assetResponse?.TestAttendanceAsset == null)
            {
                throw new Exception($"Cannot find asset with ID {assetId}.");
            }

            var mapper = new AssetMapper();
            var asset = mapper.Map(assetResponse.TestAttendanceAsset);

            // get file data
            var fileRequest = new GetFileRequest { StoredFileId = asset.StoredFileId, TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] };
            var fileResponse = _fileStorageService.GetFile(fileRequest);
            var fileData = new MemoryStream(File.ReadAllBytes(fileResponse.FilePaths[0]), false);
            fileData.Position = 0;
            File.Delete(fileResponse.FilePaths[0]);
            asset.FileData = fileData;
            asset.FileName = fileResponse.FileName;

            return asset;
        }

        public BulkFileDownloadModel GetAssetFilesAsZip(IEnumerable<int> storedFileIds)
        {
            var filesRequest = new GetFilesRequest { StoredFileIds = storedFileIds.ToArray(), TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] };
            GetFileResponse fileResponse = null;

            fileResponse = _fileStorageService.GetFiles(filesRequest);

            if (fileResponse == null)
            {
                throw new Exception("Null reponse from file storage service");
            }

            return new BulkFileDownloadModel
            {
                FileData = FileService.CreateZip(fileResponse.FilePaths),
                // UC-SAM-1002 BR8
                FileName = $"Test Assets {DateTime.Now:ddMMyy HHmmss}.zip",
                FileType = FileType.Zip
            };
        }

        public void CreateMaterial(MaterialModel model)
        {
            _assetQueryService.CreateMaterial(
                    new CreateOrUpdateMaterialRequest
                    {
                        StoredFileId = model.StoredFileId,
                        Title = model.Title,
                        TestMaterialId = model.TestMaterialId
                    });
        }

        public void UpdateMaterial(MaterialModel model)
        {
            _assetQueryService.UpdateMaterial(
                    new CreateOrUpdateMaterialRequest
                    {
                        MaterialId = model.MaterialId,
                        TestMaterialId = model.TestMaterialId,
                        Deleted = model.Deleted,
                        StoredFileId = model.StoredFileId,
                        Title = model.Title
                    });
        }

        public void DeleteMaterial(int materialId, bool permanentDelete)
        {
           _assetQueryService.DeleteMaterial(
                    new DeleteSubmittedMaterialRequest
                    {
                        MaterialId = materialId,
                        PermanentDelete = permanentDelete
                    });
        }
    }
}
