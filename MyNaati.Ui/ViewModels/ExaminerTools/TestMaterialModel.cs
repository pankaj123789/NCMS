using System;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class TestMaterialModel
    {
        public int JobID { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public string Level { get; set; }
        public DateTime? DueDate { get; set; }
    }
}