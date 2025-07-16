using System;

namespace Ncms.Contracts.Models
{
    public class NoteModel
    {
        public int? NoteId { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Highlight { get; set; }
        public bool ReadOnly { get; set; }
        
        public string Reference { get; set; }
        public int ReferenceType { get; set; }
    }

    public class PanelNoteModel : NoteModel
    {
        public int PanelId { get; set; }
    }

    public class TestNoteModel : NoteModel
    {
        public int TestSittingId { get; set; }
    }

    public class ApplicationNoteModel : NoteModel
    {
        public int ApplicationId { get; set; }
    }

    public class MaterialRequestNoteModel : NoteModel
    {
        public int MaterialRequestId { get; set; }
    }

    public class MaterialRequestPublicNoteModel : NoteModel
    {
        public int MaterialRequestId { get; set; }
    }

    public class PersonNoteModel : NoteModel
    {
 
    }

    public class NaatiEntityNoteModel : NoteModel
    {
        public int NaatiEntityId { get; set; }
    }

    public class NoteAttachmentModel
    {
        public int NoteId { get; set; }
        public int StoredFileId { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }

    //public class AttachmentModel
    //{
    //    public string FileName { get; set; }
    //    public string FilePath { get; set; }
    //    public int NoteId { get; set; }
    //    public string StoragePath { get; set; }
    //    public int StoredFileId { get; set; }
    //    public string Title { get; set; }
    //    public string Type { get; set; }
    //    public int UploadedByUserId { get; set; }
    //}
}
