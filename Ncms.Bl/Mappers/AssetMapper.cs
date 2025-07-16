using System;
using System.IO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Contracts.Models;

namespace Ncms.Bl.Mappers
{
    public class AssetMapper : BaseMapper<TestAttendanceAssetDto, AssetModel>
    {
        public override AssetModel Map(TestAttendanceAssetDto source)
        {
            return new AssetModel
            {
                TestAttendanceAssetId = source.TestAttendanceAssetId,
                StoredFileId = source.StoredFileId,
                Deleted = source.Deleted,
                Title = source.Title,
            };
        }

        public override TestAttendanceAssetDto MapInverse(AssetModel source)
        {
            throw new NotImplementedException();
        }
    }

    public class AssetSearchMapper : BaseMapper<TestAttendanceAssetSearchDto, TestAttendanceAssetSearchResultModel>
    {
        public override TestAttendanceAssetSearchResultModel Map(TestAttendanceAssetSearchDto source)
        {
            return new TestAttendanceAssetSearchResultModel
            {
                TestAttendanceAssetId = source.TestAttendanceDocumentId,
                StoredFileId = source.StoredFileId,
                FileSize = source.FileSize,
                Title = source.Title,
                DocumentType = source.DocumentType,
                // only one of these should ever have a value
                //UploadedByName = source.UploadedByPersonName ?? source.UploadedByUserName,
                UploadedDateTime = source.UploadedDateTime,
                NaatiNumber = source.NaatiNumber,
                TestAttendanceId = source.TestAttendanceId,
                TestMaterialId = source.TestMaterialId,
                MaterialId = source.MaterialId,
                FileType = Path.GetExtension(source.FileName)?.Trim('.'),
                ExaminerMarksRemoved = source.ExaminerMarksRemoved,
                EportalDownload = source.EportalDownload
            };
        }

        public override TestAttendanceAssetSearchDto MapInverse(TestAttendanceAssetSearchResultModel source)
        {
            throw new NotImplementedException();
        }
    }

}
