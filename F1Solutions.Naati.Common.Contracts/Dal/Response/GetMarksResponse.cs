namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetMarksResponse
    {
        public StandardTestComponentContract[] Components { get; set; }
        public TestSpecificationPassMarkContract OverAllPassMark { get; set; }
    }
}