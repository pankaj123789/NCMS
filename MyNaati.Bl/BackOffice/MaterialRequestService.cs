using System.IO;
using System.Linq;
using Aspose.Words;
using F1Solutions.Naati.Common.Bl.MaterialRequest;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using MyNaati.Contracts.BackOffice.MaterialRequest;

namespace MyNaati.Bl.BackOffice
{

    public class MaterialRequestService : IMaterialRequestService
    {
        private IMaterialRequestQueryService mMaterialRequestService;
        private ITokenReplacementService mTokenReplacementService;

        public MaterialRequestService(IMaterialRequestQueryService materialRequestQueryService, ITokenReplacementService tokenReplacementService)
        {
            mMaterialRequestService = materialRequestQueryService;
            mTokenReplacementService = tokenReplacementService;
        }

        public Stream ReplaceDocumentTokens(string filePath, int materialRoundId)
        {
            var fileExtension = Path.GetExtension(filePath) ?? string.Empty;
            if (fileExtension.EndsWith(".doc") || fileExtension.EndsWith(".docx"))
            {
                var searchQuery = new TestMaterialRequestSearchRequest
                {
                    Filters = new[]
                    {
                        new TestMaterialRequestSearchCriteria()
                        {
                            Filter = TestMaterialRequestFilterType.LatestRoundIdIntList,
                            Values = new[] { materialRoundId.ToString() }
                        }
                    }
                };
                var data = mMaterialRequestService.SearchTestMaterialRequests(searchQuery).Results.First();
                var helper = new MaterialRequestDocumentHelper(mTokenReplacementService, "Aspose.Words.lic");
                try
                {
                    var result = helper.ReplaceDocumentTokens(data, filePath, saveFormat: fileExtension.EndsWith(".docx") ? SaveFormat.Docx : SaveFormat.Doc);
                    return result;
                }
                finally
                {
                    System.IO.File.Delete(filePath);
                }
            }

            try
            {
                var fileData = new MemoryStream(System.IO.File.ReadAllBytes(filePath), false)
                {
                    Position = 0
                };
                return fileData;
            }
            finally
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
