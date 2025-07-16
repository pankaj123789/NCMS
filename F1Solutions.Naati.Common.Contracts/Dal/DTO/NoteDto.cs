using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class NoteDto
    {
        public int NoteId { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Highlight { get; set; }
        public bool ReadOnly { get; set; }
        public string Reference { get; set; }
        public int ReferenceType { get; set; }
    }
}