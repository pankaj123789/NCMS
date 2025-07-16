namespace Ncms.Contracts.Models
{
    public class GetMarksRequestModel
    {
        public int TestResultId { get; set; }
    }

    public class GetExaminerMarksRequestModel : GetMarksRequestModel
    {
        public int JobExaminerId { get; set; }
    }
}
