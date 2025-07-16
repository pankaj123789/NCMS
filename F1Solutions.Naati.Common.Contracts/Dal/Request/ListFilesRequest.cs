using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ListFilesRequest
    {
        public string Name { get; set; }
        public List<StoredFileType> Types { get; set; }
        public int? StoredFileId { get; set; }
        public int? RelatedId { get; set; }
        public bool IncludeDeleted { get; set; }
    }
}