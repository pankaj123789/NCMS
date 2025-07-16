namespace Ncms.Contracts.Models.File
{
    public class DocumentTypeModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool ExaminerToolsDownload { get; set; }
        public bool ExaminerToolsUpload { get; set; }
        
        // used by client
        public string DocumentType => Name;
        // used by client
        public bool EportalDownload => ExaminerToolsDownload;

        public bool MergeDocument { get; set; }
    }
}
