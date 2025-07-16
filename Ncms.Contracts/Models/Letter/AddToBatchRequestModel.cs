namespace Ncms.Contracts.Models.Letter
{
    public class AddToBatchRequestModel
    {
        public int StandardLetterId { get; set; }
        public int LetterBatchId { get; set; }
        public int EntityId { get; set; }
        public int? TestAttendanceId { get; set; }
    }
}
