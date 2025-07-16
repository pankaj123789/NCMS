using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    public static class HttpStatusCodeMapper
    {
        public static bool IsStatusCodeAnError(this HttpStatusCode statusCode)
        {
            if(statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent)
            {
                return false;
            }
            return true;
        }
    }
}
