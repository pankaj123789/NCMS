using System.Collections.Generic;

namespace Ncms.Contracts.Models.File
{
    public class FileSearchRequestModel
    {
        public string Name { get; set; }
        public List<string> Types { get; set; }
        public int? RelatedId { get; set; }
    }
}
