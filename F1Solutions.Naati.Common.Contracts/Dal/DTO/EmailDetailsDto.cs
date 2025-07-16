namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EmailDetailsDto
    {
        public int EntityId { get; set; }
        public int? EmailId { get; set; }
        public bool IsPreferredEmail { get; set; }
        public bool IncludeInPd { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public bool Invalid { get; set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool IsExaminer { get; set; }
    }
}