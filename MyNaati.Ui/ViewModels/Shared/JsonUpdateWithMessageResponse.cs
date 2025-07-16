using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class JsonUpdateWithMessageResponse
    {
        public JsonUpdateWithMessageResponse()
        {
            Errors = new List<string>();
        }

        public bool Success { get; set; }
        public List<string> Errors { get; set; }

        public string Message { get; set; }

        public void AcceptErrors(ViewDataDictionary viewData)
        {
            Errors.AddRange(viewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage));
        }
    }
}