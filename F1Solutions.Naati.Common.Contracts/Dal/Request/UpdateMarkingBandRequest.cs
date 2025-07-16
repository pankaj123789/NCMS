using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateMarkingBandRequest
    {
        public int UserId { get; set; }
        public RubricMarkingBandDto RubricMarkingBand { get; set; }
    }
}
