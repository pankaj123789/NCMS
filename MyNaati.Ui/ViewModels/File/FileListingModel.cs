using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.File
{
    public class FileListingModel
    {
        public List<string> FileNames { get; set; }

        public FileUploadModel FileUpload { get; set; }
    }
}