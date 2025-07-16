namespace Ncms.Contracts.Models.MaterialRequest.Wizard
{
    public class TestMaterialStepModel
    {
        public int PanelId { get; set; }
        public string Title { get; set; }

        public int TaskTypeId { get; set; }
        public int? LanguageId { get; set; }
        public int? SkillId { get; set; }
        public int TestMaterialTypeId { get; set; }
        public int TestMaterialDomainId { get; set; }
        public int ProductSpecificationId { get; set; }

        public double MaxBillableHours { get; set; }

    }
}
