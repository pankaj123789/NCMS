using System.Collections.Generic;
using System.ServiceModel;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetFileResponse
    {
        [MessageHeader]
        public string FileName { get; set; }
        public string[] FilePaths { get; set; }
        public IEnumerable<StoredFileType> Types { get; set; }
    }
}