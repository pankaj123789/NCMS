using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestDto
    {
        public int TestMaterialID { get; set; }
        public int JobExaminerID { get; set; }
        public int JobID { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public string Level { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateReceived { get; set; }
        public decimal? Cost { get; set; }
        public bool Approved { get; set; }
    }
}