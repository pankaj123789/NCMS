using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveMaterialRequestRoundLinksRequest
    {
        public IEnumerable<SaveMaterialRequestRoundLinkRequest> Links { get; set; }
        public object MaterialRequestRoundId { get; set; }
    }
}
