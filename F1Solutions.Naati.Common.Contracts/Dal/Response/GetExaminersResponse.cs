using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetExaminersResponse
    {
        public ExaminerDto[] Examiners { get; set; }
        public bool Extended { get; set; }
    }
}