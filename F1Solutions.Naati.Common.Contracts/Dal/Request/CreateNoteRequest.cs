namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateNoteRequest : NoteRequest
    {
        public int? NoteId { get; set; }
        public int UserId { get; set; }
        public string Note { get; set; }
        public bool Highlight { get; set; }
        public bool ReadOnly { get; set; }
    }
}