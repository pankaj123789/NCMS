namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetJobExaminerRequest
    {
        public int JobExaminerId { get; set; }
        public bool IncludeExaminerMarks { get; set; }
    }
}