namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetMaterialRequestRoundAttachmentsRequest
    {
        public int MaterialRequestRoundId { get; set; }
        public int? NAATINumber { get; set; }
        public int? PersonId { get; set; }
        public bool? NcmsAvailable { get; set; }
        public bool? ExaminerAvailable { get; set; }
    }
}
