using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPersonPhotoFileRequest
    {
        public IEnumerable<int> PersonNaatiNumbers { get; set; }
        public string  FolderPath { get; set; }
    }
}
