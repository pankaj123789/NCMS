using System;

namespace Ncms.Contracts.Models.Common
{
    public class AttachmentModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int NoteId { get; set; }
        public string StoragePath { get; set; }
        public int StoredFileId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }
}
