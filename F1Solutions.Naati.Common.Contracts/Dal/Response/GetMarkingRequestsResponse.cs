using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetMarkingRequestsResponse
    {
        public MarkingRequestDto[] MarkingRequests { get; set; }
    }
}