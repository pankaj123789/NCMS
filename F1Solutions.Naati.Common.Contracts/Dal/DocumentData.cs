using System.IO;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class DocumentData
    {
        public string FileName => Path.GetFileName(FilePath);
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string DocumentNumber { get; set; }
        [LookupDisplay(LookupType.DocumentType)]
        public StoredFileType StoredFileType { get; set; }
        public string DocumentTypeName { get; set; }
    }
}