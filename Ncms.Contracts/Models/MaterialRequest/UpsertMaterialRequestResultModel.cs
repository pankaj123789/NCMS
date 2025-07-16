using System.Collections.Generic;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class UpsertMaterialRequestResultModel
    {
        public IEnumerable<int> MaterialRequestIds { get; set;  }
        public IEnumerable<int> MaterialRequestRoundIds { get; set; }
        public IEnumerable<int> TestMaterialIds { get; set; }
        public IEnumerable<int> StoredFileIds { get; set; }
    }
}
