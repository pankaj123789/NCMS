using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetDocumentsRequest
    {
        public int TestSittingId { get; set; }
        public IEnumerable<StoredFileType> DocumentTypes { get; set; }
        public bool? ExaminerDocuments { get; set; }
    }
}