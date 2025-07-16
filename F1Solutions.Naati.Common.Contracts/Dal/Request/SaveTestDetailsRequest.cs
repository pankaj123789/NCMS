using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveTestDetailsRequest
    {
        public int AttendanceId { get; set; }
        public int? JobId { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? DueDateReview { get; set; }
        public bool Sat { get; set; }
        public int TestVenueId { get; set; }
        public int TestMaterialId { get; set; }
    }
}