using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MarkingRequestDto
    {
        public int AttendanceId { get; set; }
        public int NaatiNumber { get; set; }
        public string ApplicantName { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public string Direction { get; set; }
        public string Category { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime TestDate { get; set; }
    }
}