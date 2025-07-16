using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetUnavailabilityResponse
    {
        public ExaminerUnavailabilityDto[] Unavailability { get; set; }
    }
}