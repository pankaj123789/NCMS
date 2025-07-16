using System;
using System.Collections.Generic;

using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Bl
{
    public interface IFileCompressionHelper
    {
        string CreateZipFile(IEnumerable<int> storageFileIds, string zipFileName);
        string CreateZipFile(IEnumerable<string> storageFilePaths, string zipFileName, bool deleteFiles = true);
        string CreateZipFile(IEnumerable<CompressibleFile> storageFilePaths, string zipFileName, bool deleteFiles = true);
    }

    public class CompressibleFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}