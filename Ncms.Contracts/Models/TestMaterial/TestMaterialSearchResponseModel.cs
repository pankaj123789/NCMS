using System;

namespace Ncms.Contracts.Models.TestMaterial
{
    public class TestMaterialSearchResponseModel
    {
        public int JobID { get; set; }
        public int? MaterialID { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
        public decimal JobCost { get; set; }
        public DateTime? ToPayroll { get; set; }
    }
}
