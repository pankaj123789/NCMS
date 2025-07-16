using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public enum WiiseHttpExceptionResult
    {
        TooManyRequests = 429,
        BadRequest = 400,
        ServerUnreachable = 0,
        RequestTimeout = 408, 
        GatewayTimeout = 504
    }
}
