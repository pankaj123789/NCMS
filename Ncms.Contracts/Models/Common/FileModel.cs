using System.IO;

namespace Ncms.Contracts.Models.Common
{
    public class FileModel
    {
        public Stream FileData { get; set; }
        public string FileName { get; set; }
        public FileType FileType { get; set; }

        public FileModel()
        {
            FileType = FileType.Generic;
        }
    }
}
