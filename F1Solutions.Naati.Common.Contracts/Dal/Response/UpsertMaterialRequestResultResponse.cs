using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class UpsertMaterialRequestResultResponse
    {
        public IEnumerable<int> MaterialRequestIds { get; set; }
        public IEnumerable<int> MaterialRequestRoundIds { get; set; }
        public IEnumerable<int> TestMaterialIds { get; set; }
        public IEnumerable<int> StoredFileIds { get; set; }
    }
}
