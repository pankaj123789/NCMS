namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveExaminerMarksRequest : SaveMarksRequest
    {
        public int JobExaminerId { get; set; }
        public bool IncludePreviousMarks { get; set; }
    }
}