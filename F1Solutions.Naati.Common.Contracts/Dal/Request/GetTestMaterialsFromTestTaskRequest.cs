namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestMaterialsFromTestTaskRequest
    {
        public int TestComponentId { get; set; }
        public int? SkillId { get; set; }

        public bool? IncludeSystemValueSkillTypes { get; set; }
    }
}