namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestTaskLookupTypeDto : LookupTypeDto
    {
        public int TestComponentBaseTypeId { get; set; }
        public int TestSpecificationId { get; set; }
        public bool Active { get; set; }
        public bool CandidateBriefRequired { get; set; }

        public double DefaultMaterialRequestHours { get; set; }
    }
}