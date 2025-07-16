using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class WorkPracticeDetails
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Points { get; set; }
        public int Id { get; set; }
        public bool HasAttachments { get; set; }
    }
}