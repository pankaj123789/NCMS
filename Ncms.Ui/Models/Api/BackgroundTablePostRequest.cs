using System.Collections.Generic;

namespace Ncms.Ui.Models.Api
{
    public class BackgroundTablePostRequest
    {
        public int Id { get; set; }
        public List<KeyValuePair<string, string>> Data { get; set; }
    }
}