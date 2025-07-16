namespace Ncms.Contracts.Models.Application
{
    public class TestSpecificationInfoModel
    {
        public int TestSpecificationId { get; set; }
        public int MarkingSchemaTypeId { get; set; }

        public bool AutomaticIssuing { get; set; }
        public double? MaxScoreDifference { get; set; }
    }
}
