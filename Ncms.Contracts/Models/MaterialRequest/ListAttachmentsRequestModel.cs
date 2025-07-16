namespace Ncms.Contracts.Models.MaterialRequest
{
    public class ListAttachmentsRequestModel
    {
        public int MaterialRequestRoundId { get; set; }
        public bool? NcmsAvailable { get; set; }
        public bool? ExaminersAvailable { get; set; }
    }
}
