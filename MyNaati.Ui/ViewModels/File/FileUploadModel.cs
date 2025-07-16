using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MyNaati.Ui.ViewModels.File
{
    public class FileUploadModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }

        [Required]
        public string FileName { get; set; }
    }
}