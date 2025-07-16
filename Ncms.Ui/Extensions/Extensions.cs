using System.Collections.Generic;
using System.Net.Http.Headers;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;

namespace Ncms.Ui.Extensions
{
    public static class Extensions
    {
        public static MediaTypeHeaderValue GetHeaderValue(this FileType fileType)
        {
            return new MediaTypeHeaderValue(fileType.MediaType);
        }

     
    }
}
