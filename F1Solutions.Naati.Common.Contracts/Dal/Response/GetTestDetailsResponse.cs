namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetTestDetailsResponse
    {
        public TestSpecificationPassMarkContract OverAllPassMark { get; set; }
        public StandardTestComponentContract[] Components { get; set; }
        public TestAttendanceDocumentContract[] Attachments { get; set; }
        public string CommentsGeneral { get; set; }
        public int TestMarkingTypeId { get; set; }
        public string Feedback { get; set; }
    }
}