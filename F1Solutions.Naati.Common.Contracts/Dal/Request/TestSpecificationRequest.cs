namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestSpecificationRequest
    {
        public bool Active { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public bool ResultAutoCalculation { get; set; }
    }
}