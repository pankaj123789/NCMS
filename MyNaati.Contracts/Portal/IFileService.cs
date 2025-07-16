using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.Portal
{
    
    public interface IFileService : IInterceptableservice
    {
        
        GetFileResponse GetFile(GetFileRequest getFileRequest);

        
        void CreateOrUpdateFile(CreateOrUpdateFileRequest createOrUpdateFileRequest);

        
        GetFileListingResponse GetFileListing();
    }

    
    public class GetFileRequest
    {
        
        public string FileName { get; set; }
    }

    
    public class GetFileResponse
    {
        
        public byte[] FileBytes { get; set; }
    }

    
    public class CreateOrUpdateFileRequest
    {
        
        public string FileName { get; set; }

        
        public byte[] FileBytes { get; set; }
    }

    
    public class GetFileListingResponse
    {
        
        public List<string> FileNames { get; set; }
    }
}
