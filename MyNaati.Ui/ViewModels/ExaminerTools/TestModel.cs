using System;
using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class TestModel
    {
        public int JobID { get; set; }
        public int JobExaminerID { get; set; }
        public int TestMaterialID { get; set; }
        public int TestResultID { get; set; }
        public int TestSittingId { get; set; }
        public bool Supplementary { get; set; }
        public string SkillDisplayName { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public string Level { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int MaterialID { get; set; }
        public string Status { get; set; }
        public List<MaterialModel> Materials { get; set; }
        public bool HasAssets { get; set; }

        public int TestMarkingTypeId { get; set; }

        public bool HasDefaultSpecification { get; set; }


    }
}
