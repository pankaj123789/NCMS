using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class ExaminerUnavailableContract
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}