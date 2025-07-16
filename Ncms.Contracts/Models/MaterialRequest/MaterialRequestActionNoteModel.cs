using System;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestActionNoteModel
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
