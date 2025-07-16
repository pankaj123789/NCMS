using System;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class MaterialRequestRoundAttachmentModel
    {
        public int MaterialRequestRoundAttachmentId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string DocumentType { get; set; }
        public long FileSize { get; set; }
        public bool IsOwner { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }
}