using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal
{
    public class AssetQueryService : IAssetQueryService
    {
        public SearchTestAttendanceAssetsResponse SearchTestAttendanceAssets(SearchTestAttendanceAssetsRequest request)
        {
            // TODO: AS OF FEB 2018 THIS VIEW (vwTestSittingAsset), IS BROKEN AND NEEDS TO BE OVERHAULED. IT WILL NOT WORK AS-IS.
            // If you are working on something that uses this view, you will need to update it. PLEASE REMOVE ALL REFERENCES TO
            // TEST MATERIALS. This view should only be used for Test Assets. 
            // NOTE: Updating of this view in NaatiScriptRunner is currently commented out.
            var Query = "select * from vwTestSittingAsset where ";

            var ClauseCount = 0;

            if (!request.IncludeDeleted)
            {
                Query += "Deleted = 0";
                ClauseCount++;
            }

            if (request.TestAttendanceId != null && request.TestAttendanceId.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"TestAttendanceId in ({GetListString(request.TestAttendanceId.ToArray())})";
                ClauseCount++;
            }

            if (request.TestMaterialId != null && request.TestMaterialId.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"TestMaterialId in ({GetListString(request.TestMaterialId.ToArray())})";
                ClauseCount++;
            }

            if (request.SatDateFrom.HasValue)
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"TestSatDate >= {'\'' + request.SatDateFrom.Value.ToString("yyyy/MM/dd") + '\''}";
                ClauseCount++;
            }

            if (request.SatDateTo.HasValue)
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"TestSatDate <= {'\'' + request.SatDateTo.Value.ToString("yyyy/MM/dd") + '\''}";
                ClauseCount++;
            }

            if (request.NaatiNumber != null && request.NaatiNumber.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"NaatiNumber in ({GetListString(request.NaatiNumber.ToArray())})";
                ClauseCount++;
            }

            if (request.JobId != null && request.JobId.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"JobId in ({GetListString(request.JobId.ToArray())})";
                ClauseCount++;
            }

            if (request.OfficeId != null && request.OfficeId.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"TestOfficeId in ({GetListString(request.OfficeId.ToArray())})";
                ClauseCount++;
            }

            if (request.UploadedByPersonNaatiNo != null && request.UploadedByPersonNaatiNo.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"UploadedByPersonNaatiNo in ({GetListString(request.UploadedByPersonNaatiNo.ToArray())})";
                ClauseCount++;
            }

            if (request.UploadedByUserId != null && request.UploadedByUserId.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"UploadedByUserId in ({GetListString(request.UploadedByUserId.ToArray())})";
                ClauseCount++;
            }

            if (request.TestAttendanceAssetType != null && request.TestAttendanceAssetType.Any())
            {
                if (ClauseCount > 0)
                {
                    Query += " AND ";
                }

                Query += $"[Type] in ({GetListString(request.TestAttendanceAssetType.ToArray())})";
                ClauseCount++;
            }

            var assetQuery = NHibernateSession.Current.TransformSqlQueryAliasToBeanResult<TestAttendanceAssetSearchDto>(Query).AsEnumerable();

            assetQuery = assetQuery.Where(x => x.UploadedByPersonId != null && x.SubmittedDate != null || x.UploadedByPersonId == null).GroupBy(x => x.StoredFileId).Select(x => x.First());

            return new SearchTestAttendanceAssetsResponse
            {
                TestAttendanceAssets = assetQuery.OrderByDescending(x => x.TestAttendanceId).ToList()
            };
        }

        public GetTestAttendanceAssetResponse GetTestAttendanceAsset(GetTestAttendanceAssetRequest request)
        {
            var asset = (from testAttendanceAsset in NHibernateSession.Current.Query<TestSittingDocument>()
                         join storedFile in NHibernateSession.Current.Query<StoredFile>() on testAttendanceAsset.StoredFile.Id equals storedFile.Id
                         where testAttendanceAsset.Id == request.TestAttendanceAssetId
                         select new TestAttendanceAssetDto
                         {
                             Deleted = testAttendanceAsset.Deleted,
                             Title = testAttendanceAsset.Title,
                             StoredFileId = storedFile.Id,
                         })
                .SingleOrDefault();

            return new GetTestAttendanceAssetResponse
            {
                TestAttendanceAsset = asset
            };
        }

        public DeleteTestAttendanceAssetResponse DeleteTestAttendanceAsset(DeleteTestAttendanceAssetRequest request)
        {
            var asset = NHibernateSession.Current.Get<TestSittingDocument>(request.TestAttendanceAssetId);
            if (asset != null)
            {
                if (request.PermanentDelete)
                {
                    // note that deletion of the stored file is the responsibility of the caller.
                    // this will just delete the TestAttendanceAsset row.
                    NHibernateSession.Current.Delete(asset);
                }
                else
                {
                    asset.Deleted = true;
                    NHibernateSession.Current.Save(asset);
                }
                NHibernateSession.Current.Flush();
            }
            return new DeleteTestAttendanceAssetResponse();
        }

        public CreateOrUpdateMaterialResponse CreateMaterial(CreateOrUpdateMaterialRequest request)
        {
            var material = new TestMaterialAttachment();

            UpdateAndSaveMaterial(material, request);

            return new CreateOrUpdateMaterialResponse();
        }

        public CreateOrUpdateMaterialResponse UpdateMaterial(CreateOrUpdateMaterialRequest request)
        {
            var material = NHibernateSession.Current.Get<TestMaterialAttachment>(request.MaterialId);

            if (material == null)
            {
                material = NHibernateSession.Current.Query<TestMaterialAttachment>()
                    .SingleOrDefault(x => x.StoredFile.Id == request.StoredFileId);
            }

            if (material == null)
            {
                throw new Exception("Material does not exist: " + request.MaterialId);
            }

            UpdateAndSaveMaterial(material, request);

            return new CreateOrUpdateMaterialResponse();
        }

        private void UpdateAndSaveMaterial(TestMaterialAttachment material, CreateOrUpdateMaterialRequest request)
        {
            if (material.StoredFile == null || request.StoredFileId != material.StoredFile.Id)
            {
                var storedFile = NHibernateSession.Current.Get<StoredFile>(request.StoredFileId);
                if (storedFile == null)
                {
                    throw new Exception("Referenced StoredFile does not exist: " + request.StoredFileId);
                }
                material.StoredFile = storedFile;
            }

            if (material.TestMaterial == null || request.TestMaterialId != material.TestMaterial.Id)
            {
                var testMaterial = NHibernateSession.Current.Get<TestMaterial>(request.TestMaterialId);
                if (testMaterial == null)
                {
                    throw new Exception("Referenced TestMaterial does not exist: " + request.TestMaterialId);
                }
                material.TestMaterial = testMaterial;
            }

            material.Title = request.Title;
            material.Deleted = request.Deleted;

            NHibernateSession.Current.Save(material);
            NHibernateSession.Current.Flush();
        }

        public DeleteSubmittedMaterialResponse DeleteMaterial(DeleteSubmittedMaterialRequest request)
        {
            var asset = NHibernateSession.Current.Get<TestMaterialAttachment>(request.MaterialId);
            if (asset != null)
            {
                if (request.PermanentDelete)
                {
                    // note that deletion of the stored file is the responsibility of the caller.
                    // this will just delete the TestAttendanceAsset row.
                    NHibernateSession.Current.Delete(asset);
                }
                else
                {
                    asset.Deleted = true;
                    NHibernateSession.Current.Save(asset);
                }
                NHibernateSession.Current.Flush();
            }
            return new DeleteSubmittedMaterialResponse();
        }

        private string GetListString(int[] numbers)
        {
            var returnVal = "";
            foreach (int num in numbers)
            {
                returnVal += num + ", ";
            }
            return returnVal.Trim(new char[] { ' ', ',' });
        }

        private string GetListString(string[] words)
        {
            var returnVal = "";
            foreach (string num in words)
            {
                returnVal += '\'' + num + "\', ";
            }
            return returnVal.Trim(new char[] { ' ', ',' });
        }
    }
}
