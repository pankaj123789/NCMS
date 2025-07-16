namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PhoneDetailsDto
    {
        public int EntityId { get; set; }
        public int? PhoneId { get; set; }
        public string LocalNumber { get; set; }
        public string Note { get; set; }
        public bool PrimaryContact { get; set; }
        public bool IncludeInPd { get; set; }
        public bool AllowSmsNotification { get; set; }
        public bool Invalid { get; set; }
        public bool IsExaminer { get; set; }
        public bool ExaminerCorrespondence { get; set; }
    }
}