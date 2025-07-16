using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialDetailDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TypeLabel { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public int TestMaterialDomainId { get; set; }
        public string TypeDescription { get; set; }
        public int ApplicantsRangeTypeId { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public int TestMaterialTypeId { get; set; }
    }
}