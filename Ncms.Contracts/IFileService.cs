using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Contracts.Models.File;

namespace Ncms.Contracts
{
    public interface IFileService
    {
        IList<FileResponseModel> List(FileSearchRequestModel request);
        IList<DocumentTypeModel> GetDocumentTypes();
        IList<DocumentTypeModel> GetDocumentTypesForCategory(int categoryId);
        IList<string> ListTypes();
        void Delete(int id);
        FileResponseModel GetData(int id);
        FileResponseModel GetTempFile(string filePath);
        CreateOrUpdateFileResponseModel Create(CreateOrUpdateFileRequestModel request);
        CreateOrUpdateFileResponseModel Update(CreateOrUpdateFileRequestModel request);
        FileInfoModel GetFile(int storedFileId);
        FileResponseModel GetMaterialRoundFile(int materialRoundId, int storedFileId);
        GenericResponse<string> GetFileToken(int storedFileId);
        IEnumerable<DocumentTypeDto> GetAllowedDocumentTypesToUpload();

    }
}
