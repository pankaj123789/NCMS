using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PersonNoteData
    {
        public int NoteId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool Highlight { get; set; }
        public bool ReadOnly { get; set; }
        public int UserId { get; set; }
    }
}