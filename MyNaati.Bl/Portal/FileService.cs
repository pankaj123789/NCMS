using System.Collections.Generic;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
    //public class FileService : IFileService
    //{
    //    private IFileRepository mFileRepository;

    //    public FileService(IFileRepository fileRepository)
    //    {
    //        mFileRepository = fileRepository;
    //    }

    //    public GetFileResponse GetFile(GetFileRequest getFileRequest)
    //    {
    //        File file = mFileRepository.GetFileByName(getFileRequest.FileName);
    //        GetFileResponse response = new GetFileResponse();
    //        if (file != null)
    //            response.FileBytes = file.FileBytes;
    //        return response;
    //    }

    //    public void CreateOrUpdateFile(CreateOrUpdateFileRequest createOrUpdateFileRequest)
    //    {
    //        mFileRepository.CreateOrUpdateFile(createOrUpdateFileRequest.FileName, createOrUpdateFileRequest.FileBytes);
    //    }

    //    public GetFileListingResponse GetFileListing()
    //    {
    //        List<string> fileNames = mFileRepository.GetFileListing();
    //        var response = new GetFileListingResponse()
    //                           {
    //                               FileNames = fileNames
    //                           };
    //        return response;
    //    }
    //}
}
