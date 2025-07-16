using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    /// <summary>
    /// General file storage service contract. Only concerned with putting/getting files in some
    /// (nonspecific) storage repository.
    /// </summary>
    
    public interface IFileStorageService : IQueryService
    {
        GetFileInfoResponse GetStoredFileInfo(int storedFileId);

        GetFileResponse GetFile(GetFileRequest getFileRequest);
        
        GetFileResponse GetFiles(GetFilesRequest getFilesRequest);
        
        GetFileResponse GetTestMaterialFilesByAttendee(GetFilesRequest getFilesRequest);
        
        GetAttendeesTestSpecificationTestMaterialResponse GetFileStoreTestSpecificationTestMaterialList(GetFileStoreTestSpecificationTestMaterialRequest getFileStoreTestSpecificationTestMaterialRequest);
        
        ListFilesResponse ListFiles(ListFilesRequest listFilesRequest);
        
        DocumentTypesResponse ListDocumentTypes(ListDocumentTypesRequest request);
        
        CreateOrUpdateFileResponse CreateOrUpdateFile(CreateOrUpdateFileRequest createFileRequest);
        
        CreateOrUpdateFileResponse ReplaceFile(FileMetadataRequest updateFileRequest);
        
        void DeleteFile(DeleteFileRequest deleteFileRequest);

        void MoveFile(MoveFileRequest request);
    }
}
