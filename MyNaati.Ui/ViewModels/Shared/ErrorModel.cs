using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class ErrorModel
    {
        public HandleErrorInfo errorInfo { get; set; }

        public string message { get; set; }

        public string title { get; set; }
    }
}